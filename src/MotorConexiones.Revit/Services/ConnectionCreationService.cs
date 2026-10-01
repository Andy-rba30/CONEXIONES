using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Services
{
    /// <summary>
    /// Orquesta la creación, el borrado y la actualización de una conexión: nudo, retiros de extremo, geometría
    /// (una sola sesión de fabricación por conexión) y registro en Extensible Storage. Se llama siempre dentro de la
    /// Transaction de Revit abierta por la operación (<c>OperationScope</c>): ante cualquier excepción, la operación
    /// deshace el grupo completo.
    /// </summary>
    public static class ConnectionCreationService
    {
        public static ConnectionRecord CreateConnection(
            Document document,
            UIDocument? uidoc,
            ConnectionSpec spec,
            string rawSpecJson,
            string? connectionId,
            List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (spec == null) throw new ArgumentNullException(nameof(spec));

            // 1. Nudo: misma regla que validate y preview.
            ResolvedNode node = NodeInspector.ResolveNode(document, spec);
            NodeFrame frame = node.Frame;
            Vec3 workPointMm = frame.Origin;

            IFabricationBackend backend = BackendFactory.GetBackend(document, warnings);
            string id = !string.IsNullOrWhiteSpace(connectionId) ? connectionId! : Guid.NewGuid().ToString("D");
            var createdIds = new List<ElementId>();
            var modifiedMembers = new List<ModifiedMemberRecord>();
            LimitsConfig limits = LimitsConfigLoader.Load();
            double gussetThickness = spec.Gusset?.ThicknessMm ?? 9.525;

            using (IFabricationSession session = backend.BeginSession(document, "MotorConexiones: crear " + id))
            {
                // 2. Retiros de extremo.
                if (spec.Members != null)
                {
                    foreach (var memberSpec in spec.Members)
                    {
                        MemberInfo? info = node.Find(memberSpec.ElementId);
                        double setback = memberSpec.EndSetbackMm.GetValueOrDefault(0.0);
                        if (info != null && setback > 0.0)
                        {
                            ModifiedMemberRecord? modified = MemberModifier.ApplySetback(document, info.Instance, workPointMm, setback);
                            if (modified != null) modifiedMembers.Add(modified);
                        }
                    }
                }

                // 3. Cartela.
                if (spec.Gusset != null)
                {
                    var outline = ConnectionGeometry.GetGussetOutline(spec.Gusset);
                    if (outline.Count >= 3)
                    {
                        // La cartela va centrada en el plano XY del nudo (plano de la cercha).
                        ElementId gussetId = backend.CreatePlate(document, frame, outline, gussetThickness, 0.0, "Cartela " + (spec.Source?.Drawing ?? "nudo"));
                        if (gussetId != ElementId.InvalidElementId) createdIds.Add(gussetId);
                    }
                }

                // 4. Uniones de cada miembro.
                if (spec.Members != null)
                {
                    foreach (var memberSpec in spec.Members)
                    {
                        MemberInfo? info = node.Find(memberSpec.ElementId);
                        if (info == null || memberSpec.Attachment == null) continue;

                        var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, info.Start, info.End, workPointMm);
                        double setback = memberSpec.EndSetbackMm.GetValueOrDefault(0.0);

                        if (string.Equals(memberSpec.Attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
                        {
                            if (memberSpec.Attachment.Plate != null)
                            {
                                // Ronda 6b: la placa cuchilla apoya sobre una cara de la cartela (plate.gusset_face, +z por
                                // defecto), no en su mismo plano; los pernos atraviesan cartela + placa con ese agarre.
                                BoltStack stack = BoltStack.Compute(gussetThickness, memberSpec.Attachment.Plate, memberSpec.Attachment.Bolts, limits);
                                var corners = ConnectionGeometry.ComputeKnifePlateCorners(ux, uy, setback, memberSpec.Attachment.Plate);
                                ElementId plateId = backend.CreatePlate(document, frame, corners, stack.PlateThicknessMm, stack.PlateOffsetMm, "Placa cuchilla miembro " + memberSpec.ElementId);
                                if (plateId != ElementId.InvalidElementId) createdIds.Add(plateId);

                                if (memberSpec.Attachment.Bolts != null)
                                {
                                    BoltGrid grid = ConnectionGeometry.ComputeBoltGrid(ux, uy, setback, memberSpec.Attachment.Plate, memberSpec.Attachment.Bolts);
                                    if (grid.Count > 0)
                                    {
                                        double diameter = memberSpec.Attachment.Bolts.DiameterMm.GetValueOrDefault(15.875);
                                        createdIds.AddRange(backend.CreateBoltPattern(document, frame, grid, diameter, stack, "Pernos miembro " + memberSpec.ElementId));
                                    }
                                }
                            }
                        }

                        var weldLines = ConnectionGeometry.ComputeWeldLines(ux, uy, setback, memberSpec);
                        if (weldLines.Count > 0)
                        {
                            createdIds.AddRange(backend.CreateWelds(document, frame, weldLines, "Soldadura miembro " + memberSpec.ElementId));
                        }
                    }
                }

                createdIds.AddRange(session.Complete());
            }
            createdIds = createdIds.Where(e => e != ElementId.InvalidElementId).Distinct().ToList();

            // 5. Registro en Extensible Storage (fuera de la sesión de acero; dentro de la Transaction de Revit).
            var record = new ConnectionRecord
            {
                ConnectionId = id,
                SpecVersion = spec.SpecVersion ?? "1.0",
                ConnectionType = spec.ConnectionType ?? "gusset_node",
                SpecJson = rawSpecJson,
                CreatedElementIds = createdIds.Select(e => e.Value).ToList(),
                ModifiedMembers = modifiedMembers,
                CreatedUtc = DateTime.UtcNow.ToString("o"),
                BackendName = backend.Name
            };
            ConnectionStorageManager.SaveConnection(document, record);
            return record;
        }

        /// <summary>Ids de todos los elementos del documento (para detectar lo que crea Advance Steel al confirmar).</summary>
        public static HashSet<long> Snapshot(Document document)
        {
            return new HashSet<long>(new FilteredElementCollector(document).WhereElementIsNotElementType().ToElementIds().Select(e => e.Value));
        }

        /// <summary>
        /// Añade al registro los elementos que aparecieron en el documento desde <paramref name="snapshot"/> y que no
        /// están en él (p. ej. SteelProxyElement materializados por Advance Steel al confirmar la Transaction de Revit),
        /// para que conn_delete los borre. Excluye el propio DataStorage del registro. Llamar dentro de una Transaction.
        /// </summary>
        public static int AdoptNewElements(Document document, ConnectionRecord record, HashSet<long> snapshot, List<ApiError> warnings)
        {
            var known = new HashSet<long>(record.CreatedElementIds);
            var adopted = new List<string>();
            foreach (long id in Snapshot(document))
            {
                if (snapshot.Contains(id) || known.Contains(id)) continue;
                Element? element = document.GetElement(new ElementId(id));
                if (element == null || element is DataStorage) continue;
                record.CreatedElementIds.Add(id);
                adopted.Add(id + ":" + (element.Category?.Name ?? "?") + ":" + element.GetType().Name);
            }
            if (adopted.Count > 0)
            {
                ConnectionStorageManager.SaveConnection(document, record);
                warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                    "Se añadieron al registro " + adopted.Count + " elementos creados por Revit/Advance Steel al confirmar: " + string.Join(", ", adopted),
                    hint: "Informativo: conn_delete los borrará con la conexión."));
            }
            return adopted.Count;
        }

        public static bool DeleteConnection(Document document, string connectionId, List<ApiError> warnings, out ConnectionRecord? deletedRecord)
        {
            deletedRecord = null;
            if (document == null || string.IsNullOrWhiteSpace(connectionId)) return false;

            ConnectionRecord? record = ConnectionStorageManager.GetConnection(document, connectionId);
            if (record == null) return false;

            IFabricationBackend backend = BackendFactory.GetBackend(document, warnings);
            using (IFabricationSession session = backend.BeginSession(document, "MotorConexiones: borrar " + connectionId))
            {
                // 1. Elementos creados por el add-in (nunca otros).
                var existing = record.CreatedElementIds
                    .Select(e => new ElementId(e))
                    .Where(e => document.GetElement(e) != null)
                    .ToList();
                if (existing.Count > 0) backend.DeleteElements(document, existing);

                // 2. Restaurar los retiros de extremo.
                foreach (ModifiedMemberRecord modified in record.ModifiedMembers)
                {
                    MemberModifier.RestoreSetback(document, modified);
                }

                session.Complete();
            }

            // 3. Registro.
            ConnectionStorageManager.DeleteConnection(document, connectionId, out deletedRecord);
            return true;
        }

        public static ConnectionRecord UpdateConnection(
            Document document,
            UIDocument? uidoc,
            string connectionId,
            ConnectionSpec spec,
            string rawSpecJson,
            List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (string.IsNullOrWhiteSpace(connectionId)) throw new ArgumentException("connection_id obligatorio.", nameof(connectionId));

            if (!DeleteConnection(document, connectionId, warnings, out _))
            {
                throw new InvalidOperationException("No existe ninguna conexión con id " + connectionId + ".");
            }
            return CreateConnection(document, uidoc, spec, rawSpecJson, connectionId, warnings);
        }
    }
}
