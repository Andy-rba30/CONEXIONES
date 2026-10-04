using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Types;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Croquis 2D del Detalle D (Fase 6): se construye en Core con el fixture confirmado y los hechos del nudo real del
    /// Hangar (<see cref="FakeModelFacts"/>). Comprueba piezas y valores de las cotas sin Revit ni WPF.
    /// </summary>
    public class SketchBuilderTests
    {
        internal static string FindRepoFile(string relativePath)
        {
            string? current = AppDomain.CurrentDomain.BaseDirectory;
            while (!string.IsNullOrEmpty(current))
            {
                string candidate = Path.Combine(current, relativePath);
                if (File.Exists(candidate)) return candidate;
                current = Path.GetDirectoryName(current);
            }
            throw new FileNotFoundException("No se encontró " + relativePath + " subiendo desde " + AppDomain.CurrentDomain.BaseDirectory);
        }

        internal static (string rawJson, ConnectionSpec spec) LoadConfirmedFixture()
        {
            string json = File.ReadAllText(FindRepoFile(Path.Combine("docs", "fixtures", "detalle-D-confirmado.json")));
            return (json, ConnectionSpec.FromJson(json)!);
        }

        private static Sketch BuildDetalleD(out ConnectionSpec spec)
        {
            (_, spec) = LoadConfirmedFixture();
            var nodeInfo = SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts());
            return SketchBuilder.Build(spec, nodeInfo);
        }

        private static IEnumerable<double> Values(Sketch sketch, DimensionKind kind) =>
            sketch.Dimensions.Where(d => d.Kind == kind).Select(d => Math.Round(d.ValueMm, 3));

        [Fact]
        public void NodeInfo_FromModelFacts_UsesRealDirectionsAndWidths()
        {
            var (_, spec) = LoadConfirmedFixture();
            var info = SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts());

            Assert.False(info.IsApproximate);
            Assert.Equal(76.2, info.ChordWidthMm, 3);
            Assert.Equal(3, info.Members.Count);
            Assert.All(info.Members, m => Assert.Equal(63.5, m.WidthMm, 3));
            Assert.All(info.Members, m => Assert.Equal(1.0, Math.Sqrt(m.Ux * m.Ux + m.Uy * m.Uy), 6));

            // Ángulos con signo en el marco canónico (Fase 7, la misma regla que conn_get_node_info): la diagonal
            // inferior apunta hacia −X y hacia abajo y sale a −135°; su inclinación respecto al cordón sigue siendo 45°.
            // Las dos superiores son las del Hangar (ronda 7b): 1249630 a la izquierda (135°) y 1249631 a la derecha (45°).
            Assert.Equal(135.0, info.Find(1249630)!.AngleInPlaneDeg, 1);
            Assert.Equal(45.0, info.Find(1249631)!.AngleInPlaneDeg, 1);
            Assert.Equal(-135.0, info.Find(1249636)!.AngleInPlaneDeg, 1);
            Assert.True(info.Find(1249630)!.Uy > 0, "con el marco canónico +Y apunta hacia arriba: la diagonal superior tiene Uy > 0");
            Assert.True(info.Find(1249630)!.Ux < 0 && info.Find(1249631)!.Ux > 0, "una diagonal superior a cada lado del nudo");
            // Las dos diagonales superiores quedan a un lado del cordón y la inferior al otro.
            Assert.True(Math.Sign(info.Find(1249630)!.Uy) == Math.Sign(info.Find(1249631)!.Uy));
            Assert.True(Math.Sign(info.Find(1249636)!.Uy) != Math.Sign(info.Find(1249630)!.Uy));
            Assert.NotNull(info.LocalYInGlobal);
        }

        [Fact]
        public void DetalleD_GussetDimensionsAre565By530()
        {
            Sketch sketch = BuildDetalleD(out _);

            Assert.Equal(new[] { 565.0 }, Values(sketch, DimensionKind.GussetWidth));
            Assert.Equal(new[] { 530.0 }, Values(sketch, DimensionKind.GussetHeight));
            Assert.Equal("565,0", sketch.Dimensions.Single(d => d.Kind == DimensionKind.GussetWidth).Text);
            Assert.Equal("530,0", sketch.Dimensions.Single(d => d.Kind == DimensionKind.GussetHeight).Text);
            Assert.Equal("gusset.width_mm", sketch.Dimensions.Single(d => d.Kind == DimensionKind.GussetWidth).Path);

            SketchPolygon gusset = sketch.Polygons.Single(p => p.Kind == SketchKind.Gusset);
            Assert.Equal(8, gusset.Points.Count);
            Assert.True(gusset.IsClosed);
        }

        [Fact]
        public void DetalleD_SetbacksAre180_60_260()
        {
            Sketch sketch = BuildDetalleD(out ConnectionSpec spec);

            Assert.Equal(new[] { 180.0, 60.0, 260.0 }, Values(sketch, DimensionKind.MemberSetback));
            Assert.Equal(new[] { "members[0].end_setback_mm", "members[1].end_setback_mm", "members[2].end_setback_mm" },
                sketch.Dimensions.Where(d => d.Kind == DimensionKind.MemberSetback).Select(d => d.Path));

            // La cota del retiro va del punto de trabajo al extremo real de la barra, y ahí empieza el cuerpo dibujado.
            var nodeInfo = SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts());
            var bodies = sketch.Polygons.Where(p => p.Kind == SketchKind.MemberOutline).ToList();
            Assert.Equal(3, bodies.Count);
            for (int i = 0; i < 3; i++)
            {
                SketchDimension setback = sketch.Dimensions.Where(d => d.Kind == DimensionKind.MemberSetback).ElementAt(i);
                Assert.Equal(0.0, setback.Start.DistanceTo(new SketchPoint(0, 0)), 6);
                Assert.Equal(spec.Members[i].EndSetbackMm!.Value, setback.End.DistanceTo(new SketchPoint(0, 0)), 3);

                SketchMemberInfo info = nodeInfo.Find(spec.Members[i].ElementId)!;
                // Puntos 1 y 2 del cuerpo = cara del extremo; su punto medio está a "retiro" del origen sobre el eje.
                var face = new SketchPoint((bodies[i].Points[1].X + bodies[i].Points[2].X) / 2, (bodies[i].Points[1].Y + bodies[i].Points[2].Y) / 2);
                Assert.Equal(spec.Members[i].EndSetbackMm!.Value, face.X * info.Ux + face.Y * info.Uy, 3);
                Assert.Equal(63.5, bodies[i].Points[1].DistanceTo(bodies[i].Points[2]), 3);
                Assert.False(bodies[i].IsClosed);
            }
        }

        [Fact]
        public void DetalleD_SlotsKnifePlateAndBolts()
        {
            Sketch sketch = BuildDetalleD(out _);

            Assert.Equal(new[] { 150.0, 150.0 }, Values(sketch, DimensionKind.SlotLength));
            Assert.Equal(2, sketch.Polygons.Count(p => p.Kind == SketchKind.Slot));

            Assert.Equal(new[] { 170.0 }, Values(sketch, DimensionKind.PlateLength));
            Assert.Equal(new[] { 140.0 }, Values(sketch, DimensionKind.PlateWidth));
            Assert.Equal(new[] { 60.0 }, Values(sketch, DimensionKind.BoltSpacing));
            Assert.Equal(new[] { 40.0 }, Values(sketch, DimensionKind.BoltEdge));
            Assert.Equal(new[] { 40.0 }, Values(sketch, DimensionKind.BoltFirstRow));
            Assert.Equal("members[2].attachment.bolts.spacing_mm", sketch.Dimensions.Single(d => d.Kind == DimensionKind.BoltSpacing).Path);

            Assert.Single(sketch.Polygons, p => p.Kind == SketchKind.KnifePlate);
            var bolts = sketch.Circles.Where(c => c.Kind == SketchKind.Bolt).ToList();
            Assert.Equal(4, bolts.Count);
            Assert.All(bolts, b => Assert.Equal(15.875 / 2.0, b.RadiusMm, 6));

            // Los pernos quedan dentro del polígono de la placa cuchilla.
            SketchPolygon plate = sketch.Polygons.Single(p => p.Kind == SketchKind.KnifePlate);
            var polygon = new MotorConexiones.Core.Geometry2D.Polygon2D(plate.Points.Select(p => new MotorConexiones.Core.Geometry2D.Point2D(p.X, p.Y)));
            Assert.All(bolts, b => Assert.True(polygon.ContainsPoint(new MotorConexiones.Core.Geometry2D.Point2D(b.Center.X, b.Center.Y))));

            // La cota de paso mide 60 entre dos pernos consecutivos a lo largo de la barra.
            SketchDimension spacing = sketch.Dimensions.Single(d => d.Kind == DimensionKind.BoltSpacing);
            Assert.Equal(60.0, spacing.Start.DistanceTo(spacing.End), 3);

            // Soldaduras: 2 por ranura soldada y 2 de la placa a la barra.
            Assert.Equal(6, sketch.Lines.Count(l => l.Kind == SketchKind.Weld));
        }

        [Fact]
        public void DetalleD_BoltLabelShowsGripAndLength()
        {
            // Ronda 6b: el croquis dice lo que se creará: agarre cartela + placa, longitud del perno y cara de la cartela.
            Sketch sketch = BuildDetalleD(out _);
            SketchLabel bolts = sketch.Labels.Single(l => l.Path == "members[2].attachment.bolts.length_mm");
            Assert.StartsWith("4 pernos Ø5/8\"", bolts.Text);
            Assert.Contains("agarre 19,5 mm (cartela 9,5 + placa 10,0)", bolts.Text);
            Assert.Contains("L 44,", bolts.Text);
            Assert.Contains("placa en cara +z", bolts.Text);
            Assert.DoesNotContain("(del plano)", bolts.Text);

            // Con la longitud escrita en el plano, la etiqueta lo dice.
            var (_, spec) = LoadConfirmedFixture();
            spec.Members[2].Attachment!.Bolts!.LengthMm = 50.8;
            spec.Members[2].Attachment!.Plate!.GussetFace = "-z";
            Sketch fromSpec = SketchBuilder.Build(spec, SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));
            string text = fromSpec.Labels.Single(l => l.Path == "members[2].attachment.bolts.length_mm").Text;
            Assert.Contains("L 50,8 mm (del plano)", text);
            Assert.Contains("placa en cara -z", text);
        }

        [Fact]
        public void DetalleD_LabelsAndPieces()
        {
            Sketch sketch = BuildDetalleD(out _);

            SketchLabel thickness = sketch.Labels.Single(l => l.Path == "gusset.thickness_mm");
            Assert.Contains("3/8\"", thickness.Text);
            Assert.Contains("9,5 mm", thickness.Text);

            Assert.Contains(sketch.Labels, l => l.Text.Contains("HSS3X3X1/4"));
            Assert.Contains(sketch.Labels, l => l.Text.Contains("through_slot"));
            Assert.Contains(sketch.Labels, l => l.Text.Contains("soldadura 5,0 mm todo el contorno"));
            Assert.Equal(3, sketch.Labels.Count(l => l.Path != null && l.Path.StartsWith("members[", StringComparison.Ordinal) && l.Path.EndsWith(".profile", StringComparison.Ordinal)));
            Assert.Contains(sketch.Labels, l => l.Path == "chord.profile");

            // Cordón: eje y dos bordes a ±38,1 mm; punto de trabajo: dos trazos.
            Assert.Equal(2, sketch.Lines.Count(l => l.Kind == SketchKind.ChordEdge));
            Assert.All(sketch.Lines.Where(l => l.Kind == SketchKind.ChordEdge), l => Assert.Equal(38.1, Math.Abs(l.Start.Y), 3));
            Assert.Equal(4, sketch.Lines.Count(l => l.Kind == SketchKind.Axis));
            Assert.Equal(2, sketch.Lines.Count(l => l.Kind == SketchKind.WorkPoint));

            Assert.Equal(12, sketch.Dimensions.Count);
            Assert.Empty(sketch.Notes);

            SketchBounds bounds = sketch.GetBounds();
            Assert.False(bounds.IsEmpty);
            Assert.True(bounds.Width > 565 && bounds.Height > 530);
        }

        [Fact]
        public void ChangingThickness_ChangesTheLabel()
        {
            var (_, spec) = LoadConfirmedFixture();
            spec.Gusset!.ThicknessMm = 12.7;
            spec.Gusset.ThicknessLabel = "1/2\"";
            Sketch sketch = SketchBuilder.Build(spec, SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));

            SketchLabel thickness = sketch.Labels.Single(l => l.Path == "gusset.thickness_mm");
            Assert.Equal("cartela PL 1/2\" · 12,7 mm", thickness.Text);
            // La ranura de las barras soldadas tiene el ancho del espesor nuevo.
            SketchPolygon slot = sketch.Polygons.First(p => p.Kind == SketchKind.Slot);
            Assert.Equal(12.7, slot.Points[0].DistanceTo(slot.Points[3]), 3);
        }

        [Fact]
        public void WidthMismatch_IsShownInTheDimensionText()
        {
            var (_, spec) = LoadConfirmedFixture();
            spec.Gusset!.WidthMm = 560.0;
            Sketch sketch = SketchBuilder.Build(spec, SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));
            Assert.Equal("565,0 (width_mm = 560,0)", sketch.Dimensions.Single(d => d.Kind == DimensionKind.GussetWidth).Text);
        }

        [Fact]
        public void WithoutModel_SketchIsApproximateButComplete()
        {
            var (_, spec) = LoadConfirmedFixture();
            var nodeInfo = SketchNodeInfo.FromModelFacts(spec, null);
            Assert.True(nodeInfo.IsApproximate);

            Sketch sketch = SketchBuilder.Build(spec, nodeInfo);
            Assert.NotEmpty(sketch.Notes);
            Assert.Equal(new[] { 180.0, 60.0, 260.0 }, Values(sketch, DimensionKind.MemberSetback));
            Assert.Equal(4, sketch.Circles.Count);
            // Direcciones por ángulo (las tres a 45° desde la ronda 7b), repartidas por cuadrantes: la segunda sale a (−x, +y).
            Assert.Equal(-Math.Cos(Math.PI / 4), nodeInfo.Members[1].Ux, 6);
            Assert.Equal(Math.Sin(Math.PI / 4), nodeInfo.Members[1].Uy, 6);
            Assert.Contains(sketch.Labels, l => l.Text.Contains("(dirección aproximada)"));
        }

        [Fact]
        public void UnknownType_ReturnsEmptySketchWithNote()
        {
            var (_, spec) = LoadConfirmedFixture();
            spec.ConnectionType = "base_plate";
            Sketch sketch = SketchBuilder.Build(spec, SketchNodeInfo.FromSpecAngles(spec));
            Assert.Empty(sketch.Dimensions);
            Assert.Empty(sketch.Polygons);
            Assert.Contains(sketch.Notes, n => n.Contains("base_plate"));
        }

        [Fact]
        public void GussetNodeType_IsTheSketchProvider()
        {
            Assert.IsAssignableFrom<ISketchProvider>(GussetNodeType.Instance);
            var (_, spec) = LoadConfirmedFixture();
            Sketch viaType = ((ISketchProvider)GussetNodeType.Instance).BuildSketch(spec, SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));
            Assert.Equal(12, viaType.Dimensions.Count);
        }

        [Fact]
        public void MissingOutline_DrawsRectangleAndNotes()
        {
            var (_, spec) = LoadConfirmedFixture();
            spec.Gusset!.Outline = null;
            Sketch sketch = SketchBuilder.Build(spec, SketchNodeInfo.FromModelFacts(spec, new FakeModelFacts()));
            SketchPolygon gusset = sketch.Polygons.Single(p => p.Kind == SketchKind.Gusset);
            Assert.Equal(4, gusset.Points.Count);
            Assert.Equal(new[] { 565.0 }, Values(sketch, DimensionKind.GussetWidth));
            Assert.Equal(new[] { 530.0 }, Values(sketch, DimensionKind.GussetHeight));
            Assert.Contains(sketch.Notes, n => n.Contains("outline.points_mm"));
        }

        [Theory]
        [InlineData(565.0, "565,0")]
        [InlineData(9.525, "9,5")]
        [InlineData(12.7, "12,7")]
        [InlineData(0.04, "0,0")]
        public void SketchText_FormatsMmWithOneDecimalAndComma(double value, string expected)
        {
            Assert.Equal(expected, SketchText.Mm(value));
        }

        [Fact]
        public void SketchText_ThicknessAndWeld()
        {
            Assert.Equal("PL 3/8\" · 9,5 mm", SketchText.Thickness("3/8\"", 9.525));
            Assert.Equal("PL 10 · 10,0 mm", SketchText.Thickness("PL10", 10.0));
            Assert.Equal("PL 10,0 mm", SketchText.Thickness(null, 10.0));
            Assert.Equal("soldadura 5,0 mm todo el contorno", SketchText.Weld(5.0, true));
            Assert.Equal("soldadura 6,0 mm", SketchText.Weld(6.0, false));
        }

        [Fact]
        public void DimensionLine_IsOffsetAlongTheLeftNormal()
        {
            var dimension = new SketchDimension(new SketchPoint(0, 0), new SketchPoint(100, 0), -60, 100, "100,0", DimensionKind.GussetWidth, null);
            var (start, end) = dimension.GetDimensionLine();
            Assert.Equal(0.0, start.X, 6);
            Assert.Equal(-60.0, start.Y, 6);
            Assert.Equal(100.0, end.X, 6);
            Assert.Equal(-60.0, end.Y, 6);
        }
    }
}
