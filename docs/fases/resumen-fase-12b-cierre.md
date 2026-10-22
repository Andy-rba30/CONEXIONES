# Resumen del cierre de la ronda 12b (2026-10-22): los sondeos 25, 26 y 27 contrastados y la ronda 12c (add-in 0.12.2)

Rama `main`. Add-in **0.12.2** (pendiente de desplegar en el PC: la última instalada es la 0.12.1). Informe:
`docs/fases/fase-12.md`, sección 8. Instalación de la ronda: `docs/instalacion/fase-12c.md` (unos 35 minutos; crea y borra dos
conexiones sobre la copia). Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-12b.md`, add-in 0.12.1, 2026-10-21)

- **Sondeo 27 (el perfil)**: el tipo HSS12X8X1/2 no tiene ningún `STRUCTURAL_SECTION_COMMON_*`; guarda **`Ht` = 304,8**,
  **`b` = 203,2** y `t` = 11,81 con nombre corto; el ejemplar lleva **Cross-Section Rotation = 90°**; el sólido mide **203,2 en el
  plano de la cercha y 304,8 perpendicular**. El add-in leía 203 × 203 porque buscaba `d`/`h`/`H`/`Height` (no `Ht`) y, sin
  canto, lo infería del "8" de la designación.
- **Sondeo 26 (la soldadura)**: **`WeldLine(Point3d[], Vector3d, Vector3d)` se escribe** (`WriteToDb OK`, `Length` 254, `IsClosed`,
  espesor y gargantas 5), conectada a la plancha de apoyo con `kInShop`; `WeldPoint` también; nada queda al cancelar.
- **Sondeo 25 (el corte)**: la **viga de Advance Steel de la diagonal no se encontró** (Revit da el Guid, pero en ASObjectsMgd
  no hay ningún estático con `Guid` ni lista de `ObjectId`), así que **`BeamShortening` no se intentó**: el corte inclinado
  sigue sin decidir.
- **Puente 38/38**, `validate` del K con `kind: node` y 54,9 (el aviso dice "admitida hasta 101,6": el síntoma del perfil), del
  C con `kind: splice`; `node_info` del K con 203,2 × 203,2 (lo esperado en la 0.12.1); plantillas 2; el log sin fallos del add-in.
- **Lo que faltaba por ver (la persona)**: una sola barra pide pinchar (y abrió el Detalle D con su plantilla), media cercha manda
  a Planificar lote, el nudo N9 abre primero el empalme y después el cuadro Sí/No del nudo en K (captura literal), y el borde a
  10 da cuatro `BOLT_EDGE_DISTANCE_TOO_SMALL` junto al campo y en la tabla.
- **La plancha de 430 no estaba girada**: el constructor pone el 430 perpendicular al plano de la cercha (el global ±Y) y la
  captura `fase12-06`, mirada con calma, lo confirma (asoma 63 sobre el tubo, columnas de 2 pernos a los lados). Lo que sí
  estaba mal era la soldadura plancha-tubo, dibujada sobre 203 × 203.
- De paso: el 27 cayó a la primera sin `-SinTransaccion` (`NameError: uidoc`); el marcador de N5 ya no estaba (la copia se cerró
  sin guardar); el `discard` del plan viejo devolvió 0 porque Revit se había reiniciado.

## 2. Qué se programó (ronda 12c, add-in 0.12.2)

- **`RevitModelFacts`** lee el ancho nominal (`b`…) y el canto nominal (`d`, `h`, **`Ht`**, `Height`, `Depth`…), si faltan la
  designación AISC (canto × ancho × pared, `HssDesignation`), y el giro `STRUCTURAL_BEND_DIR_ANGLE`; con `SectionOrientation`
  deja **`width_mm` = lado en el plano** (203,2) y **`height_mm` = lado perpendicular** (304,8), que es lo que el Core, el
  simulador y las pruebas ya suponían. `node_info` suma `section_rotation_deg`. El límite de la excentricidad pasa a 152,4, la
  huella del empalme a [304,8, 203,2] y la soldadura plancha-tubo a 305 × 203.
- **`CreateWelds` con `WeldLine`** de Advance Steel: cada cadena de segmentos (`WeldChain`) es una polilínea con el filete en
  `Thickness` y las gargantas, `IsClosed`, `Connect(kInShop)` a las planchas de la misma sesión sobre las que cae y `WriteToDb`;
  en la soldadura directa (sin plancha) se escribe sin `Connect`. Al confirmar, si faltan tantos elementos como soldaduras,
  se crean con DirectShape y se avisa (`REVIT_WARNING`); el log lleva `advance_steel_weld_written` y `missing`.
- **El corte inclinado no cambia** (`END_CUT_APPROXIMATED` con los textos al día): sin viga no hay `BeamShortening`.
- **Sondeo 28** (`28-viga-advance-steel.py`): busca la viga en **todos** los ensamblados de acero (lista los estáticos de
  `SteelElementProperties`, los tipos `DatabaseManager`/`DocumentManager`/`ObjectId`, carga `ASCADLinkMgd` si falta, prueba
  estáticos con `Guid`/`String`, recorre listas de `ObjectId` por Guid y por geometría), corta con `BeamShortening` si la
  encuentra y escribe una `WeldLine` sin plancha; cancela al final (`%TEMP%\sondeo28-commit.txt` para verlo).
- De paso: `ProfileMatcher.NormalizeDesignation` convertía mal `HSS2-1/2X2-1/2X3/16` (arreglado, con la prueba antigua
  corregida), el `uidoc` del sondeo 27 protegido, la huella del tubo en validador, croquis y arranque ya no adivina con el mayor
  y el menor.

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **291/291** (23 pruebas nuevas en `Fase12cTests.cs`), `py_compile` correcto (28 incluido),
simulador **82/82** y `probar_conexiones.py` **35/35** (`addin=0.12.2`). La lectura del perfil en Revit, las `WeldLine` del add-in
y el sondeo 28: **NO PROBADOS en la nube**, se prueban en el PC.

## 4. Qué hacer ahora

1. Pasar al instalador `docs\instalacion\fase-12c.md` entero (unos 35 minutos, sobre la copia, Revit cerrado al empezar): deploy
   0.12.2, `node_info` y `validate` del K (203,2 × 304,8, giro 90, límite 152,4), **sondeo 28**, crear y borrar el nudo en K y el
   empalme (lo hace la persona desde Conectar; el instalador lista y borra), `--puente` 38/38 y el log.
2. **(la persona)**: dos capturas, `fase12c-01-soldadura-k.png` (la soldadura del nudo en K) y `fase12c-02-empalme-frente.png`
   (el empalme mirado por el eje del cordón: el 430 en horizontal y la soldadura sobre la plancha).
3. Devolver `docs\fases\resultados-fase-12c.md` y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-12.md (secciones 8.2, 8.5 y 8.7) y docs/fases/resultados-fase-12c.md. Cierra la ronda 12c:
   contrasta el sondeo 28, la lectura del perfil (node_info y validate del K) y las WeldLine del nudo en K y del empalme con lo
   esperado en 8.5, y decide: si la viga de Advance Steel apareció y BeamShortening cortó, programa la ronda 12d con el corte
   inclinado en DirectWeldBuilder; si la WeldLine sin plancha se materializó, quita la reserva por recuento; si no, deja la
   soldadura directa en DirectShape sin aviso. Actualiza el informe, el README y las propuestas. No empieces la Fase 13.
   Termina con compilación sin avisos, pruebas en verde, commit, push y un resumen corto.
   ```
