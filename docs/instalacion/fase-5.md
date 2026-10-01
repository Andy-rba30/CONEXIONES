# Instalación y prueba de punta a punta de la Fase 5 (parte A: agente instalador; parte B: la persona con Antigravity)

Objetivo: desplegar en Revit el add-in de la Fase 5 (sin las operaciones `probe_*`; el hash de `config\limits.json`
entra en el `validation_token`), comprobar que responde (`conn_ping`, `probar_conexiones.py --puente` 19/19, sondeo 14) y
después hacer, desde Antigravity, la prueba de punta a punta del encargo: de `conn_ping` a `conn_create` con el Detalle D,
mirar el nudo en Revit, y `conn_delete`. Todo sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
**nunca el original**. Parte A: unos 20 minutos. Parte B: unos 20 minutos.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Vale Windows PowerShell
5.1 o PowerShell 7; todos los comandos de la parte A van en la misma ventana, en orden.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso A-3 (`deploy.ps1` copia la DLL y falla, o deja la vieja, si Revit la tiene cargada).
  Si está abierto con un modelo, ciérralo desde Revit (sin guardar si es la copia).
- La copia `HANGAR_PRUEBA_sondeo.rvt` existe desde la Fase 1. Si no existiera: abre el original y ejecuta
  `.\scripts\revit-exec.ps1 -File scripts\sondeos\08-guardar-copia.py -SinTransaccion` (guarda la copia y sigue sobre ella).
- Si el puente MCP ya está en marcha en el puerto 8000 (ventana de `iniciar_servidor_revit.bat`), el paso A-5 lo cierra y
  lo vuelve a arrancar para que cargue el `conn_tools.py` recién instalado.

---

## Parte A: instalador

### A-1. PowerShell, rama y archivo de resultados

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin claude/laughing-pascal-tsxvkt
$salida = "docs\fases\resultados-fase-5.md"
"# Resultados de la Fase 5`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"
Anota "A-1 git" { git log -1 --oneline }
```

La última línea debe ser un commit "Fase 5: ...".

### A-2. Compilar y pasar las pruebas

```powershell
Anota "A-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
```

Se espera `0 Advertencia(s)`, `0 Errores` y `Superado: 51`. Si falla, **para aquí** y devuelve la salida.

### A-3. Revit cerrado, desplegar el add-in e instalar los archivos del MCP

Comprueba que Revit está cerrado (la primera orden solo asegura que no queda ningún proceso; si la persona tenía un
modelo abierto, debe haberlo cerrado antes desde Revit):

```powershell
Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
Anota "A-3 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "A-3 deploy e instalar-conn" { .\scripts\deploy.ps1 -NoBuild; .\mcp\instalar-conn.ps1 }
```

Se espera: `A-3 revit cerrado` vacío; `== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==` con `Copiados: ...
config\limits.json, docs\guide.md`; `copiado revit_mcp\conexiones.py (15 rutas @api.route)` y
`copiado tools\conn_tools.py (13 herramientas @mcp.tool)`.

### A-4. Abrir Revit con la copia y comprobar que la DLL nueva está cargada

**Abrir Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si Revit pregunta por el
add-in sin firmar, pulsa *Always Load*. Espera a que cargue pyRevit (unos 20 s) y comprueba:

```powershell
Anota "A-4 ping" { .\scripts\conn-call.ps1 -Operation ping }
```

Se espera `ok: true`, `document.title: HANGAR_PRUEBA_sondeo`, `backend: advancesteel` y en `operations` exactamente estas
13: `create, delete, find_profile, get, guide, list, node_info, ping, preview, schema, types, update, validate`.
**Si aparecen `probe_delete_b` o `probe_plate_b`, Revit sigue con la DLL de la Fase 4**: anótalo, cierra Revit, repite
A-3 y A-4.

### A-5. Arrancar el puente MCP (el mismo que usa Antigravity) y dejarlo abierto

```powershell
Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like "*mcp-server-for-revit-python.extension*main.py*" } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }
Start-Process -FilePath "C:\IA\iniciar_servidor_revit.bat"
Start-Sleep -Seconds 15
Anota "A-5 puente en 8000" { Get-NetTCPConnection -LocalPort 8000 -State Listen -ErrorAction SilentlyContinue | Select-Object LocalAddress, LocalPort, OwningProcess }
```

Se abre una ventana nueva con el puente (`Uvicorn running on http://127.0.0.1:8000`): **no la cierres**, la parte B la
usa. Se espera una línea con `LocalPort 8000`. Si sale vacío, copia lo que dice la ventana del puente y sigue.

