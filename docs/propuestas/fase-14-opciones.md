# Propuesta: opciones para la Fase 14 (qué hacer después de cerrar la Fase 13)

Fecha: 2026-10-31. Estado: **dudas respondidas el mismo día (sección 8): la persona aceptó todas las recomendaciones; la propuesta
queda cerrada y lo siguiente es la sesión corta de los sondeos 31 y 32** (`docs/fases/resumen-fase-14-propuesta.md`, sección 3).
Sin código. Sale del cierre de la ronda 13c (`docs/fases/fase-13.md`, secciones 9.4 y 9.5,
y `docs/fases/resumen-fase-13c-cierre.md`, sección 3): la Fase 13 quedó cerrada del todo con el add-in **0.13.2** probado en Revit y
toca elegir la fase siguiente. Este documento compara los candidatos que las fases anteriores fueron dejando apuntados, dice qué
aporta cada uno a la cercha del Hangar, cuánto cuesta, qué sondeos pide y en qué orden conviene hacerlos; termina con las dudas
numeradas para la persona, con una recomendación en cada una. Con las respuestas, la sesión siguiente escribe
`docs/prompts/fase-14.md`.

Qué quiere decir cada palabra de coste en este documento:

- **Sesión**: una sesión de Claude Code en la nube que programa una fase entera (una fase por sesión, sección 13 del encargo).
- **Ronda**: una instalación en el PC con el agente instalador (`docs/instalacion/fase-N.md`) más la sesión corta de cierre que
  contrasta los resultados. Para calibrar: la Fase 11 costó 1 sesión + 1 ronda; la 12, 1 sesión + 4 rondas (tres de ellas
  persiguiendo la viga de Advance Steel); la 13, 1 sesión + 3 rondas.

## 0. Resumen (la recomendación en pocas líneas)

- **Fase 14 = `end_plate` contra un apoyo (`kind: "to_support"`)**, con dos retoques de robustez de paso: el reintento de la
  `FabricationTransaction` al borrar conexiones seguidas y `conn-call.ps1` con salida solo JSON. **1 sesión + 2 rondas**, más una
  ronda corta de solo sondeos antes de escribir el prompt (duda D14).
- En la **misma instalación**, sin código: el sondeo 31 (inventario de la cercha: cordón inferior, montantes, apoyos del eje B), el
  sondeo 32 (columnas, bases y la API de anclajes, para preparar `base_plate`) y **el lote entero del Hangar con sus cortes** sobre
  la copia, que nunca se ha creado de una vez.
- **Fase 15 = `base_plate`**, si el sondeo 32 dice que hay columnas con base y qué hay debajo; si no, se aplaza.
- **Después**, por este orden y solo si hace falta: partir cordones desde el plan (cuando aparezca un cordón modelado de una pieza),
  V6 (edición arrastrando) y el traspaso de `mcp/` a `revit-mcp` cuando las herramientas `conn_*` dejen de cambiar en cada fase.

## 1. Dónde estamos: lo que la cercha del Hangar ya tiene y lo que le falta

La cercha de la 8c (64 barras: el cordón superior HSS12X8X1/2 en ocho tramos, el cordón central HSS3X3X1/4 en ocho tramos y 48
diagonales HSS2-1/2X2-1/2X3/16; nudos N1 a N59 del plan) se conecta hoy así con el add-in 0.13.2:

| Nudos del plan | Qué son | Estado con la 0.13.2 |
|---|---|---|
| 16 nudos del cordón central | Cartela del Detalle D (plantilla) | **Se crean en lote** desde la Fase 9 |
| 15 nudos en K del cordón superior (8 solos: N5, N12, …, N54; 7 en el mismo punto que un empalme: N9, N16, …, N52) | Soldadura directa (`direct_weld`) con cada diagonal partida a 330 y su empalme del Detalle B | **Se crean** con la cadena de Conectar (Fase 13, probada en la 13c con las tres barras) y con los pasos `split` del plan |
| 7 empalmes del cordón superior (N9E, N16E, …) | Placa de extremo del Detalle C (`end_plate`, `kind: "splice"`) | **Se crean** desde la Fase 12 (el lote de la 8c hizo 7 + 1) |
| N49 y N58 | Una sola diagonal soldada al cordón superior | **Se crean** (`direct_weld` de una barra, D4 de la 12) |
| 8 puntos "sin cordón" de abajo (Z ≈ 14 914) | 7 parejas diagonal + montante a 91° y el extremo N1 | **Ocultos**: no hay cordón en la selección (`fase-8.md` 10.2); nunca se ha sabido si el cordón inferior existe como barra (pendiente desde la 8d; D10 de la 13 lo dejó fuera) |
| Montantes contra una cara | El alzado del nudo (`cordon-superior-nudo-k.png`) enseña un montante con plancha y pernos contra el cordón | **No hay tipo**: `end_plate` solo sabe empalmar; `kind: "to_support"` está reservado en el esquema (D11 de la 12) |
| Apoyos de la cercha (eje B) y bases de columna | Detalle H1 (diagonal contra una viga, dos planchas); columnas HSS12X6X1/2 del alzado | **No hay tipo** (`to_support` / `base_plate`) |

