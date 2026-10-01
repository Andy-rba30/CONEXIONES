# Fase 2: Core (Contrato, Esquema, Unidades y Validaciones)

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt`. Versión del add-in: 0.1.0.

---

## 1. Qué se hizo

- **Modelos fuertemente tipados del contrato v1 (`gusset_node`)** en `src/MotorConexiones.Core/Contract/`:
  - `ConnectionSpec.cs`: modelo raíz (`spec_version: "1.0"`, `connection_type: "gusset_node"`, `source`, `node`, `chord`, `gusset`, `members`, `dimension_chains`, `uncertain_fields`).
  - `GussetSpec.cs`, `GussetOutline.cs`: espesores, rótulos, ancho, alto, contorno poligonal en mm, interfaz con el cordón y soldadura.
  - `MemberSpec.cs`, `AttachmentSpec.cs`: miembros diagonales y montantes, retiros de extremo (`end_setback_mm`) y uniones ranuradas soldadas (`welded_slot`) o con placa cuchilla empernada (`bolted_knife_plate`).
  - `KnifePlateSpec.cs`, `BoltPatternSpec.cs`, `WeldSpec.cs`: placa cuchilla, patrón de pernos (diámetro, rótulo, filas, columnas, paso, distancia al borde, distancia a extremo) y soldaduras de filete.
  - `DimensionChain.cs`, `UncertainField.cs`: cadenas de cotas e incertidumbres explícitas con confirmación obligatoria.
- **Geometría plana 2D (`src/MotorConexiones.Core/Geometry2D/`)**:
  - `Point2D.cs` y `Segment2D.cs`: operaciones vectoriales e intersección de segmentos.
  - `Polygon2D.cs`: polígono plano cerrado, cálculo de área con signo (Shoelace), verificación de polígono simple y cerrado (detección de auto-intersección de aristas para `OUTLINE_INVALID`) y prueba de punto interior (ray casting).
  - `Geometry2DChecks.cs`: comprobación de que el grupo de pernos está contenido dentro de la placa cuchilla (`BOLT_OUTSIDE_PLATE`) y de que la placa cuchilla expuesta solapa dentro del contorno de la cartela (`PLATE_OUTSIDE_GUSSET`).
- **Límites configurables AISC 360 y cargador en tiempo de ejecución**:
  - `config/limits.json`: completado con la tabla AISC 360 J3.4 (distancias mínimas al borde por diámetro de perno en mm: 1/2"→19 mm, 5/8"→22 mm, 3/4"→25 mm, 7/8"→28 mm, 1"→32 mm, etc.), factor de separación mínima AISC J3.3 (2.667*d) y tabla AISC 360 J2.4 (tamaño mínimo de soldadura de filete por espesor de placa: $\le 6$ mm → 3 mm; $6 < t \le 13$ mm → 5 mm; $13 < t \le 19$ mm → 6 mm; $> 19$ mm → 8 mm).
  - `src/MotorConexiones.Core/Validation/LimitsConfig.cs`: carga desde archivo JSON o string en tiempo de ejecución, con valores por defecto seguros si no se proporciona ruta.
- **Análisis de rótulos de planos y comparación de perfiles**:
  - `src/MotorConexiones.Core/Validation/LabelParser.cs`: interpreta rótulos habituales de planos de acero (`3/8"`, `5/8"`, `PL10`, `PL 3/8"`, etc.) y verifica su correspondencia con los valores numéricos en mm dentro de la tolerancia de 0.05 mm (`LABEL_VALUE_MISMATCH`). Las conversiones pasan estrictamente por `UnitConverter`.
  - `src/MotorConexiones.Core/Validation/ProfileMatcher.cs`: compara designaciones normalizadas AISC (ej. `HSS2-1/2X2-1/2X3/16` equivale a `HSS2-1-2X2-1-2X3-16 64x64`) y dimensiones de sección, generando hasta 3 sugerencias con distancia Levenshtein cuando no coinciden (`PROFILE_MISMATCH`).
