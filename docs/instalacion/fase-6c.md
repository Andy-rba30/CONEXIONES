# Instalación y prueba de la ronda 6c: el perno baja desde la cara exterior de la placa cuchilla

Objetivo: una ronda corta. La 6b dejó el paquete bien (placa cuchilla apoyada en la cartela, `Bolt Length 44,45`,
`Grip Length 19,53`) pero los pernos colgaban enteros por fuera de la cartela, porque Advance Steel extiende el perno
desde el plano del patrón hacia −Z. Ahora el plano va en la cara exterior de la placa cuchilla, con lo que el agarre
recorre placa + cartela: cabeza por un lado, tuerca por el otro. Hay que crearla, mirarla de canto, medirla con el
sondeo 16 (ya sin el fallo de la "á") y borrarla. Unos 15 minutos, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, nunca el
original. Mismas reglas que las rondas anteriores (la persona abre y cierra Revit; el instalador no ejecuta nada con
una ventana del add-in abierta).

### 6c-1. Pull, compilar, desplegar (Revit cerrado) **(instalador)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin main
$salida = "docs\fases\resultados-fase-6c.md"
"# Resultados de la ronda 6c`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
Anota "6c-1 git" { git log -1 --oneline }
Anota "6c-1 build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
Anota "6c-1 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
Anota "6c-1 deploy" { .\scripts\deploy.ps1 -NoBuild }
```

Se espera un commit "Ronda 6c: ...", `0 Errores`, `Superado: 99`, `6c-1 revit cerrado` vacío y
`== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==`.

### 6c-2. Crear el Detalle D desde la ventana **(la persona)**

1. Abre Revit 2027 con `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`.
2. ARBA > **Ejecutar especificación JSON** > `docs\fixtures\detalle-D-confirmado.json` > con la validación en verde,
   **Crear**. Anota el ID de conexión y cierra el diálogo.
3. Vista 3D, detalle **Fino**, sombreado con aristas. Mira la placa cuchilla **de canto** (como en
   `fase6b-04-nudo-pernos.png`): ahora cada perno debe atravesar la placa cuchilla y la cartela, con la cabeza sobre la
   cara exterior de la placa y la tuerca sobre la cara trasera de la cartela, sobresaliendo poco por cada lado. Captura:
   `docs\fases\capturas\fase6c-01-pernos-canto.png`. Anota en el chat: ¿atraviesan las dos placas? ¿qué hay a cada lado?

### 6c-3. Medir con el sondeo 16 **(instalador, con las ventanas del add-in cerradas)**

```powershell
Anota "6c-3 sondeo 16 agarre" { .\scripts\revit-exec.ps1 -File scripts\sondeos\16-pernos-agarre.py -SinTransaccion -TimeoutSec 600 }
Anota "6c-3 log pernos" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "advance_steel_bolts_written|advance_steel_plate_written" | Select-Object -Last 3 }
```

Se espera: en el apartado 3, cartela `z -4.76 .. 4.76`, placa cuchilla `z 4.76 .. 14.76` y los pernos
(`SteelProxyElement | Bolts`) con un intervalo que **cubra** `-4.76 .. 14.76` sobresaliendo unos milímetros por cada
lado, con `Bolt Length ... = 44.45 mm` y `Grip Length ... = 19.53 mm`; en el apartado 4, `pernos: ... atraviesan
cartela + placa OK`; en el log, `plane_z_mm: 14.7625`. Si el sondeo vuelve a fallar con un error o los pernos dicen
`NO cubren el paquete`, copia el bloque entero (el intervalo Z de los pernos es el dato que importa). Si dice
`SIN GEOMETRIA LEGIBLE`, vale la captura de la persona.

### 6c-4. Borrar, cerrar y subir (autorizado)

1. **(la persona)** ARBA > **Conexiones del modelo** > fila > **Borrar seleccionada** > **Sí** > **Cerrar**. Anota lo que dijo.
2. **(instalador)**

   ```powershell
   Anota "6c-4 sondeo 12" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
   Anota "6c-4 sondeo 13" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

   Se espera `conexiones tras borrar: 0`, las extensiones de siempre (`68.64`, `69.2`, `0.0`) y `0` restos de acero.
3. **(la persona)** Cierra la copia en Revit **sin guardar**.
4. **(instalador)**

   ```powershell
   git add docs\fases\resultados-fase-6c.md docs\fases\capturas
   git commit -m "Ronda 6c: resultados del instalador (pernos desde la cara exterior de la placa cuchilla)"
   git pull --no-rebase origin main
   git push origin main
   ```

5. Devuelve la salida completa de 6c-1, 6c-3 y 6c-4, la captura `fase6c-01-pernos-canto.png`, lo que vio la persona de
   canto y el ID de conexión.
