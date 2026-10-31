# MotorConexiones

Add-in de Autodesk Revit 2027 en C# y herramientas MCP (`conn_*`) para crear conexiones de acero (cartelas,
placas cuchilla, pernos, soldaduras y retiros de barras) a partir de una especificación JSON leída de un plano.
Lo maneja una IA a través del servidor MCP `revit-mcp` (repositorio aparte, Python + pyRevit).

Estado (2026-10-06): **Fases 0 a 9 probadas en el PC con el add-in 0.9.0; la Fase 9 (crear por lotes) quedó cerrada el 2026-10-06 (`docs/fases/fase-9.md`, sección 7; `resultados-fase-9.md`).** Fases 0 a 7:
punta a punta desde la IA, placas y pernos de Advance Steel con las medidas del contrato, ventana de previsualización 2D
con cotas, borrado desde la cinta, panel en la pestaña ARBA, pernos con agarre real y placas centradas en su plano (rondas
6b, 6c y 6d) y el catálogo de conexiones (sección 11; ronda 7b cerrada en `docs/fases/fase-7.md`, secciones 7 y 7.9).
**Fase 8 cerrada** (`docs/fases/fase-8.md`, sección 8): detección de nudos de una cercha entera y plan de lote con marcas en
el modelo, desde la IA (`conn_batch_plan`, `conn_batch_plan_get`, `conn_batch_plan_discard`) y desde la cinta (botón
**Planificar lote**), sin crear nada (sección 12). En la ronda 8b el PC confirmó la corrección del corte de los ejes: 56
barras de la cercha del Hangar → 59 nudos con **16 `ready`** con la plantilla oficial (8 como el Detalle D y 8 en espejo),
replan con el mismo token y **Editar nudo** sobre un nudo listo (`resultados-fase-8b.md`). El cierre (0.8.2) corrigió cinco
cosas pequeñas (`end_gap_mm` en la respuesta, un solo plan marcado por documento, el conteo de `discard all`, `overrides`
sin `IsEmpty`, el paso 9 del sondeo 17). **Ronda 8c** (`fase-8.md`, sección 9; sale de `docs/propuestas/flujo-intuitivo.md`):
la ventana del plan se entiende sin leer este README: cabecera con la decisión ("Se crearán 16 conexiones con … (8 iguales,
8 en espejo). 14 avisan de perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas."), **mapa de la cercha** con un
círculo por nudo del color de su estado, solo los nudos de verdad en la tabla (barras sueltas y parejas sin cordón ocultas
con contador), estados en español, columna **Qué hacer**, cuatro botones y un menú de clic derecho; en el modelo las
marcas van **por estado** (verde, ámbar, rojo, gris); la IA recibe `summary_text`, `status_text`, `advice` y
`visible_by_default` y la guía le dice que resuma en español sin volcar el JSON; con el catálogo vacío, aviso
`CATALOG_EMPTY` y botón **Abrir catálogo**; y el sondeo 19 prueba las etiquetas pinchables en el lienzo (para la Fase 10).
La ronda 8c **se probó en el PC** el 2026-10-05 (`resultados-fase-8c.md`: 64 barras → los mismos 16 listos del cordón
central, 10 nudos del cordón superior "sin plantilla que encaje" porque el Detalle D no es su típica, mapa y colores por
estado bien, `CATALOG_EMPTY`, `discard all` a cero y `--puente` 28/28). Su **cierre** (`fase-8.md`, sección 10) corrige
lo que la persona vio: la ventana del plan pasa a ser **no modal** (se queda abierta mientras orbitas y pinchas en Revit;
todo lo que toca el modelo va por un `ExternalEvent`), **Ver en Revit** hace zoom al nudo sin ningún cuadro, **Descartar**
desde la ventana quita también los cubos (los de cualquier plan) y el sondeo 19 se repite con BMP buscando el manejador de
clics por reflexión. La **ronda 8d** se probó en el PC el 2026-10-05 (`resultados-fase-8d.md`, 0.8.4): la ventana **se quedó
abierta** mientras la persona orbitaba y pinchaba, Descartar desde la ventana dejó 0 cubos con un plan del puente marcado en
otra vista, el catálogo vacío avisó con la ventana abierta y `--puente` 28/28; pero **Cordón… cerró Revit con "fatal error"**
(el diálogo de elección en modo de una sola elección lanzaba una excepción que, con la ventana no modal, nadie capturaba),
**Descartar plan no cerraba la ventana** y la etiqueta del sondeo 19 v2 **no se vio** aunque Revit aceptó el control. Su
**cierre** (`fase-8.md`, sección 11, add-in **0.8.5**) corrige el diálogo, mete cada manejador de la ventana en una red que
captura cualquier excepción (y una red en el despachador solo para las de este add-in), pincha en Revit con la ventana del
plan **oculta** y la ventana principal de Revit activada, con una línea en el log antes y otra después, cierra la ventana al
descartar y repite el sondeo 19 (v3: BMP de 24 bits de 32×32 en una ruta sin tildes, `SetVisibility`, refresco de la vista
y una segunda etiqueta en el centro de la caja de sección). La **ronda 8e** se probó en el PC el 2026-10-05
(`resultados-fase-8e.md`, 0.8.5; el puente se repitió el 2026-10-06 con Revit abierto): **Cordón… sobre N9 ya no cierra
Revit** (nueve pinchados con la ventana oculta y de vuelta, sin ningún error de ventana en el log), Descartar cierra la
ventana y deja 0 marcadores, **la etiqueta del sondeo 19 se vio y se pinchó** (capturas `fase8e-01-etiqueta` y
`fase8e-01-etiqueta-clic`), sondeos 17, 12 y 13 en cero y `--puente` **28/28**. Su cierre (`fase-8.md`, sección 12) no toca
el add-in: corrige el sondeo 19 (v4: la etiqueta B con `ElementId(Int64)` y el clic **sin cuadro**, porque el cuadro modal del
manejador dejaba a Revit sin atender a pyRevit) y da por decidida V3 (etiquetas pinchables) para la Fase 10. **La Fase 8
queda cerrada del todo.** **Fase 9** (`docs/fases/fase-9.md`, prompt en `docs/prompts/fase-9.md`, add-in **0.9.0**): el plan
se convierte en acero. `conn_batch_create` (y el botón **Crear N conexiones** de la ventana del plan) crea los nudos listos
**nudo a nudo con los tokens del plan**: cada nudo es una operación atómica propia (si uno falla se revierte solo y los demás
se quedan) anidada en un grupo del lote que se asimila al final (**una sola entrada de deshacer**; `batch_single_undo` en
`config/catalog.json` es el plan B). Informe por nudo (creada, creada con aviso, rehecha, fallida y por qué, saltada y por
qué), `conn_batch_delete` y **Borrar el lote** (una a una con las garantías de `conn_delete`), `conn_list` con `batch_id`,
la columna **Qué hacer** con botones (mejora C3) y el consejo del empalme del cordón ("El cordón termina en este nudo
(empalme)…"). Decisiones de la persona: los 14 nudos con cordón HSS4X4 se crean con la cartela del Detalle D y el aviso de
perfil (P6); los 10 nudos del cordón superior sin plantilla y los 7 empalmes quedan fuera (un `no_match` no se crea). Las
cartelas fantasma (V2) no entraron: siguen en `docs/propuestas/flujo-intuitivo.md`. Probado en la nube: 192 pruebas del
Core, simulador 62/62, `probar_conexiones.py` 28/28. **Probado en Revit el 2026-10-06** (`resultados-fase-9.md`; `fase-9.md`,
sección 7): el sondeo 20 dio "GRUPOS ANIDADOS OK", **Crear 16 conexiones** creó las 16 dos veces (2,6 s y 2,4 s, sin fallos),
un solo Ctrl+Z deshizo el lote entero, el lote por el puente (crear 2, saltar, token alterado, sin token, borrar con 6 barras
restauradas) y el sondeo 19 v4 (las dos etiquetas pinchadas sin cuadro, en naranja) salieron como se esperaba. Sin probar
todavía: **Borrar el lote desde la ventana** (el segundo lote se descartó y lo borró el sondeo 12) y `--puente` 30/30 (se
cayó en la 22 por la consola cp1252 de Windows; corregido): los dos en la instalación de la Fase 10 (Borrar el lote desde la
ventana, **hecho el 2026-10-07**: 16 borradas, 48 barras restauradas; el `--puente`, **32/32 el 2026-10-11**, ronda 10b). **Fase 10**
(`docs/fases/fase-10.md`, prompt en `docs/prompts/fase-10.md`, add-in **0.10.0**, **probada en Revit el 2026-10-07**; ronda 10b,
add-in **0.10.1**, **probada en Revit el 2026-10-11** y cerrada del todo, secciones 7 y 8 del informe): cercha sin dolor.
**Selección asistida** (pinchar **una** barra basta: el add-in añade las que la tocan con la regla del detector, por rondas,
dentro del plano de la cercha, y un cuadro ofrece Planificar con todas / Solo la selección / Cancelar; **Más… > Completar
selección** en la ventana; `expand_selection` en `conn_batch_plan`), **etiquetas pinchables** en la vista (una por nudo con su
número y el color de su estado, sobre el sondeo 19 v4; el clic **nunca abre un cuadro**: resalta la etiqueta, elige la fila en
la ventana del plan y lo escribe en su barra de estado; elegir una fila resalta su etiqueta), **cartelas fantasma** (V2: la
cartela de cada nudo listo, transparente y del color de su estado, antes de crearla; Crear la sustituye por acero), el botón
**Encargo para IA** (ronda 9b: un solo `.md` con el prompt, los datos del nudo, el esquema, las reglas de lectura y un ejemplo,
copiado al portapapeles) y el texto corregido del aviso del borrado. Probado en la nube: 217 pruebas del Core, simulador
66/66, `probar_conexiones.py` 30/30. Se prueba en el PC con `docs/instalacion/fase-10.md`, que incluye lo pendiente de la Fase 9.
**Fase 11** (`docs/fases/fase-11.md`, prompt en `docs/prompts/fase-11.md`, add-in **0.11.0**, programada y **probada en Revit
el 2026-10-12**, cerrada; sección 7 del informe): conectar un nudo **sin JSON**. El botón **Conectar** reconoce el nudo
seleccionado (el cordón y las barras que llegan; con una barra pide pinchar las demás), casa **todas** las plantillas del
catálogo y abre la previsualización con la mejor ya aplicada ("Plantilla propuesta: Nudo tipico Detalle D · igual · desvío
máx. 0,0°", **Cambiar…** para elegir otra); si ninguna encaja, una **especificación de arranque** con cartela rectangular,
unión soldada en ranura por barra y los valores de la plantilla del mismo cordón o de `config/choices.json`, con el botón
**Encargo para IA** a mano; si la selección es una cercha, manda a Planificar lote. En la previsualización, el **panel por
secciones** (Cartela, Cordón, Barra 1-2-3) sustituye a la tabla: desplegables de espesor (`3/8" · 9,5 mm`), diámetro de
perno (por defecto 1/2", 5/8", 3/4" y 7/8"), soldadura, tipo de unión (al cambiar a placa cuchilla aparecen placa y pernos
con los valores por defecto, y al volver se recupera la ranura de antes), unión al cordón, cara de la cartela y rol, que
escriben los mm y el rótulo a la vez; errores junto al campo; la tabla de siempre, el JSON (con Copiar), el contorno y las
dudas en la pestaña **Avanzado**. **V4**: miniaturas del croquis de cada plantilla (PNG de 240×180 junto al `.json`) en el
catálogo, en Abrir del catálogo, en Cambiar… y en el detalle del nudo del plan. **V5**: **Plano…** abre la imagen del detalle
(PNG o JPG, o un recorte pegado del portapapeles) a la izquierda del croquis, con zoom y encuadre; la ruta se recuerda en
`source.drawing_image` (clave nueva y opcional del contrato) y viaja con la plantilla. De paso: la línea del plan dice "(63
añadidas por la selección asistida)" también desde la cinta, el log `label_clicked` lleva `changed` y `highlighted`, los
títulos de los cuadros ya no repiten "MotorConexiones - ", y `conn_catalog_apply` admite **sin `template_id`** (la IA recibe
`data.proposal`). Probado en la nube: **238** pruebas del Core, simulador 69/69, `probar_conexiones.py` 31/31.
**Fase 12** (`docs/fases/fase-12.md`, prompt en `docs/prompts/fase-12.md`, add-in **0.12.0** programada el 2026-10-12 y **probada
en Revit el 2026-10-21** (`resultados-fase-12.md`; cierre en la sección 7); **ronda 12b** probada en Revit el 2026-10-21 y cerrada
el 2026-10-22 (sección 8); **ronda 12c** probada en Revit el 2026-10-23 y cerrada el mismo día (sección 9); **ronda 12d**
probada en Revit el 2026-10-24 y cerrada el mismo día (sección 10): el sondeo 29 tampoco encontró la viga de Advance Steel, así
que el retiro por el eje queda **definitivo**, y la advertencia de Revit resultó ser "solo visible en nivel de detalle Fino", que
el add-in **0.12.4** filtra del aviso; **la Fase 12 queda cerrada** y la 0.12.4 se despliega con la siguiente instalación):
placa de extremo y nudo soldado. **Excentricidad (opción A)**: el punto de trabajo de un nudo en K es el punto del eje del cordón a mitad entre
los cortes de las diagonales; el contrato suma `node.eccentricity_mm` y `members[].axis_offset_mm` (opcionales); hasta medio
canto del cordón el desvío es el aviso `MEMBER_ECCENTRIC` (por encima, `MEMBER_NOT_AT_NODE`); cada barra se recorta y se une
sobre su propio corte; `conn_get_node_info` devuelve `kind`, `eccentricity_mm`, `axis_offset_mm` y `axis_cut_mm`; el Detalle D
y los 16 nudos de la 8b conservan sus tokens. **Dos tipos nuevos**: `end_plate` (empalme con placa de extremo: dos planchas
octogonales, pernos alrededor del tubo por cara con tuerca a ambos lados, soldadas al tubo; Detalles **B**, **C** e **I** como
fixtures; arranque por perfil del cordón) y `direct_weld` (nudo en K soldado: cada diagonal cortada contra la cara del cordón y
soldada en todo el contorno, sin cartela; la marca del empalme de la diagonal a 330, `diagonal_splice_offset_mm`, se dibuja y se
escribe pero no se fabrica hasta la Fase 13). **Conectar y el lote** reconocen la clase `splice` (dos tramos colineales que se
encuentran), tratan un nudo en K sobre un empalme como dos nudos en el mismo punto (N9 y N9E, el empalme primero) y, sin
plantilla, escriben el arranque del tipo que toca; la cercha de la 8c da **41 listos** (16 Detalle D, 17 soldaduras directas, 7
empalmes C y 1 empalme I). La fabricación en Revit lleva sus reservas (`END_CUT_APPROXIMATED`, `BOLT_NUTS_APPROXIMATED`) hasta
que los sondeos digan otra cosa. Probado en la nube: **268** pruebas del Core, simulador 82/82, `probar_conexiones.py` 35/35.
**En Revit** (2026-10-21, `fase-12.md` 7.1): funcionó todo lo programado: Conectar en el nudo en K (Crear con 2 retiros y las
soldaduras, `END_CUT_APPROXIMATED`, guardado como "Nudo K cordon superior"), Conectar en el empalme (dos planchas octogonales de
Advance Steel, 4 patrones de N × 1 atravesándolas, `BOLT_NUTS_APPROXIMATED`), el lote de la 8c con **41 nudos y 40 creadas en 6,3 s
sin fallos**, el borrado y los sondeos a cero. El sondeo 23 confirmó la fila 4 × 1 y `Connect`; la tuerca doble no está en la API.
**Lo que no coincidió** (7.2): la excentricidad real es **54,9 mm** (no 34), el HSS12X8 se lee como 203 × 203 (límite 101,6 en vez
de 152,4; la plancha de 430 pudo salir girada), los sondeos 22 y 24 listaron `BeamShortening` y `WeldLine(Point3d[], Vector3d,
Vector3d)` sin intentarlos, y `--puente` 36/38 porque `validate` no devolvía `kind`. **Ronda 12b (0.12.1, en Revit el
2026-10-21, `fase-12.md` 8)**: `validate` con `kind` y `eccentricity_mm` (puente **38/38**), 54,9 en fixture, pruebas (`KNode55`),
simulador y guía; el **sondeo 27** dijo que el tipo HSS12X8X1/2 guarda `Ht` = 304,8 y `b` = 203,2 y que el ejemplar va girado 90°
(203,2 en el plano de la cercha, 304,8 perpendicular); el **26**, que `WeldLine(Point3d[], Vector3d, Vector3d)` **se escribe**
conectada a una plancha; el **25**, que la viga de Advance Steel de la barra de Revit **no se encuentra** buscando solo en
ASObjectsMgd (el corte inclinado sigue sin probar); y se vieron una sola barra, media cercha, el cuadro Sí/No y el borde a 10.
**Ronda 12c (0.12.2)**: `RevitModelFacts` lee el perfil con `Ht`, `b` y el giro (`width_mm` = lado en el plano, `height_mm` =
perpendicular; `section_rotation_deg` en `node_info`; el límite de la excentricidad pasa a 152,4 y la huella del empalme a
305 × 203), `CreateWelds` escribe **`WeldLine`** de Advance Steel (cadenas de segmentos, `Connect` a las planchas de la sesión,
reserva DirectShape por recuento si no se materializan), y el **sondeo 28** busca la viga en todos los ensamblados, corta con
`BeamShortening` y escribe una `WeldLine` sin plancha. Probado en la nube: **291** pruebas del Core, simulador 82/82,
`probar_conexiones.py` 35/35. **En Revit** (2026-10-23, `resultados-fase-12c.md`; `fase-12.md` 9.1): el perfil se lee 203,2 ×
304,8 con giro 90 y el límite pasa a 152,4; las **`WeldLine` se materializan** en el nudo en K (2 elementos, `missing 0`) y en el
empalme (8: 2 planchas, 4 patrones, 2 soldaduras; el 430 en horizontal); puente 38/38. El **sondeo 28 no encontró la viga** de
Advance Steel, pero no llegó a recorrer los objetos del modelo (saltaba `GetModelObjectIds` por su parámetro de salida), y Revit
devolvió un `REVIT_WARNING` por cada elemento de Advance Steel sin que el texto quedara en ningún sitio. **Ronda 12d (0.12.3)**:
cada advertencia de Revit al confirmar queda en el log (`revit_failure`: severidad, texto, elementos) y el aviso dice a qué
elementos apunta; el **sondeo 29** recorre los objetos del modelo con `DatabaseManager.GetModelObjectIds`, busca la viga por Guid
o por geometría y corta con `BeamShortening` si la encuentra: es el último intento del corte inclinado. **Cierre de la 12d
(2026-10-24, add-in 0.12.4)**: el sondeo 29 listó 5 objetos de Advance Steel, no abrió ninguno y no vio vigas, así que el retiro
por el eje queda como **definitivo** (`END_CUT_APPROXIMATED` se queda y no hay más sondeos sobre la viga); la advertencia de Revit
es "The created elements are only visible in Detail Level: Fine." (una por soldadura) y la 0.12.4 la filtra del aviso
`REVIT_WARNING` (queda en el log con `filtered: true`). Las soldaduras de Advance
Steel se ven en Revit como símbolos sin volumen y solo en nivel de detalle Fino. Probado en la nube: 291 pruebas, simulador
82/82, `probar_conexiones.py` 35/35.

**Fase 13** (`docs/fases/fase-13.md`, prompt en `docs/prompts/fase-13.md`, add-in **0.13.0** programada el 2026-10-24, **probada en
Revit el 2026-10-31** (`resultados-fase-13.md`) y **cerrada el mismo día con el add-in 0.13.1** (sección 7; la 0.13.1 se despliega con
`docs/instalacion/fase-13b.md` y está **NO PROBADA en Revit**)): **cartela automática** y **partir la diagonal**. `gusset.outline.mode: "auto"` calcula el contorno de la cartela desde las barras (rectángulo
por barra hasta el final de la ranura o la placa más 40 mm, 30 mm a cada lado, franja del cordón más 40 mm, envolvente convexa
redondeada a 5 mm con 12 vértices como mucho; `OUTLINE_AUTO_LARGE` si sale grande); lo mismo se valida, se dibuja y se fabrica, y
`conn_validate` / `conn_preview` / `conn_catalog_apply` devuelven `calculated_values.outline_points_mm`. Conectar arranca con `auto`
en vez del rectángulo; el panel tiene **Contorno: Automático / Polígono** y **Convertir en polígono**; una plantilla guardada desde
`auto` casa con 25° (`auto_angle_tolerance_deg`) y se recalcula al aplicarla; el plan arranca con `auto` los nudos con cartela sin
plantilla ("● Listo (arranque)"). El camino `polygon` no cambia (21 tokens de la 0.12.4 iguales). **Partir la diagonal**: `SplitPoint`
(la cara del cordón a (ancho/2)/|sen θ|, +330 hasta la primera plancha del Detalle B, corte en el centro de la pila: ≈ 485 mm en el
nudo en K del Hangar; `conn_get_node_info` lo devuelve por diagonal), `conn_split_member` (`/conn/split/`, con `dry_run`) y
`conn_unsplit` (`/conn/unsplit/`), el botón **Partir barra**, el registro en Extensible Storage (esquema propio) y el deshacer
(`SPLIT_IN_USE` mientras una conexión use los tramos; `conn_delete` del nudo que pidió el corte lo deshace). El tramo corto conserva
el id. **Conectar** en un nudo en K con `splice_hint` enseña la casilla "Partir la diagonal y crear el empalme del Detalle B a 330":
Crear parte, crea el empalme B y el nudo en K de un clic, todo o nada. El **plan** lleva los pasos `split` y los nudos `N…B`
(`after_split`); el **lote** los ejecuta en orden (cortes → empalmes B → N9E → N9), renueva el token del nudo en K tras el corte y
**Borrar el lote** deshace los cortes al final. `MemberSplitter` lleva `CopyElement` + `LocationCurve.Curve` con reserva
`NewFamilyInstance`. **En Revit el 2026-10-31** (todo a la primera): el filtro de la 12d (solo `END_CUT_APPROXIMATED`, dos
`revit_failure` filtradas), la cartela automática en un nudo del cordón central (7 elementos, `outline.mode auto` sin puntos) y su
plantilla propuesta en espejo en el nudo vecino, la cadena de Conectar en el nudo en K (1249631 partida a 485 mm y 1249632 a 483,
dos empalmes B de 8 elementos y el nudo en K de un clic; `SPLIT_IN_USE` al borrar el nudo en K antes que los empalmes; `unsplit`
`complete: true`), `conn_split_member` / `conn_unsplit` (`copy_element`, sin parámetros aproximados), el plan de la cercha con **31**
cortes y 31 empalmes B previstos, y `--puente` 49/49. El sondeo 30 paró por un `ElementId` ambiguo (v2 en el cierre) y el tramo nuevo
salió con la advertencia de Revit "slightly off axis". **Cierre (0.13.1, `fase-13.md` sección 7)**: los dos tramos se construyen sobre
la `Line` exacta de Revit (punto de corte proyectado en pies) y el registro guarda `original_start_feet` / `original_end_feet` para
deshacer con la curva exacta; erratas de la instalación corregidas (los sondeos 12 y 13 se llaman `12-fase3-borrar.py` y
`13-limpiar-fase1.py`). Probado en la nube: 314 pruebas, simulador 101/101, `probar_conexiones.py` 46/46.

---

## 1. Qué es MotorConexiones

Un sistema para modelar en Revit el nudo de una cercha tal como lo dibuja el plano de fabricación, sin inventar nada.
La IA lee el detalle, escribe una especificación JSON (`gusset_node`, contrato v1), la valida con el add-in y, con la
confirmación del usuario, la crea. También se puede usar sin IA desde la cinta de Revit: panel **MotorConexiones** en la
pestaña **ARBA** (o en **Conexiones** si ARBA no se pudo usar) con los botones **Ejecutar especificación JSON** (abre la
ventana de previsualización, sección 9), **Conexiones del modelo** (lista y borra, sección 10), **Catálogo** (plantillas
con nombre que se aplican a otro nudo, sección 11), **Planificar lote** (detecta los nudos de una cercha, planifica y, desde
la Fase 9, crea el lote con **Crear N conexiones** y lo quita con **Borrar el lote**; desde la Fase 10 basta pinchar una barra,
con etiquetas pinchables y cartelas fantasma en la vista, sección 12) y **Encargo para IA** (Fase 10: el encargo para una IA
del navegador en un solo `.md`, sección 9).

Lo que garantiza el add-in y dónde está probado:

| Garantía | Qué significa | Probado en |
|---|---|---|
| Validación obligatoria | `conn_create` y `conn_update` exigen el `validation_token` (SHA-256) que solo entrega `conn_validate` sin errores. Cubre la especificación, el documento y las barras del nudo. | `docs/fases/resultados-fase-3.md` y `resultados-fase-4.md` (`create` sin token → `VALIDATION_TOKEN_INVALID`; token determinista: el mismo en las Fases 3 y 4) |
| Token ligado a `config/limits.json` | Desde la Fase 5 el token incluye también el hash de los límites AISC: si editas `limits.json` entre validar y crear, el token deja de valer. | Pruebas del Core en la nube (51). **PENDIENTE DE INSTALADOR** en Revit: sondeo 14 de `docs/instalacion/fase-5.md` |
| Backend nativo Advance Steel | Cartela y placa cuchilla como `SteelProxyElement` de categoría *Plates*, pernos como *Bolts*, **con las medidas del contrato** (Advance Steel trabaja en mm; el add-in convierte desde los pies de Revit). Las soldaduras van como `DirectShape` (reserva prevista en v1) y, si Advance Steel no está disponible, todo sale por `DirectShape`. | `resultados-fase-5.md`, ronda 5b, `5b-4` (cartela 565 × 530 × 9,53 mm, cuchilla 170 × 140 × 10 mm, pernos 5/8" a 60 mm) y capturas `fase5-03/04` |
| Atómico y sin ventanas | Cada operación es un `TransactionGroup`; ante un error, rollback completo. Los diálogos de Revit se cancelan y quedan como avisos en la respuesta. | `resultados-fase-3.md` y `resultados-fase-4.md` ("sin ventanas ni cierres de Revit"; avisos `REVIT_WARNING` en `create`) |
| Reversible | `conn_delete` borra solo lo que creó el add-in y devuelve a las barras sus extensiones originales (guardadas en Extensible Storage). | `resultados-fase-3.md` y `resultados-fase-5.md` (`B-3` y `5b-6`: `extension ... -> 0.0 mm`, `conexiones tras borrar: 0`, `Elementos de acero sueltos encontrados: 0`) |
| Cliente de IA | Las 13 herramientas `conn_*` se ven y funcionan desde Antigravity por el puente HTTP del puerto 8000. | `resultados-fase-4.md`, sección "4-9 cliente de IA" |
| Punta a punta desde la IA | Desde Antigravity: `conn_ping` → guía → tipos → `node_info` → esquema → `validate` → `preview` → confirmación literal → `create` (9 elementos, 2,2 s) → `list` → `get` → confirmación literal → `delete` → `list` = 0, sin ventanas ni cierres de Revit. | `resultados-fase-5.md`, sección "B-1 Antigravity (punta a punta)" |
| Token ligado a `limits.json` | El `validation_token` incluye el hash del `limits.json` desplegado: con un `limits.json` distinto o un token alterado, `create` responde `VALIDATION_TOKEN_INVALID` y el modelo no cambia. | `resultados-fase-5.md`, `A-7` (sondeo 14, 14/14) |
| Ventana de previsualización | El botón de la cinta dibuja el nudo con cotas iguales al plano, la tabla edita el JSON y revalida, Guardar JSON no toca el original, Crear y borrar desde la cinta funcionan igual que `conn_create`/`conn_delete`. | `resultados-fase-6.md` y capturas `fase6-01` a `fase6-07` |
| Catálogo de plantillas (Fase 7) | Una conexión creada se guarda como plantilla sin IDs (`conn_catalog_save`, botón **Guardar en catálogo**); `conn_catalog_apply` la casa por ángulos con las barras de otro nudo (también en espejo), instancia la especificación y la valida con el token de siempre. El marco del nudo es canónico (X hacia +X global, +Y hacia arriba) y los ángulos van con signo. | Pruebas del Core en la nube (131: ida y vuelta del Detalle D, 4 orientaciones, sin encaje, políticas de perfil, almacén) y simulador del MCP (36/36 y 23/23). En Revit, `resultados-fase-7.md`: marco canónico sobre el nudo real (`x_axis [1,0,0]`, `chord_direction_reversed: true`, ángulos 136,9 / 44,4 / −135,6 con `side` +Y / +Y / −Y); el Detalle D creado como en el plano (capturas `fase7-02/03`); `catalog_save` desde la conexión real con copia en `catalog\`; `catalog_apply` al mismo nudo `same`, desvío 0, sin avisos y token nuevo; la misma plantilla en el nudo simétrico `mirror_x`, validada en verde y creada (capturas `fase7-06/07`); `probar_conexiones.py --puente` 25/25; sondeos 12 y 13 en cero. Ronda 7b en el PC (`resultados-fase-7b.md`): `deploy.ps1` y `conn_ping` en `0.7.0`, fixture con `1 aviso(s)` y rótulo `diagonal 1249631` (captura `fase7b-01`), plantilla oficial regenerada con `overwrite: true` (mismo `template_id`, tres `diagonal`), sondeos 12 y 13 en cero |
| Plan de lote (Fase 8, cerrada del todo; rondas 8b a 8e) | Con la cercha seleccionada, `conn_batch_plan` (o el botón **Planificar lote**) lleva cada extremo de barra al corte de su eje con el eje del cordón (las diagonales reales terminan en la cara del cordón, a 15–85 mm de su eje; se admite hasta medio canto de cada barra más 10 mm, `node_face_reach_mm`), agrupa esos puntos en nudos, reconoce el cordón que atraviesa, nombra los nudos `N1…`, casa cada uno con las plantillas (también en espejo), instancia la especificación con `source.batch_id` y la valida con el token de siempre; marca los nudos en la vista (colores y marcadores, sin escribir `Marca`; un solo plan marcado por documento) y admite correcciones acumuladas con el mismo `plan_id`. **No crea nada.** | Pruebas del Core en la nube (162: cercha sintética de 30 nudos, la cercha del Hangar de la Fase 8 reconstruida de los puntos de trabajo, 53 barras y 10 `ready`, y la **cercha de la ronda 8b con sus ejes reales**, 56 barras: 59 nudos, 16 `ready` con la plantilla oficial, 8 `same` y 8 `mirror_x`, 20 `no_match` con `NODE_CHORD_NOT_CONTINUOUS`, 23 `untyped`, el mismo plan nudo a nudo que devolvió el PC, y el plan de solo las cuatro barras del Detalle D con el nudo listo llamado N4) y simulador del MCP (45/45 y 26/26; las pruebas 22 y 23 del puente buscan el nudo listo por su cordón en vez de suponer "N1"). En Revit, ronda 8b (`resultados-fase-8b.md`, add-in 0.8.1): `deploy` y `ping` en `0.8.1`; sondeo 18 midiendo en el modelo que las diagonales del Detalle D terminan a 58,8 / 14,3 / 33,1 mm del eje del cordón con los ejes cortándolo a menos de 4 mm entre sí; `conn_batch_plan` sobre 56 barras → **`ready: 16, no_match: 20, untyped: 23`** en 953 ms, N4 (gemelo del Detalle D) `ready same` con desvío 0 y token de 64, N7 `mirror_x`, marcadores sobre el eje del cordón (capturas `fase8b-02/03`), `TEMPLATE_PROFILE_DIFFERS` en los nudos de cordón HSS4X4; replan con `exclude` e `include` con el mismo `plan_id` y **el mismo token**; ventana del plan con 16 listos y **Editar nudo** abriendo la previsualización de N4 validada (`fase8b-04/05`); `discard all` y sondeos 17, 12 y 13 a cero. Fase 8 (`resultados-fase-8.md`, 0.8.0): marcas, ventana, Ver en Revit, Cordón y Añadir nudo pinchando. **NO PROBADO en Revit (0.8.2, se comprueba en la instalación de la ronda 8c, `fase-8.md` 8.5 y 9.3)**: `end_gap_mm` en la respuesta, `PLAN_MARKS_REPLACED`, el conteo de `removed_markers`, `overrides` sin `IsEmpty`, el paso 9 del sondeo 17, `--puente` 28/28 (la 8b no lo anotó; según el chat del instalador fallaron 22, 23 y 27, y 22 y 23 suponían "N1"). **Ronda 8c (0.8.3, NO PROBADA en Revit; `docs/instalacion/fase-8c.md`)**: ventana del plan que se entiende (cabecera con la decisión, mapa de la cercha, solo nudos de verdad, estados en español con icono y color, columna Qué hacer, 4 botones + Más… + clic derecho), marcas del modelo con color por estado (los ocultos no se marcan), `summary_text` / `status_text` / `advice` / `visible_by_default` en la respuesta, aviso `CATALOG_EMPTY` con botón Abrir catálogo y sondeo 19 (etiquetas en el lienzo). Probado en la nube: 177 pruebas del Core (+15: `PlanAdviceTests` con la cabecera exacta de la cercha de la 8b y `TrussMapTests` con su alzado: 56 segmentos, 59 puntos, 16 visibles, N4 en X = −11870), simulador 47/47 y `probar_conexiones.py` 26/26. **Ronda 8c probada en el PC** (`resultados-fase-8c.md`, 0.8.3): 64 barras → los mismos 16 listos, 10 `no_match` con cordón y 7 `✖ Falta el cordón` en el cordón superior (el Detalle D es la típica del cordón central; `fase-8.md` 10.2), colores por estado y mapa (`fase8c-01/02`), `CATALOG_EMPTY` con Abrir catálogo, replan con el mismo token, `discard all` 66 → 0, sondeo 17 paso 9 y `--puente` **28/28**. **Cierre de la 8c (0.8.4, NO PROBADO en Revit; `docs/instalacion/fase-8d.md`)**: ventana del plan no modal con `ExternalEvent`, Ver en Revit sin cuadro, Descartar que quita todos los marcadores (y estado en memoria restaurado si Revit deshace), sondeo 19 v2 con BMP; en la nube 177 pruebas, simulador 47/47 y `probar_conexiones.py` 26/26. Ronda 8d en el PC (`resultados-fase-8d.md`, 0.8.4): ventana abierta mientras se orbita y se pincha (captura `fase8d-02`), Descartar desde la ventana con un plan del puente marcado en otra vista → `remaining_markers: 0` y sondeo 17 a cero, catálogo vacío con la ventana abierta, sondeo 19 v2 con el BMP aceptado y el manejador de clics registrado (etiqueta no visible), `--puente` 28/28; **Cordón… cerró Revit** (captura `fase8d-crash-revit`) y Descartar no cerró la ventana. **Cierre de la 8d (0.8.5, `fase-8.md` sección 11) probado en el PC en la ronda 8e** (`resultados-fase-8e.md`, 2026-10-05, puente repetido el 2026-10-06; `fase-8.md` sección 12): Cordón… sobre N9 seis veces y Barras… dos veces **sin cerrar Revit**, con la ventana oculta mientras se pincha y de vuelta después (9 `ribbon_batch_pick` / 9 `ribbon_batch_picked`, 0 `ribbon_batch_window_error`; captura `fase8e-02-cordon-n9`), Esc cancela (`cancelado`), Excluir / Incluir y Replanificar con la ventana abierta, Descartar desde la ventana con `closes_window: true` y `closed … discarded: true` (0 marcadores; sondeo 17 a cero), sondeo 19 v3 con **la etiqueta vista y pinchada** (`fase8e-01-etiqueta`, `fase8e-01-etiqueta-clic`; cuatro clics anotados), sondeos 12 y 13 en cero y `--puente` **28/28**. **Cerrado en la instalación de la Fase 9** (2026-10-06, `resultados-fase-9.md`; `fase-9.md` 7.5): el sondeo 19 **v4** entero (las dos etiquetas, el clic sin cuadro, naranja; captura `fase9-05`), Plantilla… cancelado, Planificar lote con la ventana abierta, Ver en Revit sin cuadro y Descartar con un plan marcado en otra vista; sin anotar todavía Editar nudo con la ventana abierta, el cordón inferior y `PLAN_MARKS_REPLACED` (Fase 10) |
| Crear por lotes (Fase 9, cerrada el 2026-10-06) | `conn_batch_create` (o **Crear N conexiones**) exige `plan_id` y el `validation_token` del plan **por nudo** (sin token no se crea nada; un nombre suelto es `INVALID_REQUEST`), lo compara con el del plan y lo vuelve a comprobar contra el modelo como `conn_create`; crea cada nudo como una operación propia (`OperationScope`: grupo, `IFailuresPreprocessor`, diálogos cancelados, adopción de los elementos de Advance Steel, registro con `source.batch_id`) anidada en un grupo exterior del lote: **el nudo que falla se revierte solo y los demás se quedan** (P9); el grupo exterior se asimila (una entrada de deshacer) o se revierte entero con `stop_on_error`. Solo se crean `ready` y `failed` (reintento); `invalid`, `no_match`, `excluded`, `untyped`, `already_connected` y los ya creados se saltan con su motivo. `conn_batch_delete` borra las conexiones del lote una a una con las garantías de `conn_delete`, en un solo grupo. | Pruebas del Core en la nube (192: `BatchCreateTests` con la cercha de la 8b y un creador simulado: 16 creadas con 14 avisos, un nudo que falla se revierte solo, `stop_on_error` revierte todo, saltados por estado, token distinto del plan, `replace_existing` → rehecha, informe JSON ida y vuelta, borrado que devuelve los nudos a listos, "ya creada en este lote" tras replanificar; `PlanActionsTests`: botones por estado y el empalme detectado con dos tramos de cordón), simulador 62/62 (crear, saltar la segunda vez, token falso, `stop_on_error`, `list` por lote, borrar) y `probar_conexiones.py` 28/28 (`batch_create` con token falso no toca el modelo; `batch_delete` de un lote inexistente). En Revit (`resultados-fase-9.md`, add-in 0.9.0, 2026-10-06, sobre la copia del modelo): sondeo 20 con `create` y `delete` del Detalle D dentro de un grupo exterior, `Assimilate() -> Committed`, `RollBack() -> RolledBack`, `list` 0 y acero suelto 0 ("GRUPOS ANIDADOS OK": `batch_single_undo` sigue en `true`); **Crear 16 conexiones** desde la ventana dos veces, `created 16, failed 0` en 2,6 s y 2,4 s (9 elementos por nudo, 14 con el aviso de perfil), informe en la ventana y marcas de los creados fuera (capturas `fase9-01` y `fase9-02`), `list` con `batches {…: 16}`; **un solo Ctrl+Z** las quitó (sondeos 12 y 13 a cero; una entrada `batch_create` en Deshacer, captura `fase9-04`); por el puente, `batch_create` de 2 (`created_with_warnings`, `undo_entries one`), saltados la segunda vez, token alterado → `failed` con `VALIDATION_TOKEN_INVALID` y `BATCH_NODE_FAILED` sin tocar el modelo, sin token → `INVALID_REQUEST`, `batch_delete` 2 (18 elementos, 6 barras restauradas) y `BATCH_EMPTY` después; N9 como `✖ Empalme del cordón` con el consejo del empalme y Excluir; botones de Qué hacer (Incluir, Plantilla…) pulsados; sondeo 19 v4 entero (captura `fase9-05`); log sin errores de ventana. **Sin probar en Revit** (instalación de la Fase 10): Borrar el lote **desde la ventana** (la operación sí, por el puente), `--puente` 30/30 (se cayó en la 22 por la consola cp1252; corregido en el cierre), `stop_on_error` con 16 nudos, `PLAN_MARKS_REPLACED` |
| Cercha sin dolor (Fase 10, add-in 0.10.0, probada en Revit el 2026-10-07; ronda 10b, add-in 0.10.1, probada en Revit el 2026-10-11; **cerrada**) | **Selección asistida**: a partir de una barra, el add-in añade las que la tocan con la misma regla del detector (el extremo de una se corta con el eje de la otra a menos del alcance de cara; empalmes por extremos pegados), por rondas, solo dentro del plano de la cercha (con una barra, el plano lo votan las que la tocan); el botón pregunta antes de planificar con todas. **Etiquetas pinchables**: un control con imagen por nudo visible (BMP de 24 bits de 32×32 escrito a mano, como el del sondeo 19 v4) en la vista marcada; el manejador de clics **nunca abre un cuadro** (resalta la etiqueta, elige la fila en la ventana, escribe en la barra de estado, anota en el log); se quitan al replanificar, al crear (las de los creados), al descartar y con `discard`; un fallo nunca impide planificar (`PLAN_LABELS_SKIPPED`). **Cartelas fantasma**: un `DirectShape` transparente con el contorno y el espesor de la cartela de cada nudo con especificación, en el marco del nudo, creado con las marcas y quitado como ellas. **Encargo para IA**: un solo `.md` escrito por el Core. | Pruebas del Core en la nube (217: `SelectionAssistTests` con la cercha sintética desde una diagonal y desde el cordón, riostra fuera del plano, selección completa, tope, y la cercha de la 8b recuperada entera desde una diagonal con los mismos 59 nudos y 16 listos; `LabelImageTests` byte a byte; `DesignBriefTests`), simulador 66/66 (`expand_selection`, `labels`, `removed_labels`) y `probar_conexiones.py` 30/30 (pruebas 29 y 30: `batch_plan` con `expand_selection` desde una barra y su descartar). **En Revit** (`resultados-fase-10.md`, add-in 0.10.0, 2026-10-07, sobre la copia; `fase-10.md` 7.1): la selección asistida **por el puente** desde la diagonal 1251723 añadió las 63 barras de la cercha de la 8c (8 cordones, 46 barras, 9 tramos, 11 rondas, ninguna fuera del plano) y el plan dio **los mismos 16 listos** de siempre; las **33 etiquetas se pusieron** (`labels_applied`, 33 BMP en `C:\Users\Public\…\etiquetas`) y se quitaron al crear, al replanificar y con `discard all` (`removed_labels 33`, `remaining_labels 0`, sondeo 21 a cero); **16 cartelas fantasma** creadas (`ghost_count 16`) y quitadas con los marcadores (49 a cero); Crear 16 dos veces desde la ventana (4,0 s y 2,1 s), Ctrl+Z y Replanificar (33 etiquetas repuestas), **Borrar el lote desde la ventana** (`deleted 16`, 144 elementos, 48 barras restauradas, 1,1 s; sondeos 12 y 13 a cero), `create` en la {3D} con los tres `REVIT_WARNING` "The created elements are only visible in Detail Level: Fine", **Encargo para IA** sobre el Detalle D (`clipboard: true`, carpeta abierta, 15 673 caracteres); log sin `label_click_failed`, `labels_failed`, `plan_event_failed` ni `ribbon_batch_window_error`. **Lo que falló**: el **cuadro de la selección asistida de la cinta** ("Corresponding button not found: defaultButton": el botón por defecto se fijaba antes de crear los enlaces; **corregido en la 0.10.1**) y, por eso, **las etiquetas no se pincharon**; `--puente` 14/22 (otro documento activo y el servidor del puerto 8000 parado). **Ronda 10b en Revit** (`resultados-fase-10b.md`, add-in 0.10.1, 2026-10-11; `fase-10.md` 8.1): **Planificar lote con una barra desde la cinta** abre el cuadro "Selección asistida: 63 barras tocan la selección" (`ribbon_batch_assist` con `choice` all / cancel / selection; Solo la selección con una barra da el cuadro de "una sola barra" y conserva el plan), la ventana con los mismos 16 listos; **las etiquetas pinchadas sin ningún cuadro** (`label_clicked` N4, N5, N6, N9, N11, N37, N40 con la ventana abierta y N23 con ella cerrada, `highlighted: true`; la ventana elige la fila; el resaltado desde la tabla y el mapa al momento, según la persona), las 33 etiquetas y 16 fantasmas visibles y de tamaño fijo al orbitar; Descartar plan a cero (113 marcas y 33 etiquetas); **`--puente` 32/32** con la copia activa y el servidor del puerto 8000 arrancado. Sin anotar: el globo, la fila elegida al reabrir, y el cuadro de Borrar el lote, Ctrl+Z con las marcas, `PLAN_MARKS_REPLACED` y Editar nudo con la ventana abierta (de la Fase 10). De paso para la Fase 11: la línea del plan no dice "(63 añadidas…)" cuando el plan sale del botón, `highlighted` en el log significa "cambió", y los títulos de los cuadros salen con "MotorConexiones - " dos veces |
| Conectar un nudo (Fase 11, add-in 0.11.0; probada en Revit el 2026-10-12; **cerrada**) | **Conectar**: lee el nudo con la misma regla que `conn_get_node_info` y `conn_catalog_apply` (`TemplateNode.FromModelFacts`: cordón el más horizontal, marco canónico, ángulos con signo), casa todas las plantillas con `TemplateChooser` (solo cuentan las que casan todas sus ranuras sin dejar barras sobrantes; gana la de menor suma de desvíos, después menor desvío máximo, después la más reciente) y abre la previsualización con la mejor; sin plantilla, `SpecStarter` escribe una especificación válida en esquema (IDs y perfiles del modelo, soldada en ranura, cartela rectangular que envuelve los extremos recortados redondeada a 5 mm, `through_slot` como duda confirmada, valores de la plantilla del mismo cordón o de `choices.json`). **No crea nada al pulsarse.** El panel escribe siempre por `SpecEditor` sobre el JSON (una sola fuente de verdad); un desplegable con rótulo escribe `*_mm` y `*_label` en la misma edición. Las miniaturas nunca bloquean (fallo → hueco gris y `thumbnail_failed`). | Pruebas del Core en la nube (238: `TemplateChooserTests` con el Detalle D en `same` y en `mirror_x`, dos plantillas con la de menor desvío ganando, la más reciente a igual desvío, la de 2 barras sin encaje con su texto y el catálogo vacío; `SpecStarterTests` con el arranque del Detalle D que pasa el esquema y el validador con los hechos del Hangar, contiene los extremos de las tres barras, toma los valores de la plantilla del mismo cordón y de la más reciente; `PanelChoicesTests` con `config/choices.json` del repositorio y los cuatro diámetros en `limits.json`; `TemplateSketchTests` con el croquis de la plantilla oficial sin modelo; `SpecEditorFase11Tests`; `DrawingImageTests`), simulador 69/69 y `probar_conexiones.py` 31/31 (21b: `catalog/apply` sin `template_id`). **En Revit** (`resultados-fase-11.md`, 2026-10-12; `fase-11.md` 7.1): Conectar sobre el Detalle D abrió la previsualización sin archivo con "Plantilla propuesta: Nudo tipico Detalle D · igual · desvío máx. 0,0°", el panel escribió `1/2" · 12,7 mm` y pernos `3/4"` sin errores, Barra 1 a placa cuchilla y vuelta, **Crear** creó 9 elementos (`delete` los borró y restauró 3 barras); el plano al lado desde archivo y desde el portapapeles; las miniaturas pintadas (ninguna gris); "(63 añadidas…)" desde la cinta, `changed` / `highlighted` en el log, títulos sin repetir; `catalog_apply` sin `template_id` con `proposal`, `validate` con `drawing_image`, `--puente` 34/34. **Lo que no validó**: el arranque sobre el cordón superior (`MEMBER_NOT_AT_NODE`): esos nudos son **nudos en K con excentricidad** (las diagonales cortan el eje del cordón a 34 mm una de otra) y el contrato exige un solo punto de trabajo; se decide en `docs/propuestas/nudos-en-k-excentricos.md` antes de la Fase 12 |
| Placa de extremo y nudo soldado (Fase 12, add-in 0.12.0, programada el 2026-10-12; **probada en Revit el 2026-10-21**, `fase-12.md` 7; ronda 12b probada el 2026-10-21 y cerrada el 2026-10-22, `fase-12.md` 8; ronda 12c probada el 2026-10-23 y cerrada, `fase-12.md` 9; ronda 12d probada en Revit el 2026-10-24 y cerrada el mismo día, `fase-12.md` 10, add-in 0.12.4; **Fase 12 cerrada**) | **Excentricidad, opción A** (`NodeFrame.Compute`, `SpecFrame`, `NodeReach.MemberReachesNodeEccentric`): punto de trabajo a mitad entre los cortes extremos, `node.eccentricity_mm` y `members[].axis_offset_mm` opcionales e invisibles si no se usan (`JsonIgnore` con `null`), `MEMBER_ECCENTRIC` hasta medio canto del cordón (`node_axis_max_offset_mm`), cada barra medida desde su propio corte, el hash de `limits.json` sin cambios con los valores por defecto. **`end_plate`** (`EndPlateSpec`, `EndPlateValidator`, `Octagon`, `BoltRing`, `BoltStack.ForTwoPlates`, `EndPlateSketch`, `EndPlateBuilder`): dos planchas octogonales a ±(holgura/2 + espesor/2), filas por cara como patrones de N × 1, retiro = espesor + holgura/2 − hueco medido (`SPLICE_SETBACK_TOO_SMALL`), `SPLICE_NOT_COLLINEAR`, `BOLT_HOLE_TOO_SMALL`, `BOLT_TOO_CLOSE_TO_TUBE`, `PLATE_SMALLER_THAN_TUBE`. **`direct_weld`** (`DirectWeldValidator`, `DirectWeldGeometry`, `DirectWeldSketch`, `DirectWeldBuilder`): cada barra desde su corte hasta la cara (medio canto en el plano), huella ancho/\|sen\|, `MEMBER_WIDER_THAN_CHORD_FACE`, `MEMBERS_OVERLAP_ON_FACE`, reserva de retiro por el eje con `END_CUT_APPROXIMATED`. **Clase `splice`** en el detector (dos tramos colineales; con otras barras, solo si la pareja es la más horizontal), dos nudos en un punto (`N9`/`N9E`), `OrderForCreation` (empalme antes que nudo), arranques por tipo en el plan (`direct_weld` y `end_plate`; la cartela sigue `no_match`), `TemplateChooser.ForNode` por clase. Ningún miembro de la API se inventa: los sondeos 22, 23 y 24 deciden el corte plano, los pernos y la soldadura | Pruebas del Core en la nube (268: `EccentricityTests` con el K sintético de 54,9 mm, la regresión de los tokens de la 8b y el hash de `limits.json`; `DirectWeldTests` con el fixture K, los avisos de cara y huella y el croquis con la marca a 330; `EndPlateTests` con los tres fixtures, los 12 pernos del C, los errores por regla y los arranques por perfil; `Fase12PlanTests` con la cercha de la 8c: 41 listos y la cabecera literal, N9E antes que N9, plantillas de los tipos nuevos), simulador 82/82 y `probar_conexiones.py` 35/35 (30b a 30e). **En Revit** (`resultados-fase-12.md`, 2026-10-21; `fase-12.md` 7.1): los dos tipos se fabrican con Advance Steel, `node_info` mide 54,9 de excentricidad, Conectar y el lote de 40 sobre el cordón superior sin fallos; **Ronda 12b** (`resultados-fase-12b.md`, 2026-10-21; `fase-12.md` 8.1): puente 38/38, `validate` con `kind`; el sondeo 27 explicó el 203 × 203 (`Ht` y el giro de 90°), el 26 escribió la `WeldLine`, el 25 no encontró la viga de Advance Steel. **Ronda 12c** (0.12.2, `resultados-fase-12c.md`, 2026-10-23; `fase-12.md` 9.1; `SectionOrientationTests` y `WeldChainTests` en la nube): el perfil leído 203,2 × 304,8 con giro 90 y límite 152,4, las `WeldLine` materializadas en los dos tipos (2 y 8 elementos, `missing 0`), el 430 en horizontal, puente 38/38; el sondeo 28 no encontró la viga porque su recorrido de los objetos no corrió. **Ronda 12d** (0.12.3, `resultados-fase-12d.md`, 2026-10-24; `fase-12.md` 10.1): las advertencias de Revit leídas en el log (`revit_failure`: "The created elements are only visible in Detail Level: Fine.", una por soldadura) y el sondeo 29 sin viga (`GetModelObjectIds` dio 5 objetos, ninguno abierto, 0 vigas): el retiro por el eje queda **definitivo**. **Cierre de la 12d** (0.12.4, `fase-12.md` 10.3): esa advertencia se filtra del aviso por el Guid de su definición (en el log con `filtered: true`); se ve en el PC con la siguiente instalación. **Fase 12 cerrada** |
| Cartela automática y partir la diagonal (Fase 13, add-in 0.13.0 programada el 2026-10-24 y **probada en Revit el 2026-10-31**; cierre con el add-in 0.13.1 el mismo día, `fase-13.md` sección 7) | **`outline.mode: "auto"`** (`Geometry2D/AutoOutline`, `Polygon2D.ConvexHull` / `Simplify`): rectángulo por barra (retiro → fin de ranura o placa + `margin_mm` 40, medio ancho + `side_margin_mm` 30), franja del cordón (medio canto + `beyond_chord_mm` 40), envolvente convexa redondeada a 5 mm hacia fuera, giros < 3° fuera, 12 vértices (`auto_outline_max_vertices`), `OUTLINE_AUTO_LARGE` (> `max_auto_gusset_mm` 1 200 o área > 2 × rectángulos); el validador resuelve `auto` antes de la regla 9; el croquis, la previsualización y **la fabricación** usan los mismos puntos; `calculated_values.outline_points_mm`; `SpecStarter` arranca con `auto`; `SpecEditor.TrySetOutlineMode` / `ConvertAutoToPolygon`; plantillas `auto` con `auto_angle_tolerance_deg` 25; el plan arranca cartelas `auto` (D2). **`SplitPoint`** (cara a (ancho/2)/\|sen θ\|, +330, +espesor+gap/2), **`SplitRecord`** (Extensible Storage, esquema `MotorConexionesSplit` propio), `MemberSplitter` (`CopyElement` + `LocationCurve.Curve`, extensiones 0 y `DisallowJoinAtEnd` en el corte, parámetros comparados; reserva `NewFamilyInstance`; `SPLIT_PARAMETERS_APPROXIMATED`, `SPLIT_UNDO_PARTIAL`, `SPLIT_FAILED`, `SPLIT_IN_USE`, `SPLIT_NOT_FOUND`), `SplitService`, operaciones `split` / `unsplit`, `SplitChainService` (la cadena de Conectar: grupo exterior, un `OperationScope` por paso, el nudo en K revalidado), `SplitPlanning` (pasos `split`, nudos `N…B` `after_split` con id provisional), `BatchRunner.StepsFor` (cortes → empalmes B → N9E → N9) con `BatchStepHandlers` (`Split`, `CreateSplice`, `Revalidate`), `BatchCreator` y Borrar el lote con `unsplit`. El tramo corto conserva el id (D6); el contrato solo añade; el camino `polygon` da los mismos tokens | Pruebas del Core en la nube (311: `AutoOutlineTests` con la caja del Detalle D al 15 % del plano, ranuras y placa dentro, ±15° frente al polígono, 16 nudos de la 8b, espejo, márgenes absurdos; `Fase13RegressionTests` con 21 tokens de la 0.12.4; `SpecEditorOutlineTests`; `TemplateAutoTests`; `SplitPointTests` (≈ 485, θ 44,4); `Fase13PlanTests` con los 31 pasos y nudos previstos de la 8c, el orden, el rollback y el fallo parcial), simulador 101/101 y `probar_conexiones.py` 46/46 (31 a 33b). **Probado en Revit el 2026-10-31** (`resultados-fase-13.md`): el filtro de la 12d, la cartela automática y la plantilla `auto` en espejo, la cadena del nudo en K (cortes a 485 y 483 mm, dos empalmes B, `SPLIT_IN_USE`, `unsplit` completo), `split` / `unsplit` por el adaptador (`copy_element`, sin parámetros aproximados), el plan con 31 cortes y el puente 49/49. **NO PROBADO en Revit**: la 0.13.1 del cierre (tramos sobre la `Line` exacta, registro en pies, sondeo 30 v2, ¿desaparece la advertencia "slightly off axis"?): `docs/instalacion/fase-13b.md` |
| Pernos con agarre real (ronda 6b) | La placa cuchilla apoya sobre una cara de la cartela (`plate.gusset_face`, `+z` por defecto) y los pernos atraviesan cartela + placa: agarre = suma de espesores y longitud calculada de `limits.json` (Detalle D: 19,5 mm y 44,45 mm) o tomada de `bolts.length_mm`. Las cotas del croquis se editan con doble clic. | Pruebas del Core en la nube (99); ronda 6b en el PC: placa apoyada y `Bolt Length 44,45` / `Grip Length 19,53` (`resultados-fase-6b.md`). Ronda 6c: pernos con cabeza en la placa y `Grip 19,52` medidos por el sondeo 16 (`resultados-fase-6c.md`). Ronda 6d (placas centradas en su plano): sondeo 16 en la sesión de la Fase 7, cartela `−4,76 .. 4,76`, placa cuchilla `4,76 .. 14,76` y pernos `−29,69 .. 24,69`, las tres `OK` (`resultados-fase-7.md`, bloque `6d-3 sondeo 16`; `fase-6.md`, sección 9) |

Lo que **no** hace: no diseña ni verifica resistencias; no lee planos PDF completos; conoce tres tipos (`gusset_node`, `direct_weld` y `end_plate` como empalme; la placa contra un apoyo y la placa base están pendientes).

---

## 2. Estructura del repositorio

```text
CONEXIONES/
├── CLAUDE.md                        Reglas permanentes del repositorio
├── MotorConexiones.sln
├── src/
│   ├── MotorConexiones.Core/        netstandard2.0, sin referencias a Revit
│   │   ├── AddinInfo.cs             Versión del add-in (0.13.0 en la Fase 13; igual que <Version> de los csproj) y spec_version (1.0)
│   │   ├── Batch/                   Fase 8: NodeDetector (segmentos → nudos), BatchOverrides (correcciones), BatchPlan (el plan
│   │   │                            y sus nudos, JSON), PlanBuilder (detección + correcciones + casado + instanciación + validación),
│   │   │                            PlanAdvice (ronda 8c: estado en español, color por estado, qué hacer, cabecera con la decisión;
│   │   │                            Fase 9: created/failed, botones de Qué hacer, empalme), TrussMap (alzado 2D de la cercha para el
│   │   │                            mapa de la ventana) y, Fase 9, BatchCreateRequest (plan_id + nudos con token), BatchReport (informe
│   │   │                            por nudo) y BatchRunner (qué se crea, en qué orden, stop_on_error; el creador lo pone Revit);
│   │   │                            Fase 10: SelectionAssist (selección asistida: barras que se tocan, plano por votación, rondas) y
│   │   │                            LabelImage (el BMP de 24 bits de la etiqueta del lienzo, fuente de píxeles 5×7); Fase 12: NodeKind (node | splice),
│   │   │                            dos nudos en un punto (N9 y N9E), arranques por tipo en PlanBuilder, OrderForCreation (empalme antes que nudo K);
│   │   │                            Fase 13: SplitPlanning (pasos split y nudos N…B previstos con id provisional), PlanSplitStep, estado after_split,
│   │   │                            BatchRunner.StepsFor (cortes → empalmes B → N9E → N9) y BatchStepHandlers (Split, CreateSplice, Revalidate), D2 en PlanBuilder
│   │   ├── Brief/DesignBriefWriter.cs   Ronda 9b (Fase 10): el encargo para una IA externa en un solo Markdown; el Detalle D confirmado embebido
│   │   ├── Catalog/                 Fase 7: CatalogTemplate (archivo de plantilla), CatalogConfig (config/catalog.json),
│   │   │                            TemplateNode (el nudo con ángulos con signo), TemplateBuilder (spec → plantilla),
│   │   │                            TemplateMatcher (4 orientaciones), TemplateInstantiator (plantilla → spec con IDs),
│   │   │                            CatalogStore (un JSON por plantilla), TemplateJson
│   │   ├── Contract/                ConnectionSpec, ChordSpec, GussetSpec, GussetOutline, MemberSpec, AttachmentSpec,
│   │   │                            KnifePlateSpec, BoltPatternSpec, WeldSpec, DimensionChain, UncertainField, SourceInfo
│   │   │                            (con template_id y batch_id), NodeRef, ApiResponse/ApiError/ApiMeta (sobre común), JsonOptions;
│   │   │                            Fase 12: EndPlateSpec (plate, bolts por cara, splice_hint), node.eccentricity_mm, members[].axis_offset_mm
│   │   ├── Editing/SpecEditor.cs    Tabla editable por ruta JSON (campos, valores, contorno, JSON con sangría); Fase 11: TrySetLabeledValue (mm + rótulo
│   │   │                            a la vez), TrySetAttachmentType (bloque de unión por defecto o el guardado), sección Origen (source.drawing_image)
│   │   ├── Editing/PanelChoices.cs  Fase 11: las listas de config/choices.json (espesores, pernos filtrados por limits.json, soldaduras, uniones),
│   │   │                            rótulo ↔ mm y "Otro: 9,2 mm"; StarterDefaults
│   │   ├── Editing/SpecStarter.cs   Fase 11 (D2, D10): la especificación de arranque (cartela rectangular, soldada en ranura, duda confirmada)
│   │   ├── Catalog/TemplateChooser.cs  Fase 11: casa todas las plantillas (MatchAll), ordena y propone (Conectar, Cambiar…, catalog_apply, PlanBuilder)
│   │   ├── Catalog/TemplateSketch.cs   Fase 11 (V4): el croquis de una plantilla sin modelo (IDs ficticios, ángulos del member_pattern)
│   │   ├── Geometry2D/              Point2D, Segment2D, Polygon2D (Fase 13: ConvexHull, Simplify), Geometry2DChecks (contorno, cruces, pernos en placa); Fase 12: Octagon;
│   │   │                            Fase 13: AutoOutline (la cartela automática: rectángulos por barra + franja del cordón → envolvente convexa a 5 mm, 12 vértices)
│   │   ├── Geometry3D/              Vec3, NodeFrame (sistema local canónico del nudo y ángulos con signo), NodeReach,
│   │   │                            ConnectionGeometry, BoltGrid, BoltPosition, WeldLine2D; ronda 12c: WeldChain (segmentos → polilíneas para WeldLine); Fase 13: SplitPoint (el punto de corte del Detalle B); Fase 12: SpecFrame (una sola regla del marco),
│   │   │                            BoltRing (pernos por cara), BoltStack.ForTwoPlates, DirectWeldGeometry (corte contra la cara)
│   │   ├── Model/IModelFacts.cs     Lo que el validador necesita del modelo, sin depender de Revit
│   │   ├── Schema/JsonSchemaValidator.cs   JSON Schema de gusset_node, direct_weld y end_plate (Fase 12) y su comprobación
│   │   ├── Sketch/                  Croquis 2D en mm: SketchPrimitives, SketchText, SketchNodeInfo, ISketchProvider,
│   │   │                            GussetNodeSketch, SketchBuilder (la ventana solo dibuja lo que sale de aquí); Fase 12: DirectWeldSketch, EndPlateSketch
│   │   ├── Storage/                 ConnectionRecord, ModifiedMemberRecord (lo que se guarda por conexión)
│   │   ├── Types/                   IConnectionType, ConnectionTypeRegistry, GussetNodeType, DirectWeldType y EndPlateType (Fase 12; los tres ISketchProvider)
│   │   ├── Units/UnitConverter.cs   ÚNICO sitio donde se convierten mm y grados a pies y radianes
│   │   └── Validation/              SpecValidator (las 10 reglas), ValidationTokenGenerator, LimitsConfig,
│   │                                LabelParser, ProfileMatcher, ErrorCodes; Fase 12: ValidationCommon (excentricidad), DirectWeldValidator, EndPlateValidator
│   ├── MotorConexiones.Revit/       net10.0-windows, add-in de Revit 2027
│   │   ├── App.cs                   IExternalApplication: panel "MotorConexiones" en la pestaña ARBA (reserva "Conexiones")
│   │   ├── RunSpecCommand.cs        Botón "Ejecutar especificación JSON": abre la ventana de previsualización y crea
│   │   ├── ListConnectionsCommand.cs  Botón "Conexiones del modelo": lista y borra
│   │   ├── ConnectCommand.cs        Botón "Conectar" (Fase 11): selección → nudo → TemplateChooser → previsualización (plantilla o arranque); Fase 13: la casilla de partir → SplitChainService
│   │   ├── SplitMemberCommand.cs    Botón "Partir barra" (Fase 13): cordón + diagonal → cuadro con un enlace por diagonal (y otra distancia) → split; con un tramo partido, deshacer
│   │   ├── PanelChoicesLoader.cs    Fase 11: lee config\choices.json y filtra los diámetros con limits.json
│   │   ├── Catalog/TemplateThumbnails.cs  Fase 11 (V4): el PNG de 240×180 junto a cada plantilla (RenderTargetBitmap del SketchCanvas sin cotas)
│   │   ├── CatalogCommand.cs        Botón "Catálogo" (Fase 7): aplicar a la selección, guardar desde conexión, eliminar
│   │   ├── BatchPlanCommand.cs      Botón "Planificar lote" (Fase 8): planifica, crea el ExternalEvent y abre la ventana del plan (no modal desde la 0.8.4);
│   │   │                            Fase 10: selección asistida con cuadro (Planificar con todas / Solo la selección / Cancelar)
│   │   ├── DesignBriefCommand.cs    Botón "Encargo para IA" (ronda 9b, Fase 10): node_info + esquema + guía + ejemplo en un .md, portapapeles, carpeta
│   │   ├── Batch/                   BatchPlanner (selección → plan → marcas; DiscardAndClean), BatchCreator (Fase 9: grupo exterior + OperationScope por
│   │   │                            nudo, token contra el modelo, marcas de los creados fuera, informe; DeleteBatch), PlanMarks (colores y marcadores), PlanRegistry (planes en memoria),
│   │   │                            PlanEvents (cierre 8c: ExternalEvent y cola de la ventana no modal), PlanSnapshot (plan + mapa + tipos leídos
│   │   │                            en contexto válido), PlanZoom (Ver en Revit sin cuadro), PlanPicker (pinchar barras desde el evento) y, Fase 10,
│   │   │                            PlanLabels + PlanLabelHandler (etiquetas pinchables del lienzo: TemporaryGraphicsManager, clic sin cuadro;
│   │   │                            PlanMarks dibuja además las cartelas fantasma; BatchPlanner.ExpandSelection lee las candidatas); Fase 13: BatchCreator con los pasos de
│   │   │                            corte (SplitService) y empalme B, el token del nudo en K renovado y Borrar el lote deshaciendo los cortes; BatchPlanner con split_steps y split_of_node
│   │   ├── Catalog/                 CatalogService (Revit → Core: construir, guardar, aplicar y validar), CatalogConfigLoader
│   │   ├── UI/                      WPF, solo en el camino de los botones: SketchCanvas, PreviewSession, PreviewWindow,
│   │   │                            ConnectionsWindow, CatalogWindow, SaveTemplateDialog, BatchPlanWindow, ChooseDialog,
│   │   │                            TrussMapCanvas (ronda 8c: el mapa de la cercha con un círculo por nudo) y RevitMainWindow
│   │   │                            (cierre 8d: activa la ventana principal de Revit antes de pinchar con la ventana del plan oculta);
│   │   │                            Fase 10: BatchPlanWindow.OnLabelClicked (etiqueta pinchada → fila y barra de estado) y Completar selección
│   │   ├── Bridge.cs                Bridge.Handle(operation, requestJson, doc, uidoc): punto de entrada del MCP
│   │   ├── LimitsConfigLoader.cs    Lee config\limits.json de la carpeta del add-in desplegado
│   │   ├── Fabrication/             IFabricationBackend, AdvanceSteelBackend, DirectShapeBackend, BackendFactory,
│   │   │                            MemberModifier (retiros de extremo); Fase 12: DirectWeldBuilder (reserva por el eje) y EndPlateBuilder (dos planchas, filas por cara);
│   │   │                            ronda 12c: AdvanceSteelBackend.CreateWelds con WeldLine (Connect a las planchas de la sesión, reserva DirectShape por recuento);
│   │   │                            Fase 13: MemberSplitter (CopyElement + LocationCurve.Curve, reserva NewFamilyInstance; Unsplit) — lo decide el sondeo 30
│   │   ├── Node/                    NodeInspector (Fase 12: SpecFrame, CutPointOf, AxisOffsetMm), RevitGeometry, RevitModelFacts (ronda 12c: Ht/b, la designación
│   │   │                            AISC y Cross-Section Rotation → width_mm en el plano, height_mm perpendicular), SelectionNodes (clases de la selección)
│   │   ├── Operations/              IOperation + una clase por operación: Ping, Guide, Types, Schema, NodeInfo,
│   │   │                            FindProfile, Validate, Preview, Create, List, Get, Update, Delete (13), SplitMember y Unsplit (Fase 13: split / unsplit) y
│   │   │                            CatalogList, CatalogGet, CatalogSave, CatalogDelete, CatalogApply (Fase 7) y
│   │   │                            BatchPlan, BatchPlanGet, BatchPlanDiscard (Fase 8) y BatchCreate, BatchDelete (Fase 9; List admite batch_id)
│   │   ├── Services/                ConnectionCreationService (crea la conexión completa y adopta los elementos de acero),
│   │   │                            ValidationService (validar contra el modelo, una sola regla), RibbonCreation (crear desde la cinta;
│   │   │                            Fase 11: los títulos de los TaskDialog sin "MotorConexiones - ")
│   │   ├── UI/SpecPanel.xaml(.cs)   Fase 11: el panel por secciones (Cartela, Cordón, Barra N) con desplegables y errores junto al campo
│   │   ├── UI/DrawingPane.xaml(.cs) Fase 11 (V5): el plano al lado del croquis (archivo, portapapeles, zoom, encuadre, ocultar)
│   │   ├── Storage/ConnectionStorageManager.cs     Extensible Storage: esquema MotorConexionesConnection (GUID fijo, v1)
│   │   ├── Transactions/OperationScope.cs          TransactionGroup + IFailuresPreprocessor + DialogBoxShowing
│   │   └── Logging/JsonLineLogger.cs               Una línea JSON por llamada en %LOCALAPPDATA%\MotorConexiones\log\
│   └── MotorConexiones.Tests/       xUnit (268 pruebas), solo Core, con los fixtures, la cercha sintética (Fakes/SyntheticTruss.cs), las del Hangar (Fakes/HangarTruss.cs, HangarTruss8b.cs, HangarTruss8c.cs) y el nudo en K de 34 mm (Fakes/KNode55.cs)
├── catalog/                         Plantillas oficiales del catálogo y sus miniaturas .png (deploy.ps1 copia las que falten al PC); ver catalog/LEEME.md
├── config/limits.json               Tolerancias y mínimos AISC 360 (J3.3, J3.4, J2.4), editable sin recompilar
├── config/choices.json              Fase 11: listas de los desplegables del panel y valores por defecto del arranque, editable sin recompilar
├── config/catalog.json              Carpetas del catálogo, tolerancia de casado (10°), aviso de desvío (5°), espejo, política de perfil,
│                                    agrupación de extremos (node_cluster_mm 10), "atraviesa el nudo" (node_axis_max_distance_mm 5) y
│                                    alcance de cara (node_face_reach_mm, 0 = medio canto de cada barra + 10; ronda 8b) y batch_single_undo
│                                    (Fase 9: true = el lote es una sola entrada de deshacer; false = una por nudo, plan B), plan_labels y
│                                    plan_ghosts (Fase 10: etiquetas pinchables y cartelas fantasma al planificar; false las desactiva sin recompilar)
├── docs/
│   ├── ENCARGO_MOTOR_CONEXIONES.md  El encargo completo, por fases
│   ├── guide.md                     Guía para la IA (la devuelve conn_get_guide), editable sin recompilar
│   ├── fixtures/                    detalle-D.json (con dudas), detalle-D-confirmado.json (dudas resueltas), detalle-D.png, cercha-vista-general.png;
│   │                                Fase 12: detalle-B, detalle-C, detalle-I (end_plate) y nudo-K-cordon-superior (direct_weld), .json + .png
│   ├── fases/                       fase-N.md (informe de cada fase), resultados-fase-N.md (salidas del PC), capturas/
│   ├── prompts/                     prompt y alcance de cada fase posterior al encargo (fase-6.md, fase-7.md, fase-7b.md, fase-8.md, fase-8c.md, fase-9.md, fase-10.md, fase-11.md, fase-12.md, fase-13.md)
│   ├── propuestas/                  Ideas por aclarar antes de programar (catalogo-y-lotes.md, flujo-intuitivo.md, encargo-ia-externa.md, placa-de-extremo.md, nudos-en-k-excentricos.md)
│   └── instalacion/                 Instrucciones literales para el agente instalador, una por fase
├── mcp/                             Archivos nuevos para la extensión revit-mcp (no se toca lo existente)
│   ├── revit_mcp/conexiones.py      Adaptador IronPython 2.7: 27 rutas /conn/... -> Bridge.Handle (Fase 13: /conn/split/ y /conn/unsplit/)
│   ├── tools/conn_tools.py          25 herramientas @mcp.tool() conn_* (CPython, SDK mcp 2.x; Fase 13: conn_split_member y conn_unsplit)
│   ├── CONTRATO-conn.md             Contrato de las rutas /conn/ (para pegar al final de CONTRATO.md de revit-mcp)
│   ├── instalar-conn.ps1            Copia los dos archivos a la extensión y añade las líneas de registro (idempotente)
│   └── pruebas/                     probar_conexiones.py (46 pruebas + 3 con --puente) y simulador_revit.py (solo nube; 101 comprobaciones)
└── scripts/
    ├── deploy.ps1                   Compila en Release y copia DLL, .addin, config\*.json y docs\guide.md a Addins\2027; plantillas de catalog\
    ├── revit-exec.ps1               Ejecuta un sondeo IronPython dentro de Revit (por /execute_code/ o -SinTransaccion)
    ├── conn-call.ps1                Llama a una operación del add-in por HTTP (ping o /conn/op/<operación>/)
    └── sondeos/                     00 a 29, 19b y capturar-nudo.py (sondeos para el instalador; 17 = marcas del plan, 18 = extremos en la cara, 19 y 19b = etiquetas en el lienzo, 20 = grupos anidados con Advance Steel, 21 = etiquetas del add-in en el lienzo, Fase 10; 22 = corte plano contra la cara, 23 = pernos a través de dos planchas, 24 = soldadura en el contorno, Fase 12; 25 = corte con BeamShortening de Advance Steel, 26 = WeldLine / WeldPoint, 27 = los parámetros y la caja real del perfil del cordón, ronda 12b; 28 = la viga de Advance Steel en todos los ensamblados, BeamShortening y WeldLine sin plancha, ronda 12c; 29 = los objetos del modelo de Advance Steel por DatabaseManager.GetModelObjectIds, la viga por Guid o geometría, BeamShortening y WeldLine conectada, ronda 12d; 30 = cómo se parte una barra en dos (CopyElement + LocationCurve.Curve o NewFamilyInstance), sus parámetros y el deshacer, Fase 13)
```

---

## 3. Requisitos previos (PC con Revit)

1. **Autodesk Revit 2027.2** (runtime .NET 10). En el PC de prueba `conn_ping` responde `backend: advancesteel`: los módulos
   de Advance Steel que instala Revit están disponibles. Si no lo estuvieran, el add-in usa `DirectShape`.
2. **SDK de .NET 10** (`dotnet --list-sdks` debe mostrar `10.x`): el instalador compila el add-in; los binarios no se suben.
3. **pyRevit** con soporte para 2027 y el servicio **Routes** activo (puerto 48884).
4. **Extensión revit-mcp** desplegada en `C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension`, con su `.venv`
   (Python 3.13 en el PC, `mcp` 2.2 y `httpx`) y `uv` en `%USERPROFILE%\.local\bin\uv.exe`.
5. **Git**, para traer la rama al PC.

---

## 4. Instalación paso a paso

Todos los comandos van en PowerShell desde la raíz del repositorio (`D:\Proyectos C#\CONEXIONES`; la ruta lleva
espacio y `#`, siempre entre comillas).

### Paso 1: compilar y pasar las pruebas

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
dotnet build MotorConexiones.sln -c Release
dotnet test MotorConexiones.sln -c Release --no-build
```

Se espera `0 Errores` y `Superado: 238`.

### Paso 2: desplegar el add-in (con Revit cerrado)

`deploy.ps1` compila en Release (salvo con `-NoBuild`) y copia `MotorConexiones.Core.dll`, `MotorConexiones.Revit.dll`,
`config\limits.json`, `config\catalog.json`, `config\choices.json` (Fase 11) y `docs\guide.md` a
`%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\`, más el manifiesto `MotorConexiones.addin`, y a la carpeta del
catálogo del usuario las plantillas de `catalog\` (y sus miniaturas `.png`) que falten. Revit bloquea la DLL mientras está abierto, así que ciérralo antes.

```powershell
.\scripts\deploy.ps1
```

Se espera `== MotorConexiones 0.11.0.0 desplegado en Revit 2027 ==`.

### Paso 3: instalar las rutas y herramientas del MCP en la extensión

```powershell
.\mcp\instalar-conn.ps1
```

Copia `mcp\revit_mcp\conexiones.py` y `mcp\tools\conn_tools.py` a la extensión y añade, si faltan, las dos líneas de
registro en `startup.py` y las dos en `tools\__init__.py`. Es idempotente: se puede repetir. Se espera
`(25 rutas @api.route)` y `(23 herramientas @mcp.tool)`.

### Paso 4: abrir Revit y comprobar el add-in

1. Abre **Revit 2027** con tu modelo (para las pruebas, la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`,
   nunca el original). Si Revit pregunta por el add-in sin firmar, pulsa *Always Load*.
