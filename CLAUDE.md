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
- No inventes miembros de la API de Revit: pruébalos antes con `execute_revit_code` (MCP `revit`)
  o compilando un fragmento.
- En el repositorio `revit-mcp` solo se añaden archivos nuevos y las líneas de registro indicadas en
  la sección 9 del encargo. No se modifican ni borran herramientas existentes.
- Código, clases, archivos de código y claves JSON en inglés. Mensajes, errores, comentarios
  importantes, informes y documentación en español.
- Commits claros en español. Sin pull requests. Solo la rama de trabajo indicada.

## Comandos

```powershell
dotnet build MotorConexiones.sln -c Release
dotnet test
.\scripts\deploy.ps1
```

## Estructura (ver sección 4 del encargo)

- `src/MotorConexiones.Core` — netstandard2.0, sin Revit: contrato, esquema, unidades, validación.
- `src/MotorConexiones.Revit` — add-in: cinta, `Bridge.Handle`, nudo, fabricación, almacenamiento.
- `src/MotorConexiones.Tests` — xUnit, solo Core, fixture Detalle D.
- `config/limits.json`, `docs/guide.md` — editables sin recompilar.
- `docs/fases/` — un informe por fase.

## Reglas técnicas que no se negocian

- Conversión de unidades en un único archivo (`Units/UnitConverter.cs`).
- Una operación = un `TransactionGroup`; error = rollback completo.
- Ninguna ventana en las rutas que usa la IA; el único diálogo permitido es el del botón de la cinta.
- `ElementId.Value` (long), nunca `IntegerValue`.
- `conn_create` y `conn_update` exigen `validation_token` de `conn_validate`.
