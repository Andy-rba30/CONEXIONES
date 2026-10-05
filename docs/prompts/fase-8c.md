# Ronda 8c: la ventana del plan de lote se entiende (mapa de la cercha, estados en español, colores por estado)

> **Ejecutada el 2026-10-05** (add-in 0.8.3): informe en `docs/fases/fase-8.md`, sección 9; instalación en
> `docs/instalacion/fase-8c.md`; resumen en `docs/fases/resumen-fase-8c.md`. El mapa cupo en la sesión: no hay ronda 8d.

Sale de `docs/propuestas/flujo-intuitivo.md` (secciones 2, 5 y 6). Se ejecuta en **una sesión**, con el prompt de la
sección 1, **antes de la Fase 9**. Es una ronda de **presentación**: no cambia la detección, el casado, la validación ni
el contrato; cambia lo que la persona ve en la ventana, en el modelo y en el chat. Las decisiones de diseño están en la
sección 9 (tomadas por recomendación; la persona puede cambiarlas en el chat antes de empezar).

## 1. Prompt para pegar en la sesión nueva (Claude Code, sesión nueva en la nube)

```
Lee CLAUDE.md, docs/prompts/fase-8c.md completo, docs/propuestas/flujo-intuitivo.md y docs/fases/fase-8.md (secciones 7 y 8).
Ejecuta SOLO la ronda 8c tal como la describe docs/prompts/fase-8c.md: ventana del plan entendible (solo nudos de verdad,
estados en español, cabecera con la decisión, columna "Qué hacer", menos botones), marcas del modelo con color por estado,
mapa de la cercha en la ventana, guía de la IA que resume el plan en español, aviso de catálogo vacío y el sondeo 19 de
etiquetas en el lienzo. No cambies el contrato ni la detección. No empieces la Fase 9. Termina con la sección 9 de
docs/fases/fase-8.md, docs/instalacion/fase-8c.md, README y docs/guide.md al día, commit, push y un resumen corto;
guarda ademas el resumen en docs/fases/resumen-fase-8c.md por si el chat no se ve.
```

## 2. Por qué esta ronda

En la ronda 8b la persona vio **59 nudos** para una cercha de 16, estados en inglés (`ready`, `no_match`, `untyped`,
`mirror_x`), tokens e IDs en la tabla, 12 botones y un color por nudo. Dijo que esa parte es la que no entiende. La Fase
9 va a poner en esa misma ventana el botón **Crear N conexiones**: conviene que la ventana se entienda **antes** de crear.
Lo que viste en la 8b explicado en palabras sencillas: `docs/propuestas/flujo-intuitivo.md`, sección 1.

## 3. Alcance (lo que se entrega)

### 3.1 Tabla del plan: solo nudos de verdad y en español (C1, C2, C3 texto, C4, C5)

- **Visibles por defecto**: los nudos con cordón (`ready`, `invalid`, `ambiguous_chord`, `offset`, `already_connected`,
  `excluded`). **Ocultos por defecto**: `untyped` (barra suelta) y los `no_match` de dos barras con
  `NODE_CHORD_NOT_CONTINUOUS` (pareja sin cordón). La cabecera lleva el contador ("20 sin cordón · 23 barras sueltas") y
  un botón **Mostrar ocultos** / **Ocultar** que los enseña en gris al final de la tabla. Los `no_match` **con** cordón
  (ninguna plantilla encaja) siguen visibles: son nudos de verdad a los que les falta plantilla.
