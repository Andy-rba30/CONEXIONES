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
   `conn_get_node_info` (sin argumentos usa la selección; o pasa `element_ids`). Anota `chord_element_id`, `origin_mm`,
   los ejes y, por cada barra, `element_id`, `type` (el perfil real del modelo) y `angle_in_plane_deg`.
   Si `existing_connections` no está vacío, ese nudo ya tiene una conexión del add-in: pregunta si quiere
   actualizarla (`conn_update`) o borrarla (`conn_delete`) antes de crear otra.
3. `conn_get_schema` (`gusset_node`): copia `data.example` y adáptalo. No añadas campos que no estén en el esquema.
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
- `members[].end_setback_mm`: distancia desde el punto de trabajo hasta el extremo recortado de la barra.
- `bolted_knife_plate`: `plate.insertion_mm` es la parte de la placa cuchilla dentro de la ranura del HSS;
  `bolts.first_row_from_plate_end_mm` se mide desde el extremo libre de la placa (el que apoya en la cartela).
  La placa cuchilla apoya **sobre una cara** de la cartela (no en su mismo plano) y los pernos atraviesan cartela +
  placa: `plate.gusset_face` dice qué cara (`"+z"` por defecto, `"-z"` la opuesta, en el sistema local); si el plano no
  lo muestra, es una duda (`uncertain_fields`). La longitud del perno se calcula del agarre (suma de espesores) más el
  suplemento de tuerca y rosca de `limits.json`; si el plano la indica, ponla en `bolts.length_mm` (`conn_validate`
  devuelve en `data.bolt_stacks` el agarre y la longitud que se crearán).
- `gusset.chord_interface`: `through_slot` (la cartela atraviesa el cordón ranurado), `split_top_bottom` o `side_lap`.
  Si el plano no lo muestra, va a `uncertain_fields`.

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
| `BOLT_OUTSIDE_PLATE`, `PLATE_OUTSIDE_GUSSET`, `OUTLINE_INVALID` | Geometría imposible | Revisa medidas y contorno; pregunta si hace falta |
| `CLASH_WITH_FOREIGN_MEMBER` | La cartela choca con una barra que no es del nudo | Avisa al usuario; quizá falte una barra en la selección |
| `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE` | IDs que no son del nudo | Repite `conn_get_node_info` con la selección correcta |
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
- **Aplicar a un nudo**: pide al usuario que seleccione el cordón y todas las barras del nudo nuevo, enséñale las
  plantillas (`conn_catalog_list`) y llama a `conn_catalog_apply` con `template_id` (y `element_ids` si no usas la
  selección). El add-in casa cada barra con una ranura por su ángulo, probando también la plantilla **en espejo**
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

## 6. Lotes (Fase 8): planificar una cercha entera antes de crear nada

El **plan de lote** detecta todos los nudos de una cercha, casa cada uno con las plantillas del catálogo y valida nudo a
nudo. **No crea nada** (crear el lote llega en la Fase 9). Flujo: planificar → revisar con el usuario → corregir →
replanificar; al final, descartar las marcas si no se sigue.

- **Planificar**: pide al usuario que seleccione en Revit **todas** las barras de la cercha (cordones, diagonales y
  montantes; sin columnas ni correas) y llama a `conn_batch_plan` (con `template_ids` si quiere una plantilla concreta;
  sin ellos se prueban todas). El add-in lleva cada extremo al corte de su eje con el eje del cordón (las diagonales reales
  terminan en la cara del cordón, no en su eje; se admiten hasta medio canto de cada barra más 10 mm), agrupa esos puntos
  (10 mm), reconoce el cordón que atraviesa cada punto, calcula el marco canónico, nombra los nudos `N1, N2…` a lo largo
  de la cercha, salta los que ya tienen conexión, casa cada nudo
  (también en espejo) y valida con las mismas reglas de `conn_validate`: cada nudo `ready` trae su `validation_token`.
  Con `mark` (por defecto) colorea en la vista las barras de cada nudo **con el color de su estado** (verde = se creará,
  ámbar = se creará con aviso, rojo = falta algo, gris = no se crea; `color_name` lo dice) y pone un marcador con su nombre
  (cubo = misma orientación que la plantilla, rombo = en espejo); las barras sueltas y las parejas sin cordón no se marcan.
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
  falta otra plantilla (crear ese nudo a mano y `conn_catalog_save`) o excluirlos. No es un fallo de la detección. `members[].end_gap_mm` dice
  cuánto se queda corta cada barra respecto al punto de trabajo (0 = llega al eje).
- **Corregir**: las correcciones del usuario van en `overrides` de otra llamada a `conn_batch_plan` **con el mismo
  `plan_id`** (se acumulan y los nombres de nudo no cambian): `exclude` / `include`, `chord: {"N4": id}`,
  `template: {"N9": "<template_id>" | null}`, `remove_member` / `add_member: {"N2": [ids]}`, `add_node: {"N11": [ids]}`
  (un nudo que no se detectó, dado por sus barras), `merge: [["N5", "N6"]]`, `split: {"N5": [[ids], [ids]]}` y
  `spec: {"N4": {...}}` (una especificación editada a mano para ese nudo; vuelve a validarse). `conn_batch_plan_get` relee
  el plan (con `node` devuelve un nudo con su especificación completa); el `overrides` que devuelve se puede enviar tal
  cual en la petición siguiente.
- **Terminar**: `conn_batch_plan_discard` quita los colores y los marcadores y olvida el plan (con `all: true` limpia
  también marcas de planes olvidados). El usuario puede tener abierta la ventana del plan de la cinta mientras tú trabajas
  (desde la 0.8.4 no es modal): los planes son los mismos en memoria, en un documento solo hay un plan marcado, y si él
  pulsa **Descartar plan** en la ventana se quitan **todos** los marcadores del documento (también los de tu plan, que sigue
  en memoria sin marcas: replanifica con su `plan_id` para volver a verlo). Llámalo si el usuario no va a seguir o antes de que guarde el modelo. Mientras no
  exista `conn_batch_create`, un nudo del plan se crea igual que siempre: `conn_batch_plan_get` con `node` → `data.node.spec`
  y `data.node.validation_token` → `conn_preview` → confirmación → `conn_create`.

## 7. Reglas de seguridad

- Nunca llames a `conn_create` ni a `conn_update` sin `conn_preview` y la confirmación explícita del usuario (también
  cuando la especificación viene de `conn_catalog_apply`).
- Si `conn_create` no responde (tiempo de espera), **no la repitas a ciegas**: `conn_list` dice si quedó creada.
- Pide confirmación antes de `conn_delete` y de `conn_update`.
- No uses `execute_revit_code` para modificar o borrar lo que creó el add-in: perderías el registro que permite
  restaurar las barras. Usa `conn_update` y `conn_delete`.
- Una petición a la vez: Revit atiende las llamadas en serie.
- Las respuestas llegan completas en JSON (`{ok, data, errors, warnings, meta}`); lee siempre `errors[].hint`.
