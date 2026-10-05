# Propuesta: que la parte de las cerchas (plan de lote) se entienda

Fecha: 2026-10-05. Estado: **propuesta, sin código**. Se convierte en fase cuando respondas las preguntas de la sección 7.

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
| 8 | **El plan no crea nada** y no se ve el final del camino: falta el botón *Crear 16 conexiones* (Fase 9). | — |
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

## 6. Dónde encaja en el plan de fases (recomendación)

Como lo difícil es esta ventana, y la Fase 9 va a poner en ella el botón *Crear*, conviene **entenderla antes de crear**:

| Fase | Entrega | Se prueba en la nube | Se prueba en el PC |
|---|---|---|---|
| **8c. Ventana del plan entendible** (ronda corta) | C1, C2, C4, C5, C8, C9. Solo presentación: textos, filtros, cabecera, colores por estado, guía. | Pruebas del Core del resumen; `probar_conexiones.py`; capturas | Misma cercha: ver 16 nudos, colores por estado, cabecera |
| **9. Crear por lotes** (como está previsto) + C3 | `conn_batch_create`, botón *Crear N conexiones*, informe por nudo, *Borrar el lote*; columna *Qué hacer*. Con la pregunta P6 del cierre de la 8 resuelta. | Simulador | Crear el lote en la cercha, deshacer, borrar |
| **10. Cercha sin dolor** | C6 (selección asistida) y C7 (ventana abierta). | Lógica de selección | Pinchar una barra y orbitar con la ventana abierta |
| **11. Conectar un nudo** | Botón *Conectar* y panel por secciones para un nudo suelto. | Generador de JSON desde el panel | Un nudo de principio a fin sin tocar JSON |
| **12. Cartela automática** (era la 10 opcional) | `outline.mode = "auto"`. | Pruebas de contorno | Nudos con otros ángulos |

Alternativa: meter la 8c dentro de la Fase 9 (una sesión más larga). Lo decides en P3.

## 7. Preguntas para ti (responde en el chat; con eso se escribe el prompt de la 8c)

- **P1.** De la tabla de la sección 2, ¿cuáles tres te molestan más? Basta con los números ("1, 3 y 6").
- **P2.** ¿Te vale ocultar por defecto las barras sueltas y las parejas sin cordón (con un contador y *mostrar los ocultos*),
  o prefieres verlas siempre en gris al final de la tabla?
- **P3.** ¿Hacemos la ronda 8c (ventana entendible) **antes** de la Fase 9, o todo junto en la Fase 9?
- **P4.** Colores en el modelo: ¿por estado (verde / ámbar / rojo / gris) como propongo, o quieres conservar un color por
  nudo para distinguirlos entre sí?
- **P5.** Numeración: ¿1…16 solo para los nudos de verdad (cambia al replanificar si aparece uno nuevo), o se mantiene
  N1…N59 fija como hoy?
- **P6.** ¿Un solo botón *Conectar* que sirva para un nudo y para la cercha (según lo que selecciones), o dos botones?
- **P7.** Palabras: ¿"cartela" o "plancha de nudo", "retiro" o "corte", "placa cuchilla" o "cuchilla", "en espejo" o
  "lado derecho"? Se usarán tus palabras.

## 8. Riesgos

- **Ocultar nudos** puede esconder un error real (por ejemplo, un nudo de verdad que salió como pareja sin cordón por un
  cordón mal dibujado). Por eso la cabecera siempre muestra el contador y *mostrar los ocultos*.
- **La ventana no modal** (C7) cambia cómo se habla con Revit (`ExternalEvent`): es la mejora con más riesgo técnico y va
  en fase propia.
- **Renumerar nudos** (P5) puede confundir si ya te acostumbraste a "N4": se decide contigo.