- **Estados en español**, con icono y color, en la columna *Estado* (el JSON del contrato no cambia; traduce la ventana):

  | `status` | Texto | Color |
  |---|---|---|
  | `ready` sin avisos | ● Listo | verde |
  | `ready` con avisos (`TEMPLATE_PROFILE_DIFFERS`, `TEMPLATE_ANGLE_DEVIATION`…) | ▲ Listo con aviso | ámbar |
  | `invalid` | ✖ No valida | rojo |
  | `no_match` con cordón | ✖ Sin plantilla que encaje | rojo |
  | `no_match` sin cordón (`NODE_CHORD_NOT_CONTINUOUS`) | ✖ Falta el cordón | rojo (oculto por defecto) |
  | `ambiguous_chord` | ✖ Dos cordones posibles | rojo |
  | `offset` | ✖ Los ejes no se cortan | rojo |
  | `already_connected` | ◌ Ya tiene conexión | gris |
  | `excluded` | ◌ Excluido | gris |
  | `untyped` | ○ Barra suelta (no es nudo) | gris (oculto por defecto) |

  *Orient.* pasa a **Espejo**: "no" / "sí" (`same` / `mirror_x`; `mirror_y` y `both` se muestran como "sí (Y)" y
  "sí (XY)"). Las columnas *Token*, *Cordón* (id), *Barras* (ids) y *Color* salen de la tabla; el id del cordón, los
  ids de las barras, el token abreviado y `end_gap_mm` se ven en el **detalle del nudo** (panel inferior), que ya existe.
- **Columna "Qué hacer"** (solo texto en esta ronda; los botones de acción llegan en la Fase 9): una frase por caso,
  escrita en el Core (`PlanAdvice`, ver 3.6) para que la IA y la ventana digan lo mismo:
  `ready` → "—"; `ready` con `TEMPLATE_PROFILE_DIFFERS` → "El cordón es HSS4X4 y la plantilla HSS3X3: se creará con la
  misma cartela; exclúyelo si no quieres"; `invalid` con `PLATE_OUTSIDE_GUSSET` → "Una barra se sale de la cartela:
  Editar nudo y agrandarla, o excluir"; `no_match` con cordón → "Ninguna plantilla encaja (N barras, ángulos …): crea
  esa típica o excluye"; sin cordón → "Falta el cordón en la selección: selecciónalo y replanifica, o Cordón…";
  `ambiguous_chord` → "Elige el cordón con Cordón…"; `offset` → "Los ejes se cruzan a X mm: corrige el modelo o
  excluye"; `already_connected` → "Se salta; replanifica con 'rehacer existentes' para incluirlo"; `untyped` → "No
  es un nudo: nada que hacer".
- **Cabecera con la decisión** (texto del Core, `PlanSummaryText`): "Se crearán **16** conexiones con *Detalle D*
  (8 iguales, 8 en espejo). 14 avisan de perfil distinto. 2 nudos no validan. Ocultos: 20 sin cordón, 23 barras sueltas."
  Con 0 listos: "Ningún nudo listo: …" y la causa más frecuente.
- **Botones**: quedan **Replanificar**, **Editar nudo**, **Ver en Revit** y **Cerrar**. El resto (Excluir/Incluir,
  Cordón…, Barras…, Plantilla…, Añadir nudo…, Quitar edición, Guardar plan JSON, Descartar plan) pasa al **menú de clic
  derecho** sobre el nudo y a un botón **Más…** (Añadir nudo, Guardar plan JSON, Descartar plan). Nada se pierde.
- **Leyenda** al pie: "Verde = se creará · Ámbar = se creará con aviso · Rojo = falta algo · Gris = no se crea".
- Título de la ventana: "MotorConexiones: conectar cercha (plan, todavía no crea nada)".

### 3.2 Marcas del modelo con color por estado (C2, decisión P4)

`PlanMarks` colorea las barras de cada nudo con el color **de su estado** (verde, ámbar, rojo, gris; mismo RGB que la
ventana), no con un color por nudo. Cubo (igual) y rombo (espejo) se mantienen; el nombre sigue en `Name` y
`Comentarios`. Los nudos ocultos por defecto en la tabla **no se marcan** (ni color ni marcador): la cercha se ve limpia
con sus 16 nudos. `color_name` / `color_rgb` de la respuesta pasan a ser los del estado (`verde`, `ambar`, `rojo`,
`gris`); se documenta en `mcp/CONTRATO-conn.md` como cambio de valores, no de claves.

