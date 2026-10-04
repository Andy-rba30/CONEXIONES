# Plantillas oficiales del catálogo

Carpeta del repositorio para las plantillas "oficiales" de la oficina (decisión P2 de `docs/propuestas/catalogo-y-lotes.md`):
un archivo `<template_id>.json` por plantilla, con el mismo formato que escribe `conn_catalog_save` en
`%LOCALAPPDATA%\MotorConexiones\catalogo\`.

- `scripts\deploy.ps1` copia a la carpeta del catálogo del usuario las plantillas de aquí que falten (no sustituye las
  que ya existen). Así un PC nuevo arranca con las típicas de la oficina.
- Para subir una plantilla al repositorio: `conn_catalog_save` (o el botón **Guardar en catálogo** con la casilla
  "Copiar también a la carpeta compartida", que apunta a esta carpeta según `shared_catalog_folder` de
  `config\catalog.json`), copiar el archivo aquí si no se copió solo, `git add catalog\*.json`, commit y push.
- La primera plantilla real (el Detalle D del Hangar) la guarda el instalador en la Fase 7 (`docs/instalacion/fase-7.md`,
  paso 7-5) desde la conexión creada en el modelo: las plantillas se miden sobre el nudo real, no se escriben a mano.
