# Propuesta: nudos en K con excentricidad (los del cordón superior) — cómo describirlos y conectarlos

Fecha: 2026-10-12. Estado: **hecha en la Fase 12 y probada en Revit el 2026-10-21** (add-in 0.12.0, `docs/fases/fase-12.md`,
sección 7): la opción A, el aviso `MEMBER_ECCENTRIC` hasta medio canto, el tipo `direct_weld` con la marca del empalme a 330 y
los fixtures de los Detalles B, C e I están en el repositorio y funcionan en el modelo (la excentricidad real del nudo en K es
**54,9 mm**, no los 34 estimados aquí; `node_info` la mide); el lote de la 8c creó 17 soldaduras directas. Lo que sigue
abierto es el **corte inclinado** contra la cara del cordón: el sondeo 22 descartó `SetEndReference`; el 25 (ronda 12b) y el 28
(ronda 12c) no encontraron la viga de Advance Steel para `BeamShortening` (el 28 ni siquiera recorrió los objetos del modelo: su
recorrido saltaba `DatabaseManager.GetModelObjectIds` por el parámetro de salida), y el **29 (ronda 12d, en Revit el 2026-10-24) tampoco la encontró**:
`GetModelObjectIds` devolvió 5 objetos, ninguno se abrió (el sondeo los abrió con `eOpenMode.kNotOpened`, el primer modo de la
lista, y la reserva `GetFilerObject` tampoco dio objeto; si alguien reabre el tema algún día, lo primero es `kNormal` en
`abrir_objeto`) y no vio vigas. **Decisión del cierre de la 12d (`fase-12.md` 10.5): el retiro por el eje (`END_CUT_APPROXIMATED`)
es definitivo y no se abren más sondeos sobre la viga de Advance Steel.** Cada barra se acorta por su
eje y la soldadura va sobre la huella en la cara como `WeldLine` de Advance Steel (materializada en Revit el 2026-10-23, 2
elementos por nudo, `fase-12.md` 9.1).
La cartela automática queda para la Fase 13. Sale del cierre de la Fase 11 (`docs/fases/fase-11.md`, 7.2): el
arranque sobre un nudo del cordón superior dio `MEMBER_NOT_AT_NODE`. Hay que decidirla **antes de la Fase 12** (cartela
automática), porque la cartela se dibuja desde el punto de trabajo y en estos nudos no está claro cuál es.

## 1. Qué son estos nudos

En el cordón superior de la cercha del Hangar (HSS12X8X1/2, 305 mm de canto) llegan en cada nudo **dos diagonales desde
abajo** (a −135,6° y −45,0°), y sus ejes **no cortan el eje del cordón en el mismo punto**: lo cortan en dos puntos a unos
**34 mm** uno del otro a lo largo del cordón (lo midió la ronda 8b, `fase-8.md` 7.5 y 7.7). Es un **nudo en K con
excentricidad** (o con "gap"), normal en cerchas de tubo: las diagonales se separan para que quepan las soldaduras, y el
proyectista acepta que los ejes no concurran. Son 8 nudos (N5, N12, N19, N26, N33, N40, N47 y N54 en el plan de la 8c) y
hoy ni el lote ni Conectar los pueden conectar.

## 2. Por qué el add-in los rechaza

El contrato (`node` + `chord` + `members`) describe un nudo con **un solo punto de trabajo** al que llegan todas las barras.
Lo comprueba la regla `NodeReach` (validación 8.10): el eje de cada barra tiene que pasar a **5 mm** o menos del punto de
trabajo (`node_axis_max_distance_mm` en `config/limits.json`). Conectar fija ese punto en el corte del cordón con la primera
diagonal; el eje de la otra pasa a unos 24 mm (34 mm × sen 45°) y sale `MEMBER_NOT_AT_NODE`. El plan de lote hace lo mismo
con la media de los dos cortes: una de las dos barras queda `reaches_node: false` y el nudo no se crea.

Subir la tolerancia a 30 mm haría validar, pero **mentiría**: la cartela, los retiros y las ranuras se calcularían desde un
punto por el que una diagonal no pasa, y el acero saldría torcido unos milímetros. No se hace sin decidirlo.

## 3. Opciones (elige una; recomiendo la A)

