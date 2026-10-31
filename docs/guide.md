# Guía para la IA: crear conexiones de acero con MotorConexiones

Esta guía la devuelve `conn_get_guide`. Vive en `docs/guide.md`, `scripts/deploy.ps1` la copia junto al add-in y el
add-in la lee en cada llamada: se puede editar sin recompilar ni reiniciar Revit. Corresponde a la sección 11 del encargo.

## 0. Qué hace el add-in y qué no

- Modela en Revit lo que dice el plano de un nudo de cercha: cartela, placas cuchilla, pernos, soldaduras y el retiro
  de las barras. Usa Advance Steel si está disponible (placas y pernos nativos, categorías Plates/Bolts) y, si no,
  sólidos DirectShape de reserva. `conn_ping` dice cuál (`backend`).
- No diseña ni verifica resistencias: si el usuario pregunta si la conexión "aguanta", dile que eso no lo hace el add-in.
- No inventa datos. Lo que no se lea con certeza en el plano va a `uncertain_fields` y lo confirma el usuario.
- v1 solo sabe crear `gusset_node` (nudo con cartela, cordón HSS continuo y diagonales/montantes HSS ranurados y
  soldados, o con placa cuchilla empernada). Otros tipos (placa base, viga-columna, empalmes) no están en v1.
- Todas las operaciones de escritura son atómicas (o se crea todo o nada) y quedan como una sola entrada de deshacer en
  Revit (`MotorConexiones: <operación> <id>`). Ninguna abre ventanas.

## 1. Flujo obligatorio, en este orden

1. `conn_ping`. Si devuelve `ADDIN_NOT_LOADED`, `REVIT_UNREACHABLE` o `CONN_ROUTE_NOT_FOUND`, díselo al usuario con la
   pista (`hint`) y para: nada más va a funcionar.
2. Pide al usuario que seleccione en Revit **el cordón y todas las barras que llegan al nudo** y llama a
   `conn_get_node_info` (sin argumentos usa la selección; o pasa `element_ids`). Anota `kind` (`node` = nudo con cordón;
   `splice` = dos tramos colineales que se encuentran: un empalme, tipo `end_plate`), `eccentricity_mm` (si no es 0 es un
   nudo en K: copia `node.eccentricity_mm` y, por barra, `axis_offset_mm`), `chord_element_id`, `origin_mm`, los ejes y,
   por cada barra, `element_id`, `type` (el perfil real del modelo), `angle_in_plane_deg` y `side`.
   Si `existing_connections` no está vacío, ese nudo ya tiene una conexión del add-in: pregunta si quiere
   actualizarla (`conn_update`) o borrarla (`conn_delete`) antes de crear otra.
3. `conn_get_schema` del tipo que toca (`gusset_node` si el plano trae cartela; `direct_weld` si las barras van soldadas
   directamente a la cara del cordón, sin cartela, como los nudos en K del cordón superior; `end_plate` si `kind` es
   `splice`: sección 7): copia `data.example` y adáptalo. No añadas campos que no estén en el esquema.
4. Lee el detalle (imagen o datos que te dé el usuario): rótulos de perfiles, espesores y medidas de placas, pernos
   (diámetro, filas, columnas, paso, distancia al borde), soldaduras y las cotas. Reglas de lectura en la sección 2.
5. Transcribe **cada cadena de cotas completa** en `dimension_chains` con el total que debería dar. El validador
   comprueba que suman (tolerancia 1 mm): es la forma de detectar lecturas equivocadas.
6. Lo ilegible, ausente o dudoso va en `uncertain_fields` con `path`, `reason` y `user_confirmed_value: null`.
   El campo apuntado puede quedar `null` solo si está en esa lista.
7. `conn_validate` con la especificación. Si hay errores, corrige los tuyos (esquema, rótulos, cadenas mal sumadas,
   perfiles) y repite; si el error depende del plano (pernos fuera de mínimos, placa fuera de la cartela, duda sin
   confirmar) pregunta al usuario y rellena `user_confirmed_value` **y** el campo correspondiente. Sin errores recibes
   `validation_token`. Las `warnings` no bloquean: enséñalas.
8. Muestra al usuario un resumen corto (tabla: cordón, cartela, cada barra con su unión, pernos, soldaduras, retiros,
   dudas y cómo se resolvieron, advertencias) y **espera su confirmación explícita**.
9. `conn_preview` (sin cambios en el modelo) y, con el visto bueno, `conn_create` con la misma especificación y el
   token. Informa el `connection_id` y el `backend`. Para enseñar el resultado usa `get_revit_view` de una vista 3D.
10. Después: `conn_list` y `conn_get` para consultar; `conn_update` (validar la especificación nueva primero) para
    cambiar; `conn_delete` para quitar la conexión y devolver las barras a su longitud original. Pide confirmación
    antes de borrar.

## 2. Cómo leer un detalle de acero

- Las cotas van en **mm** salvo que el plano diga otra cosa; los rótulos de espesores y diámetros suelen ir en pulgadas
  (`3/8"` = 9,525 mm; `Ø5/8"` = 15,875 mm; `1/4"` = 6,35 mm; `3/16"` = 4,7625 mm). Escribe el número en mm en el campo
  `*_mm` y el texto del plano tal cual en el campo `*_label`: el validador comprueba que coinciden (0,05 mm).
