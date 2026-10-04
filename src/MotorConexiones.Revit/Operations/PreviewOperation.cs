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
using MotorConexiones.Revit.Services;

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

            ResolvedNode node;
            try
            {
                node = NodeInspector.ResolveNode(doc, spec);
            }
            catch (NodeInspectionException ex)
            {
                return ApiResponse.Failure(Name, ex.Error);
            }

            List<MemberInfo> members = node.Members;
            NodeFrame frame = node.Frame;
            Vec3 workPointMm = frame.Origin;

            var elementsToCreate = new List<object>();
            var membersToModify = new List<object>();
            var boltStacks = new List<object>();
            LimitsConfig limits = LimitsConfigLoader.Load();

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
                        double currentEndMm = MemberModifier.CurrentEndDistanceMm(mInfo.Instance, workPointMm, out int endIndex);
                        membersToModify.Add(new
                        {
                            element_id = mSpec.ElementId,
                            role = mSpec.Role,
                            profile = mInfo.TypeName,
                            end = endIndex == 0 ? "start" : "end",
                            current_end_distance_mm = Math.Round(currentEndMm, 1),
                            setback_mm = setback,
                            new_extension_mm = Math.Round(MemberModifier.TargetExtensionMm(currentEndMm, setback), 1),
                            action = "Fijar Start/End Extension para que el extremo quede a setback_mm del punto de trabajo"
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
                                BoltStack stack = ConnectionCreationService.ComputeBoltStack(spec, mSpec, limits);
                                elementsToCreate.Add(new
                                {
                                    kind = "bolt_group",
                                    for_member_id = mSpec.ElementId,
                                    count = boltCount,
                                    diameter_mm = mSpec.Attachment.Bolts.DiameterMm,
                                    rows = r,
                                    columns = c,
                                    spacing_mm = mSpec.Attachment.Bolts.SpacingMm,
                                    edge_mm = mSpec.Attachment.Bolts.EdgeMm,
                                    bolt_length_mm = stack.BoltLengthMm,
                                    grip_mm = stack.GripMm
                                });
                                boltStacks.Add(new
                                {
                                    member_element_id = mSpec.ElementId,
                                    gusset_thickness_mm = stack.GussetThicknessMm,
                                    knife_plate_thickness_mm = stack.KnifeThicknessMm,
                                    knife_plate_z_offset_mm = Math.Round(stack.KnifePlateZOffsetMm, 3),
                                    gusset_face = BoltStack.GussetFace,
                                    grip_mm = Math.Round(stack.GripMm, 3),
                                    length_addition_mm = Math.Round(stack.LengthAdditionMm, 3),
                                    bolt_length_mm = Math.Round(stack.BoltLengthMm, 3),
                                    head_face_z_mm = Math.Round(stack.OuterFaceZMm, 3),
                                    length_source = "grip + AISC 7-15 (config/limits.json), redondeado a " + stack.LengthIncrementMm.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture) + " mm"
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
                backend = BackendFactory.GetBackend(doc, context.Warnings).Name,
                chord_element_id = node.Chord.Id,
                first_member_element_id = node.FirstMember.Id,
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
                members_to_modify = membersToModify,
                bolt_stacks = boltStacks
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
