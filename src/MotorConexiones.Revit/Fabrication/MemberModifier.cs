using System;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Units;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Aplica y restaura recortes y retiros de extremo (<c>end_setback_mm</c>) en barras
    /// de armazón estructural mediante los parámetros de extensión nativos de Revit.
    /// </summary>
    public static class MemberModifier
    {
        public static ModifiedMemberRecord? ApplySetback(Document document, FamilyInstance member, Vec3 workPointMm, double setbackMm)
        {
            if (member == null || setbackMm <= 0.0) return null;

            if (member.Location is not LocationCurve loc || loc.Curve == null) return null;

            Vec3 p0 = RevitGeometry.ToMm(loc.Curve.GetEndPoint(0));
            Vec3 p1 = RevitGeometry.ToMm(loc.Curve.GetEndPoint(1));

            // Extremo que concurre al nudo de trabajo
            int endNodeIndex = p0.DistanceTo(workPointMm) <= p1.DistanceTo(workPointMm) ? 0 : 1;

            BuiltInParameter bip = endNodeIndex == 0 ? BuiltInParameter.START_EXTENSION : BuiltInParameter.END_EXTENSION;
            Parameter? param = member.get_Parameter(bip);
            if (param == null || param.IsReadOnly)
            {
                bip = endNodeIndex == 0 ? BuiltInParameter.START_JOIN_CUTBACK : BuiltInParameter.END_JOIN_CUTBACK;
                param = member.get_Parameter(bip);
            }

            if (param == null || param.IsReadOnly)
            {
                return null;
            }

            double originalValueFeet = param.AsDouble();
            double setbackFeet = UnitConverter.MmToFeet(setbackMm);

            // Acortar la barra aplicando valor negativo de extensión
            double newValueFeet = originalValueFeet - setbackFeet;
            param.Set(newValueFeet);

            return new ModifiedMemberRecord
            {
                ElementId = member.Id.Value,
                EndIndex = endNodeIndex,
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
                Parameter? param = member.get_Parameter(bip);
                if (param != null && !param.IsReadOnly)
                {
                    param.Set(record.OriginalValueFeet);
                    return true;
                }
            }

            return false;
        }
    }
}
