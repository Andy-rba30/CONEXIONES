# Fase 6: ventana de previsualización 2D con cotas, borrado desde la cinta y botón en la pestaña ARBA

Fecha: 2026-10-01. Rama: `main`. Add-in 0.1.0 (Core con `Sketch/` y `Editing/`; add-in con `UI/` en WPF); adaptador y
herramientas del MCP sin cambios (`Bridge.cs`, `Operations/` y `mcp/` no se tocan: `git diff` lo confirma).

**Estado: escrita y probada en la nube (compila sin avisos, 83/83 pruebas); pendiente de probar en Revit.** Todo lo que
pasa dentro de Revit (la ventana, la cinta ARBA, crear y borrar desde los botones) está **NO PROBADO** hasta que vuelvan
los resultados de `docs/instalacion/fase-6.md` en `docs/fases/resultados-fase-6.md`.

---

## 1. Qué se hizo

- **Core, croquis 2D sin Revit (`src/MotorConexiones.Core/Sketch/`)**:
  - `SketchPrimitives.cs`: primitivas en mm en el sistema local del nudo: `SketchLine`, `SketchPolygon` (cerrado o
    abierto), `SketchCircle`, `SketchDimension` (dos puntos medidos, desplazamiento de la línea de cota, valor, texto,
    tipo `DimensionKind` y ruta JSON del campo), `SketchLabel`, `SketchBounds` y el contenedor `Sketch` con sus `Notes`.
    `SketchKind` dice qué es cada trazo (eje, borde del cordón, barra, cartela, placa, ranura, perno, soldadura, punto de
    trabajo, cota, etiqueta); la ventana elige color y grosor por ese valor.
  - `SketchText.cs`: mm con una cifra decimal y coma (`565,0`, `9,5`), grados, etiqueta de espesor (`PL 3/8" · 9,5 mm`)
    y texto de soldadura. No es una conversión de unidades: el croquis ya está en mm.
  - `SketchNodeInfo.cs`: lo que el croquis necesita del nudo (ancho del cordón, y por barra dirección en el plano, ancho
    de perfil, tipo y ángulo). `FromModelFacts(spec, IModelFacts)` lo calcula con la misma regla que validar y crear
    (`NodeFrame.Compute` con cordón + primer miembro y `ConnectionGeometry.GetMemberDirection2D`); si el nudo no se
    puede leer, `FromSpecAngles` reparte las barras por su `expected_angle_deg` y lo marca como aproximado.
  - `GussetNodeSketch.cs`: el croquis de `gusset_node`: eje y ancho del cordón, contorno de la cartela, eje y cuerpo de
    cada barra desde su extremo real (el retiro), ranura del HSS (ancho = espesor de la cartela) con sus dos soldaduras,
    placa cuchilla con los mismos vértices que se crean (`ComputeKnifePlateCorners`), pernos (`ComputeBoltPositions`),
    marca del punto de trabajo y leyenda del sistema local. Cotas: ancho y alto de la cartela (caja envolvente del
    contorno, con aviso si `width_mm`/`height_mm` dicen otra cosa), retiro de cada barra, largo de ranura, largo y ancho
    de la placa, paso, borde y primera fila de los pernos. Etiquetas: perfil del cordón, espesor de la cartela, unión al
    cordón y su soldadura, rol · id · perfil · ángulo de cada barra, soldaduras.
  - `ISketchProvider.cs` + `SketchBuilder.cs`: `SketchBuilder.Build(spec, nodeInfo)` busca el tipo en
    `ConnectionTypeRegistry` y, si implementa `ISketchProvider`, le pide el croquis; `GussetNodeType` lo implementa. Un
    tipo nuevo aporta su croquis sin tocar el núcleo; uno sin croquis devuelve un `Sketch` vacío con una nota, sin lanzar.
- **Core, editor por ruta JSON (`src/MotorConexiones.Core/Editing/SpecEditor.cs`)**: `ListFields(spec)` genera las filas
  de la tabla del paso 8 (cordón, cartela, cada barra con ranura o placa cuchilla y pernos, cadenas de cotas y dudas) con
  etiqueta en español, ruta JSON y valor como texto; `TrySetValue(json, ruta, texto)` escribe el valor **en el JSON**
  (no en el objeto) conservando el tipo que había (número, entero, booleano, texto, lista) y creando los objetos
  intermedios que falten; `TrySetOutline` edita `gusset.outline.points_mm` desde texto (`x; y` por línea);
  `TrySetElementIds` rellena `node.element_ids` con la selección de Revit; `ToPrettyJson` da el JSON con sangría para
  guardar. Trabajar sobre el JSON hace que el texto que se dibuja, el que se valida, el que firma el token y el que se
  guarda sean el mismo.
- **Add-in, ventana de previsualización (`src/MotorConexiones.Revit/UI/`, WPF)**:
  - `SketchCanvas.cs`: control que dibuja el `Sketch` con zoom por rueda, encuadre con el botón central (o arrastrando)
    y **Ajustar**; cotas con líneas de referencia, marcas y texto a lo largo de la línea; la fila seleccionada en la
    tabla se resalta en naranja. El factor mm → píxel es del control, no una conversión de unidades.
  - `PreviewSession.cs`: estado de la ventana sin WPF: JSON actual, especificación, validación (la misma de
    `conn_validate`: `NodeInspector.ResolveNode` + `RevitModelFacts` + `SpecValidator` + `config\limits.json`, y además
    los fallos del nudo como `NODE_AXES_NOT_INTERSECTING` entran como errores), croquis, filas, `Reload`, `Save`
    (sufijo `-corregido.json`, nunca encima del original) y resumen.
  - `PreviewWindow.xaml(.cs)`: tres zonas. Izquierda, croquis. Derecha, tabla (`DataGrid`) editable con doble clic y
    Enter, más el cuadro del contorno con **Aplicar contorno**. Abajo, errores y avisos con código, campo, mensaje y
    sugerencia, el token abreviado y los botones **Recargar**, **Guardar JSON**, **Validar**, **Crear** (solo activo en
    verde) y **Cancelar**. Cada cambio redibuja y revalida; un cambio rechazado se deshace y se explica.
