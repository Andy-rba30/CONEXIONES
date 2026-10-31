# Resumen de la sesión de los sondeos de la Fase 14 (2026-10-31): sondeos 31 y 32 e instrucciones para el instalador

Rama de trabajo **`main`** (esta sesión adelantó `main` a `claude/fase-14-opciones-o12qve`, sin conflicto: el cierre de la 13c y la
propuesta de la Fase 14 ya están en `main`). Add-in sin cambios: sigue el **0.13.2** probado en Revit. Sin código del add-in ni del MCP.
Copia corta del mensaje del chat.

## 1. Qué se hizo

- **`scripts/sondeos/31-inventario-cercha.py`** (solo lectura, `-SinTransaccion`). El ámbito es la selección de Revit más las barras y
  pilares del modelo que están **en el mismo plano de la cercha** (la caja de la selección ampliada 1,5 m en el plano y 600 mm fuera,
  y los dos extremos a menos de 600 mm del plano: así entran montantes, cordón inferior y apoyos sin seleccionarlos y quedan fuera las
  correas que cruzan el plano). Por barra: perfil con sus lados (b y Ht del tipo, la sección estructural o la designación; los lados
  se reparten con la convención de la ronda 12c según el giro de sección), clase (horizontal / vertical / inclinada), cotas, largo,
  uniones automáticas (`IsJoinAllowedAtEnd`) y, por extremo, **la cara más cercana y su hueco en mm** (o "empalme" si es colineal, o
  "libre"). Resúmenes: horizontales por cota Z con la respuesta **CORDON INFERIOR SI/NO** (Z ≈ 14 914), **montantes** y a qué cordón
  llegan, **apoyos** (extremos contra pilares o vigas que no son HSS) con la rejilla de Revit más cercana, extremos libres, uniones
  automáticas activas y el recuento por perfil. Sin selección recorre toda la categoría (solo resúmenes); con `%TEMP%\sondeo31-ids.txt`
  acepta ids a mano (ahí los `ElementId` se construyen con `System.Int64`, como en el sondeo 30 v3).
- **`scripts/sondeos/32-columnas-y-anclajes.py`** (solo lectura, `-SinTransaccion`, **sin ninguna transacción**: listar tipos por
  reflexión no la necesita, como en el sondeo 06). (1) Los **pilares estructurales**: tipo, material, lados, nivel y desfase de base y
  cabeza, cota de la base (por la caja, en las coordenadas de `node_info`, y por el nivel), largo, planta, giro, las dos rejillas más
  cercanas y **qué hay debajo**: por cajas envolventes (cimentación, suelo, armazón, muro, terreno, pilar, genérico, masa, techo,
  cubierta cuya cara superior queda entre 1 m por debajo y 50 mm por encima de la base, y si la base está *dentro* de algo) y con un
  **rayo hacia abajo** (`ReferenceIntersector` en una vista 3D). (2) Los **anclajes ya modelados como armazón** (familias con ANCLA,
  ANCHOR, ESPARRAGO, PERNO, BOLT, STUD, VARILLA: el sondeo 02 vio `ANCLAJE_Ø7-8- 22x22` × 72 y `ESPARRAGO 5-8 16x16` × 48) y, por pilar,
  cuántos tiene en su base, diámetro, largo, cuánto asoman, cuánto quedan embebidos y su reparto en planta. (3) Las categorías
  `OST_StructConnection*` que existen en la API 2027 (por `getattr`, sin dar por hecho el nombre; la que importa es
  `OST_StructConnectionAnchors`) y cuántos elementos tienen. (4) **La API de Advance Steel por reflexión**: los ensamblados cargados, las
  DLL de `AddIns\SteelConnections` con Anchor/Grout en el nombre, los tipos de todos los ensamblados de acero con **"Anchor" o "Grout"**
  (también los anidados, como `AnchorPattern+eAnchorType`) con su **cadena de herencia, constructores, métodos estáticos, métodos y
  propiedades relevantes** y los valores de las enumeraciones; los tipos `*BoltPattern*`; los miembros `*Anchor*`/`*Grout*` de cualquier
  otro tipo; y lo que haya en RevitAPI.dll. Ningún nombre de la API se da por hecho.
- **`docs/instalacion/fase-14-sondeos.md`**: seis pasos (traer `main`, `ping` y sondeo 00, sondeo 31 con la cercha seleccionada,
  sondeo 31 sin selección, sondeo 32, commit y push), unos 15 minutos, Revit abierto desde el principio (no se despliega nada), la
  salida en **`docs/fases/resultados-fase-14-sondeos.md`** con `Anota`, qué se espera de cada paso y qué devolver en el chat.
- Documentación: `CLAUDE.md` y el README nombran los sondeos 31 y 32; `docs/propuestas/fase-14-opciones.md` (sección 8, "Qué sigue") y
  `docs/fases/resumen-fase-14-propuesta.md` (pasos 2 y 3) quedan marcados como hechos.

## 2. Qué se probó en la nube