- **Validador estructural de esquema propio (sin dependencias NuGet adicionales)**:
  - `src/MotorConexiones.Core/Schema/JsonSchemaValidator.cs`: validador ligero propio basado exclusivamente en `System.Text.Json` que verifica obligatorios, tipos, rangos, enumeraciones y rechaza propiedades desconocidas (`additionalProperties: false` para `SCHEMA_INVALID`). Además, genera el JSON Schema Draft-07 estándar de `gusset_node`.
- **Firma canónica de validación (`validation_token`)**:
  - `src/MotorConexiones.Core/Validation/ValidationTokenGenerator.cs`: genera el hash SHA-256 canónico y determinista sobre el JSON ordenado de la especificación + datos de contexto del modelo (`ProjectInformation.UniqueId`, `UniqueId` y extremos de curva de miembros redondeados a 0.1 mm con `UnitConverter.RoundMm`).
- **Validador maestro de especificaciones (`SpecValidator.cs`)**:
  - Coordina las 10 reglas de validación de la sección 8 del encargo.
- **Abstracción del modelo (`src/MotorConexiones.Core/Model/`)**:
  - `IModelFacts.cs` y `MemberModelFacts.cs`: interfaz desacoplada para consultar existencias, perfiles, ángulos e interferencias en Revit. La Fase 3 la implementará sobre la API de Revit; las pruebas unitarias utilizan una implementación simulada (`FakeModelFacts`).
- **Tipo de conexión `gusset_node`**:
  - `src/MotorConexiones.Core/Types/GussetNodeType.cs`: implementación de `IConnectionType` que aporta esquema, ejemplo completo y validación.
- **Fixture Detalle D**:
  - `docs/fixtures/detalle-D.json`: especificación completa del caso de prueba con los IDs y coordenadas reales del nudo del Hangar.
- **Suite de pruebas unitarias xUnit (`SpecValidationTests.cs`)**:
  - 17 nuevas pruebas que cubren exhaustivamente las 7 pruebas mínimas del encargo más casos límite de geometría, límites AISC, normalización de perfiles y token determinista. Total de la solución: **38 pruebas, 100% pasando**.

---

## 2. Qué se probó en la sesión y cómo

### 2.1 Compilación completa de la solución

```text
$ dotnet build MotorConexiones.sln -c Release
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core  -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores
Tiempo transcurrido 00:00:03.78
```

> [!NOTE]
> `TreatWarningsAsErrors` está activo en todos los proyectos. No se generó ninguna advertencia ni error al compilar los tres proyectos (`Core`, `Revit` y `Tests`).

### 2.2 Pruebas unitarias automatizadas (`dotnet test -c Release`)

```text
$ dotnet test -c Release --logger "console;verbosity=normal"
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> bin/Release/net10.0/MotorConexiones.Tests.dll

Serie de pruebas para MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
Total de pruebas: 38. Correctas: 38, Con error: 0, Omitidas: 0.
Tiempo total: 0.88 segundos.
```

### 2.3 Matriz de cobertura de las pruebas mínimas del encargo (sección 12)

