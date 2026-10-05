# Ronda 8d: qué sigue y prompts (copia del chat, 2026-10-05)

Rama `main`, commit `43027cb`. Add-in **0.8.4** (ventana no modal, Ver en Revit sin cuadro, descartar limpio, sondeo 19 con
BMP). Comprobado por mí en la nube bajando `main` desde cero: build sin avisos, 177/177 pruebas.

## Qué sigue, en orden

1. **Probar la 0.8.4 en el PC** con tu instalador: `docs\instalacion\fase-8d.md` (unos 35 minutos). Es lo único que falta
   para dar por cerrada la Fase 8: la ventana que se queda abierta mientras orbitas (paso 8d-3), Ver en Revit con zoom y
   sin cuadro, Descartar que quita los cubos (8d-4), catálogo vacío (8d-5) y la etiqueta BMP pinchable (8d-6).
2. **Sesión corta de cierre** en Claude Code con los resultados (prompt en la sección 3).
3. **Fase 9** (crear por lotes), con el prompt de siempre (paso 6 de la sección 6 de `docs/fases/fase-8.md`). Antes decide:
   - **P6**: los 14 nudos con cordón HSS4X4, ¿se crean con la cartela del Detalle D o se dejan fuera?
   - Los 10 nudos del cordón superior (dos diagonales, HSS12X8) no tienen plantilla: si quieres conectarlos en el lote,
     primero crea uno a mano y guárdalo con *Guardar en catálogo*; si no, se excluyen.

## 1. Antes de pegar el prompt (la persona)

- Cierra Revit. Usa la copia `HANGAR_PRUEBA_sondeo.rvt`, nunca el original.
- Los pasos marcados **(la persona)** los haces tú en Revit; esta vez **la ventana se queda abierta** mientras orbitas, y el
  instalador solo mira o ejecuta lo que el paso le marque.

## 2. Prompt para el agente instalador en el PC

```
Lee docs\instalacion\fase-8d.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, en una sola
ventana de PowerShell. Reglas: usa la función Anota tal como está escrita (no crees scripts nuevos); repite
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force si abres otra ventana; Revit lo abre y lo cierra la
persona, no tú; los pasos marcados (la persona) los hace la persona en Revit y tú esperas a que te diga que terminó; en
el paso 8d-4 la persona y tú actuáis a la vez, sigue el orden literal del archivo; no toques src\, config\, mcp\ ni
docs\fixtures\; si un paso falla, copia el error, no modifiques nada y sigue con el siguiente. La variable del plan se
llama $plan, no $pid. Obligatorio: en el paso 8d-8 la salida de probar_conexiones.py --puente y el log del día van
dentro del archivo docs\fases\resultados-fase-8d.md con Anota; antes del commit comprueba con Select-String que el
archivo tiene las secciones "8d-8" del puente y del log, y si faltan, repítelas. Guarda las capturas con los nombres
exactos del archivo en docs\fases\capturas\. Al terminar, haz el commit y el push del paso 8d-9 y devuélveme: si la
ventana siguió abierta mientras la persona orbitaba y pinchaba (8d-3), si Ver en Revit hizo zoom sin ningún cuadro, si
al Descartar desde la ventana quedaron 0 cubos (8d-4), la salida de los sondeos 17 y 19 (y si la etiqueta se pudo
pinchar), el resultado de --puente (x/28), las anotaciones de la persona, el git status --short literal antes del
commit y al final, el git log -1 --oneline y el texto de cualquier ventana de error.
```

## 3. Después: prompt de cierre (Claude Code, sesión nueva en la nube)

Solo cuando el instalador haya subido `docs/fases/resultados-fase-8d.md` y las capturas.

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 10) y docs/fases/resultados-fase-8d.md. Cierra la ronda 8d y con ella la
Fase 8: contrasta los resultados con lo esperado en la sección 10, corrige lo que haga falta (otra ronda solo si es
imprescindible), actualiza el informe, la tabla de garantías del README y docs/propuestas/flujo-intuitivo.md (qué quedó
hecho). No empieces la Fase 9. Termina con commit, push y un resumen corto guardado también en
docs/fases/resumen-fase-8d-cierre.md.
```

## Regla de siempre

- "Lee docs\instalacion\..." o PowerShell → **tu agente instalador en el PC**.
- "Lee CLAUDE.md..." → **Claude Code**, sesión nueva en la nube.
