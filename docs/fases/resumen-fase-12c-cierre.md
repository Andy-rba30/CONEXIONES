# Resumen del cierre de la ronda 12c (2026-10-23): la 0.12.2 contrastada en Revit y la ronda 12d (add-in 0.12.3)

Rama `main`. Add-in **0.12.3** (pendiente de desplegar en el PC: la última instalada es la 0.12.2). Informe:
`docs/fases/fase-12.md`, sección 9. Instalación de la ronda: `docs/instalacion/fase-12d.md` (unos 20 minutos; crea y borra una
conexión sobre la copia). Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-12c.md`, add-in 0.12.2, 2026-10-23)

- **El perfil del cordón se lee bien**: `node_info` da `width_mm 203.2`, `height_mm 304.8`, `section_rotation_deg 90`; el
  `validate` del fixture K dice "admitida hasta **152.4** mm". La lectura del perfil (7.2.c) queda cerrada.
- **Las `WeldLine` se materializan en los dos tipos**: el nudo en K crea **2** elementos `Welds` (`missing 0`), el empalme **8** (2
  planchas, 4 patrones, 2 soldaduras, `missing 0`, huella 304,8 × 203,2); la reserva por recuento no saltó. **El 430 va en
  horizontal** (captura de frente). Puente **38/38**.
- **El sondeo 28 no encontró la viga de Advance Steel**, pero **no la buscó de verdad**: su recorrido de los objetos del modelo
  nunca corrió, porque `DatabaseManager.GetModelObjectIds` devuelve la lista por un **parámetro de salida** y el sondeo solo
  llamaba a métodos sin parámetros. `BeamShortening` sigue sin intentarse. La `WeldLine` sin plancha del sondeo, con `Commit`,
  dio 2 elementos `Welds`.
- **`REVIT_WARNING` uno por elemento de Advance Steel** (2 y 8), sin texto en el cuadro ni en el log: son advertencias del propio
  Revit al confirmar; no sabemos qué dicen.
- Las soldaduras nativas "no tienen cuerpo" y solo se ven en nivel **Fino**: es su representación en Revit, no un fallo.

## 2. Qué se programó (ronda 12d, add-in 0.12.3)

- **Las advertencias de Revit al log**: `OperationScope` escribe cada fallo como `revit_failure` (severidad, texto, elementos,
  definición, transacción) y el aviso `REVIT_WARNING` / `REVIT_ERROR` lleva "[elementos: …]". Solo registrar; filtrar después.
- **Sondeo 29** (`29-viga-objetos-advance-steel.py`): `GetModelObjectIds` con el parámetro de salida (sin filtro, solo `Beam`,
  base actual), `Open` de cada `ObjectId`, recuento de tipos, las primeras vigas con `Handle` e inicio y fin en mm (para comparar
  el origen con el de Revit), la viga por Guid o por geometría, `BeamShortening` + `AddFeature` y la `WeldLine` conectada; cancela
  salvo `%TEMP%\sondeo29-commit.txt`. **Es el último intento**: si tampoco aparece, el retiro por el eje queda como definitivo.
- **Nada más cambia**: ni la fabricación (soldaduras nativas y reserva se quedan), ni el perfil, ni el Core. Versión 0.12.3.
- Documentación: `fase-12.md` 9, README, `CLAUDE.md`, guía (las soldaduras como símbolos; los `REVIT_WARNING`), propuestas.

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **291/291**, `py_compile` correcto (29 incluido), simulador **82/82** y
`probar_conexiones.py` **35/35** (`addin=0.12.3`). Las líneas `revit_failure` y el sondeo 29: **NO PROBADOS en la nube**, se
prueban en el PC.

## 4. Qué hacer ahora

1. Pasar al instalador `docs\instalacion\fase-12d.md` entero (unos 20 minutos, sobre la copia, Revit cerrado al empezar): deploy
   0.12.3, **sondeo 29**, crear y borrar el nudo en K (lo crea la persona desde Conectar; el instalador lista y borra), `--puente`
   38/38 y el log con `revit_failure`.
2. **(la persona)**, solo si el sondeo 29 dijo `BeamShortening: AddFeature OK`: el paso 12d-4 con el archivo marca, la captura
   `fase12d-01-corte.png` y Ctrl+Z.
3. Devolver `docs\fases\resultados-fase-12d.md` y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-12.md (secciones 9.2, 9.5 y 9.7) y docs/fases/resultados-fase-12d.md. Cierra la ronda 12d:
   contrasta el sondeo 29 y las líneas revit_failure del log con lo esperado en 9.5 y decide: si el sondeo 29 encontró la viga
   de Advance Steel y BeamShortening cortó, programa la ronda 12e con el corte inclinado en DirectWeldBuilder y retira
   END_CUT_APPROXIMATED; si no la encontró, deja el retiro por el eje como definitivo, anótalo en las propuestas y no abras más
   sondeos sobre la viga. Con el texto de las advertencias de Revit, decide si se filtran del cuadro o se quedan. Actualiza el
   informe, el README y las propuestas. No empieces la Fase 13. Termina con compilación sin avisos, pruebas en verde, commit,
   push y un resumen corto.
   ```
