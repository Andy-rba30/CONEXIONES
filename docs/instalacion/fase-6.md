# Instalación y prueba de la Fase 6: ventana de previsualización 2D, borrado desde la cinta y pestaña ARBA

Objetivo: desplegar el add-in de la Fase 6 y comprobar en Revit lo que la nube no puede probar: que el panel
**MotorConexiones** aparece en la pestaña **ARBA**, que el botón **Ejecutar especificación JSON** abre la ventana con el
croquis acotado y la tabla editable, que corregir un valor redibuja y revalida, que **Guardar JSON** no toca el original,
que un valor inválido desactiva **Crear**, que **Crear** modela la conexión del Detalle D y que el botón **Conexiones del
modelo** la borra dejando las barras como estaban. Una ronda, unos 30 minutos, sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Vale Windows PowerShell
5.1 o PowerShell 7; todos los comandos van en la misma ventana, en orden. **Revit lo abre y lo cierra la persona**, no el
instalador. Los pasos marcados **(la persona)** los hace la persona en Revit; los marcados **(instalador)** van en
PowerShell. **Mientras una ventana del add-in esté abierta en Revit, el instalador no ejecuta nada** (`revit-exec.ps1` y
`conn-call.ps1` esperan a que Revit quede libre y darían tiempo agotado): espera a que la persona la cierre.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 6-2 (`deploy.ps1` no puede sustituir la DLL si Revit la tiene cargada).
- La copia `HANGAR_PRUEBA_sondeo.rvt` existe desde la Fase 1 y no tiene conexiones del add-in (la ronda 5b terminó con
  `conexiones tras borrar: 0`). Si no existiera: abre el original y ejecuta
  `.\scripts\revit-exec.ps1 -File scripts\sondeos\08-guardar-copia.py -SinTransaccion`.
- Ten a mano el plano `docs\fixtures\detalle-D.png` para comparar las cotas en el paso 6-4.
- El puente MCP (ventana de `iniciar_servidor_revit.bat`) puede estar cerrado: el paso 6-9 lo arranca.

### 6-1. PowerShell, rama y archivo de resultados **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-6.md"
"# Resultados de la Fase 6`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "6-1 git" { git log -1 --oneline }
```

La última línea debe ser un commit "Fase 6: ...". Si no, repite el `git pull` y anótalo.

### 6-2. Compilar, pasar las pruebas, Revit cerrado y desplegar **(instalador)**

```powershell
Anota "6-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "6-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 83`, `6-2 revit cerrado` vacío y
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==`. Si `6-2 revit cerrado` no está vacío, pide a la persona que
cierre Revit y repite el `deploy`. Si el build falla, **para aquí** y devuelve la salida.

### 6-3. Abrir Revit y mirar la cinta

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`. Si Revit pregunta por el
   add-in sin firmar, pulsa *Always Load*. Pulsa la pestaña **ARBA**: debe haber un panel **MotorConexiones** con dos
   botones, **Ejecutar especificación JSON** y **Conexiones del modelo**. Captura de la cinta con la pestaña ARBA a la
   vista (`Windows + Mayús + S`), guardada como `docs\fases\capturas\fase6-01-cinta.png`. Si el panel no está en ARBA,
   mira la pestaña **Conexiones** y anota en el chat dónde apareció (o si no apareció).
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "6-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "6-3 sondeo 15 cinta" { .\scripts\revit-exec.ps1 -File scripts\sondeos\15-cinta-arba.py -SinTransaccion }
   Anota "6-3 log arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "ribbon_panel_created|\"startup\"|startup_failed" | Select-Object -Last 6 }
   ```

   Se espera `ok: true` con `document.title: HANGAR_PRUEBA_sondeo`; en el sondeo 15, `pestana ARBA existe: True` y
   `panel MotorConexiones encontrado en: ARBA`, con los dos `elemento id='...MotorConexiones_RunSpec'` y
   `'...MotorConexiones_ListConnections'`; en el log, una línea `ribbon_panel_created` con `"tab":"ARBA"`. Si dice
   `"tab":"Conexiones"`, copia también `preferred_tab_error`.

### 6-4. Abrir la ventana con el Detalle D y comparar las cotas **(la persona)**

