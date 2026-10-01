# Instalación y pruebas de la Fase 3 (instrucciones para el agente instalador)

Objetivo: instalar el add-in de la Fase 3 y probar de punta a punta, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`,
las operaciones del puente (`ping`, `guide`, `types`, `schema`, `find_profile`, `node_info`, `validate`, `preview`,
`create`, `list`, `get`, `delete`) con el fixture `docs\fixtures\detalle-D-confirmado.json`, y el botón de la cinta.
Lo más importante que hay que averiguar: si la conexión se crea con **Advance Steel** (categorías Plates/Bolts) o si
el add-in tuvo que recurrir a **DirectShape**, y por qué. Nada se hace sobre el modelo original.

Devuelve la salida literal de cada paso, incluidos los errores, y las capturas indicadas.

## Antes de empezar (lo decide la persona)

- Revit 2027 **cerrado** antes del paso 5 (`deploy.ps1` copia la DLL).
- pyRevit con **Routes** activo y la extensión `revit-mcp` con `conexiones.py` instalado (ya quedó en la Fase 1).
- La copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt` existe (Fase 1). Sus IDs del nudo son los del fixture.
- Vale Windows PowerShell 5.1 o PowerShell 7. Todos los comandos van en la misma ventana, en orden.

## Pasos

1. Preparar PowerShell y el archivo de resultados (la función `Anota` guarda cada salida):

   ```powershell
   Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
   cd "D:\Proyectos C#\CONEXIONES"
   $salida = "docs\fases\resultados-fase-3.md"
   "# Resultados de la Fase 3`n`nFecha: $(Get-Date -Format s)`n" | Set-Content -Encoding UTF8 $salida
   function Anota($titulo, $bloque) {
       "`n## $titulo`n`n``````text" | Add-Content -Encoding UTF8 $salida
       $r = (& $bloque 2>&1 | Out-String)
       Write-Output $r
       $r | Add-Content -Encoding UTF8 $salida
       "``````" | Add-Content -Encoding UTF8 $salida
   }
   ```

2. Traer la rama:

   ```powershell
   Anota "2 git" { git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pull --no-rebase origin claude/laughing-pascal-tsxvkt; git log -1 --oneline }
   ```

3. Compilar y pasar las pruebas en el PC:

   ```powershell
   Anota "3 dotnet build y test" { dotnet build MotorConexiones.sln -c Release; dotnet test MotorConexiones.sln -c Release --no-build }
   ```

   Se espera `0 Errores` y `Superado: 46`. Si falla, **para aquí** y devuelve la salida.

4. Comprobar que Revit está cerrado (si aparece un proceso, ciérralo y repite):

   ```powershell
   Anota "4 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, StartTime }
   ```

5. Desplegar el add-in:

   ```powershell
   Anota "5 deploy" { .\scripts\deploy.ps1 -NoBuild }
   ```

6. **Abrir Revit 2027 con la copia `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`** (no el original). Si Revit pregunta por
   el add-in sin firmar, pulsa *Always Load*. Espera a que cargue pyRevit (unos 20 s). Comprueba que responde y que el backend es Advance Steel:

   ```powershell
   Anota "6 ping" { .\scripts\conn-call.ps1 -Operation ping }
   ```

   Se espera `ok: true`, `document.path` terminando en `HANGAR_PRUEBA_sondeo.rvt` y `backend: advancesteel`. Si dice
   `directshape`, anótalo y sigue: la prueba también vale.

7. Limpiar la placa y los pernos sueltos que dejaron los sondeos de la Fase 1 en esta copia (si no se pueden borrar, sigue):

   ```powershell
   Anota "7 sondeo 13 limpiar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\13-limpiar-fase1.py -SinTransaccion -TimeoutSec 600 }
   ```

8. **Prueba de punta a punta** (crea la conexión y la deja en el modelo). Puede tardar **varios minutos**: la primera operación
   de Advance Steel en un documento tardó 132 s en la Fase 1. No la interrumpas.

   ```powershell
   Anota "8 sondeo 11 crear" { .\scripts\revit-exec.ps1 -File scripts\sondeos\11-fase3-crear.py -SinTransaccion -TimeoutSec 900 }
   ```

   Qué mirar en la salida: cada línea `--- operacion: ok=True/False ... errores=... avisos=...`. En `create`, la lista
   `[id] SteelProxyElement | Plates` o `| Bolts` significa Advance Steel; `DirectShape | Structural Connections` significa
   que se recurrió al camino B (los avisos dicen por qué). Si `validate` da `ok=False`, copia sus errores: el sondeo para ahí.

9. **Captura del nudo**: en Revit, vista 3D, zoom al nudo (los miembros 1249510, 1249630, 1249631 y 1249636). Debe verse la
   cartela (polígono de 565 × 530 mm en el plano de la cercha), la placa cuchilla con 4 pernos en la diagonal inferior
   (miembro 1249636) y las barras acortadas según `end_setback_mm`. Guarda `docs\fases\capturas\fase3-01-conexion.png`.
   Si alguna barra se ha **alargado** en vez de acortarse, anótalo: es el signo de la extensión y se corrige en la nube.

10. Borrar la conexión y comprobar que las barras recuperan su extensión:

    ```powershell
    Anota "10 sondeo 12 borrar" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
    ```

    Se espera `delete: ok=True` y, por cada miembro, `extension inicio X -> Y`, con Y igual al valor que tenía antes de crear
    (normalmente 0). Comprueba en Revit que el nudo queda sin placas ni pernos. Captura `docs\fases\capturas\fase3-02-borrado.png`.

11. **Botón de la cinta** (lo hace la persona): pestaña **Conexiones** > **Ejecutar especificación JSON** > elegir
    `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`. Debe aparecer el resumen (tipo, cordón, cartela,
    placas cuchilla, pernos) y, al pulsar **Sí**, el diálogo de éxito con el `connection_id` y el backend. Captura
    `docs\fases\capturas\fase3-03-boton.png` con el diálogo de éxito y el nudo detrás. Si sale un diálogo de error, copia su
    texto literal. Después, borra lo creado con el paso 10 otra vez:

    ```powershell
    Anota "11 sondeo 12 borrar tras boton" { .\scripts\revit-exec.ps1 -File scripts\sondeos\12-fase3-borrar.py -SinTransaccion -TimeoutSec 900 }
    ```

12. Registro del add-in (todas las llamadas de hoy; las líneas `fabrication_transaction_*`, `advance_steel_*` son las que importan):

    ```powershell
    Anota "12 log add-in" { Get-Content -Encoding UTF8 "$env:LOCALAPPDATA\MotorConexiones\log\motorconexiones-$(Get-Date -Format yyyyMMdd).jsonl" }
    ```

13. Cerrar la copia en Revit **sin guardar**. (Si la persona lo autoriza) subir resultados y capturas:

    ```powershell
    git add docs\fases\resultados-fase-3.md docs\fases\capturas
    git commit -m "Fase 3: resultados de las pruebas en el PC"
    git pull --no-rebase origin claude/laughing-pascal-tsxvkt
    git push origin claude/laughing-pascal-tsxvkt
    ```

14. Devuelve el contenido íntegro de `docs\fases\resultados-fase-3.md`, las capturas, el texto de cualquier ventana de Revit
    que no sea de las esperadas en el paso 11, y si Revit se cerró de golpe en algún paso (en cuál y qué fue lo último impreso).
