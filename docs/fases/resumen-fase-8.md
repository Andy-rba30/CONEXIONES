# Resumen de la Fase 8 (copia del mensaje final del chat)

Fecha: 2026-10-04. Rama `main`. Add-in **0.8.0**.

**Hecho.** La Fase 8 está escrita, compilada y probada en la nube; falta probarla en Revit con el instalador.

- **Detección de nudos y plan de lote en el Core** (`src/MotorConexiones.Core/Batch/`): agrupa los extremos de las barras
  (10 mm), reconoce el cordón que atraviesa (5 mm), calcula el marco canónico y los ángulos con signo, nombra los nudos
  `N1…` a lo largo de la cercha, casa cada uno con las plantillas (también en espejo), instancia la especificación con
  `source.batch_id` y la valida con el token de siempre. Estados por nudo: `ready`, `invalid`, `no_match`,
  `ambiguous_chord`, `offset`, `untyped`, `already_connected`, `excluded`. Correcciones acumuladas con el mismo
  `plan_id`: excluir, cordón, plantilla, quitar o añadir barras, añadir nudo, fundir, separar y editar la especificación
  de un nudo.
- **Add-in**: marcas en la vista (colores por nudo, cordón grueso, marcador con el nombre: cubo = misma orientación,
  rombo = espejo), planes en memoria, operaciones `batch_plan`, `batch_plan_get` y `batch_plan_discard`, y el botón
  **Planificar lote** con su ventana (Ver en Revit, Excluir, Cordón y Barras de lista o pinchando en Revit, Añadir nudo,
  Plantilla, Editar nudo, Guardar plan JSON, Descartar). **No crea ninguna conexión.**
- **MCP**: 23 rutas y 21 herramientas (`conn_batch_plan`, `conn_batch_plan_get`, `conn_batch_plan_discard`); simulador
  y `probar_conexiones.py` ampliados; `docs/guide.md` sección 6; `mcp/CONTRATO-conn.md`; README sección 12.

**Probado en la nube.** `dotnet build` sin avisos (el add-in compila contra la API 2027, así que las API de marcas
existen); `dotnet test` **150/150** (19 pruebas nuevas con una cercha sintética de 30 nudos: cordón al revés, 6 mm /
40 mm, nudo ambiguo, desfase, barra suelta, merge/split/add_node, spec por nudo, ya conectado, JSON ida y vuelta);
simulador **45/45**; `probar_conexiones.py` **26/26** contra el simulador.

**NO PROBADO (necesita Revit).** El efecto visual de las marcas y el marcador, pinchar en Revit desde la ventana, y la
detección sobre la cercha real del Hangar (si las barras llegan al eje o tienen retiros, y cuántos nudos en espejo
validan con la cartela fija). Todo está en `docs/instalacion/fase-8.md` (unos 45 minutos, con el sondeo 17 de marcas).

**Qué haces tú ahora** (`docs/fases/fase-8.md`, sección 6): cerrar Revit, pasar `docs\instalacion\fase-8.md` al
instalador, seleccionar la cercha entera cuando lo pida, mirar lo de la sección 3 del informe, y anotar al final de
`resultados-fase-8.md` qué nudos salieron con qué estado (preguntas P1 y P2 de la sección 5). Después, la sesión de
cierre 8b con el prompt de la sección 6 y, solo entonces, la Fase 9 (crear el lote).

Archivos: `docs/prompts/fase-8.md`, `docs/fases/fase-8.md`, `docs/instalacion/fase-8.md`, `scripts/sondeos/17-marcas-plan.py`.
