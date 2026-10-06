# Resumen de la Fase 10 (2026-10-06): cercha sin dolor (selección asistida, etiquetas pinchables, cartelas fantasma y el encargo para IA)

Rama `main`. Add-in **0.10.0** (**NO PROBADO en Revit**: la última versión desplegada en el PC es la 0.9.0). Informe:
`docs/fases/fase-10.md`. Instalación: `docs/instalacion/fase-10.md`. Prompt y alcance: `docs/prompts/fase-10.md`. Copia corta
del mensaje del chat.

## 1. Qué se hizo

- **Selección asistida (C6)**: basta **pinchar una barra** de la cercha y pulsar **Planificar lote**. El add-in busca las
  barras que la tocan con la misma regla del detector (el extremo de una se corta con el eje de la otra, dentro del alcance
  de cara), por rondas hasta completar la cercha, y se queda **en el plano de la cercha** (correas y riostras fuera). Un
  cuadro dice cuántas añadió y por qué ("Se añadieron 63 barras que tocan la selección: 8 cordones que pasan de largo,
  54 barras que llegan y 1 tramo de cordón.") y ofrece **Planificar con todas**, **Solo la selección** o **Cancelar**.
  También desde la ventana (**Más… > Completar selección**) y desde la IA (`conn_batch_plan` con `expand_selection: true`,
  que devuelve `selection_expansion` y el aviso `SELECTION_EXPANDED`).
- **Etiquetas pinchables en la vista (V3)**: al planificar, cada nudo visible lleva en el punto de trabajo una etiqueta
  redonda con su número y el color de su estado (un BMP de 24 bits de 32×32 escrito a mano, como el del sondeo 19 v4, en
  una carpeta sin tildes ni espacios). **Pincharla nunca abre un cuadro**: la etiqueta se resalta (círculo blanco con el
  número en color), la ventana del plan elige esa fila y lo escribe en su barra de estado ("Etiqueta 4 pinchada en la vista:
  N4 · Listo · Nudo tipico Detalle D · igual"). Elegir una fila en la ventana resalta su etiqueta en Revit. Las etiquetas se
  quitan al replanificar, con las marcas de los nudos creados, al descartar y con `conn_batch_plan_discard`.
- **Cartelas fantasma (V2, entró)**: en cada nudo listo (también con aviso, fallido o que no valida) se dibuja un sólido
  transparente con el contorno de su cartela, de su espesor, en el marco del nudo y del color de su estado: ves en 3D lo
  que se va a crear antes de crearlo. **Crear** lo quita con las marcas del nudo creado; **Descartar** y `discard all` los
  quitan todos.
- **Encargo para IA (ronda 9b, entró)**: botón **Encargo para IA** en la cinta. Con el nudo seleccionado escribe **un solo
  archivo `.md`** (el prompt ya redactado, los datos del nudo de `conn_get_node_info`, el esquema, las secciones 2 a 4 de
  la guía y un ejemplo confirmado: la plantilla del catálogo con la misma cantidad de barras o el Detalle D embebido), lo
  copia al portapapeles y abre la carpeta `%LOCALAPPDATA%\MotorConexiones\encargos\`. Sin herramienta MCP; sin preguntar el
  tipo (solo hay uno).
- **El aviso del borrado** (`fase-9.md` 7.2, punto 3): al borrar, si Advance Steel no abre su sesión, el aviso dice "Los
  elementos se borran directamente (Document.Delete) y las barras se restauran igual", no "se crea con DirectShape".
- `config/catalog.json`: `plan_labels` y `plan_ghosts` (`true`; `false` desactiva etiquetas o fantasmas sin recompilar).
  Sondeo **21** (etiquetas del add-in: controles del lienzo, servidor de clics, carpeta de BMP, fantasmas). Versión 0.10.0
  en add-in, adaptador (25 rutas), herramientas (23) y simulador; contrato, guía, README y CLAUDE.md al día.

## 2. Qué se probó en la nube

- `dotnet build` sin avisos (0.10.0 compila contra la API 2027: `TemporaryGraphicsManager`, `InCanvasControlData`,
  `ITemporaryGraphicsHandler`, `TemporaryGraphicsCommandData`, `MultiServerService`, `TaskDialog.AddCommandLink` existen).
- `dotnet test`: **217/217** (25 nuevas: la selección asistida desde una diagonal y desde el cordón de la cercha sintética,
  riostra fuera del plano, selección completa, tope, la cercha de la 8b recuperada entera desde una diagonal con los mismos
  16 listos; la etiqueta byte a byte; el encargo con todo en orden).
- Simulador **66/66** (etiquetas, `expand_selection`, `removed_labels`) y `probar_conexiones.py` **30/30** contra el simulador
  (pruebas 29 y 30 nuevas: `batch_plan` con `expand_selection` desde una barra y su descartar; las del puente pasan a 31 y 32).

## 3. Qué NO está probado (todo lo de Revit)

Las etiquetas en el lienzo puestas por el add-in y el clic (el sondeo 19 v4 lo hizo con el mismo mecanismo, pero no desde el
add-in), la selección asistida sobre el modelo real (en la nube se probó con la cercha de la 8b reconstruida), las cartelas
fantasma, el botón Encargo para IA, el aviso del borrado y lo pendiente de la Fase 9 (Borrar el lote desde la ventana,
`--puente` entero, `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta). Todo está en `docs/instalacion/fase-10.md`.

## 4. Qué hacer ahora

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-10.md` entero (unos 90 minutos, sobre la copia).
2. Hacer los pasos marcados **(la persona)**: pinchar **una** barra y Planificar lote (el cuadro de la selección asistida),
   mirar las etiquetas y las cartelas fantasma, **pinchar la 4** (sin ningún cuadro), Crear 16, Ctrl+Z, crear otra vez y
   **Borrar el lote desde la ventana**, y el botón **Encargo para IA** sobre el Detalle D. Seis capturas (`fase10-01` a `fase10-06`).
3. Devolver `docs\fases\resultados-fase-10.md` con las anotaciones y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-10.md (secciones 5 y 6) y docs/fases/resultados-fase-10.md. Cierra la Fase 10: contrasta
   los resultados con lo esperado en 2.1 (selección asistida, etiquetas y clic sin cuadro, cartelas fantasma, Borrar el lote
   desde la ventana, Encargo para IA, el puente), corrige lo que haga falta (ronda 10b solo si es imprescindible), actualiza
   el informe y la tabla de garantías del README. No empieces la Fase 11. Termina con commit, push y un resumen corto.
   ```