- `PL 3/8"`, `PL10`: placa de ese espesor. `PL10 x 170 x 140`: espesor x largo x ancho.
- `HSS3X3X1/4`, `HSS2-1/2X2-1/2X3/16`: perfiles tubulares AISC. Escribe la designación del plano en `profile`; el
  validador la compara con el tipo del modelo aunque Revit lo llame `HSS2-1-2X2-1-2X3-16 64x64`. Si no coincide,
  `conn_find_profile` te dice qué hay cargado.
- `TIP.` = típico (vale para todos los elementos iguales). Un círculo en el símbolo de soldadura = soldadura en todo el
  contorno (`all_around: true`). El número junto al símbolo es el tamaño del filete en mm (`size_mm`).
- Pernos: `4 Ø5/8"` con cotas `40 + 60 + 40` a lo ancho significa 2 columnas a 60 mm de paso y 40 mm al borde
  (`columns: 2`, `spacing_mm: 60`, `edge_mm: 40`). Las filas se cuentan a lo largo del miembro.
- Los ángulos y las posiciones de las barras salen del modelo (`conn_get_node_info`), no del dibujo. Si el plano trae
  un ángulo, ponlo en `expected_angle_deg`: solo produce la advertencia `ANGLE_DIFFERS_FROM_MODEL` si difiere.

## 3. Sistema de coordenadas y posiciones

- Marco **canónico** del nudo (Fase 7): X = eje del cordón orientado hacia +X global (si el cordón va en Y, hacia +Y),
  Y en el plano de la cercha orientado hacia +Z global (en una cercha vertical +Y es "hacia arriba"), Z = X × Y. Origen
  = punto de trabajo (cruce del eje del cordón con el del primer miembro de `members`). No depende de cómo esté dibujado
  el cordón (`chord_direction_reversed` solo informa). `conn_get_node_info` lo devuelve, y por barra `angle_in_plane_deg`
  **con signo** desde +X (+45° arriba a la derecha, −135° abajo a la izquierda), `angle_to_chord_deg` (la inclinación
  sin signo que escribe el plano) y `side` (`+Y`/`-Y`). En `expected_angle_deg` escribe la inclinación del plano (45°):
  el validador la compara sin signo con la del modelo.
- `gusset.outline.points_mm`: vértices del contorno de la cartela en ese sistema, en mm, en orden (contorno cerrado
  implícito, sin cruces). Las cotas del plano se miden desde los bordes de la cartela: sitúala respecto al punto de
  trabajo con las cotas que lo relacionan con los bordes; si el plano no lo deja claro, es una duda.
- `gusset.outline.mode` (Fase 13): `"polygon"` (lo anterior: tú escribes `points_mm`, porque el plano acota la cartela) o
  **`"auto"`**: **no escribas `points_mm`** (sería `SCHEMA_INVALID`); el add-in calcula el contorno desde las barras del modelo
  (un rectángulo por barra desde su retiro hasta el final de la ranura o la placa más 40 mm, 30 mm a cada lado, la franja del
  cordón más 40 mm, envolvente convexa redondeada a 5 mm, 12 vértices como mucho). Usa `auto` cuando el plano **no** acota la
  cartela o cuando el usuario quiere "la cartela que haga falta"; opcionalmente `margin_mm`, `side_margin_mm` y
  `beyond_chord_mm`. `conn_validate`, `conn_preview` y `conn_catalog_apply` devuelven los vértices calculados en
  `calculated_values.outline_points_mm` (más `outline_width_mm` y `outline_height_mm`): enséñaselos al usuario como la caja de la
  cartela; si quiere cambiarlos, pasa a `polygon` copiando esos puntos y editándolos. El aviso `OUTLINE_AUTO_LARGE` dice que la
  cartela sale grande (una barra lejos o un margen absurdo). Con `auto`, `width_mm` y `height_mm` deben ser la caja calculada (o
  quítalos).
- `members[].end_setback_mm`: distancia desde el punto de trabajo hasta el extremo recortado de la barra. Fase 12: si la
  barra tiene `axis_offset_mm` (nudo en K), se mide desde **su propio corte** con el eje del cordón, no desde el punto de
  trabajo; en `end_plate` se mide desde el punto de empalme.
- **Excentricidad (Fase 12, opción A)**: cuando los ejes de las barras cortan al del cordón en puntos distintos (los nudos en
  K del cordón superior: 54,9 mm entre cortes, medidos en Revit), el punto de trabajo es el punto del eje del cordón a mitad entre esos cortes.
  `conn_get_node_info` devuelve `eccentricity_mm` y, por barra, `axis_offset_mm` (con signo a lo largo de +X) y `axis_cut_mm`;
  cópialos a `node.eccentricity_mm` y `members[].axis_offset_mm` (opcionales: el validador los mide igual). Hasta medio canto
  del cordón es el aviso `MEMBER_ECCENTRIC` y no hay que corregir nada; por encima, `MEMBER_NOT_AT_NODE`. En `node_info`,
  `width_mm` es el lado del perfil **en el plano de la cercha** y `height_mm` el **perpendicular** (`section_rotation_deg` es el
  giro de la sección): el cordón superior HSS12X8X1/2 del Hangar va girado 90°, así que mide 203,2 en el plano (su medio canto
  es 101,6) y 304,8 perpendicular; en un tubo cuadrado los dos coinciden.