Es decir: **todo lo que hay entre el cordón superior y el central se fabrica ya**; lo que falta está en los bordes (montantes, apoyos,
columnas) y abajo (el cordón inferior, si existe). Y hay una prueba que nunca se hizo: la cercha entera de una vez con sus cortes.

Pendientes que **no** son candidatos a fase y van de paso cuando una fase los toque: `conn_batch_update`, `plates.second` (planchas
distintas en los dos tramos de un empalme), `thumbnail_path` en `conn_catalog_list`, `conn_design_brief` (P4 del encargo), la lectura
directa de PDF y la comprobación resistente (fuera del encargo, sección 14).

## 2. Los candidatos, uno a uno

### A. `end_plate` contra un apoyo: `kind: "to_support"` (el montante contra una cara)

**Qué es.** La placa de extremo de la Fase 12 cuando la barra no se empalma consigo misma sino que **termina contra la cara de otra
barra**: una plancha soldada al extremo de la barra, otra soldada a la cara del apoyo, las dos cara a cara y unidas por pernos con
tuerca a ambos lados (Detalle H1 del plano; `docs/propuestas/placa-de-extremo.md`, secciones 1 y 3). Es la variante que la Fase 12
dejó reservada en el esquema (`kind`) y fuera de alcance por D11.

**Qué aporta al Hangar.** Los montantes que llegan con plancha y pernos a la cara del cordón (el alzado del nudo en K enseña uno;
cuántos hay en el modelo y a qué cordón llegan es la duda D2) y los apoyos de la cercha en el eje B (Detalle H1: la diagonal contra la
viga de apoyo). Son las primeras conexiones nuevas de verdad desde la 13: hoy esos extremos salen como soldadura directa de una barra
(la regla D4 de la 12) o `untyped`, que no es lo que dice el plano.

**Qué se reutiliza (casi todo).** El contrato de `end_plate` (plancha octogonal con esquinas recortadas, `BoltRing` por cara,
`BoltStack.ForTwoPlates`, el aviso `BOLT_NUTS_APPROXIMATED`), `EndPlateBuilder` (dos planchas, patrones N × 1, `WeldLine`), el retiro
del extremo por el eje, el croquis, el panel por tipo, las plantillas y el arranque por perfil (el Detalle B de 185 para el montante,
decisión P7 de `nudos-en-k-excentricos.md`).

**Qué es nuevo.**

1. El contrato: `support` {`element_id`, `profile`, `face`} en lugar del segundo tramo; la plancha del apoyo puede tener otro tamaño
   (es el hueco `plates.second` que la 12 reservó).
2. El marco: X es el eje de la barra que termina; la plancha del apoyo va **sobre la cara** del apoyo. En un montante a 90° coincide
   con "perpendicular al eje"; en una diagonal contra una viga, no (duda D4).
3. Dos validaciones: "la plancha cabe en la cara del apoyo" (la cara inferior del HSS12X8 del Hangar mide unos 305 mm de ancho con el
   giro de 90° que leyó la 12c; la de un HSS3X3, 76,2: una plancha de 185 se sale, duda D3) y "la barra no atraviesa el apoyo".
4. El detector: el caso **"extremo contra apoyo"** (`placa-de-extremo.md`, 3.4): una barra termina a menos de medio canto de la cara
   de otra que sigue. Hoy el plan y Conectar lo tratan como nudo de una barra. Con el caso nuevo, el arranque por tipo decide entre
   soldadura directa y placa de extremo (duda D5).
5. La fabricación: la plancha sobre la cara de un elemento que no es de la conexión y su `WeldLine` a ese elemento (la 12c soldó a
   las planchas de la misma sesión; soldar a una barra del modelo es lo que ya hace la 12c con la plancha del empalme contra el tubo,
   pero conviene verlo).

**Cuánto cuesta.** 1 sesión + 2 rondas: la primera prueba un montante a mano y por el puente; la segunda, lo que salga (la cara, el
hueco medido, el plan). Es la fase más parecida a la 12 en mecanismos ya probados, así que las rondas deberían ser cortas.

