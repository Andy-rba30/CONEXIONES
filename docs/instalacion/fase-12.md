# Instalación y prueba de la Fase 12: add-in 0.12.0, los tres sondeos, nudo en K soldado, empalme con placa de extremo y el lote de la 8c

Objetivo: desplegar el add-in **0.12.0** (`docs/fases/fase-12.md`) y comprobar sobre una **copia** del modelo, en dos mitades:
**primero los sondeos 22, 23 y 24** (deciden si la fabricación de la segunda mitad es la buena o la reserva; no dejan nada en el
modelo) y después (1) `node_info` con la clase del nudo y la excentricidad; (2) **Conectar en un nudo en K** del cordón superior
(arranque `direct_weld`, crear, guardar en catálogo); (3) **Conectar en un empalme** del cordón superior (arranque `end_plate`
Detalle C, crear); (4) una sola barra y media cercha desde Conectar; (5) **Planificar lote** con las 64 barras de la 8c y **Crear**;
(6) por el puente, `validate` de los fixtures C y K, `batch_plan` sobre N9, Borrar el lote, `delete` y `probar_conexiones.py
--puente`. Unos **90 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**. **Esta fase crea
acero de verdad en la copia** (dos conexiones sueltas y un lote) y lo borra al final.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de PowerShell.
Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 12-1. **Revit lo
abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en
PowerShell. Mientras una ventana del add-in está abierta, el instalador no ejecuta nada. **Si Revit se cerrara** en algún paso:
anota en qué botón o clic, captura del cuadro, vuelve a abrir Revit y la copia y sigue con el siguiente; el log del paso 12-9
dirá dónde se quedó. **Si un sondeo dice FALLO o PARADA, no es un fallo de la instalación**: es lo que viene a averiguar; copia
la salida entera y sigue. Lo mismo si una conexión de un tipo nuevo no se crea o sale con la reserva (`END_CUT_APPROXIMATED`,
`BOLT_NUTS_APPROXIMATED`, `REVIT_WARNING` "Se crean con DirectShape"): anótalo tal cual.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 12-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in, sin marcadores ni etiquetas (la Fase 11 terminó con todo a
  cero). Los sondeos 12, 13, 17 y 21 del paso 12-2 lo dicen y limpian. **No** hay plan marcado.
- Las plantillas del catálogo en su sitio (`templates_count: 1`, `Nudo tipico Detalle D`).
- Localizar cuatro cosas en la cercha de la 8c (la que contiene al Detalle D): (a) el **nudo en K** del cordón superior del paso
  11-5 de la Fase 11: cordón **1245533** y diagonales **1249631** y **1249632** (si los IDs no coinciden, apunta los reales en
  Propiedades); (b) el **empalme** del cordón superior N9E de la ronda 8c: tramos **1245531** y **1245530** (idem); (c) una
  diagonal **del cordón central** (por ejemplo 1249630, del Detalle D); (d) las **64 barras** de la cercha (una barra basta: la
  selección asistida añade las demás).
- Los fixtures `docs\fixtures\detalle-C.json` y `docs\fixtures\nudo-K-cordon-superior.json` del repositorio (paso 12-9).

