using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Detección de nudos (Fase 8) sobre la cercha sintética de <see cref="SyntheticTruss"/>, sin Revit.</summary>
    public class NodeDetectorTests
    {
        private static List<DetectedNode> Detect() => NodeDetector.Detect(SyntheticTruss.Bars());

        private static DetectedNode CentralNode(List<DetectedNode> nodes, int index) =>
            nodes.Single(n => n.ChordElementId == SyntheticTruss.CentralChord && n.MemberElementIds.Contains(SyntheticTruss.UpLeft(index)));

        [Fact]
        public void Detect_FindsTheNodesOfTheSyntheticTrussWithTheirStatus()
        {
            List<DetectedNode> nodes = Detect();

            Assert.Equal(30, nodes.Count);
            Assert.Equal(13, nodes.Count(n => n.Status == NodeStatus.Untyped));
            Assert.Equal(1, nodes.Count(n => n.Status == NodeStatus.AmbiguousChord));
            Assert.Equal(1, nodes.Count(n => n.Status == NodeStatus.Offset));
            Assert.Equal(15, nodes.Count(n => n.Status == NodeStatus.Detected));

            // Los cuatro nudos del cordón central tienen el cordón 100 atravesando.
            for (int i = 0; i < 4; i++)
            {
                DetectedNode node = CentralNode(nodes, i);
                Assert.True(node.ChordContinuous);
                Assert.Equal(SyntheticTruss.NodeX[i], node.WorkPointMm.X, 0);
            }
        }

        [Fact]
        public void Detect_MeasuresSignedAnglesInTheCanonicalFrame()
        {
            List<DetectedNode> nodes = Detect();
            DetectedNode left = CentralNode(nodes, 0);
            Assert.Equal(NodeStatus.Detected, left.Status);
            Assert.Equal(3, left.Members.Count);
            Assert.Equal(135.0, left.Members.Single(m => m.ElementId == SyntheticTruss.UpLeft(0)).AngleDeg, 1);
            Assert.Equal(45.0, left.Members.Single(m => m.ElementId == SyntheticTruss.UpRight(0)).AngleDeg, 1);
            Assert.Equal(-135.0, left.Members.Single(m => m.ElementId == SyntheticTruss.Lower(0)).AngleDeg, 1);
            Assert.Equal(new[] { "+Y", "+Y", "-Y" }, left.Members.Select(m => m.Side).ToArray());
            Assert.Contains("2 +Y", left.Signature());
            Assert.Contains("1 -Y", left.Signature());

            // Mitad derecha: la diagonal inferior va hacia +X y hacia abajo (−45°): es el nudo en espejo.
            // (su extremo está 6 mm por encima del eje, así que el ángulo medido es −45,1°).
            DetectedNode right = CentralNode(nodes, 2);
            Assert.InRange(right.Members.Single(m => m.ElementId == SyntheticTruss.Lower(2)).AngleDeg, -46.0, -44.0);
        }

        [Fact]
        public void Detect_ReversedChord_StillGivesTheCanonicalFrame()
        {
            List<DetectedNode> nodes = Detect();
            var upper = nodes.Where(n => n.ChordElementId == SyntheticTruss.UpperChord && n.Status == NodeStatus.Detected).ToList();
            Assert.Equal(8, upper.Count);
            Assert.All(upper, n =>
            {
                Assert.True(n.ChordContinuous);
                Assert.True(n.Frame!.ChordReversed);
                Assert.Equal(1.0, n.Frame.X.X, 6);
                Assert.Single(n.Members);
                Assert.Equal("-Y", n.Members[0].Side);
            });
            var lower = nodes.Where(n => n.ChordElementId == SyntheticTruss.LowerChord && n.Status == NodeStatus.Detected).ToList();
            Assert.Equal(4, lower.Count);
            Assert.All(lower, n => Assert.False(n.Frame!.ChordReversed));
        }

        [Fact]
        public void Detect_GroupsEndsWithinTheClusterToleranceAndNotBeyond()
        {
            List<DetectedNode> nodes = Detect();
            // 6 mm por encima del eje: la diagonal inferior se agrupa y el nudo tiene sus tres barras.
            DetectedNode six = CentralNode(nodes, 2);
            Assert.Equal(3, six.MemberElementIds.Count);
            Assert.Contains(SyntheticTruss.Lower(2), six.MemberElementIds);
            // 40 mm corta: no se agrupa; el nudo se queda con dos barras y el extremo suelto es un nudo sin tipo.
            DetectedNode forty = CentralNode(nodes, 3);
            Assert.Equal(2, forty.MemberElementIds.Count);
            DetectedNode loose = nodes.Single(n => n.WorkPointMm.X > 7000 && n.WorkPointMm.X < 7100);
            Assert.Equal(NodeStatus.Untyped, loose.Status);
            Assert.Equal(0, loose.ChordElementId);
            Assert.Equal(new[] { SyntheticTruss.Lower(3) }, loose.MemberElementIds);
        }

        [Fact]
        public void Detect_TwoThroughBars_IsAmbiguousAndAForcedChordResolvesIt()
        {
            List<DetectedNode> nodes = Detect();
            DetectedNode ambiguous = CentralNode(nodes, 1);
            Assert.Equal(NodeStatus.AmbiguousChord, ambiguous.Status);
            Assert.Equal(new[] { SyntheticTruss.CentralChord, SyntheticTruss.CrossingBar }, ambiguous.ThroughBarIds.OrderBy(i => i).ToArray());
            Assert.Contains("elige el cordón", ambiguous.StatusDetail);

            var bars = SyntheticTruss.Bars();
            DetectedNode fixedNode = NodeDetector.BuildNode(ambiguous.Name, ambiguous.WorkPointMm, ambiguous.AllElementIds, bars, new NodeDetectorOptions(), SyntheticTruss.CentralChord);
            Assert.Equal(NodeStatus.Detected, fixedNode.Status);
            Assert.Equal(SyntheticTruss.CentralChord, fixedNode.ChordElementId);
            Assert.True(fixedNode.ChordContinuous);
            Assert.Equal(3, fixedNode.Members.Count);
        }

        [Fact]
        public void Detect_OffsetAndLooseBars()
        {
            List<DetectedNode> nodes = Detect();
            DetectedNode offset = nodes.Single(n => n.Status == NodeStatus.Offset);
            Assert.Equal(SyntheticTruss.LowerChord, offset.ChordElementId);
            Assert.Contains("no se cortan", offset.StatusDetail);
            Assert.Contains("8.5 mm", offset.StatusDetail);

            var looseEnds = nodes.Where(n => n.WorkPointMm.X >= 20000).ToList();
            Assert.Equal(2, looseEnds.Count);
            Assert.All(looseEnds, n => Assert.Equal(NodeStatus.Untyped, n.Status));
        }

        [Fact]
        public void Names_AreStableAndOrderedAlongTheTruss()
        {
            List<DetectedNode> first = Detect();
            List<DetectedNode> second = Detect();
            Assert.Equal(first.Select(n => n.Name), second.Select(n => n.Name));
            Assert.Equal("N1", first[0].Name);
            Assert.Equal("N30", first[29].Name);
            Assert.Equal("N6", CentralNode(first, 0).Name);
            Assert.Equal("N11", CentralNode(first, 1).Name);
            Assert.Equal("N18", CentralNode(first, 2).Name);
            Assert.Equal("N22", CentralNode(first, 3).Name);
            for (int i = 1; i < first.Count; i++)
            {
                Assert.True(first[i - 1].WorkPointMm.X <= first[i].WorkPointMm.X + 1e-6, "los nudos van ordenados por X global");
            }
            Assert.Equal("N31", NodeDetector.NextName(first.Select(n => n.Name)));
        }

        [Fact]
        public void IsThrough_RequiresTheAxisNearAndThePointInsideTheSpan()
        {
            var bar = new DetectorBar(1, new Vec3(0, 0, 0), new Vec3(1000, 0, 0), null);
            Assert.True(NodeDetector.IsThrough(bar, new Vec3(500, 4, 0), 5, 10));
            Assert.False(NodeDetector.IsThrough(bar, new Vec3(500, 6, 0), 5, 10));
            Assert.False(NodeDetector.IsThrough(bar, new Vec3(5, 0, 0), 5, 10), "el punto está en el extremo: la barra llega, no atraviesa");
            Assert.False(NodeDetector.IsThrough(bar, new Vec3(1200, 0, 0), 5, 10));
        }
    }
}
