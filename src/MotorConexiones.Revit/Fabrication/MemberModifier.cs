using System;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Units;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Aplica y restaura el retiro de extremo (<c>end_setback_mm</c>) de una barra con los parámetros nativos
    /// "Start Extension" / "End Extension" de Revit. El retiro del contrato se mide desde el punto de trabajo del nudo
    /// hasta el extremo físico de la barra; la extensión de Revit se mide desde el extremo de la línea de ubicación
    /// (positiva = la barra se alarga más allá de su línea). Por eso la extensión nueva es
    /// <c>distancia(extremo de la línea, punto de trabajo) − retiro</c>, y no "menos el retiro" a secas.
    /// </summary>
    public static class MemberModifier
    {
        /// <summary>Distancia, medida a lo largo del eje, desde el extremo de la línea de ubicación más cercano al nudo hasta el punto de trabajo.</summary>
        public static double CurrentEndDistanceMm(FamilyInstance member, Vec3 workPointMm, out int endIndex)
        {
            endIndex = 0;
            if (member.Location is not LocationCurve location || location.Curve == null) return 0.0;
            Vec3 p0 = RevitGeometry.ToMm(location.Curve.GetEndPoint(0));
            Vec3 p1 = RevitGeometry.ToMm(location.Curve.GetEndPoint(1));
            endIndex = p0.DistanceTo(workPointMm) <= p1.DistanceTo(workPointMm) ? 0 : 1;
            Vec3 nearEnd = endIndex == 0 ? p0 : p1;
            Vec3 farEnd = endIndex == 0 ? p1 : p0;
            Vec3 axis = (nearEnd - farEnd).Normalized();
            // Proyección sobre el eje: positiva si el punto de trabajo queda más allá del extremo.
            return (workPointMm - nearEnd).Dot(axis);
        }

        /// <summary>Extensión (mm) que deja el extremo físico a <paramref name="setbackMm"/> del punto de trabajo.</summary>
        public static double TargetExtensionMm(double currentEndDistanceMm, double setbackMm) => currentEndDistanceMm - setbackMm;

        public static ModifiedMemberRecord? ApplySetback(Document document, FamilyInstance member, Vec3 workPointMm, double setbackMm)
        {
            if (member == null || setbackMm <= 0.0) return null;
            if (member.Location is not LocationCurve location || location.Curve == null) return null;

            double currentEndDistanceMm = CurrentEndDistanceMm(member, workPointMm, out int endIndex);

            BuiltInParameter bip = endIndex == 0 ? BuiltInParameter.START_EXTENSION : BuiltInParameter.END_EXTENSION;
            Parameter? parameter = member.get_Parameter(bip);
            if (parameter == null || parameter.IsReadOnly)
            {
                return null;
            }

            double originalValueFeet = parameter.AsDouble();
            double newValueFeet = UnitConverter.MmToFeet(TargetExtensionMm(currentEndDistanceMm, setbackMm));
            parameter.Set(newValueFeet);

            return new ModifiedMemberRecord
            {
                ElementId = member.Id.Value,
                EndIndex = endIndex,
                ParameterName = bip.ToString(),
                OriginalValueFeet = originalValueFeet,
                AppliedSetbackMm = setbackMm
            };
        }

        public static bool RestoreSetback(Document document, ModifiedMemberRecord record)
        {
            if (document == null || record == null) return false;

            Element? element = document.GetElement(new ElementId(record.ElementId));
            if (element is not FamilyInstance member) return false;

            if (Enum.TryParse<BuiltInParameter>(record.ParameterName, out var bip))
            {
                Parameter? parameter = member.get_Parameter(bip);
                if (parameter != null && !parameter.IsReadOnly)
                {
                    parameter.Set(record.OriginalValueFeet);
                    return true;
                }
            }

            return false;
        }
    }
}
