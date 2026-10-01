using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Camino A: Elementos nativos de fabricación de acero de Revit / Advance Steel (categoría Plates, Bolts).
    /// Accede a los ensamblados <c>ASObjectsMgd</c>, <c>ASGeometryMgd</c> y <c>Autodesk.SteelConnectionsDB</c>
    /// de manera desacoplada para permitir la compilación en cualquier entorno. Si los ensamblados de Advance Steel
    /// no están disponibles en la máquina o fallan, conmuta transparentemente a <see cref="DirectShapeBackend"/>.
    /// </summary>
    public sealed class AdvanceSteelBackend : IFabricationBackend
    {
        private readonly List<ApiError> _warnings;
        private readonly DirectShapeBackend _fallback;

        private bool? _isAvailable;
        private Type? _tPlate;
        private Type? _tBoltPattern;
        private Type? _tPoint3d;
        private Type? _tVector3d;
        private Type? _tPlane;
        private Type? _tFabTx;

        public AdvanceSteelBackend(List<ApiError> warnings)
        {
            _warnings = warnings ?? new List<ApiError>();
            _fallback = new DirectShapeBackend(_warnings);
        }

        public string Name => "advancesteel";

        public bool IsAvailable
        {
            get
            {
                if (!_isAvailable.HasValue)
                {
                    _isAvailable = TryInitializeTypes();
                }
                return _isAvailable.Value;
            }
        }

        public ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, string name)
        {
            if (!IsAvailable)
            {
                return _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, name);
            }

            try
            {
                var idsBefore = new HashSet<long>(GetDocumentElementIds(document));

                Transform transform = RevitGeometry.ToTransform(frame);

                // Origen y normal de la placa
                object ptOrigin = CreatePoint3d(transform.Origin.X, transform.Origin.Y, transform.Origin.Z);
                object vNormal = CreateVector3d(transform.BasisZ.X, transform.BasisZ.Y, transform.BasisZ.Z);
                object plane = Activator.CreateInstance(_tPlane!, ptOrigin, vNormal)!;

                // Vértices en coordenadas mundiales (en pies)
                Array arrVertices = Array.CreateInstance(_tPoint3d!, outlineMm.Count);
                for (int i = 0; i < outlineMm.Count; i++)
                {
                    var v = outlineMm[i];
                    XYZ ptWorld = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(v.X), UnitConverter.MmToFeet(v.Y), 0.0));
                    arrVertices.SetValue(CreatePoint3d(ptWorld.X, ptWorld.Y, ptWorld.Z), i);
                }

                double thicknessFeet = UnitConverter.MmToFeet(thicknessMm);
                object plate = Activator.CreateInstance(_tPlate!, plane, arrVertices, thicknessFeet)!;

                MethodInfo? writeMethod = _tPlate!.GetMethod("WriteToDb");
                if (writeMethod == null) throw new InvalidOperationException("No se encontró WriteToDb en Plate.");

                ExecuteInFabricationTransaction(document, "MotorConexiones: Crear Placa " + name, () =>
                {
                    writeMethod.Invoke(plate, null);
                });

                var idsAfter = new HashSet<long>(GetDocumentElementIds(document));
                var newIds = idsAfter.Except(idsBefore).ToList();

                if (newIds.Count > 0)
                {
                    return new ElementId(newIds.First());
                }

                _warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Advance Steel no reportó nuevos ElementIds para la placa; usando DirectShape."));
                return _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, name);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_plate_failed", error = ex.ToString() });
                _warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Fallo al crear placa en Advance Steel: " + ex.Message + ". Se recurre a DirectShape."));
                return _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, name);
            }
        }

        public IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name)
        {
            if (!IsAvailable || positionsMm.Count < 2)
            {
                return _fallback.CreateBoltGroup(document, frame, positionsMm, diameterMm, lengthMm, name);
            }

            try
            {
                var idsBefore = new HashSet<long>(GetDocumentElementIds(document));

                Transform transform = RevitGeometry.ToTransform(frame);

                // Si es un patrón regular de pernos (rectángulo o línea)
                // Calcular caja envolvente de las posiciones
                double minX = positionsMm.Min(p => p.X);
                double maxX = positionsMm.Max(p => p.X);
                double minY = positionsMm.Min(p => p.Y);
                double maxY = positionsMm.Max(p => p.Y);

                XYZ p1World = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(minX), UnitConverter.MmToFeet(minY), 0.0));
                XYZ p2World = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(maxX), UnitConverter.MmToFeet(maxY), 0.0));

                object ptRef1 = CreatePoint3d(p1World.X, p1World.Y, p1World.Z);
                object ptRef2 = CreatePoint3d(p2World.X, p2World.Y, p2World.Z);
                object vX = CreateVector3d(transform.BasisX.X, transform.BasisX.Y, transform.BasisX.Z);
                object vY = CreateVector3d(transform.BasisY.X, transform.BasisY.Y, transform.BasisY.Z);

                object pattern = Activator.CreateInstance(_tBoltPattern!, ptRef1, ptRef2, vX, vY)!;

                // Configurar propiedades si existen
                SetProperty(pattern, "ScrewDiameter", UnitConverter.MmToFeet(diameterMm));
                SetProperty(pattern, "ScrewLength", UnitConverter.MmToFeet(lengthMm));

                MethodInfo? writeMethod = _tBoltPattern!.GetMethod("WriteToDb");
                if (writeMethod == null) throw new InvalidOperationException("No se encontró WriteToDb en FinitRectScrewBoltPattern.");

                ExecuteInFabricationTransaction(document, "MotorConexiones: Crear Pernos " + name, () =>
                {
                    writeMethod.Invoke(pattern, null);
                });

                var idsAfter = new HashSet<long>(GetDocumentElementIds(document));
                var newIds = idsAfter.Except(idsBefore).Select(id => new ElementId(id)).ToList();

                if (newIds.Count > 0)
                {
                    return newIds;
                }

                _warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Advance Steel no reportó nuevos ElementIds para pernos; usando DirectShape."));
                return _fallback.CreateBoltGroup(document, frame, positionsMm, diameterMm, lengthMm, name);
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_bolts_failed", error = ex.ToString() });
                _warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Fallo al crear pernos en Advance Steel: " + ex.Message + ". Se recurre a DirectShape."));
                return _fallback.CreateBoltGroup(document, frame, positionsMm, diameterMm, lengthMm, name);
            }
        }

        public IList<ElementId> CreateWelds(Document document, NodeFrame frame, IReadOnlyList<WeldLine2D> weldsMm, string name)
        {
            // Las soldaduras se representan vía DirectShape para visualización inmediata en el modelo
            return _fallback.CreateWelds(document, frame, weldsMm, name);
        }

        private bool TryInitializeTypes()
        {
            try
            {
                Assembly? asObjects = FindAssembly("ASObjectsMgd") ?? LoadAssemblyFromRevitAddins("ASObjectsMgd.dll");
                if (asObjects == null) return false;

                _tPlate = asObjects.GetType("Autodesk.AdvanceSteel.Modelling.Plate");
                _tBoltPattern = asObjects.GetType("Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern");
                if (_tPlate == null || _tBoltPattern == null) return false;

                // Geometría desde los parámetros del constructor de Plate para evitar discrepancias de LoadContext
                var ctorPlate = _tPlate.GetConstructors().FirstOrDefault(c => c.GetParameters().Length == 3);
                if (ctorPlate == null) return false;

                var pars = ctorPlate.GetParameters();
                _tPlane = pars[0].ParameterType;
                _tPoint3d = pars[1].ParameterType.GetElementType();
                if (_tPlane == null || _tPoint3d == null) return false;

                Assembly geomAssembly = _tPlane.Assembly;
                _tVector3d = geomAssembly.GetType("Autodesk.AdvanceSteel.Geometry.Vector3d");
                if (_tVector3d == null) return false;

                Assembly? steelDb = FindAssembly("Autodesk.SteelConnectionsDB") ?? LoadAssemblyFromRevitAddins("Autodesk.SteelConnectionsDB.dll");
                if (steelDb != null)
                {
                    _tFabTx = steelDb.GetType("Autodesk.SteelConnectionsDB.FabricationTransaction");
                }

                return _tFabTx != null;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_init_failed", error = ex.ToString() });
                return false;
            }
        }

        private void ExecuteInFabricationTransaction(Document document, string txName, Action action)
        {
            if (_tFabTx == null)
            {
                action();
                return;
            }

            // Constructor: FabricationTransaction(Document doc, Boolean isReadOnly, String strName)
            object tx = Activator.CreateInstance(_tFabTx, document, false, txName)!;
            try
            {
                action();
                MethodInfo? commitMethod = _tFabTx.GetMethod("Commit");
                commitMethod?.Invoke(tx, null);
            }
            catch
            {
                MethodInfo? cancelMethod = _tFabTx.GetMethod("CancelTransaction");
                cancelMethod?.Invoke(tx, null);
                throw;
            }
            finally
            {
                if (tx is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private object CreatePoint3d(double x, double y, double z)
        {
            return Activator.CreateInstance(_tPoint3d!, x, y, z)!;
        }

        private object CreateVector3d(double x, double y, double z)
        {
            return Activator.CreateInstance(_tVector3d!, x, y, z)!;
        }

        private static void SetProperty(object target, string propName, object value)
        {
            PropertyInfo? prop = target.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                try
                {
                    prop.SetValue(target, Convert.ChangeType(value, prop.PropertyType), null);
                }
                catch { }
            }
        }

        private static Assembly? FindAssembly(string simpleName)
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (string.Equals(a.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase))
                    {
                        return a;
                    }
                }
                catch { }
            }
            return null;
        }

        private static Assembly? LoadAssemblyFromRevitAddins(string fileName)
        {
            try
            {
                string path = Path.Combine(@"C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections", fileName);
                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
            }
            catch { }
            return null;
        }

        private static IEnumerable<long> GetDocumentElementIds(Document document)
        {
            return new FilteredElementCollector(document)
                .WhereElementIsNotElementType()
                .ToElementIds()
                .Select(id => id.Value);
        }
    }
}
