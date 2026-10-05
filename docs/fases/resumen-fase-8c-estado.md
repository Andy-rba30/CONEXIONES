# Ronda 8c: cómo va (copia de la respuesta del chat)

Fecha: 2026-10-05. Rama `main`. Commit `ef95e09` subido a GitHub. Add-in **0.8.3**.

## Estado

Ya terminé: la ronda 8c está completa, con commit `ef95e09` subido a `main` en GitHub. No queda nada en curso.

## Lo que hay en el repositorio ahora

- Add-in **0.8.3** con la ventana del plan nueva:
  - cabecera con la decisión ("Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de
    perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas."),
  - mapa de la cercha con un círculo por nudo del color de su estado (clic = fila, doble clic = Ver en Revit),
  - solo nudos de verdad en la tabla (barras sueltas y parejas sin cordón ocultas, con contador y **Mostrar ocultos**),
  - estados en español con icono (● Listo, ▲ Listo con aviso, ✖ Falta el cordón…), columnas **Espejo** y **Qué hacer**,
  - 4 botones (Replanificar, Editar nudo, Ver en Revit, Cerrar) más **Más…** y el menú de clic derecho.
- Marcas del modelo **por estado**: verde = se creará, ámbar = con aviso, rojo = falta algo, gris = no se crea; los
  ocultos no se marcan.
- Para la IA: `summary_text`, `status_text`, `advice` y `visible_by_default` en la respuesta; la guía pide resumir en
  español sin volcar el JSON.
- Aviso `CATALOG_EMPTY` y botón **Abrir catálogo** cuando no hay plantillas.
- Sondeo 19 (`scripts/sondeos/19-etiquetas-lienzo.py`) para decidir las etiquetas pinchables en la Fase 10.

## Probado en la nube

| Comprobación | Resultado |
|---|---|
| `dotnet build MotorConexiones.sln -c Release` | 0 avisos, 0 errores |
| `dotnet test` | 177/177 en verde (+15 pruebas nuevas) |
| `simulador_revit.py --autocomprobar` | 47/47 |
| `probar_conexiones.py` contra el simulador | 26/26 |
| `py_compile` de `mcp/` y `scripts/sondeos/` | correcto |

## NO PROBADO (necesita Revit)

Lo visual (el mapa en pantalla, los colores en la vista, los menús, Abrir catálogo), el sondeo 19 y lo que la 0.8.2 dejó
sin probar (`end_gap_mm`, `PLAN_MARKS_REPLACED`, `removed_markers`, `overrides` sin `IsEmpty`, sondeo 17 paso 9,
`--puente` 28/28). Todo está en `docs\instalacion\fase-8c.md`.

## Qué te toca a ti

1. Cerrar Revit y pasar `docs\instalacion\fase-8c.md` al instalador (unos 50 minutos, la cercha de la 8b **con los
   cordones superior e inferior**).
2. Mirar la ventana nueva y las tres capturas `fase8c-01-colores-estado.png`, `fase8c-02-mapa.png` y
   `fase8c-03-etiqueta.png`.
3. Después, lanzar la **Fase 9** en una sesión nueva con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`,
   decidiendo antes la pregunta P6 (la plantilla del Detalle D sobre cordón HSS4X4).

## Dónde está todo

- Informe: `docs/fases/fase-8.md`, sección 9.
- Resumen completo: `docs/fases/resumen-fase-8c.md`.
- Instrucciones del instalador: `docs/instalacion/fase-8c.md`.
- Prompt ejecutado: `docs/prompts/fase-8c.md`.
