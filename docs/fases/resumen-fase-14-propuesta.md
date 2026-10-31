# Resumen de la sesión de la propuesta de la Fase 14 (2026-10-31): qué se hizo y qué sigue

Rama de trabajo `claude/fase-14-opciones-o12qve` (solo documentación; se fusiona en `main` cuando se quiera, por ejemplo al empezar
la siguiente instalación, como el paso 13c-0). Add-in sin cambios: sigue la **0.13.2** probada en Revit. Copia corta del mensaje del
chat.

## 1. Qué se hizo

- Se escribió **`docs/propuestas/fase-14-opciones.md`**: los ocho candidatos para la Fase 14 (`to_support`, `base_plate`, partir
  cordones desde el plan, V6, el cordón inferior, el traspaso de `mcp/`, los retoques de robustez y, añadido, el lote entero del
  Hangar con sus cortes), qué aporta cada uno a la cercha, cuánto cuesta en sesiones y rondas, los sondeos 31 y 32, la recomendación
  de orden y **quince dudas numeradas (D1 a D15)** con una recomendación en cada una.
- **Recomendación**: Fase 14 = `end_plate` contra un apoyo (`kind: "to_support"`, el montante o la diagonal contra la cara de otra
  barra) más los retoques de robustez (el reintento de la `FabricationTransaction` al borrar seguido y `conn-call.ps1` con salida
  solo JSON): 1 sesión + 2 rondas, con una ronda corta de solo sondeos antes del prompt. En esa misma instalación, sin código: el
  sondeo 31 (cordón inferior, montantes, apoyos del eje B), el sondeo 32 (columnas, qué hay debajo, la API de anclajes) y el lote
  entero del Hangar sobre la copia. Fase 15 = `base_plate`. Partir cordones, V6 y el traspaso de `mcp/`, para cuando haga falta.
- La rama se creó sobre `claude/sweet-carson-pqqo3s` (es `main` más el cierre de la 13c, la única rama donde existen las secciones
  9.4 y 9.5 de `fase-13.md`). Al fusionar no hay conflicto: solo añade archivos.
- Sin código, sin cambios en el add-in ni en el MCP. No se tocaron `fase-13.md`, el README ni `flujo-intuitivo.md` para enlazar la
  propuesta; si se quiere, es una línea en cada uno.

## 2. Qué sigue, en orden

1. **Leer la propuesta y responder las dudas D1 a D15.** **Hecho el 2026-10-31**: la persona aceptó las quince recomendaciones
   (sección 8 de la propuesta y sección 4 de este resumen). Las tres que más cambian el trabajo son **D1** (qué entra en la Fase 14), **D2** (si los montantes están
   modelados y a qué cordón llegan) y **D14** (correr el sondeo 31 antes de escribir el prompt).
2. **Fusionar en `main`** las dos ramas de solo documentación que están sueltas: `claude/sweet-carson-pqqo3s` (cierre de la 13c) y
   `claude/fase-14-opciones-o12qve` (esta). Puede hacerlo el instalador al empezar la siguiente instalación, como en el paso 13c-0.
3. **Si D14 es "antes" (lo recomendado)**: una sesión corta escribe los sondeos 31 y 32 y `docs/instalacion/fase-14-sondeos.md`; el
   instalador los corre (unos diez minutos, solo lectura) y devuelve la tabla en `docs/fases/resultados-fase-14-sondeos.md`.
4. **La sesión que escribe `docs/prompts/fase-14.md`** (prompt y alcance), con las respuestas y lo que digan los sondeos, con la
   estructura de `docs/prompts/fase-13.md`.
5. **La sesión que programa la Fase 14** (add-in 0.14.0), su instalación, sus rondas y su cierre, como en las fases 12 y 13.

**Hoy no hay nada para el instalador.**

## 3. Prompt para la sesión siguiente (paso 3), ya con las respuestas

```
Lee CLAUDE.md, docs/propuestas/fase-14-opciones.md (secciones 4, 6 y 8) y docs/fases/fase-13.md (sección 9). Acepté todas las
recomendaciones de la propuesta (sección 8). Escribe los sondeos de solo lectura scripts/sondeos/31-inventario-cercha.py y
scripts/sondeos/32-columnas-y-anclajes.py (IronPython 2.7 dentro de Revit 2027, con -SinTransaccion, ids con System.Int64 como el
sondeo 30 v3, sin dar por hecho ningún nombre de la API de Advance Steel: el 32 lista los tipos con "Anchor" y "Grout" en el
nombre y sus constructores) y docs/instalacion/fase-14-sondeos.md con los comandos literales para el instalador y dónde guardar
la salida (docs/fases/resultados-fase-14-sondeos.md). Sin código del add-in. Termina con commit, push y un resumen corto.
```

