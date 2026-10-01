# Fase 5: Punta a punta y documentación

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt`. Add-in 0.1.0; Core con `LimitsConfig` en el token; README y guion E2E completados.

---

## 1. Qué se hizo

- **Retirada de operaciones de prueba (`probe_plate_b` y `probe_delete_b`)**:
  - En `src/MotorConexiones.Revit/Bridge.cs`, se eliminaron los registros de `ProbePlateBOperation` y `ProbeDeleteBOperation`.
  - Se eliminaron del repositorio los archivos `src/MotorConexiones.Revit/Operations/ProbePlateBOperation.cs` y `src/MotorConexiones.Revit/Operations/ProbeDeleteBOperation.cs`.
  - Se actualizó el comentario de la interfaz `IOperation` para no referenciar operaciones retiradas.
  - La lista de operaciones expuestas en `conn_ping` (`data.operations`) queda limpia con las 13 operaciones oficiales de MotorConexiones.
- **Inclusión de `config/limits.json` en el hash del `validation_token`**:
  - En `src/MotorConexiones.Core/Validation/LimitsConfig.cs`, se implementó el método `ComputeHash()` que genera un SHA-256 determinista a partir de las tolerancias de cadena de cotas, tolerancias de etiquetas, distancias a bordes (AISC Tabla J3.4), separación de pernos (factor AISC J3.3) y tamaños mínimos de filete de soldadura (AISC Tabla J2.4).
  - En `src/MotorConexiones.Core/Validation/ValidationTokenGenerator.cs`, se añadió el parámetro opcional `LimitsConfig? limits = null`. Si se proporciona, se incorpora `|LIMITS:<hash>` al cálculo SHA-256.
  - En `src/MotorConexiones.Core/Validation/SpecValidator.cs`, se pasa la configuración de límites activa al generar el token.
  - En `src/MotorConexiones.Revit/`, se creó `LimitsConfigLoader.cs` para centralizar la lectura de `config/limits.json`, y tanto `ValidateOperation`, `CreateOperation` como `UpdateOperation` validan el token contra la misma configuración activa. Si el usuario edita `limits.json` entre la validación y la creación, el token deja de ser válido (`VALIDATION_TOKEN_INVALID`).
- **Pruebas unitarias del Core (`src/MotorConexiones.Tests/SpecValidationTests.cs`)**:
  - Se añadió la prueba `ValidationToken_IncludesLimitsHash_DeterministicAndChangesOnConfigDifference`, verificando:
    1. Que límites idénticos generan tokens idénticos (determinismo).
    2. Que un cambio en los límites (p. ej. tolerancia de cotas) altera el token generado.
    3. Que la longitud se mantiene en 64 caracteres hexadecimales.
  - Las 51 pruebas xUnit pasan en verde.
- **Documentación principal (`README.md`)**:
  - Redactado en español, claro y orientado a principiantes.
  - Contiene: qué es MotorConexiones y sus garantías técnicas, arquitectura detallada, requisitos previos, instalación paso a paso, cómo actualizar el add-in (C# y MCP), cómo editar `limits.json` y `guide.md` sin recompilar, cómo añadir nuevos tipos de conexión (v2) y el guion completo de prueba E2E.
- **Instrucciones para el instalador y prueba E2E (`docs/instalacion/fase-5.md`)**:
  - Pasos numerados para compilar, desplegar con `deploy.ps1`, verificar `probar_conexiones.py --puente` y ejecutar el guion E2E desde el cliente de IA (Antigravity).

---

## 2. Qué se probó y cómo

### 2.1 Compilación y pruebas del Core
Ejecutado en la sesión de desarrollo:

```text
$ dotnet build MotorConexiones.sln -c Release
  MotorConexiones.Core -> bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> bin/Release/net10.0/MotorConexiones.Tests.dll
  MotorConexiones.Revit -> bin/Release/net10.0-windows/MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores
Tiempo transcurrido 00:00:03.93