- `bolted_knife_plate`: `plate.insertion_mm` es la parte de la placa cuchilla dentro de la ranura del HSS;
  `bolts.first_row_from_plate_end_mm` se mide desde el extremo libre de la placa (el que apoya en la cartela).
  La placa cuchilla apoya **sobre una cara** de la cartela (no en su mismo plano) y los pernos atraviesan cartela +
  placa: `plate.gusset_face` dice qué cara (`"+z"` por defecto, `"-z"` la opuesta, en el sistema local); si el plano no
  lo muestra, es una duda (`uncertain_fields`). La longitud del perno se calcula del agarre (suma de espesores) más el
  suplemento de tuerca y rosca de `limits.json`; si el plano la indica, ponla en `bolts.length_mm` (`conn_validate`
  devuelve en `data.bolt_stacks` el agarre y la longitud que se crearán).
- `gusset.chord_interface`: `through_slot` (la cartela atraviesa el cordón ranurado), `split_top_bottom` o `side_lap`.
  Si el plano no lo muestra, va a `uncertain_fields`.
- `source.drawing_image` (Fase 11, opcional): la ruta de la imagen del detalle (PNG o JPG) en el PC de la persona; la
  ventana de previsualización la abre al lado del croquis. **No tienes que escribirla** (la pone la ventana cuando la
  persona abre el plano); si la ves en una especificación, déjala como está. El validador no comprueba que el archivo exista.

## 4. Qué hacer con cada error de `conn_validate`

