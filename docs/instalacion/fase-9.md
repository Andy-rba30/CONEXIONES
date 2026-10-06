# Instalación y prueba de la Fase 9: add-in 0.9.0, crear el lote

Objetivo: desplegar el add-in **0.9.0** (`docs/fases/fase-9.md`) y comprobar sobre una **copia** del modelo: (1) el sondeo
20 (grupos de transacción anidados con la sesión de Advance Steel: decide si el lote puede ser una sola entrada de
deshacer); (2) lo que la Fase 8 dejó sin anotar (`fase-8.md` 8.5 y 12.5); (3) los botones de *Qué hacer* y el consejo del
empalme; (4) **Crear 16 conexiones** desde la ventana del plan, **Ctrl+Z**, **Borrar el lote** y los sondeos 12 y 13 a
cero; (5) `conn_batch_create`, `conn_list` por lote y `conn_batch_delete` por el puente; (6) el sondeo 19 v4; (7)
`probar_conexiones.py --puente` 30/30 y el log. Unos 75 minutos sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. **Esta fase crea acero de verdad en la copia** (y lo borra al final).

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no
toca `src\`**); copia el error y sigue con el siguiente. La única excepción, autorizada aquí, es el paso 9-2.3: si el sondeo
20 dice "GRUPOS ANIDADOS CON PROBLEMAS", el instalador pone `"batch_single_undo": false` en el `catalog.json` **desplegado**
(no en el del repositorio). No crees scripts nuevos; usa `Anota` en la ventana de PowerShell. Todos los comandos en la misma
ventana, en orden; si abres otra, repite las tres primeras líneas del paso 9-1. **Revit lo abre y lo cierra la persona.** Los
pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van en PowerShell. Mientras la ventana del
plan está creando o borrando ("⏳ …"), el instalador no ejecuta nada. La variable del plan se llama **`$plan`**. **Si Revit se
cerrara** en algún paso: anota en qué botón, captura del cuadro, vuelve a abrir Revit y la copia y sigue con el siguiente; el
log del paso 9-8 dirá dónde se quedó.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 9-1.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in ni marcadores (la 8e terminó con 0 y 0). Si no estás
  seguro, los sondeos 12 y 13 del paso 9-2 lo dicen y limpian.
- Las plantillas del catálogo en su sitio (`templates_count: 1`, `Nudo tipico Detalle D`).
- Para el paso 9-4 se selecciona **la cercha de la 8c** (64 elementos: el cordón central en ocho tramos, sus 48 diagonales y
  los ocho tramos del cordón superior). Si el cordón inferior existe como barra de armazón estructural, selecciónalo
  también (es lo pendiente de la 8d) y anota cuántos elementos quedan.

### 9-1. Pull, archivo de resultados, build, test, deploy e instalar-conn **(instalador, Revit cerrado)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-9.md"
"# Resultados de la Fase 9`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
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
Anota "9-1 git" { git log -1 --oneline; git status --short }
Anota "9-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "9-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "9-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "9-1 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "9-1 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
Anota "9-1 catalog.json desplegado" { Get-Content "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json" | Select-String "batch_single_undo|node_face_reach_mm" }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista), un
commit "Fase 9: ..." o posterior, `0 Advertencia(s)`, `0 Errores`, **`Superado: 192`**, `9-1 revit cerrado` vacío,
`== MotorConexiones 0.9.0.0 desplegado en Revit 2027 ==` (con `config\catalog.json` entre los copiados),
`copiado revit_mcp\conexiones.py (25 rutas @api.route)`, `copiado tools\conn_tools.py (23 herramientas @mcp.tool)`,
`0.9.0.0` en la versión de la DLL y `"batch_single_undo": true` en el `catalog.json` desplegado. Si el build falla o sale
`1 Advertencia(s)`, **para aquí** y devuelve la salida.