| | Qué se hace | Qué cambia | A favor | En contra |
|---|---|---|---|---|
| **A. Punto de trabajo en el cordón, a medio camino, y excentricidad declarada** (recomendada) | El punto de trabajo es el punto del eje del cordón a mitad entre los dos cortes. El contrato suma `node.eccentricity_mm` (la distancia entre cortes, informativa) y por barra `members[i].axis_offset_mm` (cuánto se desvía su eje del punto de trabajo, con signo a lo largo del cordón). El validador admite el desvío hasta **medio canto del cordón** con un aviso nuevo `MEMBER_ECCENTRIC` (en vez del error), y **cada barra se recorta y se ranura sobre su propio eje**, no sobre el punto de trabajo. El croquis dibuja cada diagonal en su sitio real. | Contrato (dos claves), validador (una regla), `NodeFrame`/croquis (las barras ya se dibujan desde su eje real: solo cambia dónde se mide el retiro), el arranque y el plan (punto medio). | Describe la geometría de verdad; el lote y Conectar funcionan sin tocar las plantillas; la cartela automática de la Fase 12 tiene un punto claro desde el que dibujar. | Es la opción con más código (una fase corta o la primera mitad de la 12). |
| **B. Un nudo por diagonal** | Cada diagonal es un nudo de una barra con su propio punto de trabajo y su propia cartela pequeña; las dos cartelas se solapan o se juntan en una placa a mano. | Casi nada en el contrato (un nudo de una barra ya cabe); el detector tendría que dejar de juntarlas (`node_face_reach_mm`). | Rápido. | No es cómo se fabrica (una sola cartela por nudo); dos cartelas de 34 mm de separación chocan; el plan contaría el doble de nudos. |
| **C. Tolerancia alta y punto en la primera diagonal** | Subir `node_axis_max_distance_mm` a 30 mm y dejar el punto de trabajo donde lo pone Conectar hoy. | Un número en `limits.json`. | Cero código. | Mentira geométrica (sección 2): la segunda diagonal se recorta desde un punto por el que no pasa; retiros y ranuras salen desviados. **No la recomiendo** salvo para una prueba puntual. |

## 4. Qué pediría la Fase 12 con la opción A

1. Contrato: `node.eccentricity_mm` y `members[i].axis_offset_mm` (opcionales; 0 cuando concurren). `conn_get_node_info` los
   devuelve ya medidos.
2. Validación: `MEMBER_NOT_AT_NODE` solo si el desvío supera medio canto del cordón (o un `node_axis_max_offset_mm` nuevo en
   `limits.json`); por debajo, aviso `MEMBER_ECCENTRIC` con los mm.
3. Geometría: retiro, ranura y soldadura de cada barra medidos sobre su eje real (hoy ya se dibujan desde su eje; hay que
   comprobar que el corte del extremo también). El contorno de la cartela (`auto`) envuelve los dos extremos reales.
4. Plan de lote y Conectar: punto de trabajo a mitad entre los cortes; las dos barras `reaches_node: true`; las plantillas
   casan igual (los ángulos no cambian).
5. Pruebas con un nudo sintético en K de 34 mm y con los IDs reales del Hangar (1245533, 1249631, 1249632).

## 5. Preguntas para ti

- **P1.** ¿Opción A, B o C? (Recomiendo A.)
- **P2.** ¿Hasta cuánta excentricidad se acepta sin error: medio canto del cordón (152 mm en el HSS12X8) o un número fijo?
  Recomiendo medio canto, que es lo que las normas de tubo (AISC 360 K, CIDECT) toman como límite habitual para no comprobar
  el momento por excentricidad; por encima, error.
- **P3.** En estos nudos, ¿la cartela va **por fuera** del cordón (soldada a la cara inferior del HSS12X8) o **atraviesa** el
  cordón ranurado como en el Detalle D? En el plano de la cercha parece por fuera (el cordón es mucho mayor que las
  diagonales). Si es por fuera, la Fase 12 necesita `chord_interface = "face_welded"` que hoy existe en el esquema pero no
  está probado en Revit.
- **P4.** ¿Tienes un detalle del plano para estos nudos (como el Detalle D para el cordón central)? Con él, la Fase 12 lo usa
  de fixture y de plantilla.

