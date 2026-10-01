# Instalación y pruebas de la Fase 3 (instrucciones para el agente instalador)

Objetivo: compilar e instalar el add-in `MotorConexiones` en Revit 2027, verificar todas las operaciones del Bridge (ping, guide, types, schema, find_profile, node_info, validate, preview, create, list, get, delete) mediante el sondeo automatizado `11-fase3-verificacion.py`, y validar el funcionamiento interactivo del botón Ribbon "Cargar Spec..." ejecutando el archivo `docs\fixtures\detalle-D-confirmado.json`.

Las pruebas de escritura se ejecutan **sobre una copia** del modelo (`HANGAR_PRUEBA_sondeo.rvt`), nunca sobre el original.

Devuelve la salida literal de cada paso, incluidos los errores, y las capturas de pantalla indicadas.

---

## Antes de empezar (verificación humana)

- Revit 2027 **cerrado** antes de ejecutar el paso 5 (`deploy.ps1` falla si la DLL está cargada y bloqueada en memoria).
- pyRevit con servidor **Routes** activo y la extensión `revit-mcp` cargada.
- Modelo de prueba listo (`HANGAR_PRUEBA_sondeo.rvt` con el nudo del Detalle D, IDs: 1249510, 1249630, 1249631, 1249636).
- Vale Windows PowerShell 5.1 o PowerShell 7.

---

## Pasos

### 1. Preparar la consola de PowerShell y el archivo de resultados

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

### 2. Traer la rama de trabajo

```powershell
Anota "2 git" { git fetch origin; git checkout claude/laughing-pascal-tsxvkt; git pull origin claude/laughing-pascal-tsxvkt; git log -1 --oneline }
```

Se espera un commit reciente con el mensaje de la Fase 3.

### 3. Compilar la solución completa y pasar las pruebas unitarias

```powershell
Anota "3 dotnet build y test" {
    dotnet build MotorConexiones.sln -c Release
    dotnet test MotorConexiones.sln -c Release --no-build
}
```

Se espera `Build succeeded: 0 Warning(s), 0 Error(s)` y `Total: 45, Passed: 45, Failed: 0`.

### 4. Verificar que Revit esté cerrado

```powershell
Anota "4 revit cerrado" { Get-Process -Name Revit -ErrorAction SilentlyContinue | Select-Object Id, ProcessName }
```

Si aparece algún proceso de Revit, ciérralo antes de continuar.

### 5. Desplegar el add-in en Revit 2027

```powershell
Anota "5 deploy addin" { .\scripts\deploy.ps1 -Configuration Release }
```

Se espera código de salida 0 y confirmación del despliegue en `%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\`.

### 6. Abrir Revit 2027 con el modelo de prueba

Abre Revit 2027 y carga el archivo `HANGAR_PRUEBA_sondeo.rvt`.
Verifica que en la barra de herramientas aparezca la pestaña **Conexiones** y el botón **Cargar Spec...**.

### 7. Ejecutar la verificación integral de operaciones del Bridge (Sondeo 11)

Con el modelo abierto en Revit, ejecuta desde PowerShell:

```powershell
Anota "7 sondeo 11 verificacion bridge" {
    .\scripts\revit-exec.ps1 -SinTransaccion -File scripts\sondeos\11-fase3-verificacion.py
}
```

Este script comprueba en secuencia:
1. `conn_ping` (versión, backend activo y proceso).
2. `conn_get_guide` (guía en Markdown).
3. `conn_list_types` (lista `gusset_node`).
4. `conn_get_schema` (JSON Schema y ejemplo completo).
5. `conn_find_profile` (búsqueda de familias y perfiles cargados).
6. `conn_get_node_info` (geometría de miembros 1249510, 1249630, 1249631, 1249636).
7. `conn_validate` (valida `docs/fixtures/detalle-D-confirmado.json` y emite `validation_token`).
8. `conn_preview` (elementos y recortes proyectados).
9. `conn_create` (crea físicamente la conexión con el token).
10. `conn_list` (comprueba que la conexión existe en Extensible Storage).
11. `conn_get` (obtiene el detalle de la conexión por ID).
12. `conn_delete` (elimina la conexión y restaura recortes de barras).
13. `conn_list` post-delete (comprueba que el modelo quedó limpio).

### 8. Prueba visual interactiva con el botón Ribbon "Cargar Spec..."

1. En Revit, haz clic en la pestaña **Conexiones**.
2. Haz clic en el botón **Cargar Spec...**.
3. En el cuadro de diálogo de selección de archivo, selecciona:
   `D:\Proyectos C#\CONEXIONES\docs\fixtures\detalle-D-confirmado.json`
4. Observa el diálogo de confirmación: muestra el tipo de conexión, nudo, backend a utilizar y número de barras a recortar.
5. Haz clic en **Aceptar** / **Sí**.
6. Observa la creación de la cartela, placas cuchilla, pernos y recortes de miembros.
7. Toma una captura de pantalla del nudo 3D generado y guárdala como:
   `docs\fases\capturas\fase-3-ribbon-creacion.png`

### 9. Inspección de logs generados

```powershell
Anota "9 logs addin" {
    $logDir = "$env:LOCALAPPDATA\MotorConexiones\log"
    Get-ChildItem -Path $logDir -File | Sort-Object LastWriteTime -Descending | Select-Object -First 3 | ForEach-Object {
        "--- Archivo: $($_.FullName) ---"
        Get-Content -Path $_.FullName -Tail 20
    }
}
```

### 10. Confirmación y entrega

Guarda y sube `docs\fases\resultados-fase-3.md` y las capturas generadas.