### 9-2. Abrir Revit, ping, restos y el sondeo 20 (grupos anidados) **(la persona abre; el instalador llama)**

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` y ponte en la **vista 3D**
   sombreada de siempre. No selecciones nada todavía y no abras ninguna ventana del add-in.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "9-2 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "9-2 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
   Anota "9-2 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "9-2 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   Anota "9-2 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
   Anota "9-2 sondeo 20 grupos anidados" { .\scripts\revit-exec.ps1 -File scripts\sondeos\20-grupos-anidados.py -SinTransaccion -TimeoutSec 900 }
   ```

   Se espera en `ping`: `addin_version: 0.9.0` (en `data` y en `meta`) y **23 operaciones** (con `batch_create` y
   `batch_delete`). En `catalog_list`, `templates_count: 1`. Sondeos 12 y 13: `conexiones en el modelo: 0` y
   `Elementos de acero sueltos encontrados: 0` (si no, los dejan a cero). Sondeo 17 entero, con `Marcadores de plan … : 0`.
   **Sondeo 20** (puede tardar varios minutos: dos creaciones del Detalle D con Advance Steel): `1) Antes: conexiones=0`,
   `2) PARTE A`, `create dentro del grupo exterior: ok=True`, `delete dentro del grupo exterior: ok=True`,
   `exterior.Assimilate() -> Committed`, `3) Tras A: conexiones=0`; `4) PARTE B`, `create ... ok=True`,
   `exterior.RollBack() -> RolledBack`, `5) Tras B: conexiones=0 | acero suelto=<el de antes> | extensiones=<las de antes>`
   y **`6) RESULTADO: GRUPOS ANIDADOS OK: deja batch_single_undo en true`**. **Copia el bloque entero** pase lo que pase.
