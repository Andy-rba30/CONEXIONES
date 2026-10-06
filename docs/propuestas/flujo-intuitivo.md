# Propuesta: que la parte de las cerchas (plan de lote) se entienda

Fecha: 2026-10-05. Estado: **ronda 8c hecha en el repositorio** (2026-10-05, add-in 0.8.3, informe en
`docs/fases/fase-8.md`, sección 9): C1, C2, C3 (solo el texto), C4, C5, C8, C9 y V1 están programados y probados en la
nube, y el sondeo 19 para V3 está escrito; **falta probarlo en el PC** con `docs/instalacion/fase-8c.md`. Lo demás (C3
botones, V2 en la Fase 9; C6 y V3 en la 10; botón *Conectar*, V4, V5 en la 11; V6 en la 12) sigue aquí como propuesta.
**Cierre de la 8c (2026-10-05, 0.8.4)**: la 8c se probó en el PC y C7 (ventana que se queda abierta) se adelantó al cierre;
se probó con `docs/instalacion/fase-8d.md`. **Cierre de la 8d (2026-10-06, 0.8.5, `fase-8.md` sección 11)**: C7 funcionó en el
PC (orbitar y pinchar con la ventana abierta, Ver en Revit, Descartar a cero, catálogo vacío), pero **Cordón… cerró Revit**
(diálogo de elección) y Descartar no cerraba la ventana: corregidos, con pinchar en Revit con la ventana oculta y una red que
captura cualquier error de la ventana. La etiqueta del sondeo 19 (V3) no se vio: v3 se repite. Se prueba con
`docs/instalacion/fase-8e.md`. **Cierre de la 8e y de la Fase 8 (2026-10-06, `fase-8.md` sección 12; el add-in sigue en
0.8.5)**: la 8e lo confirmó en el PC (Cordón… y Barras… con la ventana oculta y de vuelta, Esc cancela, Excluir / Incluir y
Replanificar con la ventana abierta, Descartar cierra la ventana, `--puente` 28/28) y **la etiqueta del sondeo 19 se vio y se
pinchó**: **V3 queda decidida que sí para la Fase 10**. El cierre deja el sondeo 19 en v4 (la etiqueta B corregida y el clic
sin cuadro, porque el cuadro modal dejaba a Revit sin atender a pyRevit). **Fase 9 (2026-10-06, add-in 0.9.0, NO PROBADA
en Revit; `docs/fases/fase-9.md`)**: el botón **Crear N conexiones**, el informe por nudo, **Borrar el lote** y **C3 con
botones** están programados; **V2 (cartelas fantasma) no entró** (no cabía sin recortar lo anterior) y sigue aquí como
propuesta, para la Fase 10 o la 11. Sigue aquí como propuesta: V2; C6 y V3 en la 10; botón *Conectar*, V4 y V5 en la 11;
V6 en la 12.

Qué quedó hecho de cada mejora (ronda 8c):

