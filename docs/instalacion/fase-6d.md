# Instalación y prueba de la ronda 6d: las placas centradas en su plano

Objetivo: una ronda corta, igual que la 6c. El sondeo 16 de la 6c midió la cartela en `z 0 .. 9,52` y la placa cuchilla
en `9,76 .. 19,76`: Advance Steel extruye cada placa desde el plano hacia +Z en vez de centrarla, así que el paquete
entero estaba corrido 4,76 mm (la cabeza del perno quedaba metida 5 mm en la placa cuchilla y la tuerca flotaba 4,76 mm
bajo la cartela). Ahora el plano de cada placa se baja medio espesor: la cartela debe salir centrada en el plano de la
cercha (`−4,76 .. 4,76`, como las barras) y la placa cuchilla pegada encima (`4,76 .. 14,76`), con los pernos cubriendo
justo ese paquete. Unos 15 minutos, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, nunca el original. Mismas reglas que las
rondas anteriores (la persona abre y cierra Revit; el instalador no ejecuta nada con una ventana del add-in abierta).

### 6d-1. Pull, compilar, desplegar (Revit cerrado) **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-6d.md"
"# Resultados de la ronda 6d`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
Anota "6d-1 git" { git log -1 --oneline }
Anota "6d-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "6d-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6d-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
```

Se espera un commit "Ronda 6d: ...", `0 Errores`, `Superado: 99`, `6d-1 revit cerrado` vacío y
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==`.

### 6d-2. Crear el Detalle D desde la ventana **(la persona)**

1. Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`.
2. ARBA > **Ejecutar especificación JSON** > `docs\fixtures\detalle-D-confirmado.json` > con la validación en verde,
   **Crear**. Anota el ID de conexión y cierra el diálogo.
3. Vista 3D, detalle **Fino**, sombreado con aristas. Mira la placa cuchilla **de canto** (como en
   `fase6b-04-nudo-pernos.png`): cada perno debe atravesar la placa cuchilla y la cartela, con la cabeza apoyada **sobre**
   la cara exterior de la placa (no metida en ella) y la tuerca apoyada **sobre** la cara trasera de la cartela (sin
   hueco). Fíjate también en que la cartela quede centrada en las barras, no desplazada hacia un lado. Captura:
   `docs\fases\capturas\fase6d-01-pernos-canto.png`. Anota en el chat: ¿atraviesan las dos placas? ¿qué hay a cada lado?

### 6d-3. Medir con el sondeo 16 **(instalador, con las ventanas del add-in cerradas)**

```powershell
Anota "6d-3 sondeo 16 agarre" { .\scripts\revit-exec.ps1 -File scripts\sondeos\16-pernos-agarre.py -SinTransaccion -TimeoutSec 600 }
Anota "6d-3 log pernos" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "advance_steel_bolts_written|advance_steel_plate_written" | Select-Object -Last 3 }
```

Se espera: en el apartado 3, cartela `z -4.76 .. 4.76` (en la 6c salió `0 .. 9.52`), placa cuchilla `z 4.76 ..
14.76` (en la 6c, `9.76 .. 19.76`) y los pernos (`SteelProxyElement | Bolts`) en `-34.45 .. 24.68` aproximadamente
(cabeza sobre la placa cuchilla a 14,76, vástago de 44,45 hacia abajo, tuerca bajo la cartela), con `Bolt Length ... =
44.45 mm` y `Grip Length ... = 19.52 mm`; en el apartado 4, `cartela: centrada ... OK`, `placa cuchilla: ... apoya en
la cara +z ... OK` y `pernos: ... atraviesan cartela + placa OK; sobresalen ~29.7 mm por abajo y ~9.9 mm por arriba`;
en el log, `advance_steel_plate_written` de la cartela con `plane_z_mm: -4.7625` y de la cuchilla con `plane_z_mm:
4.7625`. Si el sondeo vuelve a fallar con un error o los pernos dicen
`NO cubren el paquete`, copia el bloque entero (el intervalo Z de los pernos es el dato que importa). Si dice
`SIN GEOMETRIA LEGIBLE`, vale la captura de la persona.

### 6d-4. Borrar, cerrar y subir (autorizado)

1. **(la persona)** ARBA > **Conexiones del modelo** > fila > **Borrar seleccionada** > **Sí** > **Cerrar**. Anota lo que dijo.
2. **(instalador)**

   ```powershell
   Anota "6d-4 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6d-4 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones tras borrar: 0`, las extensiones de siempre (`68.64`, `69.2`, `0.0`) y `0` restos de acero.
3. **(la persona)** Cierra la copia en Revit **sin guardar**.
4. **(instalador)**

   ```powershell
   git add docs\fases\resultados-fase-6d.md docs\fases\capturas
   git commit -m "Ronda 6d: resultados del instalador (placas centradas en su plano)"
   git pull --no-rebase origin main
   git push origin main
   ```

5. Devuelve la salida completa de 6d-1, 6d-3 y 6d-4, la captura `fase6d-01-pernos-canto.png`, lo que vio la persona de
   canto y el ID de conexión.
