using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_preview</c>: simulación en texto que lista con precisión todos los elementos que se crearían
    /// y los miembros que se modificarían, sin realizar ningún cambio en el modelo.
    /// </summary>
    public sealed class PreviewOperation : IOperation
    {
        public string Name => "preview";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            string rawSpecJson;

            if (context.TryGet("spec", out var specEl) && specEl.ValueKind == JsonValueKind.Object)
            {
                rawSpecJson = specEl.GetRawText();
            }
            else
            {
                rawSpecJson = context.Request.GetRawText();
            }

            ConnectionSpec? spec = null;
            try
            {
                spec = ConnectionSpec.FromJson(rawSpecJson);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.SchemaInvalid,
                    "No se pudo deserializar la especificación: " + ex.Message,
                    "",
                    "Verifica que el JSON sea un objeto válido."));
            }

            if (spec == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SchemaInvalid, "Especificación nula."));
            }

            var ids = new HashSet<long>();
            if (spec.Node?.ElementIds != null)
            {
                foreach (var id in spec.Node.ElementIds) ids.Add(id);
            }
            if (spec.Chord != null && spec.Chord.ElementId > 0) ids.Add(spec.Chord.ElementId);
            if (spec.Members != null)
            {
                foreach (var m in spec.Members) if (m.ElementId > 0) ids.Add(m.ElementId);
            }

            if (ids.Count < 2)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.InvalidRequest,
                    "Se requieren al menos 2 barras en node.element_ids para la simulación.",
                    "node.element_ids",
                    "Incluye los IDs del nudo."));
            }

            List<MemberInfo> members;
            try
            {
                members = NodeInspector.ReadMembers(doc, ids, "node.element_ids");
            }
            catch (NodeInspectionException ex)
            {
                return ApiResponse.Failure(Name, ex.Error);
            }

            var chord = members.FirstOrDefault(m => m.Id == spec.Chord?.ElementId) ?? NodeInspector.ChooseChord(members, default, out _);
            var firstMember = members.FirstOrDefault(m => m.Id != chord.Id);
            if (firstMember == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "No hay barras adicionales además del cordón."));
            }

            NodeFrame frame = NodeInspector.ComputeFrame(chord, firstMember);
            Vec3 workPointMm = frame.Origin;

            var elementsToCreate = new List<object>();
            var membersToModify = new List<object>();

            // 1. Cartela
            if (spec.Gusset != null)
            {
                elementsToCreate.Add(new
                {
                    kind = "gusset_plate",
                    thickness_mm = spec.Gusset.ThicknessMm,
                    thickness_label = spec.Gusset.ThicknessLabel,
                    width_mm = spec.Gusset.WidthMm,
                    height_mm = spec.Gusset.HeightMm,
                    vertices_count = spec.Gusset.Outline?.PointsMm?.Count ?? 0,
                    chord_interface = spec.Gusset.ChordInterface
                });
            }

            // 2. Miembros y uniones
            int totalBolts = 0;
            int totalKnifePlates = 0;
            int totalWelds = 0;

            if (spec.Members != null)
            {
                foreach (var mSpec in spec.Members)
                {
                    var mInfo = members.FirstOrDefault(m => m.Id == mSpec.ElementId);
                    if (mInfo == null) continue;

                    double setback = mSpec.EndSetbackMm.GetValueOrDefault(0.0);
                    if (setback > 0.0)
                    {
                        membersToModify.Add(new
                        {
                            element_id = mSpec.ElementId,
                            role = mSpec.Role,
                            profile = mInfo.TypeName,
                            setback_mm = setback,
                            action = "Acortar extensión del miembro (START/END_EXTENSION)"
                        });
                    }

                    if (mSpec.Attachment != null)
                    {
                        var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, mInfo.Start, mInfo.End, workPointMm);

                        if (string.Equals(mSpec.Attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase))
                        {
                            if (mSpec.Attachment.Plate != null)
                            {
                                totalKnifePlates++;
                                elementsToCreate.Add(new
                                {
                                    kind = "knife_plate",
                                    for_member_id = mSpec.ElementId,
                                    thickness_mm = mSpec.Attachment.Plate.ThicknessMm,
                                    length_mm = mSpec.Attachment.Plate.LengthMm,
                                    width_mm = mSpec.Attachment.Plate.WidthMm,
                                    insertion_mm = mSpec.Attachment.Plate.InsertionMm
                                });
                            }

                            if (mSpec.Attachment.Bolts != null)
                            {
                                int r = mSpec.Attachment.Bolts.Rows.GetValueOrDefault(1);
                                int c = mSpec.Attachment.Bolts.Columns.GetValueOrDefault(1);
                                int boltCount = r * c;
                                totalBolts += boltCount;
                                elementsToCreate.Add(new
                                {
                                    kind = "bolt_group",
                                    for_member_id = mSpec.ElementId,
                                    count = boltCount,
                                    diameter_mm = mSpec.Attachment.Bolts.DiameterMm,
                                    rows = r,
                                    columns = c,
                                    spacing_mm = mSpec.Attachment.Bolts.SpacingMm,
                                    edge_mm = mSpec.Attachment.Bolts.EdgeMm
                                });
                            }

                            var weldLines = ConnectionGeometry.ComputeWeldLines(ux, uy, setback, mSpec);
                            totalWelds += weldLines.Count;
                        }
                        else if (string.Equals(mSpec.Attachment.Type, "welded_slot", StringComparison.OrdinalIgnoreCase))
                        {
                            var weldLines = ConnectionGeometry.ComputeWeldLines(ux, uy, setback, mSpec);
                            totalWelds += weldLines.Count;
                            elementsToCreate.Add(new
                            {
                                kind = "welded_slot_interface",
                                for_member_id = mSpec.ElementId,
                                slot_length_mm = mSpec.Attachment.SlotLengthMm,
                                weld_size_mm = mSpec.Attachment.Weld?.SizeMm
                            });
                        }
                    }
                }
            }

            var summary = new
            {
                connection_type = spec.ConnectionType,
                working_point_mm = new[] { Math.Round(workPointMm.X, 1), Math.Round(workPointMm.Y, 1), Math.Round(workPointMm.Z, 1) },
                gusset_plates = 1,
                knife_plates = totalKnifePlates,
                bolts = totalBolts,
                weld_lines = totalWelds,
                members_modified = membersToModify.Count,
                dry_run = true
            };

            var data = new
            {
                summary = summary,
                elements_to_create = elementsToCreate,
                members_to_modify = membersToModify
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
