# Instalación y prueba de la Fase 7: catálogo de conexiones

Objetivo: desplegar el add-in de la Fase 7 y comprobar en Revit lo que la nube no puede probar: que el marco canónico del
nudo y los ángulos con signo salen como se espera y que la cartela del Detalle D queda como en el plano; que una conexión
creada se guarda como plantilla (`catalog_save`) y se vuelve a aplicar al mismo nudo (`catalog_apply`, orientación `same`)
y al nudo simétrico de la otra mitad de la cercha (`mirror_x`); que el botón **Catálogo** y los botones nuevos de la
ventana funcionan; y que las rutas nuevas pasan `probar_conexiones.py`. Una ronda, unos 40 minutos, sobre la copia
`D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Todos los comandos van en
la misma ventana, en orden. **Revit lo abre y lo cierra la persona**, no el instalador. Los pasos marcados
**(la persona)** se hacen en Revit; los marcados **(instalador)** van en PowerShell. **Mientras una ventana del add-in
esté abierta en Revit, el instalador no ejecuta nada** (`revit-exec.ps1` y `conn-call.ps1` esperarían y darían tiempo
agotado).

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 7-2.
- La copia `HANGAR_PRUEBA_sondeo.rvt` sin conexiones del add-in (la ronda anterior terminó con `conexiones en el modelo: 0`).
- Ten a mano el plano `docs\fixtures\detalle-D.png` y la captura `docs\fases\capturas\fase6-05-nudo.png` para comparar en el
  paso 7-4.
- Para el paso 7-7 la persona elige **otro nudo de la misma cercha**: el simétrico del Detalle D en la otra mitad (mismo
  cordón, una diagonal arriba a cada lado y una diagonal abajo con placa cuchilla). Si no lo hay, cualquier nudo con tres
  barras parecidas; el resultado se anota igual.

### 7-0. Llevar la rama de la Fase 7 a `main` **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git fetch origin
git checkout main
git pull --no-rebase origin main
git merge --no-edit origin/claude/fervent-allen-bxmdt1
git push origin main
git log -3 --oneline
```

Se espera `git status --short` vacío (si muestra archivos modificados, no sigas: devuelve la lista), un merge sin
conflictos (`Fast-forward` o `Merge made by`) y, al final, `Fase 7: ...` entre los tres últimos commits. Si `git merge`
dice `CONFLICT`, ejecuta `git merge --abort` y devuelve la salida completa sin tocar nada más.

### 7-1. Archivo de resultados **(instalador)**

```powershell
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-7.md"
"# Resultados de la Fase 7`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "7-1 git" { git log -1 --oneline }
```

La última línea debe ser un commit "Fase 7: ...". Si no, repite el `git pull` y anótalo.

### 7-2. Compilar, pasar las pruebas, Revit cerrado, desplegar e instalar las rutas nuevas **(instalador)**

```powershell
Anota "7-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "7-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "7-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
Anota "7-2 instalar-conn" { .\mcp\instalar-conn.ps1 }
Anota "7-2 catalog.json desplegado" { Get-Content -Encoding UTF8 "$env:APPDATA\Autodesk\Revit\Addins\2027\MotorConexiones\config\catalog.json" }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 129`, `7-2 revit cerrado` vacío,
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==` con `config\catalog.json` entre los copiados y una línea
`Catalogo: C:\Users\...\AppData\Local\MotorConexiones\catalogo (plantillas copiadas de catalog\: 0, ya existentes: 0)`,
y en `instalar-conn` `copiado revit_mcp\conexiones.py (20 rutas @api.route)` y
`copiado tools\conn_tools.py (18 herramientas @mcp.tool)`. Si el build falla, **para aquí** y devuelve la salida.

### 7-3. Abrir Revit, ping y el marco canónico del nudo

