# Fase 6: ventana de previsualización 2D, borrado desde la cinta y panel en ARBA (instrucciones para el agente instalador)

Objetivo: desplegar el add-in 0.2.0 y comprobar en Revit lo que la nube no puede probar: que los dos botones aparecen en
el panel **Conexiones** de la pestaña **ARBA**, que la ventana de previsualización dibuja el Detalle D con sus cotas,
que editar un valor en la tabla redibuja y revalida, que **Guardar JSON** escribe la copia `-corregido.json` sin tocar el
original, que un valor inválido desactiva **Crear**, que **Crear** modela la conexión, que el botón **Conexiones del
modelo** la borra y restaura las barras, y que la IA (`probar_conexiones.py --puente`) no se ve afectada. Una ronda, unos
30 minutos, sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. Los archivos
del MCP (`mcp/`) no cambian en esta fase: no hace falta `instalar-conn.ps1`.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Vale Windows PowerShell
5.1 o PowerShell 7; todos los comandos van en la misma ventana, en orden. **Revit lo abre y lo cierra la persona**, no el
instalador. Las capturas las hace la persona con `Windows + Mayús + S` y las guarda con el nombre indicado en
`docs\fases\capturas\`.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 6-2 (`deploy.ps1` no puede sustituir la DLL si Revit la tiene cargada).
- El puente MCP (ventana de `iniciar_servidor_revit.bat`) puede estar cerrado: el paso 6-9 lo arranca.
- Tener a mano el plano del Detalle D (`docs\fixtures\detalle-D.png`) para comparar las cotas en el paso 6-4.

### 6-1. PowerShell, rama y archivo de resultados

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin claude/laughing-pascal-tsxvkt
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

### 6-2. Compilar, pasar las pruebas, Revit cerrado y desplegar

```powershell
Anota "6-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
Anota "6-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 114`, `6-2 revit cerrado` vacío y
`== MotorConexiones 0.2.0.0 desplegado en Revit 2027 ==`. Si el build falla, **para aquí** y devuelve la salida.

### 6-3. Abrir Revit: dónde quedó el botón

**La persona abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si Revit
pregunta por el add-in sin firmar, pulsa *Always Load*. Espera a que cargue pyRevit (unos 20 s).

1. **La persona** mira la cinta: en la pestaña **ARBA** debe haber un panel **Conexiones** con dos botones grandes,
   **Ejecutar especificación JSON** y **Conexiones del modelo**, junto a los paneles Acero, Metrados, Encofrado,
   Georeferenciación e IA, y **no** debe existir una pestaña **Conexiones** aparte. Captura de la cinta con la pestaña
   ARBA abierta: `docs\fases\capturas\fase6-01-cinta.png`. Anota en el chat "ARBA" o "Conexiones" según dónde estén.
2. **El instalador** lo comprueba por la API y por el log:

   ```powershell
   Anota "6-3 sondeo 15 cinta" { .\scripts\revit-exec.ps1 -File scripts\sondeos\15-cinta-arba.py -SinTransaccion }
   Anota "6-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "6-3 log arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String '"startup"|ribbon_arba' | Select-Object -Last 6 }
   ```

   Se espera: en el sondeo 15, la pestaña `ARBA` con un panel de título `Conexiones` y dos botones
   `Ejecutar / especificacion JSON` y `Conexiones / del modelo`; en `ping`, `ok: true`, `addin_version: 0.2.0` y
   `backend: advancesteel`; en el log, una línea `ribbon_arba` con `"tab":"ARBA"` y otra `startup` con
   `"ribbon_tab":"ARBA"`, `"ribbon_panel":"Conexiones"` y `"arba_error":null`. Si aparece `ribbon_arba_failed`, copia la
   línea entera: el botón habrá caído a la pestaña de reserva **Conexiones**.

### 6-4. Botón "Ejecutar especificación JSON": croquis y tabla (lo mira la persona)

1. Selecciona en Revit las 4 barras del nudo del Detalle D (IDs `1249510`, `1249630`, `1249631`, `1249636`; Gestionar >
   Seleccionar por ID). No es imprescindible (el JSON ya trae los IDs), pero deja el nudo a la vista.
