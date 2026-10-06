# CLAUDE.md — MotorConexiones

Reglas permanentes de este repositorio. El encargo completo está en `docs/ENCARGO_MOTOR_CONEXIONES.md`
y se ejecuta **una fase por sesión** (sección 13 del encargo). Léelo entero antes de trabajar.

## Qué es esto

Add-in de Revit en C# (`MotorConexiones`) que crea conexiones de acero (cartelas, placas, pernos,
soldaduras, cortes) a partir de una especificación JSON. Lo maneja una IA a través del servidor MCP
`revit-mcp` (repositorio aparte, Python + pyRevit), con herramientas nuevas de prefijo `conn_`.

## Cómo trabajar

- El usuario es principiante: explica en español sencillo, con archivo, botón y comando exactos.
- Ejecuta solo la fase pedida. Al terminar: compila, pruebas en verde, commit, `docs/fases/fase-N.md`
  (Anexo B del encargo) y un resumen corto en el chat.
- Nunca informes como probado lo que no ejecutaste. Marca "NO PROBADO" y explica por qué.
- La sesión corre en la nube: **no hay Revit ni MCP**. Lo que necesite Revit se escribe como sondeo
  en `scripts/sondeos/` con instrucciones en `docs/instalacion/fase-N.md`; un agente instalador lo
  ejecuta en el PC del usuario y la salida vuelve en `docs/fases/resultados-fase-N.md` o en el chat.
- No inventes miembros de la API de Revit: compílalos contra la API 2027 o pruébalos con un sondeo.
- El repositorio `revit-mcp` es de solo lectura. Los archivos nuevos del MCP van en `mcp/` de este
  repositorio y se instalan con `mcp/instalar-conn.ps1`. No se modifican herramientas existentes.
- Código, clases, archivos de código y claves JSON en inglés. Mensajes, errores, comentarios
  importantes, informes y documentación en español.
- Commits claros en español. Sin pull requests. Solo la rama de trabajo indicada.

## Comandos

```powershell
dotnet build MotorConexiones.sln -c Release   # nube y PC
dotnet test                                   # nube y PC
.\scripts\deploy.ps1                            # solo PC (instalador)
.\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py   # solo PC: IronPython dentro de Revit
```

## Estructura (ver sección 4 del encargo)

- `src/MotorConexiones.Core` — netstandard2.0, sin Revit: contrato, esquema, unidades, validación, catálogo (`Catalog/`)
  y detección de nudos y plan de lote (`Batch/`, Fase 8) con sus textos en español y el alzado de la cercha
  (`PlanAdvice`, `TrussMap`, ronda 8c); crear por lotes (Fase 9: `BatchCreateRequest`, `BatchReport`, `BatchRunner` =
  qué nudos se crean y en qué orden, con el creador como delegado; estados `created` y `failed`; botones de "Qué hacer").
- `src/MotorConexiones.Revit` — add-in: cinta, `Bridge.Handle`, nudo, fabricación, almacenamiento, catálogo, plan y lote
  (`Batch/`: marcas en el modelo, planes en memoria, `PlanEvents` = el `ExternalEvent` de la ventana no modal del plan,
  `BatchCreator` = grupo exterior del lote + un `OperationScope` por nudo, y `DeleteBatch`; `UI/`: ventanas WPF de los
  botones, con el mapa de la cercha `TrussMapCanvas` y los botones Crear N conexiones y Borrar el lote).
- `src/MotorConexiones.Tests` — xUnit, solo Core, fixture Detalle D.
- `config/limits.json`, `config/catalog.json`, `docs/guide.md` — editables sin recompilar.
- `catalog/` — plantillas oficiales del catálogo de conexiones (Fase 7); `deploy.ps1` copia al PC las que falten.
- `docs/fases/` — un informe por fase y los resultados devueltos por el instalador.
- `docs/instalacion/` — instrucciones literales para el agente instalador, una por fase.
- `docs/propuestas/` — ideas que se aclaran con el usuario antes de convertirse en fase (sin código hasta entonces).
- `docs/prompts/` — prompt y alcance de cada fase posterior al encargo (`fase-N.md`), escritos a partir de la propuesta.
- `mcp/` — archivos nuevos del MCP (`revit_mcp/conexiones.py`, `tools/conn_tools.py`, pruebas, instalador).
- `scripts/` — `deploy.ps1`, `revit-exec.ps1`, `conn-call.ps1` y `sondeos/` (el 20 prueba los grupos anidados con Advance Steel).

## Reglas técnicas que no se negocian

- Conversión de unidades en un único archivo (`Units/UnitConverter.cs`).
- Una operación = un `TransactionGroup`; error = rollback completo. En el lote (Fase 9) cada nudo es una operación
  anidada en un grupo exterior que se asimila: el nudo que falla se revierte solo; `batch_single_undo: false` es el plan B.
- Ninguna ventana en las rutas que usa la IA; el único diálogo permitido es el del botón de la cinta.
- `ElementId.Value` (long), nunca `IntegerValue`.
- `conn_create` y `conn_update` exigen `validation_token` de `conn_validate`.