2. En la pestaña **ARBA** debe aparecer el panel **MotorConexiones** con los botones **Ejecutar especificación JSON**,
   **Conexiones del modelo** y **Catálogo**. Si ARBA no se pudo usar, el panel está en la pestaña **Conexiones** (el
   motivo queda en el log, evento `ribbon_panel_created`).
3. Espera a que pyRevit cargue (unos 20 s; pyRevit solo lee `conexiones.py` al arrancar Revit) y comprueba:

   ```powershell
   .\scripts\conn-call.ps1 -Operation ping
   ```

   Se espera `ok: true`, `addin_version: 0.11.0`, `backend: advancesteel` y, en `operations`, las 23 operaciones.

### Paso 5: arrancar el puente MCP (puerto 8000)

El puente `main.py` de la extensión traduce las llamadas del cliente de IA a HTTP contra Revit. Se arranca a mano con
`C:\IA\iniciar_servidor_revit.bat`, que ejecuta en la carpeta de la extensión:

```powershell
cd "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
& "$env:USERPROFILE\.local\bin\uv.exe" run main.py --streamable-http
```

Deja esa ventana abierta: escucha en `http://127.0.0.1:8000/mcp`. Cada vez que reinstales `conn_tools.py` hay que
reiniciar el puente.

### Paso 6: conectar el cliente de IA

