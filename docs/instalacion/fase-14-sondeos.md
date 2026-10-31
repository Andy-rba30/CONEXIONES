# Ronda de sondeos de la Fase 14: inventario de la cercha (sondeo 31) y pilares, bases y anclajes (sondeo 32)

Objetivo: ejecutar dentro de Revit 2027 dos sondeos de **solo lectura** y guardar su salida en `docs/fases/resultados-fase-14-sondeos.md`.
Con esa salida se escribe `docs/prompts/fase-14.md` (decisión D14 de `docs/propuestas/fase-14-opciones.md`, sección 8). **No hay nada
que compilar ni desplegar**: el add-in sigue siendo el **0.13.2** de la ronda 13c y los sondeos no crean, no modifican y no abren
transacciones; no queda nada que deshacer. Unos **15 minutos** sobre `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` (la copia, por
costumbre: los sondeos tampoco tocarían el original).

- El **sondeo 31** (`scripts\sondeos\31-inventario-cercha.py`) responde: si existe el **cordón inferior** como barra (Z ≈ 14 914),
  cuántos **montantes** hay y a qué cordón llegan con qué hueco, qué barras terminan contra **pilares o vigas** de apoyo (el eje B) y
  si alguna barra tiene **uniones automáticas** activas. Se corre **dos veces**: con la cercha seleccionada (detalle barra a barra) y
  sin nada seleccionado (resúmenes de todo el modelo).
- El **sondeo 32** (`scripts\sondeos\32-columnas-y-anclajes.py`) responde: qué **pilares** hay, a qué cota está su base y **qué hay
  debajo** (zapata, losa, viga, nada), cómo están modelados hoy los **anclajes** (en la Fase 0 el sondeo 02 vio familias de armazón
  `ANCLAJE_Ø7-8- 22x22` y `ESPARRAGO 5-8 16x16`), qué categorías de conexión de Revit existen y qué tipos de la API de Advance Steel
  llevan "Anchor" o "Grout" en el nombre, con sus constructores. No hace falta seleccionar nada.