- `python3 -c "ast.parse(...)"` y `pyflakes` sobre los dos sondeos: sintaxis correcta y ningún nombre sin definir aparte de los que
  Revit inyecta (`doc`, `uidoc`, `DB`, `System`, `clr`, `print`).
- **NO PROBADO en Revit** (en la nube no hay Revit): los dos sondeos enteros. Lo que puede fallar al primer intento y cómo está
  protegido: cada bloque del sondeo 32 va en su propio `try` (si el rayo falla, la medida por cajas sigue; si una DLL no se carga, se
  dice y se sigue con las cargadas); el sondeo 31 imprime `FALLO general` con el traceback dentro de la salida. Si algo para, el
  instalador copia la salida parcial y la sesión siguiente lo corrige (v2), como se hizo con el sondeo 30.

## 3. Decisiones de esta sesión

- **`main` adelantado** a la rama de la propuesta en vez de trabajar sin ella: el encargo pedía leer una propuesta y una sección 9 que
  solo existían en esa rama; el avance es un *fast-forward* (nada se pierde ni se reescribe).
- **El sondeo 32 no abre `FabricationTransaction`**: la propuesta decía "de solo lectura", pero la reflexión sobre los ensamblados no
  la necesita y abrirla ya no sería "solo lectura". Crear un anclaje de prueba será un sondeo de la Fase 15.
- **El ámbito por plano** en el sondeo 31 (y no "la selección o todo"): con solo la selección no saldrían los montantes ni los apoyos
  que nadie seleccionó; con todo el modelo saldrían 1 300 líneas. Las dos pasadas (con y sin selección) dan el detalle de la cercha y
  los resúmenes del modelo entero.
- **Los lados del perfil con la convención de la 12c** (girada 90°, el lado en el plano es `b`): es lo que hace el add-in y lo que
  corrigió el sondeo 30 v3; el hueco a la cara se mide con eso. En un pilar no se lee el giro: se imprimen los dos lados.

## 4. Qué hacer ahora

1. Dar `docs/instalacion/fase-14-sondeos.md` al agente instalador con este prompt:

   ```
   Lee docs\instalacion\fase-14-sondeos.md del repositorio D:\Proyectos C#\CONEXIONES y ejecútalo entero, en orden, del paso 14s-1 al
   14s-6, en una sola ventana de PowerShell. No hay nada que compilar ni desplegar: Revit puede estar abierto desde el principio con la
   copia HANGAR_PRUEBA_sondeo.rvt. Reglas: usa la función Anota tal como está escrita en el 14s-1 (no crees scripts nuevos); si abres
   otra ventana, repite las tres primeras líneas del 14s-1 y vuelve a definir Anota, $salida y PYTHONIOENCODING; los pasos marcados
   (la persona) los hace la persona y tú esperas (en el 14s-3 selecciona la cercha; en el 14s-4 la deselecciona); no toques src\,
   scripts\, mcp\, config\ ni docs\fixtures\; si un paso falla, copia el error, no modifiques nada y sigue. Si un sondeo imprime FALLO,
   PARADA o TRACEBACK no es un fallo de la instalación: cópialo entero y sigue; un sondeo puede tardar hasta dos minutos, no lo
   interrumpas. Al terminar, haz el commit y el push del 14s-6 y devuélveme lo que pide la sección "Qué devolver en el chat": (1) el git
   log -1 y el addin_version del ping; (2) del 14s-3, las líneas AMBITO, CORDON INFERIOR, MONTANTES con las de debajo, el bloque 3c)
   APOYOS y la línea 3e); (3) del 14s-4, las líneas "Z ~ … mm:" de 3a, el número de montantes, el bloque 3c y la línea 3e; (4) del
   14s-5, el RESUMEN de pilares entero, la línea "pilares con anclajes" con dos o tres líneas de "Anclajes por pilar", las líneas de 3)
   que digan "no existe" o tengan más de 0 elementos, y de 4a) el nombre completo de cada tipo con Anchor/Grout con sus líneas ctor(…),
   hereda: y valores:; (5) el texto entero de cualquier FALLO, PARADA, TRACEBACK o ventana de error.
   ```

2. Después, en Claude Code (sesión nueva en la nube), el prompt del paso 4 de `docs/fases/resumen-fase-14-propuesta.md`, sección 3:

   ```
   Lee CLAUDE.md, docs/propuestas/fase-14-opciones.md (sección 8: las decisiones), docs/fases/resultados-fase-14-sondeos.md y
   docs/prompts/fase-13.md. Escribe docs/prompts/fase-14.md (prompt y alcance de la Fase 14) con la estructura de docs/prompts/fase-13.md:
   decisiones ya tomadas, decisiones de la persona, alcance, reglas técnicas, pruebas en la nube, instrucciones para el instalador,
   definición de hecho, fuera de alcance, dudas y orden de recorte. Si algún sondeo paró, corrígelo primero (v2) y deja la ronda corta
   de repetirlo. Sin código. Termina con commit, push y un resumen corto.
   ```
