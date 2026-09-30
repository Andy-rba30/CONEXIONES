# Resultados de la Fase 0

Fecha: 2026-09-30T18:18:01

## Entorno

```text
620894c Fase 0: imagen del Detalle D, vista de la cercha y ruta del modelo de prueba
git version 2.55.0.windows.4
10.0.401 [C:\Program Files\dotnet\sdk]
```

## 00-version

```text
== 00-version.py -> HTTP 200 en 819 ms ==
=== 00-version ===
VersionNumber: 2027
VersionBuild: 27.2.0.39
VersionName: Autodesk Revit 2027
SubVersionNumber: 2027.2
Product: Revit
Language: English_USA
Revit.exe: C:\Program Files\Autodesk\Revit 2027\Revit.exe
Carpeta RevitAPI.dll: C:\Program Files\Autodesk\Revit 2027\RevitAPI.dll
Runtime .NET: .NET 10.0.12
Environment.Version: 10.0.12
IronPython sys.version: 2.7.12 (2.7.12.1000) [.NETStandard,Version=v2.0 on .NET 10.0.12 (64-bit)]
Proceso 64 bits: True
pyRevit: 6.5.3.26176+2017
pyRevit HOST_APP: version=2027 build=20260716_1515(x64) motor=?
Documento: HANGAR_PRUEBA
Ruta .rvt: D:\IG INGENIERÍA\Hartree\HANGAR_PRUEBA.rvt
Es familia: False
Unidades de longitud: autodesk.unit.unit:meters-1.0.0
Armazon estructural (instancias): 1064
Pilares estructurales (instancias): 145
Cerchas (instancias): 0
Conexiones estructurales (instancias): 6
=== fin 00-version ===

```

## 01-steel-api

```text
== 01-steel-api.py -> HTTP 200 en 376 ms ==
=== 01-steel-api ===
RevitAPI.dll: C:\Program Files\Autodesk\Revit 2027\RevitAPI.dll
Tipos en Autodesk.Revit.DB.Steel* (RevitAPI.dll): 1
  Autodesk.Revit.DB.Steel.SteelElementProperties
hasattr(DB, 'Steel'): True
Tipos con 'StructuralConnection' en RevitAPI.dll: 8
  Autodesk.Revit.DB.BuiltInFailures+StructuralConnectionFailures
  Autodesk.Revit.DB.Structure.StructuralConnectionApplyTo
  Autodesk.Revit.DB.Structure.StructuralConnectionApprovalType
  Autodesk.Revit.DB.Structure.StructuralConnectionCodeCheckingStatus
  Autodesk.Revit.DB.Structure.StructuralConnectionHandler
  Autodesk.Revit.DB.Structure.StructuralConnectionHandlerType
  Autodesk.Revit.DB.Structure.StructuralConnectionSettings
  Autodesk.Revit.DB.Structure.StructuralConnectionType
Carpeta de Revit: C:\Program Files\Autodesk\Revit 2027
DLL candidatas en la carpeta de Revit: 6
  RevitAPISteel.dll (291 KB)
  AssemblyDB.dll (1043 KB)
  AssemblyDBAPI.dll (1029 KB)
  AssemblyMFC.dll (189 KB)
  AssemblyUI.dll (121 KB)
  AsyncFriendlyStackTrace.dll (22 KB)
Intentos de carga (maximo 12 DLL):
- RevitAPISteel
    ya estaba cargado: RevitAPISteel, Version=27.2.0.0, Culture=neutral, PublicKeyToken=null
    tipos publicos: 32
    espacios de nombres (top 20):
      Autodesk.Revit.DB.Steel: 24
      Autodesk.Revit.DB.Structure: 6
      Autodesk.Revit.UI: 2
    tipos de interes (20, se muestran 20):
      Autodesk.Revit.DB.Steel.ISteelElement
      Autodesk.Revit.DB.Steel.ISteelModelCallback
      Autodesk.Revit.DB.Steel.ISteelModelLibrary
      Autodesk.Revit.DB.Steel.SteelBeamVoidInterval
      Autodesk.Revit.DB.Steel.SteelConnectionUtil
      Autodesk.Revit.DB.Steel.SteelElementGeometryData
      Autodesk.Revit.DB.Steel.SteelElementGeometryHistoryData
      Autodesk.Revit.DB.Steel.SteelElementParamType
      Autodesk.Revit.DB.Steel.SteelElementTempGeometryContainer
      Autodesk.Revit.DB.Steel.SteelMarksUtils
      Autodesk.Revit.DB.Steel.SteelModelFlavour
      Autodesk.Revit.DB.Steel.SteelModelInfo
      Autodesk.Revit.DB.Steel.SteelModelManager
      Autodesk.Revit.DB.Steel.SteelModelSaveBackup
      Autodesk.Revit.DB.Steel.SteelProxyElement
      Autodesk.Revit.DB.Steel.StructuralConnectionBaseUtil
      Autodesk.Revit.DB.Structure.IModifyConnectionParametersServer
      Autodesk.Revit.DB.Structure.IModifyConnectionRangesServer
      Autodesk.Revit.DB.Structure.ModifyConnectionParametersServiceData
      Autodesk.Revit.DB.Structure.ModifyConnectionRangesServiceData
- AssemblyDB
    AddReferenceToFileAndPath ERROR SystemError: Bad IL format. The format of the file 'C:\Program Files\Autodesk\Revit 2027\AssemblyDB.dll' is invalid.
- AssemblyDBAPI
    ya estaba cargado: AssemblyDBAPI, Version=27.2.0.0, Culture=neutral, PublicKeyToken=null
    tipos publicos: 0
    espacios de nombres (top 20):
    tipos de interes (0, se muestran 0):
- AssemblyMFC
    AddReferenceToFileAndPath ERROR SystemError: Bad IL format. The format of the file 'C:\Program Files\Autodesk\Revit 2027\AssemblyMFC.dll' is invalid.
- AssemblyUI
    AddReferenceToFileAndPath ERROR SystemError: Bad IL format. The format of the file 'C:\Program Files\Autodesk\Revit 2027\AssemblyUI.dll' is invalid.
- AsyncFriendlyStackTrace
    ya estaba cargado: AsyncFriendlyStackTrace, Version=1.6.0.0, Culture=neutral, PublicKeyToken=f5fdf019656607c4
    tipos publicos: 2
    espacios de nombres (top 20):
      AsyncFriendlyStackTrace: 2
    tipos de interes (0, se muestran 0):
=== fin 01-steel-api ===

```

