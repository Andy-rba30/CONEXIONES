# Resumen de la ronda 8c (copia del mensaje final del chat)

Fecha: 2026-10-05. Rama `main`. Add-in **0.8.3**. Informe: `docs/fases/fase-8.md`, sección 9. Prompt: `docs/prompts/fase-8c.md`.

**Qué se hizo.** La ventana del plan de lote ahora se entiende sin leer el README, y la IA y el modelo dicen lo mismo:

- **Cabecera con la decisión**: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de
  perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas." (con 0 listos, "Ningún nudo listo: …" y la causa).
- **Mapa de la cercha** en la ventana: el alzado con las barras en gris y un círculo por nudo con su número y el color de
  su estado; clic = elegir la fila, doble clic = Ver en Revit, globo al pasar el ratón, rueda, arrastrar, Ajustar.
- **Tabla solo con los nudos de verdad**: las barras sueltas y las parejas sin cordón van ocultas con contador y **Mostrar
  ocultos**; estados en español con icono y color (● Listo, ▲ Listo con aviso, ✖ Falta el cordón…), columna **Espejo**
  (no / sí) y columna **Qué hacer** con una frase por caso. Token, ids y `end_gap_mm` solo en el detalle del nudo.
- **Menos botones**: Replanificar, Editar nudo, Ver en Revit y Cerrar; el resto en el menú de clic derecho del nudo y en
  **Más…** (Añadir nudo, Guardar plan JSON, Descartar plan). Leyenda al pie. Nada se pierde.
- **Marcas del modelo por estado**: verde = se creará, ámbar = con aviso, rojo = falta algo, gris = no se crea (excluidos y
  ya conectados van en gris); los ocultos no se marcan: 16 marcadores en la cercha de la 8b en vez de 36.
- **Para la IA**: `summary_text`, `visible_count`, `hidden_text` en el plan y `status_text`, `advice`, `visible_by_default`
  por nudo (solo se añaden claves; `color_name` pasa a ser el del estado). La guía (sección 6) pide resumir en español
  y no volcar el JSON.
- **Catálogo vacío**: aviso `CATALOG_EMPTY`, cabecera "Ningún nudo listo: no hay plantillas…" y botón **Abrir catálogo**.
- **Sondeo 19** (`scripts/sondeos/19-etiquetas-lienzo.py`): comprueba en Revit las API de etiquetas en el lienzo (V3) sin
  inventar nombres; su salida decide la Fase 10.
- Todos los textos salen del Core (`Core/Batch/PlanAdvice.cs`); el mapa, de `Core/Batch/TrussMap.cs`. La detección, el
  casado, `PlanBuilder` y el contrato no cambian; `HangarTruss8b` sigue dando 59 nudos y 16 `ready`.

**Probado en la nube.** `dotnet build` sin avisos; `dotnet test` **177/177** (+15: `PlanAdviceTests` con la cabecera
exacta de la cercha de la 8b y los colores por estado, `TrussMapTests` con el alzado de esa cercha: 56 segmentos, 59
puntos, 16 visibles, N4 en X = −11870, N7 en −6740,5, mínimos cuadrados con barras no coplanares); simulador **47/47**
(dos comprobaciones nuevas, una con el catálogo vacío); `probar_conexiones.py` **26/26** con las claves nuevas exigidas;
`py_compile` de todo.

**NO PROBADO (necesita Revit).** Todo lo visual (el mapa en pantalla, los colores en la vista, el menú de clic derecho,
Más…, Abrir catálogo), la lectura de las barras para el mapa, el sondeo 19 y lo que la 0.8.2 dejó sin probar
(`end_gap_mm`, `PLAN_MARKS_REPLACED`, `removed_markers`, `overrides` sin `IsEmpty`, sondeo 17 paso 9, `--puente` 28/28).
Todo en `docs/instalacion/fase-8c.md` (unos 50 minutos, la cercha de la 8b **con los cordones superior e inferior**; esta
vez el puente y el log se anotan en el archivo).

**Qué haces tú ahora.** Cerrar Revit, pasar `docs\instalacion\fase-8c.md` al instalador, seleccionar la cercha con todos
sus cordones y mirar la ventana nueva (capturas `fase8c-01` a `03`). Después, la **Fase 9** (crear el lote) en una sesión
nueva con el prompt del paso 6 de la sección 6 de `fase-8.md`, decidiendo antes la pregunta P6 (la plantilla del Detalle D
sobre cordón HSS4X4).

Archivos: `src/MotorConexiones.Core/Batch/PlanAdvice.cs` y `TrussMap.cs` (nuevos), `BatchPlan.cs`, `ErrorCodes.cs`,
`AddinInfo.cs`; `src/MotorConexiones.Revit/Batch/BatchPlanner.cs`, `PlanMarks.cs`, `BatchPlanCommand.cs`,
`Operations/BatchPlanOperation.cs`, `UI/TrussMapCanvas.cs` (nuevo), `UI/BatchPlanWindow.xaml(.cs)`;
`src/MotorConexiones.Tests/PlanAdviceTests.cs` y `TrussMapTests.cs` (nuevos), `BatchPlanTests.cs`; `mcp/tools/conn_tools.py`,
`mcp/revit_mcp/conexiones.py`, `mcp/pruebas/simulador_revit.py`, `mcp/pruebas/probar_conexiones.py`, `mcp/CONTRATO-conn.md`;
`scripts/sondeos/19-etiquetas-lienzo.py` (nuevo); `docs/guide.md`, `README.md`, `CLAUDE.md`, `docs/propuestas/flujo-intuitivo.md`,
`docs/fases/fase-8.md` (sección 9), `docs/instalacion/fase-8c.md`, `docs/fases/resumen-fase-8c.md`.