2. Pulsa **ARBA > Conexiones > Ejecutar especificación JSON** y elige
   `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
3. Debe abrirse la ventana **MotorConexiones · Previsualización de conexión**: croquis a la izquierda (cordón
   horizontal, tres barras retiradas, cartela octogonal azul, placa cuchilla con 4 pernos, marcas rojas de soldadura y
   cotas verdes), tabla a la derecha (Origen, Cordón, Cartela, Contorno de la cartela, Barra 1 a 3, Cadenas de cotas,
   Dudas) y abajo "Validación: 0 errores, 2 avisos · token …" con **Crear** habilitado. Prueba la rueda (zoom), el botón
   central (encuadre) y **Ajustar**. Captura de la ventana entera: `docs\fases\capturas\fase6-02-ventana.png`.
4. Compara con el plano y anota en el chat las diferencias. Cotas que deben leerse en el croquis: cartela `565,0` y
   `530,0`; etiqueta `PL 3/8" (9,5 mm)`; retiros `retiro 180,0`, `retiro 60,0`, `retiro 260,0`; ranuras `ranura 150,0`
   (dos); placa cuchilla `PL10 (10,0 mm)`, `placa 170,0`, `placa 140,0`, `inserción 80,0`; cadena de pernos a lo largo
   `40,0 / 60,0 / 70,0` y transversal `40,0 / 60,0 / 40,0`; `4 Ø5/8" (15,9 mm)`. Anota también **hacia dónde salen las
   barras respecto a la cartela** (si la diagonal soldada sale hacia el borde ancho de la cartela o hacia el lado
   opuesto) y si el aviso naranja bajo el croquis dice que el eje Y local apunta hacia abajo.

### 6-5. Editar el espesor de la cartela y guardar el JSON corregido

1. En la tabla, grupo **Cartela**, fila **Espesor (mm)**: cambia `9,525` por `12,7` y pulsa Intro. Debe pasar: la
   etiqueta del croquis cambia a `PL 1/2" (12,7 mm)`, la fila **Etiqueta del espesor** pasa sola a `1/2"` (la línea de
   estado lo explica), el token de la cabecera "Validación" cambia y sigue "0 errores" con **Crear** habilitado. Captura:
   `docs\fases\capturas\fase6-03-espesor-12-7.png`. Anota en el chat el token abreviado de antes y el de después.
2. Pulsa **Guardar JSON**. La línea de estado debe decir `Guardado en D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado-corregido.json`.
3. **El instalador** comprueba que el original no cambió y que existe la copia:

   ```powershell
   Anota "6-5 json corregido" { Get-ChildItem docs\fixtures\detalle-D-confirmado*.json | Select-Object Name, Length, LastWriteTime; git status --short; git diff --stat -- docs\fixtures\detalle-D-confirmado.json; Select-String -Path docs\fixtures\detalle-D-confirmado-corregido.json -Pattern 'thickness' }
   ```

   Se espera: dos archivos (`detalle-D-confirmado.json` y `detalle-D-confirmado-corregido.json`); en `git status` solo
   `?? docs/fixtures/detalle-D-confirmado-corregido.json` y `docs/fases/resultados-fase-6.md`; `git diff --stat` del
   original vacío; en el corregido `"thickness_mm": 12.7` y `"thickness_label": "1/2\""`.

### 6-6. Un valor inválido desactiva Crear

1. En la tabla, grupo **Barra 3 · diagonal 1249636**, fila **Pernos: paso (mm)**: cambia `60` por `10` y pulsa Intro.
2. Debe aparecer en la lista de abajo un error `BOLT_SPACING_TOO_SMALL` con campo
   `members[2].attachment.bolts.spacing_mm` y su sugerencia, el croquis debe mostrar los pernos juntos, la cabecera
   "Validación: 1 error … · sin token" y **Crear** deshabilitado (gris). Captura: `docs\fases\capturas\fase6-04-error-paso.png`.
