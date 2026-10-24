# Resumen de la Fase 13 (2026-10-24): cartela automática y partir la diagonal

Rama `main`. Add-in **0.13.0** (**NO PROBADO en Revit**: la última versión desplegada en el PC es la 0.12.3; la 0.13.0 lleva también
el filtro de la 0.12.4). Informe: `docs/fases/fase-13.md`. Instalación: `docs/instalacion/fase-13.md`. Prompt y alcance:
`docs/prompts/fase-13.md`. Copia corta del mensaje del chat.

## 1. Qué se hizo

- **Cartela automática** (`gusset.outline.mode: "auto"`, D1 a D4): el contorno se calcula desde las barras (rectángulo por barra
  hasta el final de la ranura o la placa más 40 mm, 30 mm a cada lado, franja del cordón más 40 mm, envolvente convexa redondeada a
  5 mm, 12 vértices como mucho); lo mismo se valida, se dibuja y se fabrica. `conn_validate`, `conn_preview` y `conn_catalog_apply`
  devuelven `calculated_values.outline_points_mm`. Conectar arranca con `auto` en vez del rectángulo; el panel tiene **Contorno:
  Automático / Polígono** y **Convertir en polígono**; una plantilla guardada desde `auto` casa con 25° y se recalcula al aplicarla;
  el plan arranca con `auto` los nudos con cartela sin plantilla ("Listo (arranque)", contados aparte). Aviso `OUTLINE_AUTO_LARGE`
  si la cartela sale grande. El camino `polygon` no cambia: **21 tokens de la 0.12.4 iguales**.
- **Partir la diagonal** (D5 a D9): `SplitPoint` (cara a (ancho/2)/|sen θ|, +330 hasta la primera plancha, corte en el centro de
  la pila: ≈ 485 mm en el nudo en K del Hangar; `node_info` lo devuelve por diagonal), `conn_split_member` (`/conn/split/`, con
  `dry_run`) y `conn_unsplit` (`/conn/unsplit/`), el botón **Partir barra**, el registro en Extensible Storage (esquema propio) y
  el deshacer (`SPLIT_IN_USE` mientras una conexión use los tramos; `conn_delete` del nudo que pidió el corte lo deshace). El tramo
  corto conserva el id. **Conectar** en un nudo en K con `splice_hint` tiene la casilla "Partir la diagonal y crear el empalme del
  Detalle B a 330": Crear parte, crea el empalme B y el nudo en K de un clic, todo o nada. El **plan** lleva los pasos `split` y los
  nudos `N…B` (`after_split`); el **lote** los ejecuta en orden (cortes → empalmes B → N9E → N9), renueva el token del nudo en K
  tras el corte y **Borrar el lote** deshace los cortes al final. `MemberSplitter` lleva `CopyElement` + `LocationCurve.Curve` con
  reserva `NewFamilyInstance` y los avisos `SPLIT_PARAMETERS_APPROXIMATED` / `SPLIT_UNDO_PARTIAL`; el **sondeo 30** decide.
- **Configuración y MCP**: `choices.json` (`starter.gusset_side_margin_mm`, `starter.outline_mode`), `limits.json`
  (`max_auto_gusset_mm`, `auto_outline_max_vertices`), `catalog.json` (`auto_angle_tolerance_deg`); adaptador 0.13.0 (27 rutas),
  25 herramientas, simulador, contrato y guía al día; sondeo 30.

## 2. Qué se probó en la nube

- `dotnet build` sin avisos; `dotnet test` **311/311** (20 nuevas en `Fase13Tests.cs`: `AutoOutlineTests`, `Fase13RegressionTests`,
  `SpecEditorOutlineTests`, `TemplateAutoTests`, `SplitPointTests`, `Fase13PlanTests`); `py_compile` de `mcp/` y los sondeos;
  simulador **101/101**; `probar_conexiones.py` **46/46** contra el simulador (31 a 33b nuevas).

## 3. NO PROBADO (se prueba en el PC con `docs/instalacion/fase-13.md`, unos 75 minutos)

Todo lo que toca Revit: cómo se parte la barra (sondeo 30 **primero**), la cadena de Conectar, el botón Partir barra, el panel
Contorno y Convertir en polígono, la fabricación de la cartela automática en Advance Steel, el lote con cortes y su deshacer, el
puente. También queda la comprobación pendiente de la 12d (`fase-12.md` 10.3), en el paso 13-3.

## 4. Desvíos respecto al prompt (todos anotados en `fase-13.md`, sección 4)

Esquema de Extensible Storage **nuevo** para los cortes (un esquema no admite campos nuevos con el mismo GUID); la cadena de
Conectar con `SplitChainService` en vez de un `BatchRunner` de tres pasos (mismas garantías); el token del nudo en K se **renueva**
tras el corte (no estaba en el prompt; sin eso `conn_create` lo rechazaría); los pasos de corte salen en el detalle del nudo y los
empalmes B como filas, no como líneas indentadas.

## 5. Qué sigue

Dar `docs/instalacion/fase-13.md` al instalador y devolver `resultados-fase-13.md` con las capturas; con eso, el cierre de la fase y,
si el sondeo 30 lo pide, la ronda 13b (`MemberSplitter` con lo que el sondeo liste).