### 3.3 Mapa de la cercha en la ventana (V1)

- **Core, `Batch/TrussMap.cs`** (sin Revit): a partir del plan (puntos de trabajo, ejes y extremos de las barras, estados)
  calcula el **alzado** de la cercha: elige el plano (el de los ejes; si no son coplanares, el de mínimos cuadrados),
  proyecta cada barra a un segmento 2D en mm y cada nudo a un punto con su nombre, estado, espejo y plantilla. Devuelve
  también el rectángulo envolvente. Es geometría pura y se prueba con los fixtures.
- **Ventana**: un lienzo (misma técnica que `SketchCanvas`) arriba de la tabla, con las barras en gris, los nudos como
  círculos del color de su estado con el número dentro, los ocultos como círculos vacíos pequeños (solo con *Mostrar
  ocultos*). Clic en un círculo = seleccionar su fila (y su detalle); doble clic = **Ver en Revit**. Pasar el ratón =
  globo "N4 · Listo · Detalle D · en espejo". Rueda = zoom, arrastrar = encuadre, **Ajustar** = ver todo. El mapa se
  redibuja al replanificar.
- Si el mapa no cabe en la sesión: se entrega todo lo demás, se deja `TrussMap` con sus pruebas y la ventana sin lienzo,
  y se escribe `docs/prompts/fase-8d.md` solo con el lienzo. Se dice en el informe.

### 3.4 Guía de la IA (C8)

`docs/guide.md`, sección 6: tras `conn_batch_plan`, la IA **no vuelca el JSON**; enseña cuatro líneas en español
(`data.summary_text` nuevo, el mismo de la cabecera) y una tabla corta solo con los nudos visibles (nombre, estado en
español, espejo, plantilla, qué hacer). Los ocultos se resumen en una línea. La respuesta del MCP añade por nudo
`status_text` (español), `advice` (qué hacer) y `visible_by_default` (bool), y en `data` el `summary_text`. Solo añade
claves; no quita ninguna.

### 3.5 Catálogo vacío (C9)

Si al planificar no hay ninguna plantilla en el catálogo (desde el botón o desde la IA), la respuesta y la ventana lo
dicen en español: "No hay plantillas: crea primero la conexión de un nudo y guárdala con *Guardar en catálogo*" (aviso
`CATALOG_EMPTY`). La ventana ofrece **Abrir catálogo**. Hoy ese caso sale como todo `no_match` sin explicación.

### 3.6 Textos en el Core, no en la ventana

`Core/Batch/PlanAdvice.cs`: `StatusText(node)`, `Advice(node)`, `Color(node)`, `VisibleByDefault(node)` y
`SummaryText(plan)`. La ventana, `PlanMarks` y la respuesta del MCP los llaman; así hay una sola fuente y se prueban
sin Revit.

### 3.7 Sondeo 19: etiquetas en el lienzo (para V3, sin código de producción)

`scripts/sondeos/19-etiquetas-lienzo.py`: comprueba en Revit 2027 si existen `TemporaryGraphicsManager`,
`InCanvasControlData` e `ITemporaryGraphicsHandler` (nombres a confirmar con `clr`/`dir`; **no inventar**), crea un
control con una imagen PNG pequeña (generada por el sondeo, con un número y fondo verde) en el punto de trabajo del
Detalle D en la vista activa, captura, lo quita y comprueba que no queda nada. La salida decide si la Fase 10 usa
etiquetas pinchables o se queda con los marcadores. Es un sondeo: si falla, se anota y no bloquea la ronda.

## 4. Reglas técnicas de esta ronda

- **El contrato no cambia de forma**: `status`, `orientation`, nombres `N1…`, `overrides`, tokens, siguen iguales. Solo
  se **añaden** claves (`status_text`, `advice`, `visible_by_default`, `summary_text`) y cambian los **valores** de
  `color_name` / `color_rgb`. La detección (`NodeDetector`), el casado y `PlanBuilder` no se tocan; el fixture
  `HangarTruss8b` debe seguir dando 59 nudos y 16 `ready`.
