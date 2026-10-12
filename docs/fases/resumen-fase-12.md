# Resumen de la Fase 12 (2026-10-12): placa de extremo y nudo soldado

Rama `main`. Add-in **0.12.0** (**NO PROBADO en Revit**: la última versión desplegada en el PC es la 0.11.0). Informe:
`docs/fases/fase-12.md`. Instalación: `docs/instalacion/fase-12.md`. Prompt y alcance: `docs/prompts/fase-12.md`. Copia corta
del mensaje del chat.

## 1. Qué se hizo

- **Excentricidad (opción A).** El punto de trabajo de un nudo en K es el punto del eje del cordón a mitad entre los cortes de
  las diagonales; el contrato suma `node.eccentricity_mm` y `members[i].axis_offset_mm` (opcionales, en los tres tipos); hasta
  medio canto del cordón el desvío es el aviso `MEMBER_ECCENTRIC`, por encima `MEMBER_NOT_AT_NODE`; cada barra se recorta y se
  une sobre su propio corte. `conn_get_node_info` devuelve `kind`, `eccentricity_mm`, `axis_offset_mm` y `axis_cut_mm`. El
  Detalle D y los 16 nudos de la cercha de la 8b conservan sus tokens.
- **Tipo `end_plate`** (empalme con placa de extremo): dos planchas octogonales, pernos alrededor del tubo por cara con tuerca a
  ambos lados, soldadas al tubo; fixtures de los Detalles **B**, **C** e **I** en `docs/fixtures/`; validación propia
  (`SPLICE_*`, `BOLT_HOLE_TOO_SMALL`, `BOLT_TOO_CLOSE_TO_TUBE`, `PLATE_SMALLER_THAN_TUBE`); croquis, panel por secciones
  (Plancha, Pernos por cara, Tramo 1, Tramo 2, Empalme) y arranque por perfil del cordón (C para HSS12X8X1/2, I para HSS3X3X1/4,
  B para HSS2-1/2X2-1/2X3/16, el I escalado para otros) con el retiro calculado del hueco medido.
- **Tipo `direct_weld`** (nudo en K soldado): cada diagonal cortada contra la cara del cordón y soldada en todo el contorno,
  sin cartela; fixture `nudo-K-cordon-superior.json`; croquis con el gap entre cortes y la **marca del empalme de la diagonal
  a 330** (`diagonal_splice_offset_mm`, se dibuja y se escribe, no se fabrica: Fase 13); arranque con filete de 5 mm.
- **Conectar y el lote**: clase de nudo `splice` (dos tramos colineales que se encuentran); un nudo en K sobre un empalme son
  dos nudos en el mismo punto (N9 y N9E) y el lote crea primero el empalme; sin plantilla, el plan arranca `direct_weld` en los
  nudos de un solo lado y `end_plate` en los empalmes. La cercha de la 8c da **41 listos**: 16 Detalle D, 17 soldaduras directas
  (incluidos N49 y N58 de una sola diagonal), 7 empalmes Detalle C y 1 empalme Detalle I (el del cordón central). Conectar
  reconoce las clases, abre el empalme y después el nudo en K del mismo punto, y Cambiar… ofrece **Tipo** para forzar otro
  arranque.
- **Configuración y MCP**: `choices.json` (`hole_labels`, `diagonal_splice_offset_mm`, `starter.direct_weld`,
  `starter.end_plate.by_profile`), `limits.json` (`node_axis_max_offset_mm`, `end_plate.min_bolt_to_tube_mm`,
  `min_hole_clearance_mm`); adaptador, herramientas, simulador y contrato en 0.12.0 sin rutas ni herramientas nuevas.
- **Sondeos 22, 23 y 24** (corte plano contra la cara, pernos a través de dos planchas con tuerca doble y filas 4 × 1,
  soldadura en el contorno), todos con rollback: deciden si la fabricación es la buena o la reserva ya programada
  (`END_CUT_APPROXIMATED`, `BOLT_NUTS_APPROXIMATED`, pernos sueltos, DirectShape).

## 2. Qué se probó en la nube

- `dotnet build` sin avisos; `dotnet test` **268/268** (30 nuevas: `EccentricityTests`, `DirectWeldTests`, `EndPlateTests`,
  `Fase12PlanTests`); `py_compile` de `mcp/` y `scripts/sondeos/`; simulador **82/82**; `probar_conexiones.py` **35/35** contra
  el simulador (30b a 30e: fixture C, fixture K, plan con un empalme).

## 3. NO PROBADO (se prueba en el PC con `docs/instalacion/fase-12.md`, unos 90 minutos)

Toda la fabricación en Revit (corte contra la cara, planchas octogonales, pernos a través de dos planchas y por filas de N × 1,
soldadura en el contorno, retiro real de los tramos), `node_info` sobre el modelo (la excentricidad y la orientación del perfil
HSS12X8), Conectar y el lote de punta a punta, los croquis y paneles en WPF, el camino de una sola barra y el de la cercha.

## 4. Qué sigue

Dar `docs/instalacion/fase-12.md` al instalador (**los sondeos primero**) y devolver `resultados-fase-12.md` con las capturas;
con eso, el cierre de la fase y la ronda 12b con lo que digan los sondeos (D12). Después, la Fase 13: cartela automática y partir
la diagonal para fabricar el empalme del Detalle B.
