# Fase 7: catálogo de conexiones (plantillas con nombre que se aplican a otro nudo)

Fecha: 2026-10-01. Rama: `claude/fervent-allen-bxmdt1`. Add-in 0.1.0 (Core con `Catalog/`; add-in con `Catalog/`, cinco
operaciones nuevas, botón **Catálogo** y dos botones más en la ventana de previsualización); adaptador 0.7.0 (20 rutas) y
18 herramientas `conn_*`. Prompt y alcance: `docs/prompts/fase-7.md`, escrito a partir de las secciones 2, 3.3, 4 y 6 de
`docs/propuestas/catalogo-y-lotes.md` y de las decisiones de su sección 7.1.

**Estado: escrita y probada en la nube (compila sin avisos; 129/129 pruebas del Core; simulador del MCP 36/36 y
`probar_conexiones.py` 23/23 contra el simulador); pendiente de probar en Revit.** Todo lo que pasa dentro de Revit (el
marco canónico sobre el nudo real, guardar la plantilla desde la conexión creada, aplicarla al mismo nudo y al simétrico,
las ventanas) está **NO PROBADO** hasta que vuelvan los resultados de `docs/instalacion/fase-7.md` en
`docs/fases/resultados-fase-7.md`. La ronda 6d (placas centradas) sigue pendiente de instalador y no se toca aquí.

---

## 1. Qué se hizo

- **Marco canónico del nudo (decisión P5)**, `Core/Geometry3D/NodeFrame.cs`: X = eje del cordón orientado hacia +X global
  (si el cordón va en Y, hacia +Y; vertical, hacia +Z); Y en el plano de la cercha orientado hacia +Z global (hacia arriba
  en cerchas verticales; en horizontales, hacia +Y); Z = X × Y. Origen y regla de los 5 mm sin cambios. Propiedad
  `ChordReversed` (la curva del cordón va al revés). Ángulos con signo: `NodeFrame.SignedAngleDeg(ux, uy)` en
  [−180°, 180°), `AngleToChordDeg` (inclinación sin signo, la que escribe un plano), `AngleDifferenceDeg`, `SideOf`.
  - `RevitModelFacts`, `SketchNodeInfo` y `conn_get_node_info` devuelven el ángulo con signo; `node_info` añade
    `angle_to_chord_deg`, `side` (`+Y`/`-Y`), `chord_direction_reversed` y `frame_rule`; `conn_validate` añade
    `chord_direction_reversed` a `calculated_values`.
  - Regla 8.6 del validador: `expected_angle_deg` (del plano) se compara **sin signo** con la inclinación del modelo
    (45° = 135° = −45°); el mensaje muestra las dos inclinaciones.
  - Regla 8.9 (placa cuchilla dentro de la cartela): ahora comprueba la placa **donde está la barra de verdad** (ángulo con
    signo del modelo) y no donde dice el ángulo del plano, que siempre miraba el cuadrante +X +Y. Tolerancia nueva
    `plate_outside_gusset_tolerance_mm` (2 mm) en `config/limits.json` (entra en el hash del token): la placa del Detalle
    D termina justo en el chaflán inferior izquierdo y una esquina asoma 1,4 mm en los datos de prueba (0,2 mm en el Hangar).
- **Contrato y esquema**: `source.template_id` y `source.batch_id` (opcionales; se omiten al serializar si son nulos, así
  el token de las especificaciones de siempre no cambia por esto). Códigos nuevos: `TEMPLATE_NOT_FOUND`, `TEMPLATE_EXISTS`,
  `TEMPLATE_INVALID`, `TEMPLATE_SPEC_INVALID`, `TEMPLATE_HAS_OPEN_UNCERTAINTIES`, `TEMPLATE_NO_MATCH`,
  `CATALOG_FOLDER_UNAVAILABLE`; avisos `TEMPLATE_ANGLE_DEVIATION` y `TEMPLATE_PROFILE_DIFFERS`.
