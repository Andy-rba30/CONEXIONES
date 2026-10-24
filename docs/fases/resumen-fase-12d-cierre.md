# Resumen del cierre de la ronda 12d (2026-10-24): la 0.12.3 contrastada en Revit y el add-in 0.12.4 (Fase 12 cerrada)

Rama `main`. Add-in **0.12.4** (pendiente de desplegar en el PC: la última instalada es la 0.12.3; se despliega con la instalación
de la Fase 13). Informe: `docs/fases/fase-12.md`, sección 10. Copia corta del mensaje del chat.

## 1. Qué dijeron los resultados (`resultados-fase-12d.md`, add-in 0.12.3, 2026-10-24)

- **El sondeo 29 no encontró la viga de Advance Steel**: `GetModelObjectIds` devolvió **5** `ObjectId`, pero **ninguno se abrió**
  (`tipos recorridos: (no abierto)=5; vigas vistas: 0`). El sondeo eligió el modo `eOpenMode.kNotOpened` (el primero de la lista,
  porque buscaba uno con "Read" y no lo hay) y la reserva `GetFilerObject` tampoco dio objeto. `BeamShortening` no se intentó.
- **Las advertencias de Revit** son **"The created elements are only visible in Detail Level: Fine."**, una por soldadura
  (`severity Warning`, elementos 1322501 y 1322502, definición `73c984d7-da5a-4cd2-b83b-1c2c345d216b`). Informativas.
- El nudo en K desde Conectar: **2** elementos, `END_CUT_APPROXIMATED` + `REVIT_WARNING`; `delete` con las 2 barras restauradas.
- **`--puente` 38/38**, `templates_count 2`, el log sin `*_failed` del add-in.

## 2. Qué se decidió y qué se programó (add-in 0.12.4)

- **El corte inclinado queda como retiro por el eje, definitivo** (regla de `fase-12.md` 9.6.3: el 29 era el último intento).
  `END_CUT_APPROXIMATED` se queda; **no hay 12e ni más sondeos** sobre la viga. Que el 29 no abriera los 5 objetos por el modo
  `kNotOpened` queda anotado en `nudos-en-k-excentricos.md` (si algún día se reabre, lo primero es `kNormal`), nada más.
- **La advertencia "solo visible en nivel Fino" se filtra del aviso**: `OperationScope` la reconoce por el Guid de su definición
  (no depende del idioma), solo si es `Warning`, y la deja en el log como `revit_failure` con `filtered: true`. Las demás
  advertencias y todos los errores siguen igual. El cuadro del nudo en K dirá solo `END_CUT_APPROXIMATED`.
- **Nada más cambia**: ni la fabricación ni el Core. Versión 0.12.4 (add-in, adaptador, herramientas, simulador, contrato).
- Documentación: `fase-12.md` 10, README, `CLAUDE.md`, guía, `CONTRATO-conn.md`, `docs/instalacion/fase-12d.md` (hecha) y las
  propuestas. **La Fase 12 queda cerrada.**

## 3. Qué se probó en la nube

`dotnet build` sin avisos, `dotnet test` **291/291**, `py_compile` correcto, simulador **82/82** y `probar_conexiones.py`
**35/35** (`addin=0.12.4`). El filtro de la advertencia: **NO PROBADO en la nube** (no hay Revit); se ve en el PC con la
instalación de la Fase 13 (el nudo en K con solo `END_CUT_APPROXIMATED` y dos `revit_failure` con `filtered: true` en el log).

## 4. Qué hacer ahora

1. **Nada para el instalador**: la 0.12.4 se despliega con la instalación de la Fase 13.
2. Abrir la sesión del **prompt y alcance de la Fase 13** (sin código; como se hizo con `docs/prompts/fase-12.md`) con este prompt:

   ```
   Lee CLAUDE.md, docs/ENCARGO_MOTOR_CONEXIONES.md (secciones 7, 13 y 14), docs/prompts/fase-12.md (sección 2, la decisión D6 y la
   sección 9), docs/fases/fase-12.md (secciones 10.5 y 10.6), docs/fases/fase-11.md (sección 7.2), docs/propuestas/nudos-en-k-excentricos.md
   y docs/propuestas/flujo-intuitivo.md. Escribe docs/prompts/fase-13.md con el prompt y el alcance de la Fase 13: (1) la cartela
   automática (outline.mode = "auto": cómo se calcula el contorno desde las barras y el punto de trabajo, qué nudos de la cercha de
   la 8c la necesitan y qué pasa con el arranque de Conectar) y (2) partir la diagonal para fabricar el empalme del Detalle B a 330
   (qué hace Advance Steel o Revit al partir una barra de armazón, qué sondeo lo decide, cómo se deshace). Sin código: solo el prompt,
   el alcance, las dudas numeradas para la persona con tu recomendación en cada una, y qué sondeos hacen falta. Anota que la
   instalación de la Fase 13 despliega la 0.12.4 y comprueba el filtro de la advertencia (fase-12.md 10.3). Termina con commit, push
   y un resumen corto.
   ```

3. Con las dudas respondidas, la sesión que programe la Fase 13 con el prompt de `docs/prompts/fase-13.md`.
