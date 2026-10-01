using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_get_node_info</c>: inspecciona los miembros seleccionados (o por <c>element_ids</c>):
    /// punto de trabajo, sistema local, perfiles, material, pendiente, ángulo en el plano y conexiones existentes.
    /// </summary>
    public sealed class NodeInfoOperation : IOperation
    {
        public string Name => "node_info";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            List<long> ids = NodeInspector.ResolveIds(context.Request, context.UIDocument);

            if (ids.Count < 2)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.InvalidRequest,
                    "Se requieren al menos 2 miembros de armazón estructural para inspeccionar el nudo (recibidos: " + ids.Count + ").",
                    "element_ids",
                    "Selecciona los miembros del nudo en Revit o pasa sus IDs en 'element_ids'."));
            }

            List<MemberInfo> members;
            try
            {
                members = NodeInspector.ReadMembers(doc, ids, "element_ids");
            }
            catch (NodeInspectionException ex)
            {
                return ApiResponse.Failure(Name, ex.Error);
            }

            MemberInfo chord;
            List<MemberInfo> others;
            try
            {
                chord = NodeInspector.ChooseChord(members, context.Request, out others);
            }
            catch (NodeInspectionException ex)
            {
                return ApiResponse.Failure(Name, ex.Error);
            }

            if (others.Count == 0)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.InvalidRequest,
                    "No hay diagonales ni montantes adicionales además del cordón.",
                    "element_ids",
                    "Selecciona al menos una barra que concurra al cordón."));
            }

            NodeFrame frame;
            try
            {
                frame = NodeInspector.ComputeFrame(chord, others[0]);
            }
            catch (NodeInspectionException ex)
            {
                return ApiResponse.Failure(Name, ex.Error);
            }

            Vec3 originMm = frame.Origin;

            // Extraer datos enriquecidos de cada miembro
            var membersData = new List<object>();
            foreach (var m in members)
            {
                double dStart = m.Start.DistanceTo(originMm);
                double dEnd = m.End.DistanceTo(originMm);
                int nodeEnd = dStart <= dEnd ? 0 : 1;

                Vec3 outward = nodeEnd == 0 ? m.Direction : m.Direction * -1.0;
                double lx = outward.Dot(frame.X);
                double ly = outward.Dot(frame.Y);
                double angleInPlane = UnitConverter.RadiansToDegrees(Math.Atan2(Math.Abs(ly), lx));

                string materialName = GetStructuralMaterialName(m.Instance);

                membersData.Add(new
                {
                    element_id = m.Id,
                    family = m.FamilyName,
                    type = m.TypeName,
                    structural_type = m.Instance.StructuralType.ToString(),
                    start_mm = new[] { UnitConverter.RoundMm(m.Start.X), UnitConverter.RoundMm(m.Start.Y), UnitConverter.RoundMm(m.Start.Z) },
                    end_mm = new[] { UnitConverter.RoundMm(m.End.X), UnitConverter.RoundMm(m.End.Y), UnitConverter.RoundMm(m.End.Z) },
                    length_mm = UnitConverter.RoundMm(m.Start.DistanceTo(m.End)),
                    slope_deg = Math.Round(m.SlopeDegrees, 2),
                    angle_in_plane_deg = Math.Round(angleInPlane, 1),
                    node_end = nodeEnd,
                    material = materialName,
                    is_chord = m.Id == chord.Id
                });
            }

            // Consultar conexiones existentes en Extensible Storage que involucren a estos miembros
            var existingConnections = new List<object>();
            var storedList = ConnectionStorageManager.ListConnections(doc);
            var idSet = new HashSet<long>(ids);

            foreach (var stored in storedList)
            {
                try
                {
                    var spec = ConnectionSpec.FromJson(stored.SpecJson);
                    bool touches = false;
                    if (spec?.Node?.ElementIds != null && spec.Node.ElementIds.Any(id => idSet.Contains(id))) touches = true;
                    if (spec?.Chord != null && idSet.Contains(spec.Chord.ElementId)) touches = true;

                    if (touches)
                    {
                        existingConnections.Add(new
                        {
                            connection_id = stored.ConnectionId,
                            connection_type = stored.ConnectionType,
                            created_utc = stored.CreatedUtc,
                            created_elements_count = stored.CreatedElementIds.Count
                        });
                    }
                }
                catch { }
            }

            var data = new
            {
                origin_mm = new[] { UnitConverter.RoundMm(frame.Origin.X), UnitConverter.RoundMm(frame.Origin.Y), UnitConverter.RoundMm(frame.Origin.Z) },
                x_axis = new[] { Math.Round(frame.X.X, 6), Math.Round(frame.X.Y, 6), Math.Round(frame.X.Z, 6) },
                y_axis = new[] { Math.Round(frame.Y.X, 6), Math.Round(frame.Y.Y, 6), Math.Round(frame.Y.Z, 6) },
                z_axis = new[] { Math.Round(frame.Z.X, 6), Math.Round(frame.Z.Y, 6), Math.Round(frame.Z.Z, 6) },
                axis_distance_mm = Math.Round(frame.AxisDistanceMm, 2),
                chord_element_id = chord.Id,
                members = membersData,
                existing_connections = existingConnections
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }

        private static string GetStructuralMaterialName(FamilyInstance instance)
        {
            try
            {
                Parameter? matParam = instance.get_Parameter(BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
                if (matParam != null && matParam.HasValue)
                {
                    ElementId matId = matParam.AsElementId();
                    if (matId != ElementId.InvalidElementId)
                    {
                        Element? mat = instance.Document.GetElement(matId);
                        if (mat != null) return mat.Name;
                    }
                }
            }
            catch { }
            return "Steel";
        }
    }
}
