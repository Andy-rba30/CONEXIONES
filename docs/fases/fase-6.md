# Fase 6: ventana de previsualización 2D con cotas, borrado desde la cinta y panel en la pestaña ARBA

Fecha: 2026-10-04. Rama: `claude/laughing-pascal-tsxvkt`. Add-in **0.2.0** (Core con `Sketch/` y `Editing/`; add-in con
ventanas WPF en `UI/`); adaptador y herramientas MCP 0.4.0 **sin cambios**. Alcance: `docs/prompts/fase-6.md`.

**Estado: programada y compilada en la nube; PENDIENTE DE INSTALADOR** (`docs/instalacion/fase-6.md`). Todo lo que pasa
dentro de Revit (ventanas WPF, cinta ARBA, crear y borrar desde los botones) está **NO PROBADO** hasta que vuelvan los
resultados en `docs/fases/resultados-fase-6.md`.

---

## 1. Qué se hizo

### 1.1 Core (sin Revit, probado con xUnit)

- **`Core/Sketch/`**: el croquis como datos puros en mm, en el sistema local del nudo (sección 7 del encargo).
  - `SketchPrimitives.cs`: `SketchLine` (capas eje, arista, oculta, soldadura, punto de trabajo), `SketchPolygon`
    (barra, cartela, placa cuchilla), `SketchCircle` (perno), `SketchDimension` (dos puntos, desplazamiento, valor y
    texto `565,0`, ruta JSON del campo que mide), `SketchLabel`, `SketchModel` (listas + caja envolvente + avisos) y
    `SketchFormat` (mm con una cifra decimal y coma, grados).
  - `SketchNodeInput.cs`: lo que el croquis necesita del nudo y no está en el JSON: dirección 2D y ancho de cada barra.
    `FromModelFacts(spec, IModelFacts)` calcula el sistema local con **la misma regla que el add-in**
    (`NodeFrame.Compute` con el cordón y el primer miembro) y cada dirección con
    `ConnectionGeometry.GetMemberDirection2D`; si falta el modelo o una barra, cae a un modo **esquemático** (montantes
    verticales, diagonales con `expected_angle_deg`, cuadrantes alternos) y lo anota. Avisa también cuando el eje Y
    local apunta hacia abajo en el modelo. `ProfileDimensions` saca el ancho del nombre del perfil (`HSS3X3X1/4` → 76,2;
    `HSS2-1/2X2-1/2X3/16` → 63,5; `… 64x64` → 64) cuando el modelo no da parámetros.
  - `GussetNodeSketch.cs`: el dibujo de `gusset_node`: cordón (eje + cuerpo), punto de trabajo, cartela con cotas de
    ancho y alto (medidas sobre el contorno) y etiqueta `PL 3/8" (9,5 mm)`, cada barra con su cuerpo **a partir del
    retiro**, cota `retiro`, rótulo (rol, perfil, ángulo con el cordón), ranura oculta y cota `ranura` para
    `welded_slot`, y para `bolted_knife_plate` la placa (`ComputeKnifePlateCorners`), los pernos
    (`ComputeBoltPositions`), las cadenas de cotas a lo largo (1.ª fila / paso / resto) y transversal (borde / paso /
    borde), `inserción`, `placa` largo y ancho, y la etiqueta de pernos; las soldaduras salen de
    `ConnectionGeometry.ComputeWeldLines`, así que **se dibuja lo mismo que se crea**.
  - `ISketchProvider` + `SketchBuilder.Build(spec, node)`: elige el croquis por `connection_type` a través del registro
    de tipos; `GussetNodeType` implementa `ISketchProvider`; `gusset_node` siempre tiene croquis aunque el registro esté
    vacío (el botón de la cinta no pasa por `Bridge`); un tipo sin croquis devuelve un modelo vacío con el aviso.
