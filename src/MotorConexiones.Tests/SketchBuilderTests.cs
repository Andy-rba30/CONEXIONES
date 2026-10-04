using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry2D;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Types;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Croquis 2D del Detalle D con el nudo del Hangar simulado (<see cref="FakeModelFacts"/>). Sistema local real de ese
    /// nudo: X = +X global, Y = -Z global (cercha vertical en el plano XZ); por eso la diagonal superior 1249630 sale hacia
    /// (0,707; -0,707), el montante hacia (0; -1) y la diagonal inferior empernada 1249636 hacia (-0,707; 0,707).
    /// </summary>
    [Collection("ConnectionTypeRegistry")] // comparte el registro estático con ConnectionTypeRegistryTests: sin paralelismo
    public class SketchBuilderTests
    {
        private const double Tol = 1e-6;

        private static (SketchModel model, ConnectionSpec spec, SketchNodeInput node) BuildDetalleD()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            var node = SketchNodeInput.FromModelFacts(spec, new FakeModelFacts());
            return (SketchBuilder.Build(spec, node), spec, node);
        }

        [Fact]
        public void NodeInput_FromModelFacts_UsesRealDirectionsAndWidths()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            var node = SketchNodeInput.FromModelFacts(spec, new FakeModelFacts());

            Assert.False(node.IsSchematic);
            Assert.Equal(76.2, node.ChordWidthMm, 3);
            Assert.Equal(3, node.Members.Count);
            Assert.All(node.Members, m => Assert.True(m.FromModel));
            Assert.All(node.Members, m => Assert.Equal(63.5, m.WidthMm, 3));

            var upper = node.Find(1249630)!;
            Assert.Equal(Math.Sqrt(0.5), upper.Ux, 6);
            Assert.Equal(-Math.Sqrt(0.5), upper.Uy, 6);
            Assert.Equal(45.0, upper.AngleToChordDeg, 3);

            var vertical = node.Find(1249631)!;
            Assert.Equal(0.0, vertical.Ux, 6);
            Assert.Equal(-1.0, vertical.Uy, 6);
            Assert.Equal(90.0, vertical.AngleToChordDeg, 3);

            var lower = node.Find(1249636)!;
            Assert.Equal(-Math.Sqrt(0.5), lower.Ux, 6);
            Assert.Equal(Math.Sqrt(0.5), lower.Uy, 6);
            Assert.Equal(45.0, lower.AngleToChordDeg, 3);

            // En este nudo el eje Y local apunta hacia abajo en el modelo: el croquis lo avisa.
            Assert.Equal(-1.0, node.LocalYGlobalZ, 6);
            Assert.Contains(node.Notes, n => n.Contains("girado 180°"));
        }

        [Fact]
        public void Build_DetalleD_HasExpectedPieces()
        {
            var (model, _, _) = BuildDetalleD();

            Assert.False(model.IsSchematic);
            Assert.Equal(1, model.Polygons.Count(p => p.Kind == SketchPolygonKind.Gusset));
            Assert.Equal(1, model.Polygons.Count(p => p.Kind == SketchPolygonKind.KnifePlate));
            Assert.Equal(4, model.Polygons.Count(p => p.Kind == SketchPolygonKind.Member)); // cordón + 3 barras
            Assert.Equal(4, model.Circles.Count(c => c.Kind == SketchCircleKind.Bolt));
            Assert.Equal(6, model.Lines.Count(l => l.Kind == SketchLineKind.Weld)); // 2 por barra, como conn_preview
            Assert.Equal(4, model.Lines.Count(l => l.Kind == SketchLineKind.Axis));
            Assert.Equal(2, model.Lines.Count(l => l.Kind == SketchLineKind.WorkPoint));
            Assert.Equal(16, model.Dimensions.Count);
            Assert.Contains(model.Labels, l => l.Text == "PT");
        }

        [Fact]
        public void Build_DetalleD_DimensionsCarryTheContractValues()
        {
            var (model, _, _) = BuildDetalleD();
            var values = model.Dimensions.Select(d => Math.Round(d.ValueMm, 3)).ToList();

            foreach (double expected in new[] { 565.0, 530.0, 180.0, 60.0, 260.0, 150.0, 170.0, 140.0, 80.0, 40.0 })
            {
                Assert.Contains(expected, values);
            }

            var width = model.Dimensions.Single(d => d.Path == "gusset.width_mm");
            Assert.Equal(565.0, width.ValueMm, 6);
            Assert.Equal("565,0", width.Text);
            var height = model.Dimensions.Single(d => d.Path == "gusset.height_mm");
            Assert.Equal(530.0, height.ValueMm, 6);

            Assert.Equal(2, model.Dimensions.Count(d => d.Path != null && d.Path.EndsWith(".attachment.slot_length_mm") && Math.Abs(d.ValueMm - 150.0) < Tol));
            Assert.Equal(60.0, model.Dimensions.Single(d => d.Path == "members[2].attachment.bolts.spacing_mm" && d.Text == "60,0" && Math.Abs(d.OffsetMm - (70.0 + GussetNodeSketch.DimensionGapMm)) < Tol).ValueMm, 6);
            Assert.Equal(2, model.Dimensions.Count(d => d.Path == "members[2].attachment.bolts.edge_mm"));
            Assert.All(model.Dimensions.Where(d => d.Path == "members[2].attachment.bolts.edge_mm"), d => Assert.Equal(40.0, d.ValueMm, 6));
            Assert.Equal(40.0, model.Dimensions.Single(d => d.Path == "members[2].attachment.bolts.first_row_from_plate_end_mm").ValueMm, 6);
            Assert.Equal(80.0, model.Dimensions.Single(d => d.Path == "members[2].attachment.plate.insertion_mm").ValueMm, 6);

            // Toda cota mide de verdad la distancia entre sus dos puntos.
            Assert.All(model.Dimensions, d => Assert.Equal(d.ValueMm, d.MeasuredLengthMm, 6));
        }

        [Fact]
        public void Build_DetalleD_SetbacksEndWhereTheMembersStart()
        {
            var (model, spec, node) = BuildDetalleD();

            for (int i = 0; i < spec.Members.Count; i++)
            {
                MemberSpec member = spec.Members[i];
                SketchMemberInput input = node.Find(member.ElementId)!;
                double setback = member.EndSetbackMm!.Value;

                var dimension = model.Dimensions.Single(d => d.Path == "members[" + i + "].end_setback_mm");
                Assert.Equal(setback, dimension.ValueMm, 6);
                Assert.Equal(0.0, dimension.Start.DistanceTo(new Point2D(0, 0)), 6);
                Assert.Equal(setback * input.Ux, dimension.End.X, 6);
                Assert.Equal(setback * input.Uy, dimension.End.Y, 6);
                Assert.StartsWith("retiro ", dimension.Text);

                // El cuerpo de la barra empieza en el extremo retirado: sus dos primeros vértices están a 'setback' a lo largo del eje.
                var body = model.Polygons.Single(p => p.Kind == SketchPolygonKind.Member && p.Path == "members[" + i + "]");
                foreach (Point2D corner in body.Points.Take(2))
                {
                    double along = corner.X * input.Ux + corner.Y * input.Uy;
                    Assert.Equal(setback, along, 6);
                }
            }

            var vertical = model.Dimensions.Single(d => d.Path == "members[1].end_setback_mm");
            Assert.Equal(0.0, vertical.End.X, 6);
            Assert.Equal(-60.0, vertical.End.Y, 6);
        }

        [Fact]
        public void Build_DetalleD_BoltsAndPlateMatchConnectionGeometry()
        {
            var (model, spec, node) = BuildDetalleD();
            MemberSpec bolted = spec.Members[2];
            SketchMemberInput input = node.Find(bolted.ElementId)!;
            double setback = bolted.EndSetbackMm!.Value;

            List<BoltPosition> expectedBolts = ConnectionGeometry.ComputeBoltPositions(input.Ux, input.Uy, setback, bolted.Attachment!.Plate!, bolted.Attachment.Bolts!);
            var circles = model.Circles.Where(c => c.Kind == SketchCircleKind.Bolt).ToList();
            Assert.Equal(expectedBolts.Count, circles.Count);
            for (int i = 0; i < circles.Count; i++)
            {
                Assert.Equal(expectedBolts[i].X, circles[i].Center.X, 6);
                Assert.Equal(expectedBolts[i].Y, circles[i].Center.Y, 6);
                Assert.Equal(15.875 / 2.0, circles[i].RadiusMm, 6);
            }

            List<BoltPosition> expectedCorners = ConnectionGeometry.ComputeKnifePlateCorners(input.Ux, input.Uy, setback, bolted.Attachment.Plate!);
            var plate = model.Polygons.Single(p => p.Kind == SketchPolygonKind.KnifePlate);
            Assert.Equal(4, plate.Points.Count);
            for (int i = 0; i < 4; i++)
            {
                Assert.Equal(expectedCorners[i].X, plate.Points[i].X, 6);
                Assert.Equal(expectedCorners[i].Y, plate.Points[i].Y, 6);
            }
            Assert.Equal("members[2].attachment.plate", plate.Path);
        }

        [Fact]
        public void Build_DetalleD_ThicknessLabelsAndGussetOutline()
        {
            var (model, spec, _) = BuildDetalleD();

            Assert.Contains(model.Labels, l => l.Path == "gusset.thickness_mm" && l.Text == "PL 3/8\" (9,5 mm)");
            Assert.Contains(model.Labels, l => l.Path == "members[2].attachment.plate.thickness_mm" && l.Text == "PL10 (10,0 mm)");
            Assert.Contains(model.Labels, l => l.Path == "members[1].profile" && l.Text.StartsWith("Montante") && l.Text.Contains("90,0°"));
            Assert.Contains(model.Labels, l => l.Path == "members[2].attachment.bolts.diameter_mm" && l.Text.Contains("4 Ø5/8\""));

            var gusset = model.Polygons.Single(p => p.Kind == SketchPolygonKind.Gusset);
            Assert.Equal(spec.Gusset!.Outline!.PointsMm!.Count, gusset.Points.Count);
            Assert.Equal(-175.0, gusset.Points[0].X, 6);
            Assert.Equal(280.0, gusset.Points[0].Y, 6);

            // La caja envolvente incluye las líneas de cota desplazadas por encima del borde superior.
            Assert.True(model.BoundsMax.Y >= 280.0 + GussetNodeSketch.DimensionGapMm - Tol);
            Assert.True(model.BoundsMax.X >= 315.0 + GussetNodeSketch.DimensionGapMm - Tol);
            Assert.DoesNotContain(model.Notes, n => n.Contains("no coincide"));
        }

        [Fact]
        public void Build_WithoutModel_IsSchematicButComplete()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            var node = SketchNodeInput.FromModelFacts(spec, null);
            var model = SketchBuilder.Build(spec, node);

            Assert.True(node.IsSchematic);
            Assert.True(model.IsSchematic);
            Assert.NotEmpty(model.Notes);
            Assert.All(node.Members, m => Assert.False(m.FromModel));

            var upper = node.Find(1249630)!;
            Assert.Equal(Math.Cos(Math.PI / 4), upper.Ux, 6);
            Assert.Equal(Math.Sin(Math.PI / 4), upper.Uy, 6);
            var vertical = node.Find(1249631)!;
            Assert.Equal(0.0, vertical.Ux, 6);
            Assert.Equal(1.0, vertical.Uy, 6);
            var lower = node.Find(1249636)!;
            Assert.Equal(-Math.Cos(Math.PI / 4), lower.Ux, 6);
            Assert.Equal(-Math.Sin(Math.PI / 4), lower.Uy, 6);

            Assert.Equal(16, model.Dimensions.Count);
            Assert.Equal(4, model.Circles.Count);
            Assert.Contains(model.Labels, l => l.Text.Contains("(plano)"));
        }

        [Fact]
        public void Build_WhenMemberMissingInModel_FallsBackForThatMemberOnly()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            var facts = new FakeModelFacts();
            facts.Members.Remove(1249631);

            var node = SketchNodeInput.FromModelFacts(spec, facts);
            Assert.False(node.IsSchematic);
            Assert.False(node.Find(1249631)!.FromModel);
            Assert.True(node.Find(1249630)!.FromModel);
            Assert.Contains(node.Notes, n => n.Contains("1249631"));
        }

        [Fact]
        public void SketchBuilder_UsesRegistryAndFallsBackForGussetNode()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            ConnectionTypeRegistry.Clear();
            try
            {
                var withoutRegistry = SketchBuilder.Build(spec, null);
                Assert.False(withoutRegistry.IsEmpty);

                ConnectionTypeRegistry.Register(GussetNodeType.Instance);
                Assert.IsAssignableFrom<ISketchProvider>(ConnectionTypeRegistry.Find("gusset_node"));
                var withRegistry = SketchBuilder.Build(spec, null);
                Assert.Equal(withoutRegistry.Dimensions.Count, withRegistry.Dimensions.Count);

                spec.ConnectionType = "base_plate";
                var unknown = SketchBuilder.Build(spec, null);
                Assert.True(unknown.IsEmpty);
                Assert.Contains(unknown.Notes, n => n.Contains("base_plate"));
                Assert.Contains(unknown.Labels, l => l.Path == "connection_type");
            }
            finally
            {
                ConnectionTypeRegistry.Clear();
                ConnectionTypeRegistry.Register(GussetNodeType.Instance);
            }
        }

        [Fact]
        public void Build_GussetWithoutOutline_NotesItAndStillDrawsMembers()
        {
            var (_, spec) = Fixtures.DetalleDConfirmado();
            spec.Gusset!.Outline = null;
            var model = SketchBuilder.Build(spec, SketchNodeInput.FromModelFacts(spec, new FakeModelFacts()));

            Assert.DoesNotContain(model.Polygons, p => p.Kind == SketchPolygonKind.Gusset);
            Assert.Contains(model.Notes, n => n.Contains("contorno"));
            Assert.Equal(4, model.Polygons.Count(p => p.Kind == SketchPolygonKind.Member));
            Assert.Equal(14, model.Dimensions.Count); // sin ancho ni alto de la cartela
        }

        [Theory]
        [InlineData("HSS3X3X1/4", 76.2)]
        [InlineData("HSS2-1/2X2-1/2X3/16", 63.5)]
        [InlineData("HSS2-1-2X2-1-2X3-16 64x64", 64.0)]
        [InlineData("HSS 4X4X1/4", 101.6)]
        public void ProfileDimensions_ParsesHssWidths(string profile, double expectedMm)
        {
            Assert.True(ProfileDimensions.TryParseWidthMm(profile, out double mm));
            Assert.Equal(expectedMm, mm, 3);
        }

        [Theory]
        [InlineData("W12X26")]
        [InlineData("")]
        [InlineData(null)]
        public void ProfileDimensions_RejectsUnknownNames(string? profile)
        {
            Assert.False(ProfileDimensions.TryParseWidthMm(profile, out _));
            Assert.Equal(63.5, ProfileDimensions.WidthOrDefault(profile, 63.5), 6);
        }

        [Fact]
        public void SketchFormat_UsesOneDecimalAndComma()
        {
            Assert.Equal("565,0", SketchFormat.Mm(565));
            Assert.Equal("9,5", SketchFormat.Mm(9.525));
            Assert.Equal("12,7", SketchFormat.Mm(12.7));
            Assert.Equal("45,0°", SketchFormat.Degrees(45));
            Assert.Equal("PL 9,5 mm", GussetNodeSketch.ThicknessText(null, 9.525));
            Assert.Equal("PL 1/2\" (12,7 mm)", GussetNodeSketch.ThicknessText("1/2\"", 12.7));
        }

        [Fact]
        public void Dimension_OffsetPutsTheLineOnTheLeftOfItsDirection()
        {
            var d = new SketchDimension(new Point2D(0, 0), new Point2D(100, 0), 40, 100, "x");
            var (a, b) = d.DimensionLine();
            Assert.Equal(0.0, a.X, 6);
            Assert.Equal(40.0, a.Y, 6);
            Assert.Equal(100.0, b.X, 6);
            Assert.Equal(40.0, b.Y, 6);
            Assert.Equal("100,0", d.Text);
        }
    }
}
