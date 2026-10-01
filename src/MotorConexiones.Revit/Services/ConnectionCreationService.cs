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
    /// Servicio central que orquestra la creación, actualización y borrado de conexiones en el modelo.
    /// Coordina el cálculo geométrico del nudo, el backend de fabricación, el recorte de miembros
    /// y la persistencia en Extensible Storage.
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

            // 1. Resolver miembros del nudo
            var ids = spec.Node?.ElementIds != null && spec.Node.ElementIds.Count > 0
                ? spec.Node.ElementIds
                : (spec.Chord != null ? new List<long> { spec.Chord.ElementId } : new List<long>());

            var allIds = new HashSet<long>(ids);
            if (spec.Chord != null && spec.Chord.ElementId > 0) allIds.Add(spec.Chord.ElementId);
            if (spec.Members != null)
            {
                foreach (var m in spec.Members)
                {
                    if (m.ElementId > 0) allIds.Add(m.ElementId);
                }
            }

            var members = NodeInspector.ReadMembers(document, allIds, "node.element_ids");
            var chord = members.FirstOrDefault(m => m.Id == spec.Chord?.ElementId);
            if (chord == null)
            {
                chord = NodeInspector.ChooseChord(members, default, out _);
            }

            var firstMember = members.FirstOrDefault(m => m.Id != chord.Id);
            if (firstMember == null)
            {
                throw new InvalidOperationException("Se necesita al menos una diagonal o montante además del cordón.");
            }

            // 2. Sistema local del nudo
            NodeFrame frame = NodeInspector.ComputeFrame(chord, firstMember);
            Vec3 workPointMm = frame.Origin;

            // 3. Obtener backend de fabricación
            IFabricationBackend backend = BackendFactory.GetBackend(document, warnings);

            var createdIds = new List<ElementId>();
            var modifiedMembers = new List<ModifiedMemberRecord>();

            // 4. Aplicar retiros de extremo (setback) a los miembros
            if (spec.Members != null)
            {
                foreach (var mSpec in spec.Members)
                {
                    var mInfo = members.FirstOrDefault(m => m.Id == mSpec.ElementId);
                    double setback = mSpec.EndSetbackMm.GetValueOrDefault(0.0);
                    if (mInfo != null && setback > 0.0)
                    {
                        var mod = MemberModifier.ApplySetback(document, mInfo.Instance, workPointMm, setback);
                        if (mod != null)
                        {
                            modifiedMembers.Add(mod);
                        }
                    }
                }
            }

            // 5. Crear la Cartela (Gusset plate)
            if (spec.Gusset != null)
            {
                var gussetOutline = ConnectionGeometry.GetGussetOutline(spec.Gusset);
                if (gussetOutline.Count >= 3)
                {
                    string plateName = "Cartela " + (spec.Source?.Drawing ?? "Nudo");
                    double gussetThickness = spec.Gusset.ThicknessMm.GetValueOrDefault(9.525);
                    ElementId gussetId = backend.CreatePlate(document, frame, gussetOutline, gussetThickness, plateName);
                    if (gussetId != ElementId.InvalidElementId)
                    {
                        createdIds.Add(gussetId);
                    }
                }
            }

            // 6. Crear uniones de cada miembro (placas cuchilla, pernos, soldaduras)
            if (spec.Members != null)
            {
                foreach (var mSpec in spec.Members)
                {
                    var mInfo = members.FirstOrDefault(m => m.Id == mSpec.ElementId);
                    if (mInfo == null || mSpec.Attachment == null) continue;

                    var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, mInfo.Start, mInfo.End, workPointMm);
                    double setback = mSpec.EndSetbackMm.GetValueOrDefault(0.0);

                    // A) Placa cuchilla empernada
                    if (string.Equals(mSpec.Attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
                    {
                        if (mSpec.Attachment.Plate != null)
                        {
                            var kpCorners = ConnectionGeometry.ComputeKnifePlateCorners(ux, uy, setback, mSpec.Attachment.Plate);
                            if (kpCorners.Count >= 3)
                            {
                                string kpName = "Placa cuchilla miembro " + mSpec.ElementId;
                                double kpThickness = mSpec.Attachment.Plate.ThicknessMm.GetValueOrDefault(10.0);
                                ElementId kpId = backend.CreatePlate(document, frame, kpCorners, kpThickness, kpName);
                                if (kpId != ElementId.InvalidElementId)
                                {
                                    createdIds.Add(kpId);
                                }
                            }
                        }

                        if (mSpec.Attachment.Bolts != null && mSpec.Attachment.Plate != null)
                        {
                            var boltPositions = ConnectionGeometry.ComputeBoltPositions(ux, uy, setback, mSpec.Attachment.Plate, mSpec.Attachment.Bolts);
                            if (boltPositions.Count > 0)
                            {
                                double diaMm = mSpec.Attachment.Bolts.DiameterMm.GetValueOrDefault(15.875);
                                double lenMm = 45.0;
                                string boltName = "Pernos miembro " + mSpec.ElementId;
                                var boltIds = backend.CreateBoltGroup(document, frame, boltPositions, diaMm, lenMm, boltName);
                                createdIds.AddRange(boltIds);
                            }
                        }

                        // Soldaduras de la unión placa-miembro
                        var weldLines = ConnectionGeometry.ComputeWeldLines(ux, uy, setback, mSpec);
                        if (weldLines.Count > 0)
                        {
                            var weldIds = backend.CreateWelds(document, frame, weldLines, "Soldadura miembro " + mSpec.ElementId);
                            createdIds.AddRange(weldIds);
                        }
                    }
                    // B) Ranura soldada directamente a cartela
                    else if (string.Equals(mSpec.Attachment.Type, "welded_slot", StringComparison.OrdinalIgnoreCase))
                    {
                        var weldLines = ConnectionGeometry.ComputeWeldLines(ux, uy, setback, mSpec);
                        if (weldLines.Count > 0)
                        {
                            var weldIds = backend.CreateWelds(document, frame, weldLines, "Soldadura ranura miembro " + mSpec.ElementId);
                            createdIds.AddRange(weldIds);
                        }
                    }
                }
            }

            // 7. Persistir en Extensible Storage
            var record = new ConnectionRecord
            {
                ConnectionId = !string.IsNullOrWhiteSpace(connectionId) ? connectionId! : Guid.NewGuid().ToString("D"),
                SpecVersion = spec.SpecVersion ?? "1.0",
                ConnectionType = spec.ConnectionType ?? "gusset_node",
                SpecJson = rawSpecJson,
                CreatedElementIds = createdIds.Select(id => id.Value).ToList(),
                ModifiedMembers = modifiedMembers,
                CreatedUtc = DateTime.UtcNow.ToString("o")
            };

            ConnectionStorageManager.SaveConnection(document, record);
            return record;
        }

        public static bool DeleteConnection(Document document, string connectionId, out ConnectionRecord? deletedRecord)
        {
            deletedRecord = null;
            if (document == null || string.IsNullOrWhiteSpace(connectionId)) return false;

            var record = ConnectionStorageManager.GetConnection(document, connectionId);
            if (record == null) return false;

            // 1. Restaurar miembros modificados
            if (record.ModifiedMembers != null)
            {
                foreach (var mod in record.ModifiedMembers)
                {
                    MemberModifier.RestoreSetback(document, mod);
                }
            }

            // 2. Borrar elementos geométricos creados por el add-in
            if (record.CreatedElementIds != null && record.CreatedElementIds.Count > 0)
            {
                var toDelete = record.CreatedElementIds
                    .Select(id => new ElementId(id))
                    .Where(id => document.GetElement(id) != null)
                    .ToList();

                if (toDelete.Count > 0)
                {
                    document.Delete(toDelete);
                }
            }

            // 3. Borrar DataStorage de Extensible Storage
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
            if (string.IsNullOrWhiteSpace(connectionId)) throw new ArgumentException("connectionId requerido.", nameof(connectionId));

            // Borrar geometría anterior y restaurar miembros
            DeleteConnection(document, connectionId, out _);

            // Recrear con la nueva especificación conservando el mismo ID
            return CreateConnection(document, uidoc, spec, rawSpecJson, connectionId, warnings);
        }
    }
}