### A-6. Pruebas de humo contra Revit y por el puente (19; ninguna crea nada)

```powershell
Anota "A-6 probar_conexiones --puente" { & $py mcp\pruebas\probar_conexiones.py --puente }
```

Se espera `Resultado: 19/19 pruebas correctas`; en la prueba 18, `conn_*=13`. Si alguna da `[FALLO]`, copia su bloque
entero (nombre, detalle y cuerpo).

### A-7. Sondeo 14: token ligado a `limits.json`, sin crear nada

```powershell
Anota "A-7 sondeo 14 token y limits" { .\scripts\revit-exec.ps1 -File scripts\sondeos\14-fase5-token-limits.py -SinTransaccion -TimeoutSec 600 }
```

Se espera `RESULTADO: 14/14 comprobaciones correctas`. Lo que decide: `[OK] ping ok y sin operaciones probe_*`,
`[OK] limits.json desplegado existe y tiene el mismo hash...`, `[OK] validar dos veces da el mismo token`,
dos `[OK] create ... -> VALIDATION_TOKEN_INVALID` y `[OK] conn_list igual que antes (nada creado)`. Si alguna línea
dice `[NO PROBADO]` (la reflexión sobre el Core falló), copia el motivo y sigue: no es un fallo del add-in.

### A-8. Registro del add-in de hoy

```powershell
Anota "A-8 log add-in" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String '"operation":"(validate|create)"' | Select-Object -Last 8 }
```

Se esperan líneas `validate` con `ok:true` y líneas `create` con `"ok":false` y `VALIDATION_TOKEN_INVALID`. **Ninguna
línea `create` con `ok:true`** (la parte A no crea nada).

### A-9. Subir los resultados de la parte A (autorizado) y dejar todo abierto para la parte B

```powershell
git add docs\fases\resultados-fase-5.md
git commit -m "Fase 5: resultados del instalador (despliegue, 19/19 y sondeo 14)"
git pull --no-rebase origin claude/laughing-pascal-tsxvkt
git push origin claude/laughing-pascal-tsxvkt
```

Deja **Revit abierto con la copia** y **la ventana del puente abierta**. Devuelve la salida de A-3, A-4, A-6, A-7 y A-8
tal cual, y si hubo ventanas o cierres de Revit. Avisa a la persona de que puede empezar la parte B.

---

## Parte B: la persona, con Antigravity

### B-0. Preparación

1. Revit abierto con la copia `HANGAR_PRUEBA_sondeo.rvt` (de A-4) y la ventana del puente abierta (de A-5).
2. En Antigravity, el servidor `revit` configurado en `%USERPROFILE%\.gemini\config\mcp_config.json` con
   `"serverUrl": "http://localhost:8000/mcp"` (ya quedó así en la Fase 4). **Recarga o reinicia el servidor `revit`** en
   Antigravity para que vuelva a pedir la lista de herramientas: deben verse 79 herramientas, 13 de ellas `conn_*`.
3. Abre la carpeta `D:\Proyectos C#\CONEXIONES` como espacio de trabajo en Antigravity, para que pueda leer
   `docs\fixtures\detalle-D-confirmado.json`. Si no puede leer archivos, pega el contenido de ese JSON debajo del prompt.
4. Pon Revit a la vista con una vista 3D y el nudo del Detalle D a mano (los miembros 1249510, 1249630, 1249631 y 1249636).

