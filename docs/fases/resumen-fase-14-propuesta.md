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

1. **Leer la propuesta y responder las dudas D1 a D15** en el chat de la sesión siguiente; basta con los números ("D1 a, D2 no lo
   sé, D3 aviso, …"). Las tres que más cambian el trabajo son **D1** (qué entra en la Fase 14), **D2** (si los montantes están
   modelados y a qué cordón llegan) y **D14** (correr el sondeo 31 antes de escribir el prompt).
2. **Fusionar en `main`** las dos ramas de solo documentación que están sueltas: `claude/sweet-carson-pqqo3s` (cierre de la 13c) y
   `claude/fase-14-opciones-o12qve` (esta). Puede hacerlo el instalador al empezar la siguiente instalación, como en el paso 13c-0.
3. **Si D14 es "antes" (lo recomendado)**: una sesión corta escribe los sondeos 31 y 32 y `docs/instalacion/fase-14-sondeos.md`; el
   instalador los corre (unos diez minutos, solo lectura) y devuelve la tabla en `docs/fases/resultados-fase-14-sondeos.md`.
4. **La sesión que escribe `docs/prompts/fase-14.md`** (prompt y alcance), con las respuestas y lo que digan los sondeos, con la
   estructura de `docs/prompts/fase-13.md`.
5. **La sesión que programa la Fase 14** (add-in 0.14.0), su instalación, sus rondas y su cierre, como en las fases 12 y 13.

**Hoy no hay nada para el instalador.**

## 3. Prompt para la sesión siguiente (paso 3), rellenando las respuestas

```
Lee CLAUDE.md, docs/propuestas/fase-14-opciones.md (secciones 4 y 6) y docs/fases/fase-13.md (sección 9). Mis respuestas a las
dudas: D1 a, D2 …, D3 …, …, D14 antes, D15 sí. Escribe los sondeos de solo lectura scripts/sondeos/31-inventario-cercha.py y
scripts/sondeos/32-columnas-y-anclajes.py (IronPython 2.7 dentro de Revit 2027, con -SinTransaccion, ids con System.Int64 como el
sondeo 30 v3, sin dar por hecho ningún nombre de la API de Advance Steel: el 32 lista los tipos con "Anchor" y "Grout" en el
nombre y sus constructores) y docs/instalacion/fase-14-sondeos.md con los comandos literales para el instalador y dónde guardar
la salida (docs/fases/resultados-fase-14-sondeos.md). Sin código del add-in. Termina con commit, push y un resumen corto.
```

Si en D14 se prefiere "dentro de la instalación", el paso 3 se salta y el prompt de la sesión nueva es directamente el del paso 4:

```
Lee CLAUDE.md, docs/propuestas/fase-14-opciones.md y docs/prompts/fase-13.md. Mis respuestas a las dudas: D1 a, D2 …, …, D15 sí.
Escribe docs/prompts/fase-14.md (prompt y alcance de la Fase 14) con la estructura de docs/prompts/fase-13.md: decisiones ya
tomadas, decisiones de la persona, alcance, reglas técnicas, pruebas en la nube, instrucciones para el instalador, definición de
hecho, fuera de alcance, dudas y orden de recorte. Sin código. Termina con commit, push y un resumen corto.
```
