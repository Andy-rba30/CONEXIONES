# Qué sigue tras los resultados de la ronda 8b (2026-10-05)

Rama `main`, último commit del instalador: `ff22ace`. Este archivo es la copia del mensaje del chat, por si no se ve.

## 1. El siguiente paso es la sesión de cierre de la Fase 8 (todavía no la Fase 9)

Está escrito en `docs/fases/fase-8.md`, sección 6, paso 5.

**Dónde se pega:** en **Claude Code**, en una **sesión nueva en la nube** (como esta). No necesita Revit.

```
Lee CLAUDE.md, docs/fases/fase-8.md (sección 7) y docs/fases/resultados-fase-8b.md. Cierra la Fase 8: contrasta los
resultados de la ronda 8b con lo esperado en 7.5, corrige lo que haga falta (ronda 8c solo si es imprescindible),
actualiza el informe y la tabla de garantías del README. No empieces la Fase 9. Termina con commit, push y un resumen corto.
```

## 2. Antes de abrir esa sesión: falta la salida de las 28 pruebas

El instalador dijo que guardó la salida de `probar_conexiones.py --puente` en `docs/fases/resultados-fase-8b.md`,
pero **no es cierto**: el archivo termina en el sondeo 13. Faltan la salida de las 28 pruebas y el log del día.
La sesión de cierre los necesita para saber por qué fallaron las pruebas 22, 23 y 27.

Dos opciones:

- **La fácil (sin Revit):** copia del chat del instalador la salida completa de las 28 pruebas y pégala en la sesión
  de Claude Code justo debajo del prompt de cierre.
- **La completa (Revit abierto y servidor arrancado):** pásale este bloque a **tu agente instalador en el PC**.
  Lo añade al archivo y lo sube.

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
$salida = "docs\fases\resultados-fase-8b.md"
$py = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension\.venv\Scripts\python.exe"
"`n## 8b-7 probar_conexiones --puente`n`n``````text" | Add-Content -Encoding UTF8 $salida
& $py mcp\pruebas\probar_conexiones.py --puente 2>&1 | Out-String | Add-Content -Encoding UTF8 $salida
"``````" | Add-Content -Encoding UTF8 $salida
"`n## 8b-7 log del dia`n`n``````text" | Add-Content -Encoding UTF8 $salida
Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" | Select-String "batch_plan|ribbon_batch|startup" | Select-Object -Last 30 | Out-String | Add-Content -Encoding UTF8 $salida
"``````" | Add-Content -Encoding UTF8 $salida
git add $salida
git commit -m "Ronda 8b: salida de probar_conexiones --puente y log del dia"
git push origin main
```

## 3. Lo que la sesión de cierre tendrá que resolver

Visto al contrastar los resultados con la sección 7.5 de `docs/fases/fase-8.md`:

| Qué | Esperado (7.5) | Qué salió | Qué es |
|---|---|---|---|
| Recuento del plan | 53 elementos: `ready: 10, no_match: 26, untyped: 17` | 56 elementos: `ready: 16, no_match: 20, untyped: 23` | Seleccionaste 3 cordones más; hay que explicarlo, no es un fallo |
| Pruebas 22 y 23 (`--puente`) | `N1 ready` sobre el Detalle D | `untyped` | Es justo lo que la ronda 8b quería arreglar; hay que investigarlo con la salida de las pruebas |
| Prueba 27 (21 herramientas por el puente) | pasa | falló | Puede ser solo que el servidor no había terminado de arrancar tras los 20 s |
| Sondeo 17, paso 10 (rollback) | limpieza y rollback sin error | `The referenced object is not valid...` | Fallo pequeño del sondeo (lee el marcador después de deshacerlo), no de Revit |
| Sondeo 18 sobre el Detalle D | 85 / 20 / 48 mm del corte, cantos 76,2 y 63,5 | 86,2 / 20,4 / 47,4 mm, cantos correctos | Coincide |
| Ventana, Editar nudo, aviso de `Marca` | activo en `ready`, sin aviso | activo, sin aviso | Coincide |
| Extensiones de 1249630 / 1249631 | pendiente desde la 7b | siguen en 68,6 / 69,2 mm | Anotar o poner a 0 (sección 7.8) |

## 4. Después del cierre, y solo entonces, la Fase 9

**Dónde se pega:** en **Claude Code**, en otra sesión nueva, cuando la sesión de cierre haya terminado con commit y push.

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-8.md y docs/propuestas/catalogo-y-lotes.md completo.
Escribe docs/prompts/fase-9.md (crear por lotes: secciones 3.5, 4 y 6 de la propuesta, con las decisiones de 7.1)
y ejecuta SOLO la Fase 9. Termina con docs/fases/fase-9.md, docs/instalacion/fase-9.md, commit, push y un resumen corto.
```

## 5. Regla general: a quién va cada cosa

- Lo que empieza por **"Lee CLAUDE.md..."** → **Claude Code** (sesión nueva en la nube).
- Lo que son **comandos de PowerShell** o archivos de **`docs/instalacion/`** → **tu agente instalador en el PC**.
