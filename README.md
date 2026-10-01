# MotorConexiones

Add-in de Autodesk Revit en C# y suite de herramientas MCP (`conn_*`) para el modelado determinista y seguro de conexiones estructurales de acero asistido por Inteligencia Artificial.

---

## 1. Qué es MotorConexiones

**MotorConexiones** es un sistema para modelar nudos de conexiones metálicas en Revit a partir de una especificación JSON basada en planos de fabricación. El add-in está diseñado para ser operado directamente por una IA a través del servidor MCP `revit-mcp`, o bien de forma interactiva por el usuario desde la cinta de Revit.

### Características principales
- **Determinista:** La misma especificación y el mismo modelo generan exactamente el mismo resultado.
- **Validación obligatoria:** Ninguna conexión se crea sin haber pasado una validación exhaustiva (`conn_validate`) que entrega un `validation_token` criptográfico (SHA-256) que protege contra cambios en la especificación, el modelo o los límites de diseño.
- **Backend nativo Advance Steel:** Modela placas de cartela, placas cuchilla, grupos de pernos y soldaduras como elementos nativos de acero en Revit (categorías *Plates* y *Bolts*), con sólidos *DirectShape* como reserva.
- **Transacciones atómicas:** Toda creación, actualización o eliminación se encapsula en un `TransactionGroup` atómico. Si ocurre cualquier error, se realiza un rollback completo sin dejar objetos huérfanos.
- **Cero ventanas emergentes:** Las rutas MCP se ejecutan sin diálogos modales que bloqueen a la IA; los mensajes de advertencia de Revit se capturan automáticamente en el sobre de respuesta.
- **Restauración reversible:** Al eliminar una conexión (`conn_delete`), los perfiles tubulares recortados recuperan automáticamente sus extensiones y longitudes originales almacenadas en el modelo mediante *Extensible Storage*.

---

## 2. Arquitectura del Repositorio

```text
CONEXIONES/
├── MotorConexiones.sln
├── src/
│   ├── MotorConexiones.Core/       # netstandard2.0 (sin dependencias de Revit)
│   │   ├── Contract/               # Modelos C# de la especificación (GussetNodeSpec, ConnectionSpec)
│   │   ├── Geometry2D/             # Polígonos, contornos y geometría plana
│   │   ├── Model/                  # Interfaces de datos del modelo desacopladas (IModelFacts)
│   │   ├── Schema/                 # Generación de JSON Schema y ejemplos
│   │   ├── Types/                  # Tipos de conexión modulares (IConnectionType)
│   │   ├── Units/                  # Conversión unificada de mm/grados a pies/radianes (UnitConverter)
│   │   └── Validation/             # Validación AISC 360, cadenas de cotas y ValidationTokenGenerator
│   ├── MotorConexiones.Revit/      # net10.0-windows (add-in de Revit 2027)
│   │   ├── App.cs                  # IExternalApplication: cinta "Conexiones" y botón interactivo
│   │   ├── Bridge.cs               # Punto de entrada estático Bridge.Handle() para el MCP
│   │   ├── Fabrication/            # Motores de modelado físico (Advance Steel y DirectShape)
│   │   ├── Node/                   # Detección de nudos, orientación de ejes locales y miembros
│   │   ├── Operations/             # 13 operaciones ejecutables (ping, validate, create, etc.)
│   │   ├── Storage/                # Extensible Storage (esquema MotorConexiones.Connection)
│   │   └── Transactions/           # Control de transacciones y supresión de diálogos modales
│   └── MotorConexiones.Tests/      # xUnit (pruebas del Core con fixture Detalle D)
├── config/
│   └── limits.json                 # Límites AISC y tolerancias (editable sin recompilar)
├── docs/
│   ├── guide.md                    # Guía de modelado para la IA (servida por conn_get_guide)
│   ├── fases/                      # Informes y resultados de cada fase de desarrollo
│   ├── fixtures/                   # Detalle D (JSON de prueba y datos del nudo)
│   └── instalacion/                # Instrucciones paso a paso para el instalador
├── mcp/                            # Componentes del servidor MCP
│   ├── revit_mcp/conexiones.py     # Adaptador de rutas en IronPython 2.7 (pyRevit)
│   ├── tools/conn_tools.py         # 13 herramientas @mcp.tool() para FastMCP/CPython
│   ├── CONTRATO-conn.md            # Documentación del contrato de rutas MCP
│   ├── instalar-conn.ps1           # Script de despliegue en la extensión pyRevit
│   └── pruebas/                    # Suite de verificación de rutas y puente
└── scripts/
    ├── deploy.ps1                  # Compila y despliega el add-in en Revit 2027
    ├── revit-exec.ps1              # Ejecuta IronPython dentro de Revit
    └── conn-call.ps1               # Llamada directa a rutas /conn/ por HTTP
```

---

## 3. Requisitos Previos

1. **Autodesk Revit 2027.2** (runtime .NET 10).
2. **SDK de .NET 10** (`dotnet --list-sdks` debe mostrar `10.x`).
3. **pyRevit** instalado con soporte para Revit 2027 y el servicio Routes habilitado.
4. **Servidor MCP `revit-mcp`** desplegado en `C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension`.
5. **Python 3.11+** con `uv` instalado (`uv run main.py --streamable-http`).

