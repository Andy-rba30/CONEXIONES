# Instalación y prueba de la ronda 7b: add-in 0.7.0, montante como diagonal y plantilla oficial regenerada

Objetivo: una ronda corta (unos 15 minutos) para cerrar la Fase 7 en el PC. La ronda 7 (`resultados-fase-7.md`) salió
entera como se esperaba; la 7b solo cambia tres cosas que hay que ver en Revit: el add-in pasa a la versión **0.7.0**
(`deploy.ps1` y `conn_ping` dejan de decir `0.1.0`), el fixture `detalle-D-confirmado.json` describe la barra 1249631
como `diagonal` a 45° (decisión P3: en el Hangar es una diagonal a 44,4°, así que la validación pasa de dos avisos a
**uno**), y la plantilla oficial `catalog\6abcf116-9b97-485f-b50d-2851ca0018cc.json` se vuelve a guardar desde la
conexión creada con ese fixture para que diga `diagonal` en sus tres barras (las plantillas se miden en el nudo real, no
se editan a mano). Sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`**); copia el error y sigue con el paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana
de PowerShell. Todos los comandos van en la misma ventana, en orden; si abres otra ventana, repite las tres primeras
líneas del paso 7b-1 (en la ronda 7 el primer `deploy` falló por la política de ejecución). **Revit lo abre y lo cierra
la persona**, no el instalador. Los pasos marcados **(la persona)** se hacen en Revit; los marcados **(instalador)** van
en PowerShell. **Mientras una ventana del add-in esté abierta en Revit, el instalador no ejecuta nada.**

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 7b-2.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la ronda 7 terminó con `conexiones tras borrar: 0`).
- Ten a mano la captura `docs\fases\capturas\fase7-02-ventana.png` para comparar en el paso 7b-4.

### 7b-1. Pull y archivo de resultados **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-7b.md"
"# Resultados de la ronda 7b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
Anota "7b-1 git" { git log -1 --oneline; git status --short }
```

Se espera `git status --short` vacío antes del `pull` (si muestra archivos modificados, no sigas: devuelve la lista) y un
commit "Ronda 7b: ..." o posterior.

### 7b-2. Compilar, pasar las pruebas, Revit cerrado, desplegar **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
Anota "7b-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "7b-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "7b-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "7b-2 version de la dll" { [System.Diagnostics.FileVersionInfo]::GetVersionInfo("$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\MotorConexiones.Revit.dll").FileVersion }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 131`, `7b-2 revit cerrado` vacío,
`== MotorConexiones 0.7.0.0 desplegado en Revit 2027 ==` con `Catalogo: ... (plantillas copiadas de catalog\: 0, ya
existentes: 1)` y `0.7.0.0` en la versión de la DLL. Si el build falla o sale `1 Advertencia(s)`, **para aquí** y devuelve
la salida. No hace falta `instalar-conn.ps1`: los archivos del MCP no cambiaron.

### 7b-3. Abrir Revit, ping y catálogo

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`. En la pestaña **ARBA**, pasa
   el ratón por **Ejecutar especificación JSON**: la descripción larga debe empezar por `MotorConexiones 0.7.0`.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "7b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "7b-3 catalog_list antes" { .\scripts\conn-call.ps1 -Operation catalog_list }
   ```

   Se espera `addin_version: 0.7.0` (en `data` y en `meta`) con las 18 operaciones, y en `catalog_list`
   `templates_count: 1` con el patrón todavía `diagonal 136,9° +Y · vertical 44,4° +Y · diagonal -135,6° -Y` (la
   plantilla vieja; se regenera en 7b-5).

### 7b-4. Crear el Detalle D con el fixture nuevo **(la persona)**