| Mejora | Estado tras la 8c |
|---|---|
| C1 solo nudos de verdad | Hecho: la tabla y el mapa enseñan los nudos con cordón; barras sueltas y parejas sin cordón van ocultas con contador y **Mostrar ocultos** (los nombres `N1…N59` se mantienen, decisión P5) |
| C2 estados en español y colores por estado | Hecho en la ventana, en el mapa, en el modelo (`PlanMarks`) y en la respuesta (`status_text`, `color_name`) |
| C3 columna "Qué hacer" | Hecho el **texto** (`advice`, calculado en el Core) en la 8c y **los botones en la Fase 9** (`PlanAdvice.Actions`, clave `actions` por nudo en la respuesta; en la ventana, uno o dos botones debajo de la frase: Excluir, Incluir, Incluir (rehacer), Cordón…, Barras…, Plantilla…, Editar nudo, Ver en Revit, Abrir catálogo). Con el consejo del empalme (`fase-8.md` 12.1): "El cordón termina en este nudo (empalme): ninguna plantilla encaja con 2 diagonales; crea esa típica o excluye". NO PROBADO en Revit hasta la instalación de la Fase 9 |
| C4 cabecera con la decisión | Hecho (`summary_text`) |
| C5 sin tokens ni IDs, menos botones | Hecho: 4 botones + **Más…** + menú de clic derecho; token, IDs y `end_gap_mm` solo en el detalle del nudo |
| C6 selección asistida | Pendiente (Fase 10) |
| C7 ventana que se queda abierta | **Hecho en el cierre de la 8c** (2026-10-05, add-in 0.8.4, `ExternalEvent`; `fase-8.md` sección 10) y **probado en la 8d** (orbitar, pinchar barras, Ver en Revit, Replanificar, Descartar y catálogo vacío con la ventana abierta). El cierre de la 8d (0.8.5, sección 11) corrige lo que la 8d rompió: Cordón… cerraba Revit (diálogo de elección) y Descartar no cerraba la ventana; pinchar en Revit se hace con la ventana oculta y con dos líneas en el log. **Probado en la ronda 8e** (2026-10-05, `resultados-fase-8e.md`, `fase-8.md` 12.1): Cordón… sobre N9 seis veces y Barras… dos veces con la ventana oculta y de vuelta, Esc cancela, Excluir / Incluir y Replanificar con la ventana abierta, Descartar cierra la ventana; Revit siguió vivo y el log no tiene ningún error de ventana. Sin probar a propósito todavía: Plantilla… cancelado, Editar nudo y Planificar lote con la ventana abierta (se miran de paso en la Fase 9) |
| C8 guía de la IA | Hecho (`docs/guide.md`, sección 6: `summary_text` y tabla corta, nunca el JSON) |
| C9 primera vez guiada (catálogo vacío) | Hecho: aviso `CATALOG_EMPTY` y botón **Abrir catálogo** |
| V1 mapa de la cercha | Hecho (`TrussMap` en el Core, `TrussMapCanvas` en la ventana) |
| V2 cartelas fantasma | **No entró en la Fase 9** (el prompt lo permitía solo si cabía sin recortar lo anterior): pendiente, para la Fase 10 o la 11. Lo que haría: un `DirectShape` transparente con el contorno de la cartela de cada nudo listo (y la placa cuchilla), en el marco del nudo, coloreado por estado; Crear lo sustituye por acero y Descartar lo quita. Hoy, al crear, las marcas de los nudos creados desaparecen y el acero queda en su sitio |
| V3 etiquetas pinchables | Sondeo 19 v2 en la 8d: Revit aceptó el BMP, `AddControl` dio el índice 0 y el manejador de clics (`UI.ITemporaryGraphicsHandler` en `TemporaryGraphicsHandlerService`) se registró, pero **la etiqueta no se vio**. Sondeo 19 **v3** (cierre de la 8d): BMP de 24 bits de 32×32 en una ruta sin tildes, posición en pies, `SetVisibility`, refresco de la vista y una segunda etiqueta en el centro de la caja de sección. **En la 8e la etiqueta A (la 4 verde en N4) se vio y se pinchó** (capturas `fase8e-01-etiqueta` y `fase8e-01-etiqueta-clic`; cuatro clics anotados): **V3 es posible y va a la Fase 10**. El cierre de la 8e deja el sondeo 19 en **v4** (`fase-8.md` 12.3): la etiqueta B, que no se puso por un `ElementId` ambiguo en IronPython, y el clic **sin cuadro** (el `TaskDialog` del manejador era modal y dejó a Revit sin atender a pyRevit durante un cuarto de hora; ahora la etiqueta pinchada pasa a naranja con `UpdateControl`). Se ejecuta en la instalación de la Fase 9 o de la 10 |
| V4, V5, V6, V7 | Pendientes (Fases 11 y 12; V7 no se recomienda) |

