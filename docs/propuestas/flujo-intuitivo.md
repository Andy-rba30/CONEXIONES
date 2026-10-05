# Propuesta: hacer las conexiones más intuitivas ("Conectar" en tres pasos)

Fecha: 2026-10-05. Estado: **propuesta, sin código**. Se convierte en fase cuando respondas las preguntas de la sección 6.

Pregunta de origen: "funciona bien, pero ¿no se podría mejorar la forma de hacer las conexiones? Ahora mismo parece
muy engorroso y es complicado entenderlo".

## 1. Por qué hoy parece engorroso (diagnóstico)

Lo que hay funciona y está probado. El problema no es la mecánica, es que la persona ve **la tripa del sistema** (el
JSON, los códigos, los tokens) en vez de ver una conexión de acero. En concreto:

| # | Qué pasa hoy | Dónde se ve |
|---|---|---|
| 1 | **Para empezar hace falta un archivo JSON** (o pasar por Catálogo). No existe "selecciono las barras y pulso Conectar". | Botón *Ejecutar especificación JSON* pide un archivo |
| 2 | **Se piensa en JSON**: la tabla tiene columna *Ruta JSON* (`members[0].attachment.slot_length_mm`), el botón dice *Guardar JSON*, y el espesor hay que escribirlo dos veces (`9.525` y `3/8"`) o salta `LABEL_VALUE_MISMATCH`. | Ventana de previsualización |
| 3 | **Palabras internas a la vista**: `ready`, `no_match`, `untyped`, `mirror_x`, `end_gap_mm`, `plan_id`, token de 64 letras, `N4 · same`. | Ventana del plan, tabla de nudos |
| 4 | **Conceptos que son para la IA, no para la persona**: cadenas de cotas, dudas (`uncertain_fields`), `source`, `spec_version`. Ocupan sitio en la ventana y confunden. | Tabla de la previsualización |
| 5 | **Cuatro botones, cuatro ventanas** y la del plan tiene **12 botones**. No se sabe cuál es el siguiente paso. | Cinta ARBA > MotorConexiones |
| 6 | **La ventana se cierra para orbitar** el modelo (decisión P10, opción A). | Ventanas de previsualización y de plan |
| 7 | **La cartela se escribe como 8 puntos** `x; y`. Nadie piensa una cartela así. | Cuadro *Contorno de la cartela* |
| 8 | **El plan enseña 59 nudos cuando importan 16**: 23 son barras sueltas y 20 son parejas sin cordón. | Paso 8b-4 de la ronda 8b |
| 9 | **Para la IA, 10 pasos obligatorios** aunque el nudo sea igual que el anterior (ping, nudo, esquema, leer, cadenas, dudas, validar, resumen, preview, crear). | `docs/guide.md`, sección 1 |

Todo esto es **presentación**. El motor (Core, contrato, validación, transacciones, herramientas `conn_*`) no tiene que
cambiar: se le pone una cara nueva encima. El JSON sigue existiendo debajo, para la IA y para guardar.

## 2. Lo que NO cambia

- El contrato `gusset_node`, el esquema, `limits.json`, el catálogo y el plan de lote.
- Las 21 herramientas `conn_*` y su flujo con `validation_token` (la IA sigue sin ventanas).
- Una operación = un `TransactionGroup`; error = rollback completo.
- El croquis 2D con cotas y el doble clic sobre una cota (eso ya es intuitivo y se queda).

## 3. Cómo sería: "Conectar" en tres pasos, una sola ventana

