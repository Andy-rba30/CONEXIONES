# Fase 6: ventana de previsualización 2D con cotas, borrado desde la cinta y botón en la pestaña ARBA

Fase nueva, fuera de las cinco del encargo original (sección 13), pedida por la persona al cerrar la Fase 5. Se ejecuta
en **una sesión**, con el prompt de la sección 1. Lo demás de este archivo es el alcance que esa sesión debe cumplir.

## 1. Prompt para pegar en la sesión nueva

```
Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md, docs/fases/fase-5.md (secciones 6 y 7) y docs/prompts/fase-6.md
completo. Ejecuta SOLO la Fase 6 tal como la define docs/prompts/fase-6.md.
La ventana vive únicamente en el camino del botón de la cinta; las rutas conn_* siguen sin ventanas. La geometría del
croquis y las cotas van en Core (sin Revit, con pruebas xUnit); la ventana solo dibuja lo que Core le da.
No inventes miembros de la API de Revit ni de WPF dentro de Revit: lo que no se pueda compilar en la nube se prueba con
un sondeo o se marca NO PROBADO.
Termina con docs/fases/fase-6.md, docs/instalacion/fase-6.md, commit, push y un resumen corto.
--- RESULTADOS ---
(pega aquí la respuesta del instalador sobre cómo está hecha la pestaña ARBA, si ya la tienes; si no, borra este bloque)
--- FIN ---
```

## 2. Por qué esta fase

En la Fase 5 la IA creó la conexión del Detalle D de punta a punta y, antes de crear, mostró una tabla resumen (paso 8
del prompt B-1: cordón, cartela, cada barra con su unión, pernos, soldaduras, retiros). La persona quiere ver eso mismo
**dibujado con cotas, como un detalle de CAD en 2D**, poder corregir valores (sobre todo espesores y medidas) antes de
crear, y hacerlo desde Revit con o sin la IA. También quiere borrar conexiones sin la IA y que el botón esté en su
pestaña **ARBA**, no en una pestaña aparte.

## 3. Alcance (lo que se entrega)

### 3.1 Ventana "Previsualización de conexión" (solo en el camino del botón)

Se abre desde el botón de la cinta, después de elegir el archivo JSON (o con las barras seleccionadas, como hoy), y
sustituye al diálogo de resumen actual de `RunSpecCommand.cs`. Tiene tres zonas:

- **Izquierda, croquis 2D** del nudo en el plano de la cercha, en el sistema local del nudo (origen en el punto de
  trabajo, X a lo largo del cordón, Y perpendicular en el plano; sección 7 del encargo). Dibuja: eje y ancho del cordón y
  de cada barra, contorno de la cartela, placa cuchilla, pernos, retiros (extremo real de cada barra tras el `setback`),
  marcas de soldadura. Cotas: ancho y alto de la cartela, espesor como etiqueta (`PL 3/8"` / `9,5 mm`), paso y distancia
  al borde de los pernos, retiro de cada barra, largo de la ranura. Zoom con la rueda y encuadre con el botón central;
  botón "Ajustar". Las cotas se muestran en mm con una cifra decimal.
- **Derecha, tabla editable** con el contenido del paso 8: cordón (perfil), cartela (espesor, ancho, alto, unión al
  cordón, soldadura), cada barra (rol, perfil, ángulo del plano, tipo de unión, largo de ranura o placa cuchilla con sus
  medidas e inserción, pernos filas × columnas, paso, borde, diámetro, soldadura, retiro) y las **dudas**
  (`uncertain_fields`) con su `user_confirmed_value` editable. Cambiar un valor redibuja el croquis y vuelve a validar.
  El contorno de la cartela (`outline.points_mm`) se edita en una lista de puntos; no hace falta arrastrar vértices.
- **Abajo, estado**: errores y avisos de `SpecValidator` con código, campo y sugerencia (los mismos que ve la IA), el
  `validation_token` abreviado y los botones **Recargar** (vuelve a leer el JSON del disco, para correcciones hechas por
  el chat), **Guardar JSON** (escribe el JSON corregido junto al original con sufijo `-corregido.json`, nunca encima),
  **Validar**, **Crear** (solo activo con validación en verde) y **Cancelar**.

**Crear** hace exactamente lo que hace hoy el botón: `ConnectionCreationService` con el token recién calculado, una
operación atómica, registro en Extensible Storage, diálogo final con el `connection_id`. Nada de esto toca `Bridge` ni
las rutas `conn_*`.

### 3.2 Borrar desde la cinta

Segundo botón, "Conexiones del modelo": lista las conexiones creadas por el add-in en el documento (lo mismo que
`conn_list`: id, tipo, fecha, elementos) y permite borrar una con confirmación (lo mismo que `conn_delete`: borra lo
creado y restaura las extensiones). Puede ser una ventana pequeña propia o una pestaña dentro de la ventana anterior.

### 3.3 Botón en la pestaña ARBA

Hoy el add-in crea la pestaña `Conexiones` (constantes `TabName` y `PanelName` en `App.cs`). Qué hacer depende de cómo
esté hecha ARBA, y eso lo dice la respuesta del instalador que la persona pega bajo `--- RESULTADOS ---`:

- Si ARBA la crea otro add-in de C# por la API de Revit: añadir el panel `MotorConexiones` a esa pestaña
  (`CreateRibbonPanel("ARBA", …)`), con reserva a `Conexiones` si falla, y registrarlo en el log. Cuidado con el orden de
  carga de los add-ins: si ARBA todavía no existe cuando arranca el nuestro, hay que crearla o esperar al evento
  `ApplicationInitialized`; decidirlo con el dato real, no de memoria.