Pregunta de origen: "funciona bien, pero la parte de las cerchas, lo que se comprobó en la ronda 8b, es lo que veo
difícil de entender. ¿Se puede hacer más intuitivo?".

## 1. Qué viste en la ronda 8b, en palabras sencillas

Seleccionaste la cercha (56 barras: el cordón central entero y las diagonales, **sin** los cordones superior e inferior)
y el add-in devolvió **59 nudos**. Esto es lo que significa cada cosa:

| Palabra que viste | Qué es de verdad | En tu cercha |
|---|---|---|
| **Nudo** | Un punto del eje de un cordón donde terminan varias barras. | 59 "puntos", pero solo 16 son nudos de verdad |
| **Cordón** | La barra que pasa de largo por el nudo (no termina ahí). Sobre ella va la cartela. | El cordón central, en 8 tramos |
| `ready` (**listo**) | Nudo con cordón y tres barras que encajan con la plantilla *Detalle D*. Su conexión ya está preparada y validada. | **16** |
| `no_match` (**sin plantilla que encaje**) | Dos barras terminan en el mismo punto pero **ninguna pasa de largo**: falta el cordón en la selección. | **20**: los nudos de arriba y de abajo, porque no seleccionaste esos cordones |
| `untyped` (**barra suelta**) | Un extremo de barra solo, sin otra barra cerca. No es un nudo. | **23**: extremos de diagonales sobre los cordones que no estaban seleccionados, y un empalme |
| `same` / `mirror_x` (**igual / en espejo**) | Si el nudo es como el Detalle D o como su reflejo (el otro lado de la cercha). | 8 iguales, 8 en espejo |
| **Desvío** | Grados que las barras se apartan de los ángulos de la plantilla. Menos de 2° es perfecto. | 0° a 1,4° |
| `end_gap_mm` | Cuánto se queda corta cada barra respecto al eje del cordón (porque termina en su cara). Dato interno. | 20 a 86 mm |
| **Token** | Un sello que dice "esta especificación está validada". No lo necesitas ver. | 16 sellos |
| **Plan** | La lista de nudos preparados. **No crea nada**; crear es la Fase 9. | Plan `4ef7dd3d…` |
| **Marcas** | Colores y cubos (igual) o rombos (espejo) en la vista para ver qué nudo es cuál. Se quitan al descartar. | 36 marcadores |
| **Sondeos 17 y 18** | Pruebas internas del instalador. No forman parte del uso normal. | — |

Resumen en una frase: **de los 59, solo importan los 16 verdes; los otros 43 son ruido** que sale de no haber seleccionado
los cordones superior e inferior.

## 2. Por qué cuesta entenderlo

| # | Qué pasa hoy | Dónde |
|---|---|---|
| 1 | **Enseña 59 nudos cuando hay 16.** Las barras sueltas y las parejas sin cordón van en la misma tabla que los nudos de verdad, numerados todos seguidos (el Detalle D es "N4"). | Tabla del plan |
| 2 | **Palabras internas**: `ready`, `no_match`, `untyped`, `mirror_x`, `end_gap_mm`, `NODE_CHORD_NOT_CONTINUOUS`, `plan_id`, token de 64 letras, IDs de barras. | Tabla, avisos y respuesta del MCP |
| 3 | **No dice qué hacer.** "no_match" no explica que falta seleccionar el cordón superior; "untyped" no dice que no pasa nada. | Columnas *Errores* y *Avisos* |
| 4 | **Hay que seleccionar bien la cercha** (todos los cordones) y nadie te avisa de que faltan. | Antes de pulsar el botón |
| 5 | **12 botones** en la ventana y no se sabe cuál toca ahora. | Ventana *plan de lote* |
| 6 | **Un color por nudo** (12 colores que se repiten) en vez de un color por estado. El nombre solo se ve en Propiedades > Comentarios. | Marcas en el modelo |
| 7 | **La ventana se cierra para orbitar** (decisión P10, opción A). | Ventana del plan |
| 8 | **El plan no crea nada** y no se ve el final del camino: falta el botón *Crear 16 conexiones* (Fase 9). **Hecho en la Fase 9**: botón **Crear N conexiones** e informe en la misma ventana. | — |
| 9 | **Desde el chat**, la IA recibe un JSON enorme y lo vuelca en vez de resumirlo. | `conn_batch_plan` |

