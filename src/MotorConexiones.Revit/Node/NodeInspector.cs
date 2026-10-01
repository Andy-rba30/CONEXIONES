using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit.Node
{
    /// <summary>Un miembro del nudo leído del modelo (todo en mm).</summary>
    public sealed class MemberInfo
    {
        public MemberInfo(FamilyInstance instance, Vec3 start, Vec3 end)
        {
            Instance = instance;
            Start = start;
            End = end;
        }

        public FamilyInstance Instance { get; }
        public Vec3 Start { get; }
        public Vec3 End { get; }
        public long Id => Instance.Id.Value;
        public string FamilyName => Instance.Symbol.FamilyName;
        public string TypeName => Instance.Symbol.Name;
        public Vec3 Direction => (End - Start).Normalized();

        /// <summary>Ángulo del eje respecto a la horizontal, en grados (0 = horizontal).</summary>
        public double SlopeDegrees => UnitConverter.RadiansToDegrees(Math.Asin(Math.Min(1.0, Math.Abs(Direction.Z))));

        public object ToData() => new
        {
            element_id = Id,
            family = FamilyName,
            type = TypeName,
            structural_type = Instance.StructuralType.ToString(),
            start_mm = new[] { UnitConverter.RoundMm(Start.X), UnitConverter.RoundMm(Start.Y), UnitConverter.RoundMm(Start.Z) },
            end_mm = new[] { UnitConverter.RoundMm(End.X), UnitConverter.RoundMm(End.Y), UnitConverter.RoundMm(End.Z) },
            length_mm = UnitConverter.RoundMm(Start.DistanceTo(End)),
            slope_deg = Math.Round(SlopeDegrees, 2),
        };
    }

    /// <summary>Fallo al leer el nudo, con código del contrato.</summary>
    public sealed class NodeInspectionException : Exception
    {
        public NodeInspectionException(ApiError error) : base(error.Message)
        {
            Error = error;
        }

        public ApiError Error { get; }
    }

    /// <summary>Lee los miembros del nudo del modelo y calcula su sistema local (sección 7 del encargo).</summary>
    public static class NodeInspector
    {
        private static readonly ElementId FramingCategory = new ElementId(BuiltInCategory.OST_StructuralFraming);

        /// <summary>Convierte una lista de IDs en miembros; falla con <c>ELEMENT_NOT_FOUND</c> o <c>ELEMENT_NOT_A_MEMBER</c>.</summary>
        public static List<MemberInfo> ReadMembers(Document document, IEnumerable<long> ids, string path)
        {
            var members = new List<MemberInfo>();
            int index = 0;
            foreach (long id in ids)
            {
                string itemPath = path + "[" + index + "]";
                index++;
                Element? element = document.GetElement(new ElementId(id));
                if (element == null)
                {
                    throw new NodeInspectionException(new ApiError(ErrorCodes.ElementNotFound,
                        "No existe ningún elemento con id " + id + ".", itemPath,
                        "Selecciona los miembros en Revit y usa get_selected_elements para leer sus IDs."));
                }

                if (element is not FamilyInstance instance || element.Category == null || element.Category.Id != FramingCategory)
                {
                    throw new NodeInspectionException(new ApiError(ErrorCodes.ElementNotAMember,
                        "El elemento " + id + " (" + element.Category?.Name + ") no es armazón estructural.", itemPath,
                        "Solo se admiten vigas, diagonales y montantes de la categoría Armazón estructural."));
                }

                if (instance.Location is not LocationCurve location || location.Curve == null)
                {
                    throw new NodeInspectionException(new ApiError(ErrorCodes.ElementNotAMember,
                        "El elemento " + id + " no tiene línea de ubicación.", itemPath,
                        "Los miembros del nudo deben ser barras con eje (LocationCurve)."));
                }

                Curve curve = location.Curve;
                members.Add(new MemberInfo(instance, RevitGeometry.ToMm(curve.GetEndPoint(0)), RevitGeometry.ToMm(curve.GetEndPoint(1))));
            }
            return members;
        }

        /// <summary>IDs de la petición (<c>element_ids</c>) o, si no vienen, de la selección actual de Revit.</summary>
        public static List<long> ResolveIds(JsonElement request, Autodesk.Revit.UI.UIDocument? uiDocument)
        {
            if (request.ValueKind == JsonValueKind.Object && request.TryGetProperty("element_ids", out var idsElement) && idsElement.ValueKind == JsonValueKind.Array)
            {
                var ids = new List<long>();
                foreach (JsonElement item in idsElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Number && item.TryGetInt64(out long value)) ids.Add(value);
                }
                return ids;
            }

            if (uiDocument != null)
            {
                return uiDocument.Selection.GetElementIds().Select(id => id.Value).OrderBy(id => id).ToList();
            }
            return new List<long>();
        }

        /// <summary>
        /// Elige el cordón: <c>chord_element_id</c> si viene; si no, el miembro más horizontal (menor pendiente) y,
        /// a igualdad, el de mayor longitud. Devuelve el cordón y el resto de miembros en el orden recibido.
        /// </summary>
        public static MemberInfo ChooseChord(List<MemberInfo> members, JsonElement request, out List<MemberInfo> others)
        {
            MemberInfo? chord = null;
            if (request.ValueKind == JsonValueKind.Object && request.TryGetProperty("chord_element_id", out var chordId) && chordId.TryGetInt64(out long wanted))
            {
                chord = members.FirstOrDefault(m => m.Id == wanted);
                if (chord == null)
                {
                    throw new NodeInspectionException(new ApiError(ErrorCodes.ElementNotFound,
                        "chord_element_id " + wanted + " no está entre los element_ids del nudo.", "chord_element_id",
                        "Incluye el cordón en element_ids o quita chord_element_id para que se elija el más horizontal."));
                }
            }

            chord ??= members.OrderBy(m => m.SlopeDegrees).ThenByDescending(m => m.Start.DistanceTo(m.End)).First();
            others = members.Where(m => !ReferenceEquals(m, chord)).ToList();
            return chord;
        }

        /// <summary>Sistema local del nudo a partir del cordón y del primer miembro.</summary>
        public static NodeFrame ComputeFrame(MemberInfo chord, MemberInfo firstMember)
        {
            try
            {
                return NodeFrame.Compute(chord.Start, chord.End, firstMember.Start, firstMember.End);
            }
            catch (NodeGeometryException error)
            {
                throw new NodeInspectionException(new ApiError(error.Code, error.Message, "node", error.Hint));
            }
        }

        public static object FrameToData(NodeFrame frame) => new
        {
            origin_mm = new[] { UnitConverter.RoundMm(frame.Origin.X), UnitConverter.RoundMm(frame.Origin.Y), UnitConverter.RoundMm(frame.Origin.Z) },
            x_axis = Round(frame.X),
            y_axis = Round(frame.Y),
            z_axis = Round(frame.Z),
            axis_distance_mm = Math.Round(frame.AxisDistanceMm, 2),
        };

        private static double[] Round(Vec3 v) => new[] { Math.Round(v.X, 6), Math.Round(v.Y, 6), Math.Round(v.Z, 6) };
    }
}
