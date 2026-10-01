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

---

## 5. Revisión en la nube y correcciones antes de la instalación (2026-10-01)

El código anterior lo escribió otro agente en el PC. Revisado en la nube contra las lecciones de la Fase 1
(`docs/fases/fase-1.md` secciones 7 y 8). Compila (0 advertencias) y pasa 46 pruebas, pero tenía seis problemas de fondo
que habrían hecho que el camino A nunca se usara o que la geometría saliera mal. Corregidos en el commit de esta sección:

| Problema | Por qué importa | Corrección |
|---|---|---|
| Una `FabricationTransaction` por placa y otra por grupo de pernos, abierta con el constructor de 3 argumentos **dentro de la `Transaction` de Revit** ya abierta por `OperationScope`, y el objeto `Plate` construido **fuera** de ella | En la Fase 1 la transacción de acero tardó 49–132 s cada una y solo se probó sin transacción de Revit abierta; construir objetos AS fuera del contexto es lo que cerró Revit en la Fase 0. Con tres transacciones una conexión superaría los 180 s del MCP, y lo más probable es que el constructor de 3 argumentos fallara y todo cayera en silencio a DirectShape | `IFabricationBackend.BeginSession`: **una sola** `FabricationTransaction` por conexión (también al borrar), abierta con la sobrecarga `(Document, Boolean, String, Boolean bRevitTransactionAlreadyStarted = doc.IsModifiable)` del sondeo 06; placas y pernos se construyen y escriben dentro; `Complete()` = `Commit()`, si no, `CancelTransaction()`. El registro JSON anota qué constructor se usó (`fabrication_transaction_open`) |
| Patrón de pernos sin `Nx`, `Ny`, `Dx`, `Dy` y con las esquinas tomadas de la caja envolvente **en los ejes del nudo** | Para una placa cuchilla a 45° la caja no es el patrón; sin `Nx/Ny` el número de pernos queda al criterio de Advance Steel | `BoltGrid` (Core): esquinas = primer y último perno, ejes = a lo largo y a través del miembro, filas, columnas y paso reales; `CreateBoltPattern` fija `Nx, Ny, Dx, Dy, ScrewDiameter, ScrewLength` como en el sondeo 10 (`NumberOfScrews: 4`) |
| Retiro de extremo: `extensión = original − setback` | El contrato mide `end_setback_mm` desde el **punto de trabajo**; la extensión de Revit se mide desde el extremo de la línea de ubicación, que en el modelo real está a unos 86 mm del punto de trabajo. Con 180 mm de retiro la barra quedaba a 266 mm | `MemberModifier`: `extensión nueva = distancia(extremo de la línea, punto de trabajo) − setback`; `conn_preview` muestra `current_end_distance_mm` y `new_extension_mm`. El signo de la extensión de Revit se confirma con la captura del paso 9 |
| "Primer miembro" distinto en `validate`, `preview` y `create` (orden de un `HashSet`) | El sistema local del nudo (y por tanto la orientación de todo) cambiaba según la operación; el `validation_token` también | `NodeInspector.ResolveNode`: una sola regla para las cuatro rutas (cordón = `chord.element_id`; primer miembro = `members[0]`, como dice la sección 7 del encargo) |
| El botón de la cinta **rellenaba solo** las dudas (`profile = "HSS2-1-2X2-1-2X3-16 64x64"`, `chord_interface = "through_slot"`) | Viola el principio 6 del encargo: sin `user_confirmed_value` no hay creación; el add-in no inventa datos | El botón lista las dudas y se detiene; la carpeta inicial del diálogo ya no es una ruta fija de un PC; el diálogo de éxito muestra el backend que se usó de verdad (guardado ahora en el registro: campo `Backend`) |
| Sondeo 11 escrito sin ejecutarlo: leía `success`/`error` (el sobre tiene `ok`/`errors`), usaba `__file__` (no existe dentro de `exec`) y borraba la conexión antes de poder verla | No habría pasado de la primera línea | Sondeos nuevos `11-fase3-crear.py` (deja la conexión para la captura, imprime el sobre real y las categorías de lo creado), `12-fase3-borrar.py` (borra y comprueba las extensiones) y `13-limpiar-fase1.py` (quita la placa y los pernos sueltos de la Fase 1). `docs/instalacion/fase-3.md` reescrito con `Anota`, tiempos de espera de 900 s, capturas y el paso del botón |

Lo que sigue **PENDIENTE DE INSTALADOR** y no se puede saber desde la nube (pasos 6, 8, 9 y 10 de las instrucciones):
que la sobrecarga con `bRevitTransactionAlreadyStarted` funcione dentro de `TransactionGroup` + `Transaction`
(si no, el registro lo dirá y toda la conexión saldrá por DirectShape); el signo de Start/End Extension; que
`doc.Delete` borre `SteelProxyElement` dentro de la sesión de acero; y el tiempo total de `create`.

