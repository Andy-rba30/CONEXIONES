using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Ronda 8c (V1): el alzado de la cercha que dibuja la ventana del plan sale de <see cref="TrussMap"/>, sin Revit.</summary>
    public class TrussMapTests
    {
        [Fact]
        public void Build_HangarTruss8b_IsTheElevationOfTheTruss()
        {
            // La cercha de la 8b está en Y = 17204 constante (vertical, a lo largo de X): el alzado es el plano X–Z del modelo tal
            // cual, con 56 barras, 59 nudos (16 visibles) y el Detalle D (N4) en X = −11870 sobre el cordón central (Z = 17423).
            var bars = HangarTruss8b.Bars();
            BatchPlan plan = PlanBuilder.Build(BatchPlanTests.Hangar8bRequest());
            TrussMap map = TrussMap.Build(plan, bars);

            Assert.Equal(56, map.Segments.Count);
            Assert.Equal(59, map.Nodes.Count);
            Assert.Equal(16, map.VisibleCount);
            Assert.Equal(43, map.HiddenCount);
            Assert.True(map.IsCoplanar);
            Assert.InRange(map.MaxPlaneDistanceMm, 0.0, 1.0);
            Assert.Equal(1.0, map.AxisU.X, 1e-6);
            Assert.Equal(0.0, map.AxisU.Z, 1e-6);
            Assert.Equal(1.0, map.AxisV.Z, 1e-6);
            Assert.Equal(1.0, Math.Abs(map.Normal.Y), 1e-6);

            TrussMapNode n4 = map.Find("N4")!;
            Assert.Equal(-11870.0, n4.X, 1.0);
            Assert.Equal(17423.0, n4.Y, 1.0);
            Assert.Equal("4", n4.Number);
            Assert.Equal(NodeStatus.Ready, n4.Status);
            Assert.True(n4.VisibleByDefault);
            Assert.False(n4.IsMirrored);
            Assert.Equal(PlanAdvice.Amber, n4.ColorName);
            Assert.Equal("N4 · Listo con aviso · Nudo tipico Detalle D · igual", n4.Label);
            TrussMapNode n7 = map.Find("N7")!;
            Assert.Equal(-6740.5, n7.X, 1.0);
            Assert.True(n7.IsMirrored);
            Assert.EndsWith("· en espejo", n7.Label);

            // El rectángulo envolvente: del extremo alto de la primera diagonal del Detalle D (1251053, X = −14536,8) al último
            // extremo de diagonal a la derecha (1251723), y de la diagonal más baja (1251059, Z = 14894,7) a la más alta (19960,1).
            // (el plano de mínimos cuadrados se inclina una milésima porque Y varía 0,3 mm a lo largo de la cercha: ±1 mm basta).
            Assert.Equal(-14536.8, map.MinX, 1.0);
            Assert.Equal(67553.1, map.MaxX, 1.0);
            Assert.Equal(14894.7, map.MinY, 1.0);
            Assert.Equal(19960.1, map.MaxY, 1.0);
            Assert.True(map.Segments.Single(s => s.ElementId == HangarTruss8b.DetalleDChord).IsChord);
            Assert.False(map.Segments.Single(s => s.ElementId == HangarTruss8b.DetalleDUpLeft).IsChord);
            TrussMapSegment chord = map.Segments.Single(s => s.ElementId == HangarTruss8b.DetalleDChord);
            Assert.Equal(17423.0, chord.Y0, 1.0);
            Assert.Equal(17423.0, chord.Y1, 1.0);
            Assert.All(map.Nodes.Where(n => n.Status == NodeStatus.Ready), n => Assert.Equal(17423.0, n.Y, 1.0));
        }

        [Fact]
        public void Build_ReversedChord_GivesTheSameMap()
        {
            // Invertir el sentido de la curva del cordón central no cambia ni el plano ni las coordenadas de los nudos.
            var bars = SyntheticTruss.Bars();
            var reversed = bars.Select(b => b.ElementId == SyntheticTruss.CentralChord
                ? new DetectorBar(b.ElementId, b.EndMm, b.StartMm, b.TypeName, b.DepthMm)
                : b).ToList();
            TrussMap original = TrussMap.Build(PlanBuilder.Build(BatchPlanTests.Request(bars: bars)), bars);
            TrussMap flipped = TrussMap.Build(PlanBuilder.Build(BatchPlanTests.Request(bars: reversed)), reversed);

            Assert.Equal(original.Segments.Count, flipped.Segments.Count);
            Assert.Equal(original.Nodes.Count, flipped.Nodes.Count);
            Assert.Equal(original.AxisU, flipped.AxisU);
            Assert.Equal(original.AxisV, flipped.AxisV);
            foreach (TrussMapNode node in original.Nodes)
            {
                TrussMapNode other = flipped.Find(node.Name)!;
                Assert.Equal(node.X, other.X, 0.11);
                Assert.Equal(node.Y, other.Y, 0.11);
                Assert.Equal(node.Status, other.Status);
            }
            TrussMapSegment a = original.Segments.Single(s => s.ElementId == SyntheticTruss.CentralChord);
            TrussMapSegment b = flipped.Segments.Single(s => s.ElementId == SyntheticTruss.CentralChord);
            Assert.Equal(a.X0, b.X1, 0.11);
            Assert.Equal(a.X1, b.X0, 0.11);
            Assert.Equal(original.MinX, flipped.MinX, 0.11);
            Assert.Equal(original.MaxY, flipped.MaxY, 0.11);
            // La cercha sintética tiene un montante y una diagonal 8,5 mm fuera del plano: el plano de mínimos cuadrados se inclina
            // una millonésima; sigue siendo el alzado X–Z a efectos del dibujo.
            Assert.Equal(1.0, original.AxisU.X, 1e-6);
            Assert.Equal(1.0, original.AxisV.Z, 1e-6);
            Assert.Equal(original.MaxPlaneDistanceMm, flipped.MaxPlaneDistanceMm, 0.01);
        }

        [Fact]
        public void Build_NonCoplanarBars_UsesTheLeastSquaresPlane()
        {
            // Dos barras en el plano Y = 0 y una tercera que se sale 300 mm: el plano del mapa es el de mínimos cuadrados (mejor
            // que Y = 0 en suma de cuadrados) y el mapa lo dice (IsCoplanar = false, MaxPlaneDistanceMm > 0).
            var bars = new List<DetectorBar>
            {
                new DetectorBar(1, new Vec3(0, 0, 0), new Vec3(4000, 0, 0), "A"),
                new DetectorBar(2, new Vec3(0, 0, 0), new Vec3(2000, 0, 2000), "B"),
                new DetectorBar(3, new Vec3(4000, 0, 0), new Vec3(2000, 300, 2000), "C"),
            };
            var plan = new BatchPlan();
            plan.Nodes.Add(new PlanNode { Name = "N1", Status = NodeStatus.Untyped, WorkPointMm = new[] { 0.0, 0.0, 0.0 }, ElementIds = new List<long> { 1, 2 } });
            TrussMap map = TrussMap.Build(plan, bars);

            Assert.False(map.IsCoplanar);
            Assert.True(map.MaxPlaneDistanceMm > 10.0);
            Assert.Equal(3, map.Segments.Count);
            Assert.Single(map.Nodes);
            double fitted = 0.0;
            double flat = 0.0;
            foreach (DetectorBar bar in bars)
            {
                foreach (Vec3 p in new[] { bar.StartMm, bar.EndMm })
                {
                    double d = (p - map.Origin).Dot(map.Normal);
                    fitted += d * d;
                    flat += p.Y * p.Y;
                }
            }
            Assert.True(fitted < flat, "mínimos cuadrados: " + fitted + " debe ser menor que el plano Y = 0: " + flat);
            Assert.True(fitted > 0.0);
            Assert.True(map.AxisV.Z > 0.9);
            Assert.True(map.AxisU.X > 0.9);
            Assert.Equal(1.0, map.AxisU.Length, 1e-9);
            Assert.Equal(0.0, map.AxisU.Dot(map.AxisV), 1e-9);
            Assert.Equal(0.0, map.AxisU.Dot(map.Normal), 1e-9);
        }

        [Fact]
        public void Build_DegenerateCases_StillBuild()
        {
            // Una sola barra: el plano vertical que la contiene. Sin barras: los nudos se proyectan en el alzado X–Z.
            var one = new List<DetectorBar> { new DetectorBar(1, new Vec3(100, 50, 10), new Vec3(3100, 50, 10), "A") };
            var plan = new BatchPlan();
            plan.Nodes.Add(new PlanNode { Name = "N1", Status = NodeStatus.Untyped, WorkPointMm = new[] { 100.0, 50.0, 10.0 }, ElementIds = new List<long> { 1 } });
            TrussMap map = TrussMap.Build(plan, one);
            Assert.Single(map.Segments);
            Assert.True(map.IsCoplanar);
            Assert.Equal(1.0, map.AxisU.X, 1e-9);
            Assert.Equal(1.0, map.AxisV.Z, 1e-9);
            Assert.Equal(100.0, map.Nodes[0].X, 0.1);
            Assert.Equal(10.0, map.Nodes[0].Y, 0.1);

            TrussMap empty = TrussMap.Build(plan, new List<DetectorBar>());
            Assert.Empty(empty.Segments);
            Assert.Single(empty.Nodes);
            Assert.False(empty.IsEmpty);
            Assert.Equal(100.0, empty.Nodes[0].X, 0.1);

            TrussMap nothing = TrussMap.Build(new BatchPlan(), new List<DetectorBar>());
            Assert.True(nothing.IsEmpty);
            Assert.Equal(0.0, nothing.Width);
        }

        [Fact]
        public void Build_TrussAlongY_MapsGlobalYToTheHorizontalAxis()
        {
            // Una cercha en el plano X = 500 (va a lo largo de Y): el eje horizontal del mapa es +Y global y el vertical +Z.
            var bars = new List<DetectorBar>
            {
                new DetectorBar(1, new Vec3(500, 0, 0), new Vec3(500, 6000, 0), "cordón"),
                new DetectorBar(2, new Vec3(500, 0, 0), new Vec3(500, 1500, 1500), "diag"),
                new DetectorBar(3, new Vec3(500, 3000, 0), new Vec3(500, 1500, 1500), "diag"),
            };
            var plan = new BatchPlan();
            plan.Nodes.Add(new PlanNode { Name = "N1", Status = NodeStatus.NoMatch, WorkPointMm = new[] { 500.0, 1500.0, 1500.0 }, ElementIds = new List<long> { 2, 3 } });
            TrussMap map = TrussMap.Build(plan, bars);
            Assert.True(map.IsCoplanar);
            Assert.Equal(1.0, map.AxisU.Y, 1e-9);
            Assert.Equal(1.0, map.AxisV.Z, 1e-9);
            Assert.Equal(1500.0, map.Nodes[0].X, 0.1);
            Assert.Equal(1500.0, map.Nodes[0].Y, 0.1);
            Assert.Equal(0.0, map.MinX, 0.1);
            Assert.Equal(6000.0, map.MaxX, 0.1);
        }
    }
}
