# Instalación y prueba de la Fase 10: add-in 0.10.0, selección asistida, etiquetas pinchables, cartelas fantasma y el encargo para IA

> **Hecha el 2026-10-07** (`docs/fases/resultados-fase-10.md`; contraste en `docs/fases/fase-10.md`, sección 7). El paso 10-3
> murió en el cuadro de la selección asistida ("Corresponding button not found: defaultButton", corregido en la 0.10.1) y por
> eso el 10-4 (pinchar las etiquetas) no se hizo; el 10-9 dio 14/22 porque el documento activo era otro y el servidor del
> puerto 8000 no estaba. **Solo eso se repite** en `docs/instalacion/fase-10b.md`. No vuelvas a ejecutar esta instalación.

Objetivo: desplegar el add-in **0.10.0** (`docs/fases/fase-10.md`) y comprobar sobre una **copia** del modelo: (1) la
**selección asistida** (pinchar una barra y Planificar lote); (2) las **etiquetas pinchables** en la vista (el clic nunca abre
un cuadro: la ventana elige la fila) y las **cartelas fantasma**; (3) lo pendiente de la Fase 9 (`fase-9.md` 7.6): Editar nudo
con la ventana abierta, `PLAN_MARKS_REPLACED`, Crear 16 y Ctrl+Z (las marcas vuelven), **Borrar el lote desde la ventana**,
el texto del aviso del borrado seguido y el de los `REVIT_WARNING`; (4) `conn_batch_plan` con `expand_selection`, las etiquetas
por el puente y el sondeo 21; (5) el botón **Encargo para IA**; (6) `probar_conexiones.py --puente` entero (32/32) y el log.
Unos 90 minutos sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta fase crea acero de
verdad en la copia** (el lote de 16, dos veces) y lo borra al final.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell.
Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 10-1. **Revit lo
abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en
PowerShell. Mientras la ventana del plan está trabajando ("⏳ …"), el instalador no ejecuta nada. **Si Revit se cerrara** en
algún paso: anota en qué botón o clic, captura del cuadro, vuelve a abrir Revit y la copia y sigue con el siguiente; el log
del paso 10-9 dirá dónde se quedó. Si las etiquetas no se ven o el clic no hace nada, **no es un fallo de la instalación**:
anótalo tal cual (es lo que esta fase viene a averiguar) y sigue.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 10-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la Fase 9 terminó con 0 y 0). Los sondeos 12,
  13 y 17 del paso 10-2 lo dicen y limpian.
- Las plantillas del catálogo en su sitio (`templates_count: 1`, `Nudo tipico Detalle D`).
- Para la selección asistida hace falta saber **qué barra pinchar**: una diagonal cualquiera de la cercha de la 8c (la que
  tiene el cordón central en ocho tramos y los cordones superior e inferior); apunta su ID (Propiedades) para los pasos 10-6
  y 10-7.