Limitaciones conocidas que quedan para la Fase 5 o v2: las soldaduras se representan con DirectShape (Advance Steel
tiene `WeldPattern`/`WeldLine` pero no se ha probado); la cartela se centra en el eje del cordón para todas las
`chord_interface` (`split_top_bottom` y `side_lap` se tratan como `through_slot`); el fixture confirmado llama "vertical"
a un miembro que en el modelo de prueba es una diagonal (solo produce la advertencia `ANGLE_DIFFERS_FROM_MODEL`).

---

## 6. Resultados de la primera ronda en el PC (2026-09-30, `docs/fases/resultados-fase-3.md`) y segunda corrección

**Lo que funciona de punta a punta** (sin ventanas, sin cierres de Revit): `ping`, `guide`, `types`, `schema`, `find_profile`
(`HSS2-1/2X2-1/2X3/16` ↔ `HSS2-1-2X2-1-2X3-16 64x64`), `node_info`, `validate` (token emitido; dos advertencias
`ANGLE_DIFFERS_FROM_MODEL` esperadas por el fixture), `preview`, `create` (12 elementos en 1,1 s), `list`, `get`, `delete`
(los 12 borrados y las tres barras con su extensión original: `-93,8 → 68,6`, `-42 → 0`, `-210,2 → 0` mm) y el botón de la
cinta con el mismo fixture. Capturas `fase3-01-conexion.png`, `fase3-02-borrado.png` y `fase3-03-boton.png`: cartela de
565 × 530 en el plano de la cercha y diagonales retiradas, con el hueco visible. **El signo de Start/End Extension es el
supuesto** (negativo acorta): confirmado por la captura y por los valores restaurados.

**Advance Steel, lo que se aprendió**: la `FabricationTransaction` con `bRevitTransactionAlreadyStarted = true` **se abre
y se confirma sin error dentro de `TransactionGroup` + `Transaction`** (`fabrication_transaction_open`, `is_modifiable_after:
true`, sin `fabrication_transaction_failed`), y `WriteToDb` de la placa y del patrón (`Nx=2, Ny=2, Dx=Dy=60 mm`) no lanzó.
Pero el código buscaba los `SteelProxyElement` **justo después de `WriteToDb`**, y no aparecieron (`element_ids: []`), así
que las tres piezas se crearon con DirectShape. En la Fase 1 el sondeo 09 medía los elementos **después de `Commit()`**:
lo más probable es que Advance Steel materialice sus elementos al confirmar. Corrección (este commit):

- `IFabricationSession.Complete()` devuelve los elementos que aparecen al confirmar; las escrituras quedan "pendientes" y,
  si tras el `Commit` no aparece ninguno, entonces (y solo entonces) se crean con DirectShape.
- `conn_create`, `conn_update` y el botón toman una foto de los ids del documento antes de la operación y, tras confirmar la
  `Transaction` de Revit, **adoptan en el registro** cualquier elemento nuevo que no esté en él (por si Advance Steel los
  materializa aún más tarde). Así `conn_delete` siempre borra todo lo que creó la operación. El registro JSON lo anota
  (`fabrication_transaction_commit`, con categorías) y la respuesta lo avisa.
- `conn_get` y `conn_list` devuelven `backend` (el sondeo lo imprimía como `None`).

**Cambios que hizo el instalador por su cuenta** (revisados; se conservan con ajustes): registro de `GussetNodeType` en
`Bridge` (sin él `schema` fallaba: correcto); `MEMBER_NOT_AT_NODE` medía contra el segmento y fallaba porque los extremos
de las barras están a 18–86 mm del punto de trabajo (correcto medir contra la recta; la tolerancia de 2000 mm al extremo se
baja a **500 mm**, constante `MaxEndDistanceFromNodeMm`); `dev_exec` no capturaba `SystemExit` (correcto; además ahora
una `PARADA` del sondeo devuelve `ok: true` con `stopped: true`). Sus ocho lanzadores `ejecutar-fase3-*.ps1` se eliminan;
sus tres scripts de captura se funden en `scripts/sondeos/capturar-nudo.py` (exporta la vista 3D del nudo a PNG desde
dentro de Revit: útil para todas las fases).

**PENDIENTE DE INSTALADOR** (`docs/instalacion/fase-3b.md`, una ronda corta): comprobar si con la corrección las placas y
los pernos salen como `SteelProxyElement` (Plates/Bolts) y cuánto tarda; si siguen sin aparecer tras el `Commit`, la
`FabricationTransaction` anidada en la de Revit no materializa elementos y habrá que crear la conexión con una
`FabricationTransaction` de 3 argumentos (sin `Transaction` de Revit abierta), lo que obliga a reordenar `OperationScope`.
En cualquiera de los dos casos la Fase 3 queda funcional con DirectShape, que es lo que ya probó el PC.
