# Resumen del cierre de la Fase 12 (2026-10-21): la 0.12.0 contrastada en Revit y la ronda 12b (add-in 0.12.1)

Rama `main`. Add-in **0.12.1** (pendiente de desplegar en el PC: la última instalada es la 0.12.0). Informe:
`docs/fases/fase-12.md`, sección 7. Instalación de la ronda: `docs/instalacion/fase-12b.md` (unos 25 minutos; no crea
conexiones). Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-12.md`, add-in 0.12.0, 2026-10-21)

- **Funcionó todo lo programado** (`fase-12.md` 7.1): `node_info` con `kind: node` / `kind: splice`; **Conectar en el nudo en K**
  (arranque `direct_weld`, croquis con la excentricidad y la marca a 330, dos avisos `MEMBER_ECCENTRIC`, **Crear** con 2 retiros y
  2 soldaduras, `END_CUT_APPROXIMATED`, guardado como "Nudo K cordon superior"); **Conectar en el empalme** (arranque Detalle C,
  12 pernos, **Crear** con las dos planchas octogonales de Advance Steel, 4 patrones de N × 1 atravesándolas, retiros de 12,3,
  `BOLT_NUTS_APPROXIMATED`); **Planificar lote** con las 64 barras: 41 nudos (16 Detalle D + 17 soldaduras directas + 7 empalmes C
  + 1 empalme I), N9 sobre N9E, 41 etiquetas y 16 fantasmas; **Crear 40 conexiones en 6,3 s, 0 fallidas**; `batch_plan` sobre
  N9 da los dos nudos; Borrar el lote (40, 285 elementos) y los dos `delete` con las barras restauradas; sondeos 12 y 13 a cero;
  el log sin ningún evento de fallo del add-in.
- **Sondeo 23**: la fila de 4 pernos como patrón 4 × 1 **escribe** (`NumberOfScrews 4`) y **`Connect` une el patrón a las dos
  planchas** (`kOnSite`); los pernos sueltos también; la tuerca a ambos lados **no está en la API** (queda D9).
- **Sondeos 22 y 24**: el camino de Revit no corta (`CanSetEndReference = False`), y los dos sondeos **listaron las firmas buenas
  de Advance Steel pero no las intentaron**: `BeamShortening(eEnd, Double)` con `AngleOnY` / `AngleOnZ` (el corte plano
  inclinado), `BeamMultiContourNotch`, **`WeldLine(Point3d[], Vector3d, Vector3d)`** y `WeldPoint(Point3d, Vector3d, Vector3d)`.
- **Tres números distintos de lo esperado**: la excentricidad real del nudo en K es **54,9 mm** (no 34; desvíos ∓27,4); el add-in
  lee el HSS12X8X1/2 como **203,2 × 203,2** (por eso el límite salió 101,6 en vez de 152,4 y la plancha de 430 pudo salir girada:
  la persona la vio en vertical); y `--puente` **36/38** porque el `validate` del add-in no devolvía `kind` ni `eccentricity_mm`.
- Lo normal de siempre: la `FabricationTransaction` de borrar no se abre justo después de crear (la reserva `Document.Delete`
  funciona, como desde la Fase 9); queda el marcador gris de N5 porque el plan no se descartó.
- **Sin anotar**: una sola barra y media cercha desde Conectar, el error del borde a 10 junto al campo, el cuadro Sí/No del
  empalme + nudo en K.

## 2. Qué se corrigió (ronda 12b, add-in 0.12.1)

- `validate` (y `catalog_apply`) devuelven `calculated_values.kind` y `eccentricity_mm`, como el simulador y el contrato → el
  puente debe dar 38/38.
- La excentricidad **54,9** y los desvíos **∓27,45** en el fixture K, en el nudo sintético de las pruebas (`KNode34` →
  `KNode55`), en el simulador, la guía, el manual de `conn_get_node_info` y el contrato.
- **Sondeos nuevos** que sí intentan lo que falta: `25-corte-advance-steel.py` (la viga de Advance Steel de la diagonal por el
  puente `SteelElementProperties` y `BeamShortening` + `AddFeature` con el ángulo; `BeamMultiContourNotch` como segunda vía),
  `26-soldadura-weldline.py` (`WeldLine` cerrada sobre la huella, `Connect` y `WriteToDb`; `WeldPoint`) y `27-perfil-cordon.py`
  (solo lee: los parámetros de sección del tipo, `Cross-Section Rotation` y la caja real del sólido). Los dos primeros cancelan
  por defecto; con un archivo marca en `%TEMP%` hacen `Commit` para verlo y capturar.
- **No se toca la fabricación** (sigue la de la 0.12.0) ni la lectura del perfil: eso es la 12c, con lo que digan los sondeos.

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **268/268**, `py_compile` correcto (25, 26 y 27 incluidos), simulador **82/82** y
`probar_conexiones.py` **35/35** contra el simulador (`addin=0.12.1`). `kind` / `eccentricity_mm` en el `validate` del add-in y
los sondeos: **NO PROBADOS en la nube**, se prueban en el PC.

## 4. Qué hacer ahora

1. Pasar al instalador `docs\instalacion\fase-12b.md` entero (unos 25 minutos, sobre la copia, Revit cerrado al empezar; no crea
   conexiones): deploy 0.12.1, descartar el plan viejo, sondeos **27, 26 y 25** (en ese orden), `validate` de los fixtures y
   `--puente` 38/38.
2. **(la persona)**, si quiere ver el corte o la soldadura: crear el archivo marca que dice la instalación y repetir el sondeo;
   captura y Ctrl+Z. Y de paso, sin crear nada: Conectar con una sola barra y Conectar con el nudo N9 completo (empalme + nudo
   en K: la ventana del empalme y después el cuadro Sí/No).
3. Devolver `docs\fases\resultados-fase-12b.md` y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-12.md (secciones 7.2, 7.5 y 7.7) y docs/fases/resultados-fase-12b.md. Cierra la ronda 12b:
   contrasta los sondeos 25, 26 y 27 con lo esperado en 7.5 y el --puente 38/38, y programa la ronda 12c con lo que digan:
   RevitModelFacts con el ancho y el canto del HSS12X8 bien leídos (y el giro de la plancha si hace falta), DirectWeldBuilder
   con BeamShortening si cortó, CreateWelds con WeldLine si escribió (reserva DirectShape si no). Actualiza el informe, el
   README y las propuestas. No empieces la Fase 13. Termina con compilación sin avisos, pruebas en verde, commit, push y un
   resumen corto.
   ```
