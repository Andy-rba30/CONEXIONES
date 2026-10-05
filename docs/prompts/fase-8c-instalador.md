# Ronda 8c: prompts para el instalador y para la sesión de cierre (copia del chat)

Fecha: 2026-10-05. Add-in 0.8.3 en `main` (commit `ef95e09`). Instrucciones completas: `docs/instalacion/fase-8c.md`.

## Antes de pegarlo (la persona)

- Cierra Revit. Ten a mano la copia `HANGAR_PRUEBA_sondeo.rvt` (nunca el original).
- Cuando el instalador te lo pida (paso 8c-3), selecciona **la misma cercha de la 8b más sus cordones superior e
  inferior**. Anota cuántos elementos quedan seleccionados.
- Los pasos marcados **(la persona)** los haces tú en Revit. El instalador no ejecuta nada mientras tengas una ventana
  del add-in abierta.

## 1. Prompt para el agente instalador en el PC

```
Lee docs\instalacion\fase-8c.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, en una sola
ventana de PowerShell. Reglas: usa la función Anota tal como está escrita (no crees scripts nuevos); repite
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force si abres otra ventana; Revit lo abre y lo cierra la
persona, no tú; no ejecutes nada mientras una ventana del add-in esté abierta en Revit; los pasos marcados (la persona)
los hace la persona en Revit y tú esperas a que te diga que terminó; no toques src\, config\, mcp\ ni docs\fixtures\;
si un paso falla, copia el error, no modifiques nada y sigue con el siguiente. La variable del plan se llama $plan, no $pid.
Obligatorio: en el paso 8c-8 la salida de probar_conexiones.py --puente y el log del día van dentro del archivo
docs\fases\resultados-fase-8c.md con Anota; antes del commit comprueba con Select-String que el archivo tiene las
secciones "8c-8" del puente y del log, y si faltan, repítelas. Guarda las capturas con los nombres exactos que dice el
archivo en docs\fases\capturas\. Al terminar, haz el commit y el push del paso 8c-9 y devuélveme: el número de
elementos seleccionados en 8c-3, el summary_text y el resumen del plan (listos, con aviso, ocultos), la salida de los
sondeos 17 y 19, el resultado de --puente (x/28), las anotaciones de la persona, el git status --short literal antes
del commit y al final, el git log -1 --oneline y el texto de cualquier ventana de error.
```

## 2. Después: prompt para la sesión de cierre (Claude Code, sesión nueva en la nube)

Solo cuando el instalador haya subido `docs/fases/resultados-fase-8c.md` y las capturas.

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 9) y docs/fases/resultados-fase-8c.md. Cierra la ronda 8c: contrasta los
resultados con lo esperado en 9.3, corrige lo que haga falta, actualiza el informe y la tabla de garantías del README.
No empieces la Fase 9. Termina con commit, push y un resumen corto; guárdalo también en docs/fases/resumen-fase-8c-cierre.md.
```

## 3. Y después del cierre: la Fase 9

Con el prompt del paso 6 de la sección 6 de `docs/fases/fase-8.md`. Antes decide la pregunta P6 (sección 8.7): si los
14 nudos con cordón HSS4X4 se crean con la misma cartela del Detalle D o se dejan fuera hasta tener su propia plantilla.

## Regla de siempre

- Lo que empieza por "Lee docs\instalacion\..." o es PowerShell → **tu agente instalador en el PC**.
- Lo que empieza por "Lee CLAUDE.md..." → **Claude Code**, sesión nueva en la nube.