| Código | Qué significa | Qué haces |
|---|---|---|
| `SCHEMA_INVALID` | Campo obligatorio ausente, tipo o rango incorrecto, campo desconocido | Corrígelo tú con el esquema |
| `UNRESOLVED_UNCERTAINTY` | Una duda sin `user_confirmed_value` | Pregunta al usuario; rellena el valor confirmado y el campo |
| `DIMENSION_CHAIN_MISMATCH` | Una cadena de cotas no suma su total | Vuelve a leer el plano; si no cierra, pásalo a `uncertain_fields` |
| `LABEL_VALUE_MISMATCH` | El rótulo (`3/8"`, `PL10`) no coincide con los mm | Corrige el número o el rótulo |
| `PROFILE_MISMATCH` | El perfil escrito no es el del modelo | `conn_find_profile`; si el modelo tiene otro perfil, avisa al usuario |
| `BOLT_EDGE_DISTANCE_TOO_SMALL`, `BOLT_SPACING_TOO_SMALL` | Pernos por debajo de los mínimos AISC configurados | Depende del plano: pregunta al usuario |
| `BOLT_LENGTH_TOO_SHORT` (advertencia) | `bolts.length_mm` es menor que el agarre (cartela + placa) más tuerca, arandela y rosca | Confirma la longitud con el usuario o quita `length_mm` para que se calcule del agarre |
| `BOLT_OUTSIDE_PLATE`, `PLATE_OUTSIDE_GUSSET`, `OUTLINE_INVALID` | Geometría imposible | Revisa medidas y contorno; pregunta si hace falta (con `outline.mode: "auto"` no debería pasar: la cartela envuelve las barras) |
| `OUTLINE_AUTO_LARGE` (advertencia, Fase 13) | La cartela automática mide más de 1 200 mm de lado o su área dobla la de las barras | Enséñalo: alguna barra queda lejos del nudo o un margen es absurdo; si el usuario quiere recortarla, pasa a `polygon` con los `outline_points_mm` editados |
| `SPLIT_IN_USE`, `SPLIT_NOT_FOUND`, `SPLIT_FAILED` (Fase 13, `conn_split_member` / `conn_unsplit`) | La barra ya está partida o una conexión usa los tramos; no hay corte que deshacer; el corte cayó fuera de la barra o Revit no copió el elemento (rollback completo) | `conn_unsplit` lista los cortes; borra antes las conexiones que usan los tramos; comprueba la barra y la distancia |
| `SPLIT_PARAMETERS_APPROXIMATED`, `SPLIT_UNDO_PARTIAL` (advertencias, Fase 13) | Algún parámetro del tramo nuevo no se pudo copiar (lista); la barra no recuperó exactamente su curva al deshacer | Informativos: díselo al usuario con la lista |
| `CLASH_WITH_FOREIGN_MEMBER` | La cartela choca con una barra que no es del nudo | Avisa al usuario; quizá falte una barra en la selección |
| `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE` | IDs que no son del nudo (Fase 12: `MEMBER_NOT_AT_NODE` solo por encima de medio canto del cordón) | Repite `conn_get_node_info` con la selección correcta |
| `MEMBER_ECCENTRIC` (advertencia, Fase 12) | Nudo en K con excentricidad: el eje de la barra corta el cordón a tantos mm del punto de trabajo, hasta medio canto | Nada que corregir: enséñalo; cada barra se recorta sobre su propio eje |
| `SPLICE_NOT_COLLINEAR`, `SPLICE_SETBACK_TOO_SMALL`, `SPLICE_PROFILES_DIFFER` (Fase 12, `end_plate`) | Los dos tramos no son colineales (más de 5°); el retiro no deja sitio a la plancha (espesor + `gap_mm`/2 menos el hueco medido); perfiles distintos (advertencia) | Comprueba que son dos tramos de la misma barra; pon `end_setback_mm` al valor que dice el mensaje; revisa el perfil en el plano |
| `BOLT_HOLE_TOO_SMALL`, `BOLT_TOO_CLOSE_TO_TUBE`, `PLATE_SMALLER_THAN_TUBE` (Fase 12, `end_plate`) | Agujero no mayor que el perno (Ø18 para 5/8", Ø13/16" para 3/4"); un perno a menos de 20 mm de la cara del tubo; la plancha no cubre el tubo | Corrige `hole_mm` y `hole_label`; acerca la fila al borde o agranda la plancha; agranda la plancha (pregunta si el plano no lo da) |
| `MEMBER_WIDER_THAN_CHORD_FACE`, `MEMBERS_OVERLAP_ON_FACE` (advertencias, Fase 12, `direct_weld`) | La barra es más ancha que la cara del cordón; dos huellas se pisan en la cara | Enséñalo al usuario; revisa el perfil o la orientación del cordón |
| `END_CUT_APPROXIMATED`, `BOLT_NUTS_APPROXIMATED` (advertencias al crear, Fase 12) | El corte inclinado contra la cara no está disponible (la barra se acortó por su eje); la tuerca a ambos lados se representa con el perno estándar | Informativos: díselo al usuario |
| `NODE_AXES_NOT_INTERSECTING` | Los ejes de cordón y primer miembro no se cruzan (> 5 mm) | El modelo tiene las barras desplazadas: díselo al usuario |
| `VALIDATION_TOKEN_INVALID` (en `conn_create`/`conn_update`) | Token ausente, o la especificación o el modelo cambiaron | Vuelve a `conn_validate` con la especificación final |
| `REVIT_BUSY` | Revit tiene una orden o un diálogo abierto | Pide al usuario que lo cierre y repite |
| `NO_DOCUMENT` | No hay modelo abierto | Pide al usuario que abra el modelo |

## 5. Catálogo de plantillas (Fase 7): guardar una típica y aplicarla a otro nudo

Una **plantilla** es una conexión guardada sin los IDs del nudo, con el ángulo real de cada barra y una regla para casar
barras. Vive como archivo JSON en el PC (`conn_catalog_list` dice la carpeta). Sirve para repetir la "típica" en los
demás nudos de la cercha sin volver a leer el plano.

- **Guardar**: después de crear y comprobar un nudo, si el usuario dice "guárdala como típica", llama a
  `conn_catalog_save` con `connection_id` (de `conn_create` o `conn_list`), un `name` corto y, si lo dice, `description`
  y `tags`. También acepta `spec` (una especificación que valide) en vez de `connection_id`. Las dudas deben estar
  confirmadas (`TEMPLATE_HAS_OPEN_UNCERTAINTIES`) y la especificación sin errores (`TEMPLATE_SPEC_INVALID`). Si el nombre
  ya existe, pregunta al usuario antes de pasar `overwrite: true`. Informa el `template_id`.
- **Aplicar a un nudo**: pide al usuario que seleccione el cordón y todas las barras del nudo nuevo y llama a
  `conn_catalog_apply`. Desde la Fase 11 **no hace falta elegir la plantilla**: sin `template_id` el add-in casa **todas**
  las del catálogo y aplica la mejor (menor desvío; a igual desvío, la más reciente), la misma regla que el botón
  **Conectar** de la cinta; `data.proposal.summary_text` dice cuál eligió y cuáles más encajan (`candidates`, de mejor a
  peor, con `orientation` y `max_deviation_deg`): enséñaselo al usuario y, si prefiere otra, repite con su `template_id`.
  Si quieres forzar una, pasa `template_id` (de `conn_catalog_list`) y `element_ids` si no usas la selección. Si ninguna
  encaja, `TEMPLATE_NO_MATCH` con `data.proposal` (los intentos); con el catálogo vacío, `CATALOG_EMPTY`.
  El add-in casa cada barra con una ranura por su ángulo, probando también la plantilla **en espejo**
  (`match.orientation`: `same`, `mirror_x`, `mirror_y`, `both`), escribe los IDs, transforma la cartela y **ya valida**:
  `data.spec` y `data.validation_token` van tal cual a `conn_preview` y `conn_create`. No llames a `conn_validate` otra
  vez salvo que cambies algo de `data.spec` (entonces el token deja de valer y hay que revalidar).
- **Qué enseñar al usuario** antes de crear: la orientación usada (si salió en espejo, dilo), `match.assignments` (qué
  barra ocupa cada ranura y su desvío en grados) y los avisos `TEMPLATE_ANGLE_DEVIATION` (una barra llega con otro ángulo:
  la unión sigue el ángulo real, pero la cartela es la de la plantilla y puede no cubrirla; `conn_validate` lo marca con
  `PLATE_OUTSIDE_GUSSET`) y `TEMPLATE_PROFILE_DIFFERS` (otro perfil: se usó el del modelo; el usuario decide si sigue).
- **Si no casa** (`TEMPLATE_NO_MATCH`): `data.attempts` dice, por orientación, qué ranura no encontró barra y qué barras
  sobraron. Casi siempre falta una barra en la selección o el nudo es de otro tipo; díselo al usuario.
- `conn_catalog_get` enseña la plantilla completa; `conn_catalog_delete` borra el archivo (pide confirmación).
- Las conexiones creadas desde una plantilla llevan `source.template_id` (se ve en `conn_list`): así más adelante se
  podrán rehacer todas las que salieron de una típica.

## 6. Lotes (Fases 8 y 9): planificar una cercha entera, crear el lote y borrarlo

El **plan de lote** detecta todos los nudos de una cercha, casa cada uno con las plantillas del catálogo y valida nudo a
nudo; **planificar no crea nada**. Crear es un paso aparte (`conn_batch_create`, Fase 9) que exige los tokens del plan y
la confirmación del usuario. Flujo: planificar → revisar con el usuario → corregir → replanificar → **confirmar** →
crear el lote → informe; `conn_batch_delete` lo quita entero si no gusta; al final, descartar las marcas que queden.

- **Planificar**: pide al usuario que seleccione en Revit **todas** las barras de la cercha (cordones, diagonales y
  montantes; sin columnas ni correas) y llama a `conn_batch_plan` (con `template_ids` si quiere una plantilla concreta;
  sin ellos se prueban todas). **Fase 10, más fácil**: con `expand_selection: true` basta que el usuario **pinche una
  barra** de la cercha: el add-in añade las que la tocan (cordones que pasan de largo, barras que llegan, tramos del
  cordón) dentro del plano de la cercha y planifica con todas; la respuesta trae `selection_expansion` (`added_count`,
  `added_element_ids`, `summary_text`: "Se añadieron 63 barras que tocan la selección: …") y el aviso `SELECTION_EXPANDED`:
  dile al usuario cuántas añadió con ese `summary_text` (si `limit_reached` es `true`, pídele que seleccione la cercha a
  mano). El add-in lleva cada extremo al corte de su eje con el eje del cordón (las diagonales reales
  terminan en la cara del cordón, no en su eje; se admiten hasta medio canto de cada barra más 10 mm), agrupa esos puntos
  (10 mm), reconoce el cordón que atraviesa cada punto, calcula el marco canónico, nombra los nudos `N1, N2…` a lo largo
  de la cercha, salta los que ya tienen conexión, casa cada nudo
  (también en espejo) y valida con las mismas reglas de `conn_validate`: cada nudo `ready` trae su `validation_token`.
  Con `mark` (por defecto) colorea en la vista las barras de cada nudo **con el color de su estado** (verde = se creará,
  ámbar = se creará con aviso, rojo = falta algo, gris = no se crea; `color_name` lo dice) y pone un marcador con su nombre
  (cubo = misma orientación que la plantilla, rombo = en espejo); las barras sueltas y las parejas sin cordón no se marcan.
  Fase 10: con las marcas van también una **etiqueta pinchable** con el número de cada nudo visible (`labels`, por defecto
  `true`; `data.labels.count` y `nodes[].label_index`; pincharla en Revit elige el nudo en la ventana del plan de la cinta y
  nunca abre un cuadro; si no se pudieron poner, aviso `PLAN_LABELS_SKIPPED` y el plan sigue) y una **cartela fantasma**
  transparente en cada nudo listo (`nodes[].ghost_element_id`, `marks.ghost_count`): el usuario ve en 3D lo que se va a
  crear antes de crearlo; Crear la sustituye por acero y descartar la quita.
  Enséñaselo con `get_revit_view`. En un documento solo hay un plan
  marcado: marcar otro plan quita las marcas del anterior (aviso `PLAN_MARKS_REPLACED`; ese plan sigue en memoria y se
  vuelve a ver replanificándolo con su `plan_id`).
- **Qué enseñar al usuario** (ronda 8c): **no vuelques el JSON**. Enseña `data.summary_text` tal cual (es la misma
  cabecera que ve en la ventana: "Se crearán 16 conexiones con Nudo tipico Detalle D (8 iguales, 8 en espejo). 14 avisan de
  perfil distinto. Ocultos: 20 sin cordón, 23 barras sueltas.") y debajo una tabla corta **solo con los nudos
  `visible_by_default: true`**, con `name`, `status_text` (ya viene en español, con icono), espejo (`orientation`: `same` = no,
  `mirror_x` = sí), `template_name` y `advice` (qué hacer). Los ocultos (`visible_by_default: false`: extremos sueltos y
  parejas de dos barras sin cordón, que no son nudos) se resumen en **una línea** ("Ocultos: 20 sin cordón, 23 barras
  sueltas"), sin listarlos. Los estados internos (`ready`, `no_match`, `untyped`…), los tokens, los IDs de barra y
  `end_gap_mm` no se enseñan salvo que el usuario los pida. Cuatro líneas y una tabla de pocas filas bastan: la
  conversación debe ser corta. Si la respuesta trae el aviso `CATALOG_EMPTY`, dilo con sus palabras: no hay plantillas,
  hay que crear primero la conexión de un nudo y guardarla con `conn_catalog_save` (o el botón **Guardar en catálogo**).
  Qué significa cada estado, por si pregunta: `ready` (listo; con avisos, "listo con aviso": por ejemplo
  `TEMPLATE_PROFILE_DIFFERS`, el cordón tiene otro perfil que la plantilla y se crea con la misma cartela), `invalid` (casa
  pero no valida: suele ser la cartela fija que no cubre una barra con otro ángulo, `PLATE_OUTSIDE_GUSSET`), `no_match`
  (ninguna plantilla encaja: falta o sobra una barra, o es otro tipo de nudo; `attempts` lo detalla), `ambiguous_chord`
  (dos barras atraviesan el nudo: hay que elegir el cordón), `offset` (los ejes no se cortan: arreglar el modelo o
  excluir), `untyped` (una sola barra: extremo suelto, no se toca), `already_connected` (se salta; `replace_existing: true`
  lo planifica para rehacerlo en la Fase 9). Un `no_match` con el aviso `NODE_CHORD_NOT_CONTINUOUS` ("Falta el cordón") no
  tiene ninguna barra que lo atraviese: es un extremo de cercha, **falta el cordón en la selección** (pide al usuario que
  seleccione también los cordones superior e inferior y replanifica con el mismo `plan_id`) o el cordón está modelado en
  tramos que terminan justo ahí (un **empalme**: en la cercha del Hangar, los siete nudos del cordón superior donde acaban
  dos tramos de HSS12X8; `overrides.chord` fija uno de los tramos). Un `no_match` **con** cordón ("Sin plantilla que
  encaje") es un nudo de otro tipo: en la cercha del Hangar, los diez nudos del cordón superior tienen dos diagonales desde
  abajo y el Detalle D es la típica del cordón central con tres barras, así que no encaja en ninguna orientación; hace
  falta otra plantilla (crear ese nudo a mano y `conn_catalog_save`) o excluirlos. No es un fallo de la detección. Un
  nudo `✖ Empalme del cordón` (Fase 9) es un punto donde el cordón está cortado en dos tramos que terminan ahí: no falta
  ningún cordón en la selección; tampoco tiene plantilla y se excluye o se le crea su típica. `members[].end_gap_mm` dice
  cuánto se queda corta cada barra respecto al punto de trabajo (0 = llega al eje). Cada nudo trae además `actions`
  (los botones de la ventana: `exclude`, `chord`, `template`, `edit`, `show`…): sirven para explicar qué puede hacer el
  usuario; tú aplicas lo mismo con `overrides`.
- **Corregir**: las correcciones del usuario van en `overrides` de otra llamada a `conn_batch_plan` **con el mismo
  `plan_id`** (se acumulan y los nombres de nudo no cambian): `exclude` / `include`, `chord: {"N4": id}`,
  `template: {"N9": "<template_id>" | null}`, `remove_member` / `add_member: {"N2": [ids]}`, `add_node: {"N11": [ids]}`
  (un nudo que no se detectó, dado por sus barras), `merge: [["N5", "N6"]]`, `split: {"N5": [[ids], [ids]]}` y
  `spec: {"N4": {...}}` (una especificación editada a mano para ese nudo; vuelve a validarse). `conn_batch_plan_get` relee
  el plan (con `node` devuelve un nudo con su especificación completa); el `overrides` que devuelve se puede enviar tal
  cual en la petición siguiente.
- **Crear el lote** (Fase 9): cuando el usuario dé el visto bueno al plan (enséñale `summary_text` y la tabla corta, y
  espera su **confirmación explícita**: "crea las 16"), llama a `conn_batch_create` con `plan_id` y `nodes` = los nudos
  `ready` con el `validation_token` que trajo el plan (`[{"node": "N4", "validation_token": "…"}, …]`). Sin token no se
  crea nada, y si replanificaste después hay que pasar los tokens nuevos (`conn_batch_plan_get` los trae). El add-in crea
  cada nudo como una operación propia (igual que `conn_create`: comprueba el token contra el modelo, cartela, placas,
  pernos, soldaduras, retiros, registro con `source.batch_id` = `plan_id`): **si un nudo falla se revierte solo y los
  demás se quedan**; en Revit el lote entero es una sola entrada de deshacer (Ctrl+Z). Los nudos `invalid`, `no_match`,
  `excluded`, `untyped`, `already_connected` y los ya creados se saltan con su motivo (P6: los nudos "Listo con aviso"
  por perfil distinto **sí** se crean, con la cartela de la plantilla). Por defecto `stop_on_error: false`; con `true`
  el primer fallo revierte todo (`BATCH_STOPPED`). La llamada puede tardar varios minutos (cada nudo abre su sesión de
  Advance Steel): **si no responde, no la repitas a ciegas**: `conn_list` con `batch_id` = `plan_id` dice qué quedó
  creado. Enseña al usuario `data.summary_text` ("Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: …)…")
  y los nudos `failed` con su motivo (`nodes[].description`); no vuelques el JSON. Las marcas de los nudos creados
  desaparecen (el acero las sustituye); si después replanificas con el mismo `plan_id`, esos nudos salen
  `already_connected` con `status_text` "◌ Ya creada en este lote". Para enseñar el resultado, `get_revit_view`.
- **Borrar el lote**: `conn_batch_delete` con `batch_id` = el `plan_id` borra todas las conexiones del lote, una a una
  con las garantías de `conn_delete` (solo lo que creó el add-in; las barras recuperan su extensión), en una sola entrada
  de deshacer. **Pide confirmación** antes (dile cuántas son: `conn_list` con `batch_id`). Si el plan sigue en memoria,
  sus nudos vuelven a `ready` con su token. Sin conexiones de ese lote devuelve `deleted_count: 0` y el aviso `BATCH_EMPTY`.
- **Terminar**: `conn_batch_plan_discard` quita los colores, los marcadores, las cartelas fantasma y las etiquetas
  (`removed_labels`) y olvida el plan (con `all: true` limpia también marcas y etiquetas de planes olvidados); no toca las
  conexiones creadas. El usuario puede tener abierta la ventana del plan
  de la cinta mientras tú trabajas (desde la 0.8.4 no es modal): los planes son los mismos en memoria, en un documento
  solo hay un plan marcado, y si él pulsa **Descartar plan** en la ventana se quitan **todos** los marcadores del
  documento (también los de tu plan, que sigue en memoria sin marcas: replanifica con su `plan_id` para volver a verlo) y
  la ventana se cierra (0.8.5). Él también puede crear el lote desde la ventana (**Crear N conexiones**) y borrarlo
  (**Borrar el lote**): `conn_batch_plan_get` lo refleja (`created_count`, `last_report`). Llama a descartar si el usuario
  no va a seguir o antes de que guarde el modelo. Un nudo suelto del plan se puede crear también como siempre:
  `conn_batch_plan_get` con `node` → `data.node.spec` y `data.node.validation_token` → `conn_preview` → confirmación →
  `conn_create` (su especificación trae `source.batch_id` del plan, así que `conn_batch_delete` la borraría con el lote).

## 7. Cómo leer una placa de extremo y un nudo soldado (Fase 12)

Desde la Fase 12 hay tres tipos. `conn_list_types` los describe y `conn_get_schema` da el esquema y un ejemplo de cada uno.

**`end_plate` (empalme con placa de extremo; Detalles B, C e I del plano).** Dos tramos **colineales** de la misma barra
(`conn_get_node_info` con los dos devuelve `kind: splice` y `origin_mm` en el punto donde se encuentran) unidos por **dos
planchas** cara a cara, perpendiculares al eje, cada una soldada a su tramo, con **pernos alrededor del tubo** que atraviesan
las dos y llevan tuerca a ambos lados.

- `kind: "splice"`; `node.element_ids` con los dos tramos; `chord` es el primer tramo (`continuous: false`) y `members[0]` el
  segundo (`role: "chord"`, `expected_angle_deg: 0`). Los perfiles de los dos, con la designación del plano.
- `plate`: `thickness_mm` + `thickness_label` (`PL 1/2"` → 12,7), `width_mm` × `height_mm` (en el Detalle C el 430 va en el
  sentido del canto mayor del tubo, el 12"; en el I y el B es cuadrada), `corner_cut_mm` (las esquinas recortadas: 60; 0 =
  rectángulo), `weld` {`type: "fillet"`, `size_mm`, `all_around: true`}.
- `bolts`: `diameter_mm` + `diameter_label` (`Ø3/4"` → 19,05), `hole_mm` + `hole_label` (`Ø13/16"` → 20,638; `Ø18` → 18),
  `nuts: "both_sides"`, y **`faces`**: una fila por cara de la plancha (`top`, `bottom`, `left`, `right`) con `count`,
  `spacing_mm` (paso entre pernos de la fila; con `count: 1` no hace falta) y `edge_mm` (del borde de la plancha al eje de los
  pernos). Detalle C: arriba y abajo 4 a 90, izquierda y derecha 2 a 80, bordes de 25; Detalles B e I: 1 por cara a 25 del borde.
  Las cotas "pernos a 135" o "a 150" entre pernos opuestos son `width_mm − 2 × edge_mm`: escríbelas como `dimension_chains`.
- `gap_mm`: holgura entre las dos planchas (0 = en contacto). `chord.end_setback_mm` y `members[0].end_setback_mm`: cuánto se
  retira cada tramo desde el punto de empalme = espesor de su plancha + `gap_mm`/2 (si el tramo ya deja hueco en el modelo,
  el validador lo resta: `SPLICE_SETBACK_TOO_SMALL` dice el mínimo).
- El validador comprueba los mínimos de borde y paso de siempre por fila (`bolts.faces.<cara>.edge_mm`), que ningún perno quede
  a menos de 20 mm de la cara del tubo, que el agujero sea mayor que el perno, que la plancha cubra el tubo y que los dos tramos
  sean colineales. Al crear, el aviso `BOLT_NUTS_APPROXIMATED` dice que la tuerca doble se representa con el perno estándar.

**`direct_weld` (nudo en K soldado, sin cartela; los nudos del cordón superior).** Cada diagonal se corta contra la cara del
cordón y se suelda en todo el contorno. No hay `gusset` ni `attachment`.

- `chord` {`element_id`, `profile`, `continuous: true`, `face: "auto"`}; `members[]` de 1 a 3 barras con `role`, `profile`,
  `expected_angle_deg`, `axis_offset_mm` (de `conn_get_node_info`), `end_cut: "to_chord_face"`, `end_gap_mm` (holgura entre el
  corte y la cara; 0 si el plano no la da), `weld` {`fillet`, `size_mm` (5 en el Hangar), `all_around: true`}.
- `splice_hint` (opcional, por barra) {`detail: "B"`, `offset_from_chord_face_mm: 330`}: la marca del empalme de la diagonal
  (Detalle B) a esa distancia de la cara del cordón, medida a lo largo de la diagonal. Se dibuja y se guarda; **desde la Fase 13 se
  fabrica partiendo la diagonal** (abajo).
- **La cadena del nudo en K (Fase 13)**: la diagonal es una sola barra en el modelo, y el Detalle B pide dos tramos con una
  plancha cada uno. Orden, siempre con confirmación del usuario y `dry_run` primero: (1) `conn_get_node_info` del nudo en K: por
  diagonal, `split_point_mm` y `split_distance_mm` (≈ 485 mm del corte de los ejes: cara del cordón + 330 + espesor de la
  plancha); (2) `conn_split_member(element_id = la diagonal, chord_element_id = el cordón, dry_run: true)` para enseñar el punto;
  después sin `dry_run`: el **tramo corto** (del lado del cordón) conserva el `element_id`, el largo es `new_element_id`;
  (3) `conn_get_node_info([element_id, new_element_id])` → `kind: splice`; `conn_catalog_apply` (o `conn_validate` con el
  arranque `end_plate` del Detalle B) y `conn_create` del empalme sobre los dos tramos; (4) el nudo `direct_weld` sobre el cordón y
  los **tramos cortos** (los ids de siempre): vuelve a `conn_validate` (el token cambió con la curva de la barra) y `conn_create`.
  Para deshacer: `conn_delete` del nudo en K y de los empalmes B, después `conn_unsplit` (o `conn_delete` del nudo en K último: él
  pidió el corte y lo deshace si ya no lo usa nadie). En el botón Conectar todo esto es una casilla; en el plan de lote
  (`conn_batch_plan`), cada nudo en K trae `split_steps[]` y detrás los nudos `N5B` (`status: after_split`, `kind: splice`): el
  lote parte, crea los empalmes B y después el nudo en K, y `conn_batch_delete` deshace los cortes al final (`split_members: false`
  en `conn_batch_create` o `split_diagonals: false` en `conn_batch_plan` lo desactivan).
- `node.eccentricity_mm`: la distancia entre los cortes de las dos diagonales (54,9 mm en el Hangar). Con ella `conn_validate`
  devuelve el aviso `MEMBER_ECCENTRIC` y nada más que corregir. Al crear, el aviso `END_CUT_APPROXIMATED` dice que, hasta que
  el corte inclinado esté disponible, cada barra se acorta por su eje lo justo para no entrar en el cordón.
- Cuando el nudo en K cae sobre un empalme del cordón, son **dos conexiones** en el mismo punto: primero el `end_plate` de los
  dos tramos y después el `direct_weld`, con `members[].chord_element_id` = el tramo al que llega cada diagonal (el plan de
  lote lo hace solo: nudos `N9E` y `N9`).

**Las soldaduras creadas son de Advance Steel** (en los dos tipos, desde la ronda 12c): Revit las dibuja como **símbolos sin
volumen** y solo en nivel de detalle **Fino**. Si el usuario dice que "no se ven" o "no tienen cuerpo", no es un fallo:
`created_elements_count` cuenta cada una (2 en un nudo en K; 8 en un empalme: 2 planchas, 4 patrones de pernos y 2 soldaduras)
y `conn_delete` las borra con la conexión. Los avisos `REVIT_WARNING` que empiezan por "Advertencia de Revit: …" son avisos del
propio Revit al confirmar, con "[elementos: …]" desde la 0.12.3; la conexión está creada. Resume su texto al usuario sin asustar.
La advertencia "The created elements are only visible in Detail Level: Fine." (una por elemento de Advance Steel) **ya no llega**
desde la 0.12.4: es informativa y queda solo en el log del add-in. Desde la 0.13.2, al **partir una barra** (`conn_split_member`, la
casilla de Conectar o el lote) tampoco llega la advertencia "Beam or Brace is slightly off axis and may cause inaccuracies." del tramo
nuevo: es de la geometría del modelo (la barra original ya se sale unas milésimas de milímetro de un plano vertical) y el add-in copia
su eje exacto; queda en el log. Si llega un `REVIT_WARNING`, es otra cosa: léelo y resúmelo.

## 8. Reglas de seguridad

- Nunca llames a `conn_create` ni a `conn_update` sin `conn_preview` y la confirmación explícita del usuario (también
  cuando la especificación viene de `conn_catalog_apply`). Nunca llames a `conn_batch_create` sin haber enseñado el plan
  (`summary_text` y la tabla corta) y sin la confirmación explícita del usuario: crea acero en muchos nudos a la vez.
- Si `conn_create` o `conn_batch_create` no responden (tiempo de espera), **no los repitas a ciegas**: `conn_list` (con
  `batch_id` para un lote) dice qué quedó creado.
- Pide confirmación antes de `conn_delete`, `conn_update` y `conn_batch_delete` (este borra todas las del lote).
- No uses `execute_revit_code` para modificar o borrar lo que creó el add-in: perderías el registro que permite
  restaurar las barras. Usa `conn_update` y `conn_delete`.
- Una petición a la vez: Revit atiende las llamadas en serie.
- Las respuestas llegan completas en JSON (`{ok, data, errors, warnings, meta}`); lee siempre `errors[].hint`.