1. ARBA > **Ejecutar especificación JSON** > `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
2. Abajo debe decir en verde `Validación correcta con 1 aviso(s)`: solo `members[0].expected_angle_deg` (45° frente a
   43,1°). El aviso de `members[1]` (90° frente a 44,4°) ya no está. En el croquis la barra de arriba a la derecha se
   rotula `diagonal 1249631 · HSS2-1/2X2-1/2X3/16 · 44.4°`; el dibujo es el mismo que `fase7-02-ventana.png`. **Anota
   los 16 caracteres del token** (será distinto de `bca1ce7b8e68625d`). Captura: `docs\fases\capturas\fase7b-01-ventana.png`.
3. Pulsa **Crear**: `Conexión modelada correctamente` con `Elementos geométricos creados: 9`. **Anota el ID de conexión.**
   Cierra el diálogo.
4. Vista 3D, detalle **Fino**, sombreado; mira la placa cuchilla **de canto** (como en `fase6b-04-nudo-pernos.png`) y
   anota en el chat de qué lado de la cartela apoya respecto a la nave (es la pregunta P2 de la Fase 7; no bloquea nada).
   Captura: `docs\fases\capturas\fase7b-02-canto.png`. Cierra cualquier ventana del add-in.

### 7b-5. Regenerar la plantilla oficial desde la conexión y borrar la conexión **(instalador, ventanas cerradas)**

Sustituye `<ID>` por el ID de conexión del paso 7b-4:

```powershell
$id = "<ID>"
Anota "7b-5 catalog_save overwrite" { .\scripts\conn-call.ps1 -Operation catalog_save -Body ('{"connection_id":"' + $id + '","name":"Nudo tipico Detalle D","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","tags":["hangar","cercha","HSS"],"overwrite":true,"copy_to_shared":true}') }
Anota "7b-5 catalog_list despues" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "7b-5 git status" { git status --short; git diff --stat }
Anota "7b-5 delete conexion" { .\scripts\conn-call.ps1 -Operation delete -Body ('{"connection_id":"' + $id + '"}') }
Anota "7b-5 conn_list" { .\scripts\conn-call.ps1 -Operation list }
```

Se espera en `catalog_save`: `ok: true`, **el mismo** `template_id` `6abcf116-9b97-485f-b50d-2851ca0018cc` (al
sobrescribir por nombre se conserva), `file` y `shared_file` como en la ronda 7, `member_pattern` con `role: diagonal`
en las **tres** ranuras (ángulos 136,9 / 44,4 / −135,6 como antes) y **un** aviso `ANGLE_DIFFERS_FROM_MODEL`; en
`catalog_list`, `templates_count: 1` y el patrón `diagonal 136,9° +Y · diagonal 44,4° +Y · diagonal -135,6° -Y`; en
`git status`, una sola línea ` M catalog/6abcf116-9b97-485f-b50d-2851ca0018cc.json`; en `delete`,
`deleted_elements_count: 9`; en `list`, `connections_count: 0`. Si `catalog_save` devuelve `TEMPLATE_EXISTS`, copia el
bloque: significa que `overwrite` no llegó en el cuerpo.

### 7b-6. Restos, cerrar y subir (autorizado)

```powershell
Anota "7b-6 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "7b-6 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "7b-6 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "startup|catalog_save|ribbon_create|ribbon_preview_opened" | Select-Object -Last 10 }
```

Se espera `conexiones en el modelo: 0` (o `1` si el borrado de 7b-5 falló, y entonces el sondeo la borra),
`Elementos de acero sueltos encontrados: 0` y, en el log, `"event":"startup","addin_version":"0.7.0"`.

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)** Sube los resultados, las capturas y la plantilla regenerada, y devuelve el estado literal:

   ```powershell
   Anota "7b-6 git status antes del commit" { git status --short }
   git add catalog\*.json docs\fases\resultados-fase-7b.md docs\fases\capturas
   git commit -m "Ronda 7b: resultados del instalador (add-in 0.7.0, montante como diagonal y plantilla oficial regenerada)"
   git pull --no-rebase origin main
   git push origin main
   git status --short
   git log -1 --oneline
   ```

3. Devuelve: la salida completa de los pasos 7b-1, 7b-2, 7b-3, 7b-5 y 7b-6; el `git status --short` literal (antes del
   commit y al final, que debe quedar vacío); las anotaciones de la persona (token, si la validación dijo `1 aviso(s)`, el
   ID de conexión, de qué lado apoya la placa cuchilla); las dos capturas; y el texto de cualquier ventana de error del
   add-in o de Revit. No toques nada de `src\`, `config\`, `mcp\` ni `docs\fixtures\`.