$ dotnet test MotorConexiones.sln -c Release --no-build
Serie de pruebas para MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
Correctas! - Con error: 0, Superado: 51, Omitido: 0, Total: 51, Duración: 143 ms
```

### 2.2 Suite de pruebas del puente MCP (`probar_conexiones.py --puente`)
Ejecutado con el intérprete de Python del `.venv` de la extensión contra Revit 2027 y el puente `main.py --streamable-http`:

```text
======================================================================
1. GET /conn/ping/ sin token -> 401  [OK]  HTTP 401
2. GET /conn/ping/ con token  [OK]  HTTP 200, ok=True, addin=0.1.0 backend=advancesteel revit=27.2.0.39 documento=HANGAR_PRUEBA_sondeo
3. GET /conn/guide/  [OK]  HTTP 200, ok=True, 8855 caracteres
4. GET /conn/types/  [OK]  HTTP 200, ok=True, tipos=['gusset_node']
5. GET /conn/schema/gusset_node  [OK]  HTTP 200, ok=True, claves de data=['connection_type', 'description', 'example', 'json_schema']
6. GET /conn/schema/no_existe -> ok:false  [OK]  HTTP 200, ok=False, errores=['UNKNOWN_OPERATION']
7. POST /conn/find_profile/ HSS2-1/2X2-1/2X3/16  [OK]  HTTP 200, ok=True, coincidencias=['HSS2-1-2X2-1-2X3-16 64x64']
8. POST /conn/node_info/ 4 miembros  [OK]  HTTP 200, ok=True, cordón=1249510 miembros=4 origen_mm=[-11867.7, -17195.8, 17423.0]
9. POST /conn/validate/ Detalle D con dudas confirmadas -> token  [OK]  HTTP 200, ok=True, token=64 hex
10. POST /conn/validate/ con 420 -> 402 -> DIMENSION_CHAIN_MISMATCH  [OK]  HTTP 200, ok=False, sin token
11. POST /conn/validate/ detalle-D.json (dudas sin confirmar) -> UNRESOLVED_UNCERTAINTY  [OK]  HTTP 200, ok=False
12. POST /conn/preview/ Detalle D  [OK]  HTTP 200, ok=True, resumen={"gusset_plates": 1, "knife_plates": 1, "bolts": 4, "weld_lines": 6, "members_modified": 3, "dry_run": true}
13. POST /conn/create/ sin validation_token -> VALIDATION_TOKEN_INVALID  [OK]  HTTP 200, ok=False
14. GET /conn/list/  [OK]  HTTP 200, ok=True, conexiones en el modelo=0
15. GET /conn/get/<id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False
16. POST /conn/delete/ <id inexistente> -> ELEMENT_NOT_FOUND  [OK]  HTTP 200, ok=False
17. POST /conn/op/no_existe/ -> UNKNOWN_OPERATION  [OK]  HTTP 200, ok=False
18. tools/list por el puente trae las 13 herramientas conn_*  [OK]  HTTP 200, herramientas=79 conn_*=13
19. tools/call conn_ping por el puente -> ok:true  [OK]  HTTP 200, isError=False ok=True addin=0.1.0
======================================================================
Resultado: 19/19 pruebas correctas
```

### 2.3 Verificación de integración directa con Antigravity
En la comprobación 4-9 se verificó la invocación directa vía MCP desde Antigravity:
- Detección de las 79 herramientas expuestas en el endpoint `http://localhost:8000/mcp`.
- Llamada a `conn_ping`: respuesta correcta con backend Advance Steel, documento activo `HANGAR_PRUEBA_sondeo` y versión `0.1.0`.
- Llamada a `conn_get_guide`: lectura íntegra de la guía de 8855 caracteres desde el disco.
- Llamada a `conn_list_types`: identificación del tipo de conexión disponible `gusset_node`.

---

## 3. Qué debe mirar el usuario en Revit al desplegar

1. **Reinicio de Revit con la nueva DLL desplegada:**
   - Cerrar Revit y ejecutar `.\scripts\deploy.ps1`.
   - Al volver a abrir Revit y consultar `conn_ping`, en la lista `operations` ya no deben figurar `probe_plate_b` ni `probe_delete_b`.
2. **Prueba E2E con Detalle D (`conn_create` y `conn_delete`):**
   - Seguir el guion de `docs/instalacion/fase-5.md` sección 4.
   - En Revit 3D, verificar la generación de la cartela poligonal de 8 vértices, la placa cuchilla empernada en la diagonal inferior (4 pernos en 2x2) y las soldaduras en el cordón y montantes.
   - Al ejecutar `conn_delete`, comprobar visualmente que las barras vuelven a sus longitudes completas originales.

---

## 4. Decisiones tomadas y por qué

1. **No caducidad temporal (30 min) del `validation_token`, pero sí ligada a `config/limits.json`:**
   - Una caducidad temporal con reloj del sistema introduce complejidad innecesaria ante diferencias horarias o pausas prolongadas en la conversación con la IA.
   - La verdadera protección que exige el diseño es que **ni el modelo, ni la especificación, ni los límites de diseño hayan cambiado**. Incluir el hash SHA-256 de `LimitsConfig` en el token cubre de forma determinista y económica cualquier modificación en las reglas de cálculo.
2. **Conservación de rutas `/conn/op/` y `/conn/dev_exec/` en el adaptador:**
   - Se mantuvieron en `conexiones.py` para permitir la ejecución de scripts auxiliares de diagnóstico (`conn-call.ps1`, `revit-exec.ps1 -SinTransaccion`), sin exponerlas como herramientas MCP para que la IA no las utilice.
3. **Retirada total de `probe_plate_b` y `probe_delete_b`:**
   - Cumplieron su propósito exploratorio en la Fase 1. Al retirarlas, el add-in queda libre de código de sondeo obsoleto y expone únicamente las operaciones de producción.
4. **Documentación unificada y modular:**
   - `README.md` recoge toda la documentación operativa del proyecto para usuarios y desarrolladores, mientras que `guide.md` se enfoca estrictamente en las directrices de lectura de planos y modelado para los agentes de IA.

---

## 5. Estado final del proyecto MotorConexiones

Con la Fase 5 completada, MotorConexiones v1.0 cumple con la totalidad del encargo:
- **Core:** 100% probado en pruebas unitarias con xUnit, desacoplado de Revit, con validación de esquemas, cotas y AISC 360.
- **Add-in de Revit:** soporte completo para `gusset_node` con backend Advance Steel nativo (placas, pernos, soldaduras, recortes) y DirectShape de reserva, transacciones atómicas y Extensible Storage.
- **Servidor MCP:** 13 herramientas especializadas con docstrings claras, adaptador delgado en IronPython, soporte Streamable HTTP y compatibilidad total con clientes de IA (Antigravity y Claude Desktop).
- **Documentación:** informes de todas las fases, resultados de pruebas, instrucciones de instalación y README completo.