### 10-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-10.md"
"# Resultados de la Fase 10`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "10-1 git" { git log -1 --oneline; git status --short }
Anota "10-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "10-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "10-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "10-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "10-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "10-1 catalog.json desplegado" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json" | Select-String "batch_single_undo|plan_labels|plan_ghosts" }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Fase 10: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 217`**, `10-1 revit cerrado` vacío,
`== MotorConexiones 0.10.0.0 desplegado en Revit 2027 ==` (con `config\catalog.json` entre los copiados),
`copiado revit_mcp\conexiones.py (25 rutas @api.route)`, `copiado tools\conn_tools.py (23 herramientas @mcp.tool)`,
`0.10.0.0` en la versión de la DLL y `"plan_labels": true` y `"plan_ghosts": true` en el `catalog.json` desplegado. Si el
build falla o sale `1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 10-2. Abrir Revit, ping, restos y el sondeo 21 **(la persona abre; el instalador llama)**

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de siempre, con la cercha de la 8c a media distancia. No selecciones nada y no abras ninguna ventana del add-in.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "10-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "10-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "10-2 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "10-2 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "10-2 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "10-2 sondeo 21 etiquetas antes" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "10-2 log de arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|label_handler|ribbon_panel" | Select-Object -Last 6 }
   ```

   Se espera en `ping`: `addin_version: 0.10.0` (en `data` y en `meta`) y 23 operaciones. En `catalog_list`,
   `templates_count: 1`. Sondeos 12 y 13: `conexiones en el modelo: 0` y `Elementos de acero sueltos encontrados: 0`. Sondeo
   17 entero, con `Marcadores de plan … : 0`. **Sondeo 21**: `2) Controles en el lienzo (GetAll): 0`, `3) Servidor del
   add-in 3f6c1b2e-…: registrado=True | activo=True` (si dice `registrado=False`, cópialo: el log de arranque dirá por qué,
   `label_handler_error`), `5) Marcadores de plan en el modelo: 0`. En el log: `"event":"startup"` con
   `"addin_version":"0.10.0"` y `"label_handler":true`, y una línea `label_handler_registered`.

### 10-3. Selección asistida: pinchar UNA barra y Planificar lote **(la persona)**

1. Pincha **una diagonal** de la cercha de la 8c (solo una) y pulsa ARBA > **Planificar lote**. Debe salir un cuadro
   "Selección asistida: N barras tocan la selección" con el texto "Se añadieron N barras que tocan la selección: … cordones
   que pasan de largo, … barras que llegan y … tramos de cordón." y tres opciones. **Captura del cuadro**:
   `docs\fases\capturas\fase10-01-seleccion-asistida.png`. Anota N y el texto literal. Se esperan 63 añadidas (los 64
   elementos de la 8c menos la pinchada; si el cordón inferior existe como barra, más los de abajo) y ninguna "fuera del plano".
2. Pulsa **Planificar con las N barras**. La ventana se abre con los mismos 16 listos de siempre (cabecera "Se crearán 16
   conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. …") y, bajo la cabecera, "…
   barras seleccionadas (N añadidas por la selección asistida) · … · marcas puestas en la vista con 33 etiquetas pinchables
   y 16 cartelas fantasma" (los números pueden variar: anótalos). La barra de estado empieza por "Se añadieron …". Si los
   listos no son 16, anota qué cambió (ese es el dato que busca esta fase).
3. **Cancelar y solo la selección**: pincha otra diagonal, Planificar lote, en el cuadro pulsa **Cancelar** (no pasa nada y
   la ventana sigue con el plan anterior); otra vez Planificar lote y **Planificar solo las 1 seleccionadas**: la ventana
   dice que con una sola barra no hay nudo (o el cuadro de "no encontré ninguna otra que la toque" si no tocaba nada) y el
   plan anterior sigue. Anótalo. Después, vuelve a dejar la cercha planificada: pincha una diagonal, Planificar lote,
   **Planificar con las N barras**.

### 10-4. Etiquetas pinchables y cartelas fantasma **(la persona)**

1. Mira la cercha en la vista 3D: en cada nudo marcado hay, además del cubo o rombo, una **etiqueta redonda** con el número
   (verde, ámbar, roja o gris según el estado) y, en los 16 listos, una **cartela fantasma** (una placa transparente del
   contorno del Detalle D, verde o ámbar, centrada en el plano de la cercha). Orbita y haz zoom: las etiquetas mantienen su
   tamaño en pantalla; las cartelas son geometría. Captura general: `docs\fases\capturas\fase10-02-etiquetas-fantasmas.png`.
   Anota si las etiquetas se ven (y si todas, o solo algunas), si las cartelas fantasma están donde irían las cartelas (en
   N4 compárala con la foto de la Fase 7), y si alguna etiqueta tapa algo importante.
2. **Pincha la etiqueta 4** (la de N4, el gemelo del Detalle D). Debe pasar, sin ningún cuadro: la etiqueta pasa a círculo
   blanco con el 4 en color (resaltada), la ventana del plan elige la fila N4 y su barra de estado dice "Etiqueta 4 pinchada en
   la vista: N4 · Listo con aviso · Nudo tipico Detalle D · igual. Ver en Revit encuadra el nudo…". Captura de la ventana y la
   vista juntas: `docs\fases\capturas\fase10-03-etiqueta-pinchada.png`. Anota literalmente **si salió algún cuadro** (no debe)
   y si Revit siguió respondiendo (orbita después del clic).
3. En la **tabla** de la ventana elige N7: la etiqueta 7 pasa a resaltada y la 4 vuelve a su color. Haz lo mismo con un clic
   en el círculo 12 del **mapa**. Anota si las etiquetas cambian al momento o tardan (cambian cuando Revit queda libre).
4. Los nudos ocultos (barras sueltas, parejas sin cordón) no tienen etiqueta: no se marcan. Pincha una etiqueta **roja** (un
   nudo "Sin plantilla que encaje" del cordón superior): la fila elegida es esa y la barra de estado lo dice. Después pincha una etiqueta con la
   ventana del plan **cerrada** (Cerrar): la etiqueta se resalta igual y no sale ningún cuadro; vuelve a abrir la ventana con
   Planificar lote sin selección (la trae delante) y comprueba que la fila elegida no cambió.
5. Pasa el ratón sobre una etiqueta: el globo dice "N4 · Listo con aviso · Nudo tipico Detalle D · igual · pincha para
   elegirlo en la ventana del plan". Anótalo.

### 10-5. Lo pendiente de la Fase 9: Editar nudo, PLAN_MARKS_REPLACED, Crear 16, Ctrl+Z, Borrar el lote desde la ventana **(la persona y el instalador)**

1. **(la persona) Editar nudo con la ventana abierta**: elige N4 y pulsa **Editar nudo**: se abre la previsualización modal
   con la cabecera "Nudo N4 del plan (…)"; Cancelar. Anota si la ventana del plan sigue normal después.
2. **(instalador) PLAN_MARKS_REPLACED**: con la ventana abierta y quieta, planifica por el puente la misma cercha (usa el ID
   de la diagonal del paso 10-3 con `expand_selection`):

   ```powershell
   $tid = "6abcf116-9b97-485f-b50d-2851ca0018cc"
   $diag = <ID de la diagonal pinchada en 10-3>
   Anota "10-5 batch_plan por el puente (expand_selection)" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"element_ids":[' + $diag + '],"expand_selection":true,"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, el aviso `PLAN_MARKS_REPLACED` (el plan de la ventana pierde las marcas y las etiquetas) y el aviso
   `SELECTION_EXPANDED`, `selection_expansion` con `added_count` (igual que el cuadro del paso 10-3), `selection_count` =
   1 + añadidas, 16 listos, `labels: {count: 33, view_id: …}` (o el número que haya), `label_indices` por nudo y, por nudo
   visible, `label_index`; en los listos, `ghost_element_id`. **Copia el `plan_id`** (`$plan`). En la vista debe haber un solo
   juego de marcas, etiquetas y fantasmas (los del plan del puente).