3. **(instalador, solo si el paso 6 del sondeo dice "GRUPOS ANIDADOS CON PROBLEMAS" o alguna parte dice FALLO)**: plan B
   autorizado. Cambia la llave en el `catalog.json` **desplegado** y anótalo:

   ```powershell
   $cfg = "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json"
   (Get-Content $cfg -Encoding UTF8) -replace '"batch_single_undo": true', '"batch_single_undo": false' | Set-Content $cfg -Encoding UTF8
   Anota "9-2 plan B batch_single_undo" { Get-Content $cfg | Select-String "batch_single_undo"; .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900; .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   (el add-in lee el archivo en cada lote: no hace falta reiniciar Revit). Si el sondeo dejó una conexión o acero suelto,
   ese mismo bloque lo limpia.
4. **(la persona)** Mira el menú **Deshacer** de Revit (la flecha junto al icono): tras la parte A debe haber **una sola**
   entrada "Sondeo 20 A: lote de prueba" (no dos ni tres), y nada de la parte B. Anótalo. **No deshagas nada.**

### 9-3. Lo pendiente de la Fase 8 por el puente (8.5 y 12.5) **(la persona selecciona, el instalador llama)**

1. **(la persona)** Selecciona la cercha de la 8c (64 elementos, más el cordón inferior si existe) y deja la selección puesta.
2. **(instalador)**:

   ```powershell
   $tid = "6abcf116-9b97-485f-b50d-2851ca0018cc"
   Anota "9-3 batch_plan" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera `ok: true`, el `plan_id` (**cópialo**: es `$plan`), `summary_text` como en la 8c ("Se crearán 16 conexiones con
   Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 10 sin plantilla que encaje. 7 … "; con la
   0.9.0 los 7 del cordón superior donde acaban dos tramos salen como **`✖ Empalme del cordón`** y en `summary_text` "7
   empalmes del cordón (sin plantilla)" en vez de "7 con el cordón sin seleccionar"), y lo de la 0.8.2 (8.5): **`end_gap_mm`
   en `members[]`** (N4: 84,5 / 19,6 / 48,1 ±1) y las claves nuevas de la Fase 9: `creatable_count: 16`, `created_count:
   0`, `has_batch_connections: false`, `last_report: null`, y en cada nudo `actions` (por ejemplo `[{"key":"exclude",
   "label":"Excluir"}]` en los de aviso de perfil, `[{"key":"exclude","label":"Excluir"}]` en los empalmes, `exclude` +
   `template` en los "sin plantilla que encaje"). Si el cordón inferior entró, anota cuántos nudos de abajo cambian de
   estado respecto a la 8c (lo pendiente de la 8d).
3. **(instalador)** Con `<N>` = el nudo gemelo del Detalle D (N4 si la numeración no cambió) y `<E>` = uno de los empalmes
   (`✖ Empalme del cordón`; N9 en la 8c):

   ```powershell
   $plan = "<plan_id>"
   Anota "9-3 batch_plan_get N" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan + '","node":"<N>"}') }
   Anota "9-3 batch_plan_get empalme" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan + '","node":"<E>"}') }
   Anota "9-3 replan con overrides devuelto" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{},"include_specs":false}') -TimeoutSec 600 }
   Anota "9-3 replan excluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"exclude":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
   Anota "9-3 replan incluir" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"plan_id":"' + $plan + '","overrides":{"include":["<N>"]},"include_specs":false}') -TimeoutSec 600 }
   ```

   Se espera: `<N>` con su token; el empalme con `status_text` `✖ Empalme del cordón` y `advice` "El cordón termina en este
   nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye" (**nunca** "selecciónalo y
   replanifica"); el replan con el `overrides` devuelto `ok: true` (0.8.2: sin `IsEmpty`); excluir → `◌ Excluido` gris en el
   modelo; incluir → **el mismo token** que en el paso 2. Deja el plan puesto y marcado.

### 9-4. La ventana: Qué hacer con botones, lo pendiente de 12.5, Crear 16 conexiones, Ctrl+Z, Borrar el lote **(la persona)**

1. Con la cercha seleccionada, ARBA > **Planificar lote**. Barra de estado con `PLAN_MARKS_REPLACED` (el plan del puente
   pierde las marcas; 0.8.2) y un solo juego de marcas. El título dice "MotorConexiones: conectar cercha (plan y lote)" y
   abajo a la derecha, en negrita, **Crear 16 conexiones** (encendido). Captura de la ventana entera:
   `docs\fases\capturas\fase9-01-ventana-lote.png`.
2. **Qué hacer con botones (C3)**: en la tabla, debajo de cada frase hay botones pequeños. En un nudo `▲ Listo con aviso`:
   **Excluir** (púlsalo: la fila pasa a `◌ Excluido`, gris en el modelo, y el botón pasa a **Incluir**; púlsalo para
   volver). En el empalme (N9 en la 8c): la frase "El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2
   diagonales; crea esa típica o excluye" y el botón **Excluir** (no lo pulses). En uno `✖ Sin plantilla que encaje`:
   **Excluir** y **Plantilla…** (pulsa Plantilla… y **Cancelar**: es lo pendiente de 12.5 "Plantilla… cancelado"; anota si
   la ventana sigue normal). Clic derecho sobre el empalme > **Cordón…** > elige el otro tramo > Aceptar: el nudo sigue
   siendo `✖ Empalme del cordón` con el mismo consejo (12.1).
3. **Lo pendiente de 12.5**: **Editar nudo** sobre el Detalle D con la ventana abierta (se abre la previsualización modal,
   cabecera "Nudo N… del plan (… ). Crear se hace con el botón Crear N conexiones"; Cancelar). **Planificar lote** con la
   ventana abierta y **solo las cuatro barras del Detalle D** seleccionadas: la misma ventana se actualiza (1 listo), no
   se abre otra; luego sin selección: solo viene delante. Vuelve a seleccionar la cercha entera y **Planificar lote**.
   **Ver en Revit** sobre N4 y doble clic en el círculo 4 del mapa: anota literalmente si sale **algún cuadro** (no debe).
4. **Crear 16 conexiones**: púlsalo. Sale el cuadro de confirmación con la cabecera ("Se crearán 16 conexiones con Nudo
   tipico Detalle D…"), la explicación (cada nudo por separado; Ctrl+Z deshace el lote entero) y "¿Crear ahora?". Pulsa
   **Sí**. La ventana se apaga con "⏳ Creando 16 conexión(es) en Revit (puede tardar varios minutos)…". **Espera** (16
   sesiones de Advance Steel: cuenta el tiempo). Al terminar: la cabecera dice "Creadas 16 conexiones (14 con aviso)." (o
   "Creadas 15 …, 1 falló" si alguna falló), las filas pasan a `✔ Creada` / `✔ Creada con aviso` (o `✖ Falló al crear` con
   el motivo y los botones Ver en Revit y Excluir), la barra de estado lleva "Lote …: 16 conexiones creadas (16 con aviso).
   Una sola entrada de deshacer (Ctrl+Z). N s." y el botón pasa a **Crear 0 conexiones** (apagado) y aparece **Borrar el
   lote**. En el modelo: las cartelas, placas y pernos en los 16 nudos, **sin** color ni cubo en esos nudos; los rojos y
   grises siguen marcados. Captura de la cercha: `docs\fases\capturas\fase9-02-lote-creado.png` y de la ventana:
   `docs\fases\capturas\fase9-03-informe.png`. Anota: cuánto tardó, cuántas creadas, cuántas fallaron y el texto de la
   barra de estado. **Si alguna falló**, elige su fila, copia el detalle (panel de abajo) y pulsa **Crear 1 conexión (1
   reintento)**: anota si salió a la segunda.
5. **(instalador)** Con la ventana abierta y quieta:

   ```powershell
   Anota "9-4 list tras crear" { .\scripts\conn-call.ps1 -Operation list -Body ('{"batch_id":"' + $plan + '"}') }
   Anota "9-4 batch_plan_get tras crear" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan + '","include_specs":false}') -TimeoutSec 300 }
   ```

   Ojo: el plan del botón es **otro** `plan_id` (el de la ventana; la cabecera lo enseña abreviado y el log lo tiene
   entero): si `list` dice `connections_count: 0` con `$plan`, repite con el `plan_id` de la ventana (`batches` en la
   respuesta lista los lotes con su cuenta: **cópialo**, es `$lote`). Se espera `connections_count: 16` (o las creadas),
   cada una con `batch_id` y `template_id`, y en `batch_plan_get` del plan de la ventana `created_count: 16`,
   `has_batch_connections: true`, `last_report` con `created_count` y `summary_text`, y `status_text` `✔ Creada…` por nudo.
6. **(la persona) Deshacer con Ctrl+Z**, una sola vez, con el foco en Revit: **las 16 conexiones desaparecen a la vez** y las
   marcas de esos nudos **vuelven** (eran la última transacción del mismo grupo). Mira el menú Deshacer: la entrada debe ser
   una sola ("MotorConexiones: batch_create …"). Anota qué pasó. Si hizo falta más de un Ctrl+Z (plan B `per_node` o grupo
   exterior que no se pudo abrir), anota cuántos.
7. **(instalador)** Tras el Ctrl+Z:

   ```powershell
   Anota "9-4 sondeo 12 tras ctrl+z" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "9-4 sondeo 13 tras ctrl+z" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones en el modelo: 0` y `Elementos de acero sueltos encontrados: 0` (el deshacer de Revit también quita
   los elementos de Advance Steel). Si el sondeo 12 encuentra conexiones, es que Ctrl+Z no deshizo el lote entero: anota
   cuántas quedaron (el sondeo las borra).
8. **(la persona)** En la ventana del plan (que sigue abierta, pero ya no sabe del Ctrl+Z): **Replanificar**. Los nudos
   vuelven a `● Listo` / `▲ Listo con aviso` (las conexiones ya no están) y el botón a **Crear 16 conexiones**. Púlsalo otra
   vez y confirma: segundo lote (anota el tiempo). Después **Borrar el lote** (botón de abajo o **Más…**) > Sí: la barra de
   estado dice "Lote …: 16 conexiones borradas (… elementos, … barras restauradas). Una sola entrada de deshacer (Ctrl+Z).
   Replanificado." y la cercha vuelve a tener sus 16 marcas. Mira el menú Deshacer: una entrada "MotorConexiones:
   batch_delete …". **No deshagas.** Captura: `docs\fases\capturas\fase9-04-lote-borrado.png`.
9. **(instalador)**:

   ```powershell
   Anota "9-4 sondeo 12 tras borrar el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "9-4 sondeo 13 tras borrar el lote" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera 0 y 0, y en el sondeo 12 las extensiones de las barras del fixture como antes (68,6 / 69,2 en 1249630 /
   1249631 vienen de la 7b; las del lote son las de la cercha gemela, que no imprime).
10. **(la persona)** **Más… > Descartar plan** > Sí: la ventana se cierra y no queda ningún cubo. Lo pendiente de la 8d
    (Descartar con un plan marcado en **otra** vista): abre otra vista 3D o un alzado, selecciona la cercha, **Planificar
    lote** (marca en esa vista), vuelve a la {3D} de siempre, **Planificar lote** otra vez (marca aquí; `PLAN_MARKS_REPLACED`)
    y **Más… > Descartar plan**: en las dos vistas no debe quedar ningún cubo ni color. Anótalo.

### 9-5. El lote por el puente: crear dos nudos, listar por lote, saltar, token alterado, borrar **(instalador; la persona selecciona)**

1. **(la persona)** Selecciona la cercha entera otra vez (sin ventanas del add-in abiertas).
2. **(instalador)** Planifica sin marcas, crea **dos** nudos con sus tokens y comprueba:

   ```powershell
   Anota "9-5 batch_plan sin marcas" { .\scripts\conn-call.ps1 -Operation batch_plan -Body ('{"template_ids":["' + $tid + '"],"mark":false,"include_specs":false}') -TimeoutSec 600 }
   ```

   Copia el `plan_id` nuevo (`$plan2`) y, de dos nudos listos (N4 y N7 si la numeración no cambió), su `validation_token`
   (`$t4`, `$t7`; 64 caracteres, sin espacios). Después:

   ```powershell
   $plan2 = "<plan_id>"; $t4 = "<token de N4>"; $t7 = "<token de N7>"
   Anota "9-5 batch_create dos nudos" { .\scripts\conn-call.ps1 -Operation batch_create -Body ('{"plan_id":"' + $plan2 + '","nodes":[{"node":"N4","validation_token":"' + $t4 + '"},{"node":"N7","validation_token":"' + $t7 + '"}]}') -TimeoutSec 1800 }
   Anota "9-5 list por lote" { .\scripts\conn-call.ps1 -Operation list -Body ('{"batch_id":"' + $plan2 + '"}') }
   Anota "9-5 batch_create otra vez (saltados)" { .\scripts\conn-call.ps1 -Operation batch_create -Body ('{"plan_id":"' + $plan2 + '","nodes":[{"node":"N4","validation_token":"' + $t4 + '"},{"node":"N7","validation_token":"' + $t7 + '"}]}') -TimeoutSec 1800 }
   Anota "9-5 batch_create token alterado" { .\scripts\conn-call.ps1 -Operation batch_create -Body ('{"plan_id":"' + $plan2 + '","nodes":[{"node":"N11","validation_token":"' + ("0" * 64) + '"}]}') -TimeoutSec 1800 }
   Anota "9-5 batch_create sin token" { .\scripts\conn-call.ps1 -Operation batch_create -Body ('{"plan_id":"' + $plan2 + '","nodes":["N11"]}') }
   Anota "9-5 batch_plan_get tras el lote" { .\scripts\conn-call.ps1 -Operation batch_plan_get -Body ('{"plan_id":"' + $plan2 + '","include_specs":false}') -TimeoutSec 300 }
   Anota "9-5 batch_delete" { .\scripts\conn-call.ps1 -Operation batch_delete -Body ('{"batch_id":"' + $plan2 + '"}') -TimeoutSec 1800 }
   Anota "9-5 list tras borrar" { .\scripts\conn-call.ps1 -Operation list }
   Anota "9-5 batch_delete otra vez (vacio)" { .\scripts\conn-call.ps1 -Operation batch_delete -Body ('{"batch_id":"' + $plan2 + '"}') -TimeoutSec 1800 }
   Anota "9-5 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "9-5 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera: `batch_create` con `ok: true`, `created_count: 2`, `undo_entries: "one"`, dos `connection_ids`, `nodes[]`
   con `outcome` `created_with_warnings` (N4 y N7 tienen el aviso de perfil), `summary_text` "Lote …: 2 conexiones creadas
   (2 con aviso). Una sola entrada de deshacer (Ctrl+Z). N s." y `plan_summary_text` "Creadas 2 conexiones (2 con aviso).
   Quedan 14 listas sin crear. …"; `list` por lote: `connections_count: 2` con `batch_id` = `$plan2`; la segunda llamada:
   `created_count: 0`, `skipped_count: 2` con `reason` "ya creada en este lote (conexión …)"; el token alterado: `ok: true`,
   `failed_count: 1`, `nodes[0].errors[0].code` `VALIDATION_TOKEN_INVALID`, aviso `BATCH_NODE_FAILED`, **sin** cambios en el
   modelo; sin token: `ok: false`, `INVALID_REQUEST` con "viene sin validation_token"; `batch_plan_get`: N4 y N7 `created`
   (`✔ Creada con aviso`, `created_connection_id`), N11 `failed` (`✖ Falló al crear`, consejo "replanifica (el modelo o el
   plan cambiaron)…"), `last_report`; `batch_delete`: `deleted_count: 2`, `summary_text` "Lote …: 2 conexiones borradas (…
   elementos, 6 barras restauradas). Una sola entrada de deshacer (Ctrl+Z)."; `list` después: 0 del lote; el segundo
   `batch_delete`: `ok: true`, `deleted_count: 0` y aviso `BATCH_EMPTY`; sondeos 12 y 13 a cero.
3. **(la persona)** Mira el menú Deshacer: el lote del puente debe ser **una** entrada "MotorConexiones: batch_create …" y el
   borrado otra "MotorConexiones: batch_delete …". Anótalo. No deshagas.

### 9-6. Sondeo 19 v4: etiqueta B y clic sin cuadro **(instalador y la persona; de `fase-8.md` 12.7)**

1. **(la persona)** Ponte en la vista 3D con el Detalle D de la Fase 3 (Y ≈ −17 196) bien visible, a media distancia.
   Ninguna ventana del add-in abierta.
2. **(instalador)**:

   ```powershell
   Anota "9-6 sondeo 19 v4" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera: `0) archivo de clics vaciado`, `4)` cuatro BMP de 24 bits (A, B y sus versiones naranjas), `5)` las dos
   posiciones, `6)` **dos** controles (índices 0 y 1) y `GetAll(): 2`, `9)` "NO sale ningun cuadro".
3. **(la persona)** Mira la 4 verde en N4 y la B azul; **pincha la 4**: NO debe salir ningún cuadro y la 4 pasa a naranja.
   Captura a mano `docs\fases\capturas\fase9-05-etiqueta-v4.png`. Anota cuál de las dos etiquetas se vio y si la 4 cambió de
   color. Avisa al instalador.
4. **(instalador)**:

   ```powershell
   Anota "9-6 sondeo 19b" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
   ```

   Se espera `clic en una etiqueta: Index=0`, `UpdateControl(0): la etiqueta A pasa a naranja (el clic llego)`,
   `GetAll() antes de quitar: 2`, los dos quitados y `GetAll() despues de quitar: 0`. Tiene que responder al momento (nada
   de 300 s: ya no hay cuadro que bloquee a Revit).

### 9-7. Restos **(instalador, ventanas cerradas)**

```powershell
Anota "9-7 batch_plan_discard all" { .\scripts\conn-call.ps1 -Operation batch_plan_discard -Body '{"all":true}' }
Anota "9-7 sondeo 17 marcadores" { .\scripts\revit-exec.ps1 -File scripts\sondeos\17-marcas-plan.py -SinTransaccion -TimeoutSec 300 }
Anota "9-7 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "9-7 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera `remaining_markers: 0`, `Marcadores de plan … : 0`, `conexiones en el modelo: 0` y `Elementos de acero sueltos
encontrados: 0`. Si el sondeo 12 tarda más de un minuto, anota qué tenía Revit abierto (en la 8b y la 8e fue un cuadro).

