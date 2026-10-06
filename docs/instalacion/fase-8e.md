# Instalación y prueba de la ronda 8e: add-in 0.8.5, pinchar con la ventana abierta sin que Revit se cierre

Objetivo: desplegar el add-in **0.8.5** (cierre de la ronda 8d, `docs/fases/fase-8.md`, sección 11) y comprobar sobre la
cercha de la 8c/8d (64 elementos: el cordón central en ocho tramos, sus 48 diagonales y los ocho tramos del cordón superior)
cuatro cosas: (1) **Cordón…**, **Barras…** y **Añadir nudo…** con la ventana del plan abierta **ya no cierran Revit** (en la
8d, Cordón… sobre N9 cerró Revit con "fatal error"; la causa era el diálogo de elección) y **pinchan con la ventana oculta**;
(2) **Más… > Descartar plan cierra la ventana**; (3) **Ver en Revit** mirado a propósito; (4) el sondeo 19 **v3** (etiqueta
BMP de 24 bits de 32×32, ruta sin tildes, `SetVisibility`, refresco, dos etiquetas). Unos 25 minutos sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta ronda no crea ninguna conexión.**

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell.
Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 8e-1. **Revit lo
abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en
PowerShell. La ventana del plan puede quedarse abierta mientras el instalador llama al puente; durante un pinchado (paso 8e-3)
el instalador no ejecuta nada. La variable del plan se llama **`$plan`**. **Si Revit vuelve a cerrarse** en cualquier paso:
anota en qué botón, haz una captura del cuadro, vuelve a abrir Revit y la copia, y sigue con el paso siguiente; el log del
paso 8e-7 dirá dónde se quedó (`ribbon_batch_pick` sin `ribbon_batch_picked` después).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 8e-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la 8d terminó con `remaining_markers: 0`).
- Las plantillas del catálogo devueltas (la 8d terminó con `templates_count: 1`).