### B-1. Prompt para pegar en Antigravity (literal, completo)

```text
Vas a hacer la prueba de punta a punta del add-in MotorConexiones con las herramientas MCP conn_* del servidor "revit".
Revit tiene abierta la copia de prueba HANGAR_PRUEBA_sondeo.rvt. La especificación que vas a usar es, sin cambiar ni una
coma, el archivo D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json (Detalle D, dudas ya confirmadas).

REGLAS, por encima de todo lo demás:
1. Una llamada cada vez. Después de cada paso, muéstrame los campos clave de la respuesta (ok, códigos de errors y
   warnings, meta.duration_ms y los datos que indico) y espera a que te diga "sigue" antes del paso siguiente.
2. NUNCA llames a conn_create sin que yo haya escrito literalmente "SÍ, CREA". NUNCA llames a conn_delete sin que yo
   haya escrito literalmente "SÍ, BORRA". No llames a conn_update en esta prueba.
3. NUNCA uses execute_revit_code ni ninguna otra herramienta que modifique el modelo (ni para mirar, ni para borrar, ni
   para "arreglar" nada) sobre los elementos que cree el add-in. Solo las herramientas conn_*.
4. No modifiques la especificación del archivo. Si conn_validate devuelve errores, no los corrijas: muéstramelos y para.
5. Si conn_create no responde en 3 minutos, NO la repitas: llama a conn_list y dime qué hay.
6. Si cualquier paso devuelve ok:false o un error del puente, muéstramelo y para. No improvises.

PASOS:
1. conn_ping. Espero ok:true, backend "advancesteel", document.title "HANGAR_PRUEBA_sondeo" y, en data.operations, 13
   operaciones sin ninguna que empiece por "probe_". Si el documento no es HANGAR_PRUEBA_sondeo, PARA y dímelo.
2. conn_get_guide. Resume en 5 líneas el flujo obligatorio y las reglas de seguridad.
3. conn_list_types. Espero solo gusset_node.
4. conn_get_node_info con element_ids [1249510, 1249630, 1249631, 1249636] y chord_element_id 1249510. Muéstrame
   origin_mm, axis_distance_mm, el tipo (perfil) y angle_in_plane_deg de cada miembro y existing_connections.
5. conn_get_schema con connection_type "gusset_node". Dime solo las claves de data.
6. Lee el archivo docs\fixtures\detalle-D-confirmado.json y llama a conn_validate con ese objeto como spec, tal cual.
   Espero is_valid:true, 0 errores, avisos ANGLE_DIFFERS_FROM_MODEL y un validation_token de 64 caracteres. Muéstrame
   el token completo y calculated_values.
7. conn_preview con la misma spec. Muéstrame data.summary (espero gusset_plates 1, knife_plates 1, bolts 4,
   weld_lines 6, members_modified 3) y members_to_modify.
8. Muéstrame una tabla corta con lo que se va a crear (cordón, cartela, cada barra con su unión, pernos, soldaduras,
   retiros) y pregúntame: "¿Creo la conexión? Responde SÍ, CREA". Espera mi respuesta.
9. Solo si he escrito "SÍ, CREA": conn_create con la misma spec y el validation_token del paso 6. Muéstrame
   connection_id, created_element_ids (espero 9), backend y warnings, y meta.duration_ms.
10. conn_list (espero connections_count 1) y conn_get con ese connection_id (dime backend, created_elements_count y
    created_utc). Después dime: "Mira el nudo en Revit y haz la captura. Cuando termines, escribe 'listo'". Espera.
11. Cuando escriba "listo", pregúntame: "¿Borro la conexión? Responde SÍ, BORRA". Espera mi respuesta.
12. Solo si he escrito "SÍ, BORRA": conn_delete con ese connection_id. Muéstrame data y warnings. Después conn_list
    (espero connections_count 0).
13. Escribe un resumen final en texto plano para que lo pegue en un archivo: por cada paso, herramienta, ok, códigos de
    errores y avisos, duración en ms, y los datos clave (token, connection_id, IDs creados, resumen del preview,
    resultado del borrado).
```