- **Antigravity (configuración probada, `resultados-fase-4.md` "4-9")**. Archivo
  `%USERPROFILE%\.gemini\config\mcp_config.json`:

  ```json
  {
    "mcpServers": {
      "revit": {
        "serverUrl": "http://localhost:8000/mcp"
      }
    }
  }
  ```

  Orden que funciona: abrir Revit con el modelo → arrancar el puente con `C:\IA\iniciar_servidor_revit.bat` → en
  Antigravity, recargar (o reiniciar) el servidor `revit` para que vuelva a pedir la lista de herramientas → deben
  aparecer las 13 `conn_*` (79 herramientas en total en el PC).

- **Claude Desktop (NO PROBADA en este proyecto)**. Claude Desktop lanza `main.py` como subproceso por stdio, no por
  HTTP. La configuración siguiente está escrita a partir de la documentación general de MCP y no se ha ejecutado:
  archivo `%APPDATA%\Claude\claude_desktop_config.json`:

  ```json
  {
    "mcpServers": {
      "revit": {
        "command": "C:\\Users\\<Usuario>\\.local\\bin\\uv.exe",
        "args": ["run", "--directory", "C:\\IA\\pyrevit-ext\\mcp-server-for-revit-python.extension", "main.py"]
      }
    }
  }
  ```

  Si la usas, anota en `docs/fases/resultados-fase-5.md` si funcionó.

