# Fase 3: Add-in de Revit (C# net10.0-windows, netstandard2.0)

Fecha: 2026-10-01. Rama: `claude/laughing-pascal-tsxvkt`. Versión del add-in: 0.1.0.

---

## 1. Qué se hizo

- **Operaciones completas del Bridge por reflexión (`src/MotorConexiones.Revit/Operations/`)**:
  - `PingOperation.cs` (`ping`): informa versión del add-in, backend de fabricación activo (`advancesteel` o `directshape`), versión y build de Revit, ID de proceso y estado del documento abierto.
  - `GuideOperation.cs` (`guide`): expone el texto de la guía de especificaciones (`docs/guide.md` / recurso embebido) para instrucción directa de la IA.
  - `TypesOperation.cs` (`types`): enumera los tipos de conexión registrados (`gusset_node`) y su descripción funcional.
  - `SchemaOperation.cs` (`schema`): devuelve el esquema JSON Draft-07 y un ejemplo representativo completo para el tipo de conexión solicitado.
  - `NodeInfoOperation.cs` (`node_info`): inspecciona los miembros del nudo mediante la API de Revit, identifica el cordón, calcula el sistema local 3D (`NodeFrame`), verifica distancia entre ejes (límite 5 mm) y proyecta ángulos de barras en el plano de la cercha.
  - `FindProfileOperation.cs` (`find_profile`): busca tipos de perfil estructural cargados en el modelo coincidentes con la designación AISC o sugiere perfiles similares mediante distancia Levenshtein.
  - `ValidateOperation.cs` (`validate`): ejecuta la validación integral de la especificación contra el modelo activo (`RevitModelFacts`) y los límites AISC (`limits.json`). Si es válida y no tiene incertidumbres abiertas, emite el `validation_token` criptográfico determinista (SHA-256).
  - `PreviewOperation.cs` (`preview`): calcula y devuelve los elementos que se crearán (cartela, placas cuchilla, grupos de pernos, cordones de soldadura) y los recortes que se aplicarán a las barras sin modificar el modelo.
  - `CreateOperation.cs` (`create`): exige un `validation_token` válido, genera los elementos físicos en el modelo (vía Advance Steel o DirectShape), aplica los recortes `end_setback_mm` a las barras y persiste el registro en Extensible Storage dentro de un `OperationScope` atómico.
  - `ListOperation.cs` (`list`): lista todas las conexiones creadas presentes en el modelo leyendo Extensible Storage.
  - `GetOperation.cs` (`get`): recupera la especificación completa, elementos asociados y metadatos de una conexión por su `connection_id`.
  - `UpdateOperation.cs` (`update`): actualiza una conexión existente de forma atómica bajo el mismo `connection_id`, restaurando los miembros anteriores y generando la nueva geometría.
  - `DeleteOperation.cs` (`delete`): elimina los elementos físicos generados por el add-in, restaura los parámetros de extensión originales de las barras y elimina el registro de Extensible Storage sin tocar elementos ajenos a la conexión.
  - `Bridge.cs`: registro centralizado de las 13 operaciones con manejo robusto de excepciones y respuestas estructuradas bajo el contrato `ApiResponse`.

- **Implementación de hechos del modelo en Revit (`src/MotorConexiones.Revit/Node/RevitModelFacts.cs`)**:
  - Implementación concreta de la interfaz `IModelFacts`.
  - Inspección de `FamilyInstance` en la categoría `OST_StructuralFraming`.
  - Detección de dimensiones de perfil (`width_mm`, `height_mm`, `thickness_mm`) combinando parámetros de tipo de Revit y análisis de designaciones AISC mediante `LabelParser`.
  - Geometría de ejes 3D convertida reversiblemente a milímetros mediante `UnitConverter`.
  - Detección de choques e interferencias espaciales con miembros ajenos al nudo (`SegmentDistanceMm`).

- **Abstracción y backends de fabricación (`src/MotorConexiones.Revit/Fabrication/`)**:
  - `IFabricationBackend.cs`: interfaz común para creación de placas (`CreatePlate`), grupos de pernos (`CreateBoltGroup`) y cordones de soldadura (`CreateWelds`).
  - `AdvanceSteelBackend.cs` (Camino A): integración mediante reflexión en tiempo de ejecución con los ensamblados de Advance Steel en Revit (`ASObjectsMgd`, `ASGeometryMgd`, `Autodesk.SteelConnectionsDB`), creando objetos nativos de acero (`Plate`, `FilerObject`, `BoltGrid`, `Weld`) en unidades internas de Revit (pies), con fallback automático a DirectShape si los ensamblados no están presentes o fallan.
  - `DirectShapeBackend.cs` (Camino B): implementación geométrica paramétrica garantizada en cualquier entorno, modelando placas, pernos hexagonales y cordones de filete en DirectShape dentro de categorías estructurales estándar.
  - `BackendFactory.cs`: selección dinámica del backend disponible (prioriza Camino A y degrada limpiamente a Camino B).
  - `MemberModifier.cs`: recorta las barras aplicando `end_setback_mm` en los parámetros nativos de Revit (`START_EXTENSION` o `END_EXTENSION`) y almacena el estado previo en `ModifiedMemberRecord` para restauración exacta.
  - `ConnectionGeometry.cs` (en `Core.Geometry3D`): algoritmos geométricos 2D para contorno de cartela, cálculo de los 4 vértices de placa cuchilla, posicionamiento de pernos en 1 o varias columnas y cálculo de líneas de soldadura.

