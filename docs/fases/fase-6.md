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
- **Pruebas** (`SketchBuilderTests.cs`, 17; `SpecEditorTests.cs`, 15): de 51 a **83**.
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