### 8e-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-8e.md"
"# Resultados de la ronda 8e`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "8e-1 git" { git log -1 --oneline; git status --short }
Anota "8e-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "8e-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "8e-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "8e-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "8e-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Cierre de la ronda 8d: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 177`**, `8e-1 revit cerrado`
vacío, `== MotorConexiones 0.8.5.0 desplegado en Revit 2027 ==`, `copiado revit_mcp\conexiones.py (23 rutas @api.route)`,
`copiado tools\conn_tools.py (21 herramientas @mcp.tool)` y `0.8.5.0` en la versión de la DLL. Si el build falla o sale
`1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 8e-2. Abrir Revit, ping y sondeo 17

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de siempre, donde se ve la cercha entera. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "8e-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "8e-2 sondeo 17 marcas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `ping`: `addin_version: 0.8.5` (en `data` y en `meta`) y 21 operaciones. En el sondeo 17, los diez pasos
   con `2) Marcadores de plan … : 0`.

### 8e-3. Pinchar con la ventana abierta, Ver en Revit y lo que la 8d no llegó a probar **(la persona; el instalador solo mira)**

1. Selecciona la cercha (64 elementos) y pulsa ARBA > **Planificar lote**. Debe salir la cabecera de siempre ("Se crearán 16
   conexiones con Nudo tipico Detalle D…").
2. **Cordón… sobre N9** (el nudo que cerró Revit en la 8d): en la tabla elige **N9** (`✖ Falta el cordón`), clic derecho >
   **Cordón…**. **Revit tiene que seguir vivo** y debe abrirse el diálogo "MotorConexiones: cordón de N9" con la lista de
   barras y la fila "1245530 (cordón actual) HSS12X8X1/2" **ya marcada**. Pulsa **Pinchar en Revit…**: la ventana del plan
   **desaparece**, Revit queda delante y en su barra de estado pide "Pincha el cordón del nudo N9 (Esc para cancelar)".
   **Pulsa Esc**: la ventana del plan vuelve sola con "Sin cambios (elección cancelada)." y los botones encendidos. Anota si
   la ventana volvió delante o se quedó detrás de Revit.
3. Repite clic derecho > **Cordón…** > **Pinchar en Revit…** y esta vez **pincha uno de los dos tramos del cordón superior**
   que llegan a N9: la ventana vuelve, replanifica y N9 pasa a `✖ Sin plantilla que encaje` (un nudo de dos diagonales: el
   Detalle D no es su típica, `fase-8.md` 10.2). Anota el texto de la barra de estado.
4. **Cordón… sobre un listo**: clic derecho sobre **N4** (`▲ Listo con aviso` o `● Listo`) > **Cordón…** > elige de la lista
   la misma barra del cordón actual > Aceptar: replanifica sin cambios. (En la 8d esto también habría cerrado Revit.)
5. **Barras…**: clic derecho sobre N9 > **Barras…** > **Pinchar en Revit…** > pulsa Esc: "Sin cambios (elección cancelada)."
   Repite y pincha una diagonal cualquiera cerca de N9 y **Finalizar**: replanifica. **Más… > Añadir nudo…** > Esc: "Sin
   cambios (elección cancelada)." **Plantilla…** sobre N4: se abre con "(automática…)" marcada; Cancelar.
6. **Ver en Revit, mirado a propósito**: elige N4 y pulsa **Ver en Revit**: Revit hace zoom al nudo, deja seleccionadas sus
   barras y su marcador, **no sale ningún cuadro** (ni de Revit ni del add-in) y la ventana sigue abierta; la barra de estado
   dice "Nudo N4 encuadrado en la vista {3D} (… elementos seleccionados)…". Haz lo mismo con **doble clic** en el círculo 4
   del mapa. Anota literalmente si salió algún cuadro.
7. **Lo que la 8d no llegó a probar**: **Excluir** por clic derecho sobre N4 (gris en el modelo) e **Incluir**;
   **Replanificar** (la barra de estado pasa un instante por "⏳ Replanificando en Revit…" y los botones vuelven);
   **Editar nudo** sobre N4 (se abre la previsualización, modal; Cancelar); y **Planificar lote** con la ventana abierta y
   solo las cuatro barras del Detalle D seleccionadas (la misma ventana se actualiza con 1 nudo listo; sin selección, solo
   viene delante). Vuelve a seleccionar la cercha entera y **Planificar lote**.
8. Captura de la ventana con el diálogo de Cordón… abierto sobre N9: `docs\fases\capturas\fase8e-02-cordon-n9.png`.

### 8e-4. Descartar cierra la ventana **(la persona, después el instalador)**

1. **(la persona)** Con la ventana abierta, **Más… > Descartar plan** > Sí. **La ventana se cierra sola** y en el modelo no
   queda ningún cubo ni rombo, ni color en las barras. Anota si se cerró y cuánto tardó.
2. **(instalador)**:

   ```powershell
   Anota "8e-4 sondeo 17 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "8e-4 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
   ```

   Se espera `2) Marcadores de plan … : 0`, `removed_markers: 0` y `remaining_markers: 0`.

### 8e-5. Sondeo 19 v3: dos etiquetas **(instalador y la persona)**

1. **(la persona)** Ponte en la vista 3D con el Detalle D de la Fase 3 (Y ≈ −17 196) bien visible, a media distancia (no la
   cercha entera desde lejos). Ninguna ventana del add-in abierta.
2. **(instalador)**:

   ```powershell
   Anota "8e-5 sondeo 19 etiquetas v3" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera: `1)` vista 3D, si la caja de sección está activa y **si el punto de N4 cae dentro**; `2)` los miembros de
   `TemporaryGraphicsManager` (debe aparecer `SetVisibility`); `3)` el servicio y `UI.ITemporaryGraphicsHandler existe: True`;
   **`4) BMP A: C:\IA\MotorConexiones-sondeo19\etiqueta-A.bmp | 32x32, 24 bits, 3126 bytes (esperado 3126) | … | ruta sin
   tildes ni espacios: True`** y lo mismo para B; `5)` las dos posiciones en pies y en mm; `6)` **dos** controles añadidos
   (índices 0 y 1), `SetVisibility(…, True) llamado`, `SetTooltip`, `GetAll(): 2 control(es)`; `7)` `RefreshActiveView()` y
   `UpdateAllOpenViews()` llamados; `8)` la captura exportada; `9)` manejador registrado; `10)` las etiquetas se quedan.
   **Copia el bloque entero** aunque alguna línea diga "fallo".