- **Core, `Catalog/`** (sin Revit):
  - `CatalogTemplate.cs`: el archivo de plantilla (sección 2.1 de la propuesta): `catalog_version`, `template_id`, `name`,
    `description`, `tags`, `created_utc`, `origin`, `chord_pattern`, `member_pattern` (por ranura: `slot`, `role`,
    `angle_deg` con signo, `side`, `profile`, `model_type_name`, `profile_policy`), `matching` (`angle_tolerance_deg`,
    `allow_mirror`) y `spec_template` (la especificación sin `node`, sin `element_id` y con `slot` por barra).
    `CatalogException` lleva el error accionable del sobre.
  - `CatalogConfig.cs`: `config/catalog.json` (carpeta del catálogo con `%LOCALAPPDATA%`, carpeta compartida, tolerancia
    10°, aviso de desvío 5°, espejo, política de perfil `warn`; `node_*` reservadas para la Fase 8). `ProfilePolicy`.
  - `TemplateNode.cs`: el nudo visto por el catálogo desde `IModelFacts`: cordón, marco canónico (el plano lo define la
    barra menos paralela al cordón; como el marco es canónico no importa cuál) y barras con dirección, ángulo con signo,
    lado y perfil. `ChooseChord` con la regla de `conn_get_node_info`.
  - `TemplateBuilder.cs`: especificación + nudo → plantilla: exige nombre, cordón y barras, dudas todas confirmadas (el
    valor confirmado queda escrito en su campo si estaba vacío), quita los IDs, escribe `slot` y el patrón, limpia
    `source.template_id`.
  - `TemplateMatcher.cs`: las cuatro orientaciones (`same`, `mirror_x`, `mirror_y`, `both`), transformación de ángulos y
    puntos, y asignación barra→ranura exhaustiva con poda (casar más ranuras, luego menor suma de desvíos, dentro de la
    tolerancia). Devuelve el mejor casado o el mejor intento (`IsComplete` falso) con `Describe()`.
  - `TemplateInstantiator.cs`: plantilla + casado → especificación con IDs reales en el orden de las ranuras, contorno
    transformado (y recorrido al revés en las reflexiones simples para conservar el giro), `expected_angle_deg` =
    inclinación real, perfil según `profile_policy`, `source.template_id` (y `batch_id` si se pasa), `uncertain_fields: []`.
  - `CatalogStore.cs`: la carpeta del catálogo: `List` (archivos ilegibles como avisos), `Get`, `FindByName`, `Save`,
    `Delete`, `CopyTo` (carpeta compartida); `template_id` validado contra `../`.
