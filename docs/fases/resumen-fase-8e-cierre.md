# Resumen del cierre de la ronda 8e y de la Fase 8 (2026-10-06)

Rama `main`. Add-in **0.8.5** (sin cambios: es la que está en el PC). Informe: `docs/fases/fase-8.md`, sección 12. Copia
corta del mensaje del chat.

## 1. Qué volvió de la 8e y qué significa

- **Lo bueno, y es lo importante**: con la 0.8.5, **Cordón… sobre N9 ya no cierra Revit**. Lo pulsaste seis veces (una
  cancelada con Esc) y Barras… dos, siempre con la ventana del plan oculta mientras pinchabas y de vuelta después; el log
  tiene 9 pinchados empezados y 9 terminados, y ningún error de ventana. Descartar cerró la ventana y dejó 0 marcadores.
  Excluir / Incluir y Replanificar con la ventana abierta, bien. Sondeos 17, 12 y 13 en cero.
- **La etiqueta del sondeo 19 se vio y se pudo pinchar** (capturas `fase8e-01-etiqueta.png` y
  `fase8e-01-etiqueta-clic.png`, cuatro clics anotados). Eso decide **V3 (etiquetas con número en la vista) para la
  Fase 10: sí**.
- **El puente**: 0/1 el 2026-10-05 porque Revit ya estaba cerrado; repetido el 2026-10-06 con Revit abierto, **28/28**.
- **Lo que pasó tras pinchar la etiqueta**: el manejador abría un cuadro (`TaskDialog`). Ese cuadro es modal y vive en el
  hilo de Revit, así que mientras estuvo abierto Revit no atendió a pyRevit: los sondeos 19b y 17 agotaron sus 300 s
  ("Se canceló una tarea") y el 12 tardó 275 s (lo que tardaste en cerrar el cuadro) y luego salió bien. **Revit no se
  cerró ni se colgó**: el log no tiene ningún arranque hasta la mañana siguiente.
- **La etiqueta B no se puso** por un error del sondeo (`ElementId(1249510)` es ambiguo en IronPython: Revit tiene tres
  constructores), no de Revit.
- Un detalle para la Fase 9: tras elegir con Cordón… un tramo del cordón superior en N9, el nudo se quedó en
  "✖ Falta el cordón" (es lo previsto en 10.2: ese tramo termina en el nudo), pero el consejo sigue diciendo "selecciónalo
  y replanifica, o Cordón…", un callejón sin salida. Se arregla con los botones de *Qué hacer* de la Fase 9.

## 2. Qué se corrigió (solo el sondeo; el add-in sigue en 0.8.5)

1. **Sondeo 19 v4** (`scripts/sondeos/19-etiquetas-lienzo.py`): la etiqueta B con `ElementId(System.Int64(…))`; el clic
   **no abre ningún cuadro**: escribe la línea en `sondeo19-clics.txt` y cambia la etiqueta pinchada por su versión
   naranja (`UpdateControl`, firma comprobada contra la API 2027), para que veas que el clic llegó; el archivo de clics se
   vacía al empezar.
2. **Sondeo 19b**: solo quita los controles que Revit dice tener (tras reiniciar Revit no queda ninguno) y borra también los
   BMP naranjas.
3. Informe (sección 12), README (estado, resumen y tabla de garantías) y `docs/propuestas/flujo-intuitivo.md` (C7 probada,
   V3 decidida, Fase 10) al día.

En la nube: build sin avisos, 177/177 pruebas, simulador 47/47, puente simulado 26/26, firmas de `UpdateControl`,
`InCanvasControlData`, `ElementId` y `TemporaryGraphicsCommandData` comprobadas contra `RevitAPI.dll` 2027.2.0.
**El sondeo 19 v4 NO está probado en Revit** (no hay Revit en la nube).

## 3. Qué sigue

1. **Fase 9 (crear el lote)**, en una sesión nueva de Claude Code, con este prompt (es el del paso 6 de la sección 6 de
   `docs/fases/fase-8.md`):

   ```
   Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-8.md y docs/propuestas/catalogo-y-lotes.md completo.
   Escribe docs/prompts/fase-9.md (crear por lotes: secciones 3.5, 4 y 6 de la propuesta, con las decisiones de 7.1)
   y ejecuta SOLO la Fase 9. Incluye en docs/instalacion/fase-9.md las comprobaciones pendientes de la sección 8.5 de
   docs/fases/fase-8.md (add-in 0.8.2). Termina con docs/fases/fase-9.md, docs/instalacion/fase-9.md, commit, push y un resumen corto.
   ```

   Antes de lanzarla, decide y dilo en el mismo prompt (o en el chat):
   - **P6**: los 14 nudos con cordón HSS4X4, ¿se crean con la cartela del Detalle D (con el aviso de perfil) o se dejan fuera?
   - Los 10 nudos del cordón superior sin plantilla y los 7 empalmes: ¿se excluyen, o creas uno a mano en Revit y lo guardas
     como plantilla antes?
   - Si quieres que la Fase 9 incluya también la mejora C3 (botones en *Qué hacer*) y V2 (cartelas fantasma) de
     `docs/propuestas/flujo-intuitivo.md`, dilo; si no, solo `conn_batch_create` y el botón *Crear N conexiones*.

2. **Sondeo 19 v4 en el PC** (opcional ahora, obligatorio antes de la Fase 10; 5 minutos). Con Revit abierto en la vista 3D
   con el Detalle D visible y ninguna ventana del add-in, pásale esto al instalador:

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   git pull origin main
   $salida = "docs\fases\resultados-fase-8e.md"
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   Anota "8e-9 sondeo 19 v4" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19-etiquetas-lienzo.py -SinTransaccion -TimeoutSec 300 }
   # (la persona) mira la 4 verde en N4 y la B azul; pincha la 4: NO debe salir ningún cuadro y la 4 pasa a naranja. Avisa al instalador.
   Anota "8e-9 sondeo 19b" { .\scripts\revit-exec.ps1 -File scripts\sondeos\19b-etiquetas-quitar.py -SinTransaccion -TimeoutSec 300 }
   git add $salida
   git commit -m "Ronda 8e: sondeo 19 v4 (etiqueta B y clic sin cuadro)"
   git push origin main
   ```

   Se espera: cuatro BMP de 24 bits, dos controles y `GetAll(): 2`, y tras el clic `UpdateControl(0): la etiqueta A pasa a
   naranja (el clic llego)` en el bloque de 19b, con `GetAll() despues de quitar: 0`. El 19b tiene que responder al momento.