- **Add-in, borrar desde la cinta**: `ListConnectionsCommand.cs` + `UI/ConnectionsWindow.xaml(.cs)`: lista las
  conexiones de Extensible Storage (lo mismo que `conn_list`) y borra la seleccionada con confirmación en un
  `TaskDialog`, usando `ConnectionCreationService.DeleteConnection` dentro de un `OperationScope` (lo mismo que
  `conn_delete`: un `TransactionGroup`, rollback si falla, barras restauradas). Registro `ribbon_delete`.
- **Add-in, botón en ARBA (`App.cs`)**: el panel `MotorConexiones` va a la pestaña **ARBA** con
  `CreateRibbonTab("ARBA")` tolerante (si ya existe, se reutiliza) + `CreateRibbonPanel("ARBA", "MotorConexiones")`, con
  reserva a `Conexiones` si falla, y el resultado en el log (`ribbon_panel_created` con `tab`, `tab_already_existed` y,
  si hubo reserva, `preferred_tab_error`). Dos botones: **Ejecutar especificación JSON** y **Conexiones del modelo**.
- **`RunSpecCommand.cs`**: abre la ventana en vez del resumen de texto; si la persona pulsa Crear, crea exactamente igual
  que antes (`ConnectionCreationService` con el token recién calculado, `OperationScope`, adopción de elementos de
  Advance Steel, registro, diálogo final). Las dudas sin confirmar ya no paran con un diálogo: se ven como errores
  `UNRESOLVED_UNCERTAINTY` y se rellenan en la tabla. El diálogo de archivo pasa a `Microsoft.Win32.OpenFileDialog`
  (WPF) en lugar de la reflexión sobre Windows Forms.
- **`MotorConexiones.Revit.csproj`**: `<UseWPF>true</UseWPF>`. Comprobado antes con un proyecto de prueba: una `Window`
  con XAML compila en Linux con `EnableWindowsTargeting` (0 avisos), así que no hizo falta Windows Forms ni cargar el
  XAML en tiempo de ejecución.
- **Sondeo `scripts/sondeos/15-cinta-arba.py`**: lista las pestañas y paneles de la cinta con
  `Autodesk.Windows.ComponentManager.Ribbon` (id, título, visible, botones) y dice en qué pestaña quedó el panel
  MotorConexiones. Sirve para verificar la decisión de ARBA en el PC.
- **Pruebas** (`SketchBuilderTests.cs`, 17; `SpecEditorTests.cs`, 16): de 51 a **84**.
- **Corrección durante la ronda del instalador (paso 6-5)**: tras escribir `9` en el espesor, `12,7` se rechazaba como
  "debe ser un número entero": el editor deducía el tipo entero de la forma del valor anterior. Ahora solo `rows`,
  `columns`, `element_id` y `element_ids` son enteros; prueba `IntegerLookingValue_DoesNotTurnTheFieldIntoAnInteger`.
- **Documentación**: este informe, `docs/instalacion/fase-6.md`, README (estado, árbol, pasos 1 y 4, secciones nuevas
  "9. Previsualizar y corregir antes de crear" y "10. Borrar desde la cinta", croquis opcional al agregar un tipo) y el
  mensaje final de `scripts/deploy.ps1`. La tabla de garantías del README no cambia hasta que haya resultados.

---

## 2. Qué se probó en la nube y cómo

### 2.1 WPF compila en Linux (decisión previa)

Proyecto de prueba `net10.0-windows` + `EnableWindowsTargeting` + `UseWPF` con una `Window` en XAML, un `DataGrid`, un
`FrameworkElement` con `OnRender`, `FormattedText`, `WindowInteropHelper` y `UIApplication.MainWindowHandle`:

