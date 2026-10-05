# Qué sigue tras los resultados de la ronda 8c (2026-10-05)

Rama `main`. Commits del instalador: `d666087` (resultados) y `9628e32` (capturas). Copia del mensaje del chat.

## 1. Qué volvió del PC, en corto

| Qué | Resultado | Bien o mal |
|---|---|---|
| Elementos seleccionados | **64** (el instalador dijo 56; el plan dice 64: sí entraron cordones superior e inferior) | Bien |
| Plan | 33 nudos visibles. **16 listos** (8 iguales, 8 en espejo), 14 con aviso de perfil, 10 sin plantilla que encaje (arriba y abajo: el Detalle D no es su típica), 7 con el cordón sin seleccionar. Ocultos: 8 sin cordón y 18 barras sueltas | Bien: es lo esperado con los cordones de arriba y abajo |
| Cabecera `summary_text` | "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo)…" | Bien |
| Mapa y colores por estado | Capturas `fase8c-01-colores-estado.png` y `fase8c-02-mapa.png` | Bien |
| Replan (excluir, incluir, overrides devuelto) | Mismo plan, 15 listos con uno excluido y vuelta a 16 | Bien |
| Catálogo vacío | Aviso `CATALOG_EMPTY` y cercha en rojo | Bien |
| `discard all` por el puente | 66 marcadores quitados, 0 restantes; sondeos 17, 12 y 13 limpios | Bien |
| `probar_conexiones.py --puente` | **28/28**, y esta vez anotado en el archivo con el log | Bien |
| Sondeo 19 (etiquetas pinchables) | Revit tiene `TemporaryGraphicsManager` e `InCanvasControlData`, pero **solo acepta imágenes BMP**, no PNG. `ITemporaryGraphicsHandler` no aparece con ese nombre | A medias: hay que repetirlo con BMP |
| Tus anotaciones | (1) Al descartar desde la ventana se fueron los colores pero **los cubos se quedaron**, en gris. (2) **Ver en Revit** sacó un aviso. (3) Con la ventana abierta no se puede orbitar | (1) y (2) son fallos a corregir; (3) es la ventana modal, prevista para la Fase 10 |

## 2. Lo que tiene que resolver la sesión de cierre

- **Descartar desde la ventana deja los cubos**: el botón quita los colores pero no los marcadores (por el puente sí se
  quitaron los 66). Hay que corregirlo y probarlo.
- **El aviso de "Ver en Revit"**: averiguar qué dice (si lo recuerdas, anótalo al final de
  `docs\fases\resultados-fase-8c.md`) y quitarlo.
- **Sondeo 19 con BMP** y con el nombre correcto del manejador de clics, para decidir las etiquetas en la Fase 10.
- Explicar en el informe por qué los 10 nudos de arriba y abajo salen "sin plantilla que encaje": el Detalle D es la
  típica del cordón central; para los de arriba y abajo haría falta otra plantilla.

## 3. Prompt de cierre (Claude Code, sesión nueva en la nube)

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 9), docs/fases/resultados-fase-8c.md y docs/fases/resumen-fase-8c-que-sigue.md.
Cierra la ronda 8c: contrasta los resultados con lo esperado en 9.3, corrige que Descartar desde la ventana deje los
marcadores (cubos) en el modelo, quita el aviso de Ver en Revit, reescribe el sondeo 19 para que use BMP y busque el
manejador de clics con el nombre correcto, y explica los 10 nudos sin plantilla de los cordones superior e inferior.
Actualiza el informe y la tabla de garantías del README. No empieces la Fase 9. Termina con commit, push y un resumen
corto; guárdalo también en docs/fases/resumen-fase-8c-cierre.md y, si hace falta otra pasada en el PC,
docs/instalacion/fase-8d.md.
```

## 4. Después del cierre

La Fase 9 con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`. Antes decide P6: si los 14 nudos con cordón
HSS4X4 se crean con la cartela del Detalle D o se dejan fuera. Para los 10 de arriba y abajo, si quieres conectarlos en el
lote, primero crea uno a mano y guárdalo como plantilla.