1. **(la persona)** Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`. En la pestaña **ARBA**, el
   panel **MotorConexiones** debe tener **tres** botones: **Ejecutar especificación JSON**, **Conexiones del modelo** y
   **Catálogo**. Captura: `docs\fases\capturas\fase7-01-cinta.png`.
2. **(instalador)** Cuando pyRevit haya cargado (unos 20 s):

   ```powershell
   Anota "7-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
   Anota "7-3 node_info" { .\scripts\conn-call.ps1 -Operation node_info -Body '{"element_ids":[1249510,1249630,1249631,1249636],"chord_element_id":1249510}' }
   Anota "7-3 catalog_list vacio" { .\scripts\conn-call.ps1 -Operation catalog_list }
   ```

   Se espera: en `ping`, `operations` con `catalog_apply`, `catalog_delete`, `catalog_get`, `catalog_list`,
   `catalog_save` (18 en total). En `node_info` (es lo que cambia con el marco canónico; compara con
   `resultados-fase-6b.md`): `x_axis` ≈ `[1, 0, 0]` (antes `[-1, 0, 0]`), `y_axis` ≈ `[0, 0, 1]`, `z_axis` ≈ `[0, -1, 0]`
   (antes `[0, 1, 0]`), `chord_direction_reversed: true`, y por barra `angle_in_plane_deg` **con signo**: 1249630 ≈ 136.9
   (antes 43.1), 1249631 ≈ 44.4 (antes 135.6), 1249636 ≈ -135.6 (antes 44.4), con `angle_to_chord_deg` 43.1, 44.4 y 44.4 y
   `side` `+Y`, `+Y`, `-Y`. En `catalog_list`, `templates_count: 0` y la carpeta del catálogo. Copia los tres bloques.

### 7-4. Crear el Detalle D desde la ventana y comparar con el plano **(la persona)**

1. ARBA > **Ejecutar especificación JSON** > `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`.
2. Abajo debe decir en verde `Validación correcta con 2 aviso(s)` (los dos `ANGLE_DIFFERS_FROM_MODEL`, ahora con el texto
   "inclinación ... respecto al cordón"). **Anota los 16 caracteres del token.** La barra de botones tiene dos botones
   nuevos a la izquierda: **Abrir del catálogo** y **Guardar en catálogo**.
3. Mira el croquis: con el marco canónico, +Y apunta hacia arriba y la diagonal con placa cuchilla (1249636, −135,6°) sale
   **abajo a la izquierda**, terminando en el chaflán pequeño de la cartela, como en el plano `detalle-D.png`; las dos
   diagonales superiores salen a 136,9° (izquierda) y 44,4° (derecha). Captura: `docs\fases\capturas\fase7-02-ventana.png`.
   Anota en el chat si el dibujo coincide con el plano (en la Fase 6 salía reflejado: la placa atravesaba el borde
   inclinado largo).
4. Pulsa **Crear**. Debe salir `Conexión modelada correctamente` con `Elementos geométricos creados: 9`. **Anota el ID de
   conexión.** Cierra el diálogo.
5. Vista 3D, detalle **Fino**, sombreado, zoom al nudo: la cartela debe quedar **reflejada respecto a**
   `fase6-05-nudo.png`, con el borde inclinado largo del lado donde no hay barra y la placa cuchilla terminando en el chaflán
   pequeño. Captura: `docs\fases\capturas\fase7-03-nudo.png`. Mira también de canto en qué cara de la cartela apoya la
   placa cuchilla (con el marco canónico, `+z` es la cara opuesta a la de la ronda 6d): anótalo.

### 7-5. Guardar la plantilla desde la conexión y aplicarla al mismo nudo **(instalador, ventanas cerradas)**

Sustituye `<ID>` por el ID de conexión del paso 7-4:

```powershell
$id = "<ID>"
Anota "7-5 catalog_save" { .\scripts\conn-call.ps1 -Operation catalog_save -Body ('{"connection_id":"' + $id + '","name":"Nudo tipico Detalle D","description":"Cartela PL 3/8 565x530 con diagonales ranuradas e inferior con placa cuchilla PL10 y 4 pernos 5/8","tags":["hangar","cercha","HSS"],"copy_to_shared":true}') }
Anota "7-5 catalog_list" { .\scripts\conn-call.ps1 -Operation catalog_list }
Anota "7-5 carpeta del catalogo" { Get-ChildItem "$env:LOCALAPPDATA\MotorConexiones\catalogo" | Select-Object Name, Length; Get-ChildItem catalog -Filter *.json | Select-Object Name, Length }
```

Se espera en `catalog_save`: `ok: true`, un `template_id` (GUID), `file` en `...\AppData\Local\MotorConexiones\catalogo\`,
`shared_file` en `D:\Proyectos C#\CONEXIONES\catalog\`, `members_count: 3` y `member_pattern` con `angle_deg` 136.9 (`+Y`),
44.4 (`+Y`) y -135.6 (`-Y`). **Copia el template_id.** En `catalog_list`, `templates_count: 1`. Después:

```powershell
$tid = "<template_id>"
Anota "7-5 catalog_get" { .\scripts\conn-call.ps1 -Operation catalog_get -Body ('{"template_id":"' + $tid + '"}') }
Anota "7-5 catalog_apply mismo nudo" { .\scripts\conn-call.ps1 -Operation catalog_apply -Body ('{"template_id":"' + $tid + '","element_ids":[1249510,1249630,1249631,1249636],"chord_element_id":1249510}') }
Anota "7-5 delete conexion" { .\scripts\conn-call.ps1 -Operation delete -Body ('{"connection_id":"' + $id + '"}') }
Anota "7-5 conn_list" { .\scripts\conn-call.ps1 -Operation list }
```

Se espera en `catalog_get` la plantilla con `spec_template` sin `element_id` y con `slot` en cada barra; en `catalog_apply`,
`ok: true`, `match.orientation: "same"`, `max_deviation_deg: 0`, `is_valid: true`, un `validation_token` de 64 caracteres
distinto del de 7-4 (la especificación instanciada lleva `source.template_id`), avisos `ANGLE_DIFFERS_FROM_MODEL` **ninguno**
(el ángulo real ya va en `expected_angle_deg`); en `delete`, `deleted_elements_count: 9`; en `list`, `connections_count: 0`.