```text
$ dotnet build -c Release --nologo
  WpfTest -> .../bin/Release/net10.0-windows/WpfTest.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### 2.2 Compilación de la solución

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

Con `TreatWarningsAsErrors` en los tres proyectos. Todos los miembros usados de la API de Revit 2027 y de WPF
compilan: `UIControlledApplication.CreateRibbonTab/CreateRibbonPanel`, `Autodesk.Revit.Exceptions.ApplicationException`,
`UIApplication.MainWindowHandle`, `TaskDialog` con `DefaultButton`, `Microsoft.Win32.OpenFileDialog`,
`WindowInteropHelper`, `DataGrid`, `FormattedText`, `VisualTreeHelper.GetDpi`.

### 2.3 Pruebas (`dotnet test`), tres ejecuciones seguidas

```text
$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 277 ms - MotorConexiones.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 245 ms - MotorConexiones.Tests.dll (net10.0)
Passed!  - Failed:     0, Passed:    83, Skipped:     0, Total:    83, Duration: 223 ms - MotorConexiones.Tests.dll (net10.0)
```

Lo que comprueban las 32 pruebas nuevas, con `docs/fixtures/detalle-D-confirmado.json` y los hechos del nudo real del
Hangar (`FakeModelFacts`, coordenadas de `resultados-fase-1.md`):

| Prueba | Qué comprueba |
|---|---|
| `NodeInfo_FromModelFacts_UsesRealDirectionsAndWidths` | Direcciones unitarias y anchos (76,2 / 63,5 mm) del modelo; montante perpendicular (Ux = 0); ángulos 45°, 90° y 135° (ver 4.4); diagonales superiores a un lado del cordón y la inferior al otro |
| `DetalleD_GussetDimensionsAre565By530` | Cotas `565,0` y `530,0`, rutas `gusset.width_mm`/`height_mm`, cartela de 8 vértices |
| `DetalleD_SetbacksAre180_60_260` | Cotas de retiro 180, 60 y 260 del punto de trabajo al extremo real; el cuerpo de cada barra empieza ahí y mide 63,5 de ancho |
| `DetalleD_SlotsKnifePlateAndBolts` | Ranuras 150 y 150; placa 170 × 140; paso 60, borde 40, primera fila 40; 4 pernos de radio 7,94 dentro de la placa; 6 soldaduras |
| `DetalleD_LabelsAndPieces` | Etiqueta `3/8"` · `9,5 mm`, perfil del cordón, `through_slot`, soldadura 5,0; 2 bordes de cordón a ±38,1; 4 ejes; 12 cotas; sin notas |
| `ChangingThickness_ChangesTheLabel` | 12,7 + `1/2"` → `cartela PL 1/2" · 12,7 mm` y ranura de 12,7 |
| `WidthMismatch_IsShownInTheDimensionText` | `width_mm` 560 con contorno de 565 → `565,0 (width_mm = 560,0)` |
| `WithoutModel_SketchIsApproximateButComplete` | Sin modelo: notas, retiros, pernos y direcciones por ángulo |
| `UnknownType_ReturnsEmptySketchWithNote`, `GussetNodeType_IsTheSketchProvider`, `MissingOutline_DrawsRectangleAndNotes` | Despacho por tipo, sin excepciones, rectángulo de reserva |
| `SketchText_*`, `DimensionLine_IsOffsetAlongTheLeftNormal` | Formato `565,0`, `9,5`, etiquetas; línea de cota desplazada por la normal izquierda |
| `ListFields_CoversStep8Table` | Filas y valores de la tabla (espesor, perfil, paso, filas, ranura, cadena `75; 420; 70`, dudas confirmadas) |
| `SetThickness_WithoutLabel_GivesLabelMismatch_AndWithLabel_IsValidWithNewToken` | 9,525 → 12,7 da `LABEL_VALUE_MISMATCH`; con `1/2"` vuelve a verde y el token (64 hex) es otro |
| `SetBoltSpacing10_GivesBoltSpacingTooSmall` | Paso 10 → `BOLT_SPACING_TOO_SMALL` en `members[2].attachment.bolts.spacing_mm`, sin token |
| `SetDimensionChain_402_GivesDimensionChainMismatch`, `SetUncertainValue_ToEmpty_BlocksTheToken_AndBackAgain` | Cadena 75; 402; 70 → `DIMENSION_CHAIN_MISMATCH`; duda vacía → `UNRESOLVED_UNCERTAINTY` y vuelta |
| `InvalidTexts_AreRejectedWithoutChangingTheJson`, `IntegersAndBooleans...`, `MissingIntermediateObjects_AreCreated` | Texto no numérico, entero con decimales, índice inexistente, ruta mal escrita; `rows` sigue entero y `continuous` booleano; el esquema sigue aceptando el JSON editado |
| `Outline_RoundTripsThroughText`, `PrettyJson_KeepsTheSameTokenAsTheOriginal`, `SetElementIds_...`, `TryParseNumber_AcceptsCommaAndDot` | Contorno ida y vuelta; el JSON con sangría firma el mismo token; selección → `node.element_ids`; coma y punto |

### 2.4 Lo que no cambia

- `git status`: `Bridge.cs`, `Operations/` y `mcp/` sin modificar. Las rutas `conn_*` siguen sin ventanas.
- `python3 -m py_compile` de `mcp/` y `scripts/sondeos/*.py` (incluido el 15): correcto.
- `mcp/pruebas/simulador_revit.py --autocomprobar`: `Autocomprobación: 27/27 correctas` (no depende del C#; se ejecuta
  solo para confirmar que nada del MCP se rompió). Las 19/19 por el puente real se repiten en el PC (paso 6-9).

### 2.5 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-6.md`)

| Qué | Paso |
|---|---|
| El panel MotorConexiones aparece en la pestaña ARBA (o en Conexiones, con el motivo en el log) | 6-3 |
| La ventana se abre, dibuja el Detalle D con sus cotas y valida en verde con los dos avisos de ángulo | 6-4 |
| Las cotas coinciden con el plano (lo compara la persona) | 6-4 |
| Zoom con la rueda, encuadre con el botón central, Ajustar | 6-4 |
| Cambiar el espesor redibuja la etiqueta, cambia el token y la validación reacciona (`LABEL_VALUE_MISMATCH` hasta cambiar el rótulo) | 6-5 |
| Guardar JSON crea `-corregido.json` y no toca el original | 6-5 |
| Paso de pernos 10 → error con código y campo, Crear desactivado; Recargar vuelve al original | 6-6 |
| Crear desde la ventana: 9 elementos, `connection_id`, `conn_list` = 1, `ribbon_create` en el log con el prefijo del token | 6-7 |
| Conexiones del modelo: lista 1 y borra; sondeo 12 = 0 con extensiones originales; sondeo 13 = 0 restos | 6-8 |
| `probar_conexiones.py --puente` 19/19 | 6-9 |

### 2.6 NO PROBADO en la nube y por qué

- **Todo lo de WPF dentro de Revit**: que la ventana se muestre modal sobre Revit con `MainWindowHandle`, que el
  `DataGrid` edite con doble clic + Enter, que `FormattedText` y `GetDpi` se comporten con la escala de pantalla del PC,
  los colores y grosores del croquis, y que `Microsoft.Win32.OpenFileDialog` abra dentro de Revit. La nube compila pero
  no tiene escritorio.
- **La pestaña ARBA**: `CreateRibbonPanel("ARBA", …)` sobre una pestaña creada por otro add-in se compila, no se
  ejecuta. El dato real llega con el sondeo 15 y el log `ribbon_panel_created`.
- **Hilo y transacciones**: crear y borrar desde el botón ocurren dentro del comando externo, con la ventana ya cerrada
  (crear) o abierta pero modal (borrar), sin `ExternalEvent`. Es el mismo contexto de API que el botón de la Fase 3, pero
  con una ventana WPF por delante: lo confirma el paso 6-7 y 6-8.
