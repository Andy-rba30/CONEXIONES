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

                // Ronda 12c (sondeo 27): el tipo guarda el ANCHO nominal ("b", el 8" del HSS12X8) y el CANTO nominal ("Ht", "d", el
                // 12"); el HSS12X8X1/2 del Hangar solo trae "Ht" = 304,8, "b" = 203,2 y "t" = 11,81 con nombre corto y ningún
                // STRUCTURAL_SECTION_COMMON_* (hasta la 0.12.1 faltaba "Ht" en la cadena y el canto se infería del "8": 203 × 203).
                double nominalWidthMm = ReadLengthParamMm(symbol, BuiltInParameter.STRUCTURAL_SECTION_COMMON_WIDTH, "b", "B", "Width", "Ancho");
                double nominalDepthMm = ReadLengthParamMm(symbol, BuiltInParameter.STRUCTURAL_SECTION_COMMON_HEIGHT, "d", "h", "H", "Ht", "Height", "Depth", "Alto");
                double thicknessMm = ReadLengthParamMm(symbol, BuiltInParameter.INVALID, "t", "Thickness", "Nominal Wall Thickness", "Wall Thickness", "Espesor", "t_nom");

                // Si la familia no trae los parámetros, la designación AISC: canto × ancho × pared (HSS12X8X1/2 = 304,8 × 203,2 × 12,7).
                if ((nominalWidthMm <= 0.0 || nominalDepthMm <= 0.0) && HssDesignation.TryParseMm(typeName, out double d, out double w, out double t))
                {
                    if (nominalDepthMm <= 0.0) nominalDepthMm = d;
                    if (nominalWidthMm <= 0.0) nominalWidthMm = w;
                    if (thicknessMm <= 0.0) thicknessMm = t;
                }

                // El giro de la sección del ejemplar (Cross-Section Rotation = STRUCTURAL_BEND_DIR_ANGLE, radianes en la API): con 90°
                // el canto nominal queda perpendicular al plano de la cercha y el ancho en el plano (el cordón superior del Hangar).
                // WidthMm es el lado EN el plano y HeightMm el PERPENDICULAR (lo que el Core, el simulador y las pruebas ya suponían).
                double rotationDeg = ReadAngleDeg(fi, BuiltInParameter.STRUCTURAL_BEND_DIR_ANGLE);
                var (widthMm, heightMm) = SectionOrientation.Oriented(nominalDepthMm, nominalWidthMm, rotationDeg);

                Vec3 startMm = Vec3.Zero;
                Vec3 endMm = Vec3.Zero;
                bool connectsToNode = true;
                double angleInPlaneDeg = 0.0;
                double? axisOffsetMm = null, axisDistanceMm = null, endGapMm = null;

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
                        // Fase 7: ángulo con signo desde +X del marco canónico, en [−180°, 180°).
                        angleInPlaneDeg = NodeFrame.SignedAngleDeg(lx, ly);

                        // Llega al nudo: regla única de Core (NodeReach), probada con las coordenadas reales.
                        if (_workPointMm.HasValue)
                        {
                            connectsToNode = NodeReach.MemberReachesNode(startMm, endMm, _workPointMm.Value);
                            // Fase 12 (opción A): la distancia del eje al punto de trabajo, el hueco del extremo y dónde corta el eje de
                            // la barra al eje del cordón (la recta X del marco), con signo a lo largo de +X. El validador los usa para
                            // rebajar MEMBER_NOT_AT_NODE a MEMBER_ECCENTRIC hasta medio canto del cordón.
                            axisDistanceMm = Math.Round(NodeReach.AxisDistanceMm(startMm, endMm, _workPointMm.Value), 2);
                            endGapMm = Math.Round(NodeReach.EndGapMm(startMm, endMm, _workPointMm.Value), 2);
                            double s = NodeFrame.ChordCutParameter(_workPointMm.Value, _nodeFrame.X, startMm, endMm, out double axesDistance);
                            axisOffsetMm = !double.IsNaN(s) && axesDistance <= NodeFrame.MaxAxisDistanceMm ? Math.Round(Math.Abs(s) < 0.05 ? 0.0 : s, 2) : 0.0;
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
                    SectionRotationDeg = Math.Round(rotationDeg, 1),
                    CurveStartMm = startMm,
                    CurveEndMm = endMm,
                    AngleInPlaneDeg = Math.Round(angleInPlaneDeg, 1),
                    ConnectsToNode = connectsToNode,
                    AxisOffsetMm = axisOffsetMm,
                    AxisDistanceMm = axisDistanceMm,
                    EndGapMm = endGapMm,
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

        /// <summary>Un parámetro de ángulo del ejemplar en grados (la API lo guarda en radianes); 0 si no existe.</summary>
        private static double ReadAngleDeg(Element element, BuiltInParameter bip)
        {
            Parameter? param = element.get_Parameter(bip);
            if (param == null || !param.HasValue || param.StorageType != StorageType.Double) return 0.0;
            return UnitConverter.RadiansToDegrees(param.AsDouble());
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