---

## 5. Cómo actualizar el add-in

### Si cambió código C# (`src/`)

1. Cierra Revit (tiene la DLL bloqueada).
2. Compila, pasa las pruebas y despliega:

   ```powershell
   dotnet build MotorConexiones.sln -c Release
   dotnet test MotorConexiones.sln -c Release --no-build
   .\scripts\deploy.ps1 -NoBuild
   ```

3. Vuelve a abrir Revit.

### Si cambiaron los archivos del MCP (`mcp/`)

1. Cierra Revit y ejecuta `.\mcp\instalar-conn.ps1` (pyRevit solo recarga `conexiones.py` al arrancar Revit).
2. Vuelve a abrir Revit.
3. Cierra la ventana del puente y vuelve a arrancarlo (`C:\IA\iniciar_servidor_revit.bat`), y recarga el servidor
   `revit` en el cliente de IA para que vea las herramientas nuevas.

### Si solo cambió `config/limits.json`, `config/catalog.json`, `config/choices.json`, `docs/guide.md` o `catalog/*`

Basta `.\scripts\deploy.ps1 -NoBuild` (copia los archivos de configuración y la guía, y las plantillas y miniaturas de
`catalog/` que falten en el PC). Para `guide.md`, `catalog.json` y `choices.json` ni siquiera hace falta reiniciar Revit (el
add-in los lee en cada llamada o al abrir la ventana); para `limits.json`, ver la sección 6.