Todo esto es **presentación**. La detección, el casado con la plantilla, la validación y las herramientas `conn_*` no
tienen que cambiar: funcionan (16 nudos reproducidos uno a uno en la nube).

## 3. Lo que NO cambia

- El contrato, el esquema, el catálogo, el detector de nudos y el plan en memoria.
- Las 21 herramientas `conn_*` y el `validation_token` (la IA sigue sin ventanas).
- Una operación = un `TransactionGroup`; error = rollback completo.
- El croquis 2D con cotas y *Editar nudo* (eso ya se entiende).

## 4. Cómo sería: "Conectar cercha" en tres pasos

```
Paso 1  Pincha una barra de la cercha (o selecciónala entera) y pulsa  [ Conectar cercha ]
        El add-in añade las barras que la tocan y, si faltan cordones, lo dice:
        "Faltan los cordones superior e inferior (20 nudos sin cordón). [Añadirlos] [Seguir así]"

Paso 2  Una ventana que se queda abierta mientras orbitas:

        ┌──────────────────────────────────────────────────────────────────────────────┐
        │ Conectar cercha · Plantilla: Detalle D [cambiar]   ☑ también en espejo        │
        │ ● 16 listos   ▲ 14 con aviso   ✖ 0 con problema   · 20 sin cordón · 23 sueltos│
        │                                                       [mostrar los ocultos]   │
        ├──────────────────────────────────────────────────────────────────────────────┤
        │ Nudo │ Estado          │ Espejo │ Qué hacer                                    │
        │  1   │ ● Listo         │  no    │ —                                            │
        │  2   │ ● Listo         │  sí    │ —                                            │
        │  3   │ ▲ Perfil HSS4X4 │  no    │ La plantilla es HSS3X3. [Usar igual] [Excluir]│
        │  9   │ ✖ Falta cordón  │  —     │ Selecciona el cordón superior. [Elegir cordón]│
        ├──────────────────────────────────────────────────────────────────────────────┤
        │ Clic derecho en un nudo: Ver en Revit · Editar · Excluir · Cordón · Barras     │
        │ Verde = se creará · Ámbar = se creará con aviso · Rojo = falta algo · Gris = no es nudo │
        │                                                      [ Crear 16 conexiones ]  │
        └──────────────────────────────────────────────────────────────────────────────┘

Paso 3  [ Crear 16 conexiones ] (Fase 9). Informe en la misma ventana: 16 creadas, 0 fallidas.
        Una sola entrada de deshacer. [Borrar el lote] si no te gusta.
```

En el modelo, los colores pasan a ser **por estado** (verde, ámbar, rojo, gris), no por nudo: miras la cercha y ves de un
vistazo qué se va a crear. El cubo y el rombo (igual / espejo) se quedan.

## 5. Mejoras concretas, de más a menos impacto