### 12-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-12.md"
"# Resultados de la Fase 12`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "12-1 git" { git log -1 --oneline; git status --short }
Anota "12-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "12-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "12-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "12-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "12-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "12-1 limits.json desplegado" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json" | Select-String "node_axis_max_offset_mm|min_bolt_to_tube_mm|min_hole_clearance_mm" }
Anota "12-1 choices.json desplegado" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\choices.json" | Select-String "hole_labels|diagonal_splice_offset_mm|direct_weld|by_profile" }
```

Se espera `git status --short` vacío antes del `pull`, un commit "Fase 12: ..." o posterior, `0 Advertencia(s)`, `0 Errores`,
**`Superado: 268`**, `12-1 revit cerrado` vacío, `== MotorConexiones 0.12.0.0 desplegado en Revit 2027 ==` con `config\limits.json`
y `config\choices.json` entre los copiados, `copiado revit_mcp\conexiones.py (25 rutas @api.route)`, `copiado tools\conn_tools.py
(23 herramientas @mcp.tool)`, `0.12.0.0` en la DLL, tres líneas en `limits.json` y cuatro en `choices.json`. Si el build falla o
sale `1 Advertencia(s)`, **para aquí**.

### 12-2. Abrir Revit, ping, tipos y restos **(la persona abre; el instalador llama)**

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (único proyecto abierto) y ponte en
   la **vista 3D** sombreada de siempre. No selecciones nada todavía.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "12-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "12-2 types" { .\scripts\conn-call.ps1 -Operation types }
   Anota "12-2 schema end_plate" { .\scripts\conn-call.ps1 -Operation schema -Body '{"type":"end_plate"}' }
   Anota "12-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "12-2 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "12-2 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "12-2 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "12-2 sondeo 21 etiquetas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "12-2 log de arranque" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_panel" | Select-Object -Last 4 }
   ```

   Se espera en `ping`: `addin_version: 0.12.0` y 23 operaciones; en `types`: **tres** tipos (`gusset_node`, `direct_weld`,
   `end_plate`) con su descripción; en `schema`: `connection_type: end_plate` y un `example` con `plate` y `bolts.faces`;
   `catalog_list` con `templates_count: 1`; sondeos 12, 13, 17 y 21 a cero; en el log, `"addin_version":"0.12.0"`.

### 12-3. Los tres sondeos, antes de nada **(instalador; la persona no toca Revit)**

Los tres corren **sin** transacción envolvente (`-SinTransaccion`) porque abren la suya y la **deshacen** al final; no debe
quedar nada. Si alguno imprime `PARADA` o `ERROR`, copia la salida y sigue con el siguiente: eso es el resultado.

