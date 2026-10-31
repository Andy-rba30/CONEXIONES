# Resumen del cierre de la ronda 13c (2026-10-31): la 0.13.2 contrastada en Revit. La Fase 13 queda cerrada

Rama de trabajo `claude/sweet-carson-pqqo3s` (solo documentación; se fusiona en `main` cuando se quiera, por ejemplo al empezar la
fase siguiente). Add-in **0.13.2**, desplegado y probado en el PC. Informe: `docs/fases/fase-13.md`, sección 9. Copia corta del
mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-13c.md`, add-in 0.13.2, 2026-10-31)

- **Todo como se esperaba**: build sin avisos, 314 pruebas, 0.13.2 desplegada; `ping`, `list`, `unsplit`, sondeos 12 y 13 a cero.
- **Sondeo 30 v3**: las uniones automáticas de la diagonal y del cordón están **desactivadas en los dos extremos** en el modelo; el
  corte propio del sondeo sale ya a 484,8 mm como el add-in; el `node_info` de los tramos funciona (`kind: splice`).
- **`split` de 1249632**: `warnings` vacío; la advertencia "slightly off axis" en el log con `filtered: true`; el registro con
  `original_start_feet`, `original_end_feet` y `original_join_allowed: [false, false]`; `unsplit` completo; **el `node_info` del nudo
  en K es idéntico antes y después de deshacer**.
- **La cadena con las tres barras**: la casilla "Partir las 2 diagonales…", dos cortes (485 y 483 mm), dos empalmes B, el nudo en K con
  2 elementos y 2 retiros, y **sin `REVIT_WARNING`**. Borrados en orden y los cortes deshechos por el `delete` del nudo en K; el
  aviso de la FabricationTransaction en los borrados seguidos es el conocido de la Fase 10 (se borra igual). Puente 49/49.

## 2. Qué queda explicado y decidido

- **La cadena de la 13b partió una sola diagonal** porque el deshacer de la 0.13.1 permitía las uniones automáticas en una barra que
  las tenía desactivadas, y Revit cambió la barra. La 0.13.2 deja las uniones como estaban y el problema no se repite.
- **La advertencia "slightly off axis"** es de la geometría del modelo y en el corte queda en el log, no como `REVIT_WARNING`.
- **Sin código en este cierre**: la 0.13.2 es la definitiva de la fase. Solo documentación (informe, README, `CLAUDE.md`, propuestas,
  instalación 13c anotada y este resumen).
- Notas para más adelante (no son rondas): un reintento corto al abrir la FabricationTransaction al borrar conexiones seguidas; en las
  instalaciones, los ids se escriben a mano.

## 3. Qué hacer ahora

**La Fase 13 está cerrada del todo.** Nada para el instalador. Lo siguiente es la propuesta de la fase siguiente, sin código, en una
sesión nueva de Claude Code con este prompt:

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md (secciones 7, 13 y 14), docs/prompts/fase-13.md (sección 8), docs/fases/fase-13.md
(secciones 9.4 y 9.5), docs/propuestas/placa-de-extremo.md (sección 5), docs/propuestas/flujo-intuitivo.md (V6, V7 y la tabla de
fases) y docs/propuestas/nudos-en-k-excentricos.md. Escribe docs/propuestas/fase-14-opciones.md: qué candidatos hay para la Fase 14
(base_plate; kind to_support, el montante contra un apoyo; partir cordones o barras sin splice_hint desde el plan; contornos
cóncavos con edición arrastrando, V6; el cordón inferior si está modelado; el traspaso de mcp/ a revit-mcp; y los retoques de
robustez, como el reintento de la FabricationTransaction al borrar conexiones seguidas), qué aporta cada uno a la cercha del Hangar,
cuánto cuesta (sesiones y rondas con el instalador), qué sondeos harían falta, tu recomendación de orden, y las dudas numeradas para
la persona con tu recomendación en cada una. Sin código. Termina con commit, push y un resumen corto.
```

Con las dudas respondidas, la sesión siguiente escribe `docs/prompts/fase-14.md` (prompt y alcance), y después la que programa la fase.
