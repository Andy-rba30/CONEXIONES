# Resumen de la Fase 11 (2026-10-12): conectar un nudo sin JSON, panel por secciones, miniaturas y el plano al lado

Rama `main`. Add-in **0.11.0** (**NO PROBADO en Revit**: la última versión desplegada en el PC es la 0.10.1). Informe:
`docs/fases/fase-11.md`. Instalación: `docs/instalacion/fase-11.md`. Prompt y alcance: `docs/prompts/fase-11.md`. Copia corta
del mensaje del chat.

## 1. Qué se hizo

- **Botón Conectar** (el primero de la cinta): con el cordón y las barras del nudo seleccionadas reconoce el nudo, casa todas
  las plantillas del catálogo y abre la previsualización con la mejor ya aplicada, sin pedir ningún JSON ("Plantilla
  propuesta: Nudo tipico Detalle D · igual · desvío máx. 0,0°"; **Cambiar…** para otra). Con una barra pide pinchar las demás
  en Revit (Esc termina); si la selección es una cercha, ofrece abrir Planificar lote. Si ninguna plantilla encaja abre igual
  una **especificación de arranque**: cartela rectangular que envuelve los extremos de las barras, cada barra soldada en
  ranura, valores de la plantilla más reciente con el mismo cordón o de `config/choices.json`, la unión al cordón como duda
  confirmada y el botón **Encargo para IA** a mano. No crea nada al pulsarse.
- **Panel por secciones** en la previsualización (Cartela, Cordón, Barra 1-2-3) en vez de la tabla: desplegables de espesor
  (`3/8" · 9,5 mm`), diámetro de perno (1/2", 5/8", 3/4", 7/8"), soldadura, unión (soldada en ranura / placa cuchilla
  atornillada: al cambiar aparecen placa y pernos por defecto y al volver se recupera la ranura), unión al cordón, cara y rol;
  escriben los mm y el rótulo a la vez; errores junto al campo. La tabla de siempre, el JSON (Copiar), el contorno y las dudas
  están en la pestaña **Avanzado**. Editar nudo desde el plan estrena el panel sin más.
- **V4, miniaturas**: un PNG de 240×180 con el croquis de cada plantilla, junto a su `.json`, en el catálogo (columna y vista
  grande), en Abrir del catálogo, en Cambiar…, en Plantilla… y en el detalle del nudo del plan (no en el mapa).
- **V5, el plano al lado**: **Plano…** abre un PNG o JPG (o un recorte pegado del portapapeles) a la izquierda del croquis con
  zoom y encuadre; la ruta se recuerda en `source.drawing_image` (clave nueva y opcional del contrato) y viaja con la plantilla.
- **De paso**: la línea del plan dice "(63 añadidas por la selección asistida)" también desde la cinta; `label_clicked` lleva
  `changed` y `highlighted`; los títulos de los cuadros sin "MotorConexiones - "; `conn_catalog_apply` admite **sin
  `template_id`** (la IA recibe `data.proposal`). `config/choices.json` nuevo (editable sin recompilar). Versión 0.11.0 en
  todo; contrato, guía, README, CLAUDE.md y propuestas al día.

## 2. Qué se probó en la nube

- `dotnet build` sin avisos; `dotnet test` **238/238** (21 nuevas: chooser, arranque, listas, croquis de plantilla, editor,
  `drawing_image`); `py_compile` de `mcp/` y `scripts/sondeos/`; simulador **69/69**; `probar_conexiones.py` **31/31** contra el
  simulador (21b: `catalog/apply` sin `template_id`).

## 3. NO PROBADO (se prueba en el PC con `docs/instalacion/fase-11.md`, unos 75 minutos)

Todo lo de WPF y Revit: el panel, los PNG de las miniaturas (`RenderTargetBitmap`), el plano y el pegado, el botón Conectar
de punta a punta, los títulos y los dos retoques del lote. El arranque sobre el cordón superior puede no validar
(`CLASH_WITH_FOREIGN_MEMBER`): es lo que la fase viene a averiguar, no un fallo.

## 4. Qué sigue

Dar `docs/instalacion/fase-11.md` al instalador y devolver `resultados-fase-11.md` con las seis capturas; con eso, el cierre
de la fase (y una 11b si hace falta). Después, la Fase 12 (cartela automática).
