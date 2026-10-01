using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit.Node
{
    /// <summary>
    /// Implementación de <see cref="IModelFacts"/> conectada a la API real de Revit.
    /// Consulta el documento activo para validar existencia de barras, tipos cargados,
    /// geometría en mm, ángulos en el plano y conexiones existentes.
    /// </summary>
    public sealed class RevitModelFacts : IModelFacts
    {
        private static readonly ElementId FramingCategory = new ElementId(BuiltInCategory.OST_StructuralFraming);

        private readonly Document _document;
        private readonly NodeFrame? _nodeFrame;
        private readonly Vec3? _workPointMm;
        private List<string>? _cachedProfileNames;

        public RevitModelFacts(Document document, NodeFrame? nodeFrame = null, Vec3? workPointMm = null)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _nodeFrame = nodeFrame;
            _workPointMm = workPointMm ?? nodeFrame?.Origin;
        }

        public string ProjectUniqueId
        {
            get
            {
                try
                {
                    return _document.ProjectInformation?.UniqueId ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public bool ElementExists(long elementId)
        {
            try
            {
                return _document.GetElement(new ElementId(elementId)) != null;
            }
            catch
            {
                return false;
            }
        }

        public bool IsStructuralMember(long elementId)
        {
            try
            {
                Element? element = _document.GetElement(new ElementId(elementId));
                if (element is not FamilyInstance fi) return false;
                return fi.Category != null && fi.Category.Id.Value == FramingCategory.Value;
            }
            catch
            {
                return false;
            }
        }

        public MemberModelFacts? GetMemberFacts(long elementId)
        {
            try
            {
                Element? element = _document.GetElement(new ElementId(elementId));
                if (element is not FamilyInstance fi) return null;
                if (fi.Category == null || fi.Category.Id.Value != FramingCategory.Value) return null;

                FamilySymbol symbol = fi.Symbol;
                string familyName = symbol.FamilyName ?? string.Empty;
                string typeName = symbol.Name ?? string.Empty;

                double widthMm = 0.0;
                double heightMm = 0.0;
                double thicknessMm = 0.0;

                // Intentar leer parámetros dimensionales de la sección
                widthMm = ReadLengthParamMm(symbol, BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, "b", "Width", "B", "Ancho");
                heightMm = ReadLengthParamMm(symbol, BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, "d", "h", "H", "Height", "Alto");
                thicknessMm = ReadLengthParamMm(symbol, BuiltInParameter.INVALID, "t", "Thickness", "Nominal Wall Thickness", "Wall Thickness", "Espesor", "t_nom");

                // Si no se encontraron parámetros numéricos en la familia, inferir de la designación AISC
                if (widthMm <= 0.0 || heightMm <= 0.0)
                {
                    InferDimensionsFromTypeName(typeName, ref widthMm, ref heightMm, ref thicknessMm);
                }

                Vec3 startMm = Vec3.Zero;
                Vec3 endMm = Vec3.Zero;
                bool connectsToNode = true;
                double angleInPlaneDeg = 0.0;

                if (fi.Location is LocationCurve location && location.Curve != null)
                {
                    startMm = RevitGeometry.ToMm(location.Curve.GetEndPoint(0));
                    endMm = RevitGeometry.ToMm(location.Curve.GetEndPoint(1));

                    Vec3 dir = (endMm - startMm).Normalized();

                    // Calcular ángulo en el plano respecto al cordón o a la horizontal
                    if (_nodeFrame != null)
                    {
                        // Determinar cuál extremo llega al nudo
                        double dStart = _workPointMm.HasValue ? startMm.DistanceTo(_workPointMm.Value) : 0.0;
                        double dEnd = _workPointMm.HasValue ? endMm.DistanceTo(_workPointMm.Value) : 0.0;
                        Vec3 outward = dStart <= dEnd ? dir : dir * -1.0;

                        double lx = outward.Dot(_nodeFrame.X);
                        double ly = outward.Dot(_nodeFrame.Y);
                        angleInPlaneDeg = UnitConverter.RadiansToDegrees(Math.Atan2(Math.Abs(ly), lx));

                        // Verificar si el eje llega físicamente al nudo (tolerancia de 5 mm de la regla 7)
                        if (_workPointMm.HasValue)
                        {
                            double axisDist = DistanceFromPointToLine(_workPointMm.Value, startMm, endMm);
                            connectsToNode = axisDist <= 5.0;
                        }
                    }
                    else
                    {
                        // Si no hay marco del nudo, usar pendiente respecto a la horizontal
                        angleInPlaneDeg = UnitConverter.RadiansToDegrees(Math.Asin(Math.Min(1.0, Math.Abs(dir.Z))));
                    }
                }

                return new MemberModelFacts
                {
                    ElementId = elementId,
                    UniqueId = fi.UniqueId,
                    FamilyName = familyName,
                    TypeName = typeName,
                    WidthMm = Math.Round(widthMm, 2),
                    HeightMm = Math.Round(heightMm, 2),
                    ThicknessMm = Math.Round(thicknessMm, 2),
                    CurveStartMm = startMm,
                    CurveEndMm = endMm,
                    AngleInPlaneDeg = Math.Round(angleInPlaneDeg, 1),
                    ConnectsToNode = connectsToNode
                };
            }
            catch
            {
                return null;
            }
        }

        public IReadOnlyList<string> GetAvailableProfileNames()
        {
            if (_cachedProfileNames != null) return _cachedProfileNames;

            try
            {
                var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var collector = new FilteredElementCollector(_document)
                    .OfCategory(BuiltInCategory.OST_StructuralFraming)
                    .WhereElementIsElementType();

                foreach (ElementType element in collector)
                {
                    if (!string.IsNullOrWhiteSpace(element.Name))
                    {
                        names.Add(element.Name);
                    }
                }
                _cachedProfileNames = names.OrderBy(n => n, StringComparer.Ordinal).ToList();
            }
            catch
            {
                _cachedProfileNames = new List<string>();
            }

            return _cachedProfileNames;
        }

        public bool CheckClashWithForeignMember(long foreignMemberId, Vec3 startMm, Vec3 endMm)
        {
            try
            {
                Element? elem = _document.GetElement(new ElementId(foreignMemberId));
                if (elem == null) return false;

                if (elem.Location is LocationCurve loc && loc.Curve != null)
                {
                    Vec3 foreignStart = RevitGeometry.ToMm(loc.Curve.GetEndPoint(0));
                    Vec3 foreignEnd = RevitGeometry.ToMm(loc.Curve.GetEndPoint(1));
                    double dist = SegmentDistanceMm(startMm, endMm, foreignStart, foreignEnd);
                    return dist < 1.0; // Choque si están a menos de 1 mm
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static double ReadLengthParamMm(Element element, BuiltInParameter bip, params string[] paramNames)
        {
            if (bip != BuiltInParameter.INVALID)
            {
                Parameter? param = element.get_Parameter(bip);
                if (param != null && param.HasValue)
                {
                    return UnitConverter.FeetToMm(param.AsDouble());
                }
            }

            foreach (string name in paramNames)
            {
                Parameter? param = element.LookupParameter(name);
                if (param != null && param.HasValue && param.StorageType == StorageType.Double)
                {
                    return UnitConverter.FeetToMm(param.AsDouble());
                }
            }

            return 0.0;
        }

        private static bool TryParseAiscInchPart(string part, out double mm)
        {
            mm = 0.0;
            if (string.IsNullOrWhiteSpace(part)) return false;
            part = part.Trim();
            if (LabelParser.TryParseLabelToMm(part + "\"", out mm))
            {
                return true;
            }
            if (LabelParser.TryParseLabelToMm(part, out mm))
            {
                return true;
            }
            return false;
        }

        private static void InferDimensionsFromTypeName(string typeName, ref double widthMm, ref double heightMm, ref double thicknessMm)
        {
            // Analizar patrones tipo HSS3X3X1/4 o HSS2-1/2X2-1/2X3/16 o HSS 64x64
            string s = typeName.ToUpperInvariant();
            int hssIdx = s.IndexOf("HSS", StringComparison.Ordinal);
            if (hssIdx >= 0)
            {
                string rem = s.Substring(hssIdx + 3).Trim();
                // Si contiene sufijo métrico tipo " 64x64"
                int spaceIdx = rem.IndexOf(' ');
                if (spaceIdx > 0 && spaceIdx + 1 < rem.Length)
                {
                    string metricPart = rem.Substring(spaceIdx + 1).Trim();
                    string[] parts = metricPart.Split('X', 'x');
                    if (parts.Length >= 2 && double.TryParse(parts[0], out double mw) && double.TryParse(parts[1], out double mh))
                    {
                        if (widthMm <= 0.0) widthMm = mw;
                        if (heightMm <= 0.0) heightMm = mh;
                    }
                }

                // Si no hay métrico, intentar parsear fracciones AISC (ej. 2-1/2 o 3)
                string imperialPart = spaceIdx > 0 ? rem.Substring(0, spaceIdx) : rem;
                string[] impParts = imperialPart.Split('X', 'x');
                if (impParts.Length >= 2)
                {
                    if (widthMm <= 0.0 && TryParseAiscInchPart(impParts[0], out double iw))
                    {
                        widthMm = iw;
                    }
                    if (heightMm <= 0.0 && TryParseAiscInchPart(impParts[1], out double ih))
                    {
                        heightMm = ih;
                    }
                    if (impParts.Length >= 3 && thicknessMm <= 0.0 && TryParseAiscInchPart(impParts[2], out double it))
                    {
                        thicknessMm = it;
                    }
                }
            }
        }

        private static double DistanceFromPointToLine(Vec3 pt, Vec3 lineStart, Vec3 lineEnd)
        {
            Vec3 v = lineEnd - lineStart;
            double len = v.Length;
            if (len < 1e-9) return pt.DistanceTo(lineStart);
            Vec3 u = v.Normalized();
            Vec3 w = pt - lineStart;
            double proj = w.Dot(u);
            Vec3 closest = lineStart + u * Math.Max(0.0, Math.Min(len, proj));
            return pt.DistanceTo(closest);
        }

        private static double SegmentDistanceMm(Vec3 p1, Vec3 p2, Vec3 q1, Vec3 q2)
        {
            // Distancia mínima aproximada entre dos segmentos 3D
            Vec3 u = p2 - p1;
            Vec3 v = q2 - q1;
            Vec3 w = p1 - q1;
            double a = u.Dot(u);
            double b = u.Dot(v);
            double c = v.Dot(v);
            double d = u.Dot(w);
            double e = v.Dot(w);
            double D = a * c - b * b;
            double sc, tc;

            if (D < 1e-8)
            {
                sc = 0.0;
                tc = b > c ? d / b : e / c;
            }
            else
            {
                sc = (b * e - c * d) / D;
                tc = (a * e - b * d) / D;
            }

            sc = Math.Max(0.0, Math.Min(1.0, sc));
            tc = Math.Max(0.0, Math.Min(1.0, tc));

            Vec3 dP = w + (u * sc) - (v * tc);
            return dP.Length;
        }
    }
}
