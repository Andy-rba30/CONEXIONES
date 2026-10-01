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

- X = eje del cordón (del inicio al fin de su línea), Z = normal al plano de la cercha, Y = Z × X. Origen = punto de
  trabajo (cruce del eje del cordón con el del primer miembro de `members`). `conn_get_node_info` lo devuelve.
- `gusset.outline.points_mm`: vértices del contorno de la cartela en ese sistema, en mm, en orden (contorno cerrado
  implícito, sin cruces). Las cotas del plano se miden desde los bordes de la cartela: sitúala respecto al punto de
  trabajo con las cotas que lo relacionan con los bordes; si el plano no lo deja claro, es una duda.
- `members[].end_setback_mm`: distancia desde el punto de trabajo hasta el extremo recortado de la barra.
- `bolted_knife_plate`: `plate.insertion_mm` es la parte de la placa cuchilla dentro de la ranura del HSS;
  `bolts.first_row_from_plate_end_mm` se mide desde el extremo libre de la placa (el que apoya en la cartela).
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
| `BOLT_OUTSIDE_PLATE`, `PLATE_OUTSIDE_GUSSET`, `OUTLINE_INVALID` | Geometría imposible | Revisa medidas y contorno; pregunta si hace falta |
| `CLASH_WITH_FOREIGN_MEMBER` | La cartela choca con una barra que no es del nudo | Avisa al usuario; quizá falte una barra en la selección |
| `ELEMENT_NOT_FOUND`, `ELEMENT_NOT_A_MEMBER`, `MEMBER_NOT_AT_NODE` | IDs que no son del nudo | Repite `conn_get_node_info` con la selección correcta |
| `NODE_AXES_NOT_INTERSECTING` | Los ejes de cordón y primer miembro no se cruzan (> 5 mm) | El modelo tiene las barras desplazadas: díselo al usuario |
| `VALIDATION_TOKEN_INVALID` (en `conn_create`/`conn_update`) | Token ausente, o la especificación o el modelo cambiaron | Vuelve a `conn_validate` con la especificación final |
| `REVIT_BUSY` | Revit tiene una orden o un diálogo abierto | Pide al usuario que lo cierre y repite |
| `NO_DOCUMENT` | No hay modelo abierto | Pide al usuario que abra el modelo |

## 5. Reglas de seguridad

- Nunca llames a `conn_create` ni a `conn_update` sin `conn_preview` y la confirmación explícita del usuario.
- Si `conn_create` no responde (tiempo de espera), **no la repitas a ciegas**: `conn_list` dice si quedó creada.
- Pide confirmación antes de `conn_delete` y de `conn_update`.
- No uses `execute_revit_code` para modificar o borrar lo que creó el add-in: perderías el registro que permite
  restaurar las barras. Usa `conn_update` y `conn_delete`.
- Una petición a la vez: Revit atiende las llamadas en serie.
- Las respuestas llegan completas en JSON (`{ok, data, errors, warnings, meta}`); lee siempre `errors[].hint`.