---

## 4. Instalación Paso a Paso

### Paso 1: Compilar y desplegar el add-in en Revit
Con Revit cerrado (para permitir la copia de las DLLs bloqueadas), abre PowerShell en la raíz del repositorio:

```powershell
# Compila en Release y copia DLLs, .addin, limits.json y guide.md a %APPDATA%\Autodesk\Revit\Addins\2027\
.\scripts\deploy.ps1
```

### Paso 2: Instalar los componentes MCP en la extensión de pyRevit
Ejecuta el instalador idempotente de MCP:

```powershell
.\mcp\instalar-conn.ps1
```
Este script copia `mcp/revit_mcp/conexiones.py` y `mcp/tools/conn_tools.py` en la carpeta de la extensión de pyRevit y registra automáticamente las rutas y herramientas en `startup.py` y `tools/__init__.py`.

### Paso 3: Iniciar Revit y pyRevit
1. Abre **Autodesk Revit 2027**.
2. Abre tu modelo estructural (por ejemplo, `D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA_sondeo.rvt`).
3. En la cinta de Revit, comprueba que aparece la pestaña **Conexiones** con el botón **Ejecutar especificación JSON**.
4. Si pyRevit ya estaba abierto, pulsa **pyRevit > Reload**.

### Paso 4: Iniciar el puente MCP
Abre una consola y arranca el puente HTTP/Streamable de MCP (o ejecuta `C:\IA\iniciar_servidor_revit.bat`):

```powershell
cd "C:\IA\pyrevit-ext\mcp-server-for-revit-python.extension"
& "$env:USERPROFILE\.local\bin\uv.exe" run main.py --streamable-http
```
El servidor escuchará en `http://127.0.0.1:8000/mcp`.

### Paso 5: Conectar tu cliente de IA
Configura tu cliente de IA preferido:

- **Antigravity:** Configura `C:\Users\<Usuario>\.gemini\config\mcp_config.json`:
  ```json
  {
    "mcpServers": {
      "revit": {
        "serverUrl": "http://localhost:8000/mcp"
      }
    }
  }
  ```
- **Claude Desktop:** Configura `%APPDATA%\Claude\claude_desktop_config.json`:
  ```json
  {
    "mcpServers": {
      "revit": {
        "command": "uv",
        "args": ["run", "--directory", "C:\\IA\\pyrevit-ext\\mcp-server-for-revit-python.extension", "main.py"]
      }
    }
  }
  ```

---

## 5. Cómo Actualizar el Add-in

### Cuando se modifica código C# (`src/`):
1. Cierra Autodesk Revit para liberar el archivo DLL cargado en memoria.
2. Compila y ejecuta las pruebas unitarias:
   ```powershell
   dotnet build MotorConexiones.sln -c Release
   dotnet test MotorConexiones.sln -c Release --no-build
   ```
3. Despliega los nuevos binarios:
   ```powershell
   .\scripts\deploy.ps1
   ```
4. Vuelve a abrir Revit.

### Cuando se modifican archivos del MCP (`mcp/`):
1. Vuelve a ejecutar:
   ```powershell
   .\mcp\instalar-conn.ps1
   ```
2. En Revit, haz clic en **pyRevit > Reload** (o reinicia Revit).
3. Reinicia la ventana del puente MCP `iniciar_servidor_revit.bat`.

---

## 6. Cómo Editar `limits.json` y `guide.md` sin Recompilar

Ambos archivos fueron diseñados para evolucionar sin necesidad de abrir Visual Studio ni recompilar C#.

### Edición de Límites AISC (`config/limits.json`):
El add-in lee este archivo al validar cualquier conexión. Vive en el repositorio (`config/limits.json`) y se copia a `%APPDATA%\Autodesk\Revit\Addins\2027\MotorConexiones\config\limits.json`.

```json
{
  "schema_version": 1,
  "dimension_chain_tolerance_mm": 1.0,
  "label_value_tolerance_mm": 0.05,
  "angle_tolerance_deg": 1.0,
  "node_axis_max_distance_mm": 5.0,
  "bolts": {
    "min_spacing_factor": 2.667,
    "edge_distance_mm": {
      "12.7": 19.0,
      "15.875": 22.0,
      "19.05": 25.0
    }
  },
  "welds": {
    "min_fillet_mm": {
      "6.0": 3.0,
      "13.0": 5.0,
      "19.0": 6.0,
      "default": 8.0
    }
  }
}
```
- **Protección criptográfica:** El hash SHA-256 de los límites activos se incluye en el `validation_token`. Si modificas una tolerancia entre la validación y la creación, el token se invalida automáticamente, impidiendo creaciones con reglas desactualizadas.

### Edición de la Guía de la IA (`docs/guide.md`):
Este documento contiene las instrucciones y reglas de modelado que la IA consulta mediante `conn_get_guide`. Se lee directamente del disco en cada llamada, por lo que cualquier ajuste en las directrices de la IA surte efecto inmediato.

