# Qué sigue tras los resultados de la ronda 8e (2026-10-06)

Rama `main`. Commit del instalador: `e45b0a1`. Copia del mensaje del chat.

## 1. Qué volvió del PC, en corto

| Qué | Resultado | Bien o mal |
|---|---|---|
| **Cordón… sobre N9 con la ventana abierta** (lo que cerraba Revit) | **Revit siguió vivo.** El log tiene 9 pinchados empezados y 9 terminados, 0 errores de ventana. Captura `fase8e-02-cordon-n9.png` | **Bien: el fallo grave está corregido** |
| Descartar desde la ventana | 0 marcadores; sondeo 17 limpio después | Bien |
| Sondeo 19 v3 (etiqueta pinchable) | La etiqueta A (N4) se puso, `GetAll` la ve, y hay captura del clic (`fase8e-01-etiqueta-clic.png`): **la etiqueta se vio y se pudo pinchar**. La etiqueta B no se pudo poner (un error del sondeo al calcular el punto, no de Revit) | Bien a medias; es sondeo, no bloquea nada |
| Sondeo 19b (quitar la etiqueta) y sondeo 17 final | **No pudieron hablar con Revit** ("se canceló una tarea"): Revit estaba ocupado o bloqueado justo después del clic en la etiqueta | A aclarar |
| Sondeos 12 y 13 | Limpios (el 12 tardó 275 s, Revit iba lento) | Bien |
| `probar_conexiones.py --puente` | **0/1: "no se pudo hablar con Revit"** (conexión rechazada). O Revit estaba cerrado, o el servidor del puente no arrancó | **No cuenta**: hay que repetirlo |

Falta en el archivo lo que tú viste: **¿Revit siguió abierto después de pinchar la etiqueta, o se cerró?** Si te acuerdas,
dímelo o anótalo al final de `docs\fases\resultados-fase-8e.md`. Lo más probable es que el cuadro que abre el clic de la
etiqueta dejara a Revit "esperando" y por eso los sondeos siguientes no pudieron hablar con él.

## 2. Qué hacer ahora, en orden

### Paso 1 (instalador, 5 minutos): repetir solo el puente con Revit abierto

Abre Revit con la copia, arranca el servidor como siempre y pásale esto al instalador. Añade al archivo las dos secciones
que faltan y las sube.

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
$salida = "docs\fases\resultados-fase-8e.md"
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$py = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\.venv\Scripts\python.exe"
$env:PYTHONIOENCODING = "utf-8"
Anota "8e-7b ping" { .\scripts\conn-call.ps1 -Operation ping }
Anota "8e-7b sondeo 19b quitar (repetido)" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
Anota "8e-7b probar_conexiones --puente (repetido)" { & $py mcp\pruebas\probar_conexiones.py --puente }
Anota "8e-7b log del dia (repetido)" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|plan_event|startup" | Select-Object -Last 80 }
git add $salida
git commit -m "Ronda 8e: puente y log repetidos con Revit abierto"
git push origin main
```

Prompt para el instalador: *"Lee docs\fases\resumen-fase-8e-que-sigue.md del repositorio D:\Proyectos C#\CONEXIONES y
ejecuta el bloque de PowerShell del Paso 1 tal cual, en una sola ventana, con Revit abierto y el servidor arrancado. Si
`probar_conexiones.py --puente` no llega a 28/28, copia la salida entera. Devuélveme la salida de los cuatro bloques y el
git log -1 --oneline."*

### Paso 2 (Claude Code, sesión nueva): cierre de la 8e y de la Fase 8

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 11), docs/fases/resultados-fase-8e.md y docs/fases/resumen-fase-8e-que-sigue.md.
Cierra la ronda 8e y con ella la Fase 8: contrasta los resultados con lo esperado en 11.5 (Cordón… sobre N9 ya no cierra
Revit; Descartar cierra la ventana; la etiqueta A del sondeo 19 se vio y se pinchó; el puente se repitió en 8e-7b).
Explica por qué tras el clic en la etiqueta los sondeos no pudieron hablar con Revit y deja el sondeo 19 de forma que su
cuadro no bloquee a Revit (o que no abra cuadro y solo escriba en el log). Corrige la etiqueta B del sondeo (el error de
ElementId al calcular el punto). Actualiza el informe (sección 12), la tabla de garantías del README y
docs/propuestas/flujo-intuitivo.md (qué quedó hecho y qué pasa a las Fases 10 y 11). No empieces la Fase 9. Termina con
compilación sin avisos, pruebas en verde, commit, push y un resumen corto guardado también en
docs/fases/resumen-fase-8e-cierre.md.
```

### Paso 3: Fase 9

Con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`. Antes decide:

- **P6**: los 14 nudos con cordón HSS4X4, ¿se crean con la cartela del Detalle D o se dejan fuera?
- Los 10 nudos del cordón superior sin plantilla: ¿se excluyen, o creas uno a mano y lo guardas como plantilla antes?
