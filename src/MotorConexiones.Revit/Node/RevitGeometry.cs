using Autodesk.Revit.DB;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Revit.Node
{
    /// <summary>Paso entre la geometría de Revit (pies) y la del Core (mm). La conversión numérica vive en <see cref="UnitConverter"/>.</summary>
    public static class RevitGeometry
    {
        public static Vec3 ToMm(XYZ point) =>
            new Vec3(UnitConverter.FeetToMm(point.X), UnitConverter.FeetToMm(point.Y), UnitConverter.FeetToMm(point.Z));

        public static XYZ ToFeet(Vec3 pointMm) =>
            new XYZ(UnitConverter.MmToFeet(pointMm.X), UnitConverter.MmToFeet(pointMm.Y), UnitConverter.MmToFeet(pointMm.Z));

        /// <summary>Vector unitario (sin conversión de unidades).</summary>
        public static XYZ Direction(Vec3 unit) => new XYZ(unit.X, unit.Y, unit.Z);

        /// <summary>Transformación de Revit cuyo origen y ejes son los del sistema local del nudo.</summary>
        public static Transform ToTransform(NodeFrame frame)
        {
            Transform transform = Transform.Identity;
            transform.Origin = ToFeet(frame.Origin);
            transform.BasisX = Direction(frame.X);
            transform.BasisY = Direction(frame.Y);
            transform.BasisZ = Direction(frame.Z);
            return transform;
        }
    }
}
