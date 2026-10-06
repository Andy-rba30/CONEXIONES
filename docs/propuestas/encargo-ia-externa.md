# Propuesta: encargo para una IA externa (diseñar el JSON en el navegador, aplicarlo desde la cinta)

Fecha: 2026-10-06. Estado: **programada como ronda 9b dentro de la Fase 10** (2026-10-06, add-in 0.10.0; `docs/fases/fase-10.md`,
sección 1; **NO PROBADA en Revit**: `docs/instalacion/fase-10.md`, paso 10-8). Decisiones de la persona: P1 = un solo archivo
`.md` copiado al portapapeles; P2 = sin herramienta MCP por ahora; P3 = con un solo tipo de conexión no se pregunta el tipo.
Lo programado: `Core/Brief/DesignBriefWriter.cs` (el Markdown, probado en la nube; el Detalle D confirmado embebido en el
Core como ejemplo cuando el catálogo no tiene una plantilla con la misma cantidad de barras) y el botón **Encargo para IA**
de la cinta (`DesignBriefCommand.cs`: `node_info` por `Bridge.Handle`, archivo en `%LOCALAPPDATA%\MotorConexiones\encargos\`,
portapapeles, carpeta abierta, cuadro corto). La sección 8 (placa de extremo) sigue en `docs/propuestas/placa-de-extremo.md`.
Texto original de la propuesta a continuación. Sale de la conversación del cierre de la Fase 8: la persona
diseñará casi siempre el JSON con una IA externa **desde el navegador** (Claude, sin aplicaciones de escritorio ni API),
porque su agente local conectado al MCP no es capaz de leer un plano y escribir la especificación. La previsualización y la
confirmación siguen siendo de la persona.

## 1. El problema

Hoy, para que una IA del navegador escriba el JSON de un nudo hace falta darle a mano cinco cosas: la imagen del detalle, la
respuesta de `conn_get_node_info` (IDs reales de las barras, perfiles del modelo, ángulos, origen), el `data.example` de
`conn_get_schema`, las secciones 2, 3 y 4 de `docs/guide.md` y un ejemplo ya creado (`docs/fixtures/detalle-D-confirmado.json`
con `detalle-D.png`). Las cuatro últimas solo se consiguen con el agente local o abriendo archivos del repositorio. Aplicar el
JSON, en cambio, **ya no necesita ninguna IA**: el botón **Ejecutar especificación JSON** de la cinta lo abre, lo dibuja con
cotas, lo valida y lo crea cuando la persona acepta (Fase 6).

## 2. Lo que se propone: un botón **Encargo para IA** en la cinta

Con el nudo seleccionado en Revit (el cordón o apoyo y las barras que llegan), el botón:

1. Calcula lo mismo que `conn_get_node_info` sobre la selección (sin ventana salvo las de error de siempre).
2. Escribe `%LOCALAPPDATA%\MotorConexiones\encargos\encargo-<documento>-<fecha-hora>.md` con, en este orden:
   - **El prompt** para la IA externa (sección 3), ya redactado.
   - **Los datos del nudo**: el JSON de `node_info` tal cual (IDs, perfiles reales, `angle_in_plane_deg`, `origin_mm`,
     ejes, `existing_connections`).
   - **El esquema**: el `data.example` del tipo (`gusset_node`; cuando haya más tipos, el que elija la persona en un
     desplegable pequeño o el único disponible).
   - **Las reglas de lectura**: las secciones 2, 3 y 4 de `docs/guide.md` copiadas del archivo desplegado (editable sin
     recompilar, como hoy).
   - **Un ejemplo confirmado**: el JSON de la plantilla más parecida del catálogo (misma cantidad de barras) o, si no hay
     catálogo, `detalle-D-confirmado.json` embebido como recurso.
3. Copia el contenido al portapapeles y abre la carpeta en el Explorador. En la barra de estado de Revit (o un cuadro corto,
   es un botón de la cinta): "Encargo copiado al portapapeles: pégalo en la IA junto con la imagen del detalle".

La imagen del detalle **no** la pone el add-in: es un recorte del plano que hace la persona. El encargo le dice dónde
adjuntarla.

## 3. El prompt que va dentro del encargo

```text
Eres el diseñador de una conexión de acero para un add-in de Revit. Tu única salida es un JSON que cumple el esquema
adjunto (sección "Esquema"); no añadas campos que no estén en él. Lee el detalle que te adjunto como imagen siguiendo las
reglas de la sección "Cómo leer un detalle". Usa SOLO los element_id, perfiles y ángulos de la sección "Datos del nudo":
los ángulos y las posiciones salen del modelo, no del dibujo; si el plano trae un ángulo, ponlo en expected_angle_deg. El
cordón es chord_element_id. Transcribe cada cadena de cotas completa en dimension_chains con su total. Lo que no se lea
bien va en uncertain_fields con path, reason y user_confirmed_value: null, y el campo apuntado queda null. Espesores y
diámetros en pulgadas van en mm en el campo *_mm y con el texto del plano tal cual en *_label. La sección "Ejemplo
confirmado" es un JSON que ya se creó bien en Revit: imítalo en forma, no en valores. Antes del JSON dame una tabla
corta: cordón, cartela (espesor y contorno), cada barra con su unión, pernos, soldaduras, retiros y tus dudas. Después, el
JSON completo en un solo bloque de código, listo para guardar como archivo .json. Si más adelante te pego errores del
validador, corrige solo lo que digan y devuélveme el JSON entero otra vez.
```

## 4. El flujo completo para la persona (dos pegados por típica, sin agente local)

1. Selecciona el nudo en Revit, pulsa **Encargo para IA**.
2. En el navegador: pega el encargo, sube la imagen del detalle, recibe la tabla y el JSON. Lo guarda como `.json`.
3. **Ejecutar especificación JSON** > ese archivo: previsualización con cotas, validación, corrección de una cota con doble
   clic si hace falta, **Crear**. Si el validador rechaza algo, copia el error literal (la ventana ya lo enseña) y vuelve
   al paso 2.
4. **Guardar en catálogo** desde la previsualización. Con la Fase 9, la cercha entera sale de **Planificar lote** y **Crear**.

El agente local no interviene. La IA externa nunca toca Revit. La confirmación sigue siendo la previsualización.

## 5. Lo que NO cambia

- Ninguna ventana nueva en las rutas de la IA; el botón es de la cinta (único sitio con diálogos permitido).
- El contrato, el validador, el token y el catálogo quedan igual. El encargo es solo texto.
- `docs/guide.md` sigue siendo la única fuente de las reglas de lectura: el botón la copia, no la duplica.

## 6. Dónde encaja

Como **ronda 9b** (una sesión corta después de la Fase 9: un comando de la cinta, un escritor de Markdown en el Core con su
prueba, un recurso embebido) o como el primer trozo de la **Fase 11** (conectar un nudo sin JSON), que ya iba a leer el
nudo seleccionado y proponer la plantilla. Recomendación: 9b, porque la persona lo va a usar desde la primera típica de la
Fase 9 y es media sesión.

## 7. Preguntas para la persona

- **P1.** ¿El encargo en un solo archivo `.md` (pegar todo) o un `.md` más el `node_info.json` aparte? Recomendación: uno solo.
- **P2.** ¿Quieres también una herramienta MCP `conn_design_brief` que devuelva el mismo texto, para cuando sí uses el agente
  local? Cuesta poco (el Core lo escribe; la ruta solo lo devuelve).
- **P3.** Cuando haya más de un tipo de conexión (sección 8), ¿el botón pregunta el tipo o escribe un encargo por tipo?

## 8. Nota aparte: las conexiones de placa de extremo de los detalles G, H e I

En el plano de la cercha contra el eje B (detalles G, H, I y H1) las diagonales `HSS2-1/2X2-1/2X3/16` terminan en una
**placa `PL3/8" x 185 x 185 mm` con pernos `Ø5/8"` en agujeros `Ø18`**, atornillada al apoyo: una placa de extremo, no un
nudo con cartela. **La versión 1 no lo cubre**: el único tipo es `gusset_node` (cordón + cartela + placas cuchilla + pernos
entre cartela y cuchilla). Hacerlo es un tipo nuevo según el README, sección 7 (`IConnectionType`, contrato, validador,
esquema y ejemplo, generador con placas y pernos en Advance Steel, pruebas, guía y catálogo). Antes de proponerlo como fase
hace falta aclarar con el plano completo: si hay una placa o dos (una en la barra y otra en el apoyo), si los pernos
atraviesan el apoyo o van a una placa soldada a él, el patrón de pernos (cuántos, paso, borde) y qué elemento de Revit es
el apoyo (columna, viga o placa embebida). Con eso se escribe `docs/propuestas/placa-de-extremo.md`.