## 02-perfiles

```text
== 02-perfiles.py -> HTTP 200 en 429 ms ==
=== 02-perfiles ===
Tipos de armazon estructural cargados: 163
- [1982832] familia='ANCLAJE_Ø7-8- 22x22' tipo='ANCLAJE_Ø7-8- 22x22' instancias=72 ids=[2372362, 2372363, 2372364, 2372365, 2372366, 2372367]
    seccion: forma=RoundBar clase=StructuralSectionRoundBar Diameter=22.23mm
- [1722761] familia='Barra redonda' tipo='25mm' instancias=0 ids=[]
- [1723461] familia='Barras redondas' tipo='RB 10' instancias=0 ids=[]
- [1723587] familia='Barras redondas' tipo='RB 100' instancias=0 ids=[]
- [1723589] familia='Barras redondas' tipo='RB 105' instancias=0 ids=[]
- [1723591] familia='Barras redondas' tipo='RB 110' instancias=0 ids=[]
- [1723463] familia='Barras redondas' tipo='RB 12' instancias=0 ids=[]
- [1723465] familia='Barras redondas' tipo='RB 14' instancias=0 ids=[]
- [1723467] familia='Barras redondas' tipo='RB 15' instancias=0 ids=[]
- [1723469] familia='Barras redondas' tipo='RB 16' instancias=0 ids=[]
- [1723471] familia='Barras redondas' tipo='RB 17' instancias=0 ids=[]
- [1723473] familia='Barras redondas' tipo='RB 18' instancias=0 ids=[]
- [1723475] familia='Barras redondas' tipo='RB 19' instancias=0 ids=[]
- [1723477] familia='Barras redondas' tipo='RB 20' instancias=0 ids=[]
- [1723479] familia='Barras redondas' tipo='RB 22' instancias=0 ids=[]
- [1723481] familia='Barras redondas' tipo='RB 22.25' instancias=0 ids=[]
- [1723483] familia='Barras redondas' tipo='RB 23.6' instancias=0 ids=[]
- [1723485] familia='Barras redondas' tipo='RB 24' instancias=0 ids=[]
- [1723487] familia='Barras redondas' tipo='RB 24.5' instancias=0 ids=[]
- [1723489] familia='Barras redondas' tipo='RB 25' instancias=0 ids=[]
- [1723491] familia='Barras redondas' tipo='RB 26' instancias=0 ids=[]
- [1723493] familia='Barras redondas' tipo='RB 26.7' instancias=0 ids=[]
- [1723495] familia='Barras redondas' tipo='RB 27' instancias=0 ids=[]
- [1723497] familia='Barras redondas' tipo='RB 28' instancias=0 ids=[]
- [1723499] familia='Barras redondas' tipo='RB 29' instancias=0 ids=[]
- [1723501] familia='Barras redondas' tipo='RB 29.5' instancias=0 ids=[]
- [1723503] familia='Barras redondas' tipo='RB 29.7' instancias=0 ids=[]
- [1723505] familia='Barras redondas' tipo='RB 30' instancias=0 ids=[]
- [1723507] familia='Barras redondas' tipo='RB 31' instancias=0 ids=[]
- [1723509] familia='Barras redondas' tipo='RB 32' instancias=0 ids=[]
- [1723511] familia='Barras redondas' tipo='RB 34' instancias=0 ids=[]
- [1723513] familia='Barras redondas' tipo='RB 34.4' instancias=0 ids=[]
- [1723515] familia='Barras redondas' tipo='RB 35' instancias=0 ids=[]
- [1723517] familia='Barras redondas' tipo='RB 35.7' instancias=0 ids=[]
- [1723519] familia='Barras redondas' tipo='RB 36' instancias=0 ids=[]
- [1723521] familia='Barras redondas' tipo='RB 37' instancias=0 ids=[]
- [1723523] familia='Barras redondas' tipo='RB 38' instancias=0 ids=[]
- [1723525] familia='Barras redondas' tipo='RB 39' instancias=0 ids=[]
- [1723527] familia='Barras redondas' tipo='RB 39.2' instancias=0 ids=[]
- [1723529] familia='Barras redondas' tipo='RB 40' instancias=0 ids=[]
- [1723531] familia='Barras redondas' tipo='RB 42' instancias=0 ids=[]
- [1723533] familia='Barras redondas' tipo='RB 44' instancias=0 ids=[]
- [1723535] familia='Barras redondas' tipo='RB 45' instancias=0 ids=[]
- [1723537] familia='Barras redondas' tipo='RB 46' instancias=0 ids=[]
- [1723539] familia='Barras redondas' tipo='RB 47' instancias=0 ids=[]
- [1723541] familia='Barras redondas' tipo='RB 48' instancias=0 ids=[]
- [1723543] familia='Barras redondas' tipo='RB 49.2' instancias=0 ids=[]
- [1723545] familia='Barras redondas' tipo='RB 50' instancias=0 ids=[]
- [1723547] familia='Barras redondas' tipo='RB 51' instancias=0 ids=[]
- [1723549] familia='Barras redondas' tipo='RB 52' instancias=0 ids=[]
- [1723551] familia='Barras redondas' tipo='RB 53' instancias=0 ids=[]
- [1723553] familia='Barras redondas' tipo='RB 54' instancias=0 ids=[]
- [1723555] familia='Barras redondas' tipo='RB 55' instancias=0 ids=[]
- [1723557] familia='Barras redondas' tipo='RB 55.8' instancias=0 ids=[]
- [1723559] familia='Barras redondas' tipo='RB 56' instancias=0 ids=[]
- [1723561] familia='Barras redondas' tipo='RB 57' instancias=0 ids=[]
- [1723563] familia='Barras redondas' tipo='RB 58' instancias=0 ids=[]
- [1723565] familia='Barras redondas' tipo='RB 59' instancias=0 ids=[]
- [1723567] familia='Barras redondas' tipo='RB 60' instancias=0 ids=[]
- [1723569] familia='Barras redondas' tipo='RB 62' instancias=0 ids=[]
- [1723571] familia='Barras redondas' tipo='RB 63' instancias=0 ids=[]
- [1723573] familia='Barras redondas' tipo='RB 65' instancias=0 ids=[]
- [1723575] familia='Barras redondas' tipo='RB 70' instancias=0 ids=[]
- [1723577] familia='Barras redondas' tipo='RB 75' instancias=0 ids=[]
- [1723579] familia='Barras redondas' tipo='RB 80' instancias=0 ids=[]
- [1723581] familia='Barras redondas' tipo='RB 85' instancias=0 ids=[]
- [1723583] familia='Barras redondas' tipo='RB 90' instancias=0 ids=[]
- [1723585] familia='Barras redondas' tipo='RB 95' instancias=0 ids=[]
- [1987607] familia='C8X11.5 57x203' tipo='C8X11.5 57x203' instancias=8 ids=[2256960, 2256962, 2258611, 2258612, 2259890, 2259892]
    seccion: forma=NotDefined clase=StructuralSectionGeneralU Width=57.15mm Height=203.2mm FlangeThickness=9.53mm WebThickness=6.35mm
- [1970740] familia='CA9-X3- 76x229' tipo='CA9-X3- 76x229' instancias=0 ids=[]
- [1762844] familia='CANALETA PLUVIAL' tipo='Canaleta Pluvial' instancias=0 ids=[]
- [1980034] familia='ESPARRAGO 5-8 16x16' tipo='ESPARRAGO 5-8 16x16' instancias=48 ids=[2372318, 2372319, 2372320, 2372321, 2372322, 2372323]
    seccion: forma=RoundBar clase=StructuralSectionRoundBar Diameter=15.88mm
- [2356958] familia='HSS-Hollow Structural Section' tipo='HSS12X8X1/2' instancias=24 ids=[2247473, 2247475, 2247477, 2247478, 2247479, 2247480]
    seccion: forma=NotDefined clase=StructuralSectionGeneralH Width=203.2mm Height=304.8mm WallNominalThickness=11.81mm WallDesignThickness=0.0mm
- [2386015] familia='HSS-Hollow Structural Section' tipo='HSS2-1/2X2-1/2X3/16' instancias=0 ids=[]
    seccion: sin seccion estructural
- [2393083] familia='HSS-Hollow Structural Section' tipo='HSS3X3X1/4' instancias=12 ids=[2372326, 2372327, 2372328, 2372329, 2372346, 2372347]
    seccion: forma=NotDefined clase=StructuralSectionGeneralH Width=76.2mm Height=76.2mm WallNominalThickness=5.92mm WallDesignThickness=0.0mm
- [2367491] familia='HSS-Hollow Structural Section' tipo='HSS4X3X1/4' instancias=14 ids=[2372311, 2372312, 2372313, 2372314, 2372315, 2372316]
    seccion: forma=NotDefined clase=StructuralSectionGeneralH Width=76.2mm Height=101.6mm WallNominalThickness=5.92mm WallDesignThickness=0.0mm
- [2371865] familia='HSS-Hollow Structural Section' tipo='HSS5X5X3/8' instancias=36 ids=[2372343, 2372345, 2372728, 2372730, 2372797, 2372799]
    seccion: forma=NotDefined clase=StructuralSectionGeneralH Width=127.0mm Height=127.0mm WallNominalThickness=8.86mm WallDesignThickness=0.0mm
- [1773478] familia='HSS-Sección estructural hueca' tipo='CV-1 (2"x2")' instancias=0 ids=[]
    seccion: sin seccion estructural
- [1773476] familia='HSS-Sección estructural hueca' tipo='CV-2 (2"x3")' instancias=0 ids=[]
    seccion: sin seccion estructural
- [1773474] familia='HSS-Sección estructural hueca' tipo='CV-3 (2"x6"x2.5mm)' instancias=0 ids=[]
    seccion: sin seccion estructural
- [1975868] familia='HSS12X8X3-8 203x305' tipo='HSS12X8X3-8 203x305' instancias=0 ids=[]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=203.2mm Height=304.8mm WallNominalThickness=9.52mm WallDesignThickness=9.52mm
- [1986326] familia='HSS2-1-2X2-1-2X3-16 64x64' tipo='HSS2-1-2X2-1-2X3-16 64x64' instancias=436 ids=[2372418, 2372419, 2372420, 2372421, 2372457, 2372458]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=63.5mm Height=63.5mm WallNominalThickness=4.76mm WallDesignThickness=4.76mm
- [1976689] familia='HSS3X3X1-4 76x76' tipo='HSS3X3X1-4 76x76' instancias=152 ids=[2372289, 2372290, 2372291, 2372292, 2372293, 2372294]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=76.2mm Height=76.2mm WallNominalThickness=6.35mm WallDesignThickness=6.35mm
- [1978379] familia='HSS3X3X3-8 76x76' tipo='HSS3X3X3-8 76x76' instancias=0 ids=[]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=76.2mm Height=76.2mm WallNominalThickness=9.52mm WallDesignThickness=9.52mm
- [1977492] familia='HSS4X3X3-16 76x102' tipo='HSS4X3X3-16 76x102' instancias=0 ids=[]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=76.2mm Height=101.6mm WallNominalThickness=4.76mm WallDesignThickness=4.76mm
- [1975058] familia='HSS4X4X3-16 102x102' tipo='HSS4X4X3-16 102x102' instancias=16 ids=[2372281, 2372282, 2372283, 2372284, 2372285, 2372286]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=101.6mm Height=101.6mm WallNominalThickness=4.76mm WallDesignThickness=4.76mm
- [1973268] familia='HSS5X5X3-8 127x127' tipo='HSS5X5X3-8 127x127' instancias=0 ids=[]
    seccion: forma=RectangleHSS clase=StructuralSectionRectangleHSS Width=127.0mm Height=127.0mm WallNominalThickness=9.52mm WallDesignThickness=9.52mm
- [1984621] familia='L2X2X1-4 51x51' tipo='L2X2X1-4 51x51' instancias=44 ids=[2257759, 2257761, 2257762, 2257763, 2257764, 2257765]
    seccion: forma=LAngle clase=StructuralSectionLAngle Width=50.8mm Height=50.8mm FlangeThickness=6.35mm WebThickness=6.35mm
- [1972440] familia='L3X3X1-4 76x76' tipo='L3X3X1-4 76x76' instancias=2 ids=[2258578, 2258580]
    seccion: forma=LAngle clase=StructuralSectionLAngle Width=76.2mm Height=76.2mm FlangeThickness=6.35mm WebThickness=6.35mm
- [1981113] familia='L3X3X3-8 76x76' tipo='L3X3X3-8 76x76' instancias=28 ids=[2372348, 2372349, 2372350, 2372351, 2372352, 2372353]
    seccion: forma=LAngle clase=StructuralSectionLAngle Width=76.2mm Height=76.2mm FlangeThickness=9.52mm WebThickness=9.52mm
- [1547594] familia='M_C Shapes' tipo='C150X15.6' instancias=0 ids=[]
- [1547592] familia='M_C Shapes' tipo='C310X30.8' instancias=0 ids=[]
- [1547590] familia='M_C Shapes' tipo='C380X50.4' instancias=0 ids=[]
- [1115886] familia='M_SF_Reference Beam' tipo='M_SF_Reference Beam' instancias=0 ids=[]
- [1548682] familia='M_W Shapes' tipo='W1000X222' instancias=0 ids=[]
- [1548680] familia='M_W Shapes' tipo='W1000X249' instancias=0 ids=[]
- [1548702] familia='M_W Shapes' tipo='W150X13' instancias=0 ids=[]
- [1548700] familia='M_W Shapes' tipo='W200X15' instancias=0 ids=[]
- [1548698] familia='M_W Shapes' tipo='W310X21' instancias=0 ids=[]
- [1548696] familia='M_W Shapes' tipo='W360X32.9' instancias=0 ids=[]
- [1548694] familia='M_W Shapes' tipo='W410X38.8' instancias=0 ids=[]
- [1548692] familia='M_W Shapes' tipo='W410X46.1' instancias=0 ids=[]
- [1548690] familia='M_W Shapes' tipo='W410X53' instancias=0 ids=[]
- [1548688] familia='M_W Shapes' tipo='W760X134' instancias=0 ids=[]
- [1548686] familia='M_W Shapes' tipo='W920X201' instancias=0 ids=[]
- [1548684] familia='M_W Shapes' tipo='W920X223' instancias=0 ids=[]
- [1982035] familia='PRUEBA 900x50' tipo='PRUEBA 900x50' instancias=0 ids=[]
- [1784864] familia='VIGA METALICA' tipo='Viga Metálica LAC' instancias=0 ids=[]
- [1786295] familia='VIGA METALICA 2 niveles' tipo='Viga Metálica LAC' instancias=0 ids=[]
- [1788212] familia='VIGA TIMPANO_1' tipo='Viga Tímpano 2' instancias=0 ids=[]
- [1789515] familia='VIGA TIMPANO_2' tipo='Viga Tímpano' instancias=0 ids=[]
- [1780258] familia='Viga Cumbrera' tipo='V cumbrera' instancias=0 ids=[]
- [1780260] familia='Viga Cumbrera' tipo='Viga Cumbrera' instancias=0 ids=[]
- [1787029] familia='Viga Rectangular' tipo='0.10x0.15' instancias=0 ids=[]
- [1787045] familia='Viga Rectangular' tipo='0.10x0.20' instancias=0 ids=[]
- [1787039] familia='Viga Rectangular' tipo='0.10x0.25' instancias=0 ids=[]
- [1787047] familia='Viga Rectangular' tipo='0.12x0.10' instancias=0 ids=[]
- [1787033] familia='Viga Rectangular' tipo='0.12x0.15' instancias=0 ids=[]
- [1787043] familia='Viga Rectangular' tipo='0.12x0.20' instancias=0 ids=[]
- [1787035] familia='Viga Rectangular' tipo='0.12x0.40' instancias=0 ids=[]
- [1787041] familia='Viga Rectangular' tipo='0.15x0.20' instancias=0 ids=[]
- [1787025] familia='Viga Rectangular' tipo='0.15x0.30' instancias=0 ids=[]
- [1787031] familia='Viga Rectangular' tipo='0.15x0.35' instancias=0 ids=[]
- [1787037] familia='Viga Rectangular' tipo='0.20x0.20' instancias=0 ids=[]
- [1787021] familia='Viga Rectangular' tipo='0.25x0.20' instancias=0 ids=[]
- [1787027] familia='Viga Rectangular' tipo='0.25x0.25' instancias=0 ids=[]
- [1787019] familia='Viga Rectangular' tipo='0.25x0.30' instancias=0 ids=[]
- [1787017] familia='Viga Rectangular' tipo='0.25x0.35' instancias=0 ids=[]
- [1787023] familia='Viga Rectangular' tipo='0.25x0.40' instancias=0 ids=[]
- [1787015] familia='Viga Rectangular' tipo='0.25x0.50' instancias=0 ids=[]
- [1974256] familia='Viga Rectangular' tipo='150x100mm (IFC)' instancias=7 ids=[2245854, 2245855, 2245858, 2245859, 2245860, 2245861]
    seccion: forma=ConcreteRectangle clase=StructuralSectionConcreteRectangle Width=150.0mm Height=100.0mm
- [1969914] familia='Viga Rectangular' tipo='150x250mm (IFC)' instancias=8 ids=[2245839, 2245840, 2245841, 2245842, 2245843, 2245844]
    seccion: forma=ConcreteRectangle clase=StructuralSectionConcreteRectangle Width=150.0mm Height=250.0mm
- [1969901] familia='Viga Rectangular' tipo='150x300mm (IFC)' instancias=8 ids=[2245835, 2245846, 2245847, 2245848, 2245849, 2245850]
    seccion: forma=ConcreteRectangle clase=StructuralSectionConcreteRectangle Width=150.0mm Height=300.0mm
- [1973429] familia='Viga Rectangular' tipo='150x400mm (IFC)' instancias=3 ids=[2245851, 2245852, 2245853]
    seccion: forma=ConcreteRectangle clase=StructuralSectionConcreteRectangle Width=150.0mm Height=400.0mm
- [1969906] familia='Viga Rectangular' tipo='600x600mm (IFC)' instancias=3 ids=[2245836, 2245837, 2245838]
    seccion: forma=ConcreteRectangle clase=StructuralSectionConcreteRectangle Width=600.0mm Height=600.0mm
- [1787013] familia='Viga Rectangular' tipo='Vigueta (0.10x0.20)' instancias=0 ids=[]
- [1790323] familia='Viga Trapecio' tipo='Viga de Borde Frontal 02' instancias=0 ids=[]
- [1790325] familia='Viga Trapecio' tipo='Viga de Borde Posterior 02' instancias=0 ids=[]
- [1780985] familia='Viga de Cimentación' tipo='0.20X0.50' instancias=0 ids=[]
- [1780983] familia='Viga de Cimentación' tipo='0.25X0.40' instancias=0 ids=[]
- [1780981] familia='Viga de Cimentación' tipo='0.25X0.45' instancias=0 ids=[]
- [1780991] familia='Viga de Cimentación' tipo='0.25X0.50' instancias=0 ids=[]
- [1780979] familia='Viga de Cimentación' tipo='0.30X0.50' instancias=0 ids=[]
- [1780987] familia='Viga de Cimentación' tipo='0.30X0.55' instancias=0 ids=[]
- [1780997] familia='Viga de Cimentación' tipo='0.30X1.50' instancias=0 ids=[]
- [1780995] familia='Viga de Cimentación' tipo='0.30X1.65' instancias=0 ids=[]
- [1780977] familia='Viga de Cimentación' tipo='0.35X0.70' instancias=0 ids=[]
- [1780993] familia='Viga de Cimentación' tipo='0.40X1.50' instancias=0 ids=[]
- [1780989] familia='Viga de Cimentación' tipo='0.50X0.30' instancias=0 ids=[]
- [1781817] familia='Viga de Seccion Variable' tipo='VP-(0.25xVARIABLE)' instancias=0 ids=[]
- [1781815] familia='Viga de Seccion Variable' tipo='VP-(0.30xVARIABLE)' instancias=0 ids=[]
- [1782635] familia='Viga de Seccion Variable Inverso' tipo='VP-(0.25xVARIABLE)' instancias=0 ids=[]
- [1783370] familia='Viga en L' tipo='0.30x0.55' instancias=0 ids=[]
- [1791045] familia='Vigueta de Confinamiento' tipo='VA-01 ()' instancias=0 ids=[]
- [1791041] familia='Vigueta de Confinamiento' tipo='VA-01 (0.13x0.10)' instancias=0 ids=[]
- [1791049] familia='Vigueta de Confinamiento' tipo='VA-01 (0.13x0.20)' instancias=0 ids=[]
- [1791047] familia='Vigueta de Confinamiento' tipo='VA-02 (0.15x0.20)' instancias=0 ids=[]
- [1791043] familia='Vigueta de Confinamiento' tipo='VA-1 (0.13x0.15)' instancias=0 ids=[]
- [1969868] familia='W12X26 165x310' tipo='W12X26 165x310' instancias=128 ids=[2372330, 2372331, 2372332, 2372333, 2372334, 2372335]
    seccion: forma=IParallelFlange clase=StructuralSectionIParallelFlange Width=165.0mm Height=310.0mm FlangeThickness=9.7mm WebThickness=5.84mm
- [1985518] familia='W12X30 166x313' tipo='W12X30 166x313' instancias=0 ids=[]
- [1974233] familia='W18X50 190x457' tipo='W18X50 190x457' instancias=0 ids=[]
- [1979201] familia='W24X68 228x603' tipo='W24X68 228x603' instancias=6 ids=[2251607, 2251609, 2251611, 2251627, 2251629, 2251630]
    seccion: forma=IParallelFlange clase=StructuralSectionIParallelFlange Width=228.0mm Height=603.0mm FlangeThickness=14.9mm WebThickness=10.5mm
- [1983813] familia='W8X21 134x210' tipo='W8X21 134x210' instancias=9 ids=[2256149, 2256151, 2256153, 2256154, 2256156, 2258581]
    seccion: forma=IParallelFlange clase=StructuralSectionIParallelFlange Width=133.86mm Height=210.31mm FlangeThickness=10.16mm WebThickness=6.35mm
Instancias de armazon estructural (maximo 30):
  [2245835] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (214356.180000, -123499.260000, 9594.4) -> (202809.550000, -107008.880000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245836] Viga Rectangular : 600x600mm (IFC) | Beam | material=sin material | (211960.160000, -125176.970000, 5944.4) -> (214356.180000, -123499.260000, 5944.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245837] Viga Rectangular : 600x600mm (IFC) | Beam | material=sin material | (200413.530000, -108686.590000, 5944.4) -> (202809.550000, -107008.880000, 5944.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245838] Viga Rectangular : 600x600mm (IFC) | Beam | material=sin material | (214356.180000, -123499.260000, 5944.4) -> (202809.550000, -107008.880000, 5944.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245839] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (214226.550000, -123314.130000, 8869.4) -> (213768.270000, -122659.630000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245840] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (213011.150000, -121578.350000, 8869.4) -> (212552.290000, -120923.020000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245841] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (211910.460000, -120006.390000, 8869.4) -> (211451.600000, -119351.070000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245842] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (210237.920000, -117617.740000, 8869.4) -> (208907.800000, -115718.120000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245843] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (207324.160000, -113456.440000, 8869.4) -> (205603.430000, -110998.980000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245844] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (204487.260000, -109404.910000, 8869.4) -> (203626.900000, -108176.180000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245845] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (203454.820000, -107930.430000, 8869.4) -> (202995.960000, -107275.110000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245846] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (210781.020000, -123231.450000, 9594.4) -> (212992.730000, -121682.800000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245847] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (208007.780000, -119270.850000, 9594.4) -> (210219.5, -117722.190000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245848] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (205923.410000, -116294.040000, 9594.4) -> (208135.130000, -114745.390000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245849] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (203229.910000, -112447.300000, 9594.4) -> (205441.620000, -110898.650000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245850] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (201310.730000, -109706.410000, 9594.4) -> (203522.440000, -108157.760000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245851] Viga Rectangular : 150x400mm (IFC) | Beam | material=sin material | (214356.180000, -123499.260000, 6444.4) -> (202809.550000, -107008.880000, 6444.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245852] Viga Rectangular : 150x400mm (IFC) | Beam | material=sin material | (211960.160000, -125176.970000, 6444.4) -> (214356.180000, -123499.260000, 6444.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245853] Viga Rectangular : 150x400mm (IFC) | Beam | material=sin material | (200413.530000, -108686.590000, 6444.4) -> (202809.550000, -107008.880000, 6444.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245854] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (203454.820000, -107930.430000, 8394.4) -> (202995.960000, -107275.110000, 8394.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245855] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (204487.260000, -109404.910000, 7594.4) -> (203626.900000, -108176.180000, 7594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245856] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (200536.400000, -108600.550000, 9594.4) -> (202809.550000, -107008.880000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245857] Viga Rectangular : 150x250mm (IFC) | Beam | material=sin material | (208907.800000, -115718.120000, 8869.4) -> (208405.920000, -115001.360000, 8869.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245858] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (208864.780000, -115656.690000, 8394.4) -> (208405.920000, -115001.360000, 8394.41) mm | retiros inicio=0.0mm fin=0.0mm
  [2245859] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (207324.160000, -113456.440000, 7594.4) -> (205603.430000, -110998.980000, 7594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2245860] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (211910.460000, -120006.390000, 8394.4) -> (211451.600000, -119351.070000, 8394.41) mm | retiros inicio=0.0mm fin=0.0mm
  [2245861] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (213011.150000, -121578.350000, 8394.4) -> (212552.290000, -120923.020000, 8394.41) mm | retiros inicio=0.0mm fin=0.0mm
  [2245862] Viga Rectangular : 150x100mm (IFC) | Beam | material=sin material | (214227.130000, -123314.950000, 8394.4) -> (213768.270000, -122659.630000, 8394.41) mm | retiros inicio=0.0mm fin=0.0mm
  [2245863] Viga Rectangular : 150x300mm (IFC) | Beam | material=sin material | (212083.030000, -125090.930000, 9594.4) -> (214356.180000, -123499.260000, 9594.4) mm | retiros inicio=0.0mm fin=0.0mm
  [2247473] HSS-Hollow Structural Section : HSS12X8X1/2 | Beam | material=Steel ASTM A500, Grade B, Rectangular and Square | (175202.640000, -41238.4200000, 19933.0) -> (164952.640000, -41238.3900000, 19933.0) mm | retiros inicio=0.0mm fin=0.0mm
  ... (hay mas)
=== fin 02-perfiles ===

```