- **`Autodesk.Windows` en IronPython** (sondeo 15): `clr.AddReference("AdWindows")` y `ComponentManager.Ribbon.Tabs`
  son lo que usa el propio pyRevit, pero el sondeo no se ha ejecutado.

---

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. **Captura `fase6-01-cinta.png`**: la pestaña ARBA con el panel MotorConexiones y dos botones, junto a los paneles de
   tus otros add-ins (Columnas, Vigas, Cimientos, Encofrado, Muros, Georeferenciación) y el de pyRevit (IniciarIA). Si el
   panel salió en "Conexiones", el `preferred_tab_error` del log dice por qué.
2. **Captura `fase6-02-ventana.png`**: el croquis del Detalle D. Comprueba con el plano: cartela 565 × 530 (octógono con
   las esquinas superiores recortadas y el borde inferior derecho inclinado), tres barras con sus retiros 180 / 60 / 260,
   ranuras de 150 en las dos superiores, placa cuchilla de 170 × 140 con 4 pernos (60 de paso, 40 de borde, 40 a la
   primera fila) en la diagonal inferior, soldaduras en rojo. El dibujo está en el sistema local del nudo: la leyenda de
   abajo dice hacia dónde apunta +Y en el modelo (en el Hangar, +Y local = global (0, 0, −1), es decir, hacia abajo: el
   croquis puede salir invertido respecto al plano). Si eso te molesta, pídelo y añado un botón "Voltear Y".
3. **Captura `fase6-03-espesor-12-7.png`**: etiqueta `cartela PL 1/2" · 12,7 mm`, estado en verde y token distinto. Y la
   anotación intermedia: con solo 12,7 y el rótulo `3/8"`, `LABEL_VALUE_MISMATCH`. Es la regla 8.4 del encargo
   funcionando desde la ventana.
4. **Captura `fase6-04-error-paso.png`**: `BOLT_SPACING_TOO_SMALL` con el campo exacto y Crear en gris.
5. **Capturas `fase6-05-nudo.png` y `fase6-07-nudo-exportada.png`**: el nudo creado desde la ventana, igual que el de la
   Fase 5 (`fase5-03-placas-pantalla.png`).
6. **Captura `fase6-06-conexiones-modelo.png`** y la salida del sondeo 12: `conexiones en el modelo: 0` tras borrar desde
   la ventana, y las extensiones `68.64`, `69.2` y `0.0` de siempre.
7. Los 16 caracteres del token anotados en 6-4, 6-5 y 6-6: el de 6-4 y el de 6-6 (tras Recargar) deben ser iguales; el
   de 6-5 distinto; y el `validation_token_prefix` del log `ribbon_create` igual al de 6-4.

---

## 4. Decisiones tomadas y por qué

### 4.1 ARBA por la API de Revit, sin esperar a `ApplicationInitialized`

La respuesta del instalador (pegada en el prompt) dice que ARBA es un **híbrido**: una extensión de pyRevit
(`C:\IA\pyrevit-ext\IA-Tools.extension\ARBA.tab`, panel `IA.panel`, botón `IniciarIA.pushbutton`) **y** seis add-ins de
C# (`ColumnRebar`, `BeamRebar`, `StripFootingRebar`, `RetainingWallFormwork`, `RetainingWallRebar`, `RotarNorte`) cuyas
clases `RibbonApp` crean y añaden paneles a la misma pestaña `"ARBA"` por la API de Revit. Con ese dato, el camino es el
primero de la sección 3.3 del prompt: `CreateRibbonTab("ARBA")` tolerante + `CreateRibbonPanel("ARBA", "MotorConexiones")`.
El orden de carga no importa: la pestaña la crea el primer add-in que arranca y los demás reciben `ArgumentException`
("ya existe") y la reutilizan, que es exactamente lo que ya hacen los seis add-ins de la persona entre sí. Por eso no
hace falta esperar a `ApplicationInitialized` ni crear nada en diferido. Queda la reserva a `Conexiones` y el log por si
en el PC pasa algo distinto (por ejemplo, que pyRevit haya creado una pestaña ARBA que la API no reconozca): el sondeo 15
lo dirá. No se crea la carpeta `ribbon-arba/` (solo tendría sentido si ARBA fuera únicamente de pyRevit).

### 4.2 WPF con XAML, no Windows Forms

El prompt pedía comprobar primero que `UseWPF` compila en la nube. Compila (sección 2.1), así que la ventana es WPF con
XAML, que es lo que Revit usa internamente. El croquis se dibuja en un `FrameworkElement` propio con `OnRender`
(sin `Canvas` de WPF ni miles de objetos visuales): es rápido al redibujar con cada edición y el zoom no engorda las
líneas.

### 4.3 El JSON es la fuente de verdad de la ventana

La tabla no edita el objeto `ConnectionSpec` sino el texto JSON (`SpecEditor` con `System.Text.Json.Nodes`). Así el texto
que se valida, el que firma el `validation_token` (sección 5.4 del encargo) y el que se guarda son el mismo, los campos
que el contrato no conoce se conservan y el tipo de cada valor no cambia (`rows` sigue entero, `continuous` booleano),
con lo que el esquema (`additionalProperties: false`) sigue aceptándolo. Las rutas de la tabla son las mismas que usan
los errores del validador, así que un error en `members[2].attachment.bolts.spacing_mm` se corresponde con una fila.

### 4.4 Ángulo de la diagonal inferior: 135°

El croquis y la tabla muestran el ángulo de cada barra con la misma regla que `conn_get_node_info` y `RevitModelFacts`:
medido desde +X local (sentido inicio → fin del cordón) con `Atan2(|uy|, ux)`. La diagonal inferior del Hangar apunta
hacia −X y sale **135°**, no 45°: por eso el validador avisa `ANGLE_DIFFERS_FROM_MODEL` frente a los 45° del fixture
desde la Fase 5. No lo he cambiado (sería tocar el contrato y el validador); lo anoto como pregunta en la sección 5.

