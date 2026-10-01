using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using MotorConexiones.Core;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Fabrication
{
    /// <summary>
    /// Camino B: sólidos por extrusión en elementos <see cref="DirectShape"/> de la categoría Conexiones estructurales
    /// (o Modelos genéricos si Revit no la admite para DirectShape). Sin archivos de familia. Compila entero en la nube.
    /// </summary>
    public sealed class DirectShapeBackend : IFabricationBackend
    {
        public const string ApplicationId = AddinInfo.Name;

        private readonly List<ApiError> _warnings;

        public DirectShapeBackend(List<ApiError> warnings)
        {
            _warnings = warnings;
        }

        public string Name => "directshape";

        public ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, string name)
        {
            if (outlineMm.Count < 3) throw new ArgumentException("El contorno de la placa necesita al menos tres vértices.", nameof(outlineMm));
            if (thicknessMm <= 0) throw new ArgumentException("El espesor de la placa debe ser positivo.", nameof(thicknessMm));

            Transform transform = RevitGeometry.ToTransform(frame);
            double half = UnitConverter.MmToFeet(thicknessMm / 2.0);
            var points = new List<XYZ>(outlineMm.Count);
            foreach (BoltPosition vertex in outlineMm)
            {
                points.Add(transform.OfPoint(new XYZ(UnitConverter.MmToFeet(vertex.X), UnitConverter.MmToFeet(vertex.Y), -half)));
            }

            var curves = new List<Curve>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                curves.Add(Line.CreateBound(points[i], points[(i + 1) % points.Count]));
            }

            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(
                new List<CurveLoop> { CurveLoop.Create(curves) },
                transform.BasisZ,
                UnitConverter.MmToFeet(thicknessMm));
            return CreateShape(document, solid, name, "plate");
        }

        public IList<ElementId> CreateBoltGroup(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> positionsMm, double diameterMm, double lengthMm, string name)
        {
            if (diameterMm <= 0) throw new ArgumentException("El diámetro del perno debe ser positivo.", nameof(diameterMm));
            if (lengthMm <= 0) throw new ArgumentException("La longitud del perno debe ser positiva.", nameof(lengthMm));

            Transform transform = RevitGeometry.ToTransform(frame);
            double radius = UnitConverter.MmToFeet(diameterMm / 2.0);
            double half = UnitConverter.MmToFeet(lengthMm / 2.0);
            var ids = new List<ElementId>(positionsMm.Count);
            int index = 1;
            foreach (BoltPosition position in positionsMm)
            {
                XYZ center = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(position.X), UnitConverter.MmToFeet(position.Y), -half));
                var loop = CurveLoop.Create(new List<Curve>
                {
                    Arc.Create(center, radius, 0.0, Math.PI, transform.BasisX, transform.BasisY),
                    Arc.Create(center, radius, Math.PI, 2.0 * Math.PI, transform.BasisX, transform.BasisY),
                });
                Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(
                    new List<CurveLoop> { loop }, transform.BasisZ, UnitConverter.MmToFeet(lengthMm));
                ids.Add(CreateShape(document, solid, name + " " + index, "bolt"));
                index++;
            }
            return ids;
        }

        private ElementId CreateShape(Document document, Solid solid, string name, string kind)
        {
            ElementId categoryId = new ElementId(BuiltInCategory.OST_StructConnections);
            if (!DirectShape.IsValidCategoryId(categoryId, document))
            {
                categoryId = new ElementId(BuiltInCategory.OST_GenericModel);
                _warnings.Add(new ApiError(ErrorCodes.CategoryFallback,
                    "Revit no admite DirectShape en la categoría Conexiones estructurales; se usa Modelos genéricos.",
                    hint: "Es solo estético en la prueba técnica; la Fase 3 decidirá la categoría definitiva."));
            }

            DirectShape shape = DirectShape.CreateElement(document, categoryId);
            shape.ApplicationId = ApplicationId;
            shape.ApplicationDataId = kind + ":" + Guid.NewGuid().ToString("N");
            shape.SetShape(new List<GeometryObject> { solid });
            shape.Name = name;
            return shape.Id;
        }
    }
}