1. Pestaña ARBA > **Ejecutar especificación JSON** > elige `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
2. Se abre la ventana **MotorConexiones: previsualización de conexión**. Abajo debe decir en verde
   `Validación correcta con 2 aviso(s)` (los dos `ANGLE_DIFFERS_FROM_MODEL` de siempre) y, a la izquierda de los
   botones, `validation_token: ` seguido de 16 caracteres. **Anota en el chat esos 16 caracteres.**
3. Pulsa **Ajustar** si el croquis no se ve entero. Captura de la ventana completa:
   `docs\fases\capturas\fase6-02-ventana.png`.
4. Compara el croquis con el plano `docs\fixtures\detalle-D.png` y anota en el chat las diferencias que veas. Lo que
   debe leerse en las cotas: cartela `565,0` y `530,0`; retiros `180,0`, `60,0` y `260,0`; ranuras `150,0` (dos);
   placa cuchilla `170,0` y `140,0`; pernos `60,0` (paso), `40,0` (borde) y `40,0` (primera fila); etiqueta
   `cartela PL 3/8" · 9,5 mm`; soldaduras en rojo con `soldadura 5,0 mm todo el contorno`. Si el dibujo parece boca
   abajo respecto al plano, es normal: está en el sistema local del nudo (la leyenda de abajo dice hacia dónde apunta
   +Y en el modelo); anótalo de todas formas.
5. Prueba la rueda (zoom) y el botón central o arrastrar (encuadre) y anota si responden.

**Deja la ventana abierta** para el paso siguiente.

### 6-5. Cambiar el espesor de la cartela y guardar el JSON

1. **(la persona)** En la tabla de la derecha, sección **Cartela**, haz doble clic en el valor de **Espesor (mm)**
   (`9.525`), escribe `12,7` y pulsa Enter. Debe pasar lo siguiente: la etiqueta del croquis cambia a
   `cartela PL 3/8" · 12,7 mm`, la ranura de las barras se hace más ancha, abajo aparece el error
   `LABEL_VALUE_MISMATCH` en `gusset.thickness_label` (el rótulo `3/8"` ya no coincide con 12,7 mm), el token pasa a
   `(sin token: corrige los errores)` y **Crear** se desactiva. Anota en el chat si fue así.
2. **(la persona)** Doble clic en **Rótulo de espesor** (`3/8"`), escribe `1/2"` y Enter. La validación vuelve a verde y
   el `validation_token` es **otro** (compara los 16 caracteres con los del paso 6-4 y anótalos). Captura de la ventana:
   `docs\fases\capturas\fase6-03-espesor-12-7.png`.
3. **(la persona)** Pulsa **Guardar JSON**. Abajo debe decir `JSON guardado en ...\detalle-D-confirmado-corregido.json
   (el original no se ha tocado)`. **Deja la ventana abierta.**
4. **(instalador)** Comprueba el archivo nuevo y que el original no cambió (esto no toca Revit, se puede ejecutar con la
   ventana abierta):

   ```powershell
   Anota "6-5 archivo corregido" { Get-ChildItem docs\fixtures\detalle-D-confirmado*.json | Select-Object Name, Length, LastWriteTime; git status --short docs\fixtures; Select-String -Path docs\fixtures\detalle-D-confirmado-corregido.json -Pattern "thickness_mm|thickness_label" }
   ```

   Se espera: los dos archivos, `git status` con una sola línea `?? docs/fixtures/detalle-D-confirmado-corregido.json`
   (el original sin `M`), y en el corregido `"thickness_mm": 12.7` y `"thickness_label": "1/2\""`.

### 6-6. Un valor inválido desactiva Crear, y Recargar vuelve al original **(la persona)**

1. En la sección **Barra 3 · diagonal 1249636**, doble clic en **Pernos: paso (mm)** (`60`), escribe `10` y Enter. Debe
   aparecer el error `BOLT_SPACING_TOO_SMALL` con el campo `members[2].attachment.bolts.spacing_mm` y una sugerencia con
   el mínimo en mm; **Crear** queda desactivado; en el croquis los cuatro pernos se juntan. Captura:
   `docs\fases\capturas\fase6-04-error-paso.png`.
2. Pulsa **Recargar**. Abajo debe decir `Archivo recargado del disco`, la tabla vuelve a `9.525`, `3/8"` y `60`, la
   validación vuelve a verde y el token es el mismo del paso 6-4 (anótalo).

### 6-7. Crear la conexión desde la ventana

