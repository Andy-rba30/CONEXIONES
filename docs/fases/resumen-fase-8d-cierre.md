# Resumen del cierre de la ronda 8d (2026-10-06)

Rama `main`. Add-in **0.8.5** (`docs/fases/fase-8.md`, sección 11). Copia corta del mensaje del chat.

## 1. Qué volvió de la 8d y qué significa

- **Lo bueno**: la ventana del plan se quedó abierta mientras orbitabas y pinchabas (lo que pediste), Descartar dejó 0 cubos
  con un plan del puente marcado, el catálogo vacío avisó con la ventana abierta, `--puente` 28/28, sondeos limpios.
- **Lo malo**: Revit se cerró con "fatal error" al pulsar **Cordón…** sobre N9; **Descartar plan** no cerraba la ventana; la
  etiqueta del sondeo 19 no se vio aunque Revit aceptó el control.
- **La causa del cierre de Revit no era el pinchado**: la captura `fase8d-crash-revit.png` enseña la ventana en reposo (los
  botones encendidos, nada de "⏳ Pincha en Revit…"). Era el **diálogo de elección** (`ChooseDialog`): en modo de una sola
  elección marcaba el cordón actual con `SelectedItems.Add`, que WPF no admite; la misma excepción ya estaba en el log de la
  8c (17:32:54), donde la ventana modal la tapaba. Con la ventana no modal nadie la capturaba y Revit moría. Fallaba en
  **todos** los nudos con cordón (y Plantilla… igual), no solo en N9.

## 2. Qué se corrigió (0.8.5)

1. **El diálogo** marca con `SelectedItem` en modo de una elección (la causa).
2. **Ninguna excepción de la ventana llega a Revit**: cada manejador va dentro de una red (`Guard`): el error sale en rojo en
   la barra de estado y en el log (`ribbon_batch_window_error`), y la ventana sigue. Otra red en el despachador recoge solo
   las excepciones de este add-in.
3. **Pinchar en Revit** (Cordón…, Barras…, Añadir nudo…) se hace con la ventana del plan **oculta** y la ventana principal de
   Revit activada; al terminar (o con Esc) la ventana vuelve sola. En el log, una línea antes (`ribbon_batch_pick`) y otra
   después (`ribbon_batch_picked` / `ribbon_batch_pick_failed`).
4. **Descartar plan cierra la ventana** (antes `Close()` se llamaba con la ventana ocupada y se cancelaba solo).
5. **Sondeo 19 v3**: BMP de 24 bits de 32×32 (el de la 8d era de 32 bits), ruta sin tildes (`C:\IA\MotorConexiones-sondeo19`),
   posición en pies, `SetVisibility`, `GetAll`, refresco de la vista y **dos etiquetas** (la 4 en N4 y una B en el centro de
   la caja de sección) para saber si el problema es el control o el punto. Miembros comprobados contra la API 2027.

En la nube: build sin avisos, 177/177 pruebas, simulador 47/47, puente simulado 26/26. **NO PROBADO en Revit.**

## 3. Qué sigue

1. **Pasar `docs\instalacion\fase-8e.md` al instalador** (unos 25 minutos; Revit cerrado antes del paso 8e-1). Lo primero que
   mira es Cordón… sobre N9 con la ventana abierta: Revit tiene que seguir vivo.
2. Sesión corta de cierre de la 8e en Claude Code con `docs/fases/resultados-fase-8e.md`.
3. **Fase 9** (crear el lote) con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`. Antes decide P6 (los 14
   nudos con cordón HSS4X4) y qué hacer con los 10 nudos del cordón superior sin plantilla.

## 4. Prompt para el agente instalador en el PC

```
Lee docs\instalacion\fase-8e.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, en una sola
ventana de PowerShell. Reglas: usa la función Anota tal como está escrita (no crees scripts nuevos); repite
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force si abres otra ventana; Revit lo abre y lo cierra la
persona, no tú; los pasos marcados (la persona) los hace la persona en Revit y tú esperas a que te diga que terminó; no
toques src\, config\, mcp\ ni docs\fixtures\; si un paso falla, copia el error, no modifiques nada y sigue con el
siguiente; si Revit se cierra, anota en qué botón y sigue. Obligatorio: en el paso 8e-7 la salida de
probar_conexiones.py --puente y el log del día van dentro de docs\fases\resultados-fase-8e.md con Anota; antes del
commit comprueba con Select-String que el archivo tiene las dos secciones "8e-7". Guarda las capturas con los nombres
exactos del archivo en docs\fases\capturas\. Al terminar, haz el commit y el push del paso 8e-8 y devuélveme: si Revit
siguió vivo con Cordón… sobre N9, si la ventana se ocultó al pinchar y volvió, si Descartar cerró la ventana, si Ver en
Revit sacó algún cuadro, cuál de las dos etiquetas del sondeo 19 se vio, el resultado de --puente (x/28), las
anotaciones de la persona, el git status --short literal antes del commit y al final, el git log -1 --oneline y el
texto de cualquier ventana de error.
```

## 5. Prompt de cierre (Claude Code, sesión nueva en la nube), solo cuando el instalador haya subido los resultados

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 11) y docs/fases/resultados-fase-8e.md. Cierra la ronda 8e: contrasta los
resultados con lo esperado en 11.5, corrige lo que haga falta (otra ronda solo si es imprescindible), actualiza el
informe (sección 12), el README y docs/propuestas/flujo-intuitivo.md. No empieces la Fase 9. Termina con compilación sin
avisos, pruebas en verde, commit, push y un resumen corto guardado también en docs/fases/resumen-fase-8e-cierre.md.
```