---

## 6. Cómo editar `limits.json` y `guide.md` sin recompilar

### `config/limits.json` (tolerancias y mínimos AISC)

El add-in lo lee en cada `validate`, `create` y `update` desde la carpeta desplegada
(`%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json`). Edita el del repositorio y vuelve a
desplegar con `.\scripts\deploy.ps1 -NoBuild`.

```json
{
  "schema_version": 1,
  "dimension_chain_tolerance_mm": 1.0,
  "label_value_tolerance_mm": 0.05,
  "angle_tolerance_deg": 1.0,
  "node_axis_max_distance_mm": 5.0,
  "node_axis_max_offset_mm": 0,
  "plate_outside_gusset_tolerance_mm": 2.0,
  "end_plate": { "min_bolt_to_tube_mm": 20, "min_hole_clearance_mm": 1 },
  "bolts": {
    "min_spacing_factor": 2.667,
    "edge_distance_mm": { "12.7": 19.0, "15.875": 22.0, "19.05": 25.0, "22.225": 28.0, "25.4": 32.0, "28.575": 38.0, "31.75": 42.0 },
    "length_addition_mm": { "12.7": 17.46, "15.875": 22.23, "19.05": 25.4, "22.225": 28.58, "25.4": 31.75, "28.575": 38.1, "31.75": 41.28, "default": 1.4 },
    "length_increment_mm": 6.35
  },
  "welds": {
    "min_fillet_mm": { "6.0": 3.0, "13.0": 5.0, "19.0": 6.0, "default": 8.0 }
  }
}
```

- `dimension_chain_tolerance_mm`: cuánto puede desviarse la suma de una cadena de cotas de su total.
- `label_value_tolerance_mm`: diferencia admitida entre un rótulo (`3/8"`) y su valor en mm (9.525).
- `bolts.edge_distance_mm`: distancia mínima al borde por diámetro (AISC 360, tabla J3.4); `min_spacing_factor` por `d`
  (J3.3).
- `bolts.length_addition_mm` y `bolts.length_increment_mm` (ronda 6b): longitud del perno = agarre (espesor de la
  cartela + espesor de la placa cuchilla) + suplemento por diámetro (tuerca, arandela y rosca sobrante, RCSC tabla
  C-2.1), redondeada hacia arriba a múltiplos del incremento (1/4" = 6,35 mm; pon 5 para pernos métricos). `default` es
  un factor sobre el diámetro para diámetros fuera de la tabla. Si el JSON trae `bolts.length_mm`, manda ese valor y el
  validador solo avisa (`BOLT_LENGTH_TOO_SHORT`) si es menor que agarre + suplemento.
- `plate_outside_gusset_tolerance_mm` (Fase 7): cuánto puede asomar una esquina de la placa cuchilla fuera del contorno
  de la cartela sin error `PLATE_OUTSIDE_GUSSET`. Desde la Fase 7 esa regla comprueba la placa donde está la barra de
  verdad (ángulo con signo del modelo); la placa del Detalle D termina justo en el chaflán y asoma unas décimas.
- `welds.min_fillet_mm`: filete mínimo según el espesor más delgado (J2.4); `default` para espesores mayores.
- `node_axis_max_offset_mm` (Fase 12, opción A): hasta `node_axis_max_distance_mm` la barra concurre; hasta este valor su eje
  puede cortar el cordón lejos del punto de trabajo con el aviso `MEMBER_ECCENTRIC` (nudo en K con excentricidad); por encima,
  `MEMBER_NOT_AT_NODE`. `0` = medio canto del cordón leído del modelo (152,4 en un HSS12X8); un número lo fija en mm.
- `max_auto_gusset_mm` (Fase 13, 1 200) y `auto_outline_max_vertices` (12): el lado máximo de la caja de la cartela automática antes
  del aviso `OUTLINE_AUTO_LARGE` (también avisa si el área dobla la de los rectángulos de las barras) y cuántos vértices conserva el
  contorno calculado. Cambiar `auto_outline_max_vertices` cambia los tokens de las especificaciones `auto`; `max_auto_gusset_mm` no.
- `end_plate.min_bolt_to_tube_mm` y `end_plate.min_hole_clearance_mm` (Fase 12): distancia mínima de la cara del tubo al eje de
  cada perno de la placa de extremo (media tuerca más arandela) y cuánto mayor que el perno debe ser el agujero (Ø18 para 5/8",
  Ø13/16" para 3/4"). Las claves nuevas solo entran en el hash del token cuando no valen lo de defecto.
- El hash SHA-256 de estos valores entra en el `validation_token`: si editas el archivo entre `conn_validate` y
  `conn_create`, el token deja de valer (`VALIDATION_TOKEN_INVALID`) y hay que volver a validar. La clave `_comentario`
  no cuenta.

### `config/choices.json` (listas del panel y valores del arranque, Fase 11)

Las listas de los desplegables del panel por secciones y lo que escribe **Conectar** cuando ninguna plantilla encaja. Se
copia a `config\` del add-in con `deploy.ps1` y se lee al abrir la previsualización (sin reiniciar Revit).

- `thickness_labels`: espesores de la cartela y de la placa cuchilla, en pulgadas y en PL (`1/4"` … `1"`, `PL6` … `PL20`).
  Cada rótulo se convierte a mm con la misma regla que el validador (`3/8"` = 9,525; `PL10` = 10) y el desplegable escribe
  los dos campos a la vez: nunca deja un `LABEL_VALUE_MISMATCH`.
- `bolt_diameter_labels`: por defecto `1/2"`, `5/8"`, `3/4"` y `7/8"`. Solo se ofrecen los que existen en `limits.json`
  (`bolts.edge_distance_mm`); uno que no esté se descarta con una línea en el log (`choices_bolt_labels_discarded`). Los demás
  diámetros de `limits.json` (`1"`, `1-1/8"`, `1-1/4"`) siguen aceptados escribiéndolos en **Avanzado**: el panel los enseña
  como "Otro: 25,4 mm" sin perderlos.
- `weld_sizes_mm`: tamaños de soldadura (3, 4, 5, 6, 8 y 10). `slot_length_mm` y `end_setback_mm`: ranura y retiro por defecto.
- `attachment_types`, `chord_interfaces`, `gusset_faces`, `roles`: los valores del contrato con su texto en español
  (**soldada en ranura**, **placa cuchilla atornillada**, **atraviesa el cordón ranurado**…).
- `starter`: lo que escribe el arranque si no hay una plantilla con el mismo perfil de cordón (decisión D10): espesor de la
  cartela, unión al cordón, soldadura, cuánto sobresale la cartela más allá de la ranura (`gusset_margin_mm`) y por el otro
  lado del cordón (`gusset_beyond_chord_mm`), y la placa cuchilla y los pernos con los que el panel crea el bloque al cambiar
  una barra a placa cuchilla atornillada.
- Fase 12: `hole_labels` (los agujeros de la placa de extremo: `Ø18`, `Ø13/16"`, `Ø15/16"`, `Ø21`), `diagonal_splice_offset_mm`
  (330: la distancia de la cara del cordón al empalme de la diagonal, Detalle B, que el nudo soldado dibuja y escribe en
  `splice_hint`), `starter.direct_weld` (filete de 5 mm en todo el contorno, holgura 0, la marca del empalme) y
  `starter.end_plate` (`gap_mm` entre planchas y `by_profile`: el detalle del plano por perfil del cordón, C para HSS12X8X1/2,
  I para HSS3X3X1/4, B para HSS2-1/2X2-1/2X3/16, y `default`, el Detalle I escalado para cualquier otro perfil).
- Fase 13: `starter.gusset_side_margin_mm` (30: cuánto sobresale la cartela automática a cada lado de la barra, junto a
  `gusset_margin_mm` al final de la ranura y `gusset_beyond_chord_mm` más allá del cordón) y `starter.outline_mode` (`auto` = el
  arranque escribe la cartela automática; `polygon` = el rectángulo de la Fase 11). Los márgenes solo se escriben en el JSON si
  difieren de los del Core (40 / 30 / 40).

Sin el archivo, el add-in usa listas embebidas iguales a estas: nunca un panel vacío.

### `docs/guide.md` (guía para la IA)

Es lo que devuelve `conn_get_guide`. El add-in lo lee del disco en cada llamada, así que cualquier cambio desplegado
surte efecto de inmediato, sin reiniciar Revit. Contiene el flujo obligatorio de 10 pasos, cómo leer un detalle de
acero, el sistema local, qué hacer con cada error y las reglas de seguridad.

---

## 7. Cómo agregar un tipo de conexión nuevo

Un tipo de conexión es una clase en `src/MotorConexiones.Core/Types/` que implementa la interfaz real
`IConnectionType` (`src/MotorConexiones.Core/Types/IConnectionType.cs`):

```csharp
public interface IConnectionType
{
    string Name { get; }          // nombre estable que va en connection_type, p. ej. "base_plate"
    string Description { get; }   // descripción corta en español (la ve la IA en conn_list_types)
    string GetSchemaJson();       // JSON Schema del tipo
    string GetExampleJson();      // ejemplo JSON completo y válido
}
```

`GussetNodeType.cs` es el modelo a seguir: un `Instance` estático, las cuatro propiedades y un método `Validate` propio
que llama a `SpecValidator`. Pasos para añadir, por ejemplo, `base_plate`:

1. **Core: el tipo.** `src/MotorConexiones.Core/Types/BasePlateType.cs`:

   ```csharp
   public sealed class BasePlateType : IConnectionType
   {
       public static BasePlateType Instance { get; } = new BasePlateType();
       public string Name => "base_plate";
       public string Description => "Placa base con pernos de anclaje para columna HSS o W.";
       public string GetSchemaJson() => JsonSchemaValidator.GetBasePlateSchemaJson();
       public string GetExampleJson() => "{ ... }";
   }
   ```

2. **Core: contrato, esquema y validación.** Sus clases de contrato en `Contract/`, el esquema en
   `Schema/JsonSchemaValidator.cs`, las reglas propias en `Validation/` y las pruebas xUnit en
   `src/MotorConexiones.Tests/` con un fixture en `docs/fixtures/`.

3. **Registro.** En el constructor estático de `src/MotorConexiones.Revit/Bridge.cs`, junto al tipo existente:

   ```csharp
   if (ConnectionTypeRegistry.Find("gusset_node") == null)
   {
       ConnectionTypeRegistry.Register(GussetNodeType.Instance);
   }
   ConnectionTypeRegistry.Register(BasePlateType.Instance);
   ```

   Con solo esto, `conn_list_types` y `conn_get_schema` ya lo devuelven (leen el registro).

4. **Revit: operaciones y modelado.** Desde la Fase 12 el despacho por `connection_type` ya está hecho: `Validate`,
   `Preview`, `Create` y `Update` llaman a `IConnectionType.Validate` del tipo registrado y `ConnectionCreationService`
   despacha a su constructor (`DirectWeldBuilder`, `EndPlateBuilder`; la cartela sigue en el servicio). Para un tipo nuevo:
   su `Validate` (apoyado en `ValidationCommon` para el nudo, los perfiles y los rótulos), su constructor en `Fabrication/`
   con los `IFabricationBackend` existentes (placas, pernos, soldaduras; Advance Steel y la reserva `DirectShape`), una rama
   en `PreviewOperation` y, si Conectar debe arrancarlo, su regla en `SpecStarter.TypeFor`. El almacenamiento
   (`ConnectionStorageManager`), el `OperationScope` y el registro se reutilizan tal cual.

5. **Croquis (opcional).** Si el tipo nuevo implementa también `ISketchProvider`
   (`src/MotorConexiones.Core/Sketch/ISketchProvider.cs`, un método `BuildSketch(spec, nodeInfo, limits)` que devuelve
   las primitivas en mm; `limits` es `config/limits.json` por si el croquis muestra valores calculados, como la longitud
   de los pernos), la ventana de previsualización lo dibuja sin cambios; si no, muestra "el tipo no aporta croquis".
   Cada `SketchDimension` lleva la ruta JSON del campo que mide: así el doble clic sobre la cota lo edita.

6. **Guía.** Describe el tipo nuevo en `docs/guide.md` para que la IA sepa cuándo usarlo.

---

## 8. Guion de prueba de punta a punta con la IA

El guion completo, con el prompt literal para pegar en Antigravity, está en `docs/instalacion/fase-5.md`, parte B.
Resumen del ciclo sobre la copia `HANGAR_PRUEBA_sondeo.rvt` y el fixture `docs/fixtures/detalle-D-confirmado.json`:

1. `conn_ping`: `ok: true`, `backend: advancesteel`, `document.title: HANGAR_PRUEBA_sondeo`.
2. `conn_get_guide` y `conn_list_types`.
3. `conn_get_node_info` con `[1249510, 1249630, 1249631, 1249636]` (cordón `1249510`).
4. `conn_get_schema` de `gusset_node`.
5. `conn_validate` con el fixture: `is_valid: true`, dos avisos `ANGLE_DIFFERS_FROM_MODEL`, token de 64 hex.
6. `conn_preview`: 1 cartela, 1 placa cuchilla, 4 pernos, 6 soldaduras, 3 barras retiradas.
7. **Confirmación explícita del usuario** y `conn_create` con la especificación y el token: `connection_id` y 9 IDs.
8. `conn_list` (1 conexión) y `conn_get` (especificación guardada). Mirar el nudo en Revit: placas y pernos de Advance
   Steel a la vista.
9. **Confirmación explícita** y `conn_delete`: `conn_list` vuelve a 0 y las barras recuperan sus extensiones.

Reglas para la IA durante la prueba: nunca `conn_create` ni `conn_delete` sin confirmación; nunca `execute_revit_code`
sobre lo que creó el add-in; si `conn_create` no responde, no repetirlo: `conn_list` dice si quedó creada.

---

## 9. Previsualizar y corregir antes de crear (botón de la cinta)

El botón **Ejecutar especificación JSON** (pestaña ARBA, panel MotorConexiones) ya no muestra un resumen de texto: abre la
ventana **Previsualización de conexión**, que sirve para ver el nudo dibujado con cotas, corregir valores y crear, con o
sin la IA. Funciona así:

1. Pulsa el botón y elige el archivo JSON (por ejemplo `docs\fixtures\detalle-D-confirmado.json`). Si el JSON no trae
   los `element_ids` del nudo y tienes las barras seleccionadas en Revit, se usan las seleccionadas.
2. **Izquierda, croquis 2D** del nudo en el plano de la cercha y en el sistema local (origen en el punto de trabajo, X a
   lo largo del cordón): eje y ancho del cordón y de cada barra, contorno de la cartela, ranuras, placa cuchilla, pernos,
   retiros (el extremo real de cada barra) y soldaduras en rojo. Cotas en mm con una cifra decimal: ancho y alto de la
   cartela, retiro de cada barra, largo de ranura, largo y ancho de la placa cuchilla, paso, borde y primera fila de los
   pernos; el espesor va como etiqueta (`PL 3/8" · 9,5 mm`) y los pernos llevan su propia etiqueta con el agarre y la
   longitud que se crearán (`4 pernos Ø5/8" · agarre 19,5 mm (cartela 9,5 + placa 10,0) · L 44,5 mm · placa en cara
   +z`). Rueda = zoom, botón central (o arrastrar) = encuadre, **Ajustar** = ver todo. Las direcciones y anchos de las
   barras salen del modelo; si el nudo no se puede leer, el croquis avisa de que son aproximadas.
   **Doble clic sobre una cota** (ronda 6b): el cursor se vuelve una mano encima de las cotas; al hacer doble clic
   aparece un cuadro junto a la cota con el nombre del campo, su ruta JSON y el valor actual; escribe el nuevo y pulsa
   Enter (Esc o un clic fuera cancelan). Es lo mismo que editar la fila en la tabla: el croquis se redibuja y se vuelve
   a validar. Las cotas de ancho y alto de la cartela miden el contorno, así que al cambiarlas el contorno se estira en
   ese eje alrededor del punto de trabajo (los puntos nuevos aparecen en el cuadro del contorno) y `width_mm`/`height_mm`
   toman el valor nuevo; la barra de estado lo explica.
3. **Derecha, tabla editable**: cordón, cartela, cada barra (rol, perfil, ángulo, retiro, tipo de unión, ranura o placa
   cuchilla con su cara de la cartela, pernos con su longitud opcional, soldaduras), cadenas de cotas y las **dudas**
   (`uncertain_fields`) con su `user_confirmed_value`.
   Doble clic en un valor, escribir y Enter: el croquis se redibuja y se vuelve a validar. Acepta coma o punto decimal
   (y tras escribir un entero como `9` se puede volver a escribir `12,7`: solo filas, columnas e ids son enteros);
   las listas van separadas por punto y coma (`75; 420; 70`). El contorno de la cartela se edita en el cuadro de abajo
   (un punto por línea, `x; y`) con **Aplicar contorno**. Al seleccionar una fila, su cota se resalta en naranja.
4. **Abajo, estado**: los mismos errores y avisos que ve la IA (código, campo, mensaje y sugerencia), el
   `validation_token` abreviado y los botones:
   - **Recargar**: vuelve a leer el archivo del disco (por si lo corrigió el chat o un editor).
   - **Guardar JSON**: escribe el JSON corregido junto al original con sufijo `-corregido.json`. Nunca escribe encima.
   - **Validar**: repite la validación contra el modelo.
   - **Crear**: solo se activa con la validación en verde. Vuelve a validar, usa el token recién calculado y crea la
     conexión igual que antes: una operación atómica, registro en Extensible Storage y diálogo con el `connection_id`.
   - **Cancelar**: cierra sin tocar el modelo.

Las reglas del contrato siguen mandando: si cambias el espesor de la cartela de 9,525 a 12,7 mm sin cambiar el rótulo
`3/8"`, la validación marca `LABEL_VALUE_MISMATCH` hasta que pongas `1/2"`; si pones un paso de pernos de 10 mm, marca
`BOLT_SPACING_TOO_SMALL` y **Crear** se desactiva. Nada de esto toca las rutas `conn_*`: la IA sigue sin ventanas.

---

### Conectar un nudo sin JSON, el panel por secciones y el plano al lado (Fase 11, add-in 0.11.0; probado en Revit el 2026-10-12)

**Conectar** (el primer botón del panel) es el camino sin archivos para un nudo suelto:

1. Selecciona en Revit el cordón y las barras que llegan al nudo (dos o más barras) y pulsa **Conectar**. Con una sola barra
   seleccionada, Revit te pide pinchar el cordón y las demás barras (Esc termina); con cero, un cuadro corto. Si la
   selección forma más de un nudo con cordón (una cercha), el cuadro lo dice y ofrece abrir **Planificar lote**.
2. El add-in lee el nudo (la misma regla que `conn_get_node_info`), casa **todas** las plantillas del catálogo y abre la
   previsualización con la mejor ya aplicada. La cabecera del panel dice "Plantilla propuesta: Nudo tipico Detalle D ·
   igual · desvío máx. 0,0°. Otras que encajan: ninguna." con su miniatura; **Cambiar…** lista las que encajan (miniatura,
   orientación y desvío) y vuelve a instanciar con otra. Si ninguna encaja: "Ninguna de las N plantillas encaja con 3
   barras a +45,0°, −45,0° y −135,0°: … Especificación de arranque con cartela rectangular (valores de la plantilla X /
   valores por defecto de choices.json)": la cartela rectangular envuelve los extremos recortados de las barras (retiro +
   ranura + margen, a múltiplos de 5 mm) y asoma por los dos lados del cordón, cada barra va **soldada en ranura** y la unión
   al cordón queda como duda confirmada (`through_slot`); el botón **Encargo para IA** de la cabecera escribe el encargo sobre
   ese mismo nudo. Es un punto de partida, no la cartela automática (Fase 13): la validación manda igual. Si el nudo ya
   tiene una conexión del add-in, la cabecera lo avisa (no bloquea).
