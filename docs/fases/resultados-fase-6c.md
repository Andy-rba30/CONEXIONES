# Resultados de la ronda 6c

Fecha: 2026-10-01T12:45:12


## 6c-1 git

```text
6ce0dc4 Ronda 6c: el patrón de pernos se apoya en la cara exterior de la placa cuchilla

```

## 6c-1 build y test

```text
  Determinando los proyectos que se van a restaurar...
  Todos los proyectos están actualizados para la restauración.
  MotorConexiones.Core -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Core\bin\Release\netstandard2.0\MotorConexiones.Core.dll
  MotorConexiones.Tests -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll
  MotorConexiones.Revit -> D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Revit\bin\Release\net10.0-windows\MotorConexiones.Revit.dll

Compilación correcta.
    0 Advertencia(s)
    0 Errores

Tiempo transcurrido 00:00:07.29
Serie de pruebas para D:\Proyectos C#\CONEXIONES\src\MotorConexiones.Tests\bin\Release\net10.0\MotorConexiones.Tests.dll (.NETCoreApp,Version=v10.0)
1 archivos de prueba en total coincidieron con el patrón especificado.

Correctas! - Con error:     0, Superado:    99, Omitido:     0, Total:    99, Duración: 167 ms - MotorConexiones.Tests.dll (net10.0)

```

## 6c-1 revit cerrado

```text

```

## 6c-1 deploy

```text
== MotorConexiones 0.1.0.0 desplegado en Revit 2027 ==
Carpeta:     C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones
Manifiesto:  C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\MotorConexiones.addin
Copiados:    MotorConexiones.Core.dll, MotorConexiones.Core.pdb, MotorConexiones.Revit.dll, MotorConexiones.Revit.pdb, config\limits.json, docs\guide.md
Siguiente paso: abre Revit 2027. El panel MotorConexiones debe aparecer en la pestana 'ARBA' (o en 'Conexiones' si ARBA no se pudo usar; lo dice el log).

```

## 6c-3 sondeo 16 agarre

```text
== 16-pernos-agarre.py -> HTTP 200 en 2038 ms ==
=== 16-pernos-agarre ===
Bridge.Handle encontrado: True | version del ensamblado: 0.1.0.0
1) conexiones en el modelo: 1
   connection_id=3023ca69-8f6b-4694-bbfd-3e8934119228 | backend=advancesteel | elementos creados=9
   cartela 9.525 mm | placa cuchilla 10.0 mm | cara +z | agarre esperado 19.53 mm | paquete Z esperado -4.76 .. 14.76
2) origen (-11867.700000000001, -17195.799999999999, 17423.0) | Z local (normal a la cercha) = (-0.0, 1.0, 0.0)
   validate bolt_stacks: [{'bolt_length_mm': 44.450000000000003, 'gusset_face': '+z', 'grip_mm': 19.524999999999999, 'length_source': 'computed_from_grip', 'member_element_id': 1249636}]
3) intervalo Z local (mm) de cada elemento creado:
   [1321345] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321346] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321347] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321348] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321349] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321350] DirectShape | Structural Connections: z -2.50 .. 2.50 mm (espesor en Z 5.00; 1 solidos, 12 vertices)
   [1321351] SteelProxyElement | Plates: z -0.00 .. 9.52 mm (espesor en Z 9.53; 1 solidos, 48 vertices)
        Thickness: 0' - 0 3/8" = 9.53 mm
        Length: 1' - 10 1/4" = 565.0 mm
        Width: 1' - 8 7/8" = 530.0 mm
   [1321352] SteelProxyElement | Plates: z 9.76 .. 19.76 mm (espesor en Z 10.00; 1 solidos, 24 vertices)
        Thickness: 0' - 0 13/32" = 10.0 mm
        Length: 0' - 6 11/16" = 170.0 mm
        Width: 0' - 5 1/2" = 140.0 mm
   [1321353] SteelProxyElement | Bolts: z -29.69 .. 24.68 mm (espesor en Z 54.37; 20 solidos, 496 vertices)
        Diameter:  5/8 inch
        Bolt Length: 0' - 1 3/4" = 44.45 mm
        Grip Length: 0' - 0 25/32" = 19.52 mm
        Number on side 1: 2
        Number on side 2: 2
4) veredicto:
   placa cuchilla [1321351]: -0.00 .. 9.52 -> NO esta donde se esperaba (4.76 .. 14.76)
   placa cuchilla [1321352]: 9.76 .. 19.76 -> NO esta donde se esperaba (4.76 .. 14.76)
   pernos [1321353]: z -29.69 .. 24.68 (largo total 54.37 mm) -> atraviesan cartela + placa OK; sobresalen 24.9 mm por abajo y 9.9 mm por arriba
5) captura de perfil: no se pudo (). Haz la captura a mano (paso de la persona).
=== fin 16-pernos-agarre ===


```

## 6c-3 log pernos

```text

{"ts":"2026-10-01T12:49:50.7293707-05:00","record":{"event":"advance_steel_plate_written","name":"Cartela Detalle 
D","vertices":8,"thickness_mm":9.525,"offset_mm":0,"units":"mm"}}
{"ts":"2026-10-01T12:49:50.7606435-05:00","record":{"event":"advance_steel_plate_written","name":"Placa cuchilla 
miembro 1249636","vertices":4,"thickness_mm":10,"offset_mm":9.7625,"units":"mm"}}
{"ts":"2026-10-01T12:49:51.1487521-05:00","record":{"event":"advance_steel_bolts_written","name":"Pernos miembro 124963
6","properties":["Nx=2","Ny=2","Dx=60","Dy=60","ScrewDiameter=15.875","BindingLength=19.525","ScrewLength=44.4499999999
99996"],"count":4,"units":"mm","grip_mm":19.525,"bolt_length_mm":44.449999999999996,"length_from_spec":false,"plane_z_m
m":14.7625,"stack_min_z_mm":-4.7625,"gusset_face":"+z"}}



```

## 6c-4 sondeo 12

```text
== 12-fase3-borrar.py -> HTTP 200 en 501 ms ==
=== 12-fase3-borrar ===
--- list: ok=True en 14 ms | errores=- | avisos=-
    conexiones en el modelo: 0
--- list: ok=True en 4 ms | errores=- | avisos=-
    conexiones tras borrar: 0
    extensiones actuales de las barras del fixture (mm):
    barra 1249510: inicio -3.026 | fin -1.733
    barra 1249630: inicio 0.0 | fin 68.64
    barra 1249631: inicio 0.0 | fin 69.2
    barra 1249636: inicio 0.0 | fin 0.0
=== fin 12-fase3-borrar ===


```

## 6c-4 sondeo 13

```text
== 13-limpiar-fase1.py -> HTTP 200 en 38 ms ==
=== 13-limpiar-fase1 ===
1) Elementos de acero sueltos encontrados: 0
   nada que borrar


```
