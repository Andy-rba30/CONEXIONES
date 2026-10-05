# Resumen del cierre de la ronda 8c (copia del mensaje final del chat)

Fecha: 2026-10-05. Rama `main`. Add-in **0.8.4**, **NO PROBADO en Revit** (en el PC está la 0.8.3). Informe:
`docs/fases/fase-8.md`, sección 10. Para probar en el PC: `docs/instalacion/fase-8d.md` (unos 35 minutos).

**Los resultados de la 8c, contrastados (10.1).** Todo lo esperado en 9.3 salió bien, incluido lo que la 0.8.2 dejó sin
probar (sondeo 17 paso 9, `PLAN_MARKS_REPLACED`, `removed_markers`, `overrides` devuelto, `--puente` 28/28). Con los 64
elementos (los 56 de la 8b más los ocho tramos del cordón superior) salieron **los mismos 16 listos** del cordón central.
Tres fallos vistos por la persona y uno del sondeo 19, los cuatro corregidos aquí.

**Los 10 nudos "sin plantilla" (10.2).** Están todos en el **cordón superior** (HSS12X8): ocho nudos en K con dos diagonales
que llegan desde abajo y dos extremos con una. El *Nudo tipico Detalle D* es la típica del cordón central (HSS3X3, tres
barras: dos por arriba y una por abajo) y el casado exige llenar sus tres ranuras, así que no encaja en ninguna
orientación. Hace falta una plantilla propia (crear uno a mano y guardarlo) o excluirlos. Los 7 "falta el cordón" del
mismo cordón son sus **empalmes** (dos tramos acaban justo ahí). Y el **cordón inferior no entró en la selección** (abajo
solo hay parejas diagonal + montante). Nada de esto es un fallo de la detección.

**Las cuatro correcciones (10.3).**

1. **La ventana del plan es no modal** (`ExternalEvent`, `Revit/Batch/PlanEvents.cs`): se queda abierta mientras orbitas y
   pinchas en Revit. Todo lo que toca el modelo (replanificar, marcas, descartar, Ver en Revit, pinchar cordón / barras /
   nudo nuevo, Editar nudo, Abrir catálogo) pasa por el evento; mientras Revit trabaja los botones se apagan con "⏳ …" y
   se encienden solos. La ventana no lee el modelo: recibe un `PlanSnapshot` (plan + mapa + tipos) de cada acción. Una
   sola ventana por sesión: Planificar lote con otra selección la actualiza; sin selección la trae delante. La
   previsualización y el catálogo siguen modales (dentro del evento).
2. **Ver en Revit sin cuadro**: `PlanZoom` encuadra una caja de ±1,2 m alrededor del nudo con `ZoomAndCenterRectangle`,
   selecciona barras y marcador y no cierra nada. Sin `ShowElements` y sin el diálogo del add-in.
3. **Descartar desde la ventana quita también los cubos**: `BatchPlanner.DiscardAndClean` quita las marcas del plan, las de
   cualquier otro plan marcado del documento y los marcadores huérfanos (igual que `batch_plan_discard` con `all: true`),
   y anota en el log cuántos quedan (0). Además, si Revit deshace una operación, el estado de las marcas en memoria se
   restaura (`PlanMarkState`), para que no vuelvan a quedar cubos que nadie reclama.
4. **Sondeo 19 v2**: imagen BMP, `Position` en vez de `Location`, y el manejador de clics se **busca por reflexión** (tipos
   con `TemporaryGraphics` / `InCanvasControl` en `RevitAPI.dll` y `RevitAPIUI.dll`, servicios externos con `Temporary`); si
   aparece, registra un servidor de prueba que avisa al pinchar. Deja la etiqueta puesta para que la persona la pinche y
   `19b-etiquetas-quitar.py` limpia.

**Probado en la nube.** `dotnet build` 0 avisos; `dotnet test` **177/177**; simulador **47/47**; `probar_conexiones.py`
**26/26** (0.8.4); `py_compile` de todo. El Core no cambia (sin pruebas nuevas): todo lo nuevo es del add-in y se compila
contra la API 2027 (nombres resueltos por el compilador, ninguno inventado).

**NO PROBADO (necesita Revit).** Toda la ventana no modal (el evento, los botones, pinchar con la ventana abierta, la
previsualización modal dentro del evento), el zoom en la 3D, el descartar con marcas de otro plan en otra vista, y el
sondeo 19 v2. Todo está en `docs/instalacion/fase-8d.md`, que reproduce a propósito el caso de los cubos (plan del puente
marcado en otra vista + Descartar desde la ventana).

**Qué haces tú ahora.** Cerrar Revit y pasar `docs\instalacion\fase-8d.md` al instalador; en el paso 8d-3 orbitas y
pinchas con la ventana abierta, pruebas Ver en Revit y anotas lo que no vaya como se describe; en 8d-6 pinchas la etiqueta.
Con sus resultados, una sesión corta de cierre y después la **Fase 9** (crear el lote), decidiendo antes P6 (la plantilla
del Detalle D sobre cordón HSS4X4) y qué hacer con los nudos del cordón superior (plantilla propia o excluirlos).

Archivos: `src/MotorConexiones.Revit/Batch/PlanEvents.cs`, `PlanSnapshot.cs`, `PlanZoom.cs`, `PlanPicker.cs` (nuevos),
`BatchPlanner.cs`, `PlanMarks.cs`, `BatchPlanCommand.cs`, `UI/BatchPlanWindow.xaml(.cs)`; `AddinInfo.cs` y los csproj
(0.8.4); `mcp/tools/conn_tools.py`, `mcp/revit_mcp/conexiones.py`, `mcp/pruebas/simulador_revit.py`, `mcp/CONTRATO-conn.md`
(versión); `scripts/sondeos/19-etiquetas-lienzo.py` (reescrito) y `19b-etiquetas-quitar.py` (nuevo); `docs/fases/fase-8.md`
(sección 10), `README.md`, `docs/guide.md`, `CLAUDE.md`, `docs/propuestas/flujo-intuitivo.md`, `docs/instalacion/fase-8d.md`
y este resumen.