3. **Panel por secciones** (derecha, pestaña Panel): **Cartela** (espesor como desplegable `3/8" · 9,5 mm`, ancho, alto,
   unión al cordón, soldadura y todo el contorno), **Cordón** (ID y perfil del modelo, continuo) y **Barra 1 · diagonal ·
   +45,0° · 1249630** (unión: **soldada en ranura** o **placa cuchilla atornillada**; retiro, ranura y soldadura; o placa con
   espesor `PL10 · 10,0 mm`, largo, ancho, inserción y cara, y pernos con Ø `5/8" · 15,9 mm`, filas, columnas, paso, borde,
   primera fila y longitud). Cada cambio redibuja el croquis y vuelve a validar, como la tabla; al entrar en un campo su
   cota se resalta en naranja; los errores con ruta se enseñan además junto al campo con el borde en rojo. Un valor que no
   está en la lista aparece como "Otro: 9,2 mm" y no se pierde. Al cambiar la unión de una barra a placa cuchilla aparecen
   placa y pernos con los valores por defecto de `config/choices.json`; al volver a soldada en ranura se recupera la
   ranura de antes (en esta sesión). Los IDs y el perfil del modelo son de solo lectura en el panel (se cambian en Avanzado).
4. **Avanzado**: la tabla de siempre, el contorno punto a punto con **Aplicar contorno**, las cadenas de cotas y las dudas,
   y el JSON actual (solo lectura, con **Copiar**).
5. **Plano…** (sobre el croquis): **Abrir archivo…** (PNG o JPG) o **Pegar del portapapeles** (un recorte con Win+Mayús+S,
   también de un PDF abierto; se guarda en `%LOCALAPPDATA%\MotorConexiones\planos\`). La imagen se abre a la izquierda del
   croquis con divisor, rueda = zoom, arrastrar = encuadre y **Ajustar**; **Ocultar plano** la pliega sin olvidarla. La ruta
   queda en `source.drawing_image` (y, al guardar en catálogo, en `origin.drawing_image` de la plantilla: al aplicarla, el
   plano se abre solo si el archivo existe; si no, lo dice la barra de estado). **Quitar** borra la ruta (no el archivo).
6. **Crear** y **Guardar en catálogo** son los de siempre; la plantilla guardada sale ya con su miniatura.

Desde la ventana del plan, **Editar nudo** abre la misma ventana y estrena el panel sin más.

### Conectar un nudo en K y un empalme: soldadura directa y placa de extremo (Fase 12, add-in 0.12.0; probado en Revit el 2026-10-21; ronda 12c, add-in 0.12.2, probada en Revit el 2026-10-23; ronda 12d probada en Revit el 2026-10-24 y cerrada con el add-in 0.12.4)

Conectar clasifica la selección con el mismo detector del plan: **dos tramos colineales que se encuentran** son un **empalme**
(`kind: splice`) y abren la previsualización con el arranque de **placa de extremo** del detalle que corresponde al perfil del
cordón (Detalle **C** para HSS12X8X1/2: plancha 430 × 330 con esquinas de 60 y 12 pernos Ø3/4"; **I** para HSS3X3X1/4: 200 y 4
pernos; **B** para HSS2-1/2X2-1/2X3/16: 185 y 4 pernos; el I escalado para otros); el panel tiene Plancha, Pernos (una fila por
cara con cantidad, paso y borde, el agujero `Ø13/16" · 20,6 mm` y las tuercas), Tramo 1, Tramo 2 (su retiro: espesor + holgura/2
menos el hueco que el tramo ya deja) y Empalme (holgura). **Un cordón con todas las barras al mismo lado** (los nudos en K del
cordón superior) abre el arranque de **soldadura directa**: cada diagonal cortada contra la cara del cordón desde su propio
corte, filete de 5 mm en todo el contorno, el gap entre cortes acotado (34 mm en el Hangar) y la **marca del empalme de la
diagonal a 330** (Detalle B; se dibuja y se anota, se fabrica en la Fase 13); la validación va en verde con el aviso
`MEMBER_ECCENTRIC`. Si en el punto hay **un empalme y un nudo en K** (N9 en la cercha de la 8c), Conectar abre primero el
empalme y después pregunta si abrir el nudo en K (cada diagonal se suelda al tramo al que llega su eje). **Cambiar…** ofrece,
además de las plantillas, **Tipo: cartela rectangular / soldadura directa / placa de extremo (arranque)** para forzar otro tipo
sobre el mismo nudo. Al crear, el nudo soldado acorta hoy cada barra por su eje (`END_CUT_APPROXIMATED`: el corte inclinado
espera a que el sondeo 29 encuentre la viga de Advance Steel; el 22 descartó el camino de Revit, el 25 y el 28 no la encontraron
y el 29, que recorre los objetos del modelo, es el último intento) y el
empalme crea las dos planchas, las filas de pernos como patrones de N × 1 (o DirectShape si Advance Steel las rechaza) y pernos
estándar (`BOLT_NUTS_APPROXIMATED`: el sondeo 23 confirmó que la tuerca a ambos lados no está en la API). **En Revit** (2026-10-21):
todo lo anterior funcionó: el nudo en K con 2 retiros y 2 soldaduras, el empalme con 14 elementos, el lote de 40 sin fallos
(`fase-12.md` 7.1); el cuadro Sí/No, una sola barra y media cercha se vieron en la 12b (8.1). Desde la **ronda 12c** (0.12.2) las
soldaduras son `WeldLine` de Advance Steel (conectadas a las planchas del empalme y de la cartela; en el nudo en K sin plancha
que conectar, con reserva DirectShape si Advance Steel no las materializa) y el HSS12X8X1/2 se lee como 203,2 en el plano ×
304,8 perpendicular (`Ht`, `b` y el giro de 90° del sondeo 27). **En Revit el 2026-10-23** (`fase-12.md` 9.1): las `WeldLine` se
materializaron en el nudo en K (2 elementos) y en el empalme (8: 2 planchas, 4 patrones y 2 soldaduras), el perfil se leyó 203,2
× 304,8 con giro 90 y el 430 salió en horizontal. Las soldaduras de Advance Steel se ven en Revit como **símbolos sin volumen y
solo en nivel de detalle Fino**: no faltan, `created_elements_count` las cuenta y Borrar las borra. Desde la **ronda 12d**
(0.12.3) cada advertencia de Revit al confirmar queda en el log (`revit_failure`) y el aviso `REVIT_WARNING` dice a qué elementos
apunta; en Revit (2026-10-24) resultó ser "The created elements are only visible in Detail Level: Fine.", una por soldadura, y
desde la **0.12.4** (cierre de la 12d) ya no sale como aviso: queda en el log con `filtered: true` y el cuadro del nudo en K solo
enseña `END_CUT_APPROXIMATED`. Nada de esto abre ventanas en las rutas `conn_*`.

### Cartela automática, Contorno y partir la diagonal (Fase 13, add-in 0.13.0; NO PROBADO en Revit)

- **Cartela automática.** Conectar sobre un nudo con cartela sin plantilla (o con el catálogo vacío) abre la previsualización con la
  cartela **calculada** desde las barras (`gusset.outline.mode: "auto"`, sin `points_mm`): la cabecera dice "arranque con cartela
  automática" y el croquis la dibuja con sus chaflanes. En la sección **Cartela** del panel, el desplegable **Contorno: Automático /
  Polígono**: en Automático los puntos no se editan (ni estirando las cotas); **Polígono** convierte (escribe los puntos calculados
  en `points_mm` y la caja en ancho y alto) y desde ahí se editan como siempre. En **Avanzado**, el cuadro del contorno enseña los
  puntos calculados en solo lectura y el botón **Convertir en polígono** hace lo mismo. Crear fabrica la cartela con los puntos
  calculados (los mismos que valida y dibuja). **Guardar en catálogo** desde `auto` guarda el modo: al aplicarla en otro nudo la
  cartela se recalcula con sus barras y casa con 25° de tolerancia (`auto_angle_tolerance_deg`). Si la cartela sale grande, aviso
  `OUTLINE_AUTO_LARGE`: Convertir en polígono y recortar.
- **La casilla de Conectar.** Sobre un nudo en K con `splice_hint` (los del cordón superior), la previsualización enseña abajo la
  casilla **"Partir la diagonal N y crear el empalme del Detalle B a 330"** (marcada por defecto; también desde Ejecutar
  especificación JSON). Con ella, **Crear** hace tres cosas en una sola entrada de deshacer: parte cada diagonal a la distancia del
  Detalle B (cara del cordón + 330 + espesor de la plancha, por el eje de la barra; el tramo corto conserva el id), crea el empalme
  `end_plate` del Detalle B sobre los dos tramos y crea el nudo en K sobre el tramo corto (vuelto a validar). El cuadro final dice
  las tres; si algo falla, se deshace **todo** y la diagonal sigue entera. Sin la casilla, como en la Fase 12.
