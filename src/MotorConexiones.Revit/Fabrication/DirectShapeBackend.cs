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
    /// (o Modelos genéricos si Revit no la admite para DirectShape). Sin dependencias nativas externas.
    /// Funciona en cualquier versión y compila en todos los entornos.
    /// </summary>
    public sealed class DirectShapeBackend : IFabricationBackend
    {
        public const string ApplicationId = AddinInfo.Name;

        private readonly List<ApiError> _warnings;

        public DirectShapeBackend(List<ApiError> warnings)
        {
            _warnings = warnings ?? new List<ApiError>();
        }

        public string Name => "directshape";

        public bool IsAvailable => true;

        /// <summary>El camino B no necesita sesión: la Transaction de Revit de la operación basta.</summary>
        public IFabricationSession BeginSession(Document document, string name) => new NoSession();

        /// <summary>
        /// Un DirectShape por perno con tres sólidos: cabeza apoyada en la cara inferior del paquete (menor Z), vástago de
        /// la longitud del perno desde esa cara hacia +Z y tuerca sobre la cara superior. Así la reserva enseña lo mismo
        /// que pide el contrato: el perno atraviesa cartela + placa (ronda 6b).
        /// </summary>
        public IList<ElementId> CreateBoltPattern(Document document, NodeFrame frame, BoltGrid grid, double diameterMm, BoltStack stack, string name)
        {
            if (stack == null) throw new ArgumentNullException(nameof(stack));
            if (diameterMm <= 0) throw new ArgumentException("El diámetro del perno debe ser positivo.", nameof(diameterMm));
            if (stack.BoltLengthMm <= 0) throw new ArgumentException("La longitud del perno debe ser positiva.", nameof(stack));

            Transform transform = RevitGeometry.ToTransform(frame);
            double radius = UnitConverter.MmToFeet(diameterMm / 2.0);
            double headRadius = UnitConverter.MmToFeet(diameterMm * 0.8);
            double headHeight = UnitConverter.MmToFeet(diameterMm * 0.625);
            double nutHeight = UnitConverter.MmToFeet(diameterMm * 0.875);
            double shankLength = UnitConverter.MmToFeet(stack.BoltLengthMm);
            double zStart = UnitConverter.MmToFeet(stack.StackMinMm);
            double zTop = UnitConverter.MmToFeet(stack.StackMaxMm);
            var ids = new List<ElementId>(grid.Positions.Count);
            int index = 1;
            foreach (BoltPosition position in grid.Positions)
            {
                double x = UnitConverter.MmToFeet(position.X);
                double y = UnitConverter.MmToFeet(position.Y);
                var solids = new List<GeometryObject>
                {
                    Cylinder(transform, x, y, zStart, radius, shankLength),
                    Cylinder(transform, x, y, zStart - headHeight, headRadius, headHeight),
                };
                if (zTop + nutHeight <= zStart + shankLength + 1e-9)
                {
                    solids.Add(Cylinder(transform, x, y, zTop, headRadius, nutHeight));
                }
                ids.Add(CreateShape(document, solids, name + " " + index, "bolt"));
                index++;
            }
            return ids;
        }

        private static Solid Cylinder(Transform transform, double xFeet, double yFeet, double zFeet, double radiusFeet, double heightFeet)
        {
            XYZ center = transform.OfPoint(new XYZ(xFeet, yFeet, zFeet));
            var loop = CurveLoop.Create(new List<Curve>
            {
                Arc.Create(center, radiusFeet, 0.0, Math.PI, transform.BasisX, transform.BasisY),
                Arc.Create(center, radiusFeet, Math.PI, 2.0 * Math.PI, transform.BasisX, transform.BasisY),
            });
            return GeometryCreationUtilities.CreateExtrusionGeometry(new List<CurveLoop> { loop }, transform.BasisZ, heightFeet);
        }

        public void DeleteElements(Document document, ICollection<ElementId> elementIds)
        {
            if (elementIds.Count > 0) document.Delete(elementIds);
        }

        private sealed class NoSession : IFabricationSession
        {
            public IReadOnlyList<ElementId> Complete() => new List<ElementId>();
            public void Dispose() { }
        }

        public ElementId CreatePlate(Document document, NodeFrame frame, IReadOnlyList<BoltPosition> outlineMm, double thicknessMm, double offsetMm, string name)
        {
            if (outlineMm.Count < 3) throw new ArgumentException("El contorno de la placa necesita al menos tres vértices.", nameof(outlineMm));
            if (thicknessMm <= 0) throw new ArgumentException("El espesor de la placa debe ser positivo.", nameof(thicknessMm));

            Transform transform = RevitGeometry.ToTransform(frame);
            // Cara inferior de la placa: plano medio desplazado offsetMm menos medio espesor.
            double bottom = UnitConverter.MmToFeet(offsetMm - thicknessMm / 2.0);
            var points = new List<XYZ>(outlineMm.Count);
            foreach (BoltPosition vertex in outlineMm)
            {
                points.Add(transform.OfPoint(new XYZ(UnitConverter.MmToFeet(vertex.X), UnitConverter.MmToFeet(vertex.Y), bottom)));
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

        public IList<ElementId> CreateWelds(Document document, NodeFrame frame, IReadOnlyList<WeldLine2D> weldsMm, string name)
        {
            var ids = new List<ElementId>();
            if (weldsMm == null || weldsMm.Count == 0) return ids;

            Transform transform = RevitGeometry.ToTransform(frame);
            int index = 1;

            foreach (var weld in weldsMm)
            {
                double dist = Math.Sqrt(Math.Pow(weld.End.X - weld.Start.X, 2) + Math.Pow(weld.End.Y - weld.Start.Y, 2));
                if (dist < 1.0) continue;

                XYZ pStart = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(weld.Start.X), UnitConverter.MmToFeet(weld.Start.Y), 0.0));
                XYZ pEnd = transform.OfPoint(new XYZ(UnitConverter.MmToFeet(weld.End.X), UnitConverter.MmToFeet(weld.End.Y), 0.0));
                XYZ dir = (pEnd - pStart).Normalize();

                double radius = UnitConverter.MmToFeet(Math.Max(2.0, weld.SizeMm / 2.0));
                double lengthFeet = UnitConverter.MmToFeet(dist);

                // Ejes ortogonales para la sección del cordón
                XYZ normal = transform.BasisZ;
                XYZ cross = dir.CrossProduct(normal).Normalize();

                try
                {
                    var loop = CurveLoop.Create(new List<Curve>
                    {
                        Arc.Create(pStart, radius, 0.0, Math.PI, normal, cross),
                        Arc.Create(pStart, radius, Math.PI, 2.0 * Math.PI, normal, cross)
                    });

                    Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(
                        new List<CurveLoop> { loop }, dir, lengthFeet);

                    ids.Add(CreateShape(document, solid, name + " soldadura " + index, "weld"));
                    index++;
                }
                catch
                {
                    // Si falla la extrusión de un cordón menor, se continúa sin bloquear la creación
                }
            }

            return ids;
        }

        private ElementId CreateShape(Document document, Solid solid, string name, string kind)
        {
            return CreateShape(document, new List<GeometryObject> { solid }, name, kind);
        }

        private ElementId CreateShape(Document document, IList<GeometryObject> solids, string name, string kind)
        {
            ElementId categoryId = new ElementId(BuiltInCategory.OST_StructConnections);
            if (!DirectShape.IsValidCategoryId(categoryId, document))
            {
                categoryId = new ElementId(BuiltInCategory.OST_GenericModel);
                _warnings.Add(new ApiError(ErrorCodes.CategoryFallback,
                    "Revit no admite DirectShape en la categoría Conexiones estructurales; se usa Modelos genéricos.",
                    hint: "Los elementos se crearon correctamente bajo la categoría Modelos genéricos."));
            }

            DirectShape shape = DirectShape.CreateElement(document, categoryId);
            shape.ApplicationId = ApplicationId;
            shape.ApplicationDataId = kind + ":" + Guid.NewGuid().ToString("N");
            shape.SetShape(solids);
            shape.Name = name;
            return shape.Id;
        }
    }
}