3. **(la persona)** Pincha una diagonal y **Planificar lote** > Planificar con todas: la barra de estado lleva
   `PLAN_MARKS_REPLACED` (el plan del puente pierde las marcas) y sigue habiendo un solo juego de marcas y etiquetas. Anótalo.
   Ejecuta el sondeo 21 después (instalador): `Anota "10-5 sondeo 21 un solo juego" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }`: los controles del lienzo tienen que ser los del
   plan de la ventana (33 o los que diga su cabecera), no el doble.
4. **(la persona) Crear 16 conexiones** > Sí. Al terminar: cabecera "Creadas 16 conexiones (14 con aviso)", filas `✔`, y en el
   modelo las cartelas de verdad **sin** color, sin cubo, **sin etiqueta y sin cartela fantasma** en los 16 nudos; los rojos y
   grises siguen con su etiqueta. Captura: `docs\fases\capturas\fase10-04-lote-creado.png`. Anota el tiempo.
5. **(la persona) Ctrl+Z** una sola vez con el foco en Revit: las 16 conexiones desaparecen, **las marcas y las cartelas
   fantasma vuelven** (eran parte del mismo grupo) y **las etiquetas de esos 16 no** (no son elementos del modelo). Anótalo.
   Pulsa **Replanificar** en la ventana: los 16 vuelven a `● Listo` / `▲ Listo con aviso` y **las etiquetas se reponen**.
6. **(la persona)** **Crear 16 conexiones** otra vez > Sí. Después **Borrar el lote** (el botón de abajo) > **Sí**: anota el
   texto del cuadro de confirmación, y al terminar la barra de estado ("Lote …: 16 conexiones borradas (… elementos, 48 barras
   restauradas). Una sola entrada de deshacer (Ctrl+Z). Replanificado."), la cabecera otra vez con 16 listos y las marcas,
   etiquetas y fantasmas de vuelta. Captura: `docs\fases\capturas\fase10-05-lote-borrado.png`. Mira el menú Deshacer: una
   entrada "MotorConexiones: batch_delete …". **No deshagas.**
