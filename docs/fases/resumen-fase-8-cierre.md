# Resumen del cierre de la Fase 8 (copia del mensaje final del chat)

Fecha: 2026-10-05. Rama `main`. Add-in **0.8.2** en el repositorio (0.8.1 es la última probada en el PC). Informe:
`docs/fases/fase-8.md`, sección 8.

**Qué dijo el PC en la ronda 8b** (`resultados-fase-8b.md`). La persona seleccionó la cercha gemela del Hangar (56 barras,
cordón central entero en ocho tramos, siete HSS4X4 y uno HSS3X3, sin cordones superior ni inferior) y `conn_batch_plan`
con la plantilla oficial dio **59 nudos: 16 `ready`** (8 `same`, 8 `mirror_x`), 20 `no_match` (las parejas en K sin cordón,
con `NODE_CHORD_NOT_CONTINUOUS`) y 23 `untyped`, en 953 ms. El sondeo 18 midió en el modelo lo que la 8b suponía (las
diagonales del Detalle D a 58,8 / 14,3 / 33,1 mm del eje del cordón, con los ejes cortándolo a menos de 4 mm entre sí).
El replan con `exclude` e `include` conservó el `plan_id` y el token, los marcadores quedaron sobre el eje del cordón y
**Editar nudo** abrió la previsualización del nudo N4 validada. La Fase 8 queda **cerrada**.

**Lo que no coincidió y se corrigió (0.8.2, sin ronda 8c).**

- `members[].end_gap_mm` no venía en la respuesta del add-in (`NodeToData` no lo copiaba del Core).
- El botón **Planificar lote** marcó un plan nuevo mientras el del puente seguía marcado; al descartarlo quedaron 36
  marcadores sin color. Ahora **en un documento solo hay un plan marcado**: marcar otro quita las marcas del anterior con el
  aviso `PLAN_MARKS_REPLACED` (ese plan sigue en memoria).
- `discard all` decía `removed_markers: 0` tras quitar esos 36 (contaba solo huérfanos): ahora cuenta los que había.
- `overrides` traía una clave `IsEmpty` que, devuelta en una petición, daba `INVALID_REQUEST`: `[JsonIgnore]` y prueba.
- El sondeo 17 moría en el paso 9 al leer el Id del marcador ya borrado.
- Las pruebas 22 y 23 de `probar_conexiones.py --puente` daban por bueno el plan solo si el nudo listo se llamaba "N1";
  en Revit, con las cuatro barras del fixture, el nudo del Detalle D es **N4** (tres extremos sueltos quedan a su
  izquierda), así que fallaban aunque la detección estuviera bien. Ahora buscan el nudo `ready` que contiene el cordón.
  (El archivo `resumen-fase-8b-que-sigue.md` dice que en la 8b fallaron 22, 23 y 27, pero esa salida no está en el
  repositorio; la 27 es `tools/list` y depende de que el servidor haya arrancado.)

**Probado en la nube.** `dotnet build` sin avisos; `dotnet test` **162/162** (+4: la cercha de la 8b con sus 56 ejes reales
como fixture `HangarTruss8b` reproduce el plan del PC nudo a nudo: 59 nudos, 16 / 20 / 23, N4 y N7 con sus ángulos, los
14 avisos `TEMPLATE_PROFILE_DIFFERS` de los tramos HSS4X4; el plan de solo las cuatro barras del Detalle D, N4 `ready`; y el `overrides` de ida y vuelta); simulador **45/45**;
`probar_conexiones.py` **26/26** contra el simulador.

**NO PROBADO (necesita Revit; va en la instalación de la Fase 9, sección 8.5 del informe).** El add-in 0.8.2 entero:
`end_gap_mm` en la respuesta, `PLAN_MARKS_REPLACED`, el conteo de `removed_markers`, `overrides` sin `IsEmpty`, el paso 9
del sondeo 17, y lo que la 8b no anotó (`--puente` 28/28, el log del día, Quitar edición, Añadir nudo, el globo del botón
en gris).

**Pregunta nueva para la Fase 9 (P6).** La plantilla del Detalle D casó 14 nudos cuyo cordón es HSS4X4 (aviso
`TEMPLATE_PROFILE_DIFFERS`, política `warn`): ¿se crean con la misma cartela o la plantilla debe exigir el perfil?

**Qué haces tú ahora.** Nada en Revit. Cuando quieras seguir, lanza la Fase 9 en una sesión nueva con el prompt del paso 6
de la sección 6 de `docs/fases/fase-8.md`.

Archivos: `docs/fases/fase-8.md` (sección 8), `src/MotorConexiones.Revit/Batch/BatchPlanner.cs`,
`src/MotorConexiones.Core/Batch/BatchOverrides.cs`, `src/MotorConexiones.Core/Validation/ErrorCodes.cs`,
`src/MotorConexiones.Tests/Fakes/HangarTruss8b.cs`, `NodeDetectorTests.cs`, `BatchPlanTests.cs`,
`scripts/sondeos/17-marcas-plan.py`, `mcp/pruebas/probar_conexiones.py` (22 y 23), README (estado, garantías, sección 12, errores), `docs/guide.md`,
`mcp/CONTRATO-conn.md`, `mcp/tools/conn_tools.py`, versiones 0.8.2.
