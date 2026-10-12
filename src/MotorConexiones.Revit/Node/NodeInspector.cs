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

    /// <summary>Nudo resuelto: miembros leídos, cordón, primer miembro y sistema local.</summary>
    public sealed class ResolvedNode
    {
        public ResolvedNode(List<MemberInfo> members, MemberInfo chord, MemberInfo firstMember, NodeFrame frame, bool isSplice = false)
        {
            Members = members;
            Chord = chord;
            FirstMember = firstMember;
            Frame = frame;
            IsSplice = isSplice;
        }

        public List<MemberInfo> Members { get; }
        public MemberInfo Chord { get; }
        public MemberInfo FirstMember { get; }
        public NodeFrame Frame { get; }

        /// <summary>Fase 12: verdadero si el nudo es un empalme (marco colineal con origen en el punto de empalme).</summary>
        public bool IsSplice { get; }

        /// <summary>Fase 12: excentricidad medida (mm entre los cortes extremos de las barras sobre el eje del cordón).</summary>
        public double EccentricityMm => Frame.EccentricityMm;

        public MemberInfo? Find(long elementId) => Members.Find(m => m.Id == elementId);
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

        /// <summary>
        /// Resuelve el nudo de una especificación con una sola regla para validar, previsualizar y crear:
        /// IDs = node.element_ids ∪ chord ∪ members; cordón = chord.element_id (o el más horizontal);
        /// primer miembro = members[0] (sección 7 del encargo: "d es la dirección del primer miembro de members").
        /// </summary>
        public static ResolvedNode ResolveNode(Document document, ConnectionSpec spec)
        {
            var ids = new List<long>();
            void Add(long id)
            {
                if (id > 0 && !ids.Contains(id)) ids.Add(id);
            }
            if (spec.Chord != null) Add(spec.Chord.ElementId);
            if (spec.Members != null) foreach (var member in spec.Members) Add(member.ElementId);
            if (spec.Node?.ElementIds != null) foreach (long id in spec.Node.ElementIds) Add(id);

            if (ids.Count < 2)
            {
                throw new NodeInspectionException(new ApiError(ErrorCodes.NodeNeedsTwoMembers,
                    "La especificación necesita el cordón y al menos un miembro (se encontraron " + ids.Count + " IDs).",
                    "node.element_ids", "Rellena chord.element_id y members[*].element_id con los IDs del nudo."));
            }

            List<MemberInfo> members = ReadMembers(document, ids, "node.element_ids");
            MemberInfo? chord = spec.Chord != null ? members.Find(m => m.Id == spec.Chord.ElementId) : null;
            chord ??= ChooseChord(members, default, out _);

            MemberInfo? first = null;
            if (spec.Members != null && spec.Members.Count > 0)
            {
                first = members.Find(m => m.Id == spec.Members[0].ElementId && m.Id != chord.Id);
            }
            first ??= members.Find(m => m.Id != chord.Id);
            if (first == null)
            {
                throw new NodeInspectionException(new ApiError(ErrorCodes.NodeNeedsTwoMembers,
                    "Hace falta al menos un miembro distinto del cordón.", "members",
                    "Añade las diagonales o montantes que llegan al nudo."));
            }

            // Fase 12: la regla única del marco (SpecFrame): colineal en una placa de extremo, a mitad entre los cortes de todas
            // las barras en un nudo (opción A).
            var others = new List<(Vec3 Start, Vec3 End)>();
            if (spec.Members != null)
            {
                for (int i = 1; i < spec.Members.Count; i++)
                {
                    MemberInfo? other = members.Find(m => m.Id == spec.Members[i].ElementId && m.Id != chord.Id);
                    if (other != null) others.Add((other.Start, other.End));
                }
            }
            bool endPlate = spec.IsType("end_plate");
            NodeFrame frame = ComputeFrame(chord, first, others, endPlate);
            bool splice = endPlate || chord.Direction.Cross(first.Direction).Length < Math.Sin(5.0 * Math.PI / 180.0);
            return new ResolvedNode(members, chord, first, frame, splice);
        }

        /// <summary>Sistema local del nudo a partir del cordón y del primer miembro (la regla de siempre, sin más barras).</summary>
        public static NodeFrame ComputeFrame(MemberInfo chord, MemberInfo firstMember) => ComputeFrame(chord, firstMember, null, false);

        /// <summary>
        /// Fase 12: el marco con todas las barras (<see cref="SpecFrame"/>): colineal si es una placa de extremo o la primera
        /// barra es paralela al cordón; si no, canónico con el origen a mitad entre los cortes.
        /// </summary>
        public static NodeFrame ComputeFrame(MemberInfo chord, MemberInfo firstMember, IEnumerable<(Vec3 Start, Vec3 End)>? others, bool isEndPlate)
        {
            try
            {
                return SpecFrame.Compute(isEndPlate, chord.Start, chord.End, firstMember.Start, firstMember.End, others);
            }
            catch (NodeGeometryException error)
            {
                throw new NodeInspectionException(new ApiError(error.Code, error.Message, "node", error.Hint));
            }
        }

        /// <summary>
        /// Fase 12 (opción A): dónde corta el eje de una barra al eje del cordón (la recta X del marco), medido desde el punto de
        /// trabajo con signo a lo largo de +X; 0 si concurre o si los ejes no se cortan (a más de 5 mm).
        /// </summary>
        public static double AxisOffsetMm(NodeFrame frame, MemberInfo member)
        {
            double s = NodeFrame.ChordCutParameter(frame.Origin, frame.X, member.Start, member.End, out double distance);
            if (double.IsNaN(s) || distance > NodeFrame.MaxAxisDistanceMm) return 0.0;
            return Math.Abs(s) < 0.05 ? 0.0 : s;
        }

        /// <summary>Fase 12: el punto del eje del cordón donde corta el eje de la barra (el punto de trabajo desplazado su desvío).</summary>
        public static Vec3 CutPointOf(NodeFrame frame, MemberInfo member) => frame.Origin + frame.X * AxisOffsetMm(frame, member);

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
