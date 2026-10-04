# Prompts de la ronda 7b

## 1. Para el agente instalador en el PC (ronda 7b)

Lee docs\instalacion\fase-7b.md del repositorio D:\Proyectos C#\CONEXIONES y ejecutalo entero, en orden, en una sola
ventana de PowerShell. Reglas: usa la funcion Anota tal como esta escrita (no crees scripts nuevos); repite
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force si abres otra ventana; Revit lo abre y lo cierra la
persona, no tu; no ejecutes nada mientras una ventana del add-in este abierta en Revit; no toques src\, config\, mcp\
ni docs\fixtures\; si un paso falla, copia el error y sigue con el siguiente. Sustituye <ID> por el ID de conexion que
te de la persona en el paso 7b-4. Al terminar, haz el commit y el push del paso 7b-6 y devuelveme: la salida completa
de los pasos 7b-1, 7b-2, 7b-3, 7b-5 y 7b-6, el git status --short literal antes del commit y al final, las anotaciones
de la persona (token, numero de avisos, ID de conexion, de que lado apoya la placa cuchilla), las dos capturas y el
texto de cualquier ventana de error.

## 2. Para la sesion de cierre en la nube (cuando vuelvan los resultados)

Lee CLAUDE.md, docs/fases/fase-7.md (seccion 7) y docs/fases/resultados-fase-7b.md. Confirma que deploy y conn_ping
dicen 0.7.0, que el fixture dio 1 aviso y que la plantilla oficial de catalog\ quedo con tres diagonales. Anota el
cierre en fase-7.md y en el README, commit, push a main y un resumen corto. No empieces la Fase 8.

## 3. Para la Fase 8 (solo despues del cierre anterior)

Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-7.md y docs/propuestas/catalogo-y-lotes.md completo.
Escribe docs/prompts/fase-8.md (deteccion de nudos y plan: secciones 3.1, 3.2, 3.4, 4 y 6 de la propuesta, con las
decisiones de 7.1) y ejecuta SOLO la Fase 8. Termina con docs/fases/fase-8.md, docs/instalacion/fase-8.md, commit,
push y un resumen corto.
