# Guía para la IA (contenido de `conn_get_guide`)

Esta guía la devuelve `conn_get_guide` (Fase 4). Se puede editar sin recompilar; `scripts/deploy.ps1` la copia
junto al add-in. Corresponde a la sección 11 del encargo.

1. Llama a `conn_ping`. Si el add-in no está cargado, díselo al usuario y para.
2. Pide al usuario que seleccione en Revit los miembros del nudo y llama a `conn_get_node_info`.
3. Lee el detalle: rótulos de perfiles, placas, pernos y soldaduras. En planos de acero las cotas van en mm salvo
   que se indique otra cosa. "TIP." significa típico; el círculo en el símbolo de soldadura significa soldadura en
   todo el contorno.
4. Los ángulos y las posiciones salen del modelo, no del dibujo.
5. Transcribe cada cadena de cotas completa en `dimension_chains`, con el total que debería dar.
6. Nunca inventes un dato ilegible o ausente: va en `uncertain_fields` con el motivo.
7. Llama a `conn_validate`. Corrige lo que sea error tuyo; si el error depende del plano, pregunta al usuario.
8. Muestra al usuario un resumen corto (tabla con lo leído y las dudas) y espera su confirmación explícita.
9. Llama a `conn_preview`, luego a `conn_create` con el token, e informa el `connection_id`. Si quieres enseñar el
   resultado, usa `get_revit_view`.