### 9-8. Puente y log, anotados en el archivo **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 30
Anota "9-8 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "9-8 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_create|batch_delete|batch_node|batch_plan|ribbon_batch|plan_event|startup" | Select-Object -Last 120 }
Select-String -Path $salida -Pattern "^## 9-8" | Select-Object LineNumber, Line
```

Se espera **`Resultado: 30/30 pruebas correctas`** (la 24 dice "el nudo falla (VALIDATION_TOKEN_INVALID), nada creado", la
25 `BATCH_EMPTY`, la 29 "trae las 23 herramientas conn_*"; la 29 necesita el servidor arrancado: si falla sola, repite el
`Anota` del puente pasados otros 30 s). En el log, `"addin_version":"0.9.0"`, `batch_node_created` por nudo (16 + 2 + los
reintentos), `batch_create` con `created`, `failed`, `undo_entries` y `duration_ms`, `batch_delete` con `deleted`,
`ribbon_batch_create`, `ribbon_batch_delete`, `ribbon_batch_advice_action` (los botones de Qué hacer que pulsaste) y
**ninguna** línea `ribbon_batch_window_error`, `plan_event_failed` ni `batch_create_rolled_back` (si las hay, cópialas
enteras). **La última línea debe listar dos secciones `## 9-8`**; si falta alguna, repite su `Anota`.