```
Paso 1  Selecciona en Revit las barras del nudo (o la cercha entera) y pulsa  [ Conectar ]
        No hace falta ningún archivo.

Paso 2  El add-in reconoce el nudo, elige la plantilla del catálogo que mejor encaja y abre UNA ventana:

        ┌────────────────────────────────────────────────────────────────────────────┐
        │ Conectar nudo  ·  Plantilla: Detalle D  [cambiar]      ● Lista para crear  │
        ├─────────────────────────────┬──────────────────────────────────────────────┤
        │                             │ CARTELA                                       │
        │     croquis 2D con cotas    │   Espesor   [ 3/8"  ▾ ]  (9,5 mm)             │
        │     (el de hoy, igual)      │   Ancho × alto   565 × 530 mm                 │
        │                             │   Unión al cordón  [ ranura pasante ▾ ]       │
        │                             │ CORDÓN   HSS3X3X1/4                           │
        │                             │ BARRA 1  diagonal 45°  HSS2-1/2               │
        │                             │   Unión  [ ranura soldada ▾ ]  ranura 150 mm  │
        │                             │   Retiro 180 mm   Soldadura filete 5 mm       │
        │                             │ BARRA 2  montante …                           │
        │                             │ BARRA 3  diagonal −135°                       │
        │                             │   Unión  [ placa cuchilla con pernos ▾ ]      │
        │                             │   Pernos [ 5/8" ▾ ]  2 filas × 2  paso 75     │
        │                             │ ▸ Avanzado (contorno punto a punto, JSON)     │
        ├─────────────────────────────┴──────────────────────────────────────────────┤
        │ ✔ Sin problemas                      [ Guardar como plantilla ] [ Crear ]   │
        └────────────────────────────────────────────────────────────────────────────┘

Paso 3  Si hay un problema, se ve en español y con un botón que lleva al campo:
        "El paso entre pernos (10 mm) es menor que el mínimo (43 mm).  [Ir al campo]"
        Con el semáforo en verde, [ Crear ]. Sin tokens, sin rutas JSON, sin códigos.
```

**Si seleccionaste una cercha entera**, la misma ventana cambia a modo lote:

```
        ┌────────────────────────────────────────────────────────────────────────────┐
        │ Conectar cercha  ·  16 nudos listos · 2 con problema · 20 sin cordón ·     │
        │                     21 barras sueltas (ocultas)  [mostrar todo]            │
        ├────────────────────────────────────────────────────────────────────────────┤
        │ Nudo │ Estado          │ Plantilla  │ Espejo │ Qué hacer                   │
        │ N4   │ ● Listo         │ Detalle D  │  no    │ (clic derecho: ver, editar,  │
        │ N7   │ ● Listo         │ Detalle D  │  sí    │  excluir, cambiar cordón…)   │
        │ N12  │ ▲ Perfil HSS4X4 │ Detalle D  │  no    │ ¿Usar la misma cartela?      │
        │ N19  │ ✖ Falta cordón  │ —          │  —     │ Añade el cordón y replanifica│
        ├────────────────────────────────────────────────────────────────────────────┤
        │ (la ventana se queda abierta mientras orbitas y pinchas en Revit)          │
        │                                              [ Crear 16 conexiones ]       │
        └────────────────────────────────────────────────────────────────────────────┘
```

Lo de hoy no se pierde: *Abrir JSON…*, *Guardar JSON*, el contorno punto a punto y las cadenas de cotas siguen ahí,
pero dentro de **Avanzado**, no en la primera pantalla.

## 4. Mejoras concretas, de más a menos impacto

| # | Mejora | Qué cambia para ti | Esfuerzo | Qué se reutiliza |
|---|---|---|---|---|
| M1 | **Botón único "Conectar"**: selección → nudo → plantilla que encaja → croquis. Sin archivo. | Un clic para empezar. | Medio | `NodeDetector`, el casador del catálogo y la ventana de hoy |
| M2 | **Panel de propiedades en lenguaje de taller**, por secciones (Cartela, Cordón, Barra 1-2-3), con desplegables: espesor de placa (1/4", 5/16", 3/8", 1/2"…) que pone los mm solo, tipo de unión, diámetro de perno. JSON y cadenas de cotas a *Avanzado*. | Dejas de escribir dos veces el espesor y de ver rutas JSON. Se acaba `LABEL_VALUE_MISMATCH`. | Medio | El JSON se genera por detrás; la validación es la misma |
| M3 | **Vocabulario**: estados y avisos en español (`ready` → Listo, `no_match` → Sin plantilla que encaje, `untyped` → Barra suelta, `mirror_x` → En espejo), errores con la solución primero y botón *Ir al campo*. Tokens e IDs fuera de la vista (siguen en el log). | Entiendes qué pasa sin leer el README. | Bajo | Solo textos y la tabla |
| M4 | **Ventana que se queda abierta** (opción B de P10, `ExternalEvent`): orbitas, pinchas barras y la ventana sigue. | Se acaba cerrar y abrir para mirar el modelo. | Medio | La lógica del plan y de la previsualización no cambia |
| M5 | **Plan más limpio**: cabecera con el resumen, barras sueltas ocultas por defecto, menú de clic derecho por nudo en vez de 12 botones, columna *Qué hacer*. | Ves los 16 que importan, no 59. | Bajo-medio | `BatchPlanWindow` |
| M6 | **Cartela automática** (`outline.mode = "auto"`, ya prevista como Fase 10): el contorno sale de las barras y de un margen; se puede retocar después. | No escribes 8 puntos. | Alto | Nueva regla en el Core, con pruebas |
| M7 | **Para la IA, un atajo**: `conn_connect` (selección + plantilla → especificación validada en una llamada) y una guía corta "nudo repetido en 3 pasos". | En el chat: "conecta estas barras como el Detalle D" y listo. | Bajo-medio | `conn_catalog_apply` ya hace casi todo |
| M8 | **Ayuda dentro de la ventana**: panel "¿Qué hago ahora?" con los tres pasos y un globo por campo. | Sin abrir documentación. | Bajo | Textos |
| M9 | **Arrastrar en el croquis** (esquinas de la cartela, grupo de pernos). | Editar con el ratón. | Alto | Más adelante |