```powershell
Anota "12-3 sondeo 22 corte plano" { .\scripts\revit-exec.ps1 -File scripts\sondeos\22-corte-plano.py -SinTransaccion -TimeoutSec 600 }
Anota "12-3 sondeo 23 pernos dos planchas" { .\scripts\revit-exec.ps1 -File scripts\sondeos\23-pernos-dos-planchas.py -SinTransaccion -TimeoutSec 600 }
Anota "12-3 sondeo 24 soldadura contorno" { .\scripts\revit-exec.ps1 -File scripts\sondeos\24-soldadura-contorno.py -SinTransaccion -TimeoutSec 600 }
Anota "12-3 sondeo 12 tras los sondeos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12-3 sondeo 13 tras los sondeos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Qué mirar y qué devolver literalmente (son **las tres decisiones de la fase**):

- **Sondeo 22**: la línea "StructuralFramingUtils, metodos estaticos: …" (toda), si `SetEndReference OK` con la extensión y
  `Cut Length` antes y después, "Caras del extremo ni paralelas ni perpendiculares al eje (corte plano inclinado): N" (**1 =
  el corte plano existe**), `RemoveEndReference` y `Transaction deshecha`; y la lista "Tipos de ASObjectsMgd que recortan vigas"
  con sus constructores. Si la diagonal no se encuentra (`PARADA: no encuentro…`): **(la persona)** selecciona el cordón 1245533
  y la diagonal 1249631 y el instalador repite ese `Anota`.
- **Sondeo 23**: el RESUMEN del final: `planchas: … WriteToDb OK`, `fila 4x1: WriteToDb OK; NumberOfScrews=4…` (o `FALLO`),
  `Connect: OK con …` (o `FALLO`), `sueltos 1x1: 4/4 escritos`; las dos listas del paso 3 (propiedades y tipos con Nut / Washer /
  Assembly) **enteras**; "Elementos nuevos con la transaccion abierta" (categorías) y "Elementos nuevos tras cerrar: 0".
- **Sondeo 24**: los constructores y enumeraciones de `WeldPattern`, `WeldLine`, `WeldPoint` **enteros**; `WeldLine con
  constructor utilizable: …`; el RESUMEN (`WeldLine: WriteToDb OK…` o `FALLO…`, `WeldPoint: …`); "Elementos nuevos tras cerrar: 0".
- Sondeos 12 y 13 **a cero** después. Si el 13 encuentra restos, los limpia: anótalo (sería que `CancelTransaction` no deshizo
  algo).

**(la persona)** Si Revit muestra algún cuadro durante los sondeos, captura `docs\fases\capturas\fase12-01-corte.png` (el cuadro o
la diagonal cortada si la ves) y `fase12-02-pernos.png` si el sondeo 23 deja algo visible antes de cancelar; si no se ve nada,
no hacen falta capturas de este paso.

### 12-4. `node_info` con la clase del nudo y la excentricidad **(instalador)**

```powershell
Anota "12-4 node_info nudo en K" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1245533,1249631,1249632]}' }
Anota "12-4 node_info empalme" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1245531,1245530]}' }
Anota "12-4 node_info Detalle D" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1249510,1249630,1249631,1249636],"chord_element_id":1249510}' }
```

Se espera en el nudo en K: `kind: node`, **`eccentricity_mm` ≈ 34** (anota el número exacto), `chord_element_id: 1245533`,
`origin_mm` en Z ≈ 19933, y por diagonal `axis_offset_mm` ≈ **−17 y +17** (con signo contrario), `axis_cut_mm`, `end_gap_mm`,
`reaches_node: true`, `width_mm` / `height_mm` / `wall_mm` del perfil (en el cordón 203,2 × 304,8 × 12,7 o al revés: **anota en
qué orden salen**). En el empalme: **`kind: splice`**, `eccentricity_mm: 0`, `origin_mm` en el punto donde se encuentran los
tramos (X ≈ −4180). En el Detalle D: `kind: node`, `eccentricity_mm: 0` y el `origin_mm` de siempre (−11867,7, −17195,8,
17423,0). Si algún ID no existe (`ELEMENT_NOT_FOUND`), usa los apuntados en "Antes de empezar" y repite.

### 12-5. Conectar en un nudo en K: arranque `direct_weld`, Crear y guardar en catálogo **(la persona)**

1. Selecciona las tres barras del nudo en K (1245533, 1249631, 1249632) y pulsa ARBA > **Conectar**. **Sin pedir ningún
   archivo** se abre la previsualización con la cabecera "Especificación de arranque de soldadura directa: cada barra cortada
   contra la cara del cordón y soldada en todo el contorno (valores por defecto de choices.json)…" (o "valores de la plantilla
   …" si hubiera una `direct_weld`), el croquis con el cordón, las dos diagonales **cortadas contra la cara inferior** desde su
   propio corte, la cota del gap entre cortes (**≈ 34**) y la marca del empalme **"empalme B a 330"** en cada diagonal; a la
   derecha el panel con Cordón (cara, excentricidad) y Barra 1 / Barra 2 (desvío, corte, holgura, soldadura, marca del empalme).
   Abajo, la validación **en verde** con los avisos `MEMBER_ECCENTRIC` (uno por diagonal) y ninguno más. Captura:
   `docs\fases\capturas\fase12-03-nudo-k.png`. Anota la cabecera literal, el gap que dibuja y los códigos de la lista.
2. En **Barra 1 > Soldadura** elige `6 mm`: el croquis cambia el rótulo de la soldadura; vuelve a `5 mm`. Cambia la **marca del
   empalme** a 300 y vuelve a 330 (es un valor del JSON: `splice_hint.offset_from_chord_face_mm`). Anota si algún error quedó
   junto a un campo.
3. Pulsa **Cambiar…**: además de las plantillas (ninguna de este tipo) debe ofrecer "Tipo: cartela rectangular (arranque)",
   "Tipo: soldadura directa (arranque)" y "Tipo: placa de extremo (arranque)". Elige **cartela rectangular**: la ventana pasa al
   arranque de la Fase 11 (ahora **debe validar o dar `MEMBER_ECCENTRIC`**, ya no `MEMBER_NOT_AT_NODE`: anótalo). Vuelve con
   Cambiar… a **soldadura directa**.
4. Pulsa **Crear** (solo con la validación en verde; si no, anota los códigos y Cancelar). Anota el `connection_id` y los avisos
   del cuadro: se espera **`END_CUT_APPROXIMATED`** (la reserva: las diagonales acortadas por su eje) y quizá `REVIT_WARNING`.
   Mira en Revit el extremo de cada diagonal (acortado, perpendicular a su eje, con la esquina fuera del cordón) y las dos
   soldaduras (DirectShape rojos sobre la cara). Captura: `docs\fases\capturas\fase12-04-nudo-k-creado.png`.
5. Vuelve a seleccionar las tres barras, **Conectar** (avisa de que ya tiene conexión), **Guardar en catálogo** con el nombre
   `Nudo K cordon superior`, y **Cancelar**. En ARBA > **Catálogo** debe aparecer con su miniatura (el croquis del nudo
   soldado) y `connection_type` `direct_weld`.
6. **(instalador)** `Anota "12-5 list tras el nudo en K" { .\scripts\conn-call.ps1 -Operation list }` y
   `Anota "12-5 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }`. Se espera `connections_count: 1`
   (`connection_type: direct_weld`) y `templates_count: 2`.