| Requisito del encargo (Sección 12) | Prueba unitaria | Resultado |
|---|---|---|
| **1. Detalle D con dudas confirmadas valida sin errores** | `DetalleD_ValidSpec_WithConfirmedUncertainties_ValidatesCleanlyAndProducesToken` | **PASÓ** (0 errores, `validation_token` SHA-256 de 64 caracteres generado). |
| **2. Borde superior 420 cambiado a 402 da `DIMENSION_CHAIN_MISMATCH`** | `DimensionChain_Mismatch_420_To_402_YieldsDimensionChainMismatch` | **PASÓ** (`DIMENSION_CHAIN_MISMATCH` reportado con diferencia 18 mm > tolerancia 1 mm). |
| **3. Pernos con borde de 15 mm da `BOLT_EDGE_DISTANCE_TOO_SMALL`** | `BoltEdgeDistance_15mm_YieldsBoltEdgeDistanceTooSmall` | **PASÓ** (`BOLT_EDGE_DISTANCE_TOO_SMALL` reportado contra el mínimo de 22 mm de tabla J3.4). |
| **4. `thickness_label` "3/8\"" con `thickness_mm` 12 da `LABEL_VALUE_MISMATCH`** | `LabelValueMismatch_ThicknessLabel3_8_With12mm_YieldsLabelValueMismatch` | **PASÓ** (`LABEL_VALUE_MISMATCH` detectado por diferencia de 2.475 mm > tolerancia 0.05 mm). |
| **5. Duda del montante sin confirmar impide obtener el token** | `UnresolvedUncertainty_PreventsValidationToken` | **PASÓ** (`UNRESOLVED_UNCERTAINTY` generado, `ValidationToken` resulta `null`). |
| **6. JSON Schema acepta el fixture y rechaza campo desconocido** | `JsonSchema_AcceptsFixtureAndRejectsUnknownProperty` | **PASÓ** (Acepta `detalle-D.json`; rechaza claves no definidas en raíz y en objetos anidados con `SCHEMA_INVALID`). |
| **7. Conversión de unidades reversible mm ↔ pies ↔ mm (< 0.001 mm)** | `UnitConversion_MmToFeetToMm_ReversibleUnder1Micron` | **PASÓ** (Error medido < 1e-9 mm en todo el rango). |
| **8. Contorno poligonal auto-intersecante** | `Polygon2D_SelfIntersecting_YieldsOutlineInvalid` | **PASÓ** (`OUTLINE_INVALID` reportado al cruzarse aristas no consecutivas). |
| **9. Pernos fuera de placa cuchilla** | `BoltPattern_OutsidePlate_YieldsBoltOutsidePlate` | **PASÓ** (`BOLT_OUTSIDE_PLATE` emitido cuando el ancho de placa es insuficiente). |
| **10. Placa cuchilla fuera de cartela** | `KnifePlate_OutsideGusset_YieldsPlateOutsideGusset` | **PASÓ** (`PLATE_OUTSIDE_GUSSET` emitido al salir esquinas del contorno). |
| **11. Separación de pernos menor a 2.667*d** | `BoltSpacing_TooSmall_YieldsBoltSpacingTooSmall` | **PASÓ** (`BOLT_SPACING_TOO_SMALL` emitido con $s = 30$ mm vs mínimo 42.3 mm). |
| **12. Soldadura por debajo de tabla AISC J2.4** | `Weld_BelowMinimum_YieldsWarningWeldBelowMinimum` | **PASÓ** (Emite advertencia `WELD_BELOW_MINIMUM` sin bloquear el token). |
| **13. Equivalencia normalizada de perfiles y sugerencias** | `ProfileMatcher_Equivalence_HSSNormalized` | **PASÓ** (Detecta coincidencia `HSS2-1/2X2-1/2X3/16` vs `HSS2-1-2X2-1-2X3-16 64x64`; sugiere perfiles más cercanos). |
| **14. Validaciones dependientes del modelo (IDs y ángulos)** | `ModelFacts_Validations_ElementNotFound_And_AngleDiffers` | **PASÓ** (`ELEMENT_NOT_FOUND` al pasar ID inexistente; `ANGLE_DIFFERS_FROM_MODEL` como advertencia). |
| **15. Token canónico determinista independiente del orden de claves** | `CanonicalJson_ProducesDeterministicSha256Token` | **PASÓ** (Mismo SHA-256 ante distinto orden de propiedades). |
| **16. Carga de `config/limits.json` desde disco** | `LimitsConfig_LoadsFromRepositoryFile` | **PASÓ** (Verifica `schema_version: 1`, tolerancias y valores de tablas AISC). |
| **17. Registro y provisión de esquema/ejemplo de `GussetNodeType`** | `GussetNodeType_RegistersAndProvidesSchemaAndExample` | **PASÓ** (Verifica esquema Draft-07 y ejemplo completo deserializable). |

---

## 3. Pruebas en Revit y Agente Instalador