### B-2. Mientras la conexión existe (entre los pasos 10 y 11 del prompt): mirar y capturar

Esto responde al pendiente de la Fase 3 (sección 8 de `docs/fases/fase-3.md`): en la Fase 3 la captura exportada
desde dentro de Revit no mostraba las placas ni los pernos de Advance Steel; en la Fase 1 sí se veían en pantalla al cabo
de unos segundos. Hay que mirarlo **a ojo**:

1. En Revit, vista 3D, zoom al nudo. Espera 10 segundos. Debe verse la cartela (polígono de 565 × 530 mm en el plano de
   la cercha), la placa cuchilla con 4 pernos en la diagonal inferior (miembro 1249636), las 6 soldaduras (sólidos finos)
   y las tres barras acortadas. Si las placas o los pernos no aparecen, prueba a orbitar la vista o a pulsar *Fine* en
   el nivel de detalle, y espera otros 10 segundos.
2. **Captura de pantalla** (Win+Shift+S) con el nudo a la vista y guárdala como
   `D:\Proyectos C#\CONEXIONES\docs\fases\capturas\fase5-02-pantalla.png`.
3. Captura exportada desde dentro de Revit (la hace el instalador o tú, en la ventana de PowerShell de la parte A):

   ```powershell
   .\scripts\revit-exec.ps1 -File scripts\sondeos\capturar-nudo.py -SinTransaccion
   Rename-Item "docs\fases\capturas\fase3-captura.png" "fase5-01-conexion.png" -Force
   ```

4. Anota el veredicto para el archivo de resultados: "Placas y pernos de Advance Steel visibles en pantalla: SÍ/NO
   (tras cuántos segundos)" y "visibles en la captura exportada fase5-01: SÍ/NO".

Después escribe `listo` en Antigravity y sigue con los pasos 11 a 13 del prompt.

### B-3. Tras `conn_delete`: extensiones restauradas (lo ejecuta el instalador)

En la ventana de PowerShell de la parte A (si se cerró, repite las líneas de A-1 **sin** la que crea el archivo con
`Set-Content`, para no borrar los resultados de la parte A):

```powershell
Anota "B-3 sondeo 12 extensiones tras conn_delete" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
```

Se espera `conexiones en el modelo: 0`, `conexiones tras borrar: 0` y, al final, las extensiones actuales de las barras
del fixture iguales a las originales de la copia (Fase 3): `1249630: inicio 0.0 | fin 68.64`, `1249631: inicio 0.0 |
fin 69.2`, `1249636: inicio 0.0 | fin 0.0`. Si el sondeo encuentra una conexión (porque `conn_delete` falló en
Antigravity), la borra él y muestra `extension X -> Y` por barra: anótalo, es un resultado importante.

### B-4. Guardar lo que dijo Antigravity

Pega a mano al final de `docs\fases\resultados-fase-5.md`:

- Bajo un título `## B-1 Antigravity (punta a punta)`: el resumen final del paso 13 del prompt y, si hubo algún error,
  la respuesta completa de la herramienta que falló.
- Bajo `## B-2 comprobación a ojo`: el veredicto de B-2 (placas y pernos visibles en pantalla SÍ/NO, y en la captura
  exportada SÍ/NO).

### B-5. Cerrar y subir (autorizado)

1. Cierra la copia en Revit **sin guardar** (Archivo > Cerrar > *No guardar*). Cierra la ventana del puente.
2. Sube resultados y capturas:

   ```powershell
   git add docs\fases\resultados-fase-5.md docs\fases\capturas
   git commit -m "Fase 5: resultados de la prueba de punta a punta con Antigravity"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

3. Devuelve: el resumen de Antigravity (B-4), el veredicto de B-2, la salida de B-3, las dos capturas, el texto de
   cualquier ventana de Revit que haya aparecido, y si Revit se cerró de golpe en algún paso (en cuál y qué fue lo último
   impreso).
