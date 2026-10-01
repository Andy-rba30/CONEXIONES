# Fase 5, ronda 5b: Advance Steel en milímetros (instrucciones para el agente instalador)

Objetivo: comprobar la corrección de unidades del backend de Advance Steel. En la parte B de la Fase 5 la conexión se
creó bien (`ok: true`, 9 elementos, borrado limpio) pero las placas y los pernos salieron unas 300 veces más pequeños
de lo pedido: el add-in pasaba pies a Advance Steel y Advance Steel trabaja en milímetros. Ahora el add-in pasa
milímetros. Esta ronda repite solo crear, medir, capturar y borrar, con el sondeo 11, que ahora **imprime las medidas
reales** de cada placa y patrón de pernos (las mismas que muestra la paleta de Propiedades de Revit). Diez minutos.
Sobre la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`, **nunca el original**.

Reglas: si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión; copia el error y sigue con el
paso siguiente. No crees scripts nuevos; usa `Anota` directamente en la ventana de PowerShell. Vale Windows PowerShell
5.1 o PowerShell 7; todos los comandos van en la misma ventana, en orden.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 5b-2 (`deploy.ps1` no puede sustituir la DLL si Revit la tiene cargada). Si la
  parte B dejó la copia abierta, ciérrala desde Revit **sin guardar**.
- El puente MCP (ventana de `iniciar_servidor_revit.bat`) puede quedarse abierto o cerrado: esta ronda no lo usa.
- Los resultados de la parte B (`docs\fases\resultados-fase-5.md`, B-3, B-4 y B-5) deben estar ya subidos; esta ronda
  **añade** al final de ese archivo, no lo crea de nuevo.

### 5b-1. PowerShell, rama y archivo de resultados (se añade al archivo de la Fase 5)

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin claude/laughing-pascal-tsxvkt
$salida = "docs\fases\resultados-fase-5.md"
"`n# Ronda 5b: Advance Steel en milímetros`n`nFecha: $(Get-Date -Format s)`n" | Add-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
Anota "5b-1 git" { git log -1 --oneline }
```

La última línea debe ser un commit "Fase 5b: ...". Si dice "Fase 5: resultados ..." u otro, repite el `git pull` y
anótalo.

### 5b-2. Compilar, pasar las pruebas, Revit cerrado y desplegar

```powershell
Anota "5b-2 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue; Start-Sleep -Seconds 3
Anota "5b-2 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "5b-2 deploy" { .\scripts\deploy.ps1 -NoBuild }
```

Se espera `0 Advertencia(s)`, `0 Errores`, `Superado: 51`, `5b-2 revit cerrado` vacío y
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==`. Si el build falla, **para aquí** y devuelve la salida.

### 5b-3. Abrir Revit con la copia y comprobar la DLL

**Abrir Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si Revit pregunta por el
add-in sin firmar, pulsa *Always Load*. Espera a que cargue pyRevit (unos 20 s) y comprueba:

```powershell
Anota "5b-3 ping" { .\scripts\conn-call.ps1 -Operation ping }
```

Se espera `ok: true`, `document.title: HANGAR_PRUEBA_sondeo` y `backend: advancesteel`.

### 5b-4. Crear la conexión y leer las medidas reales (lo que decide esta ronda)

```powershell
Anota "5b-4 sondeo 11 crear y medir" { .\scripts\revit-exec.ps1 -File scripts\sondeos\11-fase3-crear.py -SinTransaccion -TimeoutSec 900 }
```

Puede tardar hasta 3 minutos (Advance Steel). Las líneas que importan están al final, debajo de cada
`[id] SteelProxyElement | Plates` o `| Bolts`:

- Cartela: `Thickness: 3/8" = 9.52 mm`, `Length` y `Width` de unos 565 y 530 mm (en pulgadas, 1' 10 1/4" y 1' 8 7/8").
- Placa cuchilla: `Thickness` 10 mm, `Length` 170 mm, `Width` 140 mm.
- Pernos: `Diameter 5/8" = 15.88 mm`, `Length on side 1` e `Intermediate distance` 60 mm (2 3/8"), `Number on side` 2 y 2.

Si vuelven a salir 2 mm, 0,2 mm o 0, la corrección no ha surtido efecto: anótalo tal cual y sigue. Si sale
`DirectShape | Structural Connections` en placas o pernos, Advance Steel no entró: copia los avisos de `create`.

### 5b-5. Mirar y capturar (la persona mira; el instalador exporta)

1. **La persona**, en la vista 3D de Revit: nivel de detalle **Fino**, estilo **Sombreado**, zoom al nudo. Ahora deben
   verse la cartela, la placa cuchilla y los 4 pernos en la diagonal inferior, además de las barras acortadas. Captura
   con `Windows + Mayús + S`, guardada como `docs\fases\capturas\fase5-03-placas-pantalla.png`. Anota en el chat
   "SÍ se ven" o "NO se ven" y, si no, qué se ve.
2. **El instalador** exporta la vista desde dentro de Revit:

   ```powershell
   Anota "5b-5 captura exportada" { .\scripts\revit-exec.ps1 -File scripts\sondeos\capturar-nudo.py -SinTransaccion }
   Rename-Item "docs\fases\capturas\fase3-captura.png" "fase5-04-placas-exportada.png" -Force
   ```

### 5b-6. Borrar y comprobar que no queda nada

```powershell
Anota "5b-6 sondeo 12 borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
Anota "5b-6 restos de acero" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
```

Se espera `conexiones en el modelo: 1`, `conexiones tras borrar: 0`, las extensiones `1249630: inicio 0.0 | fin 68.64`,
`1249631: inicio 0.0 | fin 69.2`, `1249636: inicio 0.0 | fin 0.0`, y en el sondeo 13 `Elementos de acero sueltos
encontrados: 0`. El sondeo 13 **guarda la copia** solo si encuentra restos; si no encuentra ninguno, no toca nada.

### 5b-7. Registro del add-in de hoy

```powershell
Anota "5b-7 log" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "fabrication|advance_steel|\"create\"|\"delete\"" | Select-Object -Last 40 }
```

Importan las líneas `advance_steel_plate_written` y `advance_steel_bolts_written` (ahora llevan `"units":"mm"` y, en los
pernos, `Dx=60`, `ScrewDiameter=15.875`) y `fabrication_transaction_commit` con sus categorías.

### 5b-8. Cerrar y subir (autorizado)

1. Cierra la copia en Revit **sin guardar** (Archivo > Cerrar > *No guardar*).
2. Sube resultados y capturas:

   ```powershell
   git add docs\fases\resultados-fase-5.md docs\fases\capturas
   git commit -m "Fase 5b: resultados de Advance Steel en milímetros"
   git pull --no-rebase origin claude/laughing-pascal-tsxvkt
   git push origin claude/laughing-pascal-tsxvkt
   ```

3. Devuelve: las medidas impresas por el sondeo 11 (5b-4), el "SÍ/NO se ven" de la persona (5b-5), la salida del
   sondeo 12 y del 13 (5b-6), las dos capturas, el texto de cualquier ventana de Revit que haya aparecido, y si Revit se
   cerró de golpe en algún paso (en cuál y qué fue lo último impreso).