- **Persistencia en Extensible Storage (`src/MotorConexiones.Revit/Storage/` y `src/MotorConexiones.Core/Storage/`)**:
  - Esquema registrado `MotorConexionesConnection` con GUID fijo `8A6C4D2E-3F1B-4E5A-9C7D-2E4F6A8B0C1D` y VendorId `MCNX`.
  - Almacenamiento desacoplado en elementos invisibles `DataStorage`.
  - `ConnectionStorageManager.cs`: operaciones atómicas de creación, consulta, listado y eliminación de entidades en el modelo.
  - `ConnectionRecord.cs` y `ModifiedMemberRecord.cs`: modelos de datos fuertemente tipados con serialización JSON para trazabilidad y rollback completo.

- **Servicio central de creación (`src/MotorConexiones.Revit/Services/ConnectionCreationService.cs`)**:
  - Orquestador del flujo de creación y borrado.
  - Envoltura atómica estricta en `OperationScope` (`TransactionGroup` asimilado únicamente si la operación concluye con éxito; rollback inmediato en caso de error).
  - Asociación de comentarios en elementos creados con el `connection_id` para trazabilidad visual en Revit.

- **Comando Ribbon "Cargar Spec..." (`src/MotorConexiones.Revit/RunSpecCommand.cs`)**:
  - Implementación de `IExternalCommand` con atributos `TransactionMode.Manual` y `RegenerationOption.Manual`.
  - Selector nativo de archivos de Windows (invocado dinámicamente sin acoplamientos pesados de plataforma) para elegir archivos JSON de conexión.
  - Validación previa completa con `SpecValidator` y `RevitModelFacts`.
  - Cuadro de confirmación interactivo que presenta al usuario el resumen del nudo, tipo, backend y barras que serán recortadas.
  - Ejecución atómica y notificación del resultado con ID de la conexión creada.

- **Fixture de prueba y sondeo automatizado**:
  - `docs/fixtures/detalle-D-confirmado.json`: especificación del Detalle D con incertidumbres confirmadas (montante HSS2-1/2X2-1/2X3/16 e interfaz de cartela through_slot).
  - `scripts/sondeos/11-fase3-verificacion.py`: script de prueba IronPython para ejecutar las 12 operaciones del Bridge secuencialmente dentro de Revit.

- **Suite de pruebas unitarias xUnit (`src/MotorConexiones.Tests/`)**:
  - `ConnectionRecordTests.cs`: verificación de serialización/deserialización JSON de registros de conexión y miembros modificados.
  - `ConnectionGeometryTests.cs`: pruebas unitarias de cálculo de vértices de placas cuchilla, patrones de pernos, líneas de soldadura y vectores directores en el plano del nudo.
  - Total: **45 pruebas pasando (100% de éxito)** en Release mode.

---

## 2. Qué se probó en la sesión y cómo

### 2.1 Compilación de la solución completa

Se ejecutó la compilación en modo Release con `TreatWarningsAsErrors=true` en todos los proyectos:

```text
$ dotnet build MotorConexiones.sln -c Release
  MotorConexiones.Core -> bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> bin/Release/net10.0/MotorConexiones.Tests.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores
Tiempo transcurrido 00:00:01.71
```

> [!NOTE]
> La compilación produce 0 advertencias y 0 errores. Todos los tipos y miembros utilizados de la API de Revit 2027 existen y son compatibles con .NET 10.

### 2.2 Pruebas unitarias automatizadas (`dotnet test -c Release`)

```text
$ dotnet test -c Release --logger "console;verbosity=normal"
Serie de pruebas para MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
Total de pruebas: 45. Correctas: 45, Con error: 0, Omitidas: 0.
Duración total: 139 ms.
```

### 2.3 Matriz de componentes y cobertura de operaciones