3. Vuelve a poner `60` en el paso y `9,525` en el espesor de la cartela (la etiqueta debe volver a `3/8"`), o pulsa
   **Recargar** para leer otra vez el archivo original. Comprueba que "Validación: 0 errores" y **Crear** habilitado.

### 6-7. Crear la conexión desde la ventana

1. Pulsa **Crear**. La ventana se cierra, Revit trabaja unos segundos (Advance Steel) y aparece el diálogo
   **MotorConexiones - Conexión creada** con el `ID de conexión`, `Elementos creados: 9` (o cerca) y `Barras retiradas: 3`.
   Anota en el chat el ID. Cierra el diálogo.
2. **La persona**, en la vista 3D: nivel de detalle **Fino**, estilo **Sombreado**, zoom al nudo: cartela, placa cuchilla
   con 4 pernos y barras acortadas, como en la ronda 5b. Captura: `docs\fases\capturas\fase6-05-nudo-creado.png`.
3. **El instalador**:

   ```powershell
   Anota "6-7 conn_list" { .\scripts\conn-call.ps1 -Operation list }
   ```

   Se espera `ok: true` y `connections_count: 1` con el mismo `connection_id` del diálogo.

### 6-8. Botón "Conexiones del modelo": borrar

1. Pulsa **ARBA > Conexiones > Conexiones del modelo**. Debe abrirse una ventana con una fila: el id de la conexión,
   `gusset_node`, `Detalle D`, la fecha, `9` elementos, las barras `1249510, 1249630, 1249631, 1249636` y `advancesteel`.
   Captura: `docs\fases\capturas\fase6-06-conexiones-modelo.png`.
2. Selecciona la fila, pulsa **Borrar seleccionada** y responde **Sí** a la confirmación. La línea de estado debe decir
   `Borrada <id>: 9 elementos eliminados y 3 barras restauradas.` y la lista quedar vacía ("No hay conexiones…").
   Cierra la ventana con **Cerrar**.
3. **El instalador** comprueba que no queda nada y que las barras volvieron a su extensión:

   ```powershell
   Anota "6-8 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6-8 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones en el modelo: 0`, las extensiones `1249630: inicio 0.0 | fin 68.64`, `1249631: inicio 0.0 | fin 69.2`,
   `1249636: inicio 0.0 | fin 0.0` y `Elementos de acero sueltos encontrados: 0`.

### 6-9. La IA no se ve afectada: 19/19 por el puente

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 15
Anota "6-9 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 19/19 pruebas correctas` (el `addin_version` que imprime la prueba 1 debe ser `0.2.0`). Si alguna
da `[FALLO]`, copia su bloque entero.

### 6-10. Registro del add-in de hoy

```powershell
Anota "6-10 log" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String 'ribbon|preview_|run_spec_ribbon|delete_ribbon|connections_window|_failed' | Select-Object -Last 40 }
```

Importan `preview_window_opened`, `preview_edit` (una por valor cambiado, con `path` y `note`), `preview_saved`,
`preview_window_accepted`, `run_spec_ribbon_created`, `connections_window_closed` con `"deleted":1`, `delete_ribbon` y
que **no** haya ninguna línea `*_failed`.

### 6-11. Cerrar y subir (autorizado)

1. La persona cierra la copia en Revit **sin guardar** (Archivo > Cerrar > *No guardar*) y cierra la ventana del puente.
2. Sube resultados y capturas (el archivo `detalle-D-confirmado-corregido.json` **no** se sube: es la prueba del paso 6-5
   y se puede borrar después):

   ```powershell
   git add docs\fases\resultados-fase-6.md docs\fases\capturas
   git commit -m "Fase 6: resultados del instalador (ventana de previsualización, borrado desde la cinta, panel ARBA)"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

3. Devuelve: dónde quedó el botón (6-3) con la salida del sondeo 15, las seis capturas, las diferencias de cotas y la
   orientación que anotó la persona (6-4), los dos tokens (6-5), la salida de 6-5, 6-7, 6-8, 6-9 y 6-10, el texto de
   cualquier ventana de error de Revit o del add-in que haya aparecido, y si Revit se cerró de golpe en algún paso (en
   cuál y qué fue lo último que se vio).
