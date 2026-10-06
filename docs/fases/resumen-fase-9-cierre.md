# Resumen del cierre de la Fase 9 (2026-10-06): crear por lotes, contrastado en Revit

Rama `main`. Add-in **0.9.0** (sin cambios en este cierre: no hace falta ronda 9b). Informe: `docs/fases/fase-9.md`,
sección 7. Resultados del instalador: `docs/fases/resultados-fase-9.md` y capturas `fase9-01`, `02`, `04` y `05`. Copia
corta del mensaje del chat.

## 1. Qué salió como se esperaba

- **Sondeo 20**: crear y borrar el Detalle D dentro de un grupo exterior, `Assimilate() -> Committed`, `RollBack() ->
  RolledBack`, conexiones 0, acero suelto 0, extensiones como antes: **"GRUPOS ANIDADOS OK"**. `batch_single_undo` se queda
  en `true`; el riesgo principal de la fase queda cerrado.
- **Crear 16 conexiones** desde la ventana, dos veces (antes y después del Ctrl+Z): 16 creadas, 0 fallidas, 9 elementos
  por nudo, en **2,6 s y 2,4 s** (0,15–0,25 s por nudo, muy por debajo de lo previsto). Informe en la ventana ("Creadas 16
  conexiones (14 con aviso)", filas `✔`, barra de estado con "Una sola entrada de deshacer (Ctrl+Z). 2.6 s."), marcas de los
  creados fuera, `list` con `batches {…: 16}`, `batch_plan_get` con `created_count 16` y `last_report`.
- **Un solo Ctrl+Z** quitó las 16 (sondeos 12 y 13 a cero; en Deshacer, una sola entrada `batch_create`).
- **El lote por el puente**: crear 2 (`created_with_warnings`, `undo_entries one`, 0,5 s), saltadas la segunda vez, token
  alterado → `failed` con `VALIDATION_TOKEN_INVALID` sin tocar el modelo, sin token → `INVALID_REQUEST`, `batch_delete` 2
  (18 elementos, 6 barras restauradas, una entrada de deshacer) y `BATCH_EMPTY` después; sondeos 12 y 13 a cero.
- **Qué hacer con botones** y el **empalme**: N9 como `✖ Empalme del cordón` con "El cordón termina en este nudo
  (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye" y Excluir; Incluir y Plantilla…
  pulsados desde la tabla.
- **Sondeo 19 v4**: las dos etiquetas (la 4 en N4 y la B) vistas, pinchadas **sin ningún cuadro** y en naranja; 19b al
  momento. V3 (etiquetas pinchables) lista para la Fase 10.
- **Lo pendiente de la Fase 8** (8.5 y 12.5): `end_gap_mm`, `overrides` devuelto tal cual, `discard all`, sondeo 17, el log,
  Plantilla… cancelado, Ver en Revit sin cuadro, Planificar lote con la ventana abierta y Descartar con un plan en otra
  vista: cerrados (`fase-9.md` 7.5).

## 2. Qué no coincidió y qué se hizo

- **`probar_conexiones.py --puente` murió en la prueba 22** (`UnicodeEncodeError` por el "●" de `● Listo` en la consola
  cp1252 de Windows): 21/21 hasta ahí y las 22 a 30 sin ejecutar. **Corregido** (`sys.stdout` con `errors="replace"`),
  reproducido y comprobado en la nube con `PYTHONIOENCODING=cp1252` (28/28). El 30/30 en el PC se repite en la Fase 10.
- **Borrar el lote desde la ventana no se pulsó**: el segundo lote se cerró con Descartar y lo borró el sondeo 12 (las 16
  entradas `delete` de la captura `fase9-04` son suyas). La operación sí se probó por el puente. Queda para la Fase 10.
- **Los 16 `delete` seguidos** del sondeo 12 avisaron desde el segundo ("cannot start a fabrication transaction while
  asynchronous fabrication tasks are queued…"); el borrado funcionó igual. El texto del aviso ("se crea con DirectShape")
  es el de crear y confunde: se corrige con la versión siguiente del add-in.
- `fase9-03-informe.png` era una copia de `fase9-01` (que sí enseña el informe): se quita. Las capturas de la 8e que borró
  el sondeo 19 ya estaban restauradas (`959b4c9`).
- Sin anotar (se miran en la Fase 10): `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta, el cordón inferior, si
  Ctrl+Z devuelve las marcas y el texto de los 3 `REVIT_WARNING` por nudo del lote (van al log, no al informe, a propósito).

## 3. Qué se probó en la nube

- `probar_conexiones.py` contra el simulador con la consola en cp1252: antes, el mismo `UnicodeEncodeError` de la prueba
  22; después, **28/28** (y 28/28 en UTF-8). `py_compile` correcto, simulador 62/62.
- `dotnet build` sin avisos y `dotnet test` **192/192** (nada cambió en `src/`; se repite para dejarlo anotado).

## 4. Qué hacer ahora

1. **No hay nada que instalar**: el add-in del PC (0.9.0) es el del repositorio. Lo pendiente de probar (sección 2) va en la
   instalación de la Fase 10.
2. **Fase 10** en una sesión nueva, con este prompt (`fase-9.md` 7.8):

   ```
   Lee CLAUDE.md, docs/propuestas/flujo-intuitivo.md y docs/fases/fase-9.md (secciones 7.6 y 7.8). Escribe
   docs/prompts/fase-10.md (C6 selección asistida, V3 etiquetas pinchables sobre el sondeo 19 v4, y V2 si cabe sin recortar
   lo anterior) y ejecuta solo la Fase 10 con el add-in 0.10.0. Incluye en docs/instalacion/fase-10.md lo pendiente de
   fase-9.md 7.6 (Borrar el lote desde la ventana, --puente 30/30, PLAN_MARKS_REPLACED, Editar nudo con la ventana abierta)
   y corrige el texto del aviso del borrado (fase-9.md 7.2, punto 3). Termina con compilación sin avisos, pruebas en verde,
   docs/fases/fase-10.md, README, commit, push y un resumen corto.
   ```
