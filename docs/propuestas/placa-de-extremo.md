# Propuesta: tipo de conexión nuevo `end_plate` (dos planchas atornilladas cara a cara) y su variante `base_plate` (anclaje)

Fecha: 2026-10-06. Estado: **propuesta** (sin código). Sale de los detalles G, H, I y H1 del plano de la cercha contra el
eje B que enseñó la persona en el cierre de la Fase 8, y de su aclaración: **son dos planchas, una en cada viga, unidas por
pernos con tuerca a ambos lados**; los elementos son vigas; más adelante quiere lo mismo entre una **columna y su base**
(anclaje).

## 1. Qué es y por qué no cabe en lo que hay

En el detalle H1 la diagonal `HSS2-1/2X2-1/2X3/16` termina en una plancha `PL3/8" x 185 x 185 mm` soldada a su extremo; la
viga de apoyo lleva otra plancha igual; las dos van cara a cara y las unen pernos `Ø5/8"` en agujeros `Ø18`, con tuerca a
cada lado (los pernos no roscan en nada: atraviesan las dos planchas). Nada de cartela, nada de placa cuchilla.

La versión 1 solo conoce `gusset_node`: un cordón que pasa, una cartela en el plano del nudo y, por barra, una placa
cuchilla con pernos entre cartela y cuchilla. Su esquema no puede describir una plancha perpendicular al eje de la barra ni
pernos que atraviesan dos planchas. Es un **tipo nuevo**, por el camino que describe el README en su sección 7.

## 2. Qué se reutiliza (la mayor parte)

- **El flujo y las reglas**: `conn_get_node_info`, `conn_get_schema`, `conn_validate` con `validation_token`,
  `conn_preview`, confirmación, `conn_create`, `conn_delete` reversible, un `TransactionGroup` por operación, sin ventanas.
  El tipo entra por `IConnectionType` y `conn_list_types` lo anuncia solo.
- **El contrato de pernos** (`BoltPatternSpec`: diámetro con rótulo, filas, columnas, paso, borde, longitud) y el de
  soldaduras (`WeldSpec`), las cadenas de cotas y las dudas (`uncertain_fields`), tal cual.
- **La fabricación**: el backend ya crea planchas por contorno y espesor (`CreatePlate`) y patrones de pernos con pila de
  espesores (`CreateBoltPattern` con `BoltStack`, el agarre real de la ronda 6b). Dos planchas y un patrón que las atraviesa
  es el mismo mecanismo con otro marco.
- **El marco del nudo** (`NodeFrame`): hoy X es el cordón; aquí X sería el eje de la barra que termina y la plancha el plano
  perpendicular en su extremo.
- **El catálogo**: guardar como plantilla y aplicar por ángulos funciona igual si el tipo expone sus "ranuras" (una barra y
  un apoyo).
- **El botón Ejecutar especificación JSON** y la previsualización con cotas (habrá que dibujar el alzado de las dos planchas
  con el patrón de pernos, un croquis nuevo pero con el mismo editor).

## 3. Qué es nuevo

1. **`Types/EndPlateType.cs`** con su esquema y ejemplo. Borrador de las claves (en inglés, como manda CLAUDE.md):

   ```json
   {
     "spec_version": "1.0",
     "connection_type": "end_plate",
     "member": { "element_id": 0, "profile": "HSS2-1/2X2-1/2X3/16", "end": "start|end", "end_setback_mm": 0 },
     "support": { "element_id": 0, "profile": "HSS…", "face": "auto|+y|-y|+z|-z" },
     "plates": {
       "member_plate": { "thickness_mm": 9.525, "thickness_label": "3/8\"", "width_mm": 185, "height_mm": 185, "weld": { "size_mm": 5, "all_around": true } },
       "support_plate": { "thickness_mm": 9.525, "thickness_label": "3/8\"", "width_mm": 185, "height_mm": 185, "weld": { "size_mm": 5, "all_around": true } },
       "gap_mm": 0
     },
     "bolts": { "diameter_mm": 15.875, "diameter_label": "Ø5/8\"", "hole_mm": 18, "rows": 2, "columns": 2, "spacing_mm": 0, "edge_mm": 0, "nuts": "both_sides" },
     "dimension_chains": [], "uncertain_fields": []
   }
   ```

   Lo que queda por decidir con el plano completo: cómo se reparten los 185 mm (paso y borde de los pernos), si la plancha
   del apoyo se centra en la cara del apoyo o en el eje de la barra, y si el retiro de la barra (`end_setback_mm`) es cero
   (la plancha va soldada al corte) o hay una holgura.

