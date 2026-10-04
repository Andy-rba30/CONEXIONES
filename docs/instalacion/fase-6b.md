# Instalación y prueba de la ronda 6b: decimales en la tabla, cotas editables con doble clic y pernos con agarre real

Objetivo: desplegar la ronda 6b y comprobar en Revit tres cosas que la nube no puede probar: (1) que en la tabla de la
ventana se puede escribir `9` y después `12,7` en el mismo campo (antes, tras un entero, los decimales se rechazaban);
(2) que el **doble clic sobre una cota del croquis** abre un cuadro junto a la cota, Enter aplica el valor y el dibujo y
la validación reaccionan; (3) que la placa cuchilla ya apoya sobre una cara de la cartela y los **pernos atraviesan
cartela + placa** con el agarre real (19,5 mm) y una longitud calculada (44,45 mm), en vez de 45 mm fijos flotando en el
plano de la cartela. El sondeo 16 lo mide con números; las capturas lo enseñan. Una ronda, unos 30 minutos, sobre la
copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Vale Windows PowerShell
5.1 o PowerShell 7; todos los comandos van en la misma ventana, en orden. **Revit lo abre y lo cierra la persona**, no el
instalador. Los pasos marcados **(la persona)** los hace la persona en Revit; los marcados **(instalador)** van en
PowerShell. **Mientras una ventana del add-in esté abierta en Revit, el instalador no ejecuta nada** (`revit-exec.ps1` y
`conn-call.ps1` esperan a que Revit quede libre y darían tiempo agotado): espera a que la persona la cierre.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 6b-2 (`deploy.ps1` no puede sustituir la DLL si Revit la tiene cargada).
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la Fase 6 terminó con `conexiones tras borrar: 0`).
  Si no existiera: abre el original y ejecuta `.\scripts\revit-exec.ps1 -File scripts\sondeos\08-guardar-copia.py -SinTransaccion`.
- El puente MCP (ventana de `iniciar_servidor_revit.bat`) puede estar cerrado: el paso 6b-9 lo arranca.

### 6b-1. PowerShell, rama y archivo de resultados **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-6b.md"
"# Resultados de la ronda 6b`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "6b-1 git" { git log -1 --oneline }
```

La última línea debe ser un commit "Ronda 6b: ...". Si no, repite el `git pull` y anótalo.

### 6b-2. Compilar, pasar las pruebas, Revit cerrado y desplegar **(instalador)**

```powershell
Anota "6b-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "6b-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6b-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "6b-2 limits desplegado" { Select-String -Path "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json" -Pattern "length_addition_mm|length_increment_mm" }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 99` (la Fase 6 tenía 84), `6b-2 revit cerrado` vacío,
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==` y dos líneas con `length_addition_mm` y `length_increment_mm`
en el `limits.json` desplegado. Si `6b-2 revit cerrado` no está vacío, pide a la persona que cierre Revit y repite el
`deploy`. Si el build falla, **para aquí** y devuelve la salida.

### 6b-3. Abrir Revit y comprobar el add-in

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (si pregunta por el add-in
   sin firmar, *Always Load*).
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "6b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

   Se espera `ok: true`, `addin_version: 0.1.0`, `backend: advancesteel` y `document.title: HANGAR_PRUEBA_sondeo`.

### 6b-4. Abrir la ventana con el Detalle D y ver la etiqueta de los pernos **(la persona)**

1. Pestaña ARBA > **Ejecutar especificación JSON** > `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
2. Abajo debe decir en verde `Validación correcta con 2 aviso(s)` y `validation_token: ` con 16 caracteres. **Anótalos.**
3. Pulsa **Ajustar**. Junto a la placa cuchilla (barra 3), por fuera de las cotas, hay una etiqueta nueva:
   `4 pernos Ø5/8" · agarre 19,5 mm (cartela 9,5 + placa 10,0) · L 44,5 mm · placa en cara +z`. En la tabla, sección
   **Barra 3 · diagonal 1249636**, hay dos filas nuevas: **Placa: cara de la cartela** (vacía = +z) y **Pernos: longitud
   (mm)** (vacía = se calcula). La cabecera del croquis dice ahora `Doble clic en una cota: editar su valor`.
   Captura de la ventana completa: `docs\fases\capturas\fase6b-01-ventana.png`. Anota en el chat si la etiqueta y las
   dos filas están.

**Deja la ventana abierta.**

### 6b-5. Decimales después de un entero (corrección de la Fase 6) **(la persona)**