| # | Mejora | Qué cambia para ti | Esfuerzo | Qué se reutiliza |
|---|---|---|---|---|
| C1 | **Solo nudos de verdad en la tabla**; barras sueltas y parejas sin cordón ocultas, con un contador en la cabecera y *mostrar los ocultos*. Numeración 1…16 solo para los nudos reales. | Ves 16, no 59. | Bajo | El detector ya los distingue |
| C2 | **Estados en español y de tres colores**: Listo, Con aviso, Falta algo, No es nudo. Lo mismo en las marcas del modelo (color por estado). | Entiendes la cercha de un vistazo. | Bajo | Textos y la paleta de marcas |
| C3 | **Columna "Qué hacer"** con una frase y un botón por caso: falta cordón → *Elegir cordón*; perfil distinto → *Usar igual* / *Excluir*; se sale de la cartela → *Editar*. | Sabes el siguiente paso sin leer el README. | Medio | Los avisos de hoy, convertidos en acciones |
| C4 | **Cabecera con la decisión**: "Se crearán 16 conexiones con Detalle D, 8 en espejo; 14 avisan de perfil". | Ves el final del camino. | Bajo | El `summary` del plan |
| C5 | **Sin tokens, IDs ni JSON a la vista**; 12 botones → menú de clic derecho + 3 botones (Replanificar, Crear, Cerrar). *Guardar plan JSON* y *Descartar* a un menú *Más…*. | Menos ruido. | Bajo | `BatchPlanWindow` |
| C6 | **Selección asistida**: pinchas una barra y el add-in añade las que la tocan; si faltan cordones lo avisa antes de planificar. | Se acaba "me faltó el cordón superior". | Medio | El mismo alcance de cara del detector |
| C7 | **Ventana que se queda abierta** (opción B de P10, `ExternalEvent`): orbitas y pinchas sin cerrarla. | Se acaba cerrar y abrir. | Medio-alto | La lógica del plan no cambia |
| C8 | **Para el chat**: la guía dice a la IA que resuma el plan en español en cuatro líneas (listos, con aviso, sin cordón, sueltos) y nunca vuelque el JSON. | Conversación corta. | Bajo | `docs/guide.md`, sección 6 |
| C9 | **Primera vez guiada**: sin plantillas en el catálogo, el botón explica "crea primero un nudo y guárdalo como plantilla" y ofrece abrirlo. | No te quedas con 0 listos sin saber por qué. | Bajo | Catálogo |

**Y para un nudo suelto** (fuera de la cercha), lo mismo en pequeño, para más adelante: botón *Conectar* que reconoce el
nudo y propone la plantilla sin pedir un archivo JSON; panel por secciones (Cartela, Cordón, Barra 1-2-3) con desplegables
de espesor y perno que ponen los mm solos; JSON, contorno punto a punto y cadenas de cotas en una pestaña *Avanzado*.

## 6. Y más visual: lo que se puede dibujar (no solo tablas)

Hoy todo son tablas porque las fases se construyeron alrededor del JSON y de la IA. **No es la única manera.** Un add-in
de Revit puede dibujar lo que quiera en sus ventanas (WPF) y, dentro del modelo, puede colorear, poner elementos
"fantasma" transparentes y marcadores con imagen. Lo que no puede es pintar encima de la vista 3D como en un videojuego;
por eso lo visual se reparte entre la ventana y el modelo.