## 03-llamar-dll

```text
== 03-llamar-dll.py -> HTTP 200 en 347 ms ==
=== 03-llamar-dll ===
1) RevitAPI en AppDomain: SI RevitAPI, Version=27.2.0.0, Culture=neutral, PublicKeyToken=null
   Location: C:\Program Files\Autodesk\Revit 2027\RevitAPI.dll
   AssemblyLoadContext: Default (DefaultAssemblyLoadContext)
2a) UnitUtils.ConvertToInternalUnits(1000 mm) por reflexion = 3.28083989501 pies (esperado 3.28084)
2b) Document.GetDocumentVersion(doc) por reflexion: GUID=2517b758-2d6a-4b00-ad21-e6936a982b39 guardados=1
2c) revit.uidoc disponible: True; tipo: Autodesk.Revit.UI.UIDocument
2c) firma (String, String, Document) construida: String, String, Document
3) RevitAddInUtility.dll existe: True (C:\Program Files\Autodesk\Revit 2027\RevitAddInUtility.dll)
   ya cargado antes: True
   cargado ahora: True en contexto Default (DefaultAssemblyLoadContext)
   RevitProductUtility.GetAllInstalledRevitProducts(): 3 productos
     - Revit 2026 | Revit2026 | C:\Program Files\Autodesk\Revit 2026\
     - Revit 2025 | Revit2025 | C:\Program Files\Autodesk\Revit 2025\
     - Revit 2027 | Revit2027 | C:\Program Files\Autodesk\Revit 2027\
4) Ensamblados fuera de la carpeta de Revit y del runtime (add-ins), con su AssemblyLoadContext:
   Autodesk.Assistant.Application | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Autodesk.Assistant.Application.dll
   Autodesk.Assistant.Tools | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Autodesk.Assistant.Tools.dll
   Autodesk.Assistant.Application.resources | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\en-US\Autodesk.Assistant.Application.resources.dll
   Microsoft.Extensions.Hosting | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Hosting.dll
   Microsoft.Extensions.Hosting.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Hosting.Abstractions.dll
   Microsoft.Extensions.Logging.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.Abstractions.dll
   Microsoft.Extensions.Diagnostics.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Diagnostics.Abstractions.dll
   Microsoft.Extensions.DependencyInjection.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.DependencyInjection.Abstractions.dll
   Microsoft.Extensions.Configuration.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.Abstractions.dll
   Microsoft.Extensions.Logging | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.dll
   ModelContextProtocol | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\ModelContextProtocol.dll
   ModelContextProtocol.Core | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\ModelContextProtocol.Core.dll
   Microsoft.Extensions.Configuration | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.dll
   Microsoft.Extensions.Primitives | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Primitives.dll
   Microsoft.Extensions.DependencyInjection | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.DependencyInjection.dll
   Microsoft.Extensions.Configuration.EnvironmentVariables | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.EnvironmentVariables.dll
   Microsoft.Extensions.FileProviders.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.FileProviders.Abstractions.dll
   Microsoft.Extensions.FileProviders.Physical | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.FileProviders.Physical.dll
   Microsoft.Extensions.Configuration.FileExtensions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.FileExtensions.dll
   Microsoft.Extensions.Configuration.CommandLine | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.CommandLine.dll
   Microsoft.Extensions.Options | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Options.dll
   Microsoft.Extensions.Diagnostics | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Diagnostics.dll
   Microsoft.Extensions.Configuration.Json | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.Json.dll
   Microsoft.Extensions.Configuration.UserSecrets | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.UserSecrets.dll
   Microsoft.Extensions.Configuration.Binder | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Configuration.Binder.dll
   Microsoft.Extensions.FileSystemGlobbing | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.FileSystemGlobbing.dll
   Microsoft.Extensions.Logging.Configuration | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.Configuration.dll
   Microsoft.Extensions.Logging.Debug | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.Debug.dll
   Microsoft.Extensions.Logging.EventSource | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.EventSource.dll
   Microsoft.Extensions.Logging.EventLog | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.EventLog.dll
   Microsoft.Extensions.Logging.Console | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Logging.Console.dll
   Microsoft.Extensions.Options.ConfigurationExtensions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.Options.ConfigurationExtensions.dll
   Microsoft.Extensions.AI.Abstractions | AUTODESKASSISTANT (AddInLoadContext) | C:\Program Files\Autodesk\Revit Assistant 2027\27.2.0\Microsoft.Extensions.AI.Abstractions.dll
   StripFootingRebar | Default (DefaultAssemblyLoadContext) | C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\StripFootingRebar\StripFootingRebar.dll
   BeamRebar | Default (DefaultAssemblyLoadContext) | C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\BeamRebar\BeamRebar.dll
   ColumnRebar | Default (DefaultAssemblyLoadContext) | C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027\ColumnRebar\ColumnRebar.dll
   KallpaBIM | Default (DefaultAssemblyLoadContext) | C:\Program Files\Autodesk\Revit\Addins\2027\KallpaBIM\KallpaBIM.dll
   pyRevitLoader | Default (DefaultAssemblyLoadContext) | C:\Program Files\pyRevit-Master\bin\netcore\engines\IPY2712PR\pyRevitLoader.dll
   Mono.Unix | Default (DefaultAssemblyLoadContext) | C:\Program Files\pyRevit-Master\bin\netcore\engines\IPY2712PR\Mono.Unix.dll
   pyRevitAssemblyBuilder | Default (DefaultAssemblyLoadContext) | C:\Program Files\pyRevit-Master\bin\netcore\engines\IPY2712PR\pyRevitAssemblyBuilder.dll
   ... (hay mas)
   total ensamblados en AppDomain: 672
5) SchemaBuilder.AcceptableName('MotorConexiones.Connection') = False
5) SchemaBuilder.AcceptableName('MotorConexionesConnection') = True
5) SchemaBuilder.AcceptableName('MotorConexiones_Connection') = True
=== fin 03-llamar-dll ===

```

