# Instalación y prueba de la Fase 11: add-in 0.11.0, botón Conectar, panel por secciones, miniaturas y el plano al lado

Objetivo: desplegar el add-in **0.11.0** (`docs/fases/fase-11.md`) y comprobar sobre una **copia** del modelo: (1) **Conectar**
con el nudo del Detalle D seleccionado abre la previsualización con la plantilla propuesta, sin pedir ningún archivo, y el
**panel por secciones** (desplegables de espesor, pernos y unión) escribe los mm y el rótulo a la vez; (2) el **plano al lado**
del croquis (archivo y portapapeles); (3) **Conectar sin plantilla** sobre un nudo del cordón superior (especificación de
arranque con cartela rectangular); (4) las **miniaturas** del catálogo; (5) los tres retoques de paso de la Fase 10 (la línea
"(63 añadidas…)" desde la cinta, `changed` / `highlighted` en el log, los títulos de los cuadros); (6) por el puente,
`catalog_apply` sin `template_id`, `validate` con `source.drawing_image` y `probar_conexiones.py --puente` 34/34. Unos 75
minutos sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta fase crea acero de verdad en
la copia** (dos conexiones de un nudo cada una) y lo borra al final.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell.
Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 11-1. **Revit lo
abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en
PowerShell. Mientras una ventana del add-in está abierta, el instalador no ejecuta nada. **Si Revit se cerrara** en algún paso:
anota en qué botón o clic, captura del cuadro, vuelve a abrir Revit y la copia y sigue con el siguiente; el log del paso 11-8
dirá dónde se quedó. Si el arranque del paso 11-5 no valida, o una miniatura sale gris, **no es un fallo de la instalación**:
anótalo tal cual (es lo que esta fase viene a averiguar) y sigue.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 11-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in, sin marcadores ni etiquetas (la ronda 10b terminó con todo a
  cero). Los sondeos 12, 13, 17 y 21 del paso 11-2 lo dicen y limpian. **No** hay plan marcado.
- Las plantillas del catálogo en su sitio (`templates_count: 1`, `Nudo tipico Detalle D`).
- Localizar tres nudos: (a) el **Detalle D** (cordón 1249510 y barras 1249630, 1249631, 1249636); (b) **el siguiente nudo
  del cordón central** hacia +X, con su cordón y sus 3 barras (apunta sus IDs en Propiedades); (c) **un nudo del cordón
  superior** con su cordón y las dos diagonales que llegan (2 barras: ninguna plantilla de 2 barras en el catálogo). Apunta
  los IDs de (b) y (c) para los pasos 11-4 y 11-5.
- El archivo `docs\fixtures\detalle-D.png` del repositorio (para el paso 11-4).

### 11-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-11.md"
"# Resultados de la Fase 11`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "11-1 git" { git log -1 --oneline; git status --short }
Anota "11-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "11-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "11-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "11-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "11-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "11-1 choices.json desplegado" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\choices.json" | Select-String "bolt_diameter_labels|gusset_thickness_label|slot_length_mm" }
```

Se espera `git status --short` vacío antes del `pull`, un commit "Fase 11: ..." o posterior, `0 Advertencia(s)`, `0 Errores`,
**`Superado: 238`**, `11-1 revit cerrado` vacío, `== MotorConexiones 0.11.0.0 desplegado en Revit 2027 ==` con
**`config\choices.json`** entre los copiados y `miniaturas .png copiadas: 0` (el repositorio no trae PNG; los dibuja el add-in),
`copiado revit_mcp\conexiones.py (25 rutas @api.route)`, `copiado tools\conn_tools.py (23 herramientas @mcp.tool)`, `0.11.0.0`
en la DLL y las tres claves en el `choices.json` desplegado. Si el build falla o sale `1 Advertencia(s)`, **para aquí**.

### 11-2. Abrir Revit, ping y restos **(la persona abre; el instalador llama)**

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (único proyecto abierto) y ponte en
   la **vista 3D** sombreada de siempre. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "11-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "11-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "11-2 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "11-2 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "11-2 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "11-2 sondeo 21 etiquetas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "11-2 log de arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_panel" | Select-Object -Last 4 }
   ```

   Se espera en `ping`: `addin_version: 0.11.0` y 23 operaciones; `catalog_list` con `templates_count: 1`; sondeos 12, 13, 17
   y 21 a cero; en el log, `"addin_version":"0.11.0"`. En la cinta (ARBA > MotorConexiones) el primer botón es **Conectar**.

