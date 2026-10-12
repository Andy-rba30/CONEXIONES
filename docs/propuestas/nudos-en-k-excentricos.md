# Propuesta: nudos en K con excentricidad (los del cordón superior) — cómo describirlos y conectarlos

Fecha: 2026-10-12. Estado: **propuesta** (sin código). Sale del cierre de la Fase 11 (`docs/fases/fase-11.md`, 7.2): el
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
