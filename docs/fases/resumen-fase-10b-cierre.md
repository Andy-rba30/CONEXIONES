# Resumen del cierre de la ronda 10b y de la Fase 10 (2026-10-11): la 0.10.1 contrastada en Revit

Rama `main`. Add-in **0.10.1**, ya instalado en el PC; **este cierre no cambia el add-in** (no hay ronda 10c, nada que
desplegar). Informe: `docs/fases/fase-10.md`, sección 8. Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-10b.md` y las anotaciones de la persona, 2026-10-11)

Todo lo que la ronda tenía que probar salió como se esperaba (`fase-10.md` 8.1):

- **Planificar lote desde la cinta con una barra**: el cuadro "Selección asistida: 63 barras tocan la selección" salió con su
  texto literal y sus dos opciones (captura `fase10b-01`); Planificar con las 64 dio los mismos 16 listos (`fase10b-02`);
  Cancelar no tocó nada; Solo la selección con una barra dio el cuadro de "una sola barra" y conservó el plan (`fase10b-05`).
  Ningún `ribbon_batch_plan_failed` ni `ribbon_batch_assist_dialog_failed` en el día: **lo que corregía la 0.10.1 funciona**.
- **Etiquetas**: las 33 se vieron con sus colores (`fase10b-03`), **la 4 se pinchó sin ningún cuadro** y la ventana eligió N4
  (`fase10b-04`, `label_clicked … highlighted: true, window_open: true`); otras siete con la ventana abierta y N23 con ella
  cerrada; el resaltado desde la tabla (N7) y desde el mapa (12) al momento; Descartar plan dejó 0 marcas y 0 etiquetas.
- **`--puente` 32/32** con la copia como único proyecto abierto y el servidor del puerto 8000 arrancado (el instalador lo
  levantó: no estaba).
- Catálogo con 1 plantilla (la sobrante borrada por el puente), sondeos 12, 13, 17 y 21 a cero, log sin errores.

## 2. Lo que no coincidió (cosmético; va de paso en la Fase 11, `fase-10.md` 8.2)

- La línea del plan no dice "(63 añadidas por la selección asistida)" cuando el plan sale del botón (el botón amplía la
  selección antes de planificar y el plan no guarda la expansión); la barra de estado sí lo dice.
- `highlighted: false` en los clics repetidos sobre la misma etiqueta significa "no cambió nada", no un fallo; el campo pasa
  a `changed` en la Fase 11.
- Los títulos de los cuadros salen "MotorConexiones - MotorConexiones - …" (Revit antepone el nombre del add-in).
- Sin anotar: el globo, la fila elegida al reabrir la ventana y el texto literal de la barra de estado tras pinchar la 4.
- El sondeo 17 pisó la captura de la Fase 8 (`fase8-01-sondeo17.png`) y el commit del instalador la subió así.

## 3. Qué cambió en el repositorio (sin tocar `src/`)

- Restaurada `docs/fases/capturas/fase8-01-sondeo17.png` (la de la Fase 8).
- `scripts/sondeos/17-marcas-plan.py` exporta ahora a `fase8-01-sondeo17-exportada.png` y solo borra `*-exportada*.png`
  (`py_compile` correcto; NO PROBADO en Revit: solo cambian rutas).
- Documentación al día: `fase-10.md` (sección 8 y notas en 7.6, 7.7 y 7.10), `docs/instalacion/fase-10b.md` (nota de hecha),
  README (estado, tabla de garantías y sección 12), `docs/propuestas/flujo-intuitivo.md` (C6, V2 y V3 hechos y probados).
- Decisiones: P1 (posición de las etiquetas) y P3 (la ventana al pinchar) se quedan como están; el servidor del 8000 se
  arranca siempre antes del `--puente`; los sondeos que exportan imagen escriben con sufijo `-exportada`.

## 4. Qué se probó en la nube

`dotnet build MotorConexiones.sln -c Release` sin avisos, `dotnet test` 217/217 (nada cambió en el código: es la comprobación
de rigor) y `py_compile` del sondeo 17.

## 5. Qué sigue

**La Fase 10 está cerrada del todo y no hay nada que instalar.** Lo siguiente es la **Fase 11, "Conectar un nudo"**: botón
*Conectar* que reconoce el nudo seleccionado y propone la plantilla sin JSON, panel por secciones con desplegables que ponen
los mm solos, V4 (catálogo con miniaturas), V5 (el plano al lado del croquis) y los tres retoques de paso. Primero se escribe
el prompt de la fase a partir de la propuesta. Prompt para esa sesión:

```
Lee CLAUDE.md, docs/propuestas/flujo-intuitivo.md (la sección "Y para un nudo suelto", las filas V4 y V5 y la fila 11 del
calendario), docs/fases/fase-10.md (secciones 8.2 y 8.5) y docs/prompts/fase-10.md como modelo. Escribe docs/prompts/fase-11.md:
el prompt y el alcance de la Fase 11 "Conectar un nudo" (botón Conectar sin JSON, panel por secciones, V4 miniaturas, V5 plano
al lado, y los tres retoques de paso de la 8.5), con qué entra y qué no, qué se prueba en la nube y qué en el PC, y las dudas
que debo aclarar contigo antes de programar. Sin código. Termina con commit, push y un resumen corto.
```