### 9-9. Cerrar y subir (autorizado)

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)**:

   ```powershell
   Anota "9-9 git status antes del commit" { git status --short }
   git add docs\fases\resultados-fase-9.md docs\fases\capturas
   git commit -m "Fase 9: resultados del instalador (0.9.0, sondeo 20, lote creado y borrado, puente)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 9-1, 9-2, 9-3, 9-5, 9-6, 9-7 y 9-8 (con el bloque entero del sondeo 20); el
   `git status --short` literal (antes del commit y al final, que debe quedar vacío); las anotaciones de la persona del paso
   9-2.4 (el menú Deshacer tras el sondeo 20), del 9-4 (botones de Qué hacer, el consejo del empalme, Plantilla… cancelado,
   Editar nudo y Planificar lote con la ventana abierta, Ver en Revit sin cuadro, cuánto tardó Crear 16 conexiones, cuántas
   se crearon y cuántas fallaron con su motivo, qué hizo Ctrl+Z y cuántas entradas había en Deshacer, Borrar el lote,
   Descartar con un plan marcado en otra vista, y si el cordón inferior existe como barra), del 9-5.3 y del 9-6.3; las
   cinco capturas; y el texto de cualquier ventana de error. No toques nada de `src\`, `config\` (salvo el plan B del paso
   9-2.3, en la carpeta desplegada), `mcp\` ni `docs\fixtures\`.