Cuando el instalador devuelva la tabla de los sondeos, el prompt de la sesión del paso 4 es:

```
Lee CLAUDE.md, docs/propuestas/fase-14-opciones.md (sección 8: las decisiones), docs/fases/resultados-fase-14-sondeos.md y
docs/prompts/fase-13.md. Escribe docs/prompts/fase-14.md (prompt y alcance de la Fase 14) con la estructura de docs/prompts/fase-13.md: decisiones ya
tomadas, decisiones de la persona, alcance, reglas técnicas, pruebas en la nube, instrucciones para el instalador, definición de
hecho, fuera de alcance, dudas y orden de recorte. Sin código. Termina con commit, push y un resumen corto.
```

## 4. Qué recomiendo en cada duda (respuesta a "¿qué recomiendas para las dudas?")

Son las recomendaciones de la sección 6 de `docs/propuestas/fase-14-opciones.md`, resumidas. Dos de ellas dependen de cosas que solo
la persona puede saber mirando el modelo (D2, los montantes; D9, las columnas y el concreto): en las dos recomiendo que el sondeo lo
mida, pero si la respuesta está a la vista, se escribe en lugar de "sondeo".

| Duda | Qué recomiendo |
|---|---|
| **D1** Qué entra en la Fase 14 | **(a)**: `to_support` más robustez; en su instalación, los sondeos 31 y 32, el cordón inferior y el lote entero. |
| **D2** Los montantes | Que lo responda el **sondeo 31**. Si ya sabes si están modelados y a qué cordón llegan, dilo: ahorra la ronda. Si no hay ninguno, los apoyos del eje B son el caso de prueba. |
| **D3** Plancha más ancha que la cara del apoyo | **Aviso** con los mm, no error. Error solo si la plancha no toca la cara. |
| **D4** Orientación de las planchas con una barra inclinada | Planchas **paralelas a la cara del apoyo**; la barra se corta en bisel. |
| **D5** Arranque de una barra sola contra una cara | Montante a 90° (± 5°) → `to_support` con el Detalle B de 185. Diagonal inclinada → `direct_weld` como hoy. El desplegable Tipo para cambiarlo. |
| **D6** Hueco entre planchas y retiro | Medir el hueco real y calcular el retiro que falta, nunca negativo. `gap_mm` 0. |
| **D7** Robustez | **Sí a las dos**: reintento de la `FabricationTransaction` con contador en el log (si no sirve, el aviso baja al log) y `conn-call.ps1` con `-SoloJson`. |
| **D8** Cordón inferior | Que lo busque el sondeo 31. Si existe, **sus nudos entran** en el lote de la 14 sin código. |
| **D9** `base_plate` | **No decidir aún**: el sondeo 32 mide las columnas y qué hay debajo, y la Fase 15 se escribe con eso. Sin concreto, una cota en el JSON. |
| **D10** Partir cordones desde el plan | **Aplazar**. Cuando se haga, re-apuntar solas las conexiones del tramo nuevo. |
| **D11** V6 (edición arrastrando) | **Después** de los tipos nuevos, y solo las esquinas de la cartela al principio. |
| **D12** Traspaso de `mcp/` a `revit-mcp` | **Esperar** a que las `conn_*` dejen de cambiar en cada fase. |
| **D13** Lote entero del Hangar | **Sí**, al final de la instalación de la 14, sobre la copia. |
| **D14** Cuándo corre el sondeo 31 | **Antes** de escribir el prompt: ronda corta de solo sondeos. |
| **D15** Alcance y rondas | **Sí**: 1 sesión + 2 rondas y el orden de recorte de la sección 5 de la propuesta. |

Si se acepta todo tal cual, la línea para pegar dentro del prompt de la sección 3 es:

```
D1 a, D2 sondeo 31, D3 aviso, D4 paralelas a la cara, D5 montante a to_support y diagonal a direct_weld, D6 medir el hueco y gap 0,
D7 sí a las dos, D8 sondeo y que entren, D9 esperar al sondeo 32, D10 aplazar y re-apuntar, D11 después y solo esquinas,
D12 esperar, D13 sí, D14 antes, D15 sí
```

Si se cambia alguna, basta con sustituirla en esa línea.