---

## 7. Cómo Agregar un Tipo de Conexión Nuevo

La arquitectura de MotorConexiones es completamente extensible y modular. Para añadir un nuevo tipo de conexión (por ejemplo, placa base `base_plate` o unión viga-columna `beam_column`):

1. **Definir el tipo en el Core:**
   En `src/MotorConexiones.Core/Types/`, crea una clase que implemente `IConnectionType`:
   ```csharp
   public sealed class BasePlateType : IConnectionType
   {
       public string Name => "base_plate";
       public string Description => "Placa base con pernos de anclaje para columna HSS/W.";
       public JsonDocument SchemaJson => ...;
       public JsonDocument ExampleJson => ...;
       public void Validate(ConnectionSpec spec, IModelFacts modelFacts, ValidationResult result, LimitsConfig limits) { ... }
   }
   ```
2. **Definir los contratos de datos (si aplica):**
   Crea las clases correspondientes en `src/MotorConexiones.Core/Contract/` para tipar los elementos específicos de la conexión.
3. **Registrar el tipo en el Registro Central:**
   En `Bridge.cs`, añade el nuevo tipo durante la inicialización:
   ```csharp
   ConnectionTypeRegistry.Register(BasePlateType.Instance);
   ```
4. **Implementar el backend de modelado en Revit:**
   En `src/MotorConexiones.Revit/Fabrication/`, implementa la geometría nativa de Advance Steel y los sólidos de reserva DirectShape para ese tipo.
5. **Agregar pruebas unitarias:**
   Crea fixtures JSON en `docs/fixtures/` y agrega pruebas en `MotorConexiones.Tests` que verifiquen el esquema, ejemplos y reglas de validación.

---

## 8. Guion de Prueba de Punta a Punta (E2E) para la IA

Sigue este guion paso a paso en tu cliente de IA (como Antigravity) para verificar el ciclo de vida completo de una conexión:

1. **Comprobar el add-in:**
   Llama a `conn_ping`. Debe responder `ok: true`, `backend: "advancesteel"` y `document.title` correspondiente al modelo abierto.
2. **Consultar la guía:**
   Llama a `conn_get_guide` para cargar las instrucciones de modelado.
3. **Inspeccionar el nudo:**
   Pide al usuario que seleccione el cordón y las diagonales en Revit (o pasa los IDs del nudo de prueba: `[1249510, 1249630, 1249631, 1249636]`). Llama a `conn_get_node_info`. Anota los ángulos, tipos de perfil y el origen local.
4. **Obtener el esquema:**
   Llama a `conn_get_schema` con `gusset_node` para conocer la estructura y ver un ejemplo válido.
5. **Validar la especificación:**
   Llama a `conn_validate` pasando la especificación del Detalle D (con las incertidumbres resueltas en `user_confirmed_value`). Debe devolver `is_valid: true`, 0 errores y un `validation_token` de 64 caracteres hexadecimales.
6. **Previsualizar sin alterar el modelo:**
   Llama a `conn_preview` con la misma especificación. Revisa el resumen de elementos a crear (cartela, placa cuchilla, pernos, soldaduras) y miembros a recortar.
7. **Crear la conexión:**
   Tras la confirmación explícita del usuario, llama a `conn_create` con la especificación y el `validation_token`. Debe responder `ok: true`, entregando un `connection_id` único y la lista de IDs creados en Revit.
8. **Consultar la conexión en el modelo:**
   Llama a `conn_list` (debe mostrar 1 conexión) y `conn_get` con el `connection_id`.
9. **Eliminar la conexión y verificar restauración:**
   Llama a `conn_delete` con el `connection_id`. Comprueba que la conexión desaparece de `conn_list` y que las diagonales recuperan sus dimensiones originales sin ningún elemento residual.

---

## 9. Seguridad y Manejo de Errores

| Código de Error | Significado | Acción recomendada |
|---|---|---|
| `VALIDATION_TOKEN_INVALID` | El token no coincide, expiró por cambio en el modelo o en `limits.json`. | Volver a llamar a `conn_validate` para revalidar el modelo. |
| `DIMENSION_CHAIN_MISMATCH` | Las cotas leídas no suman el total especificado (> 1 mm). | Corregir la lectura del plano o marcar la cota en `uncertain_fields`. |
| `UNRESOLVED_UNCERTAINTY` | Hay campos en `uncertain_fields` sin confirmación del usuario. | Preguntar al usuario y asignar el valor en `user_confirmed_value`. |
| `REVIT_BUSY` | Revit tiene un comando activo o una transacción abierta. | Cerrar el comando en Revit y reintentar la llamada. |
| `ELEMENT_NOT_FOUND` | Un ElementId no existe en el documento activo. | Volver a inspeccionar el nudo con `conn_get_node_info`. |
| `NODE_AXES_NOT_INTERSECTING` | Los ejes de las barras no se cruzan a menos de 5 mm. | Informar al usuario para ajustar el modelo analítico/físico. |

---

## 10. Licencia

Proyecto desarrollado para automatización de ingeniería estructural con Autodesk Revit y Model Context Protocol (MCP).