### 12-6. Conectar en un empalme: arranque `end_plate` Detalle C y Crear **(la persona)**

1. Selecciona los dos tramos del empalme (1245531 y 1245530) y pulsa **Conectar**. La cabecera: "Especificación de arranque de
   placa de extremo (empalme) con el Detalle C (valores por defecto de choices.json)…"; el croquis con la plancha octogonal de
   **430 × 330** (esquinas de 60), la huella del tubo y los **12 pernos** (4 + 4 arriba y abajo a 90, 2 + 2 a los lados a 80), y
   la sección con las dos planchas; el panel con Plancha, Pernos (una fila por cara: arriba, abajo, izquierda, derecha, el
   agujero `Ø13/16" · 20,6 mm`, tuercas a ambos lados), Tramo 1, Tramo 2 (retiros ≈ 12,7 menos el hueco que ya haya) y Empalme
   (holgura 0). Validación en verde (anota los avisos si los hay). Captura: `docs\fases\capturas\fase12-05-empalme.png`.
2. En **Pernos > arriba > borde** escribe `10`: debe aparecer `BOLT_EDGE_DISTANCE_TOO_SMALL` **junto al campo** y en la lista de
   abajo, y Crear se desactiva. Vuelve a `25`.
3. Pulsa **Crear**. Anota el `connection_id` y los avisos: se espera **`BOLT_NUTS_APPROXIMATED`** (12 pernos estándar) y quizá
   `REVIT_WARNING` ("Se crean con DirectShape" si Advance Steel rechazó el patrón de una fila: anótalo literal). Mira en Revit:
   las dos planchas perpendiculares al cordón, **el 430 en horizontal** (si sale en vertical, anótalo: es el riesgo 1 del
   informe), los pernos atravesando las dos planchas y los dos tramos retirados. Captura:
   `docs\fases\capturas\fase12-06-empalme-creado.png`.
4. **(instalador)** `Anota "12-6 list tras el empalme" { .\scripts\conn-call.ps1 -Operation list }` y
   `Anota "12-6 log del empalme" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "end_plate_built|direct_weld_member|advance_steel_bolts|advance_steel_plate" | Select-Object -Last 12 }`
   (el sondeo 12 **no** se lanza aquí: borraría las dos conexiones; se usa al final, en el 12-9). Se esperan 2 conexiones
   (`direct_weld` y `end_plate`) y en el log `end_plate_built` con `rows: 4`, `bolts: 12`, `bolt_length_mm` y
   `tube_footprint_mm` (anota los números) y `advance_steel_bolts_written` por fila (o `advance_steel_bolts_failed`: anótalo).

### 12-7. Una sola barra y la cercha desde Conectar **(la persona; pendiente desde la Fase 11)**

1. Pincha **una** diagonal del cordón central (1249630) y pulsa **Conectar**: debe pedir pinchar el cordón y las demás barras
   del nudo (Esc termina). Pincha 1249510, 1249631 y 1249636 y Esc: se abre la previsualización del **Detalle D con la plantilla
   propuesta**. **Cancelar**. Anota si el pinchado barra a barra resultó cómodo.