Reglas: las de siempre. Si un paso falla, no modifiques ningún archivo del repositorio ni de la extensión (**el instalador no toca
`src\`, `scripts\` ni `mcp\`**); copia el error y sigue con el siguiente. No crees scripts nuevos; usa `Anota` en la ventana de
PowerShell. Todos los comandos en la misma ventana, en orden; si abres otra, repite las tres primeras líneas del paso 14s-1 y vuelve a
definir `Anota`, `$salida` y `PYTHONIOENCODING`. **Revit lo abre y lo cierra la persona.** Los pasos marcados **(la persona)** se hacen
en Revit; los marcados **(instalador)** van en PowerShell. Si un sondeo imprime `FALLO`, `PARADA` o `TRACEBACK`, **no es un fallo de la
instalación**: cópialo entero (la salida parcial vale) y sigue. Un sondeo puede tardar hasta dos minutos: no lo interrumpas.

## Antes de empezar (lo decide la persona)

- Revit 2027 **abierto** con la copia `HANGAR_PRUEBA_sondeo.rvt` (es la única vez que no hace falta cerrarlo: no se despliega nada),
  pyRevit cargado con Routes activo y la extensión `revit-mcp` con `conexiones.py` instalado (lo dejó la ronda 13c; la ruta
  `-SinTransaccion` lo necesita).
- La copia sin conexiones del add-in ni barras partidas (la 13c terminó con `list` 0 y `splits_count: 0`). Si no es así, da igual para
  estos sondeos: solo leen.
- Una **vista 3D** activa (la de siempre). El sondeo 32 la usa para lanzar un rayo hacia abajo desde cada base de pilar; sin vista 3D
  lo dice y se salta esa medida.

### 14s-1. Traer `main`, archivo de resultados **(instalador; Revit puede estar abierto)**

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git status --short
git pull --no-rebase origin main
git log -1 --oneline
$salida = "docs\fases\resultados-fase-14-sondeos.md"
"# Resultados de la ronda de sondeos de la Fase 14 (sondeos 31 y 32)`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
function Anota($titulo, $bloque) {
    "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
    $r = (& $bloque 2>&1 | Out-String)
    Write-Output $r
    $r | Add-Content -Encoding UTF8 $salida
    "``````" | Add-Content -Encoding UTF8 $salida
}
$env:PYTHONIOENCODING = "utf-8"
Anota "14s-1 git" { git log -1 --oneline; git status --short; Test-Path scripts\sondeos\31-inventario-cercha.py; Test-Path scripts\sondeos\32-columnas-y-anclajes.py }
```

Se espera: `git log -1` con "Fase 14: sondeos 31 y 32 …" y los dos `Test-Path` en `True`. Si `git pull` da conflicto, **para** y copia la
salida.

### 14s-2. Comprobar que Revit responde **(instalador)**

```powershell
Anota "14s-2 ping" { .\scripts\conn-call.ps1 ping }
Anota "14s-2 sondeo 00" { .\scripts\revit-exec.ps1 -File scripts\sondeos\00-version.py }
```

Se espera: `addin_version 0.13.2` y, del sondeo 00, el título del documento (la copia), `Armazon estructural (instancias)` en torno a
1 300 y `Pilares estructurales (instancias) 145` (son los números de la Fase 0; si cambian, no pasa nada: se anotan).

### 14s-3. Sondeo 31 con la cercha seleccionada **(la persona selecciona; instalador)**

**(la persona)** En la vista 3D selecciona **la cercha de la ronda 8c**: el cordón central en sus ocho tramos, sus 48 diagonales y los
ocho tramos del cordón superior (64 elementos; en Propiedades debe decir *Structural Framing (64)* o parecido). **Si ves montantes
verticales o un cordón inferior, selecciónalos también**: da igual seleccionar de más, el sondeo añade por su cuenta todo lo que está
en el mismo plano de la cercha (a menos de 600 mm) dentro de la caja de la selección ampliada 1,5 m. Lo que importa es que **no quede
nada de otra cercha** (la gemela está a más de 30 m: no hay riesgo). Deja la selección hecha y avisa.

```powershell
Anota "14s-3 sondeo 31 (cercha seleccionada)" { .\scripts\revit-exec.ps1 -File scripts\sondeos\31-inventario-cercha.py -SinTransaccion -TimeoutSec 900 }
```

Se espera, en este orden: la línea `1) En el modelo: … Seleccionados: 64 elementos (seleccion de Revit)`; `plano de la cercha: Y = …`
(la cercha de la 13c está en Y ≈ −17 196) y `AMBITO: 64 seleccionadas + N vecinas en el mismo plano`; el bloque `2)` con **una línea por
barra** (las vecinas llevan `*`) y en cada extremo `-> [id] tipo cara … hueco … mm`, `empalme (colineal)` o `libre`; el bloque `3)`:
`3a` las horizontales por cota (Z ≈ 19 950 el cordón superior, 17 400 el central) y la línea **`CORDON INFERIOR … SI`** o **`NO`**;
`3b` **`MONTANTES … : N`** con a qué cordón llega cada uno; `3c` los extremos contra pilares o vigas con la rejilla más cercana; `3d` los
extremos libres; `3e` las uniones automáticas activas (en la 13c estaban desactivadas); `3f` el recuento. Devuelve la salida **entera**.

Si la persona no puede seleccionar (por ejemplo, instalación a distancia), el sondeo acepta los ids en un archivo; con los del nudo
del Detalle D y del nudo en K el ámbito queda pequeño, pero sirve para comprobar que corre:

```powershell
Set-Content $env:TEMP\sondeo31-ids.txt "1249510`n1249630`n1249631`n1249632`n1249636`n1245533"
Anota "14s-3 sondeo 31 (ids por archivo)" { .\scripts\revit-exec.ps1 -File scripts\sondeos\31-inventario-cercha.py -SinTransaccion -TimeoutSec 900 }
Remove-Item $env:TEMP\sondeo31-ids.txt
```

### 14s-4. Sondeo 31 sin selección: todo el modelo **(la persona deselecciona; instalador)**

**(la persona)** Pulsa **Esc** dos veces hasta que no quede nada seleccionado (Propiedades debe decir *Vista 3D*). Avisa.

```powershell
Anota "14s-4 sondeo 31 (todo el modelo)" { .\scripts\revit-exec.ps1 -File scripts\sondeos\31-inventario-cercha.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `AMBITO: sin seleccion, toda la categoria de armazon estructural (… barras); se imprimen solo los resumenes` y el bloque `3)`
del modelo entero: todas las cotas con barras horizontales (si hay cordones inferiores en alguna cercha, salen aquí), todos los
montantes, todos los extremos contra pilares y vigas (con sus rejillas), las uniones automáticas activas en todo el modelo y el
recuento por perfil. Puede tardar un minuto o dos.

### 14s-5. Sondeo 32: pilares, bases y anclajes **(instalador; sin seleccionar nada)**

```powershell
Anota "14s-5 sondeo 32" { .\scripts\revit-exec.ps1 -File scripts\sondeos\32-columnas-y-anclajes.py -SinTransaccion -TimeoutSec 900 }
```

Se espera: `1) PILARES ESTRUCTURALES en el modelo: 145` y una línea por pilar con `base … = Z`, las rejillas (`B a … mm / 3 a … mm`) y
`DEBAJO: [id] tipo (categoria) cara superior Z …: … mm bajo la base` (o `nada hasta 1000 mm por debajo`) más `| rayo: …`; el
`RESUMEN de pilares por tipo, material, cota de base y que hay debajo`; `2) ANCLAJES MODELADOS COMO ARMAZON` con `ANCLAJE_… 72
ejemplares` y `ESPARRAGO … 48` (si siguen en la copia), y **`Anclajes por pilar`** con cuántos tiene cada uno, su diámetro, cuánto asoman
y cuánto quedan embebidos y su reparto en planta; `3)` las categorías `OST_StructConnection*` que existen y cuántos elementos tienen
(se espera 0 en casi todas; lo interesante es si **`OST_StructConnectionAnchors` existe**); `4a)` los tipos con **Anchor** o **Grout**
(en la Fase 1 el sondeo 06 vio `Autodesk.AdvanceSteel.Modelling.AnchorPattern` y su enumeración `eAnchorType`) con **`constructores`**,
`hereda:` (si cuelga de `ScrewBoltPattern`, el anclaje es un patrón de pernos más), los métodos relevantes y las propiedades; `4b)` los
tipos `*BoltPattern*`; `4c)` los miembros `*Anchor*`/`*Grout*` de otros tipos; `4d)` lo que haya en RevitAPI.dll. Devuelve la salida
**entera** aunque sea larga. Si el rayo da `ERROR` en todos los pilares, cópialo: la medida por cajas (`DEBAJO`) vale igual.

Si hace falta mirar solo unos pilares (segunda pasada, opcional): `Set-Content $env:TEMP\sondeo32-ids.txt "ID1`nID2"` y repetir;
después `Remove-Item $env:TEMP\sondeo32-ids.txt`.

### 14s-6. Commit y push **(instalador)**

```powershell
Select-String -Path $salida -Pattern "^## 14s-" | Select-Object LineNumber, Line
git add docs\fases\resultados-fase-14-sondeos.md
git commit -m "Fase 14: resultados de los sondeos 31 y 32 (inventario de la cercha, pilares, bases y anclajes)"
git push origin main
git status --short
git log -1 --oneline
```

Se espera: las secciones `14s-1` a `14s-5` en el archivo y el push a `main` sin errores. **(la persona)** Puede cerrar Revit sin guardar
(los sondeos no cambiaron nada; si Revit pregunta si quieres guardar, es por otra cosa: di que no).

## Qué devolver en el chat (además del archivo)

1. El `git log -1 --oneline` del 14s-1 y el `addin_version` del ping.
2. Del **14s-3**: la línea `AMBITO: …`; la línea **`CORDON INFERIOR …`**; la línea **`MONTANTES … : N`** y las de debajo (a qué cordón
   llega cada montante y con qué hueco); todo el bloque **`3c) APOYOS`**; la línea **`3e) UNIONES AUTOMATICAS …`**.
3. Del **14s-4**: las líneas `Z ~ … mm:` de `3a`, el número de montantes de `3b`, el bloque `3c` y la línea `3e`.
4. Del **14s-5**: el `RESUMEN de pilares …` entero; la línea `pilares con anclajes: …` y dos o tres líneas de `Anclajes por pilar`; las
   líneas de `3)` que digan `no existe` o tengan más de 0 elementos; de `4a)`, el nombre completo de cada tipo con Anchor/Grout, sus
   líneas `ctor(…)` y `hereda: …`, y los `valores:` de las enumeraciones.
5. El texto de cualquier `FALLO`, `PARADA`, `TRACEBACK` o ventana de error, entero.