### 4.5 Cambiar el espesor no deja la validación en verde por sí solo

El paso 4 del prompt esperaba que al pasar de 9,525 a 12,7 mm "la validación siga en verde". No puede: el rótulo `3/8"`
deja de coincidir con 12,7 mm y la regla 8.4 marca `LABEL_VALUE_MISMATCH` (es lo que la prueba
`SetThickness_WithoutLabel_GivesLabelMismatch_AndWithLabel_IsValidWithNewToken` fija). Las instrucciones del instalador
piden cambiar también el rótulo a `1/2"`, con lo que vuelve el verde y el token cambia. Lo considero correcto: la
ventana aplica las mismas reglas que la IA.

### 4.6 Crear después de cerrar la ventana; borrar con la ventana abierta

**Crear** cierra la ventana y crea en el comando (como antes), con el token que la ventana acaba de recalcular. Así la
ventana no toca el modelo y la operación atómica no tiene una ventana propia por delante mientras corre. **Borrar** se
hace desde la ventana de conexiones con un `TaskDialog` de confirmación, dentro del mismo `OperationScope` que usa
`conn_delete`; la ventana es modal pero el código corre en el hilo del comando, como cualquier diálogo de un add-in.

### 4.7 Dudas sin confirmar ya no paran en un diálogo

Antes el botón se detenía con "Dudas sin resolver". Ahora la ventana abre igual, muestra `UNRESOLVED_UNCERTAINTY` por
cada duda y la persona rellena `user_confirmed_value` en la sección **Dudas** de la tabla. Es lo que pedía la sección
3.1 del prompt.

### 4.8 Cotas de la cartela: lo que se mide, no lo que se declara

`565,0` y `530,0` se miden sobre la caja envolvente del contorno (es lo que Advance Steel reporta como Length/Width,
ronda 5b). Si `width_mm`/`height_mm` difieren más de 0,5 mm, la cota lo dice: `565,0 (width_mm = 560,0)`. Así una
incoherencia entre contorno y medidas declaradas se ve en el dibujo.

---

## 5. Pendientes, riesgos y preguntas

- **P1 (riesgo principal)**: la ventana dentro de Revit. Si al abrirla aparece un error de ensamblado
  (`PresentationFramework` o `System.Xaml`), el log tendrá `ribbon_preview_failed` con el detalle; Revit 2027 trae WPF,
  así que no lo espero, pero es lo primero que miraré en los resultados.
- **P2**: la pestaña ARBA. Si pyRevit crea su pestaña ARBA antes que los add-ins de C# y la API no la reconoce, el
  panel caerá en `Conexiones` con el motivo en el log y el sondeo 15 mostrará los ids de cada pestaña. En ese caso, el
  camino B del prompt (`ribbon-arba/` dentro de la extensión de pyRevit) se haría en una ronda corta.
- **P3**: el ángulo de 135° (4.4). ¿Quieres que el contrato acepte el ángulo "del plano" sin signo (45° = 135°) para que
  desaparezca el aviso `ANGLE_DIFFERS_FROM_MODEL` del Detalle D? Es un cambio pequeño en `SpecValidator` con su prueba.
- **P4**: el croquis se dibuja en el sistema local (sección 7 del encargo), que en el Hangar tiene +Y hacia abajo. Si
  prefieres verlo como en el plano, se añade un botón "Voltear Y" en la ventana (solo cambia el dibujo, no el contrato).
- **P5**: la ventana edita vértices del contorno en una lista de texto, no arrastrándolos (así lo pedía el prompt). Si lo
  usas mucho, se puede añadir un modo de arrastre en una fase posterior.
- **P6**: `Guardar JSON` escribe `X-corregido.json`; si el archivo abierto ya es `X-corregido.json`, escribe
  `X-corregido-<fecha>.json` para no pisar lo abierto. Si prefieres otro criterio, es una línea.
- **Pendientes anteriores que siguen**: P2 de la Fase 5 (`UNKNOWN_CONNECTION_TYPE` para `conn_get_schema`), P3 de la
  Fase 5 (Claude Desktop NO PROBADO), soldaduras nativas y `BoltPattern.Connect` para v2, traspaso de `mcp/` a `revit-mcp`.

---

## 6. Ronda 6b (2026-10-01): decimales, cotas editables con doble clic y pernos con agarre real

Los resultados de la Fase 6 (`resultados-fase-6.md`, capturas `fase6-01` a `fase6-07`) dieron todo lo previsto: panel
en ARBA, ventana con cotas iguales al plano, validación reactiva, Guardar JSON, Crear (9 elementos) y borrado desde la
cinta con las barras restauradas. La persona pidió tres correcciones; esta ronda las hace. **Estado: escrita y probada en
la nube (compila sin avisos, 99/99 pruebas); pendiente de probar en Revit con `docs/instalacion/fase-6b.md`.**

### 6.1 Qué se hizo

1. **Decimales después de un entero (tabla de la ventana).** El PC lo reprodujo en el paso 6-5 (el log tiene nueve
   `ribbon_preview_edit_rejected` con `'thickness_mm' debe ser un número entero` tras escribir `9`). La corrección ya
   estaba en `main` desde el commit `e9e5c86` (el editor decide qué campos son enteros por el contrato, `rows`,
   `columns`, `element_id`, `element_ids`, y no por la forma del valor anterior; prueba
   `IntegerLookingValue_DoesNotTurnTheFieldIntoAnInteger`), pero el instalador compiló antes de ese commit (su build dio
   **83** pruebas, la cuenta anterior a la corrección; con ella son 84 y con esta ronda 99). No hay código nuevo para
   esto: el paso 6b-5 lo verifica en el PC tras el `git pull`.