3. **(la persona)** Mira la vista: debe verse un círculo **verde con un 4** en el nudo del Detalle D y un círculo **azul con
   una B** en el centro de la caja de sección (o del modelo). Orbita un poco. Anota **cuál de las dos se ve** (ninguna, solo
   la B, las dos) y haz una **captura a mano** con el nombre `docs\fases\capturas\fase8e-01-etiqueta.png`. Si se ve alguna,
   **píncha**la: debe salir un cuadro "MotorConexiones - sondeo 19: Clic recibido…" (ciérralo).
4. **(instalador)**:

   ```powershell
   Anota "8e-5 sondeo 19b quitar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera `1)` los clics anotados (o "ningún clic anotado…"), `2) GetAll() antes de quitar: 2`, `3) Control 0 quitado` y
   `Control 1 quitado`, `4) … GetAll() despues de quitar: 0`, `5)` servidor desactivado y `6) … : 0`. No bloquea.

### 8e-6. Restos **(instalador, ventanas cerradas)**

```powershell
Anota "8e-6 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "8e-6 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "8e-6 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera `Marcadores de plan … : 0`, `conexiones en el modelo: 0` y `Elementos de acero sueltos encontrados: 0`.

### 8e-7. Puente y log, anotados en el archivo **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 30
Anota "8e-7 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8e-7 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|plan_event|startup" | Select-Object -Last 80 }
Select-String -Path $salida -Pattern "^## 8e-7" | Select-Object LineNumber, Line
```

Se espera **`Resultado: 28/28 pruebas correctas`** (la 27 necesita el servidor arrancado: si falla sola, repite el `Anota`
del puente pasados otros 30 s). En el log, `"addin_version":"0.8.5"`, **`ribbon_batch_pick`** (con `window_hidden: true` y
`revit_activated`) seguido de **`ribbon_batch_picked`** (con el id pinchado o "cancelado") por cada pinchado del paso 8e-3,
`ribbon_batch_discard` con `closes_window: true` seguido de `"action":"closed","discarded":true`, y **ninguna** línea
`ribbon_batch_window_error`, `ribbon_batch_pick_failed` ni `plan_event_failed` (si las hay, cópialas enteras: dicen qué
falló). **La última línea debe listar dos secciones `## 8e-7`**; si falta alguna, repite su `Anota`.

### 8e-8. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "8e-8 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-8e.md docs\fases\capturas
   git commit -m "Ronda 8e: resultados del instalador (0.8.5, pinchar con la ventana oculta, Descartar cierra la ventana, sondeo 19 v3)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 8e-1, 8e-2 y 8e-4 a 8e-7; el `git status --short` literal (antes del commit y
   al final, que debe quedar vacío); las anotaciones de la persona del paso 8e-3 (si Revit siguió vivo con Cordón… sobre N9,
   si la ventana se ocultó al pinchar y volvió delante, el texto de la barra de estado tras pinchar el tramo, si Ver en Revit
   sacó algún cuadro, qué pasó con Excluir, Replanificar, Editar nudo y Planificar lote con la ventana abierta), del 8e-4 (si
   la ventana se cerró sola al descartar) y del 8e-5 (cuál de las dos etiquetas se vio y si salió el cuadro del clic); las
   dos capturas; y el texto de cualquier ventana de error. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