- Nombres de nudo **estables** (`N4` sigue siendo `N4`): la IA, las correcciones y los marcadores dependen de ellos
  (decisión P5).
- Sin ventanas en las rutas `conn_*`. La ventana del plan sigue **modal** (la no modal es la Fase 10, C7).
- Marcas: cambios de vista y `DirectShape`, una sola entrada de deshacer, `discard` las quita todas (sondeo 17 lo
  comprueba).
- `ElementId.Value`; nada de miembros de la API sin compilar contra 2027 o sondear (sondeo 19).
- Textos de la ventana, avisos y guía en español; código, claves JSON y nombres de clase en inglés.
- Versión **0.8.3** en add-in, adaptador y herramientas (regla `0.N.x` para rondas).
- Unidades solo en `Units/UnitConverter.cs`; `TrussMap` trabaja en mm.

## 5. Pruebas en la nube

- `PlanAdviceTests`: texto, color, visibilidad y consejo para cada estado y aviso de la tabla de 3.1; `SummaryText`
  sobre `HangarTruss8b` = "Se crearán 16 conexiones con Detalle D (8 iguales, 8 en espejo). 14 avisan de perfil
  distinto. Ocultos: 20 sin cordón, 23 barras sueltas." (o el texto que se fije, exacto); con catálogo vacío,
  `CATALOG_EMPTY`.
- `TrussMapTests`: sobre `HangarTruss8b`, 56 segmentos, 59 puntos, 16 visibles; el plano del alzado es el de la cercha
  (Z = 17423 constante → alzado en X–Y del modelo), N4 en X = −11870 y N7 en X = −6740,5; sobre `SyntheticTruss`, el
  cordón invertido no cambia el mapa; con tres barras no coplanares, mínimos cuadrados.
- `probar_conexiones.py` y el simulador: las claves nuevas presentes en `batch_plan` y `batch_plan_get`; `color_name`
  del estado; `CATALOG_EMPTY` con el catálogo vacío (el simulador lo permite). Siguen 26/26 (28 con `--puente`), más las
  que se añadan.
- `dotnet build` sin avisos, `dotnet test` en verde, `py_compile` de `mcp/` y `scripts/sondeos/`.

## 6. Instrucciones para el instalador (`docs/instalacion/fase-8c.md`)

Una ronda sobre la copia del Hangar, con `Anota` y resultados en `docs/fases/resultados-fase-8c.md`. **Esta vez la
salida de `probar_conexiones.py --puente` y el log del día se anotan en el archivo** (la 8b los perdió: usar `Anota`
también para ellos y comprobar al final que el archivo tiene esas dos secciones antes del commit).

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado (`0.8.3.0`), `instalar-conn.ps1`.
2. Abrir Revit: `conn_ping` → `0.8.3`.
3. **(la persona)** seleccionar la misma cercha de la 8b **y esta vez también los cordones superior e inferior** →
   `conn_batch_plan` con la plantilla oficial: anotar `summary_text`, cuántos visibles y ocultos, y captura de la
   cercha con los colores por estado (`fase8c-01-colores-estado.png`). Se espera: más nudos listos que 16 (los de
   arriba y abajo ya tienen cordón) o, si la plantilla no encaja arriba y abajo, `no_match` **con** cordón y su consejo.
4. Botón **Planificar lote** con la misma selección: capturas de la ventana con el mapa (`fase8c-02-mapa.png`), la
   cabecera, la tabla con estados en español y la columna *Qué hacer*; clic en un círculo del mapa; **Mostrar ocultos**;
   menú de clic derecho (Excluir); **Replanificar**; **Editar nudo** sobre un listo.
5. Catálogo vacío: mover temporalmente las plantillas (`%LOCALAPPDATA%\MotorConexiones\catalogo\*.json` a una carpeta
   aparte), **Planificar lote** → aviso `CATALOG_EMPTY` y botón **Abrir catálogo**; devolver las plantillas.