**Sondeos.** El 31 (inventario de la cercha, sección 4) **antes de programar**: cuántos montantes hay, a qué llegan, cuánto hueco queda
entre su extremo y la cara, y qué barras terminan contra columnas o vigas en el eje B. No hace falta API nueva: planchas, pernos y
`WeldLine` están probados.

**Riesgos.** Que los montantes no estén modelados (entonces quedan solo los apoyos del eje B y la fase se queda corta); que el extremo
de la barra en el modelo no llegue a la cara (hueco grande) o la atraviese; que la soldadura de la plancha a una barra ajena se
comporte distinto de la del empalme.

### B. `base_plate` (el anclaje de una columna al concreto)

**Qué es.** La placa de extremo donde el "apoyo" no es una barra sino el **concreto**: una plancha en la base de la columna, pernos de
anclaje embebidos (tuerca de nivelación abajo y tuerca arriba), un espesor de grout y, si está modelado, la zapata o la losa como
referencia (`placa-de-extremo.md`, sección 4). La propuesta original lo ponía como "Fase 14" porque entonces `end_plate` iba a ser la
13; `end_plate` se adelantó a la 12 y `base_plate` quedó sin fecha.

**Qué aporta al Hangar.** Las bases de las columnas HSS12X6X1/2 del alzado (`modulo-tipico-03-tijeral.png`), si están en el modelo. No
toca ningún nudo de la cercha: sería la primera conexión fuera de ella, y la persona la pidió en el cierre de la Fase 8 ("más
adelante quiere lo mismo entre una columna y su base").

**Qué se reutiliza.** La plancha, el patrón de pernos, la pila de espesores y el croquis de `end_plate`; y el "apoyo" de `to_support`
(por eso conviene hacer A antes: la mitad de B es "el miembro termina contra algo").

**Qué es nuevo.** El apoyo `{ "kind": "concrete", "top_elevation_mm", "grout_mm" }`, los `anchors` en lugar de `bolts` (longitud
embebida, gancho o cabeza, tuerca de nivelación), las validaciones "borde al concreto" y "longitud embebida", la plancha en un plano
horizontal bajo una columna vertical, y **cómo se materializa un anclaje en Advance Steel**, que no se ha visto nunca: hoy los pernos son
patrones entre planchas; un anclaje embebido es otro objeto, o un perno largo sin segunda plancha con aviso.

**Cuánto cuesta.** 1 sesión + 2 rondas, **después** del sondeo 32. Si el sondeo dice que no hay anclajes en la API, la fase sale igual
con "perno estándar largo + aviso" (como se hizo con la tuerca doble, D9 de la 12).

**Sondeos.** El 32 (sección 4): las columnas del modelo y su cota de base, qué hay debajo (zapata, losa, nada) y la lista de tipos de los
ensamblados de Advance Steel con "Anchor" en el nombre y sus constructores, como hizo el sondeo 06 con las transacciones. No se da por
hecho ningún nombre de la API.

**Riesgos.** Que el concreto no esté modelado (se trabaja con una cota, P5 de `placa-de-extremo.md`); que la API de anclajes no exista o
no se abra desde IronPython (reserva: perno largo con aviso); está en la lista "fuera de v1" del encargo (sección 14), como lo estaban
los empalmes antes de la 12: es una ampliación consciente.

### C. Partir cordones o barras sin `splice_hint` desde el plan

**Qué es.** Hoy el plan solo propone cortes (`split`) en las diagonales de los nudos en K que llevan `splice_hint` (el Detalle B a
330). El botón **Partir barra** y `conn_split_member` ya cortan cualquier barra de armazón a la distancia que se pida, pero el plan no
lo planifica para cordones ni para barras sin marca (`docs/prompts/fase-13.md`, sección 8). Sería: una marca de empalme en cualquier
barra del plan (distancia desde un extremo o desde una cara) con su paso `split` y su nudo `N…B` previsto, como ya existe para las
diagonales.

**Qué aporta al Hangar.** **Hoy, nada**: el cordón superior y el central ya están modelados en ocho tramos cada uno y sus empalmes se
crean como `splice` donde el modelo tiene la junta. Serviría si el cordón inferior existiera como una sola barra (sus empalmes del
Detalle I habría que fabricarlos partiéndolo), para otra cercha con el cordón de una pieza, o para colocar un empalme donde el plano lo
dibuja y el modelo no tiene junta (los 610 mm del alzado frente al nudo, D5 de la 12).

**Qué es nuevo.** Poco en el corte (la operación existe) y bastante en el **registro**: al partir un cordón que ya tiene conexiones a
los dos lados del corte, las del lado del tramo nuevo quedan apuntando a un id que ya no las contiene. Hay que decidir si se re-apuntan
solas (cambiar el `element_id` guardado en cada conexión afectada) o si se prohíbe (`SPLIT_IN_USE` ampliado a "conexión en cualquier
punto del tramo que se separa"). D6 de la 13 esquivó esto porque el tramo corto conserva el id y el largo nunca tenía nada.

**Cuánto cuesta.** ½ a 1 sesión + 1 ronda, sin sondeo (el mecanismo del corte está probado en Revit). La ronda prueba un cordón con
conexiones a ambos lados.

**Riesgos.** Es la que más puede romper lo que ya funciona (el almacenamiento de las conexiones y el deshacer de los cortes), y sin un
cordón de una pieza en el modelo no hay con qué probarla en el Hangar.

### D. Contornos cóncavos con edición arrastrando (V6)

**Qué es.** En el croquis de la previsualización, arrastrar con el ratón las esquinas de la cartela (y, después, el grupo de pernos o el
largo de la ranura); las cotas se actualizan y se revalida (`flujo-intuitivo.md`, V6, esfuerzo medio-alto). Con eso una cartela
automática (envolvente convexa, D1 de la 13) se podría recortar a una forma en L o con entrantes sin escribir vértices a mano (hoy:
Convertir en polígono, doble clic en las cotas y la pestaña Avanzado).

**Qué aporta al Hangar.** Comodidad al editar, no acero nuevo: las 16 cartelas del Hangar son del Detalle D (plantilla) y ninguna
necesita un entrante. Tiene más sentido cuando haya una cercha con cartelas automáticas que salgan grandes (`OUTLINE_AUTO_LARGE`).

**Qué es nuevo.** Todo en la ventana WPF (el lienzo del croquis de `PreviewWindow`): detectar el vértice bajo el ratón, arrastrarlo con
imanes a 5 mm, escribir el polígono por `SpecEditor` (el JSON sigue siendo la única fuente de verdad), revalidar al soltar; y qué pasa
con una cartela `auto` al tocarla (se convierte a `polygon`, como hoy).

**Cuánto cuesta.** 1 sesión + 1 ronda. **No se puede probar en la nube**: la ventana solo se ve en el PC y quien arrastra es la persona
(el instalador no puede). Por eso la ronda es suya, con capturas.

**Sondeos.** Ninguno de Revit.

**Riesgos.** La estabilidad de la ventana (un error en el lienzo con la ventana no modal, lección de la 8d); el esfuerzo es alto para lo
que se usa hoy.

### E. El cordón inferior, si está modelado

**Qué es.** Desde la ronda 8d está pendiente saber si el cordón inferior HSS3X3X1/4 (Z ≈ 14 914) existe como barra de armazón
estructural o si abajo solo hay parejas diagonal + montante (`fase-8.md` 10.2; D1b de la 12; D10 de la 13: "sigue fuera"). El Detalle I
(el empalme de ese cordón, plancha de 200 con 4 pernos a 150) ya es fixture y arranque por perfil desde la Fase 12.

**Qué aporta al Hangar.** Si existe: los 7 u 8 nudos de abajo entran en el plan (con la cartela automática o una plantilla nueva, y el
montante como tercera barra) y sus empalmes del Detalle I se crean solos por perfil, **sin código nuevo**. Si no existe: nada que hacer, y
los montantes de abajo pasan a ser el caso A (contra qué llegan).

**Cuánto cuesta.** 0 sesiones; una pregunta del sondeo 31 (o la persona lo selecciona y pide `node_info`). Solo habría código si al
planificarlo falla algo, y entonces sería una ronda pequeña, no una fase.

**Riesgos.** Ninguno; es la comprobación más barata de la lista y la que lleva más tiempo sin respuesta.

### F. El traspaso de `mcp/` a `revit-mcp`

**Qué es.** Hoy los archivos nuevos del MCP (`mcp/revit_mcp/conexiones.py`, `mcp/tools/conn_tools.py`, `mcp/CONTRATO-conn.md`, las
pruebas y el simulador) viven en este repositorio y `mcp/instalar-conn.ps1` los copia a la extensión de pyRevit en cada instalación. El
traspaso es llevarlos al repositorio `revit-mcp` (de la persona, de solo lectura para las sesiones) y que la instalación sea solo un
`git pull` allí (`fase-5.md` 7.3: "lo hace la persona según el encargo").

**Qué aporta al Hangar.** Nada en la cercha. Orden en los repositorios y una instalación con un paso menos.

**Cuánto cuesta.** ½ sesión (preparar los archivos tal como van en `revit-mcp`, el texto del commit y la sección de su `CONTRATO.md`;
decidir si `instalar-conn.ps1` se retira o queda como atajo) + el commit de la persona en `revit-mcp` + 1 ronda corta (recargar pyRevit
y `probar_conexiones.py --puente`, que hoy da 49/49). Hay que cambiar también `CLAUDE.md` (la regla "los archivos nuevos del MCP van en
`mcp/`").

**Sondeos.** Ninguno.

**Riesgos.** El de verdad es el **ritmo**: `conexiones.py` y `conn_tools.py` cambian de versión en casi todas las fases (rutas y
herramientas nuevas: 25 rutas en la 10, dos herramientas más en la 13). Con los archivos en `revit-mcp`, cada fase necesitaría un commit
de la persona en el otro repositorio antes de instalar, y dos repositorios que ir al compás. Mientras las `conn_*` sigan creciendo, la
copia por `instalar-conn.ps1` es más cómoda.

### G. Retoques de robustez

Tres cosas pequeñas apuntadas en `fase-13.md` 9.4 y en rondas anteriores; ninguna justifica una fase sola y todas caben de paso en la
siguiente que toque el add-in o los scripts:

1. **El reintento de la `FabricationTransaction` al borrar conexiones seguidas.** Desde la Fase 9 (`fase-9.md` 7.2, punto 3), al borrar
   dos o más conexiones seguidas, desde la segunda Advance Steel responde "cannot start a fabrication transaction while asynchronous
   fabrication tasks are queued for execution"; el add-in avisa, cae a `Document.Delete` y los elementos se borran igual con las barras
   restauradas (confirmado en la 10, la 12 y la 13c). El cambio: antes de rendirse, **reintentar unas pocas veces con una espera corta**
   y anotar en el log cuántos intentos hicieron falta. **Lo que no se sabe**: si las tareas en cola se ejecutan mientras la petición
   sigue dentro del contexto de la API de Revit; puede que el reintento nunca las vea terminar. Por eso va con contador: si en la ronda
   el contador dice que nunca sirvió, la alternativa es bajar el aviso al log (`filtered: true`, como las dos advertencias de Revit) y
   dejar de molestar, porque el borrado ya está bien.
2. **Los ids a mano en las instalaciones.** `conn-call.ps1` imprime una cabecera antes del JSON y `ConvertFrom-Json` sobre su salida deja
   la variable vacía (`fase-13.md` 9.4). Un interruptor `-SoloJson` (o la cabecera por el flujo de error) permite encadenar comandos en
   `docs/instalacion/` sin copiar ids a mano. Solo toca `scripts/`.
3. **El sondeo 30 v3 como diagnóstico de una barra** (desvío de eje, uniones, parámetros, el corte y el deshacer): ya está escrito; solo
   hay que nombrarlo así en `docs/instalacion/` cuando haga falta diagnosticar otra barra.

**Qué aporta al Hangar.** Borrados y lotes sin avisos que asustan; instalaciones más cortas. **Cuánto cuesta.** Un cuarto de sesión
dentro de la Fase 14 + la comprobación en su primera ronda (tres `delete` seguidos mirando el log). **Sondeos.** Ninguno.

### H. (Añadido) El lote entero del Hangar con sus cortes, sobre la copia

No está en la lista de candidatos pero sí en los pendientes: **nunca se ha creado la cercha entera de una vez con los cortes**. La
instalación de la 13 no creó el lote hasta que la cadena de un nudo funcionara (paso 13-9), y la 13b y la 13c se centraron en el corte.
Hoy la cadena funciona con las tres barras (13c-5), y el plan de la 8c tendría unos 30 cortes, 30 empalmes B, 15 nudos en K, 7 empalmes
C, 2 soldaduras de una barra y 16 cartelas: unas 70 conexiones y 100 pasos en un solo grupo. Es la prueba que dice si el add-in está
listo para una cercha de verdad: el tiempo total, el aviso de la `FabricationTransaction` en cadena, y Borrar el lote deshaciendo 30
cortes.

**Cuánto cuesta.** 0 sesiones; media hora del instalador en la ronda de la Fase 14, sobre la copia, con los sondeos 12 y 13 a cero al
final. **Riesgo.** Que algo falle a escala (tiempo, un corte que no se deshace): mejor saberlo ahora que con un modelo nuevo.

## 3. Tabla comparativa

| | Candidato | Aporte al Hangar | Coste | Sondeos | Riesgo | Cuándo |
|---|---|---|---|---|---|---|
| A | `to_support` | **Alto**: montantes y apoyos del eje B, las primeras conexiones nuevas | 1 sesión + 2 rondas | 31 | Medio | **Fase 14** |
| B | `base_plate` | Medio: las bases de las columnas, fuera de la cercha | 1 sesión + 2 rondas | 32 | Medio-alto (API de anclajes) | Fase 15 |
| C | Partir cordones desde el plan | Hoy ninguno | ½–1 sesión + 1 ronda | Ninguno | Alto (almacenamiento) | Cuando haya un cordón de una pieza |
| D | V6 edición arrastrando | Bajo (comodidad) | 1 sesión + 1 ronda de la persona | Ninguno | Medio (ventana) | Después de los tipos |
| E | Cordón inferior | Alto si existe, sin código | 0 + una pregunta del sondeo 31 | 31 | Ninguno | **En la ronda de la 14** |
| F | Traspaso de `mcp/` | Ninguno en la cercha | ½ sesión + commit de la persona + 1 ronda | Ninguno | Bajo, pero dos repositorios al compás | Cuando las `conn_*` se estabilicen |
| G | Robustez | Bajo-medio (avisos, instalaciones) | ¼ sesión de paso | Ninguno | Bajo | **De paso en la 14** |
| H | Lote entero del Hangar | **Alto**: la prueba real | 0 + ½ hora del instalador | Ninguno | Se descubre lo que haya | **En la ronda de la 14** |

## 4. Sondeos que harían falta

Los números siguen al 30. Los dos son de **solo lectura** (van con `-SinTransaccion` y no tocan el modelo) y devuelven una tabla en
texto que el instalador pega en `docs/fases/resultados-fase-14.md`.

- **Sondeo 31, `31-inventario-cercha.py`** (para A, E y H). Con la cercha seleccionada (o toda la categoría de armazón estructural si
  no hay selección): por barra, id, perfil, cota de los dos extremos, si es horizontal, vertical o inclinada, y para cada extremo la
  barra más cercana cuya cara está a menos de medio canto (id, cara, hueco en mm). Responde de una vez: ¿hay una barra horizontal
  HSS3X3 en Z ≈ 14 914 (cordón inferior)?; ¿cuántos montantes hay, a qué cordón llegan y con qué hueco?; ¿qué barras terminan contra
  columnas o vigas en el eje B?; ¿alguna barra tiene uniones automáticas activas? Es el que decide el alcance real de la Fase 14, así
  que conviene correrlo **antes** de escribir `docs/prompts/fase-14.md` (duda D14).
- **Sondeo 32, `32-columnas-y-anclajes.py`** (para B). Las columnas estructurales del modelo (perfil, cota de base, longitud), qué
  elemento hay bajo cada base (cimentación, losa, suelo, nada) y a qué cota; y, dentro de una `FabricationTransaction` de solo lectura,
  la lista de tipos de los ensamblados de Advance Steel cuyo nombre contenga "Anchor" (y "Grout", por si acaso) con sus constructores,
  como hizo el sondeo 06 con las transacciones. No da por hecho ningún nombre: si no sale nada, `base_plate` se programa con pernos
  largos y aviso.

Para C, D, F y G no hace falta sondeo: el corte, la ventana, el MCP y el borrado ya se conocen; lo que haya que ver se ve en la ronda.

## 5. Recomendación de orden

1. **Fase 14: `to_support` (A) + robustez (G); en su instalación, los sondeos 31 y 32, el cordón inferior (E) y el lote entero (H).**
   Por qué primero: es lo que añade conexiones de verdad al Hangar con el menor riesgo (reutiliza todo lo de la 12 y la 13), cierra la
   familia de la placa de extremo tal como la planeó `placa-de-extremo.md`, y deja preparado el "apoyo" que `base_plate` necesita. La
   instalación aprovecha el viaje para responder las tres preguntas que llevan meses abiertas (cordón inferior, columnas, lote entero)
   sin escribir código para ellas.
2. **Fase 15: `base_plate` (B)**, con lo que diga el sondeo 32. Si no hay columnas con base en el modelo o la persona no lo necesita
   aún, se salta y la 15 es la siguiente de esta lista.
3. **Después, según aparezca la necesidad**: C (cuando haya un cordón de una pieza), D (cuando haya cartelas automáticas que recortar),
   F (cuando una fase no toque las `conn_*`, o al terminar los tipos).

Alternativa, si la persona prefiere las columnas antes que los montantes: Fase 14 = `base_plate` con el sondeo 32 **primero** (una
ronda de solo sondeos antes de programar: 1 sesión + 3 rondas en total) y `to_support` en la 15. Lo desaconsejo porque la 14 se
quedaría esperando al sondeo y porque `to_support` resuelve la mitad de `base_plate` (el miembro que termina contra un apoyo).

Orden de recorte dentro de la 14, si no cabe: (1) el plan y el lote sobre los nudos `to_support` (quedan Conectar y el puente, de uno en
uno); (2) el arranque automático por tipo (la persona elige `end_plate` en el desplegable Tipo); (3) el reintento de la
`FabricationTransaction` (pasa a la 15). **Nunca se recorta**: el tipo de punta a punta en Conectar y por el puente, la validación de la
cara del apoyo, los sondeos 31 y 32 ni el lote entero del Hangar.

## 6. Dudas para la persona (con la recomendación en cada una)

- **D1. ¿Qué entra en la Fase 14?** (a) `to_support` + robustez, con los sondeos 31 y 32, el cordón inferior y el lote entero en su
  instalación; (b) `base_plate` primero; (c) otra cosa de la lista. **Recomiendo (a).**
- **D2. Los montantes.** En el alzado del nudo en K hay un montante vertical con plancha y pernos contra el cordón. ¿Están los montantes
  modelados en el Hangar y a qué llegan: al cordón superior, al central, al inferior? Si no lo sabes, **recomiendo** que lo responda el
  sondeo 31 y que la fase se programe con lo que diga (y, si no hay ninguno, con los apoyos del eje B como caso de prueba).
- **D3. La plancha del apoyo más ancha que la cara.** Una plancha del Detalle B (185) cabe en la cara inferior del HSS12X8 (unos 305 mm)
  pero no en la de un HSS3X3 (76,2). Si pasa, ¿error, o aviso (la plancha sobresale y se suelda en los dos bordes del tubo)?
  **Recomiendo aviso** con los mm y que la guía lo explique; error solo si la plancha no toca la cara.
- **D4. La orientación de las planchas cuando la barra llega inclinada** (Detalle H1, diagonal contra una viga). ¿Las dos planchas
  paralelas a la **cara del apoyo** (la barra se corta en bisel) o perpendiculares al **eje de la barra** (la plancha del apoyo quedaría
  inclinada sobre la cara, como una cuña)? **Recomiendo paralelas a la cara del apoyo**: es como se suelda una plancha a una viga; en un
  montante a 90° las dos opciones coinciden. P4 de `placa-de-extremo.md` sigue abierta: si en H1 ves otra cosa, dilo.
- **D5. El arranque de una barra sola contra una cara.** Hoy sale soldadura directa (D4 de la 12, por N49 y N58). Con `to_support`,
  ¿cuál es el arranque por defecto? **Recomiendo**: montante a 90° ± 5° → `end_plate` `to_support` con el Detalle B de 185 (decisión
  P7); diagonal inclinada → `direct_weld` como hoy; y el desplegable Tipo de Cambiar… para elegir lo otro.
- **D6. El hueco entre las planchas y el retiro.** Si en el modelo el montante llega hasta la cara, la pila de las dos planchas (2 × 9,5
  + `gap_mm`) obliga a retirar la barra. **Recomiendo** lo mismo que D8 de la 12: medir el hueco real y calcular el retiro que falta,
  nunca negativo; `gap_mm` 0 (planchas en contacto, D2 de la 12).
- **D7. La robustez (G).** ¿Entra el reintento de la `FabricationTransaction` en la 14 con su contador en el log, y si no sirve se baja
  el aviso al log? ¿Y `conn-call.ps1` con `-SoloJson`? **Recomiendo sí a las dos**: son pequeñas y se comprueban en la misma ronda.
- **D8. El cordón inferior (E).** ¿Lo busca el sondeo 31 o prefieres seleccionarlo tú y pedir `node_info`? **Recomiendo el sondeo**: deja
  la respuesta escrita en `resultados-fase-14.md`. Si existe, ¿sus nudos entran en el lote de la 14 (sin código, con la cartela
  automática) o se miran después? **Recomiendo que entren**: sería la primera prueba real de la cartela automática, que en el Hangar no
  tiene hoy ningún nudo que la necesite.
- **D9. `base_plate` (B).** ¿Hay columnas en el modelo del Hangar y está modelado lo que hay debajo (zapata, losa)? ¿Pernos de anclaje
  con gancho o con cabeza, y con tuerca de nivelación? (P5 de `placa-de-extremo.md`.) **Recomiendo** no decidirlo aún: el sondeo 32 lo
  mide en la ronda de la 14 y la Fase 15 se escribe con eso. Si el concreto no existe, una cota en el JSON.
- **D10. Partir cordones desde el plan (C).** ¿Hay alguna cercha con el cordón modelado de una pieza, o algún empalme que quieras donde
  el modelo no tiene junta? Si no, **recomiendo aplazarlo**. Y cuando se haga: al partir un cordón con conexiones a los dos lados,
  ¿re-apuntar solas las conexiones del tramo nuevo (**recomendado**) o prohibir el corte con `SPLIT_IN_USE`?
- **D11. V6 (D).** ¿La quieres antes de los tipos nuevos o después? **Recomiendo después**, y empezando solo por las esquinas de la
  cartela (pernos y ranuras más adelante). La ronda la haces tú: arrastrar no lo puede hacer el instalador.
- **D12. El traspaso de `mcp/` (F).** ¿Ahora o cuando las `conn_*` dejen de cambiar en cada fase? **Recomiendo esperar**: cada fase
  obligaría a un commit tuyo en `revit-mcp` antes de instalar. Cuando lo quieras, la sesión prepara los archivos y el texto del commit
  (`fase-5.md` 7.4).
- **D13. El lote entero del Hangar (H).** ¿Entra en la instalación de la 14, sobre la copia, al final (unas 70 conexiones y 30 cortes)?
  **Recomiendo sí**: es la prueba que falta y no cuesta código.
- **D14. Cuándo se corre el sondeo 31.** ¿**Antes** de escribir `docs/prompts/fase-14.md` (una ronda corta de solo sondeos, sin código,
  y el prompt se escribe sabiendo cuántos montantes y apoyos hay) o dentro de la instalación de la 14 (el prompt se escribe con
  hipótesis y una ronda 14b ajusta)? **Recomiendo antes**: son diez minutos del instalador y evitan programar a ciegas.
- **D15. Alcance y rondas.** ¿Aceptas 1 sesión + 2 rondas para la 14 y el orden de recorte de la sección 5? **Recomiendo sí.**

## 7. Qué pasa después

1. La persona responde las dudas en el chat; basta con los números ("D1 a, D2 no lo sé, D3 aviso, …").
2. Si D14 es "antes": una sesión corta escribe los sondeos 31 y 32 y `docs/instalacion/fase-14-sondeos.md`; el instalador los corre y
   devuelve la tabla en `docs/fases/resultados-fase-14-sondeos.md`.
3. La sesión siguiente escribe `docs/prompts/fase-14.md` con la estructura de `docs/prompts/fase-13.md` (decisiones ya tomadas,
   decisiones de la persona, alcance, reglas técnicas, pruebas en la nube, instrucciones para el instalador, definición de hecho, fuera
   de alcance, dudas y orden de recorte).
4. Después, la sesión que programa la Fase 14 (add-in **0.14.0**, las 314 pruebas del Core y el simulador en verde), su ronda y su
   cierre, como en las fases 12 y 13.

## 8. Decisiones de la persona (2026-10-31): la propuesta queda cerrada

La persona aceptó las quince recomendaciones de la sección 6 tal cual:

- **D1 → (a)**: la Fase 14 es `end_plate` contra un apoyo (`kind: "to_support"`) más los retoques de robustez (G); en su instalación
  van los sondeos 31 y 32, el cordón inferior (E) y el lote entero del Hangar (H).
- **D2 → el sondeo 31** dice cuántos montantes hay y a qué cordón llegan; si no hay ninguno, los apoyos del eje B son el caso de prueba.
- **D3 → aviso** con los mm cuando la plancha del apoyo sobresale de la cara; error solo si no la toca.
- **D4 → planchas paralelas a la cara del apoyo**; la barra inclinada se corta en bisel.
- **D5 → arranque por tipo**: montante a 90° ± 5° → `to_support` con el Detalle B de 185; diagonal inclinada → `direct_weld` como hoy;
  el desplegable Tipo para cambiarlo.
- **D6 → medir el hueco real** y calcular el retiro que falta, nunca negativo; `gap_mm` 0.
- **D7 → sí a las dos**: el reintento de la `FabricationTransaction` con contador en el log (si no sirve, el aviso baja al log) y
  `conn-call.ps1` con `-SoloJson`.
- **D8 → el sondeo 31** busca el cordón inferior; si existe, sus nudos entran en el lote de la 14 sin código.
- **D9 → no se decide aún**: el sondeo 32 mide las columnas y qué hay debajo; la Fase 15 (`base_plate`) se escribe con eso.
- **D10 → aplazado**; cuando se haga, las conexiones del tramo nuevo se re-apuntan solas.
- **D11 → después** de los tipos nuevos, empezando por las esquinas de la cartela.
- **D12 → esperar** a que las `conn_*` dejen de cambiar en cada fase.
- **D13 → sí**: el lote entero del Hangar al final de la instalación de la 14, sobre la copia.
- **D14 → antes**: una ronda corta de solo sondeos (31 y 32) antes de escribir `docs/prompts/fase-14.md`.
- **D15 → sí**: 1 sesión + 2 rondas y el orden de recorte de la sección 5.

**Qué sigue**: la sesión corta que escribe los sondeos 31 y 32 y `docs/instalacion/fase-14-sondeos.md` (prompt en
`docs/fases/resumen-fase-14-propuesta.md`, sección 3); con la tabla del instalador, la sesión que escribe `docs/prompts/fase-14.md`.