2. **Doble clic en una cota del croquis para cambiar su valor.**
   - `UI/SketchCanvas.cs`: al dibujar cada cota guarda dónde quedó su texto y su línea (píxeles); `HitTestDimension`
     dice qué cota hay bajo el ratón (texto girado o línea, 7 px de tolerancia); el cursor pasa a mano encima de una
     cota; el doble clic izquierdo sobre una cota lanza el evento `DimensionActivated` (y no inicia el encuadre).
   - `UI/PreviewWindow.xaml(.cs)`: un cuadro naranja (`DimensionEditor`) aparece junto a la cota con el nombre del
     campo, su ruta JSON y el valor actual seleccionado; Enter aplica, Esc o clic fuera cancelan. Para las cotas con
     campo propio (retiro, ranura, largo y ancho de placa, paso, borde, primera fila) escribe en el JSON por la misma
     ruta que la tabla (`SpecEditor.TrySetValue`), redibuja, revalida, selecciona la fila y lo dice en la barra de
     estado. Para el **ancho y el alto de la cartela**, que se miden sobre el contorno, usa
     `SpecEditor.TrySetGussetSize` (nuevo en Core): escribe `width_mm`/`height_mm` y estira el contorno en ese eje
     alrededor del punto de trabajo (x = 0 o y = 0) para que su caja envolvente mida el valor nuevo, con las
     coordenadas redondeadas a 0,01 mm; la barra de estado lo explica y remite al cuadro del contorno. Registro
     `ribbon_preview_dimension_edit` / `ribbon_preview_dimension_rejected`.
   - Pruebas: `SetGussetSize_StretchesTheOutlineAboutTheWorkPoint` (565 → 575: −250 → −254,42 y 315 → 320,58; alto
     530 → 500; el esquema sigue aceptando y hay token; 0 se rechaza sin tocar el JSON).
3. **Pernos que no se ajustaban al espesor de las placas.** Causa, en `ConnectionCreationService` y
   `AdvanceSteelBackend` de la Fase 3: la placa cuchilla se creaba **en el mismo plano que la cartela** (ocupando su
   mismo volumen) y los pernos con `ScrewLength = 45 mm` fijos y sin agarre (Advance Steel ponía `Grip Length 80 mm` por
   su cuenta, `resultados-fase-5.md`), de ahí los vástagos largos de `fase6-05-nudo.png`. Ahora:
   - `Core/Geometry3D/BoltStack.cs` (nuevo): el paquete que atraviesan los pernos. La placa cuchilla apoya sobre una
     cara de la cartela (`plate.gusset_face`: `+z` por defecto o `-z`), así que su plano medio queda a
     ±(t_cartela + t_placa)/2 y el **agarre es t_cartela + t_placa** (Detalle D: 9,525 + 10 = 19,525 mm). La longitud
     del perno sale de `bolts.length_mm` si el plano la trae; si no, de `config/limits.json`: agarre + suplemento por
     diámetro (`bolts.length_addition_mm`, RCSC tabla C-2.1: Ø5/8" + 22,23 mm) redondeado hacia arriba a múltiplos de
     `bolts.length_increment_mm` (1/4") → **44,45 mm (1 3/4")**. Las dos claves nuevas entran en el hash de los límites
     (y por tanto en el token); un `limits.json` antiguo sigue funcionando con los valores por defecto.
   - Contrato: `plate.gusset_face` (`"+z"` | `"-z"`, opcional) y `bolts.length_mm` (opcional) en `KnifePlateSpec`,
     `BoltPatternSpec`, el esquema (`JsonSchemaValidator`, con comprobación del enumerado) y la tabla de la ventana
     (filas "Placa: cara de la cartela" y "Pernos: longitud (mm)"). Advertencia nueva `BOLT_LENGTH_TOO_SHORT` si
     `length_mm` < agarre + suplemento. Sin estos campos, el fixture del Detalle D no cambia.
   - Creación: `IFabricationBackend.CreatePlate` recibe el desplazamiento en Z del plano medio (0 para la cartela,
     `stack.PlateOffsetMm` = 9,76 mm para la cuchilla) y `CreateBoltPattern` recibe el `BoltStack`. En Advance Steel el
     plano del patrón se pone en la cara inferior del paquete (`StackMinMm` = −4,76 mm) con la normal +Z, y se fijan
     `BindingLength` (agarre) y `ScrewLength` (longitud), propiedades que existen en el volcado de la Fase 1. En la
     reserva `DirectShape`, cada perno es cabeza + vástago + tuerca desde esa cara. El registro
     `advance_steel_bolts_written` anota `grip_mm`, `bolt_length_mm`, `plane_z_mm` y `gusset_face`;
     `advance_steel_plate_written`, `offset_mm`.
   - `conn_validate` devuelve `data.bolt_stacks[]` (`grip_mm`, `bolt_length_mm`, `length_source`, `gusset_face`) y
     `conn_preview` añade `gusset_face`, `offset_from_gusset_plane_mm`, `grip_mm`, `length_mm` y `length_source` a la
     placa cuchilla y al grupo de pernos. El croquis muestra la etiqueta
     `4 pernos Ø5/8" · agarre 19,5 mm (cartela 9,5 + placa 10,0) · L 44,5 mm · placa en cara +z` (ruta
     `members[2].attachment.bolts.length_mm`; con `length_mm` del plano añade "(del plano)").
   - Sondeo `scripts/sondeos/16-pernos-agarre.py` (nuevo): con la conexión creada, proyecta la geometría de cada
     elemento sobre el eje Z local y escribe `[z_min, z_max]`; lee `Bolt Length` y `Grip Length`; da el veredicto
     (cartela centrada, placa apoyada en la cara, pernos cubriendo el paquete) y exporta una captura de perfil. El
     sondeo 11 actualiza lo esperado (44,45 y 19,53 mm).
   - Pruebas: `BoltStackTests.cs` (8) y `DetalleD_BoltLabelShowsGripAndLength`: de 84 a **99**.
4. **Documentación**: `docs/instalacion/fase-6b.md`, README (garantías, `limits.json`, sección 9, cuentas), `docs/guide.md`
   (cara de la placa y longitud del perno, `BOLT_LENGTH_TOO_SHORT`), `mcp/CONTRATO-conn.md` y los docstrings de
   `conn_get_schema`, `conn_validate` y `conn_preview` en `mcp/tools/conn_tools.py`.

### 6.2 Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` como en la Fase 0 (`dot.net` sigue bloqueado por el proxy) y NuGet
sirvió los paquetes de la API 2027.

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:    99, Skipped:     0, Total:    99, Duration: 328 ms - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile scripts/sondeos/16-pernos-agarre.py   # correcto
```

| Prueba nueva | Qué comprueba |
|---|---|
| `DetalleD_GripIsGussetPlusPlate_AndLengthComesFromTheTable` | Agarre 19,525; cara +z; plano medio de la placa a 9,7625; paquete −4,7625 .. 14,7625; longitud 44,45 (no 45 fijos) |
| `NegativeFace_PutsThePlateOnTheOtherSide`, `ParseSide_ReadsTheContractValues` | `-z` invierte los desplazamientos; `+z`, vacío, ` -Z `, texto raro |
| `LengthFromTheDrawing_IsUsedAsIs`, `ShortBoltLength_IsAWarning_NotAnError` | `length_mm` 50,8 se respeta; 30 da la advertencia `BOLT_LENGTH_TOO_SHORT` con token; sin `length_mm` no avisa |
| `Limits_BoltLength_RoundsUpToQuarterInch_AndFallsBackToFactor`, `RepoLimitsJson_CarriesTheLengthTable` | 19,525 → 44,45; 25 → 50,8; M20 → 1,4·d; el hash cambia con la tabla; `limits.json` antiguo sigue valiendo; el del repositorio trae la tabla |
| `GussetFace_IsAcceptedByTheSchema_AndOtherTextIsRejected` | `-z` pasa el esquema y da token; `lado` → `SCHEMA_INVALID` en `members[2].attachment.plate.gusset_face`; las dos filas nuevas de la tabla |
| `SetGussetSize_StretchesTheOutlineAboutTheWorkPoint` | Ancho 565 → 575 y alto 530 → 500 estiran el contorno; croquis y tabla lo muestran; 0 se rechaza |
| `DetalleD_BoltLabelShowsGripAndLength` | La etiqueta del croquis con agarre, longitud y cara; "(del plano)" con `length_mm` |

Las 84 pruebas anteriores siguen pasando sin cambios (el fixture no usa los campos nuevos; el token del fixture cambia
porque el hash de `limits.json` incluye la tabla nueva, como está previsto).

### 6.3 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-6b.md`)