- **Partir barra.** El botón nuevo: con el cordón y la diagonal seleccionados, un cuadro con un enlace por diagonal ("Partir la barra
  1249631 (HSS…) · corte a 485 mm del eje del cordón…") y **Otra distancia…** (el único cuadro con entrada); el resultado dice el
  tramo corto (el original) y el largo (el nuevo). Con un tramo ya partido seleccionado, el mismo botón **deshace** el corte (borra
  el tramo nuevo y devuelve la curva al original), solo si ninguna conexión usa los tramos (`SPLIT_IN_USE`). **Borrar** una conexión
  que pidió cortes (la cadena de Conectar) los deshace también; `conn_split_member` y `conn_unsplit` son lo mismo para la IA.
- **El plan.** Cada nudo en K con `splice_hint` lleva en su detalle las líneas "· Partir 1249631 a 330 (Detalle B): corte a 485 mm …
  → empalme N5B" y debajo las filas **N5B** / **N5B2** "● Empalme B (tras partir)"; **Crear N conexiones** parte, crea los empalmes B
  y después el nudo en K (y el empalme del cordón `N9E` antes que `N9`); **Borrar el lote** deshace los cortes al final. La cabecera
  lo dice: "Además se parten N diagonales a 330 (Detalle B) y se crean N empalmes B antes de sus nudos en K".

### Encargo para IA (ronda 9b, Fase 10, add-in 0.10.0; probado en Revit el 2026-10-07)

Para quien diseña el JSON con una IA del navegador (Claude) en vez de con el agente local: con el nudo seleccionado (el cordón
y las barras que llegan), el botón **Encargo para IA** calcula lo mismo que `conn_get_node_info`, escribe **un solo archivo
Markdown** en `%LOCALAPPDATA%\MotorConexiones\encargos\encargo-<documento>-<fecha-hora>.md` con, en este orden, el prompt
ya redactado (`docs/propuestas/encargo-ia-externa.md`, sección 3), los datos del nudo (IDs reales, perfiles, ángulos, origen
y ejes), el esquema (`data.example` de `conn_get_schema`), las reglas de lectura (secciones 2, 3 y 4 de `docs/guide.md`,
copiadas del archivo desplegado) y un ejemplo confirmado (la plantilla del catálogo con la misma cantidad de barras o, si no
hay, el Detalle D de la Fase 3 embebido en el add-in); lo copia al portapapeles, abre la carpeta y lo dice en un cuadro corto.
La persona lo pega en la IA **junto con la imagen del detalle** (un recorte del plano: el add-in no la pone), guarda el JSON
que recibe y lo aplica con **Ejecutar especificación JSON**. Con un solo tipo de conexión no pregunta el tipo; no hay
herramienta MCP (decisiones P1 a P3 de la propuesta). El texto lo escribe el Core (`Brief/DesignBriefWriter.cs`, probado en la
nube); el botón solo recoge los datos y guarda.

## 10. Borrar desde la cinta

El botón **Conexiones del modelo** (mismo panel) abre una ventana con las conexiones creadas por el add-in en el
documento, lo mismo que devuelve `conn_list`: `connection_id`, tipo, fecha, elementos creados, barras modificadas, barras
del nudo y backend. Selecciona una y pulsa **Borrar seleccionada**: tras confirmar, hace lo mismo que `conn_delete`
(borra solo lo que creó el add-in y devuelve a las barras sus extensiones originales) en una operación atómica. Si algo
falla, se deshace todo y la ventana lo dice. **Actualizar** vuelve a leer la lista. El borrado queda en el log como
`ribbon_delete`.

---

## 11. Catálogo de conexiones (Fase 7)

Una **plantilla** es una conexión guardada sin los IDs del nudo (`node`, `chord.element_id`, `members[].element_id`),
con el ángulo real de cada barra medido en el modelo (`member_pattern`: ranura, rol, ángulo con signo, lado, perfil) y
una regla de casado (`matching`: tolerancia de 10° y si se admite espejo). Vive como archivo
`%LOCALAPPDATA%\MotorConexiones\catalogo\<template_id>.json` (carpeta en `config\catalog.json`, `catalog_folder`); la
carpeta `catalog\` del repositorio guarda las "oficiales" y `deploy.ps1` copia al PC las que falten.

**Miniaturas (V4, Fase 11; probadas en Revit el 2026-10-12: ninguna gris).** Cada plantilla tiene al lado un PNG de 240×180 con su croquis sin cotas
(`<template_id>.png`, en la misma carpeta que su `.json`), pintado por el add-in con el mismo lienzo de la previsualización a
partir del croquis que calcula el Core sin modelo (los ángulos y perfiles del `member_pattern`). Se dibuja al abrir la ventana
del catálogo y al guardar una plantilla, y se rehace si falta o si el `.json` es más nuevo; con "Copiar también a la carpeta
compartida" el PNG va con el JSON, y `deploy.ps1` copia los `.png` de `catalog\` que falten. Se ve en la ventana del catálogo
(columna **Croquis** y vista grande de la elegida), en **Abrir del catálogo**, en **Cambiar…** del panel, en **Plantilla…** y en el
detalle del nudo de la ventana del plan (no en el mapa). Si no se puede dibujar, un hueco gris con el nombre y `thumbnail_failed`
en el log: nunca impide abrir una ventana. `conn_catalog_list` no cambia (la IA no ve imágenes).

**Marco canónico del nudo (decisión P5 de la propuesta).** Desde la Fase 7, `NodeFrame` orienta X hacia +X global y Y
hacia +Z global (hacia arriba en cerchas verticales), Z = X × Y: el mismo nudo da el mismo marco aunque el cordón esté
dibujado al revés, así las plantillas se pueden comparar entre nudos. `conn_get_node_info` devuelve `angle_in_plane_deg`
**con signo** (+45° arriba a la derecha, −135° abajo a la izquierda), `angle_to_chord_deg` (inclinación sin signo, la del
plano) y `side`. `expected_angle_deg` sigue siendo la inclinación del plano: la regla 8.6 compara sin signo. Ojo: en el
Hangar el cordón está dibujado hacia −X, así que el dibujo local se refleja en X respecto a las Fases 3 a 6 (la placa
cuchilla del Detalle D pasa a terminar en el chaflán inferior izquierdo, como en el plano) y `plate.gusset_face` `+z`
pasa a ser la otra cara física; `docs/fases/fase-7.md` lo detalla.

**Desde la IA** (`docs/guide.md`, sección 5): `conn_catalog_save` con el `connection_id` de una conexión creada (o con
`spec`) y un nombre → `template_id`; `conn_catalog_list` y `conn_catalog_get` para ver las plantillas;
`conn_catalog_apply` con `template_id` y las barras del nudo (o la selección) → casa cada barra con una ranura por su
ángulo probando `same`, `mirror_x`, `mirror_y` y `both`, escribe los IDs, transforma la cartela, pone el ángulo real en
`expected_angle_deg`, añade `source.template_id` y **ya valida**: `data.spec` y `data.validation_token` van tal cual a
`conn_preview` y `conn_create`. `TEMPLATE_NO_MATCH` explica en `data.attempts` qué ranura no encontró barra;
`TEMPLATE_ANGLE_DEVIATION` avisa si una barra se desvía más de `angle_deviation_warning_deg` (5°);
`TEMPLATE_PROFILE_DIFFERS` avisa si el perfil es otro (`profile_policy: warn` por defecto; `require` conserva el de la
plantilla y entonces `PROFILE_MISMATCH`; `ignore` escribe el del modelo sin avisar). `conn_catalog_delete` borra el archivo.

**Desde la cinta**: botón **Catálogo** (lista con búsqueda; **Aplicar a la selección** casa la plantilla con las barras
seleccionadas y abre la ventana de previsualización con la especificación instanciada, como archivo virtual cuyo
**Guardar JSON** escribe en `Documentos\MotorConexiones\`; **Crear** hace lo mismo que el botón de la sección 9;
**Guardar desde conexión…** convierte una conexión del modelo en plantilla; **Eliminar**). En la ventana de
previsualización, **Abrir del catálogo** aplica una plantilla al nudo abierto y **Guardar en catálogo** guarda la
especificación actual (solo con la validación en verde). Al guardar se puede marcar "Copiar también a la carpeta
compartida" (`shared_catalog_folder`, por defecto `catalog\` del repositorio).

Lo que **no** hace todavía: calcular la cartela según los ángulos (`outline.mode = "auto"`, Fase 13): la cartela de la
plantilla se copia tal cual y, si una barra se sale, `conn_validate` lo marca. Crear por lotes es la sección 12.

---

## 12. Planificar y crear un lote: los nudos de una cercha entera (Fases 8 y 9)

Con la cercha seleccionada (cordones, diagonales y montantes), el **plan de lote** hace el trabajo repetitivo antes de
crear nada: lleva cada extremo de barra al **corte de su eje con el eje del cordón** (ronda 8b: en la cercha real las
diagonales terminan en la cara del cordón, a 15–85 mm de su eje, y agrupando solo los extremos a 10 mm nada se juntaba;
un extremo se lleva al corte si está a menos de medio canto de cada barra más 10 mm de ese eje, `node_face_reach_mm` de
`config\catalog.json` lo fija a mano), agrupa esos puntos a menos de 10 mm (`node_cluster_mm`), reconoce la barra que
atraviesa cada punto como cordón (5 mm, `node_axis_max_distance_mm`), calcula el marco canónico y el ángulo con signo de
cada barra (y `end_gap_mm`: cuánto se queda corta respecto al punto de trabajo, que es la media de los cortes de las barras
del nudo con el cordón), nombra los nudos `N1, N2…` a lo largo de la cercha, salta los que ya
tienen conexión del add-in
(`replace_existing` para rehacerlos), casa cada nudo con las plantillas del catálogo (también en espejo), instancia la
especificación con `source.template_id` y `source.batch_id` y la valida con las mismas reglas y el mismo token que
`conn_validate`. Estados por nudo: `ready`, `invalid`, `no_match`, `ambiguous_chord`, `offset`, `untyped`,
`already_connected`, `excluded` y, desde la Fase 9, `created` y `failed`. **Planificar no crea ninguna conexión**: crear
es `conn_batch_create` o el botón **Crear N conexiones** (más abajo, "Crear el lote").

**Clases de nudo y arranques (Fase 12, add-in 0.12.0; probado en Revit el 2026-10-21: 41 nudos y 40 creadas sin fallos).** El detector distingue dos clases
(`nodes[].kind`): **`node`** (un cordón y las barras que llegan) y **`splice`** (dos tramos colineales de la misma barra que se
encuentran: un empalme; antes salía `untyped` o "sin cordón"). Un nudo en K que cae sobre un empalme del cordón son **dos
nudos en el mismo punto**: `N9` (el nudo, con el aviso `NODE_AT_SPLICE`; cada diagonal lleva `chord_element_id` = el tramo al
que llega su eje) y `N9E` (el empalme, con rombo en el mapa y un marcador propio); **Crear** hace primero el empalme. Si ninguna
plantilla de la clase del nudo encaja, el plan escribe una **especificación de arranque** (`starter: true`, `template_name`
"arranque direct_weld" o "arranque end_plate (Detalle C)"): soldadura directa en un nudo con todas las barras a un lado del
cordón, placa de extremo del detalle del perfil en un empalme; un nudo con barras a los dos lados (cartela) sigue saliendo
`no_match` hasta la cartela automática (Fase 13). La cabecera los cuenta aparte: "Se crearán 41 conexiones: 16 con Nudo tipico
Detalle D (8 iguales, 8 en espejo), 17 soldaduras directas (arranque), 7 empalmes (arranque Detalle C), 1 empalme (arranque
Detalle I). 14 avisan de perfil distinto. 2 avisan de excentricidad. 7 nudos en K están sobre un empalme." Los nudos en K
excéntricos salen `ready` con `eccentricity_mm` y `axis_offset_mm` por barra; el detalle del nudo enseña la clase, el tipo y la
excentricidad.

**Marcas en el modelo** (opción A de la propuesta): en la vista activa, las barras de cada nudo se colorean **con el color
de su estado** (ronda 8c: verde = se creará, ámbar = se creará con aviso, rojo = falta algo, gris = no se crea; el mismo
color que la ventana; línea y superficie, el cordón con línea gruesa; las barras sueltas y las parejas sin cordón no se
marcan) y en el punto de trabajo se pone un marcador pequeño con el nombre del nudo
(`DirectShape` de Modelos genéricos: cubo = misma orientación que la plantilla, rombo = en espejo; el nombre se ve en
Propiedades > `Comentarios`, que empieza por `N7 · …`; desde la ronda 8b no se escribe `Marca`, porque Revit avisaba
"Elements have duplicate Mark values"). Replanificar renueva las marcas; descartar las quita (y `conn_batch_plan_discard` con
`all: true` limpia marcas de planes que el add-in ya no recuerde y dice cuántos marcadores había). **En un documento solo
hay un plan marcado** (cierre de la Fase 8): marcar un plan nuevo, desde la IA o desde el botón, quita las marcas del
anterior con el aviso `PLAN_MARKS_REPLACED`; ese plan sigue en memoria y se vuelve a ver replanificándolo con su `plan_id`.
**Descartar plan** desde la ventana (0.8.4) hace con los marcadores lo mismo que `all: true`: quita los de cualquier plan y los
huérfanos, sin olvidar los demás planes (siguen en memoria sin marcas); después no queda ningún cubo ni rombo en el documento
y **la ventana se cierra** (cierre de la ronda 8d, 0.8.5: en la 8d se quedaba abierta). Las marcas son cambios de vista y elementos pequeños, nunca acero, y forman una sola entrada de deshacer.

**Desde la IA** (`docs/guide.md`, sección 6): `conn_batch_plan` (selección o `element_ids`, `template_ids` opcional) →
tabla de nudos → correcciones en `overrides` con el mismo `plan_id` (`exclude`/`include`, `chord`, `template`,
`remove_member`/`add_member`, `add_node`, `merge`, `split`, `spec`) → `conn_batch_plan_get` (con `node`, un nudo con su
especificación y su token) → `conn_batch_plan_discard`. `conn_batch_plan` devuelve por defecto los nudos sin la
especificación completa (`include_specs: false`), para no volcar 3 KB por nudo. El `overrides` de una respuesta se puede
devolver tal cual en la petición siguiente (desde 0.8.2 no lleva claves auxiliares). Desde la ronda 8c la respuesta trae
además `summary_text` (la decisión en español, la misma cabecera de la ventana), `visible_count`, `hidden_text` y, por nudo,
`status_text` (estado en español con icono), `advice` (qué hacer) y `visible_by_default`; `color_name` es el del estado. La
guía pide a la IA que enseñe `summary_text` y una tabla corta solo con los nudos visibles, nunca el JSON. Si el catálogo no
tiene plantillas, aviso `CATALOG_EMPTY`.

**Desde la cinta** (ronda 8c): botón **Planificar lote** (sin selección reabre el último plan del documento). La ventana
"MotorConexiones: conectar cercha (plan y lote)" tiene, de arriba abajo:

- **La cabecera con la decisión**: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de
  perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas." Con 0 listos dice "Ningún nudo listo: …" y la causa más
  frecuente. Debajo, el plan, las barras seleccionadas y las plantillas.
- **El mapa de la cercha**: el alzado con las barras en gris (el cordón más grueso) y un círculo por nudo del color de su
  estado con el número dentro. Clic en un círculo = elegir su fila; doble clic = **Ver en Revit**; pasar el ratón = "N4 ·
  Listo con aviso · Nudo tipico Detalle D · igual"; rueda = zoom; arrastrar = mover; **Ajustar** = ver toda la cercha. Los
  ocultos salen como círculos vacíos pequeños solo con **Mostrar ocultos**. El mapa se recalcula al replanificar.
- **La tabla, solo con los nudos de verdad** (los que tienen cordón): Nudo, Estado (● Listo, ▲ Listo con aviso, ✖ No
  valida, ✖ Sin plantilla que encaje, ✖ Falta el cordón, ✖ Dos cordones posibles, ✖ Los ejes no se cortan, ◌ Ya tiene
  conexión, ◌ Excluido, ○ Barra suelta), Espejo (no / sí), Plantilla, Desvío y **Qué hacer** (una frase por caso: "El
  cordón es HSS4X4… y la plantilla HSS3X3…: se creará con la misma cartela; exclúyelo si no quieres", "Falta el cordón en la
  selección: selecciónalo y replanifica, o Cordón…", "Una barra se sale de la cartela: Editar nudo y agrandarla, o
  excluir", "El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o
  excluye"…) **con botones debajo de la frase** (Fase 9, mejora C3: Excluir, Incluir, Incluir (rehacer), Cordón…, Barras…,
  Plantilla…, Editar nudo, Ver en Revit, Abrir catálogo, según el caso; cada uno hace lo mismo que la entrada del menú de
  clic derecho). Las barras sueltas y las parejas sin cordón van **ocultas por defecto**; la línea de encima las cuenta
  ("Ocultos: 20 sin cordón, 23 barras sueltas") y **Mostrar ocultos** las enseña en gris al final. Los nombres `N1…` no
  cambian (decisión P5). El id del cordón, los ids de las barras, cuánto se queda corta cada una (`end_gap_mm`) y el token
  abreviado se ven en el **detalle del nudo** (panel de abajo), no en la tabla.
- **Clic derecho sobre un nudo**: Ver en Revit, Editar nudo, Quitar edición, Excluir/Incluir, Cordón…, Barras…, Plantilla…
  (de una lista o **pinchando en Revit**: mientras pinchas, la ventana del plan se oculta y Revit queda delante; al
  terminar o con Esc vuelve sola, cierre de la ronda 8d). **Más…**: Añadir nudo… (pinchar las barras de
  un nudo que no se detectó), Guardar plan JSON (`Documentos\MotorConexiones\plan-<id>.json`, con el último informe),
  **Borrar el lote** (Fase 9) y Descartar plan (quita todos los colores y marcadores del documento, también los de otros
  planes, y olvida el plan; no toca las conexiones creadas).
- **Botones**: **Replanificar**, **Editar nudo** (abre la previsualización con la especificación del nudo; lo que se cambie
  vale solo para ese nudo; solo con nudos listos o que no validan: el botón en gris lo dice al pasar el ratón), **Ver en
  Revit** (selecciona las barras del nudo y su marcador y hace zoom en la vista activa, **sin ningún cuadro y sin cerrar la
  ventana**; doble clic en el mapa hace lo mismo), **Crear N conexiones** (Fase 9, en negrita; ver "Crear el lote") y
  **Cerrar** (deja las marcas y el plan para seguir después). Leyenda al pie: "Verde = se creará (o creada) · Ámbar = con
  aviso · Rojo = falta algo o falló · Gris = no se crea".
- **Catálogo vacío**: si no hay ninguna plantilla, la cabecera dice "Ningún nudo listo: no hay plantillas en el catálogo…",
  la barra de estado lleva el aviso `CATALOG_EMPTY` y aparece **Abrir catálogo** (al cerrarlo se replanifica).

Todos esos textos salen del Core (`PlanAdvice`), los mismos que recibe la IA. **La ventana se queda abierta** (cierre de la
ronda 8c, 0.8.4; opción B de la decisión P10, mejora C7 adelantada de la Fase 10): orbita, haz zoom y pincha en Revit con la
ventana a un lado. Como fuera de un comando Revit no admite su API, todo lo que toca el modelo (replanificar, marcas,
descartar, Ver en Revit, pinchar, Editar nudo, Abrir catálogo) pasa por un `ExternalEvent` (`Batch/PlanEvents.cs`): mientras
Revit trabaja, los botones se apagan y la barra de estado dice "⏳ …"; al terminar se encienden solos y la ventana se
repinta con lo que devolvió Revit (`PlanSnapshot`). La previsualización (Editar nudo) y el catálogo siguen siendo ventanas
modales cortas, abiertas dentro del evento. Hay una sola ventana del plan por sesión: pulsar **Planificar lote** con otra
selección la actualiza y sin selección la trae delante; si Revit está con una acción de la ventana, el botón lo dice. El
documento activo tiene que ser el del plan. Mientras Revit ejecuta una acción, la ventana no se puede cerrar (Esc cancela
una elección). **Cierre de la ronda 8d (0.8.5)**: en la 8d, **Cordón…** sobre un nudo que ya tenía cordón cerró Revit con
"fatal error"; la causa fue el diálogo de elección (`ChooseDialog`), que en modo de una sola elección marcaba el cordón
actual con `SelectedItems.Add`, cosa que WPF no admite y que, con la ventana no modal, ninguna captura recogía (en la 8c, con
la ventana modal, el mismo fallo solo cerraba la ventana: está en el log de ese día). Ahora el diálogo marca con
`SelectedItem`, **cada manejador de la ventana va dentro de una red** (`Guard`: el error sale en rojo en la barra de estado
y en el log como `ribbon_batch_window_error`, y la ventana sigue), hay otra red en el despachador de WPF que recoge solo las
excepciones de este add-in, **pinchar en Revit** (Cordón…, Barras…, Añadir nudo…) se hace con la ventana del plan **oculta**
y la ventana principal de Revit activada, con una línea en el log antes (`ribbon_batch_pick`) y otra después
(`ribbon_batch_picked` o `ribbon_batch_pick_failed`), y **Descartar plan cierra la ventana** al terminar (en la 8d `Close()`
se llamaba con la ventana todavía ocupada y el propio cierre lo cancelaba). **Todo esto se probó en el PC en la ronda 8e**
(2026-10-05, `resultados-fase-8e.md`; `fase-8.md`, sección 12): Cordón… sobre N9 seis veces y Barras… dos veces con la
ventana oculta y de vuelta, Esc cancela, Excluir / Incluir y Replanificar con la ventana abierta, Descartar cierra la
ventana; Revit siguió vivo y el log no tiene ningún `ribbon_batch_window_error`.

### Crear el lote y borrarlo (Fase 9, add-in 0.9.0, probado en Revit el 2026-10-06)

Con el plan revisado, **Crear N conexiones** (N = nudos listos, más los que fallaron en un intento anterior) abre un cuadro
de confirmación con la cabecera del plan y crea las conexiones **nudo a nudo con los tokens del plan**: cada nudo es la
misma operación atómica que `conn_create` (el token se comprueba contra el modelo; cartela, placas cuchilla, pernos,
soldaduras, retiros, elementos de Advance Steel adoptados, registro con `source.batch_id` = `plan_id` y
`source.template_id`), con su propio `TransactionGroup`, **anidado** en un grupo exterior del lote que se asimila al final:
**si un nudo falla se revierte solo y los demás se quedan** (decisión P9), y en Revit todo el lote es **una sola entrada de
deshacer** (Ctrl+Z lo deshace entero). Mientras Revit trabaja la ventana dice "⏳ Creando N conexiones…" (cada nudo abre
su sesión de Advance Steel; en el Hangar salieron a 0,15–0,25 s por nudo, 16 en 2,6 s). Al terminar, la cabecera dice "Creadas 15 conexiones (14 con
aviso), 1 falló", la tabla enseña `✔ Creada`, `▲`→`✔ Creada con aviso` y `✖ Falló al crear` con el motivo en *Qué hacer*
("Falló al crear (FABRICATION_FAILED: …): corrige y pulsa Crear otra vez (solo crea los que faltan), o excluye"), la barra
de estado lleva el informe ("Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: …). Una sola entrada de
deshacer (Ctrl+Z). 48,2 s.") y las marcas (color y marcador) de los nudos creados desaparecen; las de los demás siguen.
Los nudos `invalid`, `no_match` (también los empalmes), `excluded`, `untyped` y `already_connected` no se crean nunca
(decisión de la persona: los 10 nudos del cordón superior sin plantilla y los 7 empalmes quedan fuera); los "Listo con
aviso" por perfil distinto **sí** (P6: los 14 nudos con cordón HSS4X4 llevan la cartela del Detalle D). **Borrar el lote**
(botón y **Más…**) pide confirmación y borra todas las conexiones cuyo `source.batch_id` es el del plan, una a una con
las garantías de `conn_delete` (solo lo que creó el add-in; las barras recuperan su extensión), en una sola entrada de
deshacer, y replanifica. Si se replanifica con el lote creado, esos nudos salen `◌ Ya creada en este lote` con los
botones Ver en Revit e Incluir (rehacer). Desde la IA: `conn_batch_create` (`plan_id`, `nodes` con `validation_token`,
`stop_on_error`) devuelve el mismo informe por nudo (`outcome` `created` / `created_with_warnings` / `updated` /
`failed` / `skipped` / `rolled_back`, `summary_text`); `conn_batch_delete` (`batch_id`) borra el lote; `conn_list`
enseña `batch_id` por conexión y filtra por lote (`docs/guide.md`, sección 6). `config/catalog.json` lleva
`batch_single_undo`: `true` (grupo exterior, una entrada de deshacer) o `false` (plan B de la propuesta: una entrada por
nudo, por si el sondeo 20 dijera que los grupos anidados no conviven con Advance Steel). **Probado en Revit el
2026-10-06** sobre la copia del modelo (`resultados-fase-9.md`; `fase-9.md`, sección 7): el sondeo 20 (crear y borrar el
Detalle D dentro de un grupo exterior, asimilar y revertir) dio "GRUPOS ANIDADOS OK"; **Crear 16 conexiones** creó las 16
dos veces en 2,6 s y 2,4 s, con el informe en la ventana (captura `fase9-01`) y una sola entrada `batch_create` en Deshacer
(captura `fase9-04`); un solo Ctrl+Z las quitó (sondeos 12 y 13 a cero); por el puente, crear 2, saltar la segunda vez,
token alterado, sin token y `conn_batch_delete` (6 barras restauradas) salieron como se esperaba. **Borrar el lote desde la
ventana** quedó sin pulsar (la operación se probó por el puente): **hecho en la instalación de la Fase 10** (2026-10-07,
`fase-10.md` 7.5: `ribbon_batch_delete` con `deleted 16`, 48 barras restauradas, sondeos 12 y 13 a cero).

---

### Selección asistida, etiquetas pinchables y cartelas fantasma (Fase 10, add-in 0.10.0; en Revit el 2026-10-07; ronda 10b, add-in 0.10.1, en Revit el 2026-10-11)

Estado tras la ronda 10b (`docs/fases/fase-10.md`, secciones 7 y 8): **todo probado en Revit** sobre la cercha de la 8c. La
selección asistida funciona **por el puente** y **desde la cinta** (pinchar una diagonal y Planificar lote abre el cuadro
"Selección asistida: 63 barras tocan la selección" con Planificar con las 64 barras / Planificar solo las 1 seleccionadas /
Cancelar; el fallo de la 0.10.0, "Corresponding button not found: defaultButton", está corregido en la 0.10.1). Las 33
etiquetas y los 16 fantasmas se ven, **las etiquetas se pinchan sin ningún cuadro** (la etiqueta pasa a círculo blanco, la
ventana elige la fila; también con la ventana cerrada), el resaltado sigue a la tabla y al mapa, y Descartar plan lo deja todo
a cero. Capturas `fase10b-01` a `fase10b-05`. Lo que queda es cosmético y va de paso en la Fase 11 (`fase-10.md` 8.2).

- **Selección asistida (mejora C6)**: basta **pinchar una barra** de la cercha y pulsar **Planificar lote**. El add-in lee
  todas las barras de armazón estructural del documento y añade, por rondas, las que tocan la selección con la misma regla
  del detector (el extremo de una barra se corta con el eje de la otra a menos del alcance de cara; dos tramos paralelos
  pegados por los extremos son un empalme), **solo dentro del plano de la cercha** (correas, arriostrados y riostras entre
  cerchas quedan fuera; con una sola barra el plano lo votan las que la tocan). Si añade alguna, un cuadro dice cuántas y
  por qué ("Se añadieron 63 barras que tocan la selección: 8 cordones que pasan de largo, 54 barras que llegan y 1 tramo de
  cordón.") y ofrece **Planificar con las N barras**, **Planificar solo las M seleccionadas** o **Cancelar**; si no añade
  ninguna, planifica directamente. Con una barra sola y nada que la toque, lo dice. Desde la ventana, **Más… > Completar
  selección** hace lo mismo con la selección del plan y replanifica. Desde la IA, `conn_batch_plan` con `expand_selection:
  true` (la respuesta trae `selection_expansion` y el aviso `SELECTION_EXPANDED`; `docs/guide.md`, sección 6). Tope de 400
  barras (aviso en el texto). Todo esto solo lee el modelo; se probó en la nube con la cercha de la 8b (desde una diagonal se
  recuperan las 56 barras y el plan da los mismos 16 listos).
- **Etiquetas pinchables (mejora V3)**: al planificar con marcas, cada nudo visible lleva en su punto de trabajo una etiqueta
  redonda con su número y el color de su estado (un control con imagen en el lienzo de Revit: `TemporaryGraphicsManager`, lo
  que el sondeo 19 v4 demostró en el PC; el BMP es de 24 bits y 32×32, escrito a mano por el Core, en la primera carpeta sin
  tildes ni espacios de `%LOCALAPPDATA%\MotorConexiones\etiquetas`, `%PUBLIC%\MotorConexiones\etiquetas`,
  `C:\MotorConexiones\etiquetas` o la temporal, anotada en el log). Pasar el ratón enseña el globo del mapa. **Pinchar una
  etiqueta nunca abre un cuadro**: la etiqueta se resalta (círculo blanco con el número en color), la ventana del plan elige
  esa fila (enseñándola si estaba oculta) y escribe en su barra de estado "Etiqueta 4 pinchada en la vista: N4 · Listo · Nudo
  tipico Detalle D · igual. Ver en Revit encuadra el nudo". Elegir una fila en la tabla o en el mapa resalta su etiqueta en
  Revit. Las etiquetas no son elementos del modelo (ni transacción ni Deshacer; se van al cerrar el documento): se quitan al
  replanificar (se renuevan), al crear el lote (las de los nudos creados, con sus marcas; tras un Ctrl+Z las marcas vuelven y
  las etiquetas no: Replanificar las repone), al descartar y con `conn_batch_plan_discard` (`removed_labels`); Descartar plan y
  `discard all` no dejan ninguna. Si no se pudieron poner, aviso `PLAN_LABELS_SKIPPED` y el plan sigue. El manejador de clics
  se registra al arrancar el add-in (y en el botón Planificar lote por si acaso); el sondeo 21 cuenta los controles, el
  servidor y los BMP. `plan_labels: false` en `config\catalog.json` las desactiva sin recompilar.
- **Cartelas fantasma (mejora V2)**: con las marcas, en cada nudo con especificación (listo, listo con aviso, fallido o que
  no valida) se dibuja un `DirectShape` transparente con el contorno y el espesor de su cartela, en el marco del nudo leído del
  modelo (el mismo que usa la fabricación) y del color de su estado: ves en 3D lo que se va a crear antes de crearlo.
  **Crear** la quita con las marcas del nudo creado (el acero la sustituye; Ctrl+Z la devuelve con las marcas), **Descartar**
  y `discard all` las quitan todas (llevan el `ApplicationId` de los marcadores y `ApplicationDataId` `<plan>:<nudo>:ghost`).
  Solo la cartela, sin la placa cuchilla. `plan_ghosts: false` las desactiva.

## 13. Cómo probar sin Revit

Lo que se puede ejecutar en cualquier máquina (Linux, macOS o Windows) sin Revit ni pyRevit:

```bash
dotnet build MotorConexiones.sln -c Release          # Core, Revit y Tests (0 avisos)
dotnet test MotorConexiones.sln -c Release --no-build  # 291 pruebas del Core (croquis, editor, pernos, catálogo, plan de lote, cerchas del Hangar, textos y mapa del plan, crear por lotes, selección asistida, etiqueta, encargo; Fase 12: excentricidad, direct_weld, end_plate, la cercha de la 8c)
python3 -m py_compile mcp/revit_mcp/conexiones.py mcp/tools/conn_tools.py mcp/pruebas/*.py scripts/sondeos/*.py
```

**`mcp/pruebas/simulador_revit.py`** (solo para desarrollo; nada lo copia a la extensión) carga el `conexiones.py` real
con módulos `pyrevit`, `clr` y `System` simulados, sustituye `Bridge.Handle` por una imitación en Python del add-in
(mismas operaciones y códigos de error, datos del nudo del Detalle D y, Fase 12, del nudo en K y los empalmes del cordón superior) y sirve las rutas `/revit_mcp/conn/...` en el
puerto 48884 con token, igual que pyRevit Routes. Con `--extension <clon de revit-mcp>` usa el `seguridad.py` real.

```bash
# 1) Autocomprobación en proceso: 25 rutas, token, dev_exec, acentos, catálogo, plan de lote, catálogo vacío, crear y borrar el lote, etiquetas, selección asistida y, Fase 12, tres tipos, excentricidad y arranques (82 comprobaciones)
python3 mcp/pruebas/simulador_revit.py --autocomprobar

# 2) Servir el simulador y pasarle el script de pruebas (35 pruebas)
python3 mcp/pruebas/simulador_revit.py &        # escribe el token en el archivo literal "%LOCALAPPDATA%\RevitMcp\token"
python3 mcp/pruebas/probar_conexiones.py        #   de la carpeta actual, que es el que abre el script en Linux

# 3) Con el puente real (38 pruebas): clon de revit-mcp con conn_tools.py copiado y registrado en tools/__init__.py,
#    un venv con "mcp[cli]>=2.2,<3" y httpx, y "python main.py --streamable-http" en la carpeta del clon
python3 mcp/pruebas/probar_conexiones.py --puente
```

Lo que prueba de verdad: el adaptador, la forma de las peticiones, el script de pruebas, las 23 herramientas y el puente.
Lo que **no** prueba: nada de lo que pasa dentro de Revit (geometría, Advance Steel, almacenamiento). Eso solo lo prueba
el instalador en el PC con `docs/instalacion/fase-N.md`.

---

## 14. Errores más comunes

| Código | Significado | Qué hacer |
|---|---|---|
| `VALIDATION_TOKEN_INVALID` | Falta el token o no coincide: la especificación, el documento, las barras del nudo o `limits.json` cambiaron desde `conn_validate`. No caduca por tiempo. | Volver a llamar a `conn_validate` con la misma especificación y usar el token nuevo. |
| `UNRESOLVED_UNCERTAINTY` | Hay entradas en `uncertain_fields` sin `user_confirmed_value`. | Preguntar al usuario y poner el valor confirmado. |
| `DIMENSION_CHAIN_MISMATCH` | Una cadena de cotas no suma su total (tolerancia de `limits.json`). | Releer el plano o llevar la cota a `uncertain_fields`. |
| `LABEL_VALUE_MISMATCH` | Un rótulo (`3/8"`, `PL10`) no coincide con su valor en mm. | Corregir el número o el rótulo. |
| `PROFILE_MISMATCH` | El perfil escrito no coincide con el tipo del elemento; la respuesta sugiere los más parecidos. | Usar `conn_find_profile` y corregir. |
| `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL`, `BOLT_OUTSIDE_PLATE` | Pernos fuera de los mínimos AISC o fuera de la placa. | Revisar la cota en el plano o los mínimos de `limits.json`. |
| `NODE_AXES_NOT_INTERSECTING` | Los ejes del cordón y del primer miembro distan más de 5 mm. | Avisar al usuario: hay que ajustar el modelo. |
| `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE` | Un ID no existe, no es armazón estructural o no llega al nudo. | Volver a `conn_get_node_info` con la selección correcta. |
| `REVIT_BUSY` | Hay una orden abierta en Revit o el documento es de solo lectura. | Cerrar la orden en Revit y repetir. |
| `UNKNOWN_OPERATION` | Operación o tipo de conexión no registrado (también lo da `conn_get_schema` con un tipo inexistente). | Ver `operations` en `conn_ping` o `conn_list_types`. |
| `ADDIN_NOT_LOADED`, `NO_DOCUMENT` | El add-in no está en Revit, o no hay documento abierto. | `scripts\deploy.ps1` con Revit cerrado; abrir un modelo. |
| `REVIT_UNREACHABLE`, `CONN_ROUTE_NOT_FOUND`, `REVIT_RESTARTED`, `REVIT_TIMEOUT` | Los genera el puente: Revit cerrado, `conexiones.py` sin instalar, token cambiado o tiempo agotado. | Abrir Revit; `mcp\instalar-conn.ps1`; reintentar; comprobar con `conn_list`. |
| `TEMPLATE_NOT_FOUND`, `TEMPLATE_EXISTS`, `TEMPLATE_INVALID` | Catálogo (Fase 7): el `template_id` no existe, ya hay una plantilla con ese nombre (falta `overwrite: true`) o el archivo no se puede leer. | `conn_catalog_list`; otro nombre o `overwrite`; corregir o borrar el archivo. |
| `TEMPLATE_SPEC_INVALID`, `TEMPLATE_HAS_OPEN_UNCERTAINTIES` | Lo que se quiere guardar como plantilla no valida o tiene dudas sin confirmar. | Corregir con `conn_validate`; confirmar las dudas. |
| `TEMPLATE_NO_MATCH` | Ninguna orientación casa todas las ranuras con las barras seleccionadas. | Revisar la selección (`data.attempts` dice qué falta) o usar otra plantilla. |
| `PLAN_NOT_FOUND` | Plan de lote (Fase 8): no hay ningún plan con ese `plan_id` en memoria (se descartó o Revit se reinició). | Volver a `conn_batch_plan` con la cercha seleccionada; `conn_batch_plan_discard` con `all: true` si quedaron marcas. |
| `PLAN_MARKS_SKIPPED` (aviso) | La vista activa no admite colores por elemento (plantilla de vista, plano). | Abrir una vista 3D y replanificar. |
| `PLAN_MARKS_REPLACED` (aviso, 0.8.2) | Al marcar este plan se quitaron las marcas de otro plan del mismo documento (sigue en memoria sin marcas): en un documento solo hay un plan marcado. | Nada; para volver a ver el otro plan, replanificarlo con su `plan_id`; para olvidarlo, `conn_batch_plan_discard`. |
| `CATALOG_EMPTY` (aviso, ronda 8c) | El catálogo no tiene ninguna plantilla: ningún nudo puede casar y todos salen `no_match` ("✖ Sin plantillas en el catálogo"). | Crear primero la conexión de un nudo y guardarla con **Guardar en catálogo** (o `conn_catalog_save`); la ventana ofrece **Abrir catálogo**. |
| `NODE_CHORD_NOT_CONTINUOUS` (aviso por nudo, ronda 8b) | Ninguna barra atraviesa el nudo: el cordón es la más horizontal de las que llegan. | Si es un extremo de cercha, nada; si falta el cordón en la selección, seleccionarlo y replanificar (o `overrides.chord`); si es un empalme (otro tramo del cordón sigue por el otro lado), ese nudo no tiene plantilla: crear su típica o excluir. |
| `BATCH_STOPPED` (Fase 9) | `conn_batch_create` con `stop_on_error: true`: un nudo falló y el lote entero se revirtió. El informe va en `data`. | Mirar `data.nodes` (el nudo con `outcome: failed` y sus `errors`), corregir o excluir ese nudo y volver a crear. |
| `BATCH_NODE_FAILED` (aviso, Fase 9) | Un nudo del lote falló y se revirtió solo; los demás siguen. | En el plan queda `failed` con su especificación y su token: corregir y pulsar Crear otra vez (solo crea los que faltan), o excluir. |
| `BATCH_EMPTY` (aviso, Fase 9) | `conn_batch_delete` no encontró ninguna conexión con ese `batch_id`. | `conn_list` enseña el `batch_id` de cada conexión y `batches` cuenta por lote. |
| `BATCH_UNDO_SPLIT` (aviso, Fase 9) | El grupo exterior del lote no se pudo abrir: cada nudo quedó como una entrada de deshacer propia. | Nada que corregir en el modelo; anotarlo (es el plan B de la propuesta, `batch_single_undo: false`). |
| `SELECTION_EXPANDED` (aviso, Fase 10) | La selección asistida añadió barras a la selección y el plan se calculó con todas; el mensaje dice cuántas y por qué. | Nada: es informativo. Si no se quería, planificar con `expand_selection: false` (desde el botón, "Planificar solo las seleccionadas"). |
| `CATALOG_EMPTY` (error en `conn_catalog_apply` sin `template_id`, Fase 11) | El catálogo no tiene ninguna plantilla: no hay nada que aplicar. | Crear un nudo y guardarlo como plantilla (Catálogo o `conn_catalog_save`), o usar Conectar, que abre una especificación de arranque. |
| `TEMPLATE_NO_MATCH` sin `template_id` (Fase 11) | Ninguna plantilla casa todas sus ranuras con las barras del nudo sin dejar barras sobrantes; `data.proposal.summary_text` dice por qué. | Comprobar la selección (el cordón y todas las barras), crear la típica de ese nudo o pasar `template_id` para forzar una (el plan de lote sí admite barras sobrantes). |
| `thumbnail_failed`, `drawing_pane_failed` (solo en el log, Fase 11) | No se pudo dibujar la miniatura de una plantilla (hueco gris) o abrir una imagen del plano. | Nada bloquea: mirar el log; abrir otra imagen (PNG o JPG) o pegar un recorte. |
| `PLAN_LABELS_SKIPPED` (aviso, Fase 10) | Las etiquetas del lienzo no se pudieron poner o quitar (la vista no las admite o el control falló); el plan sigue con colores, marcadores y cartelas fantasma. | Mirar `labels_failed` en el log; abrir una vista 3D y replanificar; `plan_labels: false` en `catalog.json` las desactiva. |
| `MEMBER_ECCENTRIC` (aviso, Fase 12) | Nudo en K con excentricidad: el eje de una barra corta el cordón lejos del punto de trabajo, hasta medio canto del cordón (`node_axis_max_offset_mm`). | Nada: se crea igual, cada barra recortada sobre su propio eje. Por encima de medio canto sería `MEMBER_NOT_AT_NODE`. |
| `NODE_AT_SPLICE` (aviso por nudo, Fase 12) | El nudo en K está sobre un empalme del cordón: hay dos nudos en el mismo punto (N9 y N9E) y cada diagonal se suelda al tramo al que llega. | Nada: el lote crea primero el empalme. |
| `SPLICE_NOT_COLLINEAR`, `SPLICE_SETBACK_TOO_SMALL`, `SPLICE_PROFILES_DIFFER` (Fase 12, `end_plate`) | Los dos tramos forman más de 5° (no es un empalme); el retiro no deja sitio a su plancha (espesor + holgura/2 menos el hueco medido); los tramos son de perfiles distintos (aviso: manda el mayor). | Revisar la selección (dos tramos de la misma barra); subir `end_setback_mm` a lo que dice el mensaje; comprobar el perfil en el plano. |
| `BOLT_HOLE_TOO_SMALL`, `BOLT_TOO_CLOSE_TO_TUBE`, `PLATE_SMALLER_THAN_TUBE` (Fase 12, `end_plate`) | El agujero no es mayor que el perno (Ø18 para 5/8", Ø13/16" para 3/4"); un perno queda a menos de `min_bolt_to_tube_mm` de la cara del tubo; la plancha no cubre la huella del tubo. | Corregir `hole_mm` / `hole_label`; acercar la fila al borde o agrandar la plancha; agrandar la plancha o revisar el perfil. |
| `MEMBER_WIDER_THAN_CHORD_FACE`, `MEMBERS_OVERLAP_ON_FACE` (avisos, Fase 12, `direct_weld`) | La barra es más ancha que la cara del cordón contra la que se corta; las huellas de dos barras se pisan en la cara. | Revisar el perfil o la orientación del cordón; separar las diagonales o aceptar la soldadura compartida. |
| `END_CUT_APPROXIMATED`, `BOLT_NUTS_APPROXIMATED` (avisos al crear, Fase 12) | El corte inclinado contra la cara no está disponible todavía (la barra se acortó por su eje desde su propio corte); la tuerca a ambos lados no está en la API (perno estándar a través de las dos planchas). | Nada: informativos. La tuerca doble no está en la API (sondeo 23). El corte inclinado queda **como está, definitivo** (cierre de la 12d, 2026-10-24): el sondeo 29, el último intento, tampoco encontró la viga de Advance Steel; la barra se retira por su eje y el aviso lo dice. |

La tabla completa, con `path`, `message` y `hint`, está en `mcp/CONTRATO-conn.md` y en `docs/guide.md`.

---

## 15. Licencia

No se ha definido una licencia. Uso interno del autor del repositorio.