2. Selecciona **media cercha** (una ventana de selección con varios nudos) y pulsa **Conectar**: el cuadro "Esto parece una
   cercha (N nudos): usa Planificar lote" con el enlace "Abrir Planificar lote con esta selección"; púlsalo: se abre la ventana
   del plan con esa selección. **Descartar plan**. Anota N.

### 12-8. Planificar lote con las 64 barras de la 8c y Crear **(la persona)**

1. Pincha una barra de la cercha y **Planificar lote** → **Planificar con las 64 barras**. La cabecera debe decir algo como "Se
   crearán 39 conexiones: 16 con Nudo tipico Detalle D (8 iguales, 8 en espejo), 15 soldaduras directas (arranque) [o "con Nudo K
   cordon superior" si la plantilla del 12-5 casó], 6 empalmes (arranque Detalle C)[, 1 empalme (arranque Detalle I)]. … avisan de
   perfil distinto. … avisan de excentricidad. 7 nudos en K están sobre un empalme." con **2 `already_connected`** (el nudo en K
   y el empalme creados en 12-5 y 12-6). Anota la cabecera **literal** y la cuenta de cada estado. En el **mapa**, los empalmes
   son **rombos**; en la tabla, N9 y **N9E** (clase `empalme`), y en el detalle de N9 la línea de clase, tipo y excentricidad.
   Captura: `docs\fases\capturas\fase12-07-plan.png`.
2. Pulsa **Crear N conexiones** y confirma. Al terminar, el informe (cuántas creadas, cuántas fallaron, el tiempo). Anota los
   nudos **`failed` con su error literal** (el motivo más probable es la fabricación de un tipo nuevo). Captura:
   `docs\fases\capturas\fase12-08-lote.png`.
3. **(instalador)**:

   ```powershell
   Anota "12-8 list tras el lote" { .\scripts\conn-call.ps1 -Operation list }
   Anota "12-8 sondeo 17 tras el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "12-8 sondeo 21 tras el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
   Anota "12-8 log del lote" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_create|direct_weld_member|end_plate_built|_failed" | Select-Object -Last 80 }
   ```

   Se espera `connections_count` = 2 + las creadas (`batches` con el `plan_id`), el sondeo 17 con los marcadores de los no creados
   (o a cero si se crearon todos) y el 21 con sus etiquetas; en el log, `direct_weld_member` por diagonal y `end_plate_built` por
   empalme, y las líneas `_failed` que haya (cópialas enteras).

### 12-9. Puente, borrado y log, anotados en el archivo **(instalador; la persona no toca Revit)**

**(la persona)** Ninguna ventana del add-in abierta; la copia es el único proyecto abierto y su vista 3D está activa.

```powershell
Anota "12-9 validate fixture C" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\detalle-C.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "12-9 validate fixture K" { $spec = Get-Content -Raw -Encoding UTF8 docs\fixtures\nudo-K-cordon-superior.json | ConvertFrom-Json; .\scripts\conn-call.ps1 -Operation validate -Body (@{ spec = $spec } | ConvertTo-Json -Depth 20 -Compress) }
Anota "12-9 batch_plan N9 (4 elementos, mark false)" { .\scripts\conn-call.ps1 -Operation batch_plan -Body '{"element_ids":[1245531,1245530,<DIAGONAL IZQUIERDA DE N9>,<DIAGONAL DERECHA DE N9>],"mark":false}' }
Anota "12-9 list antes de borrar" { .\scripts\conn-call.ps1 -Operation list }
```

Para `batch_plan`, los IDs de las dos diagonales de N9 están en el detalle del nudo N9 de la ventana del paso 12-8 (o en
`resultados-fase-8c.md`); si no los tienes, pasa solo los dos tramos. Se espera en `validate` del C: `ok: true` con
`validation_token` y `calculated_values.kind: splice` (si da `SPLICE_SETBACK_TOO_SMALL`, anota el mensaje: dice cuánto hueco
mide); del K: `ok: true` con dos avisos `MEMBER_ECCENTRIC` y `eccentricity_mm` ≈ 34 (si alguno de los dos da
`already_connected`/errores por las conexiones creadas, anótalo). En `batch_plan`: **dos nudos** (`kind: node` con aviso
`NODE_AT_SPLICE` y `kind: splice`, ambos `already_connected` o `ready`), y después descarta ese plan con
`.\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"plan_id":"<PLAN_ID>"}'`. Después, **borra lo creado**: el lote
desde la ventana (**Borrar el lote**, la persona) o por el puente, y las dos sueltas con sus `connection_id` (12-5 y 12-6):