| Componente | Archivo principal | Estado de compilación / prueba |
|---|---|---|
| **Bridge Central** | `src/MotorConexiones.Revit/Bridge.cs` | Compilado (0 w / 0 e) |
| **Operación ping** | `Operations/PingOperation.cs` | Compilado (0 w / 0 e) |
| **Operación guide** | `Operations/GuideOperation.cs` | Compilado (0 w / 0 e) |
| **Operación types** | `Operations/TypesOperation.cs` | Compilado (0 w / 0 e) |
| **Operación schema** | `Operations/SchemaOperation.cs` | Compilado (0 w / 0 e) |
| **Operación node_info** | `Operations/NodeInfoOperation.cs` | Compilado (0 w / 0 e) |
| **Operación find_profile**| `Operations/FindProfileOperation.cs` | Compilado (0 w / 0 e) |
| **Operación validate** | `Operations/ValidateOperation.cs` | Compilado (0 w / 0 e) |
| **Operación preview** | `Operations/PreviewOperation.cs` | Compilado (0 w / 0 e) |
| **Operación create** | `Operations/CreateOperation.cs` | Compilado (0 w / 0 e) |
| **Operación list** | `Operations/ListOperation.cs` | Compilado (0 w / 0 e) |
| **Operación get** | `Operations/GetOperation.cs` | Compilado (0 w / 0 e) |
| **Operación update** | `Operations/UpdateOperation.cs` | Compilado (0 w / 0 e) |
| **Operación delete** | `Operations/DeleteOperation.cs` | Compilado (0 w / 0 e) |
| **Hechos del modelo** | `Node/RevitModelFacts.cs` | Compilado (0 w / 0 e) |
| **Fabricación DirectShape**| `Fabrication/DirectShapeBackend.cs`| Compilado (0 w / 0 e) |
| **Fabricación Advance Steel**| `Fabrication/AdvanceSteelBackend.cs`| Compilado (0 w / 0 e) |
| **Recorte de barras** | `Fabrication/MemberModifier.cs` | Compilado (0 w / 0 e) |
| **Geometría de conexión** | `Core/Geometry3D/ConnectionGeometry.cs` | 5 pruebas unitarias xUnit pasadas |
| **Extensible Storage** | `Storage/ConnectionStorageManager.cs` | Compilado (0 w / 0 e) |
| **Registro de conexión** | `Core/Storage/ConnectionRecord.cs` | 2 pruebas unitarias xUnit pasadas |
| **Servicio de creación** | `Services/ConnectionCreationService.cs` | Compilado (0 w / 0 e) |
| **Botón Ribbon** | `RunSpecCommand.cs` | Compilado (0 w / 0 e) |

---

## 3. Decisiones técnicas tomadas

1. **Advance Steel por reflexión (Camino A)**:
   - Para permitir compilación sin dependencias binarias rígidas de Advance Steel en entornos de compilación cruzada, el backend `AdvanceSteelBackend` interactúa con `ASObjectsMgd` y `Autodesk.SteelConnectionsDB` dinámicamente por reflexión en tiempo de ejecución. Si Advance Steel no está presente o lanza una excepción, el sistema conmuta automáticamente al backend `DirectShapeBackend` (Camino B).
2. **Unidades de fabricación**:
   - En Revit interno, la creación de geometría en DirectShape y Advance Steel opera en pies (`feet`). La traducción desde los valores en milímetros del contrato pasa estrictamente a través de `UnitConverter.MmToFeet()`.
3. **Persistencia limpia y rollback**:
   - El recorte de barras se almacena en `ModifiedMemberRecord` registrando el nombre exacto del parámetro de extensión de Revit (`START_EXTENSION` o `END_EXTENSION`) y el valor numérico en pies original, lo que permite que `conn_delete` restaure la longitud original de las barras al 100%.
4. **Diálogo de archivos sin dependencias de GUI pesadas**:
   - `RunSpecCommand` abre el diálogo de archivos mediante reflexión sobre `System.Windows.Forms.OpenFileDialog` presente en el proceso anfitrión de Revit en Windows, preservando compatibilidad y evitando advertencias del SDK.

---

## 4. Qué quedó pendiente y para quién

- **PENDIENTE DE INSTALADOR (en PC con Revit)**:
  - Ejecutar los pasos de `docs/instalacion/fase-3.md`:
    - Despliegue con `scripts/deploy.ps1`.
    - Ejecución de `scripts/sondeos/11-fase3-verificacion.py` para verificar las 12 operaciones del Bridge dentro de Revit 2027.
    - Clic interactivo en el botón Ribbon "Cargar Spec..." con `docs/fixtures/detalle-D-confirmado.json` y captura de pantalla.
- **FASE 4 (Siguiente fase)**:
  - Implementación del servidor MCP (rutas HTTP en revit-mcp y herramientas `conn_*` de IA) para exponer estas capacidades al cliente Claude/Cursor.
