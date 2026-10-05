# Resumen de la ronda 8b (copia del mensaje final del chat)

Fecha: 2026-10-05. Rama `main`. Add-in **0.8.1**. Informe: `docs/fases/fase-8.md`, sección 7.

**Qué pasó en el PC (Fase 8, 2026-10-04).** Las marcas, la ventana del plan y el descarte funcionaron, pero de 53 barras
salieron 97 nudos (93 `untyped`, 4 `no_match`, **0 `ready`**). Reconstruyendo los 53 ejes a partir de los puntos de
trabajo del plan se ve la causa: las diagonales de la cercha real terminan en la **cara** del cordón, no en su eje (la
1249630 a 58,8 mm por encima, la 1249636 a 33,1 por debajo, la 1249631 a 14,3), aunque sus **ejes** cortan el eje del
cordón 1249510 a menos de 4 mm entre sí. La agrupación de extremos a 10 mm no podía juntarlas.

**Qué se corrigió (0.8.1).**

- **Detección** (`Core/Batch/NodeDetector.cs`): cada extremo se lleva al **corte de su eje con el eje del cordón** (o de la
  barra vecina que llega al mismo nudo) si está a menos del **alcance de cara**: medio canto de cada barra más 10 mm
  (HSS3X3 + HSS 2-1/2 = 79,85 mm; 40 mm por barra si el modelo no da medidas; `node_face_reach_mm` en `config/catalog.json`
  lo fija a mano, 0 = según el canto). Los cortes se agrupan a 10 mm, y los que caen sobre el mismo cordón a menos de un
  alcance (nudos en K con excentricidad). `members[].end_gap_mm` dice cuánto se queda corta cada barra. Aviso nuevo por
  nudo `NODE_CHORD_NOT_CONTINUOUS` cuando ninguna barra atraviesa el nudo (extremo de cercha o cordón que falta en la
  selección).
- **Sondeo 17**: `AttributeError: Name` (IronPython con `FamilySymbol.Name`) → `DB.Element.Name.__get__`. **Sondeo 18**
  nuevo: mide en Revit la distancia de cada extremo al eje vecino (la misma regla del Core).
- **Paso 8-5**: `$pid` es el número de proceso de PowerShell (por eso `PLAN_NOT_FOUND` con `35920`); ahora `$plan`.
- **Marcadores**: ya no escriben `Marca` (Revit avisaba "Elements have duplicate Mark values"); el nombre va en `Name` y
  al principio de `Comentarios`.
- **Editar nudo** solo se activa con nudos que tienen especificación (`ready` o `invalid`); con 0 `ready` no había nada que
  editar. El botón en gris ahora lo explica al pasar el ratón.

**Probado en la nube.** `dotnet build` sin avisos; `dotnet test` **158/158** (+8: la **cercha real del Hangar** como fixture,
`Tests/Fakes/HangarTruss.cs`, da 53 nudos con **10 `ready`** con la plantilla oficial del catálogo: 5 `same` y 5 `mirror_x`,
el Detalle D a 136,9 / 44,4 / −135,6 con sus diagonales a 84,5 / 19,6 / 48,1 mm del punto de trabajo; con la regla
antigua la misma cercha da los 97 nudos del PC); simulador **45/45**; `probar_conexiones.py` **26/26** contra el simulador.

**NO PROBADO (necesita Revit).** El canto real que lee `RevitModelFacts` en las familias del Hangar, la geometría exacta
(el fixture viene de los puntos de trabajo redondeados a 0,1 mm), la validación en Revit de los otros nueve nudos, el
sondeo 17 corregido, el 18, la ventana con **Editar nudo** y que no salga el aviso de `Marca`. Todo en
`docs/instalacion/fase-8b.md` (unos 40 minutos, misma cercha; se espera `ready: 10, no_match: 26, untyped: 17` con los
mismos 53 elementos, más si se seleccionan también los cordones superior e inferior).

**Qué haces tú ahora** (`docs/fases/fase-8.md`, sección 6, paso 5): cerrar Revit, pasar `docs\instalacion\fase-8b.md` al
instalador, seleccionar la misma cercha (si puedes, con los cordones superior e inferior) y mirar lo de la sección 7.5.
Después, la sesión de cierre con el prompt de la sección 6 y, solo entonces, la Fase 9.

Archivos: `docs/fases/fase-8.md` (sección 7), `docs/instalacion/fase-8b.md`, `docs/fases/resumen-fase-8b.md`,
`scripts/sondeos/17-marcas-plan.py`, `scripts/sondeos/18-extremos-cara.py`, `src/MotorConexiones.Tests/Fakes/HangarTruss.cs`,
README (estado, garantías, sección 12, errores), `docs/guide.md`, `mcp/CONTRATO-conn.md`, `config/catalog.json`.