## 04-ensamblados

```text
== 04-ensamblados.py -> HTTP 200 en 383 ms ==
=== 04-ensamblados ===
1) Ensamblados cargados relacionados con acero:
   Autodesk.SteelConnectionsDB | version 27.2.0.0 | Default | C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\Autodesk.SteelConnectionsDB.dll
   RevitAPISteel | version 27.2.0.0 | Default | C:\Program Files\Autodesk\Revit 2027\RevitAPISteel.dll
   Autodesk.SteelConnectionsDB.resources | version 27.0.4.0 | Default | C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\en-US\Autodesk.SteelConnectionsDB.resources.dll
   Autodesk.SteelConnectionsUI | version 27.2.0.0 | Default | C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\Autodesk.SteelConnectionsUI.dll
   Autodesk.SteelConnections.ASRvtFamilyMapping | version 27.2.0.0 | Default | C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections\Autodesk.SteelConnections.ASRvtFamilyMapping.dll
   total: 5
2) Archivos en la carpeta de Revit (C:\Program Files\Autodesk\Revit 2027):
   AssemblyDB.dll (1043 KB)
   AssemblyDBAPI.dll (1029 KB)
   AssemblyMFC.dll (189 KB)
   AssemblyUI.dll (121 KB)
   AsyncFriendlyStackTrace.dll (22 KB)
   RevitAPISteel.dll (291 KB)
   total: 6
   subcarpetas con 'steel' o AddIns: ['AddIns']
3) Complementos:
   C:\ProgramData\Autodesk\ApplicationPlugins: 4 entradas
     3dsMax_Navisworks_Exporter_2025.Addin.bundle
     AutoCAD_Navisworks_Exporter_2025.Addin.bundle
     ReCapMeshRevit.bundle
     Structural Toolkit for Revit 2020.bundle
   (no existe) C:\ProgramData\Autodesk\Revit\Addins\2027
   C:\Users\Andy Bayona Antón\AppData\Roaming\Autodesk\Revit\Addins\2027: 13 entradas
     BeamRebar
     ColumnRebar
     ColumnRebar.addin
     ColumnRebar_Vigas.addin
     ColumnRebar_Vigas_Cimientos.addin
     RetainingWallFormwork
     RetainingWallFormwork.addin
     RetainingWallRebar
     RetainingWallRebar.addin
     StripFootingRebar
     TiposBarraPeru
     TiposBarraPeru.addin
     pyRevit.addin
4) Pestana 'Steel' de la cinta: no accesible (Exception: Unexpected tab.
Parameter name: tabName)
4) Pestana 'Acero' de la cinta: no accesible (Exception: Unexpected tab.
Parameter name: tabName)
   StructuralConnectionHandlerType cargados en el documento: 7
     [944957] Generic Connection
     [944979] Shear plate
     [944980] Embed beam seat
     [944981] Embed plate clip angle
     [944982] Seated beam connection
     [944983] Shear splice plate
     [944984] Stiffened seated beam connection
   StructuralConnectionHandler (conexiones existentes): 0
   Elementos de la categoria OST_StructConnections: 6
=== fin 04-ensamblados ===

```
