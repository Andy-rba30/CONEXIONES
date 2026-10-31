# Resumen del cierre de la ronda 13b (2026-10-31): la 0.13.1 contrastada en Revit y el add-in 0.13.2

Rama de trabajo `claude/sweet-carson-pqqo3s` (se lleva a `main` en el paso 13c-0 de la instalación). Add-in **0.13.2** (pendiente de
desplegar en el PC: la última instalada es la 0.13.1). Informe: `docs/fases/fase-13.md`, sección 8. Instalación de la ronda:
`docs/instalacion/fase-13c.md`. Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-13b.md`, add-in 0.13.1, 2026-10-31)

- **La 0.13.1 funcionó**: build, 314 pruebas, deploy; los sondeos 12 y 13 corrieron esta vez; `split` / `unsplit` de 1249632 con
  `copy_element` y sin parámetros aproximados; el puente 49/49 con el servidor arrancado por el bloque nuevo.
- **El sondeo 30 v2 corrió entero** y midió lo que faltaba: la diagonal 1249631 **se sale 0,005871 mm de un plano vertical** en 3,5 m
  (0,000096°) y el cordón 0,024 mm en 10,5 m. Los dos tramos salen con el mismo desvío que la barra entera: **la advertencia "slightly
  off axis" es de la geometría del modelo**, no del corte (con la `Line` exacta de la 0.13.1 siguió saliendo). El sondeo tenía dos
  erratas (el lado del cordón al revés: corte a 557 en vez de 485; el `node_info` de los tramos con un `long`): v3.
- **Dos cosas que no coincidieron**: (1) el `split` seguía devolviendo el `REVIT_WARNING` (el resumen del instalador decía "vacío", el
  archivo lo trae) y el registro **no devolvía** `original_start_feet` (la 0.13.1 lo guardaba pero no lo proyectaba a la API);
  (2) **la cadena de Conectar partió una sola diagonal** (1249631) y el nudo en K salió con 1 elemento: no se sabe si la persona
  seleccionó dos barras o si 1249632 dejó de entrar en el nudo tras el corte y el deshacer del paso anterior.
- Menores: el `node_info` de los tramos dio 500 y el `unsplit` del paso listó en vez de deshacer porque la variable `$s` de PowerShell
  salió vacía (el instalador deshizo a mano, `complete: true`); el aviso "No se pudo abrir la FabricationTransaction … asynchronous
  fabrication tasks are queued" al borrar el nudo en K justo después del empalme es el conocido de la Fase 10 (se borra igual).

## 2. Qué se decidió y qué se programó (add-in 0.13.2)

- **La advertencia "slightly off axis" del corte se filtra**, solo en la transacción del corte (`SplitService.Split`, por donde pasan
  la operación suelta, el botón, la cadena y el lote): queda en el log como `revit_failure` con `filtered: true`, como la de la 12d.
  Mecanismo general: `OperationScope.StartTransaction(…, informativeDefinitions)`. En cualquier otra transacción seguiría llegando.
- **El deshacer restaura las uniones automáticas tal como estaban**: hasta la 0.13.1 `Unsplit` las permitía en los dos extremos sin
  saber el estado original (en un modelo importado suelen estar desactivadas; permitirlas puede cambiar la geometría en silencio y es
  la causa candidata de que 1249632 no entrara en el nudo). El registro guarda `original_join_allowed` y el sondeo 30 v3 imprime el
  estado antes de partir.
- La API devuelve ya `original_start_feet`, `original_end_feet` y `original_join_allowed` (contrato al día).
- `docs/instalacion/fase-13c.md` (unos 25 minutos): llevar la rama a `main`, desplegar la 0.13.2, sondeo 30 v3, `split` de 1249632 sin
  `REVIT_WARNING` (y `filtered: true` en el log), **`node_info` del nudo en K antes y después de deshacer** (la pregunta abierta), la
  cadena con las **tres** barras (dos cortes), el puente. Los ids se escriben a mano en los comandos (sin `$s`).
- Versión 0.13.2 en add-in, adaptador, herramientas, simulador y contrato; `CLAUDE.md`, README, guía y propuestas al día.

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **314/314** (las pruebas del cierre ampliadas con `original_join_allowed`), `py_compile`
correcto, simulador **101/101** y `probar_conexiones.py` **46/46** contra el simulador (`addin=0.13.2`). **NO PROBADO en Revit**: el
filtro, las uniones restauradas, el sondeo 30 v3 y la respuesta a la pregunta de la cadena.

## 4. Qué hacer ahora

1. Dar `docs/instalacion/fase-13c.md` al agente instalador con este prompt:

   ```
   Lee docs\instalacion\fase-13c.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, del paso 13c-0 al 13c-6,
   en una sola ventana de PowerShell. Reglas: el paso 13c-0 lleva la rama claude/sweet-carson-pqqo3s a main (si el merge da conflicto,
   para y copia la salida); usa la función Anota tal como está escrita en el 13c-1 (no crees scripts nuevos); si abres otra ventana,
   repite las tres primeras líneas del 13c-0 y vuelve a definir Anota, $salida, $ext, $py, $log y PYTHONIOENCODING; Revit lo abre y lo
   cierra la persona; los pasos marcados (la persona) los hace la persona y tú esperas; no toques src\, config\, mcp\ ni docs\fixtures\;
   si un paso falla, copia el error, no modifiques nada y sigue. Muy importante: los ids que el archivo marca como NEW_ELEMENT_ID,
   SPLIT_ID, ID_EMPALME_1, ID_EMPALME_2 e ID_NUDO_K los copias tú de la salida anterior y los escribes a mano en el comando (nada de
   variables $s ni ConvertFrom-Json). Si el sondeo 30 dice FALLO o PARADA, no es un fallo de la instalación: cópialo entero y sigue.
   Al terminar, haz el commit y el push del 13c-6 y devuélveme: (1) build, número de pruebas (se esperan 314) y la línea de deploy
   con 0.13.2; (2) del sondeo 30 v3, las líneas "uniones automaticas permitidas" de la diagonal y del cordón, la línea "corte a … mm"
   y la de node_info de los tramos; (3) del 13c-4: si warnings del split salió vacío, la línea revit_failure del log con filtered
   true, si el registro trae original_start_feet y original_join_allowed, si unsplit dio complete true, y si el node_info del nudo
   en K tras deshacer es igual que el del 13c-2 (las dos diagonales con reaches_node true); si no es igual, las dos salidas enteras;
   (4) del 13c-5: cuántas barras seleccionó la persona, el texto de la casilla ("Partir las 2 diagonales" o "Partir la diagonal"), el
   cuadro literal de Crear y si "Advertencias" llevaba REVIT_WARNING; (5) el resultado de --puente (x/49) y las líneas _failed del
   log; (6) el git status --short antes del commit y al final, el git log -1 --oneline y el texto de cualquier ventana de error.
   ```

2. Después, en Claude Code (sesión nueva en la nube), el cierre de la 13c:

   ```
   Lee CLAUDE.md, docs/fases/fase-13.md (secciones 7 y 8) y docs/fases/resultados-fase-13c.md. Cierra la ronda 13c y con ella la
   Fase 13: contrasta con 8.1 a 8.3 (¿el split salió sin REVIT_WARNING y con filtered true en el log? ¿el node_info del nudo en K es
   igual antes y después de deshacer? ¿la cadena con tres barras partió las dos diagonales?), corrige lo que haga falta (otra ronda
   solo si 1249632 sigue sin entrar en el nudo), actualiza el informe, el README y las propuestas. No empieces otra fase. Termina con
   commit, push y un resumen corto guardado también en docs/fases/resumen-fase-13c-cierre.md.
   ```
