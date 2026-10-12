# Resumen del cierre de la Fase 11 (2026-10-12): la 0.11.0 contrastada en Revit

Rama `main`. Add-in **0.11.0**, ya instalado en el PC; **este cierre no cambia el add-in** (no hay ronda 11b, nada que
desplegar). Informe: `docs/fases/fase-11.md`, sección 7. Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-11.md`, 2026-10-12)

Todo lo programado funciona en Revit (`fase-11.md` 7.1):

- **Conectar con plantilla** sobre el Detalle D: la ventana se abrió sin pedir archivo con "Plantilla propuesta: Nudo tipico
  Detalle D · igual · desvío máx. 0,0°"; el panel escribió `1/2" · 12,7 mm` y pernos `3/4"` sin errores, Barra 1 pasó a
  placa cuchilla y volvió; **Crear** creó 9 elementos (y `delete` los borró restaurando 3 barras).
- **El plano al lado** desde archivo y desde el portapapeles, con divisor y la ruta guardada en `source.drawing_image`.
- **Miniaturas** en el catálogo, en Conectar y en el detalle del nudo del plan; ninguna gris.
- **Retoques**: "(63 añadidas por la selección asistida)" desde la cinta, `changed` / `highlighted` en el log, títulos sin
  repetir.
- Por el puente: `catalog_apply` sin `template_id` con `proposal`, `validate` con `drawing_image`, **`--puente` 34/34**, log
  sin ningún evento de fallo, modelo a cero al final.

## 2. Lo que no validó y por qué (`fase-11.md` 7.2)

El **arranque sobre el cordón superior** dio `MEMBER_NOT_AT_NODE` en la segunda diagonal. No es un fallo del add-in: esos
nudos son **nudos en K con excentricidad** (las dos diagonales cortan el eje del cordón HSS12X8 a 34 mm una de otra, medido
ya en la ronda 8b) y el contrato describe un nudo con un solo punto de trabajo al que llegan todas las barras (tolerancia de
5 mm). Subir la tolerancia haría validar pero dibujaría el acero desde un punto por el que una diagonal no pasa. Hay que
decidir cómo se describen: `docs/propuestas/nudos-en-k-excentricos.md` (tres opciones; recomiendo la A: punto de trabajo a
medio camino, excentricidad declarada en el contrato y tolerada hasta medio canto con aviso, cada barra recortada sobre su
eje real).

## 3. Qué cambió en el repositorio (sin tocar `src/`)

`fase-11.md` (sección 7), nota de hecha en `docs/instalacion/fase-11.md`, README, `docs/propuestas/flujo-intuitivo.md`, la
propuesta nueva y este resumen. Probado en la nube: compilación sin avisos y 238/238 pruebas.

## 4. Qué sigue

1. **Responder las cuatro preguntas de `docs/propuestas/nudos-en-k-excentricos.md`** (opción, tolerancia, cartela por fuera o
   atravesando, si hay detalle del plano).
2. Con las respuestas, escribir el prompt de la **Fase 12**: cartela automática **y** nudo en K. Prompt para esa sesión:

   ```
   Lee CLAUDE.md, docs/propuestas/nudos-en-k-excentricos.md (con mis respuestas: [P1..P4]), docs/propuestas/flujo-intuitivo.md
   (fila 12 del calendario y V6), docs/fases/fase-11.md (secciones 7.2, 7.5 y 7.6) y docs/prompts/fase-11.md como modelo.
   Escribe docs/prompts/fase-12.md: el prompt y el alcance de la Fase 12 "Cartela automática y nudos en K" (outline.mode =
   "auto", la excentricidad en el contrato y en la validación, el arranque y el plan sobre el cordón superior, y de paso el
   camino de una sola barra desde Conectar), con qué entra y qué no, qué se prueba en la nube y qué en el PC, y las dudas
   que debo aclarar contigo antes de programar. Sin código. Termina con commit, push y un resumen corto.
   ```
3. Nada que instalar hasta la Fase 12.