> [!NOTE]
> **No se requiere ejecución en Revit ni agente instalador para la Fase 2.**
>
> Según la sección 13 del encargo (`docs/ENCARGO_MOTOR_CONEXIONES.md`):
> *"Fase 2. Core: Contrato, esquema, unidades, validaciones, config/limits.json, fixture Detalle D y todas las pruebas de la sección 12. Se prueba en la nube con dotnet test; no necesita Revit ni instalador."*
>
> Todo el código reside en `MotorConexiones.Core` y `MotorConexiones.Tests`, sin interacción con APIs nativas ni procesos externos. Las pruebas del modelo se validaron con la suite simulada de hechos (`FakeModelFacts`).

---

## 4. Decisiones tomadas y por qué

1. **Abstracción `IModelFacts` en el Core**:
   Para cumplir con la regla de que el Core no dependa de Revit (`netstandard2.0`), todas las comprobaciones que involucran el estado del modelo (`PROFILE_MISMATCH`, `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE`, `ANGLE_DIFFERS_FROM_MODEL`, `CLASH_WITH_FOREIGN_MEMBER`) y los datos para la firma del token (`ProjectInformation.UniqueId`, `UniqueId` y curvas de los miembros) se consultan a través de `IModelFacts`. La Fase 3 implementará `RevitModelFacts` dentro de `MotorConexiones.Revit` sin modificar el Core.
2. **Validador de JSON Schema propio**:
   Siguiendo la condición de no añadir nuevos paquetes NuGet externos y usar solo `System.Text.Json`, se implementó `JsonSchemaValidator`. Aplica una validación estricta de árbol con soporte explícito para `additionalProperties: false`, garantizando que cualquier clave desconocida o mal escrita produzca `SCHEMA_INVALID`.
3. **Conversión unificada de unidades (prohibido 304.8 fuera de `UnitConverter`)**:
   Se ampliaron los métodos de `UnitConverter` con `InchesToMm` y `MmToInches` derivados exclusivamente de `FeetToMm` y `MmToFeet`. `LabelParser` y todos los algoritmos geométricos usan estas funciones sin literales dispersos.
4. **Normalización flexible de perfiles HSS**:
   Revit y las bibliotecas habituales reemplazan `/` por `-` en nombres de familia (ej. `HSS2-1-2X2-1-2X3-16`) y suelen añadir sufijos métricos como ` 64x64`. `ProfileMatcher` normaliza expresiones fraccionarias, remueve sufijos métricos y compara los componentes outer width, height y thickness, garantizando que los planos con designación estándar AISC coincidan con las familias cargadas en el modelo.
5. **Polígono cerrado canónico en `Polygon2D`**:
   Los vértices en JSON pueden incluir o no el punto de cierre duplicado. `Polygon2D` remueve el vértice final si es idéntico al primero para operar en representación canónica, aplicando Shoelace y chequeos de intersección arista por arista.
6. **Manejo de tolerancias configurable**:
   Todas las tolerancias numéricas (cadenas de cotas: 1.0 mm; rótulos: 0.05 mm; ángulos: 1.0°; nudo: 5.0 mm; pernos y soldaduras AISC) se leen directamente de `config/limits.json`, permitiendo ajustarlas en obra o taller sin recompilar.

---

## 5. Pendientes, riesgos y preparación para la Fase 3

### 5.1 Estado de la Fase 2
- **Fase 2 COMPLETADA al 100%**:
  - Compilación en Release: 0 advertencias, 0 errores.
  - Pruebas xUnit: 38 pruebas en verde.
  - Fixture `docs/fixtures/detalle-D.json` preparado y validado.
  - Límites en `config/limits.json` completados.

### 5.2 Preparación para la Fase 3 (Add-in de Revit)
- La Fase 3 conectará este Core con Revit 2027:
  - Implementación de `RevitModelFacts : IModelFacts` en `MotorConexiones.Revit/Node/`.
  - Conexión del backend de fabricación de Advance Steel (confirmado como camino A en la Fase 1) detrás de `IFabricationBackend` para generar cartela, pernos y soldaduras.
  - Almacenamiento en Extensible Storage (`MotorConexionesConnection`).
  - Botón de la cinta Ribbon leyendo y validando el JSON de Detalle D mediante `SpecValidator`.