| Qué | Paso |
|---|---|
| `9` y luego `12,7` en Espesor (mm) se aceptan (corrección de la Fase 6 desplegada) | 6b-5 |
| Cursor de mano sobre una cota; doble clic abre el cuadro junto a la cota con el valor seleccionado | 6b-6.1 |
| Enter aplica (retiro 180 → 200: cota, barra, tabla y estado); un valor malo da el error y desactiva Crear | 6b-6.2, 6b-6.3 |
| Esc y clic fuera cancelan sin cambiar nada | 6b-6.4 |
| Ancho de la cartela 565 → 575 estira el contorno (−254,42 / 320,58) y lo dice | 6b-6.5 |
| Recargar devuelve el fixture y el token inicial | 6b-6.6 |
| La placa cuchilla apoya en la cara +z y los pernos atraviesan cartela + placa (sondeo 16 y captura de canto) | 6b-7 |
| `Bolt Length 44,45` y `Grip Length 19,53` en los parámetros; `BindingLength`/`ScrewLength` sin `no existe` en el log | 6b-7 |
| Borrar desde la cinta y sondeos 12/13 en cero; 19/19 por el puente | 6b-8, 6b-9 |

### 6.4 NO PROBADO en la nube y por qué

- **Hacia qué lado extiende Advance Steel el agarre desde el plano del patrón.** `BindingLength`, `ScrewLength` e
  `IsInverted` existen (volcado de la Fase 1), pero no hay dato de si el perno empieza en el plano y sigue la normal,
  si la normal apunta a la cabeza o a la tuerca, o si el agarre se centra en el plano. He elegido la lectura más
  habitual (el plano es la cara donde empieza el perno y el agarre sigue la normal +Z), con el plano en la cara inferior
  del paquete. El sondeo 16 mide el intervalo Z real de los pernos frente al de las placas: si no cubre el paquete, la
  corrección es cambiar `StackMinMm` por `StackMaxMm` (o el centro) en `AdvanceSteelBackend.CreateBoltPattern`, o fijar
  `IsInverted`; una ronda corta. La reserva `DirectShape` no tiene esta duda.
- **Que `BindingLength` sea escribible y que Advance Steel respete `ScrewLength`** cuando también se fija el agarre: lo
  dice la lista `properties` del registro (`no existe` / `ERROR` si no) y los parámetros `Bolt Length` / `Grip Length`.
- **Que la geometría de los `SteelProxyElement` se pueda leer** con `get_Geometry` (la Fase 1 vio `BoundingBox` nulo). El
  sondeo prueba con detalle fino y con la vista activa; si no hay geometría, quedan los parámetros y la captura.
- **El cuadro de edición de la cota en WPF dentro de Revit** (foco, Enter, Esc, clic fuera, posición junto a la cota).
- **Que el cambio de cara (`-z`) sea el que quiere la persona**: el plano del Detalle D no lo dice; por defecto `+z`
  (en el Hangar, +Z local = global +Y según la leyenda del croquis).

### 6.5 Decisiones tomadas y por qué