7. **(instalador)** tras el borrado:

   ```powershell
   Anota "10-5 sondeo 12 tras borrar el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "10-5 sondeo 13 tras borrar el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "10-5 log del borrado" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "ribbon_batch_delete|batch_delete|fabrication_transaction_failed" | Select-Object -Last 8 }
   ```

   Se espera 0 y 0, `ribbon_batch_delete` con `deleted: 16` y `batch_delete` con `deleted 16`. Si hay líneas
   `fabrication_transaction_failed` con `"for_deletion":true`, el aviso correspondiente de la respuesta (lo enseña el sondeo 12
   si tuvo que borrar algo, o el bloque `10-6`) debe decir "**para borrar: … Los elementos se borran directamente**", nunca
   "Toda la conexión se crea con DirectShape".

### 10-6. El aviso del borrado seguido y el texto de los REVIT_WARNING **(instalador)**

```powershell
$spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-D-confirmado.json
Anota "10-6 validate Detalle D" { .\scripts\conn-call.ps1 -Operation validate -Body ('{"spec":' + $spec + '}') -TimeoutSec 300 }
```

Copia el `validation_token` (`$tok`) y crea, mira los avisos y borra:

```powershell
$tok = "<validation_token>"
Anota "10-6 create Detalle D en la 3D (avisos de Revit)" { .\scripts\conn-call.ps1 -Operation create -Body ('{"spec":' + $spec + ',"validation_token":"' + $tok + '"}') -TimeoutSec 600 }
Anota "10-6 list" { .\scripts\conn-call.ps1 -Operation list }
```

Se espera `ok: true` y en `warnings[]` los `REVIT_WARNING` con su `message`: **copia los textos** (es lo pendiente de
`fase-9.md` 7.2, punto 4: se espera algo como "The created elements are only visible in Detail Level: Fine"). Después, con el
`connection_id` de la respuesta (`$cid`):