- **Add-in**:
  - `Catalog/CatalogConfigLoader.cs` (lee `config\catalog.json` desplegado y resuelve las carpetas) y
    `Catalog/CatalogService.cs` (construir desde `spec` o desde `connection_id`, guardar con control de nombre
    repetido, aplicar a un nudo y validar, datos para las respuestas).
  - `Services/ValidationService.cs`: la validación contra el modelo en un solo sitio (marco, hechos, validador, fallos del
    nudo como errores); `ValidateOperation` usa sus helpers de `calculated_values` y `bolt_stacks`.
  - Operaciones `catalog_list`, `catalog_get`, `catalog_save`, `catalog_delete`, `catalog_apply` registradas en `Bridge`.
    `catalog_apply` responde `ok` = la especificación valida, con `data.spec`, `data.validation_token`, `data.match` y, con
    `TEMPLATE_NO_MATCH`, `data.attempts` (un intento por orientación). `conn_list` añade `template_id` por conexión (P13).
  - `Services/RibbonCreation.cs`: el código de crear desde la cinta, sacado de `RunSpecCommand` y compartido con el botón
    nuevo. `CatalogCommand.cs` + `UI/CatalogWindow.xaml(.cs)` (lista con búsqueda, **Aplicar a la selección**, **Guardar
    desde conexión…**, **Eliminar**, **Actualizar**; modo "elegir" para la previsualización) y `UI/SaveTemplateDialog.xaml(.cs)`
    (nombre, descripción, etiquetas, política de perfil, sustituir, copia a la carpeta compartida, conexión del modelo).
  - `PreviewSession` admite un **archivo virtual** (plantilla aplicada: `Recargar` desactivado, **Guardar JSON** escribe
    en `Documentos\MotorConexiones\`); `PreviewWindow` tiene **Abrir del catálogo** y **Guardar en catálogo**.
  - `App.cs`: tercer botón **Catálogo** en el panel.
- **MCP**: `mcp/revit_mcp/conexiones.py` (5 rutas `/conn/catalog/...`, 0.7.0), `mcp/tools/conn_tools.py` (5 herramientas
  con su manual, 0.7.0, 18 en total), `mcp/pruebas/simulador_revit.py` (imitación del catálogo, 20 rutas, 36
  comprobaciones), `mcp/pruebas/probar_conexiones.py` (pruebas 18 a 23 del catálogo; 25 con `--puente`),
  `mcp/CONTRATO-conn.md` (rutas, códigos, marco canónico).
- **Configuración, scripts y documentación**: `config/catalog.json`; `config/limits.json` (`plate_outside_gusset_tolerance_mm`);
  `catalog/LEEME.md` (plantillas oficiales, P2); `scripts/deploy.ps1` copia `config\catalog.json` y las plantillas de
  `catalog\` que falten; `scripts/conn-call.ps1` (comentario); `docs/guide.md` (sección 5 "Catálogo de plantillas", marco
  canónico en la 3); README (estado, árbol, cuentas, sección 11 nueva, errores); `CLAUDE.md` (estructura);
  `docs/prompts/fase-7.md`; este informe y `docs/instalacion/fase-7.md`.
- **Pruebas**: `CatalogTests.cs` (14) y `NodeFrameTests` ampliadas (canónico, cordón al revés, nudo del Hangar, ángulos con
  signo e inclinación); `SketchBuilderTests`, `SpecEditorTests` y `FakeModelFacts` actualizados al marco canónico. De 99 a
  **129**.

---

## 2. Qué se probó en la nube y cómo

El SDK de .NET 10 (10.0.112) se instaló por `apt` y NuGet sirvió los paquetes de la API 2027, como en la Fase 6.

```text
$ dotnet build MotorConexiones.sln -c Release --nologo
  MotorConexiones.Core -> src/MotorConexiones.Core/bin/Release/netstandard2.0/MotorConexiones.Core.dll
  MotorConexiones.Tests -> src/MotorConexiones.Tests/bin/Release/net10.0/MotorConexiones.Tests.dll
  MotorConexiones.Revit -> src/MotorConexiones.Revit/bin/Release/net10.0-windows/MotorConexiones.Revit.dll
Build succeeded.
    0 Warning(s)
    0 Error(s)

$ dotnet test MotorConexiones.sln -c Release --no-build --nologo
Passed!  - Failed:     0, Passed:   129, Skipped:     0, Total:   129, Duration: 412 ms - MotorConexiones.Tests.dll (net10.0)

$ python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py   # correcto
$ python3 mcp/pruebas/simulador_revit.py --autocomprobar
  [OK] 20 rutas registradas  registradas: 20
  ...
  [OK] POST /conn/catalog/save/ con el fixture -> template_id
  [OK] POST /conn/catalog/apply/ al mismo nudo -> ok, token y spec con template_id
  [OK] POST /conn/catalog/delete/ -> ok
  [OK] GET /conn/catalog/get/<borrada> -> TEMPLATE_NOT_FOUND (sin documento también responde)
Autocomprobación: 36/36 correctas

$ python3 mcp/pruebas/simulador_revit.py &  &&  python3 mcp/pruebas/probar_conexiones.py
18. GET /conn/catalog/list/  [OK]  HTTP 200, ok=True, plantillas=0 ...
19. POST /conn/catalog/save/ desde el fixture -> template_id  [OK]  ... barras=3 ...
20. GET /conn/catalog/get/<id> -> plantilla sin element_id y con slot  [OK]  ... patrón=[(0, 43.1, '+Y'), (1, 135.6, '+Y'), (2, 45.0, '+Y')]
21. POST /conn/catalog/apply/ al mismo nudo -> ok, token, orientación same  [OK]  ... orientación=same desvío_máx=0.0 token=67afdfd289a9...
22. POST /conn/catalog/apply/ plantilla inexistente -> TEMPLATE_NOT_FOUND  [OK]
23. POST /conn/catalog/delete/ -> borrada  [OK]
Resultado: 23/23 pruebas correctas
```

(El simulador imita el catálogo de forma deliberadamente simple, sin espejos: prueba el adaptador, las herramientas y el
script, no la lógica del Core, que la prueban las xUnit.)

Lo que comprueban las pruebas nuevas del Core, con `docs/fixtures/detalle-D-confirmado.json` y los hechos del nudo del
Hangar (`FakeModelFacts`, ahora con ángulos con signo: 45°, 90° y −135°):

| Prueba | Qué comprueba |
|---|---|
| `NodeFrameTests` (canónico) | Cordón hacia +X y hacia −X dan el mismo marco (`ChordReversed` distingue); el nudo real del Hangar pasa a X = +X global, Y = +Z, Z = −Y; ángulos con signo por cuadrante; inclinación (135 → 45, −135,6 → 44,4, 370 → 10); diferencia por el camino corto |
| `TemplateNode_FromTheHangarFacts_HasCanonicalSignedAngles` | 45 / 90 / −135, lados `+Y`, `+Y`, `-Y`, +Y hacia arriba, `ChooseChord` elige el cordón, ID inexistente → `ELEMENT_NOT_FOUND` |
| `Build_RemovesIdsWritesThePatternAndRoundTripsThroughJson` | Sin `element_id` ni `node`, con `slot`; patrón 45 / 90 / −135; etiquetas limpias; los valores confirmados de las dudas quedan en sus campos; el archivo se relee igual; `FromJson` devuelve nulo con basura |
| `Build_WithOpenUncertainties_IsRejected` | `detalle-D.json` (dudas abiertas) → `TEMPLATE_HAS_OPEN_UNCERTAINTIES`; sin nombre → `INVALID_REQUEST` |
| `RoundTrip_SameNode_InstantiatesTheSameSpecAndValidatesWithAToken` | Orientación `same`, desvíos 0, mismos IDs y contorno, `expected_angle_deg` 45 / 90 / 45, `source.template_id`, sin `slot`; el validador da token y el esquema acepta el JSON |
| `MirroredNodes_AreMatchedInTheRightOrientationAndTheOutlineFlips` | Nudo reflejado en X, en Z y en los dos → `mirror_x`, `mirror_y`, `both`; la placa cuchilla sigue en la diagonal inferior; el contorno se refleja y la especificación valida contra el nudo reflejado |
| `MirroredNode_WithAllowMirrorFalse_DoesNotMatch` | Sin espejo no casa; forzando `mirror_x` sí |
| `NodeWithAMissingMember_HasNoMatchAndInstantiationExplainsIt` | 4 intentos, 2 de 3 ranuras, `TEMPLATE_NO_MATCH` con "ranura 1" |
| `ExtraMember_IsReportedAsUnassigned_AndASlightlyRotatedOneGetsADeviationWarning` | Barra sobrante en `unassigned_members`; 7° → `TEMPLATE_ANGLE_DEVIATION` (y `expected_angle_deg` 52); 25° → sin encaje |
| `ProfilePolicy_WarnWritesTheModelProfile_RequireKeepsTheTemplateOne` | `warn` → aviso y perfil del modelo (valida); `require` → perfil de la plantilla y `PROFILE_MISMATCH`; `ignore` → sin aviso |
| `Store_SavesListsGetsFindsAndDeletesTemplates` | Carpeta temporal: guardar, listar (archivo roto → aviso `TEMPLATE_INVALID`), leer, buscar por nombre, `../` rechazado, copiar a la compartida, borrar |
| `Config_LoadsTheRepoFileAndFallsBackOnBadValues` | `config/catalog.json` del repositorio; valores malos → por defecto |
| `Schema_AcceptsSourceTemplateIdAndBatchId_AndRejectsOtherSourceKeys` | `source.template_id` y `batch_id` pasan; `source.catalog` no; sin `template_id` el JSON serializa como antes |
| `Validator_PlateRule_UsesTheRealMemberAngleWithTheConfiguredTolerance` | Con 2 mm el fixture valida; con 0 mm `PLATE_OUTSIDE_GUSSET` "barra a −135°"; sin modelo se usa el ángulo escrito |
| `Validator_AngleRule_ComparesTheInclinationNotTheSign` | 45° del plano frente a −135° del modelo: sin aviso; 60° → aviso con las dos inclinaciones |

### 2.1 PENDIENTE DE INSTALADOR (se prueba en Revit con `docs/instalacion/fase-7.md`)

| Qué | Paso |
|---|---|
| `ping` lista las 5 operaciones nuevas; `instalar-conn.ps1` copia 20 rutas y 18 herramientas | 7-2, 7-3 |
| El marco canónico sobre el nudo real: `x_axis [1,0,0]`, `chord_direction_reversed: true`, ángulos 136,9 / 44,4 / −135,6 | 7-3 |
| El croquis y el nudo creado del Detalle D salen como en el plano (placa cuchilla en el chaflán inferior izquierdo), es decir, reflejados respecto a `fase6-05-nudo.png`; cara de la placa cuchilla | 7-4 |
| `catalog_save` desde la conexión creada: archivo en la carpeta del usuario y copia en `catalog\`; patrón con los ángulos reales | 7-5 |
| `catalog_apply` al mismo nudo: `same`, desvío 0, token, sin avisos de ángulo | 7-5 |
| Botón **Catálogo**: aplicar a la selección, ventana con archivo virtual, Guardar JSON en Documentos, Crear, borrar | 7-6 |
| La misma plantilla en el nudo simétrico: `mirror_x`, cartela reflejada, validación | 7-7 |
| `probar_conexiones.py --puente` 25/25 (crea y borra la plantilla de prueba) | 7-8 |
| Sondeos 12 y 13 en cero; solo queda la plantilla oficial | 7-9 |

### 2.2 NO PROBADO en la nube y por qué

- **Todo lo de Revit**: lectura del nudo real, Extensible Storage de la conexión guardada, escritura en
  `%LOCALAPPDATA%` y en `catalog\` del repositorio desde el add-in, `Environment.ExpandEnvironmentVariables` con
  `%LOCALAPPDATA%` en Windows (en Linux las pruebas usan carpetas temporales), las ventanas WPF nuevas (`CatalogWindow`,
  `SaveTemplateDialog`, los botones de `PreviewWindow`) y la selección de Revit en **Aplicar a la selección**. Compila,
  no se ha ejecutado.
- **El efecto físico del marco canónico en el Hangar** (sección 4.1): está calculado con las coordenadas de
  `resultados-fase-3.md` y probado en `NodeFrameTests`, pero la cartela reflejada y la cara de la placa cuchilla solo se
  ven en Revit.
- **Un nudo simétrico real**: las pruebas reflejan las barras del Detalle D; en el Hangar las diagonales del otro vano
  pueden llegar con otros ángulos (`TEMPLATE_ANGLE_DEVIATION`) o dejar la placa fuera de la cartela (`PLATE_OUTSIDE_GUSSET`).
  Es el caso que la propuesta (3.6) deja para corregir a mano o para la cartela automática (Fase 10).
- **El puente MCP real** con las 18 herramientas (`--puente`): en la nube solo se probó contra el simulador.

---

## 3. Qué debo mirar yo en Revit cuando el instalador termine

1. **`7-3 node_info`**: `x_axis` ≈ `[1, 0, 0]` y `chord_direction_reversed: true` (hasta la Fase 6 era `[-1, 0, 0]`). Los
   ángulos: 1249630 → 136,9°, 1249631 → 44,4°, 1249636 → −135,6°; `angle_to_chord_deg` 43,1 / 44,4 / 44,4. Si salen con
   otro signo, el marco no es el esperado: dímelo con el bloque entero.
2. **Captura `fase7-02-ventana.png`** frente al plano `detalle-D.png`: la diagonal con placa cuchilla abajo a la
   **izquierda**, terminando en el chaflán pequeño (el borde inclinado largo queda a la derecha, sin barra). Las cotas
   son las de siempre (565 × 530, retiros 180 / 60 / 260, placa 170 × 140, pernos 60 / 40 / 40). Debe decir
   `Validación correcta con 2 aviso(s)`: los dos avisos de ángulo siguen (1249630: 45° frente a 43,1°; 1249631: 90° del
   plano frente a 44,4° del modelo, porque en el Hangar ese "montante" es una diagonal).
3. **Captura `fase7-03-nudo.png`** frente a `fase6-05-nudo.png`: la cartela debe salir **reflejada** y la placa cuchilla
   terminar en el chaflán, no atravesar el borde inclinado (que es lo que pasaba, sin que nadie lo midiera, desde la Fase
   5: sección 4.1). Mira también de canto en qué cara apoya la placa cuchilla: con el marco canónico `+z` es la otra cara.
   Si prefieres la cara de la ronda 6d, basta `"gusset_face": "-z"` en el fixture.
4. **`7-5 catalog_save`**: `member_pattern` con 136,9 / +Y, 44,4 / +Y, −135,6 / −Y, `file` en tu carpeta de usuario y
   `shared_file` en `catalog\` del repositorio (así la plantilla "oficial" se sube con el commit del instalador).
5. **`7-5 catalog_apply`**: `orientation: "same"`, `max_deviation_deg: 0`, `is_valid: true`, sin avisos de ángulo (el ángulo
   real ya va en `expected_angle_deg`) y token nuevo (lleva `source.template_id`).
6. **Capturas `fase7-04` y `fase7-05`**: la ventana del catálogo con la plantilla y la previsualización con la cabecera
   `Plantilla '...' aplicada al nudo del cordón 1249510 (same)` y Recargar en gris.
7. **Paso 7-7, el nudo simétrico**: la orientación (`mirror_x`), el croquis con la cartela reflejada y la placa cuchilla en
   la diagonal inferior, y si validó o qué error dio. Es la prueba de verdad de la fase: lo demás es repetir el mismo nudo.

---

## 4. Decisiones tomadas y por qué

### 4.1 El marco canónico cambia la geometría creada del Detalle D en el Hangar, y la deja como el plano

La propuesta decía que P5 "cambia lo que muestra `conn_get_node_info` y el croquis, no la geometría creada". No es exacto
y hay que decirlo: el contorno de la cartela (`outline.points_mm`) está en coordenadas locales, así que si el marco
cambia, la misma especificación produce otra cartela. En el Hangar el cordón 1249510 está dibujado hacia −X global
(`resultados-fase-3.md`: `frame_x = [-1, 0, 0]`), y con el marco canónico X pasa a +X global: el dibujo local se refleja en
X y Z pasa de +Y global a −Y global (Y ya apuntaba hacia arriba). Consecuencias para el fixture `detalle-D-confirmado.json`:

- La cartela se crea **reflejada en X** respecto a las Fases 5 y 6. Según el plano, la nueva es la correcta: en
  `detalle-D.png` la diagonal inferior con placa cuchilla baja hacia la **izquierda** y termina en el chaflán pequeño
  (125 × 135) perpendicular a ella; en el Hangar esa diagonal (1249636) va hacia −X global y hacia abajo, es decir, hacia
  −X local en el marco canónico: coincide. Con el marco antiguo la barra caía en +X local, donde el contorno tiene el borde
  inclinado largo (350 × 210), y la placa cuchilla lo atravesaba unos 45 mm (se ve en `fase6-05-nudo.png`, la pieza
  pequeña sobre el borde inclinado). Nadie lo midió porque la regla 8.9 comprobaba la placa con el ángulo del plano, en
  el cuadrante +X +Y, y no donde estaba la barra. La ronda 7 lo verá en la captura `fase7-03`.
- `plate.gusset_face` `+z` pasa a ser la cara −Y global (antes +Y). El plano no dice la cara; si se prefiere la de la
  ronda 6d, `"gusset_face": "-z"`.
- El token del fixture no cambia por el marco (no entra en él) pero sí por la clave nueva de `limits.json` (sección 4.3).

No se ha tocado el fixture ni se ha "migrado" nada: la especificación es la misma, se interpreta en el marco canónico.

### 4.2 Ángulo con signo en el modelo, inclinación sin signo en el contrato

`member_pattern.angle_deg` y `angle_in_plane_deg` van con signo desde +X en [−180°, 180°): es lo que distingue "arriba a
la derecha" de "abajo a la izquierda" y lo que hace posible casar barras y detectar espejos. `expected_angle_deg` sigue
siendo lo que escribe un plano (una inclinación, 45°), y la regla 8.6 compara las inclinaciones: así el P3 de la Fase 6
(45° del plano frente a 135° del modelo) deja de avisar, y 45° frente a 43,1° sigue avisando. El instanciador escribe en
`expected_angle_deg` la inclinación real (no el ángulo con signo) para que una especificación instanciada se lea como una
hecha a mano y la regla 8.9 se comporte igual con y sin modelo.

### 4.3 La regla 8.9 comprueba la placa donde está la barra, con una tolerancia de 2 mm

Era necesario para el catálogo: con una plantilla aplicada en espejo, la comprobación en el cuadrante +X +Y daba
`PLATE_OUTSIDE_GUSSET` aunque la placa estuviera dentro. Ahora, con modelo, se usa el ángulo con signo real; sin modelo,
el ángulo escrito. Con el ángulo real, la placa cuchilla del Detalle D asoma 1,4 mm por el chaflán (0,2 mm con los
ángulos del Hangar): el plano la hace terminar justo en el borde. De ahí `plate_outside_gusset_tolerance_mm: 2.0`,
editable en `limits.json`, que entra en el hash de los límites (el token del fixture cambia; el instalador lo anota).
Encoger la cartela (530 → 500 mm en la ventana) ahora deja la placa fuera y lo dice: antes no lo detectaba.

### 4.4 Formato de plantilla: la especificación sin IDs más el patrón

Se siguió la sección 2.1 de la propuesta tal cual, con tres añadidos: `model_type_name` por ranura (el nombre del tipo del
modelo de origen, por si el rótulo del plano y el tipo no coinciden), `origin.element_ids` (para volver al nudo de origen)
y `uncertain_fields` siempre vacío con los valores confirmados escritos en sus campos (una plantilla no puede arrastrar
preguntas, hecho 6 de la propuesta). `template_id` es el nombre del archivo y se valida contra `../`.

### 4.5 Espejo: cuatro orientaciones, el contorno recorrido al revés

Una reflexión invierte el sentido de giro del polígono; el instanciador recorre la lista al revés en `mirror_x` y
`mirror_y` (en `both` el giro se conserva) para que el contorno tenga siempre el mismo sentido que el original.
`Polygon2D` no depende del sentido, pero Advance Steel recibe la misma orientación de siempre. `plate.gusset_face` se
copia tal cual: el plano decide la cara, no la orientación del nudo.

### 4.6 Perfil distinto: `warn` por defecto (P3)

`warn` escribe el perfil del modelo y avisa (`TEMPLATE_PROFILE_DIFFERS`), `require` conserva el de la plantilla (y
`conn_validate` da `PROFILE_MISMATCH`, como hoy), `ignore` escribe el del modelo sin avisar. La política se guarda por
ranura y para el cordón; la de `config/catalog.json` es la que se usa al guardar si no se indica otra.

### 4.7 Un solo sitio para validar contra el modelo y para crear desde la cinta

`ValidationService` reúne lo que hacían por separado `ValidateOperation` y `PreviewSession` (marco, hechos, validador,
fallos del nudo como errores) y lo usan `catalog_save`, `catalog_apply` y las ventanas. `RibbonCreation` es el código de
crear que tenía `RunSpecCommand`, compartido ahora con el botón Catálogo (el `OperationScope`, la adopción de elementos y
el diálogo final son los mismos).

### 4.8 La plantilla aplicada es un "archivo virtual" en la ventana

`PreviewSession` admite `isVirtualFile`: **Recargar** se desactiva y **Guardar JSON** escribe en
`Documentos\MotorConexiones\<plantilla>-nudo-<cordón>.json` (lo decía la propuesta, 2.4). Así la ventana de la Fase 6 se
reutiliza sin cambios de fondo, y lo que se crea pasa por el mismo camino que un JSON abierto del disco.

---

## 5. Pendientes, riesgos y preguntas

Decisiones tomadas con la persona en el chat (2026-10-01), antes de la ronda del instalador:

- **P1 (riesgo principal), la cartela reflejada en el Hangar (4.1). Decisión: se deja la orientación nueva y no se toca
  el fixture.** Según el plano, la placa cuchilla debe terminar en el chaflán pequeño, que es lo que da el marco canónico;
  lo de la Fase 6 era un defecto sin medir. Se confirma con la captura `fase7-03-nudo.png`; solo si el plano estuviera
  "visto desde el otro lado" se reflejaría el contorno (x → −x), sin tocar código.
- **P2, la cara de la placa cuchilla (`gusset_face`) cambia de lado físico con el marco canónico. Decisión: se deja `+z`
  y se decide viendo el modelo** (vista de canto del paso 7-4). Si se prefiere la cara de la ronda 6d, es una sola clave en
  el fixture (`"gusset_face": "-z"`), sin recompilar.
- **P3, el "montante" del fixture (1249631, rol `vertical`, 90°) es en el Hangar una diagonal a 44°; por eso sigue el
  aviso de ángulo. Decisión: cambiarlo a `diagonal` 45° en una ronda corta**, después de los resultados del instalador
  (desaparece el aviso falso y la plantilla queda fiel al nudo real; la lectura del plano sigue documentada en
  `detalle-D.png` y en el encargo).
- **P4, tolerancias de casado. Decisión: se dejan 10° para casar (`angle_tolerance_deg`) y 5° para avisar
  (`angle_deviation_warning_deg`)**, los valores de la propuesta. Si en el paso 7-7 el nudo simétrico no casa
  (`TEMPLATE_NO_MATCH`), se sube la tolerancia **en esa plantilla** (`matching.angle_tolerance_deg` en su archivo JSON), no
  la global de `config\catalog.json`.
- **P5, `probar_conexiones.py` escribe y borra una plantilla real en la carpeta del catálogo del PC (pruebas 19 a 23).
  Decisión: se deja así** (crea, comprueba y borra en la misma ejecución). Una opción `--sin-catalogo` solo tendría sentido
  con una carpeta de catálogo compartida en red.
- **Ronda 6d**: hacerla en la misma sesión de Revit que la 7, porque el sondeo 16 mide el nudo que se crea en el paso 7-4.
- **P6**: la ronda 6d (placas centradas) sigue pendiente de instalador; esta fase no la cambia. Conviene hacerla en la
  misma sesión de Revit que la 7 (el sondeo 16 mide el nudo creado en 7-4).
- **Pendientes anteriores que siguen**: P2 de la Fase 5 (`UNKNOWN_CONNECTION_TYPE` para `conn_get_schema`), P3 de la
  Fase 5 (Claude Desktop NO PROBADO), soldaduras nativas y `BoltPattern.Connect` para v2, traspaso de `mcp/` a `revit-mcp`,
  P9 de la ronda 6b (`BOLT_INSIDE_MEMBER_SLOT`).
- **Siguientes fases**: 8 (detección de nudos y plan con marcas en el modelo) y 9 (crear por lotes) según la propuesta;
  `source.batch_id` y las claves `node_*` de `config/catalog.json` ya quedan reservadas.

---

## 6. Qué hace la persona, en orden, para validar lo hecho antes de seguir programando

1. **Llevar la Fase 7 a `main`** (en el PC, en PowerShell):

   ```powershell
   cd "D:\Proyectos C#\CONEXIONES"
   git fetch origin
   git checkout main
   git pull --no-rebase origin main
   git merge origin/claude/fervent-allen-bxmdt1
   git push origin main
   ```

   Si `git merge` dice que hay conflictos, no toques nada: pégame la salida.

2. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-7.md` entero. Dentro de esa misma sesión de Revit, después
   del paso 7-4 (el Detalle D recién creado) y antes del 7-5 (que lo borra), pedirle también la medición de la ronda 6d
   (`docs\instalacion\fase-6d.md`, paso 6d-3, sondeo 16) sobre ese nudo.

3. **Mirar en Revit lo de la sección 3 de este informe**, en este orden: los ejes y ángulos de `7-3 node_info`; el croquis
   (`fase7-02`) y el nudo creado (`fase7-03`) frente al plano `detalle-D.png`; la salida de `catalog_save` y
   `catalog_apply`; la ventana del catálogo; el nudo simétrico del paso 7-7 (orientación `mirror_x`, validación o error); el
   `25/25` del paso 7-8; los sondeos 12 y 13 en cero.

4. **Dejar constancia**: el instalador sube `docs\fases\resultados-fase-7.md`, las capturas y la plantilla de `catalog\`
   (paso 7-9). Añade al final de ese archivo tus anotaciones (lo que viste, lo que no coincide) y, si cambias de opinión
   sobre alguna decisión de la sección 5, escríbelo ahí.

5. **Abrir la sesión de cierre** (ronda 7b) sobre el repositorio, en `main` o en la rama de trabajo que elijas, con este
   prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-7.md (secciones 5 y 6), docs/fases/resultados-fase-7.md y docs/fases/resultados-fase-6d.md.
   Cierra la Fase 7 con una ronda corta 7b: corrige lo que digan los resultados, cambia el montante del fixture a diagonal 45°
   (P3), actualiza el informe y la tabla de garantías del README. No empieces la Fase 8.
   Termina con el informe actualizado, docs/instalacion/fase-7b.md si hace falta otra ronda, commit, push y un resumen corto.
   ```

6. **Solo con la 7b cerrada**, lanzar la Fase 8 con una sesión nueva: su prompt se escribe a partir de la sección 3 de la
   propuesta (`docs/prompts/fase-8.md`), como se hizo con la 7:

   ```
   Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-7.md y docs/propuestas/catalogo-y-lotes.md completo.
   Escribe docs/prompts/fase-8.md (detección de nudos y plan: secciones 3.1, 3.2, 3.4, 4 y 6 de la propuesta, con las decisiones de 7.1)
   y ejecuta SOLO la Fase 8. Termina con docs/fases/fase-8.md, docs/instalacion/fase-8.md, commit, push y un resumen corto.
   ```
