# ¿El cierre de la 8c corrige lo de la navegación 3D? Sí (copia del chat, 2026-10-05)

Archivo con el prompt actualizado: `docs/fases/resumen-fase-8c-que-sigue.md`, sección 3 (commit `4daf2c0`).

## Qué te pasó y por qué

1. **El aviso al pulsar "Ver en Revit"** lo saca Revit, no el add-in. El botón usa una función de Revit (`ShowElements`)
   que siempre abre ese cuadro ("Cerrar"). **Se cambia** por un zoom directo al nudo, sin cuadro.
2. **No poder orbitar** es porque la ventana del plan es **modal**: bloquea Revit, y cuando se cierra para "Ver en Revit",
   el comando la vuelve a abrir enseguida. Estaba previsto arreglarlo en la Fase 10 (mejora C7 de la propuesta), pero
   como es lo que más te molesta, **se adelanta a este cierre**: la ventana pasa a ser **no modal** (con `ExternalEvent`).
   Se queda abierta a un lado, orbitas y pinchas en el modelo cuando quieras, y "Ver en Revit" solo hace zoom sin cerrar
   nada.

## Qué más corrige el cierre

- Descartar desde la ventana deja los cubos en el modelo: se corrige.
- El sondeo 19 se repite con imagen BMP (Revit no acepta PNG).
- Se explica por qué los 10 nudos de arriba y abajo salen "sin plantilla que encaje" (el Detalle D es la típica del
  cordón central).

## Si no cabe en una sesión

La ventana no modal es el cambio más grande. El prompt dice que, si no cabe, entregue el resto y deje la ventana no modal
en una ronda **8d** inmediata, antes de la Fase 9. En los dos casos saldrá `docs\instalacion\fase-8d.md` para probarlo
en el PC con tu instalador.

## Prompt para pegar en Claude Code (sesión nueva en la nube)

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 9), docs/fases/resultados-fase-8c.md, docs/fases/resumen-fase-8c-que-sigue.md
y docs/propuestas/flujo-intuitivo.md (C7). Cierra la ronda 8c: contrasta los resultados con lo esperado en 9.3 y corrige
cuatro cosas. (1) La ventana del plan pasa a ser NO MODAL con ExternalEvent: se queda abierta mientras la persona orbita y
pincha en Revit; todo lo que toca el modelo (replanificar, marcas, descartar, Ver en Revit, elegir en Revit) va por el
ExternalEvent; la ventana de previsualización (Editar nudo) puede seguir modal. (2) Ver en Revit hace zoom al nudo sin
ShowElements ni ningún cuadro de Revit y sin cerrar la ventana. (3) Descartar desde la ventana quita también los
marcadores (cubos), igual que batch_plan_discard por el puente. (4) El sondeo 19 usa BMP y busca el manejador de clics
con el nombre correcto. Explica en el informe los 10 nudos sin plantilla de los cordones superior e inferior. Si la
ventana no modal no cabe en la sesión, entrega el resto, deja docs/prompts/fase-8d.md solo con ella y dilo. Actualiza el
informe, el README (sección 12 y tabla de garantías) y la guía. No empieces la Fase 9. Termina con commit, push,
docs/instalacion/fase-8d.md para probar en el PC y un resumen corto guardado también en docs/fases/resumen-fase-8c-cierre.md.
```

## Después

Cuando esa sesión termine, pasas `docs\instalacion\fase-8d.md` a tu instalador. Con sus resultados, otra sesión corta de
cierre y, solo entonces, la Fase 9.