- **La placa cuchilla apoya en una cara de la cartela, no en su plano.** Es como se construye (dos chapas solapadas y
  empernadas); la alternativa de desplazar la barra para que la placa quede centrada no se hace porque la barra está
  donde la modeló la persona. Queda el campo `gusset_face` para elegir la cara; sin él, `+z`.
- **Longitud del perno por tabla editable, no fija ni "mágica".** Agarre real + suplemento RCSC por diámetro, redondeo
  a 1/4"; todo en `limits.json` para que la persona lo cambie sin recompilar (por ejemplo a múltiplos de 5 mm para
  pernos métricos). Si el plano trae la longitud, manda el plano y el validador solo avisa si es corta.
- **Doble clic en la cota de la cartela estira el contorno alrededor del punto de trabajo.** La cota del ancho mide el
  contorno (decisión 4.8); si la persona la cambia, lo natural es que la cartela cambie. Escalar en un eje alrededor del
  punto de trabajo mantiene las proporciones y la posición relativa al nudo, y el texto de estado dice qué pasó y dónde
  mirar. Las cadenas de cotas del plano no se tocan (siguen sumando su total), por eso el aviso en la barra de estado.
- **Hit-test en el canvas, no controles WPF por cota**: las cotas se dibujan con `OnRender`; guardar su rectángulo de
  texto y su línea en píxeles al dibujar es barato y no cambia la forma de dibujar. El cursor de mano avisa de que la
  cota es editable.

### 6.6 Pendientes, riesgos y preguntas

- **P7**: el sentido del agarre en Advance Steel (6.4). Si el sondeo 16 dice que los pernos no cubren el paquete, hago
  la ronda 6c con la cara contraria o `IsInverted`.
- **P8**: `BoltPattern.Connect(plates, kOnSite)` conectaría los pernos a las dos placas (agarre calculado por Advance
  Steel y agujeros en las placas). No lo he usado: requiere los objetos `Plate` de Advance Steel en memoria dentro de la
  misma `FabricationTransaction` y no está probado; sigue para v2.
- **P9 (del fixture, no de esta ronda)**: con `insertion_mm 80` y `length_mm 170` la placa cuchilla tiene 90 mm libres,
  pero la segunda fila de pernos cae a 100 mm del extremo libre (40 + 60), es decir, 10 mm dentro de la ranura del HSS.
  El plano (`40, 60 y 43 mm, además de 38 cerca del extremo`) no cierra; el encargo ya lo señala como duda. Si quieres,
  en una ronda corta se añade la comprobación `BOLT_INSIDE_MEMBER_SLOT`.
- **P10**: ¿quieres que el doble clic sobre la etiqueta de los pernos abra la fila **Pernos: longitud (mm)**? Hoy solo
  las cotas (líneas con número) se editan desde el croquis; las etiquetas se editan en la tabla.

---

## 7. Ronda 6c (2026-10-01): el perno baja desde la cara exterior de la placa cuchilla

Resultados de la 6b (`resultados-fase-6b.md`, capturas `fase6b-01` y `fase6b-04`): los decimales tras un entero se
aceptan (9 → 12,7), las cotas se editan con doble clic tal como estaba previsto (Enter aplica, Esc y clic fuera cancelan,
el paso 10 da `BOLT_SPACING_TOO_SMALL`, el ancho estira el contorno, Recargar devuelve el token `2db4259a365fd679`), la
placa cuchilla apoya sobre la cartela (`offset_mm: 9.7625`) y Advance Steel aceptó `BindingLength=19.525` y
`ScrewLength=44.45` (parámetros `Bolt Length 44,45` y `Grip Length 19,53`). Pero **el perno entero quedó colgando por
fuera de la cara trasera de la cartela**: cabeza, vástago y tuerca visibles por un solo lado. Es la duda P7: Advance
Steel extiende el perno desde el plano del patrón **hacia −Z** (en contra de la normal), y el plano estaba en la cara
inferior del paquete.

**Qué cambia (solo `AdvanceSteelBackend.CreateBoltPattern`)**: el plano del patrón pasa de `StackMinMm` (−4,76 mm) a
`StackMaxMm` (14,76 mm, la cara exterior de la placa cuchilla). Con el mismo agarre, el perno recorre placa + cartela
hacia −Z: cabeza sobre la placa cuchilla, tuerca sobre la cara trasera de la cartela. El registro anota ahora
`plane_z_mm: 14.7625` y `stack_min_z_mm`. La reserva `DirectShape` no cambia (dibuja el perno ella misma). Sin cambios
en Core: 99/99 pruebas, compilación sin avisos.

**Sondeo 16 corregido**: falló en la 6b con `UnicodeDecodeError ... byte 0xe1 in position 28` al reenviar a `validate`
la especificación devuelta por `conn_get` ("La etiqueta del montante est**á**": `json.dumps` de IronPython sobre un
texto no ASCII, el mismo fallo que ya salva `conexiones.py` con `_json_ascii`). Ahora lee el fixture del disco, como el
sondeo 11, y no serializa nada que venga del Bridge. Sigue siendo la medida objetiva: intervalo Z de cartela, placa y
pernos.

**Otros datos de la 6b**: `probar_conexiones.py --puente` dio 17/19 porque el puente no llegó a arrancar en los 15 s
(los dos fallos son "sin puente", no del add-in; la 6b-9 del paso anterior dio 19/19 con el mismo código del MCP). La
captura de perfil del sondeo (apartado 5) no se generó por el fallo anterior.

**PENDIENTE DE INSTALADOR**: `docs/instalacion/fase-6c.md` (crear, mirar de canto, sondeo 16, borrar). **NO PROBADO en
la nube**: que con el plano arriba el perno cubra exactamente el paquete (si Advance Steel midiera el agarre desde otra
referencia, el intervalo Z del sondeo 16 lo dirá y el ajuste sería otro desplazamiento de una línea).