```powershell
Anota "12-9 batch_delete" { .\scripts\conn-call.ps1 -Operation batch_delete -Body '{"batch_id":"<PLAN_ID DEL 12-8>"}' }
Anota "12-9 delete nudo K" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL 12-5>"}' }
Anota "12-9 delete empalme" { .\scripts\conn-call.ps1 -Operation delete -Body '{"connection_id":"<ID DEL 12-6>"}' }
Anota "12-9 sondeo 12 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "12-9 sondeo 13 restos" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "12-9 sondeo 17 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "12-9 sondeo 21 final" { .\scripts\revit-exec.ps1 -File scripts\sondeos\21-etiquetas-addin.py -SinTransaccion -TimeoutSec 300 }
Anota "12-9 ping antes del puente" { .\scripts\conn-call.ps1 -Operation ping }
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 45
Anota "12-9 servidor 8000 escuchando" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "12-9 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "12-9 catalog_list final" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "12-9 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|ribbon_connect|ribbon_create|direct_weld_member|end_plate_built|advance_steel_|fabrication_|batch_create|batch_delete|catalog_save|_failed|ribbon_batch_window_error|plan_event_failed" | Select-Object -Last 200 }
Select-String -Path $salida -Pattern "^## 12-9" | Select-Object LineNumber, Line
```

Se espera: `batch_delete` y los dos `delete` con las barras restauradas (`restored_members_count`: las diagonales recuperan su
extensión; **anota si el extremo cortado no vuelve**), sondeos 12, 13, 17 y 21 a cero; `ping` con `document.title:
HANGAR_PRUEBA_sondeo`; `servidor 8000 escuchando` con una fila (si sale vacío, espera 30 s y repite ese `Anota`); **`Resultado:
38/38 pruebas correctas`** (35 contra 48884, con las 30b a 30e nuevas, y 31, 32 y 32b contra el puente; si falla alguna de las
30b a 30e, copia su bloque: son las nuevas y hablan con el modelo real); `templates_count: 2` (la del 12-5 se queda: la persona
decide si la borra). En el log: `"addin_version":"0.12.0"`, `ribbon_connect` con `starter: true` y el tipo, `direct_weld_member`
(con `axis_offset_mm`, `axis_setback_mm`, `face_y_mm`), `end_plate_built`, `batch_create` y **ninguna** línea
`ribbon_connect_failed`, `ribbon_create_failed`, `advance_steel_plate_failed`, `advance_steel_bolts_failed`,
`fabrication_transaction_failed`, `plan_event_failed` ni `ribbon_batch_window_error` (si las hay, cópialas enteras).
**La última línea debe listar todas las secciones `## 12-9`**; si falta alguna, repite su `Anota`.

### 12-10. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "12-10 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-12.md docs\fases\capturas
   git commit -m "Fase 12: resultados del instalador (0.12.0, sondeos 22-24, nudo en K, empalme, lote de la 8c, puente)"
   git pull --no-rebase origin main
   git push origin main
   ```

Qué devolver: `docs/fases/resultados-fase-12.md` (lo escribe `Anota`), las capturas `fase12-03` a `fase12-08` (y `01`, `02` si
hubo algo que ver) y, en el chat, lo que no quedó en el archivo: **las tres salidas de los sondeos resumidas en una línea cada
una** (corta / no corta; fila 4 × 1 sí / no, tuerca doble sí / no; `WeldLine` sí / no), las cabeceras literales de 12-5, 12-6 y
12-8, el `eccentricity_mm` medido, en qué sentido salió la plancha de 430, los `failed` del lote y cualquier cuadro inesperado.