- Si ARBA es una extensión de pyRevit: la API de Revit no la ve. El camino es un `pushbutton` dentro de esa extensión
  cuyo `script.py` lance nuestro comando (`PostCommand` con el `RevitCommandId` del comando externo, o reflexión sobre
  `RunSpecCommand`). Los archivos de ese botón van en este repositorio, en una carpeta nueva `ribbon-arba/`, con un
  instalador `.ps1` que los copie a la extensión, igual que `mcp/instalar-conn.ps1`.
- Si no hay respuesta del instalador: no adivinar. Escribir el sondeo `scripts/sondeos/15-cinta-arba.py` que liste las
  pestañas y paneles de la cinta con `Autodesk.Windows.ComponentManager.Ribbon` (id, título, origen) y dejar el botón en
  `Conexiones` hasta tener el dato. Marcar NO PROBADO.

## 4. Reglas técnicas de esta fase

- **Core sin Revit**: la geometría del croquis va en `src/MotorConexiones.Core/Sketch/` como primitivas simples en mm
  (líneas, polígonos, círculos, cotas con sus dos puntos y su texto, etiquetas). `SketchBuilder.Build(spec, nodeInfo)`
  devuelve esa lista a partir de la especificación y de los datos del nudo (ángulos y anchos de perfil). Pruebas xUnit
  con el fixture del Detalle D: número de piezas, valores de las cotas (565, 530, 60, 40, 180, 60, 260…), retiros.
- **La ventana solo dibuja**: WPF en `src/MotorConexiones.Revit/UI/`. Comprobar primero que `<UseWPF>true</UseWPF>`
  compila en la nube con `EnableWindowsTargeting` (ya está activo); si no compila, usar Windows Forms o cargar el XAML en
  tiempo de ejecución, y anotarlo en el informe. Hilo de Revit: la ventana se abre modal desde el comando externo, sin
  `ExternalEvent`, porque todo ocurre dentro del comando.
- **Unidades**: solo `Units/UnitConverter.cs` convierte. El croquis trabaja en mm; la ventana escala a píxeles con un
  factor propio que no es una conversión de unidades.
- **Sin ventanas en el camino de la IA**: `Bridge.Handle` y las rutas `conn_*` no cambian. Las pruebas de
  `mcp/pruebas/probar_conexiones.py` deben seguir en 19/19.
- Código y claves en inglés; textos de la ventana, mensajes y documentación en español. `ElementId.Value`, nunca
  `IntegerValue`.
- Una operación = un `TransactionGroup`; Crear y Borrar de la ventana usan los servicios existentes, no transacciones
  nuevas.

## 5. Instrucciones para el instalador (`docs/instalacion/fase-6.md`)

Una ronda, sobre la copia `HANGAR_PRUEBA_sondeo.rvt`, con `Anota` como en las fases anteriores y resultados en
`docs/fases/resultados-fase-6.md`. Revit lo abre y lo cierra la persona, no el instalador:

1. `git pull`, build, test, `deploy.ps1` con Revit cerrado (y el instalador de `ribbon-arba/` si existe).
2. Abrir Revit con la copia. Captura de la cinta: dónde está el botón (ARBA o Conexiones).
3. Botón con `docs/fixtures/detalle-D-confirmado.json`: captura de la ventana con el croquis y la tabla. La persona
   compara las cotas con el plano del Detalle D y anota diferencias.
4. Cambiar en la tabla el espesor de la cartela de 9,525 a 12,7 mm: ver que el croquis cambia la etiqueta, que el token
   cambia y que la validación sigue en verde; **Guardar JSON** y comprobar que existe `detalle-D-confirmado-corregido.json`
   y que el original no cambió (`git status` limpio salvo el archivo nuevo).
5. Poner un valor inválido (paso de pernos 10 mm): ver el error con código y campo, y que **Crear** se desactiva.
6. Volver a 9,525, **Crear**: captura del nudo en Revit (Fino, Sombreado) y `conn_list` por `conn-call.ps1` = 1.
7. Botón "Conexiones del modelo": borrar la conexión; sondeo 12 debe dar `conexiones en el modelo: 0` y las extensiones
   originales; sondeo 13 `Elementos de acero sueltos encontrados: 0`.
8. `probar_conexiones.py --puente` 19/19 (la IA no se ve afectada).
9. Cerrar sin guardar, subir resultados y capturas.

## 6. Definición de hecho

- Compila en la nube sin avisos; `dotnet test` en verde con las pruebas nuevas del croquis.
- `docs/fases/fase-6.md` según el Anexo B del encargo, con la lista de lo NO PROBADO (todo lo de WPF dentro de Revit
  hasta que vuelva el instalador).
- `docs/instalacion/fase-6.md` literal, con `Anota`, capturas con nombre fijo y qué devolver.
- README: sección nueva "Previsualizar y corregir antes de crear" y "Borrar desde la cinta"; la tabla de garantías no
  cambia hasta que haya resultados.
- Commit en español en la rama de trabajo indicada; sin pull requests.

## 7. Fuera de alcance de esta fase

- Render 3D, imágenes en `conn_preview`, `outline.mode = "auto"`.
- Editar la conexión ya creada desde la ventana (`conn_update` sigue siendo solo de la IA).
- Otros tipos de conexión: la ventana se diseña para `gusset_node`, pero `SketchBuilder` recibe el tipo por
  `IConnectionType` para que un tipo nuevo pueda aportar su propio croquis.