### 7-6. Botón Catálogo: aplicar a la selección y crear **(la persona)**

1. Selecciona en Revit las cuatro barras del nudo del Detalle D (cordón 1249510 y 1249630, 1249631, 1249636).
2. ARBA > **Catálogo**. La ventana lista `Nudo tipico Detalle D` (3 barras, cordón HSS3X3X1/4, patrón con los tres ángulos).
   Captura: `docs\fases\capturas\fase7-04-catalogo.png`. Selecciona la fila y pulsa **Aplicar a la selección**.
3. Se abre la ventana de previsualización con la cabecera `Plantilla 'Nudo tipico Detalle D' aplicada al nudo del cordón
   1249510 (same)`, la validación en verde (sin avisos de ángulo) y **Recargar** desactivado. Captura:
   `docs\fases\capturas\fase7-05-plantilla-aplicada.png`. Pulsa **Guardar JSON**: abajo debe decir que lo escribió en
   `Documentos\MotorConexiones\Nudo tipico Detalle D-nudo-1249510.json`. Pulsa **Crear**: diálogo de éxito con
   `Plantilla del catálogo: <template_id>` y 9 elementos. Anota el ID.
4. ARBA > **Conexiones del modelo**: la lista debe mostrar la conexión; **Borrar seleccionada** > Sí. Cierra.

### 7-7. Aplicar la plantilla a otro nudo de la cercha **(la persona)**

1. Selecciona el cordón y todas las barras del **nudo simétrico** (otra mitad de la cercha). ARBA > **Catálogo** >
   **Aplicar a la selección**.
2. Si casa: la cabecera dice la orientación (se espera `mirror_x` en el nudo simétrico; `same` si elegiste uno igual) y el
   croquis muestra la cartela reflejada con la placa cuchilla en la diagonal inferior. Captura:
   `docs\fases\capturas\fase7-06-otro-nudo-ventana.png`. Anota en el chat la orientación, los avisos (por ejemplo
   `TEMPLATE_ANGLE_DEVIATION` si las diagonales llegan con otro ángulo) y si la validación está en verde.
3. Si está en verde, **Crear**; vista 3D al nudo nuevo, captura `docs\fases\capturas\fase7-07-otro-nudo.png`; después
   **Conexiones del modelo** > borrar. Si da `PLATE_OUTSIDE_GUSSET` u otro error, no crees nada: copia el error y el campo
   (es el caso previsto en la propuesta: la cartela fija no cubre una barra con otro ángulo).
4. Si la ventana del catálogo dice que la plantilla no casa (`TEMPLATE_NO_MATCH`), copia el texto entero de la barra de
   estado: dice qué ranura no encontró barra y qué barras sobraron.

### 7-8. Las rutas nuevas por el puente: 25/25 **(instalador, ventanas cerradas)**

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 20
Anota "7-8 puente en 8000" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
Anota "7-8 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 25/25 pruebas correctas` (las pruebas 19 a 23 crean y borran la plantilla `PRUEBA probar_conexiones`;
la 24 debe decir `herramientas=... conn_*=18`). Si alguna da `[FALLO]`, copia su bloque entero.

### 7-9. Restos, registro, cerrar y subir (autorizado)

```powershell
Anota "7-9 sondeo 12 restos de conexiones" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "7-9 sondeo 13 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
Anota "7-9 log del dia" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "catalog_|ribbon_" | Select-Object -Last 40 }
Anota "7-9 catalogo final" { .\scripts\conn-call.ps1 -Operation catalog_list }
```

Se espera `conexiones en el modelo: 0`, `Elementos de acero sueltos encontrados: 0` y, en `catalog_list`, solo
`Nudo tipico Detalle D` (la plantilla `PRUEBA probar_conexiones` ya no está).

1. **(la persona)** Cierra la copia en Revit **sin guardar**.
2. **(instalador)** Sube los resultados, las capturas y la plantilla oficial (P2 de la propuesta):

   ```powershell
   git add catalog\*.json docs\fases\resultados-fase-7.md docs\fases\capturas
   git commit -m "Fase 7: resultados del instalador (catálogo de conexiones, marco canónico y plantilla del Detalle D)"
   git pull --no-rebase origin main
   git push origin main
   ```

3. Devuelve: la salida completa de los pasos 7-0, 7-2, 7-3, 7-5, 7-8 y 7-9; las anotaciones de la persona (token de 7-4,
   si el croquis y el nudo coinciden con el plano, en qué cara apoya la placa cuchilla, los IDs de conexión, la orientación
   y los avisos del paso 7-7); las siete capturas; y el texto de cualquier ventana de error del add-in o de Revit.
4. Después de esta ronda, con Revit otra vez **cerrado**, sigue con `docs\instalacion\fase-6d.md` completo (ronda 6d,
   placas centradas): es independiente y crea y borra su propia conexión.