1. **(la persona)** Con la validación en verde, pulsa **Crear**. Puede tardar hasta un minuto (Advance Steel). Debe
   salir el diálogo `Conexión modelada correctamente en el modelo` con `ID de conexión`, `Elementos geométricos
   creados: 9` y `Backend de fabricación utilizado: advancesteel`. **Anota en el chat el ID de conexión.** Cierra el
   diálogo. Si en vez de eso sale `Error en Modelado`, copia el texto entero.
2. **(la persona)** En la vista 3D, nivel de detalle **Fino**, estilo **Sombreado**, zoom al nudo: cartela, placa
   cuchilla con 4 pernos y barras acortadas, como en la Fase 5. Captura: `docs\fases\capturas\fase6-05-nudo.png`.
3. **(instalador)** Con todas las ventanas del add-in cerradas:

   ```powershell
   Anota "6-7 conn_list" { .\scripts\conn-call.ps1 -Operation list }
   Anota "6-7 captura exportada" { .\scripts\revit-exec.ps1 -File scripts\sondeos\capturar-nudo.py -SinTransaccion }
   Rename-Item "docs\fases\capturas\fase3-captura.png" "fase6-07-nudo-exportada.png" -Force
   Anota "6-7 log crear" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "ribbon_preview_opened|ribbon_preview_edit|ribbon_preview_saved|ribbon_create" | Select-Object -Last 12 }
   ```

   Se espera `connections_count: 1`, y en el log una línea `ribbon_create` con `created_elements: 9` y
   `validation_token_prefix` igual a los 16 caracteres del paso 6-4.

### 6-8. Borrar desde la cinta

1. **(la persona)** Pestaña ARBA > **Conexiones del modelo**. La ventana lista una conexión (id, tipo, fecha, 9
   elementos, 3 barras modificadas, `Detalle D: cordón 1249510, diagonal 1249630, ...`). Captura:
   `docs\fases\capturas\fase6-06-conexiones-modelo.png`. Selecciona la fila, pulsa **Borrar seleccionada** y responde
   **Sí**. Abajo debe decir `Borrada ...: 9 elemento(s) eliminados y 3 barra(s) restauradas en N ms` y la lista queda
   vacía. Pulsa **Cerrar**. Anota en el chat lo que dijo.
2. **(instalador)** Con la ventana cerrada:

   ```powershell
   Anota "6-8 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6-8 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones en el modelo: 0` (ya la borró la ventana), `conexiones tras borrar: 0`, las extensiones
   `1249630: inicio 0.0 | fin 68.64`, `1249631: inicio 0.0 | fin 69.2`, `1249636: inicio 0.0 | fin 0.0`, y en el sondeo 13
   `Elementos de acero sueltos encontrados: 0`. Si el sondeo 12 dice `conexiones en el modelo: 1`, la ventana no borró:
   el sondeo la borra él y hay que anotarlo.

### 6-9. La IA no se ve afectada: 19/19 por el puente **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 15
Anota "6-9 puente en 8000" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "6-9 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 19/19 pruebas correctas`. Si alguna da `[FALLO]`, copia su bloque entero. La ventana del puente se
puede cerrar después.

### 6-10. Registro, cerrar y subir (autorizado)

```powershell
Anota "6-10 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "ribbon_|startup" | Select-Object -Last 40 }
```

1. **(la persona)** Cierra la copia en Revit **sin guardar** (Archivo > Cerrar > *No guardar*).
2. **(instalador)** Sube resultados y capturas. El archivo `detalle-D-confirmado-corregido.json` **no se sube** (es la
   prueba del paso 6-5; bórralo o déjalo sin añadir):

   ```powershell
   Remove-Item docs\fixtures\detalle-D-confirmado-corregido.json -ErrorAction SilentlyContinue
   git add docs\fases\resultados-fase-6.md docs\fases\capturas
   git commit -m "Fase 6: resultados del instalador (ventana de previsualización, borrado desde la cinta, pestaña ARBA)"
   git pull --no-rebase origin main
   git push origin main
   ```

3. Devuelve: la salida completa de los pasos 6-2, 6-3, 6-5, 6-7, 6-8 y 6-9; las anotaciones de la persona (dónde
   apareció el panel, los 16 caracteres del token en 6-4, 6-5 y 6-6, las diferencias de cotas con el plano, el ID de
   conexión, qué dijo la ventana al borrar); las siete capturas; el texto de cualquier ventana de error del add-in o
   de Revit que haya aparecido, y si Revit se cerró de golpe en algún paso (en cuál y qué fue lo último que se vio).