## 6. Lo que enseña el plano (2026-10-12, corregido con los detalles B y C) y la recomendación corregida

Imágenes en esta carpeta: `cordon-superior-nudo-k.png` (el nudo del cordón superior en alzado), `detalle-C-empalme-cordon-superior.png`
(Detalle C y sección C1), `detalle-B-empalme-diagonal.png` (Detalle B y sección B1) y `detalle-I-placa-extremo.png` /
`seccion-I1-placa-extremo.png` (Detalle I y sección I1). La persona corrigió la primera lectura: **los detalles del cordón
superior son el C y el B; el I es de otro sitio** (por confirmar: P7).

Qué hay en el cordón superior:

- **Detalle C / sección C1: empalme del cordón superior** (HSS12X8X1/2). Dos planchas `PL1/2" × 330 × 430` cara a cara,
  perpendiculares al eje del cordón, con **12 pernos Ø3/4"** en agujeros Ø13/16" repartidos **alrededor del tubo**: 4 arriba y
  4 abajo (a 90 mm, con 20 mm hasta las esquinas recortadas) y 2 por lado (a 80 mm), bordes de 60 mm; esquinas recortadas. Son
  los nudos que el plan de lote llama "empalme del cordón" (7 en la cercha de la 8c).
- **Detalle B / sección B1: empalme de la diagonal** (HSS2-1/2X2-1/2X3/16, la diagonal del Detalle D). Dos planchas
  `PL3/8" × 185 × 185` octogonales (esquinas recortadas: 60 + 65 + 60), **4 pernos Ø5/8"** en agujeros Ø18 a 135 mm, uno centrado
  en cada cara del tubo. En el alzado del nudo se ve uno en cada diagonal, un poco por debajo del cordón: la diagonal llega al
  cordón **soldada directamente a su cara inferior** (sin cartela) y el empalme atornillado va en la propia diagonal, a una
  distancia del cordón que estas imágenes no acotan.
- **Detalle I / sección I1** (HSS3X3X1/4, `PL3/8"` con octógono de 200 según las cotas y 185 según el rótulo, 4 pernos Ø5/8" a
  150): el mismo tipo de empalme, en una barra HSS3X3. Falta saber cuál (el montante vertical del alzado, o el cordón central).

Es decir, **todo lo del cordón superior son placas de extremo** (`docs/propuestas/placa-de-extremo.md`): el cordón se empalma
con el Detalle C y cada diagonal con el Detalle B; **el nudo en K en sí es una soldadura directa de las diagonales a la cara
del cordón**, sin cartela ni placas. La excentricidad de 34 mm entre las dos diagonales es la separación ("gap") de ese nudo
soldado y no afecta a ninguna pieza: solo se anota.

Con eso, las respuestas que recomiendo:

- **P1 → A, solo para describir el nudo.** El punto de trabajo a mitad y `axis_offset_mm` por barra hacen falta para que el
  plan y Conectar reconozcan estos nudos y para `conn_get_node_info`. Lo que se construye en ellos es otra cosa: la soldadura
  directa de cada diagonal a la cara del cordón (corte del extremo de la diagonal contra la cara, soldadura en todo el
  contorno) y, si se quiere modelar, el empalme Detalle B en la diagonal.
- **P2 → medio canto del cordón, con aviso** (`MEMBER_ECCENTRIC`). Por encima, error.
- **P3 → ninguna de las dos: soldadura directa a la cara del cordón.** Es un tipo de conexión nuevo y pequeño (`direct_weld`:
  recorte del extremo de la diagonal al perfil de la cara, soldadura todo el contorno, sin piezas), más sencillo que la cartela.
- **P4 → sí.** Detalle C (empalme del cordón, 12 pernos) y Detalle B (empalme de la diagonal, 4 pernos) son los fixtures del tipo
  `end_plate`; el alzado del nudo, el del nudo soldado.

**Consecuencia para el calendario**: la Fase 12 pasa a ser **"Placa de extremo y nudo soldado"**: (1) la excentricidad en el
contrato y en la validación (opción A); (2) el tipo `end_plate` con el patrón de pernos **alrededor del tubo** (n por cara con
sus pasos y bordes, no una cuadrícula filas × columnas) y la plancha con esquinas recortadas, con los Detalles B y C como
fixtures; (3) el tipo `direct_weld` para el nudo en K del cordón superior; (4) Conectar y el lote sobre esos nudos y sobre los
7 empalmes del cordón, que hoy no tienen plantilla. La **cartela automática** (`outline.mode = "auto"`) se aplaza a la
Fase 13: en esta cercha ningún nudo la necesita hoy.

