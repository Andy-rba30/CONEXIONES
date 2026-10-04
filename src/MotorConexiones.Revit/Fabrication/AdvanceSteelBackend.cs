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
    /// Camino A: elementos nativos de fabricación de acero de Revit / Advance Steel (categorías Plates y Bolts).
    /// Usa por reflexión los ensamblados que Revit ya tiene cargados (<c>ASObjectsMgd</c>, <c>ASGeometryMgd</c>,
    /// <c>Autodesk.SteelConnectionsDB</c>); ninguno está en NuGet, así que esta clase compila sin ellos.
    /// Todo lo que se sabe de esa API viene de los sondeos 06, 09 y 10 de la Fase 1 (docs/fases/resultados-fase-1.md):
    /// - <c>FabricationTransaction(Document, Boolean isReadOnly, String)</c> y la sobrecarga con
    ///   <c>Boolean bRevitTransactionAlreadyStarted</c>; <c>Commit()</c>, <c>CancelTransaction()</c>, <c>Dispose()</c>.
    /// - <c>Plate(Plane, Point3d[], Double)</c>, <c>Plane(Point3d, Vector3d)</c>, <c>WriteToDb()</c>.
    /// - <c>FinitRectScrewBoltPattern(Point3d, Point3d, Vector3d, Vector3d)</c> con <c>Nx, Ny, Dx, Dy, ScrewDiameter, ScrewLength</c>.
    /// - UNIDADES: Advance Steel trabaja en MILÍMETROS, no en los pies internos de Revit. La Fase 1 lo dio por "pies" a
    ///   partir de una captura; la Fase 5 (B-2, docs/fases/resultados-fase-5.md) lo desmintió con la paleta de
    ///   Propiedades: con 60 mm de paso entre pernos pasados como 0,19685 pies, Revit mostró 1/128" (0,198 mm), y la
    ///   cartela de 565 × 530 × 9,5 mm salió de 2 × 1 × 0 mm. Por eso aquí las coordenadas de Revit (pies) se pasan a mm
    ///   con <see cref="UnitConverter.FeetToMm"/> justo antes de entregarlas a Advance Steel, y las medidas del contrato
    ///   (ya en mm) se entregan tal cual.
    /// - Los tipos de geometría se toman de los parámetros de los constructores (mismo contexto de carga que ASObjectsMgd).
    /// - Los objetos de Advance Steel solo se construyen y escriben DENTRO de la FabricationTransaction (fuera, Revit se cierra).
    /// - PERNOS (Fase 6b): la placa cuchilla va sobre la cara +Z de la cartela (plano desplazado <see cref="BoltStack.KnifePlateZOffsetMm"/>)
    ///   y el patrón de pernos se apoya en la cara exterior de la placa cuchilla (<see cref="BoltStack.OuterFaceZMm"/>) con
    ///   <c>ScrewLength</c> = agarre + suplemento AISC 7-15. Después de <c>WriteToDb</c> se intenta
    ///   <c>Connect(FilerObject[] {cartela, cuchilla}, eAssemblyLocation)</c> (firma del sondeo 06) para que Advance Steel
    ///   haga los agujeros y calcule el agarre; si la llamada no existe o falla, los pernos quedan igualmente en su sitio y se
    ///   anota en el registro. La propiedad <c>Portioning</c> de la placa (0,5 = espesor centrado en el plano) se fija si existe;
    ///   si no existe, el registro lo dice y el sondeo 16 mide dónde quedó la placa. NO PROBADO en Revit hasta la ronda 6b.
    /// Si algo falla, se recurre a <see cref="DirectShapeBackend"/> para ese elemento y se anota una advertencia.
    /// </summary>
    public sealed class AdvanceSteelBackend : IFabricationBackend
    {
        private const string SteelConnectionsFolder = @"C:\Program Files\Autodesk\Revit 2027\AddIns\SteelConnections";

        private readonly List<ApiError> _warnings;
        private readonly DirectShapeBackend _fallback;

        private bool? _isAvailable;
        private Type? _tPlate;
        private Type? _tBoltPattern;
        private Type? _tPoint3d;
        private Type? _tVector3d;
        private Type? _tPlane;
        private Type? _tFabTx;
        private ConstructorInfo? _ctorPlate;
        private ConstructorInfo? _ctorPattern;
        private ConstructorInfo? _ctorPlane;
        private ConstructorInfo? _ctorPoint;
        private ConstructorInfo? _ctorVector;
        private SteelSession? _activeSession;

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

        public IFabricationSession BeginSession(Document document, string name)
        {
            if (!IsAvailable)
            {
                return _fallback.BeginSession(document, name);
            }
            if (_activeSession != null && !_activeSession.Ended)
            {
                throw new InvalidOperationException("Ya hay una sesión de fabricación de acero abierta.");
            }
            _activeSession = new SteelSession(this, document, name);
            return _activeSession;
        }

        public ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, double zOffsetMm, string name)
        {
            if (!IsAvailable || !HasOpenSession(document, "placa " + name))
            {
                return _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, zOffsetMm, name);
            }

            try
            {
                Transform transform = RevitGeometry.ToTransform(frame);
                double zOffsetFeet = UnitConverter.MmToFeet(zOffsetMm);

                // Plano medio de la placa: origen del nudo desplazado zOffset por la normal Z (en mm para Advance Steel).
                XYZ planeOrigin = transform.OfPoint(new XYZ(0.0, 0.0, zOffsetFeet));
                object plane = _ctorPlane!.Invoke(new[]
                {
                    CreateSteelPoint(planeOrigin),
                    CreateVector3d(transform.BasisZ.X, transform.BasisZ.Y, transform.BasisZ.Z),
                });

                Array vertices = Array.CreateInstance(_tPoint3d!, outlineMm.Count);
                for (int i = 0; i < outlineMm.Count; i++)
                {
                    XYZ world = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(outlineMm[i].X), UnitConverter.MmToFeet(outlineMm[i].Y), zOffsetFeet));
                    vertices.SetValue(CreateSteelPoint(world), i);
                }

                // El espesor del contrato ya está en mm: se entrega tal cual. Portioning 0,5 = espesor centrado en el plano
                // (si la propiedad no existe en esta versión, SetProperty lo anota y Advance Steel aplica su valor por defecto).
                object plate = _ctorPlate!.Invoke(new object[] { plane, vertices, thicknessMm });
                string portioning = SetProperty(plate, "Portioning", 0.5);
                InvokeWriteToDb(_tPlate!, plate);
                _activeSession!.SteelObjects[name] = plate;
                // El SteelProxyElement aparece al confirmar la sesión (Complete); hasta entonces no hay ElementId.
                _activeSession!.Pending.Add(new PendingItem(name, "plate", () => new[] { _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, zOffsetMm, name) }));
                JsonLineLogger.Write(new { @event = "advance_steel_plate_written", name, vertices = outlineMm.Count, thickness_mm = thicknessMm, z_offset_mm = zOffsetMm, portioning, units = "mm" });
                return ElementId.InvalidElementId;
            }
            catch (Exception error)
            {
                return FallbackPlate(document, frame, outlineMm, thicknessMm, zOffsetMm, name, error);
            }
        }

        public IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name)
        {
            // Posiciones sueltas: Advance Steel trabaja con patrones; se deja al camino B (lo usa solo la prueba técnica).
            return _fallback.CreateBoltGroup(document, frame, positionsMm, diameterMm, lengthMm, name);
        }

        public IList<ElementId> CreateBoltPattern(Document document, NodeFrame frame, BoltGrid grid, BoltStack stack, IReadOnlyList<string> connectToPlateNames, string name)
        {
            if (stack == null) throw new ArgumentNullException(nameof(stack));
            if (!IsAvailable || grid.Count == 0 || !HasOpenSession(document, "pernos " + name))
            {
                return _fallback.CreateBoltPattern(document, frame, grid, stack, connectToPlateNames, name);
            }

            try
            {
                Transform transform = RevitGeometry.ToTransform(frame);

                // Plano del patrón: cara exterior de la placa cuchilla (cabeza del perno); el vástago va hacia −Z.
                double faceZ = UnitConverter.MmToFeet(stack.OuterFaceZMm);
                XYZ first = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(grid.FirstCorner.X), UnitConverter.MmToFeet(grid.FirstCorner.Y), faceZ));
                XYZ opposite = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(grid.OppositeCorner.X), UnitConverter.MmToFeet(grid.OppositeCorner.Y), faceZ));
                XYZ along = transform.OfVector(new XYZ(grid.Ux, grid.Uy, 0.0));
                // Ejes del patrón: X a lo largo y Y "hacia −v" para que la normal (X × Y) apunte hacia −Z, hacia dentro del paquete.
                XYZ across = transform.OfVector(new XYZ(-grid.Vx, -grid.Vy, 0.0));

                object pattern = _ctorPattern!.Invoke(new[]
                {
                    CreateSteelPoint(first),
                    CreateSteelPoint(opposite),
                    CreateVector3d(along.X, along.Y, along.Z),
                    CreateVector3d(across.X, across.Y, across.Z),
                });

                // Mismas propiedades que en el sondeo 10 (NumberOfScrews = 4 antes de escribir), en mm (ver cabecera).
                var set = new List<string>
                {
                    SetProperty(pattern, "Nx", grid.CountAlong),
                    SetProperty(pattern, "Ny", grid.CountAcross),
                    SetProperty(pattern, "Dx", grid.SpacingMm),
                    SetProperty(pattern, "Dy", grid.SpacingMm),
                    SetProperty(pattern, "ScrewDiameter", stack.DiameterMm),
                    SetProperty(pattern, "ScrewLength", stack.BoltLengthMm),
                    SetProperty(pattern, "BindingLength", stack.GripMm),
                };
                InvokeWriteToDb(_tBoltPattern!, pattern);
                string connect = TryConnect(pattern, connectToPlateNames);
                _activeSession!.Pending.Add(new PendingItem(name, "bolts", () => _fallback.CreateBoltPattern(document, frame, grid, stack, connectToPlateNames, name)));
                JsonLineLogger.Write(new
                {
                    @event = "advance_steel_bolts_written",
                    name,
                    properties = set,
                    count = grid.Count,
                    grip_mm = stack.GripMm,
                    bolt_length_mm = stack.BoltLengthMm,
                    head_face_z_mm = stack.OuterFaceZMm,
                    gusset_face = BoltStack.GussetFace,
                    connect,
                    units = "mm",
                });
                return new List<ElementId>();
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_bolts_failed", name, error = error.ToString() });
                _warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                    "Fallo al crear los pernos '" + name + "' con Advance Steel: " + Describe(error) + ". Se crean con DirectShape."));
                return _fallback.CreateBoltPattern(document, frame, grid, stack, connectToPlateNames, name);
            }
        }

        /// <summary>
        /// <c>BoltPattern.Connect(FilerObject[] elems, eAssemblyLocation location)</c> (sondeo 06) con las placas creadas en esta
        /// sesión. Devuelve un texto para el registro; nunca lanza: si no se puede, los pernos quedan donde el patrón los puso.
        /// </summary>
        private string TryConnect(object pattern, IReadOnlyList<string> plateNames)
        {
            try
            {
                var targets = new List<object>();
                var missing = new List<string>();
                foreach (string plateName in plateNames ?? new List<string>())
                {
                    if (_activeSession != null && _activeSession.SteelObjects.TryGetValue(plateName, out object? plate)) targets.Add(plate);
                    else missing.Add(plateName);
                }
                if (targets.Count == 0) return "sin placas de Advance Steel a las que unir" + (missing.Count > 0 ? " (" + string.Join(", ", missing) + ")" : "");

                MethodInfo? connect = pattern.GetType().GetMethod("Connect", BindingFlags.Public | BindingFlags.Instance);
                if (connect == null) return "Connect: no existe";
                ParameterInfo[] parameters = connect.GetParameters();
                if (parameters.Length != 2 || !parameters[0].ParameterType.IsArray || !parameters[1].ParameterType.IsEnum)
                {
                    return "Connect: firma inesperada (" + string.Join(", ", parameters.Select(p => p.ParameterType.Name)) + ")";
                }

                Type elementType = parameters[0].ParameterType.GetElementType()!;
                Array array = Array.CreateInstance(elementType, targets.Count);
                for (int i = 0; i < targets.Count; i++)
                {
                    if (!elementType.IsInstanceOfType(targets[i])) return "Connect: la placa no es " + elementType.Name;
                    array.SetValue(targets[i], i);
                }

                // eAssemblyLocation: se prefiere el valor "en obra" (kOnSite); si no se reconoce, el primero del enum.
                Type locationType = parameters[1].ParameterType;
                string[] names = Enum.GetNames(locationType);
                string chosen = names.FirstOrDefault(n => n.IndexOf("Site", StringComparison.OrdinalIgnoreCase) >= 0) ?? names.FirstOrDefault() ?? "";
                if (chosen.Length == 0) return "Connect: eAssemblyLocation sin valores";
                object location = Enum.Parse(locationType, chosen);

                connect.Invoke(pattern, new[] { array, location });
                return "Connect OK con " + targets.Count + " placas, " + chosen + (missing.Count > 0 ? "; sin objeto: " + string.Join(", ", missing) : "");
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_connect_failed", error = error.ToString() });
                return "Connect: ERROR " + Describe(error);
            }
        }

        public IList<ElementId> CreateWelds(Document document, NodeFrame frame, IReadOnlyList<WeldLine2D> weldsMm, string name)
        {
            // v1: las soldaduras se representan con DirectShape (WeldPattern/WeldLine de Advance Steel quedan para v2).
            return _fallback.CreateWelds(document, frame, weldsMm, name);
        }

        public void DeleteElements(Document document, ICollection<ElementId> elementIds)
        {
            // Los SteelProxyElement se borran dentro de la sesión de fabricación (si está abierta); los DirectShape, directamente.
            if (elementIds.Count > 0) document.Delete(elementIds);
        }

        // ------------------------------------------------------------------ sesión

        private bool HasOpenSession(Document document, string what)
        {
            if (_activeSession != null && !_activeSession.Ended && _activeSession.Opened) return true;
            _warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                "No hay una FabricationTransaction abierta para '" + what + "'; se crea con DirectShape."));
            return false;
        }

        /// <summary>Elemento escrito con WriteToDb a la espera del Commit, con su reserva en DirectShape.</summary>
        private sealed class PendingItem
        {
            public PendingItem(string name, string kind, Func<IEnumerable<ElementId>> fallback)
            {
                Name = name;
                Kind = kind;
                Fallback = fallback;
            }

            public string Name { get; }
            public string Kind { get; }
            public Func<IEnumerable<ElementId>> Fallback { get; }
        }

        private sealed class SteelSession : IFabricationSession
        {
            private readonly AdvanceSteelBackend _owner;
            private readonly Document _document;
            private readonly string _name;
            private readonly object? _transaction;
            private readonly HashSet<long> _idsBefore;
            private bool _completed;

            public List<PendingItem> Pending { get; } = new List<PendingItem>();

            /// <summary>Objetos de Advance Steel escritos en esta sesión, por nombre (para <c>Connect</c> de los pernos).</summary>
            public Dictionary<string, object> SteelObjects { get; } = new Dictionary<string, object>(StringComparer.Ordinal);

            public SteelSession(AdvanceSteelBackend owner, Document document, string name)
            {
                _owner = owner;
                _document = document;
                _name = name;
                _idsBefore = new HashSet<long>(GetDocumentElementIds(document));
                Type tx = owner._tFabTx!;
                try
                {
                    // Dentro de una operación la Transaction de Revit ya está abierta (doc.IsModifiable): se usa la sobrecarga
                    // (Document, Boolean isReadOnly, String, Boolean bRevitTransactionAlreadyStarted) del sondeo 06.
                    bool revitTransactionStarted = document.IsModifiable;
                    ConstructorInfo? ctor4 = tx.GetConstructor(new[] { typeof(Document), typeof(bool), typeof(string), typeof(bool) });
                    ConstructorInfo? ctor3 = tx.GetConstructor(new[] { typeof(Document), typeof(bool), typeof(string) });
                    if (revitTransactionStarted && ctor4 != null)
                    {
                        _transaction = ctor4.Invoke(new object[] { document, false, name, true });
                    }
                    else if (ctor3 != null)
                    {
                        _transaction = ctor3.Invoke(new object[] { document, false, name });
                    }
                    else
                    {
                        throw new InvalidOperationException("FabricationTransaction sin constructor conocido.");
                    }
                    Opened = true;
                    JsonLineLogger.Write(new { @event = "fabrication_transaction_open", name, revit_transaction_started = revitTransactionStarted, is_modifiable_after = document.IsModifiable });
                }
                catch (Exception error)
                {
                    Opened = false;
                    JsonLineLogger.Write(new { @event = "fabrication_transaction_failed", name, error = error.ToString() });
                    owner._warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                        "No se pudo abrir la FabricationTransaction de Advance Steel: " + Describe(error) + ". Toda la conexión se crea con DirectShape."));
                }
            }

            public bool Opened { get; }
            public bool Ended { get; private set; }

            public IReadOnlyList<ElementId> Complete()
            {
                var created = new List<ElementId>();
                if (!Opened || Ended) return created;

                _owner._tFabTx!.GetMethod("Commit")?.Invoke(_transaction, null);
                _completed = true;

                List<long> newIds = GetDocumentElementIds(_document).Where(id => !_idsBefore.Contains(id)).ToList();
                var categories = new List<string>();
                foreach (long id in newIds)
                {
                    Element? element = _document.GetElement(new ElementId(id));
                    categories.Add(id + ":" + (element?.Category?.Name ?? "?") + ":" + (element?.GetType().Name ?? "?"));
                }
                JsonLineLogger.Write(new { @event = "fabrication_transaction_commit", name = _name, pending = Pending.Count, new_elements = categories, is_modifiable_after = _document.IsModifiable });

                if (Pending.Count > 0 && newIds.Count == 0)
                {
                    _owner._warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                        "Advance Steel no materializó ningún elemento al confirmar la sesión (" + Pending.Count + " escrituras); se crean con DirectShape.",
                        hint: "Mira las líneas fabrication_transaction_* del registro del add-in."));
                    foreach (PendingItem item in Pending)
                    {
                        created.AddRange(item.Fallback());
                    }
                    return created;
                }

                created.AddRange(newIds.Select(id => new ElementId(id)));
                return created;
            }

            public void Dispose()
            {
                if (Ended) return;
                Ended = true;
                if (!Opened) return;
                try
                {
                    if (!_completed)
                    {
                        _owner._tFabTx!.GetMethod("CancelTransaction")?.Invoke(_transaction, null);
                    }
                }
                catch (Exception error)
                {
                    JsonLineLogger.Write(new { @event = "fabrication_transaction_cancel_failed", error = error.ToString() });
                }
                finally
                {
                    (_transaction as IDisposable)?.Dispose();
                }
            }
        }

        // ------------------------------------------------------------------ reflexión

        private bool TryInitializeTypes()
        {
            try
            {
                Assembly? asObjects = FindAssembly("ASObjectsMgd") ?? LoadFromSteelConnections("ASObjectsMgd.dll");
                if (asObjects == null) return false;

                _tPlate = asObjects.GetType("Autodesk.AdvanceSteel.Modelling.Plate");
                _tBoltPattern = asObjects.GetType("Autodesk.AdvanceSteel.Modelling.FinitRectScrewBoltPattern");
                if (_tPlate == null || _tBoltPattern == null) return false;

                _ctorPlate = _tPlate.GetConstructors().FirstOrDefault(c => ParameterNames(c).SequenceEqual(new[] { "Plane", "Point3d[]", "Double" }));
                _ctorPattern = _tBoltPattern.GetConstructors().FirstOrDefault(c => ParameterNames(c).SequenceEqual(new[] { "Point3d", "Point3d", "Vector3d", "Vector3d" }));
                if (_ctorPlate == null || _ctorPattern == null) return false;

                // Geometría desde los parámetros del constructor: mismo AssemblyLoadContext que ASObjectsMgd (lección de la Fase 1).
                ParameterInfo[] plateParameters = _ctorPlate.GetParameters();
                _tPlane = plateParameters[0].ParameterType;
                _tPoint3d = plateParameters[1].ParameterType.GetElementType();
                _tVector3d = _ctorPattern.GetParameters()[2].ParameterType;
                if (_tPlane == null || _tPoint3d == null || _tVector3d == null) return false;

                _ctorPlane = _tPlane.GetConstructor(new[] { _tPoint3d, _tVector3d });
                _ctorPoint = _tPoint3d.GetConstructor(new[] { typeof(double), typeof(double), typeof(double) });
                _ctorVector = _tVector3d.GetConstructor(new[] { typeof(double), typeof(double), typeof(double) });
                if (_ctorPlane == null || _ctorPoint == null || _ctorVector == null) return false;

                Assembly? steelDb = FindAssembly("Autodesk.SteelConnectionsDB") ?? LoadFromSteelConnections("Autodesk.SteelConnectionsDB.dll");
                _tFabTx = steelDb?.GetType("Autodesk.SteelConnectionsDB.FabricationTransaction");
                return _tFabTx != null;
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "advance_steel_init_failed", error = error.ToString() });
                return false;
            }
        }

        private static IEnumerable<string> ParameterNames(ConstructorInfo constructor) =>
            constructor.GetParameters().Select(p => p.ParameterType.Name);

        private object CreatePoint3d(double x, double y, double z) => _ctorPoint!.Invoke(new object[] { x, y, z });

        /// <summary>Punto de Revit (pies, unidades internas) convertido al <c>Point3d</c> de Advance Steel, que trabaja en mm.</summary>
        private object CreateSteelPoint(XYZ revitPoint) =>
            CreatePoint3d(UnitConverter.FeetToMm(revitPoint.X), UnitConverter.FeetToMm(revitPoint.Y), UnitConverter.FeetToMm(revitPoint.Z));

        private object CreateVector3d(double x, double y, double z) => _ctorVector!.Invoke(new object[] { x, y, z });

        private static void InvokeWriteToDb(Type type, object target)
        {
            MethodInfo? write = type.GetMethod("WriteToDb");
            if (write == null) throw new InvalidOperationException("El tipo " + type.Name + " no tiene WriteToDb.");
            write.Invoke(target, null);
        }

        private static string SetProperty(object target, string propertyName, object value)
        {
            PropertyInfo? property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null || !property.CanWrite) return propertyName + ": no existe";
            try
            {
                property.SetValue(target, Convert.ChangeType(value, property.PropertyType, System.Globalization.CultureInfo.InvariantCulture), null);
                return propertyName + "=" + Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception error)
            {
                return propertyName + ": ERROR " + Describe(error);
            }
        }

        private ElementId FallbackPlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, double zOffsetMm, string name, Exception error)
        {
            JsonLineLogger.Write(new { @event = "advance_steel_plate_failed", name, error = error.ToString() });
            _warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                "Fallo al crear la placa '" + name + "' con Advance Steel: " + Describe(error) + ". Se crea con DirectShape."));
            return _fallback.CreatePlate(document, frame, outlineMm, thicknessMm, zOffsetMm, name);
        }

        private static string Describe(Exception error)
        {
            Exception inner = error;
            while (inner.InnerException != null) inner = inner.InnerException;
            return inner.GetType().Name + ": " + inner.Message;
        }

        private static Assembly? FindAssembly(string simpleName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    if (string.Equals(assembly.GetName().Name, simpleName, StringComparison.OrdinalIgnoreCase)) return assembly;
                }
                catch
                {
                    // Ensamblados dinámicos sin nombre: se ignoran.
                }
            }
            return null;
        }

        private static Assembly? LoadFromSteelConnections(string fileName)
        {
            try
            {
                string path = Path.Combine(SteelConnectionsFolder, fileName);
                // LoadFrom (no LoadFile): comparte el contexto Default, que es donde Revit carga el módulo de acero.
                return File.Exists(path) ? Assembly.LoadFrom(path) : null;
            }
            catch
            {
                return null;
            }
        }

        private static IEnumerable<long> GetDocumentElementIds(Document document)
        {
            return new FilteredElementCollector(document).WhereElementIsNotElementType().ToElementIds().Select(id => id.Value);
        }
    }
}