### 11-3. Conectar con plantilla: el Detalle D, el panel y Crear **(la persona)**

1. Selecciona las cuatro barras del Detalle D (1249510, 1249630, 1249631, 1249636) y pulsa ARBA > **Conectar**. **Sin pedir
   ningún archivo** debe abrirse la previsualización con la cabecera del panel "Plantilla propuesta: Nudo tipico Detalle D ·
   igual · desvío máx. …° . Otras que encajan: ninguna." con la miniatura de la plantilla, el croquis a la izquierda y, a la
   derecha, el **panel por secciones** (Cartela, Cordón, Barra 1 · diagonal · …, Barra 2, Barra 3). Captura:
   `docs\fases\capturas\fase11-01-conectar.png`. Anota el texto literal de la cabecera (el desvío) y si la validación de abajo
   está en verde.
2. En **Cartela > Espesor** elige `1/2" · 12,7 mm`: el croquis debe decir `cartela PL 1/2" · 12,7 mm` y la lista de abajo **no**
   debe tener `LABEL_VALUE_MISMATCH` (anota si lo tiene). En la **Barra 3** (la de la placa cuchilla) cambia **Pernos: Ø** a
   `3/4" · 19,1 mm`: anota si aparece `BOLT_EDGE_DISTANCE_TOO_SMALL` (con borde 40 **no** debería: el mínimo de 3/4" es 25).
   Vuelve a `3/8"` y a `5/8"`.
3. En la **Barra 1** cambia **Unión** a `placa cuchilla atornillada`: deben aparecer los campos de la placa (PL10, 170, 140,
   80, +z) y de los pernos (5/8", 2, 2, 60, 40, 40) y el croquis dibuja la placa. Vuelve a `soldada en ranura`: vuelve la
   ranura de **150**. Anota si algún error quedó junto a un campo (borde rojo con el mensaje debajo).
4. Pestaña **Avanzado**: la tabla de siempre y, abajo, el JSON con los cambios (pulsa **Copiar** y pégalo en el bloc de notas
   para comprobar que llegó). Vuelve a **Panel**. Captura: `docs\fases\capturas\fase11-02-panel.png`.
5. Pulsa **Crear** (solo si la validación está en verde; si no, anota los códigos y Cancelar). Anota el `connection_id` del
   cuadro. El título del cuadro debe ser una sola vez "Crear conexión" bajo el nombre del add-in (no "MotorConexiones -
   MotorConexiones - …").
6. **(instalador)** `Anota "11-3 list tras crear" { .\scripts\conn-call.ps1 -Operation list }`. Se espera `connections_count: 1`
   (el sondeo 12 **no** se lanza aquí: borraría la conexión; se usa al final, en el 11-8).

### 11-4. El plano al lado del croquis **(la persona)**

1. Selecciona el nudo (b) (el siguiente del cordón central, cordón y 3 barras) y pulsa **Conectar**: la ventana con la
   plantilla propuesta (anota la orientación: igual o en espejo).
2. Pulsa **Plano… > Abrir archivo…** y elige `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D.png`: la imagen aparece a la
   **izquierda** del croquis con un divisor; rueda = zoom, arrastrar = encuadre, **Ajustar**. En Avanzado, el JSON lleva
   `source.drawing_image` con la ruta. Captura: `docs\fases\capturas\fase11-03-plano.png`.
3. Pulsa **Ocultar plano** (el panel se pliega) y después **Plano… > Mostrar plano** (vuelve con la misma imagen).
4. Haz un recorte de cualquier cosa con **Win+Mayús+S** y pulsa **Plano… > Pegar del portapapeles**: la imagen pegada
   sustituye a la anterior y la barra de estado dice la ruta (`%LOCALAPPDATA%\MotorConexiones\planos\plano-…png`).
5. **Cancelar** sin crear.

### 11-5. Conectar sin plantilla: el arranque sobre el cordón superior **(la persona)**

1. Selecciona el nudo (c) (cordón superior y sus 2 diagonales) y pulsa **Conectar**. La cabecera debe decir "Ninguna de las 1
   plantilla encaja con 2 barras a …" seguido de "Especificación de arranque con cartela rectangular (valores de la plantilla
   Nudo tipico Detalle D)" si el cordón superior es HSS3X3X1/4, o "(valores por defecto de choices.json)" si es otro perfil,
   con los botones **Encargo para IA** y, si hubiera otras candidatas, Cambiar…. El croquis enseña una **cartela rectangular**
   y las dos barras **soldadas en ranura**; en el panel la unión al cordón aparece como `atraviesa el cordón ranurado` y en
   Avanzado hay una duda confirmada (`gusset.chord_interface`). Anota el texto literal de la cabecera y **los códigos** de la
   lista de abajo (errores y avisos). Captura: `docs\fases\capturas\fase11-04-arranque.png`.
2. Si la validación está en verde, **Crear** y anota el `connection_id`. Si no, anota los errores literales y **Cancelar**: es
   lo que esta fase viene a averiguar, no un fallo de la instalación.
3. **(instalador)** `Anota "11-5 list tras el arranque" { .\scripts\conn-call.ps1 -Operation list }`. Se esperan 2 conexiones
   (o 1 si el arranque no validó).

### 11-6. Miniaturas del catálogo **(la persona; el instalador mira la carpeta)**

1. Pulsa ARBA > **Catálogo**: la fila del Detalle D tiene la columna **Croquis** con la miniatura y, al elegirla, la vista
   grande a la derecha del detalle. Si la miniatura sale **gris con el nombre**, anótalo (es el hueco de "no se pudo dibujar").
   Captura: `docs\fases\capturas\fase11-05-catalogo.png`. El título del cuadro de Eliminar (si lo abres y cancelas) debe
   ser "Catálogo" una sola vez.
2. **(instalador)** `Anota "11-6 catalogo dir" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\catalogo" | Select-Object Name, Length, LastWriteTime }`.
   Se espera un `6abcf116-….png` junto al `.json`.
3. **(la persona)** Si el arranque del 11-5 validó: vuelve a abrir **Conectar** sobre el nudo (c) (dará el aviso de que ya
   tiene conexión), pulsa **Guardar en catálogo** con el nombre `PRUEBA fase 11`, Cancelar, y en **Catálogo** comprueba que la
   plantilla nueva aparece **con miniatura**; después **Eliminar**. Si no validó, salta este punto y anótalo.

### 11-7. Los retoques de paso **(la persona)**

1. Pincha **una** diagonal de la cercha de la 8c y **Planificar lote** → **Planificar con las 64 barras**. La línea bajo la
   cabecera debe decir "… barras seleccionadas **(63 añadidas por la selección asistida)** …" (en la 10b no lo decía). Anótala.
2. Pincha la **etiqueta 4** en la vista **dos veces** seguidas. En el detalle del nudo N4 (abajo en la ventana) debe verse la
   **miniatura** del Detalle D junto al texto; el mapa de la cercha sigue sin miniaturas. Captura:
   `docs\fases\capturas\fase11-06-titulo.png` con un cuadro abierto (por ejemplo, Planificar lote con nada seleccionado y la
   ventana cerrada abre "Pincha primero una barra…"): el título debe ser "Planificar lote" una sola vez.
3. **Descartar plan** en la ventana.
4. **(instalador)**:

   ```powershell
   Anota "11-7 label_clicked" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "label_clicked" | Select-Object -Last 4 }
   Anota "11-7 sondeo 17 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "11-7 sondeo 21 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera en `label_clicked`: el primero con `"changed":true` y el segundo con `"changed":false`, **los dos** con
   `"highlighted":true`; sondeos 17 y 21 a cero.

### 11-8. Puente, borrado y log, anotados en el archivo **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Anota "11-8 catalog_apply sin template_id" { .\scripts\conn-call.ps1 -Operation catalog_apply -Body '{"element_ids":[1249510,1249630,1249631,1249636],"chord_element_id":1249510}' }
Anota "11-8 validate con drawing_image" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-D-confirmado.json | ConvertFrom-Json; $spec.source | Add-Member -NotePropertyName drawing_image -NotePropertyValue "D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D.png" -Force; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "11-8 list antes de borrar" { .\scripts\conn-call.ps1 -Operation list }
```

Se espera en `catalog_apply`: `ok: true` (o `false` solo por errores de validación), `data.proposal.chosen.template_id` =
`6abcf116-…`, `data.proposal.summary_text` que empieza por "Plantilla propuesta: Nudo tipico Detalle D" y `data.spec.source.
template_id` el mismo. En `validate`: `ok: true` con `validation_token` (la clave nueva pasa el esquema). En `list`, las 2
conexiones (o 1). Después, **borra las conexiones creadas** con sus `connection_id` (los de los pasos 11-3 y 11-5):

```powershell
Anota "11-8 delete 1" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL 11-3>"}' }
Anota "11-8 delete 2" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL 11-5>"}' }
Anota "11-8 sondeo 12 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "11-8 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "11-8 sondeo 17 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "11-8 sondeo 21 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
Anota "11-8 ping antes del puente" { .\scripts\conn-call.ps1 -Operation ping }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "11-8 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "11-8 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "11-8 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "11-8 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_connect|ribbon_preview|thumbnail|drawing_pane|choices_|catalog_apply|label_clicked|ribbon_batch_assist|ribbon_create|_failed|ribbon_batch_window_error|plan_event_failed" | Select-Object -Last 150 }
Select-String -Path $salida -Pattern "^## 11-8" | Select-Object LineNumber, Line
```

Se espera: los dos `delete` con las barras restauradas; sondeos 12, 13, 17 y 21 a cero; `ping` con `document.title:
HANGAR_PRUEBA_sondeo`; `servidor 8000 escuchando` con una fila (si sale vacío, espera 30 s y repite ese `Anota`); **`Resultado:
34/34 pruebas correctas`** (31 contra 48884, con la 21b nueva, y 31, 32 y 32b contra el puente; si falla solo la 32b, copia su
bloque: es la nueva); `templates_count: 1`. En el log: `"addin_version":"0.11.0"`, `ribbon_connect` por cada Conectar (con
`mode: template` o `starter`, `proposal` o `starter: true`), `ribbon_preview_edit` con `from: panel`, `thumbnail_written` (al
menos uno), `drawing_pane_opened` (`from: archivo` y `from: portapapeles`), `catalog_apply` con `chosen_automatically: true`,
`label_clicked` con `changed` y `highlighted`, y **ninguna** línea `ribbon_connect_failed`, `thumbnail_failed`,
`drawing_pane_failed`, `label_click_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas enteras).
**La última línea debe listar todas las secciones `## 11-8`**; si falta alguna, repite su `Anota`.

### 11-9. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "11-9 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-11.md docs\fases\capturas
   git commit -m "Fase 11: resultados del instalador (0.11.0, Conectar, panel, plano, miniaturas, retoques, puente)"
   git pull --no-rebase origin main
   git push origin main
   ```

Qué devolver: `docs/fases/resultados-fase-11.md` (lo escribe `Anota`), las seis capturas (`fase11-01` a `fase11-06`) y, en el
chat, lo que no quedó en el archivo: los textos literales de las cabeceras (11-3 y 11-5), los códigos de validación del
arranque, si alguna miniatura salió gris, y cualquier cuadro inesperado.