Preguntas que quedan para la persona:

- **P5.** En el Detalle I, ¿manda 185 × 185 (rótulo) o 200 (cotas de la sección I1)? En el B las cotas y el rótulo coinciden (185).
- **P6.** ¿A qué distancia del cordón (de su cara a las planchas, a lo largo de la diagonal) va el empalme del Detalle B? Si el
  plano no lo acota, se propone un valor editable en `choices.json`.
- **P7.** ¿De qué barra es el Detalle I: del montante vertical que se ve en el alzado del nudo (con plancha y pernos contra el
  cordón) o del cordón central HSS3X3? Y ese montante, ¿cómo se une al cordón superior?
- **P8.** ¿Las diagonales llegan al cordón superior soldadas directamente a su cara (como se ve en el alzado) o hay alguna
  pieza entre medias que no se ve a esa escala?

## 7. Decisiones de la persona (2026-10-12) — la propuesta queda cerrada

Con el alzado completo de la cercha (`modulo-tipico-03-tijeral.png`: módulo típico 03, sección de tijeral longitudinal, ejes 2
y 3, cordón superior HSS12X8X1/2 a +274,636, cordón HSS3X3X1/4 a 2 510 por debajo, diagonales HSS2-1/2X2-1/2X3/16, columnas
HSS12X6X1/2; detalles A a I señalados):

- **P1 → A**: punto de trabajo a mitad entre los cortes, `node.eccentricity_mm` y `members[i].axis_offset_mm` en el contrato,
  cada barra medida sobre su eje real.
- **P2 → medio canto del cordón**: hasta ahí, aviso `MEMBER_ECCENTRIC`; por encima, `MEMBER_NOT_AT_NODE`.
- **P3 → soldadura directa a la cara del cordón** (tipo nuevo `direct_weld`), sin cartela ni placas en el nudo.
- **P4 → sí**: Detalles B y C como fixtures de `end_plate`; el alzado del nudo (`cordon-superior-nudo-k.png`) como fixture del
  nudo soldado.
- **P5 → 200**: en el Detalle I mandan las cotas de la sección I1 (octógono de 200, esquinas de 60, pernos a 150); el rótulo
  "185 × 185" se trata como error del plano.
- **P6 → no hay más detalle**: la distancia del cordón al empalme de la diagonal (Detalle B) no está acotada. Va como valor
  editable en `config/choices.json` (`diagonal_splice_offset_mm`, propuesta: 300) y en el panel.
- **P7 → el Detalle I es del cordón inferior** (HSS3X3X1/4): es el empalme de ese cordón, el que el plan llama "empalme del
  cordón" (7 en la cercha de la 8c). El montante vertical del alzado no tiene detalle propio: se trata como el Detalle B
  (placa de extremo de 185, 4 pernos) hasta que se diga otra cosa.
- **P8 → el alzado** (`modulo-tipico-03-tijeral.png`): el nudo del cordón superior es el **Detalle G** del plano, que no se ha
  visto a escala 1/10; en el alzado del nudo las diagonales llegan a la cara inferior del cordón sin pieza intermedia. Se
  toma **soldadura directa** como hipótesis de trabajo; si el Detalle G enseña otra cosa, se corrige antes de programar.

Alcance de la **Fase 12, "Placa de extremo y nudo soldado"**, con estas decisiones: (1) excentricidad en el contrato y en la
validación; (2) tipo `end_plate` con el patrón de pernos alrededor del tubo y la plancha con esquinas recortadas, fixtures
Detalle B (185, 4 pernos Ø5/8" a 135), Detalle C (330 × 430, 12 pernos Ø3/4") y Detalle I (200, 4 pernos Ø5/8" a 150);
(3) tipo `direct_weld` para el nudo en K; (4) Conectar y el lote sobre los 8 nudos del cordón superior y los 7 empalmes;
(5) la cartela automática se aplaza a la Fase 13. El prompt se escribe en `docs/prompts/fase-12.md`.
