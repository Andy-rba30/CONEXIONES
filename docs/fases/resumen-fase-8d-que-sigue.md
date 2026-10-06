# Qué sigue tras los resultados de la ronda 8d (2026-10-06)

Rama `main`. Commit del instalador: `4c72a2c`. Copia del mensaje del chat.

## 1. Qué volvió del PC, en corto

| Qué | Resultado | Bien o mal |
|---|---|---|
| Ventana del plan abierta mientras orbitas y pinchas (8d-3) | **Sí**, captura `fase8d-02-ventana-abierta.png` | **Bien: era lo que pediste** |
| Ver en Revit sin cuadro | El instalador lo da por bueno pero no lo vio directamente ("asumido") | A confirmar en la siguiente pasada |
| Descartar desde la ventana con marcas de otro plan (8d-4) | 0 cubos, `remaining_markers: 0`; sondeo 17 limpio | Bien |
| Catálogo vacío con la ventana abierta (8d-5) | Aviso y colores en rojo | Bien |
| Sondeo 19 v2 (etiqueta BMP) | Revit aceptó el BMP, puso el control (índice 0) y registró el manejador de clics sin error. **Pero la etiqueta no se ve en pantalla** (`fase8d-01-etiqueta.png`), así que no se pudo pinchar. 19b la quitó bien | A medias: la API funciona, el dibujo no aparece |
| Sondeos 12 y 13; `--puente` | Limpios; **28/28**, anotado en el archivo con el log | Bien |
| **Tu anotación 1: Revit se cerró** ("fatal error") al hacer clic derecho en un nudo → **Cordón…** → pinchar el cordón en el modelo. Captura `fase8d-crash-revit.png` | **Fallo grave**, hay que corregirlo antes de la Fase 9 | Mal |
| Tu anotación 2: Más… → Descartar plan → Sí: quita los cubos pero **la ventana no se cierra sola** | Fallo pequeño | Mal |

## 2. Qué tiene que resolver la sesión de cierre

- **El cierre inesperado de Revit al pinchar el cordón.** Es la combinación nueva de esta ronda: la ventana ya no es
  modal y el "pinchar en Revit" (`PickObject`) se ejecuta dentro del `ExternalEvent` con la ventana abierta. Revit no
  tolera bien pinchar con una ventana no modal activa encima. La corrección esperable: antes de pinchar, ocultar (o
  apagar) la ventana del plan y activar la ventana principal de Revit; pinchar; volver a mostrar la ventana; y proteger
  el pinchado con captura de errores y una línea en el log antes y después, para que si vuelve a pasar sepamos dónde.
  Lo mismo para **Barras…** y **Añadir nudo…**, que pinchan igual.
- **Descartar plan debe cerrar la ventana** (o dejarla vacía con el aviso "plan descartado").
- **Ver en Revit**: comprobarlo a propósito en la siguiente pasada (el instalador no lo vio).
- **Etiqueta invisible (sondeo 19)**: la API acepta el control pero no se dibuja. Probar en el sondeo: refrescar la vista
  después de añadirla, `SetVisibility`, un BMP de 24 bits sin transparencia y de tamaño pequeño (32×32), y la posición en
  pies (unidades internas de Revit), no en mm. Sigue siendo sondeo, no bloquea la Fase 9.

Como el fallo grave necesita probarse en Revit, saldrá **`docs\instalacion\fase-8e.md`** (corta: pinchar cordón y barras
con la ventana abierta, Descartar, Ver en Revit, sondeo 19 v3).

## 3. Prompt de cierre (Claude Code, sesión nueva en la nube)

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 10), docs/fases/resultados-fase-8d.md y docs/fases/resumen-fase-8d-que-sigue.md.
Cierra la ronda 8d: contrasta los resultados con lo esperado en la sección 10 y corrige: (1) Revit se cierra con "fatal
error" al pinchar el cordón desde Cordón… con la ventana del plan no modal abierta (también Barras… y Añadir nudo…):
antes de PickObject oculta la ventana del plan y activa la ventana principal de Revit, pincha, vuelve a mostrarla,
captura cualquier excepción y escribe en el log una línea antes y otra después del pinchado; (2) Descartar plan desde la
ventana cierra la ventana; (3) sondeo 19 v3 para que la etiqueta se vea: refresco de la vista tras AddControl,
SetVisibility, BMP de 24 bits de 32×32 y posición en unidades internas de Revit, sin inventar miembros de la API.
Actualiza el informe (sección 11), el README y la propuesta. No empieces la Fase 9. Termina con compilación sin avisos,
pruebas en verde, commit, push, docs/instalacion/fase-8e.md (corta: pinchar cordón y barras con la ventana abierta,
Descartar, Ver en Revit a propósito, sondeo 19 v3) y un resumen corto guardado también en docs/fases/resumen-fase-8d-cierre.md.
```

## 4. Después

1. Pasar `docs\instalacion\fase-8e.md` al instalador (misma regla de siempre: cierra Revit antes).
2. Sesión corta de cierre de la 8e.
3. **Fase 9** con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`. Antes decide P6 (los 14 nudos con
   cordón HSS4X4) y qué hacer con los 10 nudos del cordón superior sin plantilla.