- **`Core/Editing/`**: la tabla editable como datos.
  - `SpecFieldCatalog.Build(spec, node)`: las filas del paso 8 del guion de la Fase 5 (origen, cordón, cartela, contorno
    vértice a vértice, cada barra con solo las filas de su tipo de unión, ángulo y ancho del modelo en solo lectura,
    cadenas de cotas, dudas con `user_confirmed_value`), con ruta JSON, grupo, etiqueta en español, valor formateado y
    ayuda.
  - `SpecFieldEditor.Apply(spec, path, text)`: interpreta el texto por la ruta (números con coma o punto, enteros,
    `sí/no`, opciones, listas `75; 420; 70`, puntos `-175; 280`, vacío = null) y lo coloca en el campo; al cambiar el
    tipo de unión crea los objetos vacíos de la otra rama; al cambiar `thickness_mm` o `diameter_mm` **sincroniza la
    etiqueta** si dejaba de coincidir (`LabelFormatter`: `12,7` → `1/2"`, `10` → `PL10`, `20` → `20 mm`) y lo dice en
    `Note`. Errores en español sin tocar la especificación.
  - `SpecValueParser`: lectura y escritura de esos textos.
- **`Validation/LabelFormatter.cs`**: inverso de `LabelParser` (fracciones de 1/64" con tolerancia 0,05 mm, o métrico);
  todo lo que genera lo vuelve a leer `LabelParser` (prueba de ida y vuelta).
- **`SpecValidator`**: regla 2b nueva: los campos que el esquema declara obligatorios pero que pueden llegar como `null`
  (espesor/ancho/alto de la cartela, rol y retiro de cada barra, `slot_length_mm`, medidas de la placa cuchilla y de
  los pernos, tamaño de los filetes) dan `SCHEMA_INVALID` salvo que su ruta esté en `uncertain_fields`. Antes una placa
  cuchilla con todo `null` validaba (el comprobador del esquema trata `null` como ausente) y la creación usaba valores
  por defecto. Sin este cierre, cambiar el tipo de unión en la ventana habría dejado **Crear** habilitado con medidas
  inventadas.
- `ConnectionSpec.ToJson(indented)` y `JsonOptions.Indented` para el archivo `-corregido.json`.

### 1.2 Add-in (compila en la nube; NO PROBADO en Revit)

- `MotorConexiones.Revit.csproj`: `<UseWPF>true</UseWPF>`. Comprobado primero con un proyecto de prueba en la nube:
  con `EnableWindowsTargeting` el compilador de XAML corre en Linux (`*.g.cs` generados) y la solución compila con
  **0 avisos**. No hizo falta Windows Forms ni cargar XAML en tiempo de ejecución.
- `UI/SketchView.cs`: control WPF que dibuja un `SketchModel` en `OnRender` (capas con trazos distintos, pernos con
  cruz, marcas de soldadura, cotas con líneas de referencia, marcas oblicuas y texto girado siempre legible, indicador
  de ejes X/Y). Rueda = zoom alrededor del cursor, botón central = encuadre, `Fit()` = Ajustar. El factor mm → píxel es
  solo de pantalla.
- `UI/PreviewWindow.xaml(.cs)`: las tres zonas del alcance 3.1 (croquis + Ajustar + avisos; tabla agrupada con
  columnas Campo / Valor / Ruta JSON y ayuda en el detalle de la fila; lista de errores y avisos con código, campo,
  mensaje y sugerencia; estado con token abreviado; botones Recargar, Guardar JSON, Validar, Crear, Cancelar). Editar un
  valor (Intro) llama a `SpecFieldEditor` por la ruta, actualiza el JSON, recalcula nudo, croquis y validación y, si el
  texto no se entiende, devuelve el valor anterior y explica por qué. **Crear** solo cierra la ventana con
  `DialogResult = true`; no toca el modelo.
- `UI/PreviewSession.cs`: estado de la ventana (especificación, JSON, nudo, croquis, validación); `Refresh()` resuelve
  el nudo con `NodeInspector.ResolveNode`, construye `RevitModelFacts` y llama a Core; `Reload()` vuelve a leer el
  archivo; `SaveCorrected()` escribe `<nombre>-corregido.json` junto al original (nunca encima), con sangría.
- `RunSpecCommand.cs`: archivo (ahora con `Microsoft.Win32.OpenFileDialog` de WPF, sin reflexión sobre Windows Forms) →
  IDs de la selección si el JSON no los trae (como antes) → ventana modal (`ShowDialog`, dueño = ventana principal de
  Revit, mismo hilo, sin `ExternalEvent`) → si acepta y `CanCreate`, crea **exactamente como antes**
  (`OperationScope` + `ConnectionCreationService.CreateConnection` + `AdoptNewElements`) y muestra el `connection_id`.
  Las dudas sin confirmar ya no paran el comando antes de la ventana: se ven como errores y se confirman en la tabla.
- `UI/ConnectionsWindow.xaml(.cs)` y `ModelConnectionsCommand.cs` (alcance 3.2): lista las conexiones del
  `ConnectionStorageManager` (id, tipo, plano, fecha local, elementos, barras, backend) y borra la seleccionada tras
  confirmar, con el mismo `OperationScope` + `ConnectionCreationService.DeleteConnection` que `conn_delete`.
- `App.cs` (alcance 3.3, primer caso): `CreateRibbonTab("ARBA")` en try/catch, `GetRibbonPanels("ARBA")` busca el
  panel **Conexiones** y lo crea si falta, dos botones grandes con iconos dibujados en código (`UI/RibbonIcons.cs`,
  `DrawingVisual` + `RenderTargetBitmap`, como los add-ins de ARBA). Si cualquier paso lanza, reserva a la pestaña
  **Conexiones** / panel **MotorConexiones** de antes y lo anota (`ribbon_arba_failed`); la línea `startup` del log lleva
  `ribbon_tab`, `ribbon_panel` y `arba_error`. No se tocan los paneles IA, Acero, Metrados ni Encofrado.
- Versión 0.2.0 (`AddinInfo`, los dos `.csproj`, simulador, nota en `mcp/CONTRATO-conn.md`); `deploy.ps1` dice ahora
  dónde debe aparecer el panel; `CLAUDE.md` nombra las ventanas permitidas.

### 1.3 Sondeos, instrucciones y documentación

- `scripts/sondeos/15-cinta-arba.py`: lista pestañas, paneles y botones con `Autodesk.Windows.ComponentManager.Ribbon`
  (solo lectura) para ver dónde quedó el panel.
- `docs/instalacion/fase-6.md`: una ronda de 11 pasos con `Anota`, capturas con nombre fijo (`fase6-01` a `fase6-06`) y
  qué devolver.
- `README.md`: estado, árbol, pasos de instalación (ARBA, 0.2.0, 114 pruebas), secciones nuevas **8. Previsualizar y
  corregir antes de crear** y **9. Borrar desde la cinta**, `ISketchProvider` en "agregar un tipo nuevo", fila
  `SCHEMA_INVALID`. La tabla de garantías no cambia.
- Este informe.

Sin cambios en `mcp/` salvo la versión del simulador y una nota en el contrato; `Bridge.Handle` y las 13 operaciones no
se tocan.

## 2. Qué se probó en la nube y cómo

### 2.1 Compilación (Core, Revit con WPF y Tests)

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

La carpeta de salida del add-in sigue teniendo solo `MotorConexiones.Core.dll`, `MotorConexiones.Revit.dll`, sus `.pdb` y
el `.addin`: WPF viene con el runtime de .NET 10 de Revit, `deploy.ps1` no cambia. SDK en la nube: `dotnet-sdk-10.0`
10.0.112 por `apt` (igual que en las fases anteriores); paquetes `Nice3point.Revit.Api.*` 2027.2.0 desde nuget.org.

Comprobación previa de WPF en Linux (proyecto de prueba aparte, fuera del repositorio): `net10.0-windows` +
`EnableWindowsTargeting` + `UseWPF` con una ventana XAML (Canvas, DataGrid, evento `MouseWheel`) y otra en código:
`Build succeeded`, `0 Warning(s)`, `obj/Release/net10.0-windows/ProbeWindow.g.cs` generado.

### 2.2 Pruebas unitarias (`dotnet test -c Release`)

```text
Passed!  - Failed:     0, Passed:   114, Skipped:     0, Total:   114
```

Tres pasadas seguidas en verde (antes había una carrera entre `SketchBuilderTests` y `ConnectionTypeRegistryTests` por
el registro estático de tipos: ahora comparten colección de xUnit y `gusset_node` no depende del registro). 51 pruebas
anteriores sin cambios + 63 nuevas:

- `SketchBuilderTests` (fixture `detalle-D-confirmado.json` + `FakeModelFacts` del nudo real del Hangar):
  direcciones y anchos del modelo (1249630 → (0,707; -0,707), 1249631 → (0; -1), 1249636 → (-0,707; 0,707); 76,2 y
  63,5 mm); piezas (1 cartela, 1 placa cuchilla, 4 cuerpos de barra, 4 pernos, 6 soldaduras, 4 ejes, 16 cotas); cotas
  con los valores del contrato (565, 530, 180, 60, 260, 150 ×2, 170, 140, 80, 40, 60, 70) y cada cota mide de verdad la
  distancia entre sus puntos; los retiros terminan donde empieza cada barra (montante: extremo en (0; -60)); pernos y
  placa idénticos a `ConnectionGeometry`; etiquetas `PL 3/8" (9,5 mm)`, `PL10 (10,0 mm)`, `Montante … 90,0°`,
  `4 Ø5/8"`; caja envolvente con las cotas; modo esquemático completo sin modelo; barra ausente → esquemática solo ella;
  registro vacío / tipo desconocido; cartela sin contorno; `ProfileDimensions`; `SketchFormat`; desplazamiento de cota.
- `SpecFieldEditorTests`: filas del catálogo (valores `9,525`, `3/8"`, `-175; 280`, `75; 420; 70`, `sí`, solo las
  filas del tipo de unión de cada barra); **espesor 9,525 → 12,7 actualiza la etiqueta a `1/2"`, la validación sigue en
  verde y el token cambia** (lo que pide el paso 6-5); mismo valor no toca la etiqueta; **paso de pernos 10 →
  `BOLT_SPACING_TOO_SMALL` en `members[2]…spacing_mm` y sin token** (paso 6-6); textos inválidos fallan en español sin
  tocar la especificación; vértice, cadena 402 → `DIMENSION_CHAIN_MISMATCH`, duda vacía → `UNRESOLVED_UNCERTAINTY`;
  cambio a `bolted_knife_plate` crea placa y pernos vacíos y el validador los pide campo por campo; diámetro 19,05 →
  `3/4"`; ida y vuelta por JSON con sangría; analizador de números con coma y punto.
- `LabelFormatterTests`: fracciones exactas, rechazo de métricos, estilo de la etiqueta anterior, diámetros, y que
  `LabelParser` lee todo lo que `LabelFormatter` escribe.

### 2.3 Python

`python3 -m py_compile` de `scripts/sondeos/15-cinta-arba.py` y `mcp/pruebas/simulador_revit.py`: sin errores.

### 2.4 PENDIENTE DE INSTALADOR (`docs/instalacion/fase-6.md`)

| Qué | Paso |
|---|---|
| Panel **Conexiones** en la pestaña **ARBA** con los dos botones (captura, sondeo 15, `ping` 0.2.0, log `ribbon_arba`) | 6-3 |
| La ventana abre con el Detalle D: croquis, tabla, "0 errores, 2 avisos", zoom, encuadre, Ajustar; cotas iguales al plano | 6-4 |
| Espesor 9,525 → 12,7: etiqueta `1/2"`, token nuevo, validación en verde; **Guardar JSON** crea `-corregido.json` y el original no cambia | 6-5 |
| Paso de pernos 10 → `BOLT_SPACING_TOO_SMALL` y **Crear** deshabilitado | 6-6 |
| **Crear** desde la ventana: diálogo con `connection_id`, nudo en Revit, `conn_list` = 1 | 6-7 |
| **Conexiones del modelo**: lista, borrado con confirmación, sondeo 12 (0 conexiones, extensiones restauradas) y 13 (0 restos) | 6-8 |
| `probar_conexiones.py --puente` 19/19 (la IA no se ve afectada) | 6-9 |
| Registro sin `*_failed` | 6-10 |

### 2.5 NO PROBADO (no se puede en la nube)

Todo lo de WPF dentro de Revit: que la ventana se abra modal sobre Revit y en su hilo, el `DataGrid` agrupado y su
edición con Intro, el redibujado del `SketchView`, el `OpenFileDialog` de WPF, el `MessageBox` de confirmación, los
iconos dibujados con `RenderTargetBitmap` durante `OnStartup`, que `GetRibbonPanels("ARBA")` vea los paneles de los
otros add-ins y que `panel.Visible = true` no moleste, y el comportamiento del add-in cuando ARBA no existe (reserva).
Los miembros de la API de Revit usados (`CreateRibbonTab`, `GetRibbonPanels(string)`, `RibbonPanel.Name/Visible`,
`UIApplication.MainWindowHandle`, `ButtonData.LargeImage/Image`) existen en los paquetes 2027.2.0: compilan. El sondeo
15 usa `Autodesk.Windows` (AdWindows), que no está en NuGet: cada propiedad va con `getattr` protegido y, si algo falta,
imprime el error en vez de parar.

## 3. Qué debo mirar yo cuando el instalador termine

1. **Captura `fase6-01-cinta.png`**: el panel **Conexiones** dentro de **ARBA**, junto a Acero, Metrados, Encofrado,
   Georeferenciación e IA, y ninguna pestaña **Conexiones** aparte. Si está aparte, en el log hay `ribbon_arba_failed`
   con la excepción: pégamela.
2. **Captura `fase6-02-ventana.png`**: compara el croquis con el plano. Lo que debe coincidir está listado en el paso
   6-4. Fíjate sobre todo en **hacia dónde salen las barras respecto a la cartela**: con el nudo real del Hangar el eje
   Y local apunta hacia abajo (sección 5, riesgo 1), así que la diagonal soldada 1249630 sale hacia -Y y la empernada
   1249636 hacia +Y, donde está el borde ancho (420) del contorno. Si en el plano la diagonal empernada está en el lado
   estrecho, el contorno del fixture está transcrito al revés respecto al sistema local y hay que corregir el fixture
   (o la regla del signo de Y), no la ventana.
3. **Captura `fase6-03-espesor-12-7.png`**: etiqueta `PL 1/2" (12,7 mm)`, fila "Etiqueta del espesor" = `1/2"`, "0
   errores", token distinto del de la captura anterior.
4. **Captura `fase6-04-error-paso.png`**: `BOLT_SPACING_TOO_SMALL`, campo `members[2].attachment.bolts.spacing_mm`,
   **Crear** gris.
5. **Captura `fase6-05-nudo-creado.png`**: igual que en la ronda 5b (cartela, cuchilla con 4 pernos, barras acortadas).
6. **Captura `fase6-06-conexiones-modelo.png`** y la salida de los sondeos 12 y 13 tras borrar.
7. En `6-10 log`: ninguna línea `*_failed`; `preview_edit` con `note` en el cambio de espesor.

## 4. Decisiones tomadas y por qué

- **ARBA, primer caso del alcance 3.3, con panel "Conexiones"**: la respuesta del instalador dice que ARBA la crean 10
  add-ins de C# con `CreateRibbonTab` en try/catch y que basta buscar/crear el panel con `GetRibbonPanels("ARBA")`, sin
  esperar a `ApplicationInitialized`. Se sigue al pie de la letra (el prompt hablaba de un panel "MotorConexiones"; la
  respuesta, más reciente y concreta, pide "Conexiones"). La reserva a la pestaña antigua queda por si ARBA falla.
- **WPF con XAML** porque compila en la nube (comprobado antes de escribir la ventana). Un solo control de dibujo
  (`SketchView`, `OnRender`) en vez de cientos de `Shape` en un `Canvas`: más simple de redibujar en cada edición.
- **La ventana no crea ni borra por sí misma**: Crear cierra la ventana y el comando crea con el código de siempre;
  Borrar lo ejecuta el comando a través de un delegado con `OperationScope`. Así los servicios y las transacciones son
  exactamente los de las rutas `conn_*` y la ventana solo dibuja y edita (regla del prompt).
- **El croquis se dibuja en el sistema local tal cual** (X derecha, Y arriba), porque las coordenadas del contrato
  (`outline.points_mm`) están en ese sistema y es lo que la persona edita; cuando el eje Y local apunta hacia abajo en
  el modelo, el croquis lo avisa en vez de girar el dibujo a escondidas.
- **Sincronizar la etiqueta al editar el número** en vez de dejar `LABEL_VALUE_MISMATCH`: el paso 6-5 pide que al pasar
  de 9,525 a 12,7 la validación siga en verde, y la etiqueta es un dato derivado del número. Si el nuevo valor no es
  una fracción exacta de pulgada, la etiqueta pasa a métrico (`PL12.5`). Siempre se anota en la línea de estado.
- **Regla 2b del validador** (campos obligatorios nulos): necesaria para que la ventana no habilite Crear con valores
  por defecto tras cambiar el tipo de unión; respeta `uncertain_fields` como el resto de reglas.
- **Versión 0.2.0** para que el instalador compruebe con `conn_ping` que Revit cargó la DLL de esta fase.
- **Tabla con columnas de texto** (no desplegables) para no depender de plantillas WPF que no se pueden probar aquí;
  las opciones válidas se ven en la ayuda de la fila y el editor rechaza lo que no esté en la lista.

## 5. Pendientes, riesgos y preguntas para ti

1. **Riesgo: orientación del contorno del Detalle D.** Con los datos reales del nudo (`FakeModelFacts`, copiados de la
   Fase 1), `NodeFrame.Compute` da Y local = -Z global (la cercha está en el plano XZ y la regla del signo de Z elige
   +Y global). Así, la ranura de la diagonal soldada 1249630 (de 180 a 330 mm hacia (0,707; -0,707)) **sale del
   contorno del fixture a partir de unos 200 mm**: de sus 150 mm, unos 130 quedan fuera, por debajo del borde inclinado
   (comprobado numéricamente con el polígono del fixture: dentro a 180 y 195 mm, fuera de 210 a 330), mientras la placa
   cuchilla de 1249636 queda entera dentro del lado ancho y la ranura del montante también está dentro. El validador no
   lo detecta porque `PLATE_OUTSIDE_GUSSET` solo mira la placa cuchilla. La ventana lo hará visible en el
   paso 6-4; según lo que veas, habrá que girar el contorno del fixture (o decidir que el signo de Y se elija mirando
   hacia arriba) en una sesión de cierre. No se ha tocado nada de esto en esta fase.
2. **NO PROBADO**: toda la lista de la sección 2.5. Si la ventana no abre, el log tendrá `preview_window_failed` con la
   excepción completa; si la cinta falla, `ribbon_arba_failed`.
3. **Edición en la tabla**: si al pulsar Intro el valor no se aplica hasta cambiar de fila, el `DataGrid` está
   confirmando la celda en otro momento del esperado; el arreglo es pequeño (forzar `CommitEdit` en `KeyDown`), pero
   quiero el dato real antes.
4. **Pendiente de la Fase 5 (P2)**: `conn_get_schema` de un tipo inexistente sigue respondiendo `UNKNOWN_OPERATION`.
5. **README**: la tabla de garantías sigue igual hasta que lleguen los resultados; entonces se añaden las filas
   "Previsualización y corrección desde la cinta" y "Borrado desde la cinta" con su referencia.
6. **Pregunta**: ¿quieres que la ventana también permita **actualizar** una conexión ya creada (`conn_update` desde la
   cinta)? Está fuera del alcance de esta fase, pero la sesión de la ventana ya tiene todo lo necesario.

---

## 6. Rondas del 2026-10-04 en el PC: primera (DLL ajena) y 6b (DLL de la rama)

Resultados en `docs/fases/resultados-fase-6.md` (commits `25817d0` y `9f3aa26`) y capturas `fase6-01` a `fase6-06`.

### 6.1 Primera ronda: Revit no ejecutó el add-in de este repositorio

`conn_ping`, el log de arranque y las 19 pruebas del puente dieron `addin_version 0.1.0`, el panel de ARBA se llamaba
`MotorConexiones`, y el log tenía eventos `ribbon_panel_created`, `ribbon_preview_opened`, `ribbon_preview_edit`,
`ribbon_create` y un bloque `bolt_stacks` en `validate` que **no existen en ningún commit de la rama** (los reales son
`ribbon_arba`, `preview_window_opened`, `preview_edit`, `run_spec_ribbon_created`). El agente instalador (Antigravity)
tenía en `D:\Proyectos C#\CONEXIONES` una implementación propia de la ventana, la compiló y la desplegó, y después la
borró con `git stash` y `git reset --hard` (el `stash` quedó vacío). Lo que la persona vio en esa ronda (entre otras cosas,
**edición de cotas con doble clic en el croquis** y **pernos bien colocados** con longitud calculada desde el agarre)
era ese código, no el de la rama. Esa ronda **no vale** como prueba de la Fase 6; se repitió.

### 6.2 Ronda 6b con la DLL de la rama (0.2.0): lo que funcionó y lo que no

Funcionó: `deploy` 0.2.0 con Revit cerrado; panel **Conexiones** en la pestaña **ARBA** (sondeo 15 y log `ribbon_arba`);
la ventana con croquis, tabla, edición desde la tabla (espesor 12,7 → etiqueta `1/2"`, paso 10 → `BOLT_SPACING_TOO_SMALL`
con Crear deshabilitado), Recargar, Crear (9 elementos, Advance Steel); Conexiones del modelo con borrado (9 elementos, 3
barras restauradas); sondeos 12 y 13 limpios; 19/19 por el puente; ningún `*_failed`. Cotas del croquis comprobadas contra
el plano por la persona.

No funcionó o no se probó:

1. **Pernos** (captura `fase6-05-nudo.png`): los 4 pernos salían sueltos del plano medio de la cartela, sin atravesar
   la placa cuchilla, y la placa cuchilla se creaba **en el mismo plano que la cartela** (se cruzaban). Es el código de
   la Fase 3 sin cambios: `CreatePlate` ponía las dos placas en z = 0 y `CreateBoltPattern` apoyaba el patrón en z = 0
   con `ScrewLength = 45` fijo (en la Fase 5b, `Grip Length 80 mm` ya delataba que los pernos no abrazaban nada). La
   primera ronda lo había resuelto en el código ajeno (`bolt_stacks`, `gusset_face +z`, `computed_from_grip`), por eso la
   persona lo vio como "el mismo error que ya estaba solucionado".
2. **Doble clic en las cotas**: la ventana de la rama solo editaba desde la tabla.
3. **Guardar JSON**: no se pulsó en 6b (el único `preview_saved` del día es de la DLL ajena).
4. Mensaje de la ventana de borrado al revés ("No hay conexiones… Borrada …").
5. El informe anterior dio el 19/19 y el "4-9" de la primera ronda por buenos sin mirar la versión: lección anotada en
   `docs/instalacion/fase-6b.md` (el instalador no toca `src\` y devuelve `git status` literal).

## 7. Corrección 6b (sesión en la nube, add-in 0.2.1)

### 7.1 Qué se hizo

- **Paquete de pernos** (`Core/Geometry3D/BoltStack.cs`, nuevo): cartela centrada en el plano del nudo; placa cuchilla
  apoyada sobre la cara **+Z** de la cartela (plano medio a `(t_cartela + t_cuchilla) / 2`); agarre = suma de espesores;
  longitud del perno = agarre + suplemento de la **tabla 7-15 del AISC Manual** (nueva en `config/limits.json`:
  `bolts.length_addition_mm` por diámetro y `length_increment_mm` = 6,35), redondeada hacia arriba a 1/4". Detalle D:
  agarre 19,525 mm, 5/8" → +22,225 → **44,45 mm (1-3/4")**. Las dos claves nuevas entran en el hash de `limits.json`
  (y por tanto en el `validation_token`).
- **Backends** (`IFabricationBackend` con `zOffsetMm` en `CreatePlate` y `BoltStack` + nombres de placas en
  `CreateBoltPattern`): DirectShape crea la cuchilla desplazada y los pernos desde la cara exterior de la cuchilla hacia
  −Z; Advance Steel crea el plano de la cuchilla desplazado, fija `Portioning = 0,5` si la propiedad existe, apoya el
  patrón de pernos en la cara exterior de la cuchilla con `YDirection` invertida para que la normal mire hacia dentro del
  paquete, fija `ScrewLength` y `BindingLength`, y después de `WriteToDb` intenta `Connect(FilerObject[] {cartela,
  cuchilla}, eAssemblyLocation)` (firma del sondeo 06) guardando los objetos de la sesión por nombre. Todo queda en el
  registro (`advance_steel_plate_written` con `z_offset_mm` y `portioning`; `advance_steel_bolts_written` con `grip_mm`,
  `bolt_length_mm`, `head_face_z_mm`, `connect`), y nada de ello interrumpe la creación si falla.
- `conn_preview` devuelve `bolt_stacks` (espesores, desplazamiento, agarre, suplemento, longitud, cara de la cabeza) y la
  ventana lo muestra en una línea bajo el croquis.
- **Doble clic en el croquis** (`UI/SketchView.cs`, `UI/PreviewWindow.xaml(.cs)`): cada cota y cada rótulo con ruta JSON
  registra su zona en pantalla; al pasar el ratón se resalta (mano y recuadro); el doble clic selecciona la fila de la
  tabla y abre un editor en sitio junto al cursor (campo, ruta, valor); Intro aplica por el mismo `ApplyEdit` de la tabla
  (misma validación, mismo registro `preview_edit`), Esc cancela. Para el contorno de la cartela abre el primer vértice.
- Mensaje de la ventana de borrado: primero lo que pasó ("Borrada …"), después cuántas quedan.
- Versión **0.2.1**; sondeo nuevo `scripts/sondeos/16-medir-conexion.py` (mide la conexión existente sin crear ni borrar:
  `Bolt Length`, `Grip Length`, espesores…); sondeo 11 con los valores esperados nuevos; `docs/instalacion/fase-6b.md`.

### 7.2 Qué se probó en la nube

```text
dotnet build MotorConexiones.sln -c Release   → 0 Warning(s), 0 Error(s) (Core, Revit con WPF, Tests)
dotnet test                                     → Passed! 123/123 (114 anteriores + 9 nuevas: BoltStackTests y limits.json)
py_compile (mcp y sondeos 00-16)                → sin errores
simulador --autocomprobar                       → 27/27;  probar_conexiones.py contra el simulador → 17/17
```

Pruebas nuevas: agarre y longitud del Detalle D (19,525 → 44,45), posiciones Z de la cuchilla y de las caras, otros
diámetros (1/2" → 38,1; 3/4" → 50,8; 1" → 57,15), redondeo, interpolación y reserva de la tabla, límites personalizados
(cambian la longitud y el hash), `limits.json` del repositorio con las claves nuevas.

### 7.3 NO PROBADO (PENDIENTE DE INSTALADOR, `docs/instalacion/fase-6b.md`)

| Qué | Paso |
|---|---|
| Que Advance Steel acepte el plano desplazado de la cuchilla y que `Portioning` exista (si no, la cuchilla puede quedar media a cada lado del plano desplazado: el sondeo 16 lo mide) | 6b-5 |
| Que el patrón de pernos apoyado en la cara exterior ponga la cabeza en esa cara y el vástago hacia la cartela (si saliera al revés, el arreglo es `IsInverted` o quitar el signo de `YDirection`: una línea, con el dato real) | 6b-5, captura `fase6b-03` |
| Que `Connect` exista con esa firma y haga los agujeros y el agarre (`Grip Length 19.5`); si no, los pernos quedan igualmente en su sitio con 44,45 mm | 6b-5 (sondeo 16 y log) |
| Doble clic sobre cotas y rótulos: resaltado, editor, Intro, Esc, fila seleccionada; cota de una cota inclinada | 6b-4 |
| Guardar JSON con la 0.2.1 | 6b-4 |
| Mensaje de borrado | 6b-6 |

### 7.4 Decisiones

- **Longitud de perno por tabla y no fija**: la 45 mm de la Fase 3 era un valor "por defecto hasta que el contrato lo
  pida"; el contrato sigue sin pedirla y lo correcto es derivarla del agarre con la tabla del AISC Manual, editable en
  `limits.json` como el resto de mínimos. Coincide con el 44,45 que la persona vio en la primera ronda.
- **Cara +Z de la cartela para la cuchilla**: el plano no lo fija; se elige la cara +Z (la que da la regla del signo de
  Z del sistema local, sección 7 del encargo) y queda escrito en `BoltStack.GussetFace` y en `conn_preview`.
- **`Connect` como mejora, no como condición**: si falla, se anota y la geometría ya está donde debe; así la ronda no
  depende de una llamada que solo conocemos por el volcado de tipos del sondeo 06.
- **Editor en sitio reutiliza `ApplyEdit`**: el doble clic no es otra forma de editar, es un atajo a la misma fila de la
  tabla, con la misma validación y el mismo registro.

### 7.5 Pendientes que siguen

- Orientación del contorno (sección 5, punto 1): la persona anotó en 6b que las barras salen hacia el lado ancho de la
  cartela y que las cotas coinciden con el plano; con la captura `fase6-02-ventana.png` el croquis es coherente con el
  plano del Detalle D. Se deja como está; si la ranura de la diagonal soldada se ve fuera de la cartela en Revit, se revisa.
- `UNKNOWN_OPERATION` en `conn_get_schema` (P2 de la Fase 5).
- Soldaduras nativas de Advance Steel (v2).
