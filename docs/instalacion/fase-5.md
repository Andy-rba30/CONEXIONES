# Fase 5: Punta a punta y documentación (instrucciones para el instalador y prueba de IA)

Objetivo: desplegar el add-in final (operaciones `probe_*` retiradas, `config/limits.json` integrado en el hash del `validation_token`), verificar la suite automatizada (`probar_conexiones.py --puente`) y ejecutar la prueba de punta a punta (E2E) desde el cliente de IA (Antigravity en `http://localhost:8000/mcp`) sobre el modelo `HANGAR_PRUEBA_sondeo.rvt`.

---

## 1. Preparación y despliegue del add-in actualizado

Ejecuta en PowerShell desde `D:\Proyectos C#\CONEXIONES`:

```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass -Force
cd "D:\Proyectos C#\CONEXIONES"
git pull --no-rebase origin claude/laughing-pascal-tsxvkt

# 1. Cerrar Revit si estaba abierto (para permitir sobreescribir la DLL)
Stop-Process -Name Revit -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 3

# 2. Compilar en Release y desplegar en Revit 2027
dotnet build MotorConexiones.sln -c Release
dotnet test MotorConexiones.sln -c Release --no-build
.\scripts\deploy.ps1

# 3. Asegurar que los archivos del MCP están al día
.\mcp\instalar-conn.ps1
```

Se espera:
- Compilación y 51 pruebas xUnit en verde.
- `== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==`.
- Archivos `limits.json` y `docs/guide.md` copiados junto a la DLL.

---

## 2. Iniciar Revit y el puente MCP

1. Abre **Autodesk Revit 2027** con el modelo de prueba:
   `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`
   Espera a que pyRevit termine de cargar las extensiones (unos 20 segundos).
2. Inicia el puente MCP ejecutando `C:\IA\iniciar_servidor_revit.bat` (o en PowerShell):
   ```powershell
   cd "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
   & "$env:USERPROFILE\.local\bin\uv.exe" run main.py --streamable-http
   ```
   Deja esa ventana abierta (escuchando en `http://127.0.0.1:8000`).

---

## 3. Verificación automatizada contra el puente

En otra consola de PowerShell:

```powershell
$ext = "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
$py = "$ext\.venv\Scripts\python.exe"

& $py "D:\Proyectos C#\CONEXIONES\mcp\pruebas\probar_conexiones.py" --puente
```

Se espera `Resultado: 19/19 pruebas correctas`.

---

## 4. Guion de Prueba de Punta a Punta (E2E) con Antigravity

Con Antigravity conectado a `http://localhost:8000/mcp`, sigue este guion paso a paso:

### Paso 4.1: Comprobar el servicio (`conn_ping`)
Pide a la IA que llame a `conn_ping`.
- **Resultado esperado:** `ok: true`, `backend: "advancesteel"`, `document.title: "HANGAR_PRUEBA_sondeo"`. En `operations`, ya **no** deben aparecer `probe_plate_b` ni `probe_delete_b`.

### Paso 4.2: Consultar la guía (`conn_get_guide`)
Pide a la IA que llame a `conn_get_guide` y resuma el flujo de trabajo de 10 pasos.

### Paso 4.3: Inspeccionar el nudo (`conn_get_node_info`)
Pide a la IA que inspeccione el nudo del fixture con los IDs `[1249510, 1249630, 1249631, 1249636]` (cordón horizontal continuo `1249510`).
- **Resultado esperado:** Origen local en `[-11867.7, -17195.8, 17423.0]`, 4 miembros reconocidos con sus perfiles (`HSS3X3X1/4` y `HSS2-1-2X2-1-2X3-16 64x64`), `existing_connections: []`.

### Paso 4.4: Consultar el esquema (`conn_get_schema`)
Llama a `conn_get_schema` con `connection_type: "gusset_node"`.

### Paso 4.5: Validar la especificación del Detalle D (`conn_validate`)
Envía la especificación del Detalle D con las dudas resueltas (`docs/fixtures/detalle-D.json` con `user_confirmed_value` completado: `"HSS2-1/2X2-1/2X3/16"` para `members[1].profile` y `"through_slot"` para `gusset.chord_interface`).
- **Resultado esperado:** `is_valid: true`, 0 errores, advertencias informativas `ANGLE_DIFFERS_FROM_MODEL` y un `validation_token` de 64 caracteres hex.

### Paso 4.6: Previsualizar la conexión (`conn_preview`)
Llama a `conn_preview` con la misma especificación.
- **Resultado esperado:** `ok: true`, resumen con `gusset_plates: 1`, `knife_plates: 1`, `bolts: 4`, `weld_lines: 6`, `members_modified: 3`.

### Paso 4.7: Crear la conexión física en Revit (`conn_create`)
Llama a `conn_create` con la especificación y el `validation_token`.
- **Resultado esperado:** `ok: true`, devuelve `connection_id` (GUID) y la lista de IDs creados en Revit (cartela, placas cuchilla, pernos y soldaduras). En Revit 3D se aprecian físicamente los elementos creados y los retiros aplicados a las diagonales.

### Paso 4.8: Consultar la conexión guardada (`conn_list` y `conn_get`)
- `conn_list`: devuelve `connections_count: 1` con el `connection_id`.
- `conn_get` con ese ID: devuelve la especificación completa guardada en el *Extensible Storage*.

### Paso 4.9: Eliminar la conexión y verificar reversibilidad (`conn_delete`)
Llama a `conn_delete` con el `connection_id`.
- **Resultado esperado:** `ok: true`. Los elementos creados se eliminan y las barras HSS vuelven exactamente a sus cotas y extensiones originales. `conn_list` vuelve a mostrar 0 conexiones.

---

## 5. Cierre
Cierra Revit **sin guardar cambios** para dejar el modelo de prueba limpio para futuras verificaciones.