6. Sondeo 19 (etiquetas en el lienzo): salida completa y captura si funciona (`fase8c-03-etiqueta.png`).
7. `conn_batch_plan_discard` (`all: true`); sondeos 17, 12 y 13 en cero.
8. `probar_conexiones.py --puente` (28/28) **anotado en el archivo**, y el log del día filtrado por
   `batch_plan|ribbon_batch|startup` (últimas 30 líneas), también anotado.
9. Commit de `docs/fases/resultados-fase-8c.md` y capturas; push.

Lo NO PROBADO de la 0.8.2 (`docs/fases/fase-8.md`, 8.5: `end_gap_mm` en la respuesta, `PLAN_MARKS_REPLACED`,
`removed_markers`, `overrides` sin `IsEmpty`, paso 9 del sondeo 17) se comprueba en esta misma ronda: añadir los pasos.

## 7. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas; simulador y `probar_conexiones.py` en
  verde; `py_compile` de todo.
- `docs/fases/fase-8.md`, **sección 9 "Ronda 8c"**, con qué se hizo, qué se probó, lo NO PROBADO y las decisiones;
  `docs/instalacion/fase-8c.md` literal con `Anota` y capturas con nombre fijo; `docs/fases/resumen-fase-8c.md`.
- README (sección 12 y tabla de garantías), `mcp/CONTRATO-conn.md` (claves nuevas, valores de color, `CATALOG_EMPTY`),
  `docs/guide.md` (sección 6) y `CLAUDE.md` (estructura, si hay archivos nuevos) al día.
- `docs/propuestas/flujo-intuitivo.md`: marcar qué quedó hecho (C1, C2, C3 texto, C4, C5, C8, C9, V1) y qué sigue.
- Commit en español en `main`; sin pull requests.

## 8. Fuera de alcance de esta ronda

- Crear el lote (`conn_batch_create`, botón **Crear N conexiones**): Fase 9.
- Botones de acción en la columna *Qué hacer* (C3 completo) y cartelas fantasma (V2): Fase 9.
- Selección asistida (C6), ventana no modal (C7) y etiquetas pinchables en producción (V3): Fase 10.
- Botón **Conectar** para un nudo suelto, panel por secciones, miniaturas (V4), plano al lado (V5): Fase 11.
- Cartela automática (V6 arrastrar, `outline.mode = "auto"`): Fase 12.
- Renumerar los nudos (P5): no; los nombres siguen estables.

## 9. Decisiones tomadas por recomendación (cámbialas en el chat antes de empezar)

| Pregunta de la propuesta | Decisión |
|---|---|
| P1 Qué molesta más | 1 (59 nudos), 2 (palabras internas), 3 (no dice qué hacer). Es lo que ataca esta ronda. |
| P2 Ocultar barras sueltas y parejas sin cordón | Sí, ocultas por defecto, con contador y **Mostrar ocultos**. |
| P3 Antes o después de la Fase 9 | **Antes**: ronda 8c. |
| P4 Colores en el modelo | **Por estado** (verde, ámbar, rojo, gris). Cubo y rombo se quedan. |
| P5 Numeración | Se mantiene `N1…N59` (estable). Los ocultos no se ven, así que la lista visible es corta igualmente. |
| P6 Un botón o dos | Se decide en la Fase 11; esta ronda no toca la cinta. |
| P7 Palabras | cartela, retiro, placa cuchilla, "en espejo". Si prefieres otras, dilo y se usan. |
| P8 Opciones visuales | **V1 (mapa)** en esta ronda; **V2 (fantasmas)** en la Fase 9; V3 solo el sondeo 19; V4 y V5 en la Fase 11. |
| P9 Detalles como imagen | Sin respuesta: V5 queda para la Fase 11. |
| P10 Fantasmas también en los de aviso | Sí, en ámbar (Fase 9). |
