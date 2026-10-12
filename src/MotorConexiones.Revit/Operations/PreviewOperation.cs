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
            LimitsConfig limits = LimitsConfigLoader.Load();
            double gussetThickness = spec.Gusset?.ThicknessMm ?? 9.525;

            // Fase 12: los tipos nuevos tienen su propia simulación.
            if (spec.IsType(DirectWeldValidator.TypeName)) return ApiResponse.Success(Name, PreviewDirectWeld(doc, spec, node, context), context.Warnings);
            if (spec.IsType(EndPlateValidator.TypeName)) return ApiResponse.Success(Name, PreviewEndPlate(doc, spec, node, limits, context), context.Warnings);

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
                            BoltStack? stack = mSpec.Attachment.Plate != null
                                ? BoltStack.Compute(gussetThickness, mSpec.Attachment.Plate, mSpec.Attachment.Bolts, limits)
                                : null;

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
                                    insertion_mm = mSpec.Attachment.Plate.InsertionMm,
                                    gusset_face = stack!.FaceLabel,
                                    offset_from_gusset_plane_mm = Math.Round(stack.PlateOffsetMm, 3)
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
                                    edge_mm = mSpec.Attachment.Bolts.EdgeMm,
                                    grip_mm = stack != null ? Math.Round(stack.GripMm, 3) : (double?)null,
                                    length_mm = stack != null ? Math.Round(stack.BoltLengthMm, 3) : mSpec.Attachment.Bolts.LengthMm,
                                    length_source = stack == null ? "unknown" : stack.LengthFromSpec ? "spec" : "computed_from_grip"
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
                members_to_modify = membersToModify
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }

        /// <summary>Fase 12: lo que crearía <c>direct_weld</c>: por barra, el retiro por el eje (reserva del sondeo 22) y la soldadura sobre la huella.</summary>
        private static object PreviewDirectWeld(Document doc, ConnectionSpec spec, ResolvedNode node, OperationContext context)
        {
            NodeFrame frame = node.Frame;
            var facts = new RevitModelFacts(doc, frame);
            double chordHalf = DirectWeldBuilder.ChordHalfDepthMm(facts.GetMemberFacts(node.Chord.Id));
            var elements = new List<object>();
            var modify = new List<object>();
            foreach (var mSpec in spec.Members ?? new List<MemberSpec>())
            {
                MemberInfo? info = node.Find(mSpec.ElementId);
                if (info == null) continue;
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, info.Start, info.End, frame.Origin);
                double offset = NodeInspector.AxisOffsetMm(frame, info);
                var mf = facts.GetMemberFacts(info.Id);
                double width = mf != null && mf.WidthMm > 0 ? Math.Min(mf.WidthMm, mf.HeightMm > 0 ? mf.HeightMm : mf.WidthMm) : 63.5;
                DirectWeldEnd end = DirectWeldGeometry.ComputeEnd(ux, uy, offset, chordHalf, width, mSpec.EndGapMm ?? 0.0);
                Vec3 cut = frame.Origin + frame.X * offset;
                double currentEndMm = MemberModifier.CurrentEndDistanceMm(info.Instance, cut, out int endIndex);
                modify.Add(new
                {
                    element_id = mSpec.ElementId,
                    role = mSpec.Role,
                    profile = info.TypeName,
                    end = endIndex == 0 ? "start" : "end",
                    axis_offset_mm = Math.Round(offset, 1),
                    current_end_distance_mm = Math.Round(currentEndMm, 1),
                    setback_mm = Math.Round(end.AxisSetbackMm, 1),
                    ideal_setback_mm = Math.Round(end.SetbackMm, 1),
                    new_extension_mm = Math.Round(MemberModifier.TargetExtensionMm(currentEndMm, end.AxisSetbackMm), 1),
                    action = "Retiro por el eje hasta la cara del cordón (reserva del corte inclinado, sondeo 22), medido desde el corte propio de la barra",
                });
                elements.Add(new
                {
                    kind = "direct_weld",
                    for_member_id = mSpec.ElementId,
                    chord_element_id = mSpec.ChordElementId ?? node.Chord.Id,
                    weld_size_mm = mSpec.Weld?.SizeMm,
                    all_around = mSpec.Weld?.AllAround,
                    footprint_mm = Math.Round(end.FootprintLengthMm, 1),
                    face_y_mm = Math.Round(end.FaceY, 1),
                    end_gap_mm = mSpec.EndGapMm ?? 0.0,
                    splice_hint = mSpec.SpliceHint == null ? null : new { detail = mSpec.SpliceHint.Detail, offset_from_chord_face_mm = mSpec.SpliceHint.OffsetFromChordFaceMm },
                });
            }
            return new
            {
                summary = new
                {
                    connection_type = spec.ConnectionType,
                    backend = BackendFactory.GetBackend(doc, context.Warnings).Name,
                    chord_element_id = node.Chord.Id,
                    first_member_element_id = node.FirstMember.Id,
                    working_point_mm = new[] { Math.Round(frame.Origin.X, 1), Math.Round(frame.Origin.Y, 1), Math.Round(frame.Origin.Z, 1) },
                    eccentricity_mm = Math.Round(frame.EccentricityMm, 1),
                    gusset_plates = 0,
                    knife_plates = 0,
                    bolts = 0,
                    weld_lines = elements.Count,
                    members_modified = modify.Count,
                    end_cut = "axis_setback (END_CUT_APPROXIMATED hasta el sondeo 22)",
                    dry_run = true,
                },
                elements_to_create = elements,
                members_to_modify = modify,
            };
        }

        /// <summary>Fase 12: lo que crearía <c>end_plate</c>: los dos retiros, las dos planchas, las filas de pernos y las dos soldaduras.</summary>
        private static object PreviewEndPlate(Document doc, ConnectionSpec spec, ResolvedNode node, LimitsConfig limits, OperationContext context)
        {
            NodeFrame frame = node.Frame;
            double t = spec.Plate?.ThicknessMm ?? 0.0;
            double gap = Math.Max(0.0, spec.GapMm ?? 0.0);
            var modify = new List<object>();
            foreach (var (id, setback, role) in new[] { (node.Chord.Id, spec.Chord?.EndSetbackMm, "tramo 1"), (spec.Members?.Count > 0 ? spec.Members[0].ElementId : 0, spec.Members?.Count > 0 ? spec.Members[0].EndSetbackMm : null, "tramo 2") })
            {
                MemberInfo? info = node.Find(id);
                if (info == null || !setback.HasValue || setback.Value <= 0) continue;
                double currentEndMm = MemberModifier.CurrentEndDistanceMm(info.Instance, frame.Origin, out int endIndex);
                modify.Add(new { element_id = id, role, profile = info.TypeName, end = endIndex == 0 ? "start" : "end", current_end_distance_mm = Math.Round(currentEndMm, 1), setback_mm = setback.Value, new_extension_mm = Math.Round(MemberModifier.TargetExtensionMm(currentEndMm, setback.Value), 1), action = "Retiro desde el punto de empalme para dejar sitio a la plancha" });
            }
            BoltStack stack = BoltStack.ForTwoPlates(t, t, gap, spec.Bolts, limits);
            var rows = BoltRing.Rows(spec.Plate, spec.Bolts).Where(r => r.Count > 0).Select(r => new { face = r.Face, count = r.Count, spacing_mm = r.SpacingMm, edge_mm = r.EdgeMm, pattern = r.Count + " x 1" }).ToList();
            var elements = new List<object>
            {
                new { kind = "end_plate", for_member_id = node.Chord.Id, thickness_mm = t, width_mm = spec.Plate?.WidthMm, height_mm = spec.Plate?.HeightMm, corner_cut_mm = spec.Plate?.CornerCutMm, vertices_count = Core.Geometry2D.Octagon.Points(spec.Plate?.WidthMm ?? 0, spec.Plate?.HeightMm ?? 0, spec.Plate?.CornerCutMm ?? 0).Count, weld_size_mm = spec.Plate?.Weld?.SizeMm },
                new { kind = "end_plate", for_member_id = spec.Members?.Count > 0 ? spec.Members[0].ElementId : 0, thickness_mm = t, width_mm = spec.Plate?.WidthMm, height_mm = spec.Plate?.HeightMm, corner_cut_mm = spec.Plate?.CornerCutMm, vertices_count = Core.Geometry2D.Octagon.Points(spec.Plate?.WidthMm ?? 0, spec.Plate?.HeightMm ?? 0, spec.Plate?.CornerCutMm ?? 0).Count, weld_size_mm = spec.Plate?.Weld?.SizeMm },
                new { kind = "bolt_ring", count = BoltRing.Count(spec.Bolts), diameter_mm = spec.Bolts?.DiameterMm, hole_mm = spec.Bolts?.HoleMm, nuts = spec.Bolts?.Nuts ?? "both_sides", nuts_note = "perno estándar con aviso BOLT_NUTS_APPROXIMATED (D9)", grip_mm = Math.Round(stack.GripMm, 3), length_mm = Math.Round(stack.BoltLengthMm, 3), length_source = stack.LengthFromSpec ? "spec" : "computed_from_grip", rows },
            };
            return new
            {
                summary = new
                {
                    connection_type = spec.ConnectionType,
                    kind = spec.Kind,
                    backend = BackendFactory.GetBackend(doc, context.Warnings).Name,
                    chord_element_id = node.Chord.Id,
                    first_member_element_id = node.FirstMember.Id,
                    working_point_mm = new[] { Math.Round(frame.Origin.X, 1), Math.Round(frame.Origin.Y, 1), Math.Round(frame.Origin.Z, 1) },
                    gusset_plates = 0,
                    end_plates = 2,
                    knife_plates = 0,
                    bolts = BoltRing.Count(spec.Bolts),
                    weld_lines = 8,
                    members_modified = modify.Count,
                    gap_mm = gap,
                    dry_run = true,
                },
                elements_to_create = elements,
                members_to_modify = modify,
            };
        }
    }
}
