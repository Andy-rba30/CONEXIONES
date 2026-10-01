using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
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
        /// <summary>Longitud de perno por defecto cuando el contrato no la trae (v1 no la pide).</summary>
        private const double DefaultBoltLengthMm = 45.0;

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
                        double thickness = spec.Gusset.ThicknessMm.GetValueOrDefault(9.525);
                        ElementId gussetId = backend.CreatePlate(document, frame, outline, thickness, "Cartela " + (spec.Source?.Drawing ?? "nudo"));
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
                                var corners = ConnectionGeometry.ComputeKnifePlateCorners(ux, uy, setback, memberSpec.Attachment.Plate);
                                double thickness = memberSpec.Attachment.Plate.ThicknessMm.GetValueOrDefault(10.0);
                                ElementId plateId = backend.CreatePlate(document, frame, corners, thickness, "Placa cuchilla miembro " + memberSpec.ElementId);
                                if (plateId != ElementId.InvalidElementId) createdIds.Add(plateId);

                                if (memberSpec.Attachment.Bolts != null)
                                {
                                    BoltGrid grid = ConnectionGeometry.ComputeBoltGrid(ux, uy, setback, memberSpec.Attachment.Plate, memberSpec.Attachment.Bolts);
                                    if (grid.Count > 0)
                                    {
                                        double diameter = memberSpec.Attachment.Bolts.DiameterMm.GetValueOrDefault(15.875);
                                        createdIds.AddRange(backend.CreateBoltPattern(document, frame, grid, diameter, DefaultBoltLengthMm, "Pernos miembro " + memberSpec.ElementId));
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

                session.Complete();
            }

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