| # | Opción visual | Cómo se ve | Esfuerzo | Qué se reutiliza |
|---|---|---|---|---|
| V1 | **Mapa de la cercha en la ventana**: el alzado de la cercha dibujado en la ventana, con un círculo por nudo (verde, ámbar, rojo, gris) y su número. Clic en un círculo → zoom en Revit y el croquis con cotas de ese nudo al lado. Pasar el ratón → "Listo · Detalle D · en espejo". | Ves la cercha entera y sus nudos de un vistazo, como en el plano. | Medio | Los ejes y puntos de trabajo del plan y el mismo lienzo del croquis 2D |
| V2 | **Cartelas fantasma antes de crear**: en cada nudo listo se dibuja la cartela (y la placa cuchilla) transparente, coloreada por estado, en el sitio exacto. *Crear* cambia los fantasmas por acero de verdad; *Descartar* los quita. | Ves en 3D lo que se va a crear antes de crearlo. | Medio | El contorno de la plantilla y el marco del nudo; los `DirectShape` de las marcas de hoy |
| V3 | **Etiquetas con número en la vista** que se pueden pinchar: un rótulo con el número y el color pegado a cada nudo en el 3D; clic en el rótulo → ese nudo se selecciona en la ventana. Revit lo permite desde la versión 2023 (controles con imagen en el lienzo). | Como las chinchetas de un mapa. | Medio, **NO PROBADO**: hace falta un sondeo en Revit 2027 | Si no funcionara, se queda el marcador de hoy |
| V4 | **Catálogo con miniaturas**: cada plantilla con su croquis en pequeño; eliges por la imagen, no por el nombre. | Reconoces la típica de un vistazo. | Bajo-medio | El croquis 2D pasado a imagen |
| V5 | **El plano al lado del croquis**: abres la imagen del detalle (PNG o página de PDF) junto al croquis con cotas y comparas uno con otro. | Comparas plano y modelo sin cambiar de ventana. | Bajo | Solo la ventana |
| V6 | **Edición arrastrando** en el croquis: esquinas de la cartela, grupo de pernos, largo de ranura; las cotas se actualizan y se revalida. | Diseñas con el ratón. | Medio-alto | El doble clic sobre cotas de hoy |
| V7 | **Vista 3D dentro de la ventana** (visor propio). | Un 3D pequeño en la ventana. | Alto | Nada; **no lo recomiendo**: Revit ya es el 3D, mejor V2 con la ventana abierta (C7) |

**Cómo quedaría la ventana con V1 + V2:**

```
┌────────────────────────────────────────────────────────────────────────────────┐
│ Conectar cercha · Plantilla: Detalle D [cambiar]   ● 16 listos ▲ 14 avisos     │
├──────────────────────────────────────────┬─────────────────────────────────────┤
│  MAPA DE LA CERCHA (clic en un nudo)     │  NUDO 4 · Listo · Detalle D         │
│                                          │                                     │
│   ●────●────●────●────●────●────●        │     croquis 2D con cotas            │
│    \  / \  / \  / \  / \  / \  /         │     (el de hoy)                     │
│     ●4   ●7   ●12  ●14  ●19  ●21         │                                     │
│    /  \ /  \ /  \ /  \ /  \ /  \         │  Qué hacer: —                       │
│   ○────○────○────○────○────○────○        │  [Ver en Revit] [Editar] [Excluir]  │
│  ● listo  ▲ aviso  ✖ falta algo  ○ oculto│                                     │
├──────────────────────────────────────────┴─────────────────────────────────────┤
│ En el modelo: cartelas fantasma en verde / ámbar; se crean al pulsar           │
│                                                     [ Crear 16 conexiones ]    │
└────────────────────────────────────────────────────────────────────────────────┘
```

Si solo se hicieran dos: **V1 (mapa) y V2 (cartelas fantasma)**. Son las que convierten "una tabla de 59 filas" en
"una cercha con 16 puntos verdes y sus cartelas dibujadas". V4 y V5 son baratas y se pueden colar en cualquier fase.

## 7. Dónde encaja en el plan de fases (recomendación)

Como lo difícil es esta ventana, y la Fase 9 va a poner en ella el botón *Crear*, conviene **entenderla antes de crear**:

| Fase | Entrega | Se prueba en la nube | Se prueba en el PC |
|---|---|---|---|
| **8c. Ventana del plan entendible** | C1, C2, C4, C5, C8, C9 y **V1 (mapa de la cercha)**. Presentación: textos, filtros, cabecera, colores por estado, guía y el mapa. Si el mapa no cabe en la sesión, pasa a una 8d. | Pruebas del Core del resumen y del mapa (coordenadas del alzado); `probar_conexiones.py`; capturas | Misma cercha: ver 16 nudos en el mapa, colores por estado, clic en un nudo |
| **9. Crear por lotes** (como está previsto) + C3 + **V2 (cartelas fantasma)** | `conn_batch_create`, botón *Crear N conexiones*, informe por nudo, *Borrar el lote*; columna *Qué hacer*; cartelas fantasma que se vuelven acero al crear. Con la pregunta P6 del cierre de la 8 resuelta. Sondeo para V3 (etiquetas pinchables). | Simulador; geometría de los fantasmas | Crear el lote en la cercha, deshacer, borrar; ver los fantasmas; resultado del sondeo V3 |
| **10. Cercha sin dolor** | C6 (selección asistida), C7 (ventana abierta: **hecha en el cierre de la 8c y probada en la 8d y la 8e**) y **V3 (el sondeo 19 dijo que sí en la 8e)**: etiquetas con el número en la vista, pinchables, con un manejador que nunca abre un cuadro (cambia la etiqueta, selecciona el nudo y escribe en la barra de estado). | Lógica de selección | Pinchar una barra, orbitar con la ventana abierta, pinchar una etiqueta |
| **11. Conectar un nudo** | Botón *Conectar* y panel por secciones para un nudo suelto; V4 (miniaturas) y V5 (plano al lado). | Generador de JSON desde el panel | Un nudo de principio a fin sin tocar JSON |
| **12. Cartela automática** (era la 10 opcional) | `outline.mode = "auto"`. | Pruebas de contorno | Nudos con otros ángulos |

Alternativa: meter la 8c dentro de la Fase 9 (una sesión más larga). Lo decides en P3.

## 8. Preguntas para ti (responde en el chat; con eso se escribe el prompt de la 8c)

- **P1.** De la tabla de la sección 2, ¿cuáles tres te molestan más? Basta con los números ("1, 3 y 6").
- **P2.** ¿Te vale ocultar por defecto las barras sueltas y las parejas sin cordón (con un contador y *mostrar los ocultos*),
  o prefieres verlas siempre en gris al final de la tabla?
- **P3.** ¿Hacemos la ronda 8c (ventana entendible) **antes** de la Fase 9, o todo junto en la Fase 9?
- **P4.** Colores en el modelo: ¿por estado (verde / ámbar / rojo / gris) como propongo, o quieres conservar un color por
  nudo para distinguirlos entre sí?
- **P5.** Numeración: ¿1…16 solo para los nudos de verdad (cambia al replanificar si aparece uno nuevo), o se mantiene
  N1…N59 fija como hoy?
- **P6.** ¿Un solo botón *Conectar* que sirva para un nudo y para la cercha (según lo que selecciones), o dos botones?
- **P8.** De las opciones visuales de la sección 6, ¿cuáles quieres? Recomiendo V1 y V2; dime si alguna no te interesa.
- **P9.** ¿Tienes los detalles como imagen (PNG, JPG o PDF)? Es lo que necesita V5 (el plano al lado del croquis).
- **P10.** Las cartelas fantasma (V2): ¿solo en los nudos listos, o también en los que tienen aviso (en ámbar)?
- **P7.** Palabras: ¿"cartela" o "plancha de nudo", "retiro" o "corte", "placa cuchilla" o "cuchilla", "en espejo" o
  "lado derecho"? Se usarán tus palabras.

## 9. Riesgos

- **Ocultar nudos** puede esconder un error real (por ejemplo, un nudo de verdad que salió como pareja sin cordón por un
  cordón mal dibujado). Por eso la cabecera siempre muestra el contador y *mostrar los ocultos*.
- **La ventana no modal** (C7) cambia cómo se habla con Revit (`ExternalEvent`): es la mejora con más riesgo técnico y va
  en fase propia.
- **Renumerar nudos** (P5) puede confundir si ya te acostumbraste a "N4": se decide contigo.
- **Las etiquetas pinchables (V3)** dependen de una parte de la API de Revit que no se ha usado aún en este proyecto:
  primero un sondeo, y si no va, se queda el marcador de hoy.
- **Los fantasmas (V2)** son elementos del modelo (transparentes, en una categoría propia): hay que garantizar que
  *Descartar* y *Crear* no dejen ninguno, igual que hoy con los marcadores (sondeo 17).
