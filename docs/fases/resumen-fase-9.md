# Resumen de la Fase 9 (2026-10-06): crear por lotes

Rama `main`. Add-in **0.9.0** (**NO PROBADO en Revit**: la última versión desplegada en el PC es la 0.8.5). Informe:
`docs/fases/fase-9.md`. Instalación: `docs/instalacion/fase-9.md`. Prompt y alcance: `docs/prompts/fase-9.md`. Copia corta
del mensaje del chat.

## 1. Qué se hizo

- **El plan se convierte en acero.** `conn_batch_create` (y el botón **Crear N conexiones** de la ventana del plan) crea
  los nudos listos **nudo a nudo con los tokens del plan**. Cada nudo es la misma operación atómica que `conn_create`
  (token comprobado contra el modelo, cartela, placas, pernos, soldaduras, retiros, registro con `source.batch_id`), con
  su propio grupo de transacción, **anidado** en un grupo exterior del lote que se asimila al final: **si un nudo falla se
  revierte solo y los demás se quedan** (tu decisión P9), y en Revit todo el lote es **una sola entrada de deshacer**
  (Ctrl+Z). `stop_on_error: true` revierte el lote entero.
- **Informe por nudo**: creada, creada con aviso, rehecha, fallida (con el código y el motivo), saltada (con el motivo) o
  revertida; cuentas, `connection_ids`, duración y un resumen en español ("Lote 4ef7dd3d: 15 conexiones creadas (14 con
  aviso), 1 falló (N7: …). Una sola entrada de deshacer (Ctrl+Z)."). Después de crear, los nudos salen `✔ Creada` /
  `✔ Creada con aviso` / `✖ Falló al crear` (con qué hacer), sus marcas desaparecen y la cabecera dice "Creadas 16
  conexiones (14 con aviso)". Un nudo fallido conserva su token: **Crear** otra vez solo crea los que faltan.
- **Borrar el lote**: `conn_batch_delete` y el botón **Borrar el lote** borran todas las conexiones del lote una a una con
  las garantías de `conn_delete` (solo lo que creó el add-in; barras restauradas), en una entrada de deshacer. `conn_list`
  enseña `batch_id` por conexión y filtra por lote.
- **Tus decisiones**: los 14 nudos con cordón HSS4X4 **se crean** con la cartela del Detalle D y el aviso de perfil (P6);
  los 10 nudos del cordón superior sin plantilla y los 7 empalmes **quedan fuera** (un `no_match` no se crea nunca).
- **Qué hacer con botones (C3)**: debajo de cada consejo, uno o dos botones (Excluir, Incluir, Incluir (rehacer), Cordón…,
  Barras…, Plantilla…, Editar nudo, Ver en Revit, Abrir catálogo). Los decide el Core y la IA los recibe en `actions`.
- **El consejo del empalme** (`fase-8.md` 12.1): cuando el cordón termina en el nudo y otro tramo sigue por el otro lado,
  el estado es `✖ Empalme del cordón` y el consejo "El cordón termina en este nudo (empalme): ninguna plantilla encaja
  con 2 diagonales; crea esa típica o excluye". Nunca "selecciónalo y replanifica".
- **Plan B sin recompilar**: `config/catalog.json` lleva `batch_single_undo` (`true` = una entrada de deshacer; `false` =
  una por nudo) por si el **sondeo 20** (nuevo: grupos anidados con Advance Steel) dijera que no conviven.
- **Cartelas fantasma (V2): no entraron** (no cabían sin recortar lo anterior). Siguen en la propuesta.
- Versión 0.9.0 en add-in, adaptador (25 rutas), herramientas (23) y simulador; contrato, guía, README y CLAUDE.md al día.

## 2. Qué se probó en la nube

- `dotnet build` sin avisos (0.9.0 compila contra la API 2027; los `TransactionGroup` anidados existen en la API, pero su
  convivencia con Advance Steel solo la prueba el PC).
- `dotnet test`: **192/192** (15 nuevas: el lote sobre la cercha de la 8b con un creador simulado, 16 creadas con 14
  avisos; un nudo que falla se revierte solo; `stop_on_error` con y sin grupo exterior; saltados por estado; token
  distinto del plan; `replace_existing` → rehecha; informe en JSON; borrado que devuelve los nudos a listos; "ya creada en
  este lote" tras replanificar; los botones por estado y el empalme detectado con dos tramos de cordón).
- Simulador **62/62** (crear, saltar la segunda vez, token falso, `stop_on_error`, `list` por lote, borrar, `BATCH_EMPTY`,
  `PLAN_NOT_FOUND`) y `probar_conexiones.py` **28/28** contra el simulador (el script sigue sin crear nada en el modelo).

## 3. Qué NO está probado (todo lo de Revit)

Los grupos anidados con la sesión de Advance Steel (sondeo 20), Ctrl+Z del lote entero, Crear y Borrar desde la ventana,
el informe en pantalla, los botones de Qué hacer, el consejo del empalme sobre N9 y lo pendiente de la Fase 8 (8.5, 12.5
y el sondeo 19 v4). Todo está en `docs/instalacion/fase-9.md`.

## 4. Qué hacer ahora

1. **Cerrar Revit** y pasar al instalador `docs\instalacion\fase-9.md` entero. Empieza por el **sondeo 20**: si dice
   "GRUPOS ANIDADOS OK", el lote será una sola entrada de deshacer; si no, el instalador pone `batch_single_undo: false`
   en el `catalog.json` desplegado (una entrada por nudo) y sigue.
2. Hacer los pasos marcados **(la persona)**: sobre todo **Crear 16 conexiones** desde la ventana (sobre la copia),
   **Ctrl+Z** una sola vez, crear otra vez y **Borrar el lote**, con los sondeos 12 y 13 a cero después. Cinco capturas
   (`fase9-01` a `fase9-05`).
3. Devolver `docs\fases\resultados-fase-9.md` con las anotaciones y abrir la sesión de cierre con este prompt:

   ```
   Lee CLAUDE.md, docs/fases/fase-9.md (secciones 5 y 6) y docs/fases/resultados-fase-9.md. Cierra la Fase 9: contrasta
   los resultados con lo esperado en 2.1 (sondeo 20, Crear 16 conexiones, Ctrl+Z, Borrar el lote, el puente), corrige lo
   que haga falta (ronda 9b solo si es imprescindible), actualiza el informe y la tabla de garantías del README. No
   empieces la Fase 10. Termina con commit, push y un resumen corto.
   ```
