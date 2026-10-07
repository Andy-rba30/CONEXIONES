# Resumen del cierre de la Fase 10 (2026-10-07): resultados contrastados y ronda 10b (add-in 0.10.1)

Rama `main`. Add-in **0.10.1** (pendiente de desplegar en el PC: la última instalada es la 0.10.0). Informe:
`docs/fases/fase-10.md`, sección 7. Instalación de la ronda: `docs/instalacion/fase-10b.md` (solo lo que no se pudo probar).
Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-10.md`, add-in 0.10.0, 2026-10-07)

- **Funcionó**: el manejador de clics registrado en el arranque; la **selección asistida por el puente** (desde la diagonal
  1251723 se añadieron las 63 barras de la cercha de la 8c en 11 rondas, ninguna fuera del plano) con **los mismos 16 listos**
  de siempre; las **33 etiquetas** puestas (33 BMP en `C:\Users\Public\MotorConexiones\etiquetas`) y quitadas al crear, al
  replanificar y con `discard all` (a cero); las **16 cartelas fantasma** creadas y quitadas con los marcadores; **Crear 16**
  dos veces desde la ventana (4,0 s y 2,1 s), Ctrl+Z y Replanificar; **Borrar el lote desde la ventana** (16 borradas, 144
  elementos, 48 barras restauradas, 1,1 s; sondeos 12 y 13 a cero: lo pendiente de la Fase 9); los tres `REVIT_WARNING` de
  un `create` en la {3D} son "The created elements are only visible in Detail Level: Fine"; el **Encargo para IA** escribió
  el archivo, lo copió al portapapeles y abrió la carpeta; el log sin ninguna línea de error de las vigiladas.
- **Falló**: el botón **Planificar lote con una barra** murió en el cuadro de la selección asistida con "Corresponding button
  not found: defaultButton" (la búsqueda de las 63 barras sí funcionó: está en el log). Por eso **nadie pinchó una etiqueta**
  (ni un `label_clicked` en el día). Y `probar_conexiones.py --puente` dio **14/22**: el documento activo era `MODELO CERCO`
  (los IDs del fixture no existen ahí) y el servidor del puerto 8000 no estaba escuchando.
- **Sin anotar** (no llegaron capturas ni anotaciones de la persona): el cuadro de Borrar el lote, si Ctrl+Z devolvió las
  marcas, `PLAN_MARKS_REPLACED`, Editar nudo con la ventana abierta, dónde salen los fantasmas. Nada en el log en contra.
- De paso: el catálogo tenía **dos** plantillas (la `PRUEBA probar_conexiones` que dejó el `--puente` de la Fase 9 al caerse);
  el encargo la usó como ejemplo. La 10b la borra antes de planificar desde la cinta.

## 2. Qué se corrigió (ronda 10b, add-in 0.10.1)

- `BatchPlanCommand.cs`: el `TaskDialog` de la selección asistida fijaba `DefaultButton = CommandLink1` **antes** de
  `AddCommandLink`, y Revit no encuentra ese botón. Ahora se asigna **después** de añadir los dos enlaces y, si el cuadro
  fallara igual, se planifica con todas (la opción por defecto), se anota `ribbon_batch_assist_dialog_failed` y la barra de
  estado lo dice. Nada más cambia en el código.
- Versión 0.10.1 en el add-in, el adaptador, las herramientas, el simulador y el contrato (solo la versión).
- Documentación al día: `fase-10.md` (sección 7: contraste, correcciones, pendientes), `fase-9.md` 7.6, README (estado,
  tabla de garantías, secciones 9 y 12), propuestas, nota de "hecha" en `docs/instalacion/fase-10.md`.

## 3. Qué se probó en la nube

`dotnet build` sin avisos (la 0.10.1 compila contra la API 2027), `dotnet test` **217/217**, `py_compile` correcto, simulador
**66/66** y `probar_conexiones.py` **30/30** contra el simulador (`addin=0.10.1`). El cuadro corregido solo se puede ver en
Revit (`TaskDialog` no existe fuera): **NO PROBADO en la nube**, se prueba en el PC.

## 4. Qué hacer ahora

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-10b.md` entero (unos 30 minutos, sobre la copia, con la copia
   como **único** proyecto abierto; no crea conexiones).
2. Hacer los pasos marcados **(la persona)**: pinchar **una** barra y Planificar lote (ahora debe salir el cuadro "Selección
   asistida: 63 barras tocan la selección"), Planificar con las 64 barras (16 listos), Cancelar y Solo la selección; mirar
   las etiquetas, **pinchar la 4** (sin ningún cuadro), N7 en la tabla, 12 en el mapa, una roja, el clic con la ventana
   cerrada, el globo y Descartar plan. Cuatro capturas (`fase10b-01` a `fase10b-04`).
3. Devolver `docs\fases\resultados-fase-10b.md` con las anotaciones y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-10.md (secciones 7.6 y 7.7) y docs/fases/resultados-fase-10b.md. Cierra la ronda 10b y la
   Fase 10: contrasta los resultados con lo esperado en 7.6 (el cuadro de la selección asistida desde la cinta, las etiquetas
   pinchadas sin cuadro, el resaltado desde la tabla y el mapa, --puente 32/32), corrige lo que haga falta (ronda 10c solo si
   es imprescindible), actualiza el informe, el README y las propuestas. No empieces la Fase 11. Termina con compilación sin
   avisos, pruebas en verde, commit, push y un resumen corto.
   ```