Si solo se hiciera una, sería **M1 + M2 + M3** juntas: es lo que convierte "ejecutar un JSON" en "conectar un nudo".

## 5. Dónde encaja en el plan de fases

La Fase 9 (crear el lote) está a medio camino y no conviene parar. Propuesta de orden, una fase por sesión:

| Fase | Entrega | Se prueba en la nube | Se prueba en el PC |
|---|---|---|---|
| **9. Crear por lotes** (como está previsto) | `conn_batch_create`, botón *Aplicar lote*, informe por nudo. Con la pregunta P6 del cierre de la 8 resuelta. | Simulador | La cercha del Hangar |
| **10. Conectar en tres pasos** (M1, M2, M3, M8; M7 si cabe) | Botón *Conectar*, panel por secciones, vocabulario, ayuda. Los botones de hoy siguen funcionando. | Pruebas del Core del generador de JSON desde el panel; capturas de la ventana | Un nudo del Hangar de principio a fin sin tocar JSON |
| **11. Ventana abierta y plan limpio** (M4, M5) | `ExternalEvent`, cabecera, filtros, menú por nudo. | Lógica de filtros | Orbitar con la ventana abierta |
| **12. Cartela automática** (M6; era la Fase 10 opcional) | `outline.mode = "auto"` con regla y pruebas. | Pruebas de contorno | Nudos con ángulos distintos |

Alternativa si prefieres **ver la mejora antes**: hacer la Fase 10 primero y dejar la 9 para después. Es posible porque
el lote no depende de la ventana nueva. Lo decides tú en P3.

## 6. Preguntas para ti (responde en el chat; con eso se escribe `docs/prompts/fase-10.md`)

- **P1.** ¿Quién va a usar esto más: tú en la ventana de Revit, o la IA desde el chat? Decide dónde se invierte más.
- **P2.** De la tabla de la sección 1, ¿cuáles tres te molestan más? (basta con los números: por ejemplo "2, 5 y 6").
- **P3.** ¿Hacemos primero la Fase 9 (crear el lote) y después la ventana nueva, o al revés?
- **P4.** ¿Qué espesores de placa y diámetros de perno usas de verdad en tu taller? Si no contestas, se ponen las listas
  habituales (placas 1/4" a 1"; pernos 1/2" a 1") y se pueden editar en `config/limits.json` sin recompilar.
- **P5.** ¿Un solo botón *Conectar* que sirva para un nudo y para la cercha entera (según lo que selecciones), o dos
  botones separados?
- **P6.** Lo avanzado (JSON, contorno punto a punto, cadenas de cotas, dudas): ¿lo dejamos en una pestaña *Avanzado* o
  desaparece de la ventana del todo y queda solo para la IA?
- **P7.** ¿Hay alguna palabra de taller que prefieras? Por ejemplo: "cartela" o "plancha de nudo", "retiro" o "corte",
  "placa cuchilla" o "cuchilla". Se usarán tus palabras en la ventana.

## 7. Riesgos

- **Dos caminos para lo mismo** (ventana nueva y botones antiguos) durante una fase: se mantienen los dos hasta que la
  nueva esté probada en el PC y entonces los viejos pasan a *Avanzado*.
- **La ventana no modal** (M4) cambia cómo se habla con Revit (`ExternalEvent`): es la mejora con más riesgo técnico y
  por eso va en una fase propia.
- **Desplegables cerrados** (espesores, pernos): si falta un valor, el campo admite escribirlo a mano y el validador
  sigue mandando.