2. **Validaciones propias**: pernos dentro de las dos planchas con los mínimos de `limits.json` (borde, paso), agujero
   mayor que el perno, longitud del perno = dos espesores + holgura + tuerca y arandela a cada lado (la pila la calcula el
   add-in, como en la 6b), la plancha del apoyo cabe en la cara del apoyo, el perfil del plano coincide con el del modelo.

3. **Geometría**: el extremo de la barra se recorta a la cara de la plancha (ya existe `MemberModifier`), la plancha del
   miembro se coloca perpendicular a su eje en el extremo, la del apoyo pegada a la cara del apoyo que mira a la barra (o
   la que diga `face`), los pernos perpendiculares a las planchas con `nuts: both_sides`.

4. **Detección para el lote**: hoy `NodeDetector` da por `untyped` un nudo con una sola barra. Para este tipo el "nudo" es
   **una barra que termina contra un apoyo** (la barra que llega y el elemento al que llega, que no la atraviesa ni termina
   en el mismo punto). Es un caso nuevo del detector: "extremo contra apoyo". Sin eso, `end_plate` se crea de uno en uno
   (que es como se empieza) y el lote llega en una ronda posterior.

5. **Croquis de previsualización** del alzado de las planchas con el patrón de pernos y las cotas de borde y paso.

## 4. La variante de anclaje: `base_plate`

Es la misma idea con dos cambios:

- El "miembro" es una **columna** (su extremo inferior) y el "apoyo" no es una barra sino el **concreto** (una cara
  horizontal a una cota): la plancha del apoyo desaparece y en su lugar van **pernos de anclaje** embebidos (longitud de
  anclaje, gancho o cabeza, tuerca de nivelación abajo y tuerca arriba: también "tuercas a ambos lados"), con un espesor de
  **grout** entre la plancha y el concreto.
- Las validaciones cambian de "cabe en la cara del apoyo" a "borde al concreto" y "longitud embebida".

En el esquema sería `connection_type: "base_plate"` con `member` = la columna, `support` = `{ "kind": "concrete",
"top_elevation_mm": …, "grout_mm": … }` y `anchors` en vez de `bolts`. Conviene diseñar `end_plate` ya pensando en esto: la
plancha del miembro, el patrón y la pila de espesores son comunes; lo que cambia es la segunda plancha. **Recomendación**:
una misma familia de código (`PlateConnectionBase`) con dos tipos públicos, para no duplicar.

## 5. Dónde encaja y cuánto cuesta

- **Fase 13: `end_plate`** (una sesión larga o dos cortas): tipo, validador, generador, croquis, pruebas del Core con el
  detalle H1 como fixture (`docs/fixtures/detalle-H1.json` + su imagen), guía (sección nueva "cómo leer una placa de
  extremo") y una ronda en el PC con sondeos: medir en el modelo la cara del apoyo y comprobar los pernos con tuercas a
  ambos lados en Advance Steel (hoy los pernos del gusset tienen cabeza en la placa: hace falta un sondeo como el 16).
- **Fase 14: `base_plate`** (una sesión): sobre lo anterior, los pernos de anclaje, el grout y el concreto como apoyo.
- **Después, si se quiere en lote**: el caso "extremo contra apoyo" del detector y el casado en el catálogo (una ronda).

Antes de las dos, conviene terminar lo ya abierto: la instalación de la Fase 9 (ya programada, add-in 0.9.0) y la ronda 9b
del encargo para IA externa (`docs/propuestas/encargo-ia-externa.md`), porque el JSON de estas placas lo escribirá también
la IA del navegador y el encargo tendrá que llevar el esquema del tipo que toque.

## 6. Preguntas para la persona (con el plano completo delante)

- **P1.** En H1, ¿cuántos pernos hay y cómo se reparten en los 185 x 185 mm (paso y borde)? ¿El patrón es el mismo en G, H e I?
- **P2.** ¿La plancha del apoyo va soldada a la cara de la viga de apoyo, o es la propia ala de un perfil abierto?
- **P3.** ¿Hay holgura entre las dos planchas (`gap_mm`) o van en contacto?
- **P4.** ¿La barra se corta a escuadra (plancha perpendicular a su eje) o la plancha es vertical y la barra llega inclinada
  (corte en bisel)? En H1 parece perpendicular al eje; en G, H e I, contra el eje B, podría ser vertical.
- **P5.** Para el anclaje: ¿el concreto está modelado en Revit (losa, zapata, muro) o solo hay una cota? ¿Pernos de anclaje
  con gancho o con cabeza, y con tuerca de nivelación?
- **P6.** ¿Quieres estas placas también en el lote (varias diagonales contra el mismo apoyo de una vez), o de una en una
  basta para empezar?
