# Ronda 8e: qué sigue y prompts (copia del chat, 2026-10-06)

Rama `main`, commit `a953b63`. Add-in **0.8.5**. Comprobado por mí en la nube bajando `main` desde cero: build sin avisos,
177/177 pruebas, Python compila.

## Qué pasó en la 8d, en una frase

Revit no se cerró por pinchar: se cerró por el **diálogo de elección** (Cordón…, Plantilla…), que marcaba el cordón actual
con una llamada que la ventana no admite. Con la ventana modal ese error quedaba tapado; con la no modal, mataba a Revit.
Ya está corregido, y además ahora cualquier error de la ventana se escribe en el log y se enseña en rojo en vez de cerrar
Revit. Descartar ya cierra la ventana. El sondeo 19 va en versión 3 (BMP de 24 bits, ruta sin tildes, dos etiquetas).

## Qué sigue, en orden

1. **Probar la 0.8.5 en el PC**: `docs\instalacion\fase-8e.md` (unos 25 minutos). Lo primero que se prueba es **Cordón…
   sobre N9 con la ventana abierta**, que es lo que cerraba Revit. Después Barras…, Añadir nudo…, Ver en Revit a propósito,
   Descartar (debe cerrar la ventana) y las dos etiquetas del sondeo 19.
2. **Sesión corta de cierre** en Claude Code (sección 3). Si todo sale bien, con ella queda cerrada la Fase 8 entera.
3. **Fase 9** (crear por lotes). Antes decide P6 (los 14 nudos con cordón HSS4X4) y qué hacer con los 10 nudos del cordón
   superior sin plantilla.

## 1. Antes de pegar el prompt (la persona)

- Cierra Revit. Usa la copia `HANGAR_PRUEBA_sondeo.rvt`, nunca el original.
- Los pasos marcados **(la persona)** los haces tú en Revit con la ventana abierta; el instalador solo mira o ejecuta lo
  que el paso le marque. Si Revit volviera a cerrarse, anota exactamente qué botón pulsaste y en qué nudo.

## 2. Prompt para el agente instalador en el PC

```
Lee docs\instalacion\fase-8e.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, en una sola
ventana de PowerShell. Reglas: usa la función Anota tal como está escrita (no crees scripts nuevos); repite
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force si abres otra ventana; Revit lo abre y lo cierra la
persona, no tú; los pasos marcados (la persona) los hace la persona en Revit y tú esperas a que te diga que terminó; no
toques src\, config\, mcp\ ni docs\fixtures\; si un paso falla, copia el error, no modifiques nada y sigue con el
siguiente. La variable del plan se llama $plan, no $pid. Si Revit se cierra solo, anótalo con el paso y el botón exactos,
espera a que la persona lo vuelva a abrir y sigue. Obligatorio: en el paso 8e-7 la salida de probar_conexiones.py
--puente y el log del día van dentro del archivo docs\fases\resultados-fase-8e.md con Anota; antes del commit comprueba
con Select-String que el archivo tiene las secciones "8e-7" del puente y del log, y si faltan, repítelas. Guarda las
capturas con los nombres exactos del archivo en docs\fases\capturas\. Al terminar, haz el commit y el push del paso 8e-8
y devuélveme: si Cordón… sobre N9, Barras… y Añadir nudo… funcionaron con la ventana abierta y sin cerrar Revit; si Ver
en Revit hizo zoom sin ningún cuadro; si Descartar cerró la ventana; si las dos etiquetas del sondeo 19 se vieron y se
pudieron pinchar; el resultado de --puente (x/28); las anotaciones de la persona; el git status --short literal antes
del commit y al final; el git log -1 --oneline y el texto de cualquier ventana de error.
```

## 3. Después: prompt de cierre (Claude Code, sesión nueva en la nube)

Solo cuando el instalador haya subido `docs/fases/resultados-fase-8e.md` y las capturas.

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 11) y docs/fases/resultados-fase-8e.md. Cierra la ronda 8e y con ella la
Fase 8: contrasta los resultados con lo esperado en la sección 11, corrige lo que haga falta (otra ronda solo si es
imprescindible), actualiza el informe, la tabla de garantías del README y docs/propuestas/flujo-intuitivo.md (qué quedó
hecho y qué pasa a las Fases 10 y 11). No empieces la Fase 9. Termina con commit, push y un resumen corto guardado
también en docs/fases/resumen-fase-8e-cierre.md.
```

## Regla de siempre

- "Lee docs\instalacion\..." o PowerShell → **tu agente instalador en el PC**.
- "Lee CLAUDE.md..." → **Claude Code**, sesión nueva en la nube.