```powershell
$cid = "<connection_id>"
Anota "10-6 delete" { .\scripts\conn-call.ps1 -Operation delete -Body ('{"connection_id":"' + $cid + '"}') -TimeoutSec 600 }
Anota "10-6 sondeo 12 (16 borrados seguidos no hay: solo comprueba a cero)" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera `deleted_elements_count: 9` y, si `delete` avisó de la sesión de Advance Steel, el texto nuevo ("para borrar … se
borran directamente"). El aviso del borrado **seguido** (16 de una vez) ya se vio en 10-5.7 con Borrar el lote.

### 10-7. El puente: expand_selection, etiquetas y descartar; sondeo 21 a cero **(instalador; la persona cierra la ventana)**

1. **(la persona)** Cierra la ventana del plan con **Cerrar** (deja las marcas).
2. **(instalador)** con `$plan` = el `plan_id` de la ventana (la cabecera lo enseña abreviado; `batch_plan_get` sin `plan_id`
   devuelve el último):

   ```powershell
   Anota "10-7 batch_plan_get (etiquetas)" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body '{"include_specs":false}' -TimeoutSec 300 }
   Anota "10-7 batch_plan expand_selection desde una barra, sin marcas" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"element_ids":[' + $diag + '],"expand_selection":true,"mark":false,"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera en `batch_plan_get`: `labels.count` > 0, `label_view_id`, `label_indices` y `marks.ghost_count: 16`. En el segundo:
   `selection_expansion.added_count` como en 10-3, `labels.count: 0` (sin marcas no hay etiquetas), `is_marked: false`. Copia
   su `plan_id` (`$plan2`). Después:

   ```powershell
   $plan2 = "<plan_id>"
   Anota "10-7 batch_plan_discard del plan sin marcas" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body ('{"plan_id":"' + $plan2 + '"}') }
   Anota "10-7 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
   Anota "10-7 sondeo 21 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "10-7 sondeo 17 tras descartar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera `removed_labels: 0` en el primero, y en `all`: `removed_markers` (los cubos, rombos y fantasmas del plan de la
   ventana), `remaining_markers: 0`, `removed_labels` (las etiquetas que había) y `remaining_labels: 0`; sondeo 21 con
   `GetAll: 0` y `5) Marcadores de plan en el modelo: 0 (… fantasmas: 0)`; sondeo 17 a cero. **(la persona)** Mira la vista: sin
   colores, sin cubos, sin fantasmas y sin etiquetas.

### 10-8. Encargo para IA **(la persona; el instalador mira la carpeta)**

1. **(la persona)** Selecciona el nudo del Detalle D de la Fase 3 (las cuatro barras 1249510, 1249630, 1249631 y 1249636) y
   pulsa ARBA > **Encargo para IA**. Debe salir un cuadro "Encargo copiado al portapapeles" con la ruta del archivo y abrirse
   el Explorador en `%LOCALAPPDATA%\MotorConexiones\encargos\` con el `.md` marcado. Captura del cuadro:
   `docs\fases\capturas\fase10-06-encargo.png`. Pega el portapapeles en el Bloc de notas: empieza por "# Encargo para una IA
   externa: conexión gusset_node en HANGAR_PRUEBA_sondeo" y tiene las secciones Prompt, Datos del nudo, Esquema, Cómo leer un
   detalle, Ejemplo confirmado y Qué devolver. Anota cuál es el ejemplo ("la plantilla 'Nudo tipico Detalle D' del catálogo"
   o "el Detalle D de la Fase 3 … embebido").
2. **(instalador)**:

   ```powershell
   Anota "10-8 encargos" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\encargos" | Select-Object Name, Length; Get-Content -Encoding UTF8 (Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\encargos\*.md" | Sort-Object LastWriteTime | Select-Object -Last 1).FullName | Select-String "^# |^## " }
   ```

3. **(la persona, opcional pero recomendado)** Pega el encargo en Claude (navegador) con la imagen `docs\fixtures\detalle-D.png`,
   guarda el JSON que devuelva como `Documentos\MotorConexiones\encargo-detalle-D.json` y aplícalo con **Ejecutar
   especificación JSON** (previsualización, validación; **no hace falta crear**: Cancelar). Anota si el validador aceptó el JSON
   a la primera y, si no, qué errores dio.

### 10-9. Puente y log, anotados en el archivo **(instalador, ventanas cerradas)**

```powershell
Anota "10-9 sondeo 12 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "10-9 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 30
Anota "10-9 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "10-9 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|label_|labels_|selection_expanded|ribbon_batch_assist|ribbon_batch_label|ribbon_batch_expand|ghost_|batch_create|batch_delete|ribbon_batch_delete|ribbon_design_brief|plan_event_failed|ribbon_batch_window_error" | Select-Object -Last 150 }
Select-String -Path $salida -Pattern "^## 10-9" | Select-Object LineNumber, Line
```

Se espera **`Resultado: 32/32 pruebas correctas`** (la 29 planifica con `expand_selection` desde una barra: en Revit añade la
cercha entera que contiene al Detalle D, así que `añadidas` será un número grande; la 30 la descarta; la 31 necesita el
servidor arrancado: si falla sola, repite el `Anota` del puente pasados otros 30 s). En el log: `"addin_version":"0.10.0"`,
`label_handler_registered`, `label_folder` (la carpeta elegida y `"plain":true`), `labels_applied` con `count`,
`label_clicked` por cada etiqueta pinchada (con `node` y `highlighted: true`), `ribbon_batch_label_clicked`,
`ribbon_batch_assist` con `choice` (`all`, `selection`, `cancel`), `selection_expanded` con `added` y `duration_ms`,
`labels_removed`, `labels_cleared`, `ribbon_batch_delete` con `deleted: 16`, `ribbon_design_brief` con `clipboard: true`, y
**ninguna** línea `label_click_failed`, `labels_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas
enteras). **La última línea debe listar dos secciones `## 10-9`**; si falta alguna, repite su `Anota`.

### 10-10. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "10-10 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-10.md docs\fases\capturas
   git commit -m "Fase 10: resultados del instalador (0.10.0, seleccion asistida, etiquetas, cartelas fantasma, encargo, puente)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 10-1, 10-2, 10-5 (bloques del instalador), 10-6, 10-7, 10-8.2 y 10-9; el `git
   status --short` literal (antes del commit y al final, que debe quedar vacío); las anotaciones de la persona de los pasos
   10-3 (N y el texto del cuadro; los 16 listos; Cancelar y Solo la selección), 10-4 (si las etiquetas y las cartelas fantasma se
   ven, **si salió algún cuadro al pinchar**, si la ventana eligió la fila y qué dijo la barra de estado, los cambios de
   resaltado desde la tabla y el mapa, el clic con la ventana cerrada, el globo), 10-5 (Editar nudo, `PLAN_MARKS_REPLACED`,
   Crear 16 y el tiempo, qué hizo Ctrl+Z con marcas, fantasmas y etiquetas, Borrar el lote desde la ventana con el texto del
   cuadro y de la barra de estado) y 10-8 (el cuadro del encargo, el ejemplo usado y, si lo probaste, cómo le fue a la IA);
   las seis capturas; y el texto de cualquier ventana de error. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
