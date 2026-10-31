# Resumen del cierre de la Fase 13 (2026-10-31): la 0.13.0 contrastada en Revit y el add-in 0.13.1

Rama de trabajo `claude/sweet-carson-pqqo3s` (se lleva a `main` en el paso 13b-0 de la instalación). Add-in **0.13.1** (pendiente de
desplegar en el PC: la última instalada es la 0.13.0). Informe: `docs/fases/fase-13.md`, sección 7. Instalación de la ronda:
`docs/instalacion/fase-13b.md`. Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-13.md`, add-in 0.13.0, 2026-10-31)

- **Todo lo nuevo funcionó a la primera**: el filtro de la 12d (solo `END_CUT_APPROXIMATED`, dos `revit_failure` con `filtered: true`),
  la **cartela automática** en un nudo del cordón central (7 elementos, `outline.mode auto` sin puntos; Contorno y Convertir en polígono),
  la **plantilla `auto`** propuesta en espejo en el nudo vecino, la **cadena de Conectar** en el nudo en K (1249631 partida a 485 mm y
  1249632 a 483, dos empalmes B de 8 elementos y el nudo en K de un clic; `SPLIT_IN_USE` al borrar el nudo en K antes que los empalmes;
  `unsplit` `complete: true`), **`conn_split_member` / `conn_unsplit`** (`mechanism: copy_element`, `approximated_parameters: []`), el
  **plan** de la cercha con 31 cortes y 31 empalmes B previstos (sin crear), y el **puente 49/49** con las 25 herramientas.
- **El sondeo 30 no corrió**: `Multiple targets could match: ElementId(…)` en `barra()` (el mismo tropiezo del sondeo 19 v3: en
  IronPython hay que escribir `DB.ElementId(System.Int64(...))`). Lo que venía a decidir lo decidió el add-in: el corte con
  `copy_element` conserva los parámetros y el deshacer deja la barra entera. **`MemberSplitter` se queda como está.**
- **El tramo nuevo salió con la advertencia de Revit "Beam or Brace is slightly off axis"** (`REVIT_WARNING`, en los cortes por el
  adaptador y en la cadena). El Core ya proyecta el punto de corte sobre el eje de la barra; quedan dos causas: que la diagonal original
  ya se salga un poco de un plano vertical (Revit lo repite sobre el elemento nuevo) o el viaje de unidades pies → mm → pies.
- **Erratas de la instalación**: los sondeos 12 y 13 se llamaban mal (`12-estado-barras.py`, `13-limpiar-restos.py`; son
  `12-fase3-borrar.py` y `13-limpiar-fase1.py`) y no corrieron; `$pid` en el 13-9; "30 diagonales" (son 31); la ruta del log. El
  paso 13-10 no arrancaba el servidor MCP y el puente falló a la primera; el instalador lo arrancó y repitió.

## 2. Qué se decidió y qué se programó (add-in 0.13.1)

- **El corte con `copy_element` es definitivo**; el sondeo 30 (v2 corregida) queda como diagnóstico, no decide.
- **Los dos tramos se construyen sobre la `Line` exacta de Revit**: el punto de corte se proyecta con `Line.Project` en pies y los
  extremos son los del original, sin pasar por mm. El registro del corte guarda `original_start_feet` / `original_end_feet` y
  `Unsplit` restaura la curva exacta (los cortes de la 0.13.0 se deshacen desde los mm). Una barra no recta da `SPLIT_FAILED` claro.
  Con esto se descarta la causa "unidades" de la advertencia; el sondeo 30 v2 mide la otra (el `dY` de la diagonal original).
- **No se filtra la advertencia a ciegas**: si en la 13b sigue saliendo con la 0.13.1, es de la geometría del modelo y se decidirá con
  la persona si se filtra por el Guid de su definición (como la de la 12d) o se deja.
- Erratas de `docs/instalacion/fase-13.md` corregidas y anotadas; `docs/instalacion/fase-13b.md` nueva (unos 25 minutos: llevar la rama a
  `main`, desplegar la 0.13.1, sondeo 30 v2, sondeos 12 y 13 reales, `split` / `unsplit` de 1249632 mirando la advertencia, la cadena
  opcional, el puente con el bloque que arranca el servidor).
- Versión 0.13.1 en add-in, adaptador, herramientas, simulador y contrato; `CLAUDE.md`, README y las propuestas al día.

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **314/314** (3 nuevas en `Fase13CierreTests`: el registro de la 0.13.0 sin pies carga, el de la
0.13.1 va y vuelve con los pies exactos, y `Segments` deja el punto de corte sobre el eje con los números reales de la diagonal
1249632), `py_compile` correcto, simulador **101/101** y `probar_conexiones.py` **46/46** contra el simulador.
**NO PROBADO en Revit**: el camino nuevo de `MemberSplitter` y `Unsplit`, el sondeo 30 v2 y si la advertencia desaparece.

## 4. Qué hacer ahora

1. Dar `docs/instalacion/fase-13b.md` al agente instalador con este prompt:

   ```
   Lee docs\instalacion\fase-13b.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, del paso 13b-0 al 13b-6,
   en una sola ventana de PowerShell. Reglas: el paso 13b-0 lleva la rama claude/sweet-carson-pqqo3s a main (si el merge da conflicto,
   para y copia la salida); usa la función Anota tal como está escrita en el 13b-1 (no crees scripts nuevos); si abres otra ventana,
   repite las tres primeras líneas del 13b-0 y vuelve a definir Anota, $salida, $ext, $py, $log y PYTHONIOENCODING; Revit lo abre y lo
   cierra la persona; los pasos marcados (la persona) los hace la persona y tú esperas; no toques src\, config\, mcp\ ni docs\fixtures\;
   si un paso falla, copia el error, no modifiques nada y sigue. Si el sondeo 30 dice FALLO o PARADA, o un split trae REVIT_WARNING,
   no es un fallo de la instalación: cópialo entero y sigue. Al terminar, haz el commit y el push del 13b-6 y devuélveme: (1) build,
   número de pruebas (se esperan 314) y la línea de deploy con 0.13.1; (2) si los sondeos 12 y 13 corrieron esta vez; (3) la salida
   ENTERA del sondeo 30 v2, sobre todo las líneas "diagonal 1249631 ANTES: dX | dY | dZ" y "el punto del marco estaba a N mm";
   (4) en el 13b-4, si warnings del split salió vacío o sigue el REVIT_WARNING "slightly off axis", la línea revit_failure del log
   con su definition, si el registro trae original_start_feet, y si unsplit dio complete true; (5) el cuadro literal de la cadena del
   13b-5 si se hizo; (6) el resultado de --puente (x/49) y las líneas _failed del log; (7) el git status --short antes del commit y al
   final, el git log -1 --oneline y el texto de cualquier ventana de error.
   ```

2. Después, en Claude Code (sesión nueva en la nube), el cierre de la 13b:

   ```
   Lee CLAUDE.md, docs/fases/fase-13.md (sección 7) y docs/fases/resultados-fase-13b.md. Cierra la ronda 13b y con ella la Fase 13:
   contrasta con 7.2.1 (¿desapareció el REVIT_WARNING "slightly off axis"? ¿qué dY tiene la diagonal original?), decide con la persona
   si la advertencia se filtra por su definición como la de la 12d o se deja, corrige lo que haga falta (otra ronda solo si es
   imprescindible), actualiza el informe, el README y las propuestas. No empieces otra fase. Termina con commit, push y un resumen corto
   guardado también en docs/fases/resumen-fase-13b-cierre.md.
   ```
