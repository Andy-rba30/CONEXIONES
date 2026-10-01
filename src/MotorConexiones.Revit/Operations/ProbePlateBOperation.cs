using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// Prueba técnica de la Fase 1 por el camino B: una placa rectangular y 4 pernos (2x2) en el nudo formado por
    /// los <c>element_ids</c> (o la selección). Petición opcional:
    /// <c>{ "element_ids": [...], "chord_element_id": 111, "plate": {"width_mm":200,"height_mm":200,"thickness_mm":10},
    ///       "bolts": {"diameter_mm":15.875,"spacing_mm":60,"length_mm":40}, "offset_x_mm": 0 }</c>.
    /// Todo dentro de un TransactionGroup: o se crea todo o nada.
    /// </summary>
    public sealed class ProbePlateBOperation : IOperation
    {
        public string Name => "probe_plate_b";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            List<long> ids = NodeInspector.ResolveIds(context.Request, context.UIDocument);
            if (ids.Count < 2)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.NodeNeedsTwoMembers,
                    "Hacen falta al menos dos miembros (cordón y una diagonal); se recibieron " + ids.Count + ".",
                    "element_ids", "Selecciona en Revit el cordón y las barras que llegan al nudo, o pasa element_ids."),
                    context.Warnings);
            }

            NodeFrame frame;
            MemberInfo chord;
            List<MemberInfo> others;
            List<MemberInfo> members;
            try
            {
                members = NodeInspector.ReadMembers(doc, ids, "element_ids");
                chord = NodeInspector.ChooseChord(members, context.Request, out others);
                frame = NodeInspector.ComputeFrame(chord, others[0]);
            }
            catch (NodeInspectionException error)
            {
                return ApiResponse.Failure(Name, error.Error, context.Warnings);
            }

            double width = ReadNumber(context, "plate", "width_mm", 200);
            double height = ReadNumber(context, "plate", "height_mm", 200);
            double thickness = ReadNumber(context, "plate", "thickness_mm", 10);
            double diameter = ReadNumber(context, "bolts", "diameter_mm", 15.875);
            double spacing = ReadNumber(context, "bolts", "spacing_mm", 60);
            double boltLength = ReadNumber(context, "bolts", "length_mm", thickness + 30);
            double offsetX = context.GetDouble("offset_x_mm", 0);

            var outline = new List<BoltPosition>
            {
                new BoltPosition(offsetX - width / 2, -height / 2),
                new BoltPosition(offsetX + width / 2, -height / 2),
                new BoltPosition(offsetX + width / 2, height / 2),
                new BoltPosition(offsetX - width / 2, height / 2),
            };
            var bolts = new List<BoltPosition>
            {
                new BoltPosition(offsetX - spacing / 2, -spacing / 2),
                new BoltPosition(offsetX + spacing / 2, -spacing / 2),
                new BoltPosition(offsetX + spacing / 2, spacing / 2),
                new BoltPosition(offsetX - spacing / 2, spacing / 2),
            };

            var backend = new DirectShapeBackend(context.Warnings);
            ElementId plateId;
            IList<ElementId> boltIds;
            string probeId = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            try
            {
                using var scope = new OperationScope(doc, context.UIApplication, Name, probeId, context.Warnings);
                using (Transaction transaction = scope.StartTransaction(doc, "Placa y pernos de prueba (camino B)"))
                {
                    plateId = backend.CreatePlate(doc, frame, outline, thickness, "MotorConexiones sondeo B placa " + probeId);
                    boltIds = backend.CreateBoltGroup(doc, frame, bolts, diameter, boltLength, "MotorConexiones sondeo B perno " + probeId);
                    scope.CommitOrThrow(transaction);
                }
                scope.Commit();
            }
            catch (Exception error) when (error is not NodeInspectionException)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.FabricationFailed,
                    "No se pudo crear la placa de prueba: " + error.GetType().Name + ": " + error.Message,
                    hint: "Se deshizo todo. Revisa las advertencias devueltas."), context.Warnings);
            }

            var data = new
            {
                backend = backend.Name,
                probe_id = probeId,
                chord = chord.ToData(),
                members = others.Select(m => m.ToData()).ToList(),
                frame = NodeInspector.FrameToData(frame),
                plate = new { element_id = plateId.Value, width_mm = width, height_mm = height, thickness_mm = thickness, offset_x_mm = offsetX },
                bolts = new { element_ids = boltIds.Select(id => id.Value).ToList(), diameter_mm = diameter, spacing_mm = spacing, length_mm = boltLength },
                undo_entry = "MotorConexiones: " + Name + " " + probeId,
            };
            return ApiResponse.Success(Name, data, context.Warnings);
        }

        private static double ReadNumber(OperationContext context, string group, string name, double defaultValue)
        {
            if (context.TryGet(group, out var element) && element.ValueKind == System.Text.Json.JsonValueKind.Object
                && element.TryGetProperty(name, out var value) && value.ValueKind == System.Text.Json.JsonValueKind.Number)
            {
                return value.GetDouble();
            }
            return defaultValue;
        }
    }
}