1. En la tabla, sección **Cartela**, doble clic en el valor de **Espesor (mm)** (`9.525`), escribe `9` y Enter. La
   etiqueta pasa a `cartela PL 3/8" · 9,0 mm` y aparece `LABEL_VALUE_MISMATCH` (normal: el rótulo 3/8" ya no coincide).
2. Doble clic otra vez en **Espesor (mm)**, escribe `12,7` y Enter. **Debe aceptarse**: etiqueta `cartela PL 3/8" ·
   12,7 mm`. Si abajo saliera en rojo `'thickness_mm' debe ser un número entero`, la corrección no está desplegada:
   anótalo literalmente.
3. Doble clic en **Rótulo de espesor**, escribe `1/2"` y Enter: vuelve el verde y el token es otro. Anota en el chat si
   los tres pasos fueron así.
4. Pulsa **Recargar**: la tabla vuelve a `9.525` y `3/8"`, y el token es el del paso 6b-4.

### 6b-6. Editar cotas con doble clic en el croquis **(la persona)**

1. Pasa el ratón por encima del texto `180,0` (retiro de la barra 1, diagonal 1249630): el cursor debe cambiar a una
   mano. Haz **doble clic** sobre ese texto. Junto a la cota aparece un cuadro naranja con `Retiro (mm) ·
   members[0].end_setback_mm`, el valor `180` seleccionado y la nota `Enter aplica · Esc cancela`; en la tabla queda
   seleccionada la fila **Retiro (mm)** de la barra 1 y la cota se pinta en naranja.
2. Escribe `200` y pulsa Enter. La cota pasa a `200,0`, el extremo de la barra se aleja 20 mm del punto de trabajo, la
   ranura se desplaza con ella, la tabla muestra `200` y abajo dice `Cota 'Retiro (mm)': 180 → 200 mm. Croquis
   redibujado y validación repetida.` con la validación en verde. Captura: `docs\fases\capturas\fase6b-02-cota-editada.png`.
3. Doble clic sobre `60,0` (paso de los pernos de la barra 3), escribe `10`, Enter: debe aparecer el error
   `BOLT_SPACING_TOO_SMALL` en `members[2].attachment.bolts.spacing_mm`, los pernos se juntan y **Crear** se desactiva.
   Doble clic otra vez sobre esa cota (ahora `10,0`), escribe `60`, Enter: vuelve el verde.
4. Doble clic sobre `150,0` (ranura de la barra 1), escribe `999` y pulsa **Esc**: el cuadro se cierra y nada cambia
   (la cota sigue en `150,0`). Haz doble clic de nuevo, escribe `999` y haz clic en cualquier otro sitio de la ventana:
   también se cancela. Anota si fue así.
5. Doble clic sobre `565,0` (ancho de la cartela, cota de abajo), escribe `575`, Enter. La cartela se ensancha, la cota
   dice `575,0`, en el cuadro del contorno los puntos de la izquierda pasan a `-254,42` y los de la derecha a `320,58`
   (los demás en proporción), la fila **Ancho (mm)** dice `575`, y abajo dice que el contorno se estiró en X alrededor
   del punto de trabajo. Anota en el chat el texto de la barra de estado y si la validación sigue en verde.
6. Pulsa **Recargar**: todo vuelve al fixture (`180,0`, `60,0`, `565,0`, `9.525`) y el token es el del paso 6b-4.
   Anótalo.

### 6b-7. Crear desde la ventana y medir los pernos

1. **(la persona)** Con la validación en verde (tras Recargar), pulsa **Crear**. Debe salir `Conexión modelada
   correctamente` con `Elementos geométricos creados: 9` y `Backend de fabricación utilizado: advancesteel`. **Anota el
   ID de conexión.** Cierra el diálogo.
2. **(la persona)** En la vista 3D (detalle **Fino**, estilo **Sombreado con aristas**), acércate a la placa cuchilla de la
   diagonal inferior y gira la vista hasta ver el paquete **de canto**: la cartela y la placa cuchilla deben verse como
   dos placas pegadas cara con cara (ya no en el mismo plano) y cada perno debe atravesar las dos con la cabeza por un
   lado y la tuerca por el otro, sobresaliendo poco (unos 2 cm en total), no 4 cm como en `fase6-05-nudo.png`. Captura:
   `docs\fases\capturas\fase6b-04-nudo-pernos.png`. Anota en el chat lo que ves: ¿la placa cuchilla está pegada a una
   cara de la cartela? ¿los pernos atraviesan las dos? ¿por qué lado quedan las cabezas?
3. **(instalador)** Con todas las ventanas del add-in cerradas:

   ```powershell
   Anota "6b-7 conn_list" { .\scripts\conn-call.ps1 -Operation list }
   Anota "6b-7 sondeo 16 agarre" { .\scripts\revit-exec.ps1 -File scripts\sondeos\16-pernos-agarre.py -SinTransaccion -TimeoutSec 600 }
   Anota "6b-7 log crear" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "advance_steel_plate_written|advance_steel_bolts_written|ribbon_create|ribbon_preview_dimension" | Select-Object -Last 12 }
   ```

   Se espera `connections_count: 1`. En el sondeo 16: `validate bolt_stacks` con `grip_mm: 19.525` y
   `bolt_length_mm: 44.45`; en el apartado 3, la cartela con `z -4.76 .. 4.76`, la placa cuchilla con `z 4.76 .. 14.76`
   y los pernos (`SteelProxyElement | Bolts`) con un intervalo que **cubra** `-4.76 .. 14.76` sobresaliendo unos
   milímetros por cada lado; `Bolt Length ... = 44.45 mm` y `Grip Length ... = 19.53 mm`; en el apartado 4, `cartela:
   centrada ... OK`, `placa cuchilla: ... apoya en la cara +z de la cartela OK` y `pernos: ... atraviesan cartela +
   placa OK`; y en el 5, la captura `fase6b-03-pernos-perfil.png`. Si los pernos dicen `NO cubren el paquete`, salen
   `SIN GEOMETRIA LEGIBLE` o sobresalen decenas de mm por un solo lado, **copia el bloque entero**: es el dato que decide
   hacia qué lado extiende Advance Steel el agarre desde el plano del patrón. En el log, `advance_steel_plate_written`
   de la placa cuchilla debe traer `offset_mm: 9.7625` y `advance_steel_bolts_written` debe traer
   `BindingLength=19.525`, `ScrewLength=44.45`, `grip_mm`, `plane_z_mm: -4.7625` (si alguna propiedad dice `no existe`
   o `ERROR`, cópialo).

### 6b-8. Borrar desde la cinta

1. **(la persona)** Pestaña ARBA > **Conexiones del modelo** > selecciona la fila > **Borrar seleccionada** > **Sí**.
   Abajo debe decir `Borrada ...: 9 elemento(s) eliminados y 3 barra(s) restauradas`. **Cerrar**. Anótalo.
2. **(instalador)** Con la ventana cerrada:

   ```powershell
   Anota "6b-8 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6b-8 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones en el modelo: 0`, `conexiones tras borrar: 0`, las extensiones `1249630: inicio 0.0 | fin
   68.64`, `1249631: inicio 0.0 | fin 69.2`, `1249636: inicio 0.0 | fin 0.0`, y `Elementos de acero sueltos encontrados:
   0`.

### 6b-9. La IA no se ve afectada: 19/19 por el puente **(instalador)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 15
Anota "6b-9 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 19/19 pruebas correctas`. Si alguna da `[FALLO]`, copia su bloque entero.

### 6b-10. Registro, cerrar y subir (autorizado)

```powershell
Anota "6b-10 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "ribbon_|advance_steel_" | Select-Object -Last 40 }
```

1. **(la persona)** Cierra la copia en Revit **sin guardar** (Archivo > Cerrar > *No guardar*).
2. **(instalador)** Sube resultados y capturas (`detalle-D-confirmado-corregido.json`, si existe, **no se sube**):

   ```powershell
   Remove-Item docs\fixtures\detalle-D-confirmado-corregido.json -ErrorAction SilentlyContinue
   git add docs\fases\resultados-fase-6b.md docs\fases\capturas
   git commit -m "Ronda 6b: resultados del instalador (decimales, cotas con doble clic, pernos con agarre real)"
   git pull --no-rebase origin main
   git push origin main
   ```

3. Devuelve: la salida completa de los pasos 6b-2, 6b-3, 6b-7, 6b-8 y 6b-9; las anotaciones de la persona (token de
   6b-4 y de los Recargar, si `12,7` se aceptó tras `9`, cómo se comportó el cuadro de la cota en cada punto de 6b-6, el
   texto de la barra de estado al cambiar el ancho, el ID de conexión, qué se ve en el paquete de canto y por qué lado
   quedan las cabezas, qué dijo la ventana al borrar); las cuatro capturas (`fase6b-01` a `fase6b-04`); el texto de
   cualquier ventana de error del add-in o de Revit, y si Revit se cerró de golpe en algún paso.
