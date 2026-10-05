using System;
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
                // El punto de trabajo es la media de los cortes con el eje del cordón: en X = 5000 la diagonal que termina 6 mm
                // por encima del eje lo corta 6 mm a la derecha, así que la media queda 2 mm a la derecha.
                Assert.InRange(node.WorkPointMm.X, SyntheticTruss.NodeX[i] - 3.0, SyntheticTruss.NodeX[i] + 3.0);
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
            DetectedNode loose = nodes.Single(n => n.WorkPointMm.X > 7050 && n.WorkPointMm.X < 7100);
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

        // ---- Ronda 8b: extremos cortados en la cara del cordón ----

        private const double S45 = 0.70710678118;

        /// <summary>
        /// Nudo como el del Detalle D en el Hangar: cordón HSS3X3 (76,2 mm) a lo largo de X y tres diagonales HSS 2-1/2
        /// (63,5 mm) a 45° cuyos extremos NO llegan al eje del cordón: la de arriba a la izquierda se queda a
        /// <paramref name="upLeftGapMm"/> del eje (medido perpendicular al cordón), la de abajo a 33 mm y la de arriba a la
        /// derecha a 14 mm. Los tres ejes cortan el eje del cordón en X = 3000.
        /// </summary>
        private static List<DetectorBar> FaceCutNode(double upLeftGapMm = 58.8, double chordDepth = SyntheticTruss.ChordDepth, double diagonalDepth = SyntheticTruss.DiagonalDepth)
        {
            Vec3 P(double x, double z) => new Vec3(x, 0, z);
            return new List<DetectorBar>
            {
                new DetectorBar(1, P(0, 0), P(6000, 0), SyntheticTruss.ChordType, chordDepth),
                new DetectorBar(2, P(3000 - upLeftGapMm, upLeftGapMm), P(3000 - 2000, 2000), SyntheticTruss.DiagonalType, diagonalDepth), // 135°, corta
                new DetectorBar(3, P(3000 - 2000, -2000), P(3000 - 33, -33), SyntheticTruss.DiagonalType, diagonalDepth),                 // −135°, corta
                new DetectorBar(4, P(3000 + 14, 14), P(3000 + 2000, 2000), SyntheticTruss.DiagonalType, diagonalDepth),                   // 45°, corta
            };
        }

        [Fact]
        public void Detect_EndsCutAtTheChordFace_AreGroupedAtTheCutWithTheChordAxis()
        {
            List<DetectedNode> nodes = NodeDetector.Detect(FaceCutNode());

            // Un nudo con el cordón atravesando y las tres diagonales, más los dos extremos del cordón y los tres extremos lejanos.
            Assert.Equal(6, nodes.Count);
            DetectedNode node = nodes.Single(n => n.ChordElementId == 1 && n.MemberElementIds.Count > 0);
            Assert.Equal(NodeStatus.Detected, node.Status);
            Assert.True(node.ChordContinuous);
            Assert.Equal(new long[] { 2, 3, 4 }, node.MemberElementIds.OrderBy(i => i).ToArray());
            Assert.Equal(3000.0, node.WorkPointMm.X, 0.5);
            Assert.Equal(0.0, node.WorkPointMm.Z, 0.5);
            Assert.Equal(135.0, node.Members.Single(m => m.ElementId == 2).AngleDeg, 1);
            Assert.Equal(-135.0, node.Members.Single(m => m.ElementId == 3).AngleDeg, 1);
            Assert.Equal(45.0, node.Members.Single(m => m.ElementId == 4).AngleDeg, 1);
            Assert.All(node.Members, m => Assert.True(m.ReachesNode));
            // Cuánto se queda corta cada barra (distancia del extremo real al punto de trabajo): 58,8·√2, 33·√2 y 14·√2.
            Assert.Equal(58.8 / S45, node.Members.Single(m => m.ElementId == 2).EndGapMm, 0.3);
            Assert.Equal(33.0 / S45, node.Members.Single(m => m.ElementId == 3).EndGapMm, 0.3);
            Assert.Equal(14.0 / S45, node.Members.Single(m => m.ElementId == 4).EndGapMm, 0.3);
            Assert.Equal(2, nodes.Count(n => n.Status == NodeStatus.Untyped && n.MemberElementIds.SequenceEqual(new long[] { 1 })));
        }

        [Fact]
        public void Detect_FaceReach_FollowsTheProfileDepthOrTheConfiguredValue()
        {
            // Alcance de cara = 38,1 + 31,75 + 10 = 79,85 mm: a 79 mm del eje se agrupa, a 95 mm no.
            Assert.Equal(3, NodeDetector.Detect(FaceCutNode(79.0)).Single(n => n.ChordContinuous).MemberElementIds.Count);
            List<DetectedNode> far = NodeDetector.Detect(FaceCutNode(95.0));
            Assert.Equal(2, far.Single(n => n.ChordContinuous).MemberElementIds.Count);
            Assert.Single(far, n => n.Status == NodeStatus.Untyped && n.MemberElementIds.SequenceEqual(new long[] { 2 }) && n.WorkPointMm.X > 2800 && n.WorkPointMm.X < 3000);

            // Sin canto conocido se suponen 40 mm por barra (90 mm de alcance): 85 sí, 95 no.
            Assert.Equal(3, NodeDetector.Detect(FaceCutNode(85.0, 0, 0)).Single(n => n.ChordContinuous).MemberElementIds.Count);
            Assert.Equal(2, NodeDetector.Detect(FaceCutNode(95.0, 0, 0)).Single(n => n.ChordContinuous).MemberElementIds.Count);

            // node_face_reach_mm fija el alcance: con 120 mm los 95 se agrupan; con 20 mm solo la diagonal a 14 mm.
            var wide = new NodeDetectorOptions { FaceReachMm = 120.0 };
            Assert.Equal(3, NodeDetector.Detect(FaceCutNode(95.0), wide).Single(n => n.ChordContinuous).MemberElementIds.Count);
            var narrow = new NodeDetectorOptions { FaceReachMm = 20.0 };
            Assert.Equal(new long[] { 4 }, NodeDetector.Detect(FaceCutNode(), narrow).Single(n => n.ChordContinuous).MemberElementIds.ToArray());

            // FromConfig: 0 = según el canto; un valor positivo manda.
            Assert.Equal(0.0, NodeDetectorOptions.FromConfig(new Core.Catalog.CatalogConfig()).FaceReachMm);
            Assert.Equal(55.0, NodeDetectorOptions.FromConfig(new Core.Catalog.CatalogConfig { NodeFaceReachMm = 55 }).FaceReachMm);
            Assert.Equal(79.85, new NodeDetectorOptions().FaceReach(FaceCutNode()[1], FaceCutNode()[0]), 2);
        }

        [Fact]
        public void Detect_FaceCut_DoesNotSnapToParallelBarsNorAcrossAnOffset()
        {
            Vec3 P(double x, double z) => new Vec3(x, 0, z);
            double tan3 = Math.Tan(3.0 * Math.PI / 180.0);
            var bars = new List<DetectorBar>
            {
                // Dos tramos de cordón casi colineales (quiebro de 3°) con un salto de 20 mm: sus rectas se cortan a 380 mm, pero
                // con menos de 5° entre ejes no hay corte que valga; cada extremo se queda donde está (dos extremos sueltos).
                new DetectorBar(1, P(0, 0), P(5000, 0), SyntheticTruss.ChordType, SyntheticTruss.ChordDepth),
                new DetectorBar(2, P(5003, 20), P(10003, 20 + 5000 * tan3), SyntheticTruss.ChordType, SyntheticTruss.ChordDepth),
                // Montante 8,5 mm fuera del plano sobre el cordón 1 (los ejes no se cortan): su extremo se queda donde está.
                new DetectorBar(3, new Vec3(2000, 8.5, 0), new Vec3(2000, 8.5, 1500), SyntheticTruss.DiagonalType, SyntheticTruss.DiagonalDepth),
            };
            List<DetectedNode> nodes = NodeDetector.Detect(bars);
            List<BarEnd> ends = NodeDetector.EffectiveEnds(bars, new NodeDetectorOptions());
            BarEnd chordEnd = ends.Single(e => e.Bar.ElementId == 1 && e.RawMm.X == 5000);
            Assert.Null(chordEnd.CutWithElementId);
            Assert.Equal(0.0, chordEnd.GapMm, 6);
            Assert.Single(nodes, n => n.Status == NodeStatus.Untyped && n.MemberElementIds.SequenceEqual(new long[] { 1 }) && n.WorkPointMm.X == 5000);
            Assert.Single(nodes, n => n.Status == NodeStatus.Untyped && n.MemberElementIds.SequenceEqual(new long[] { 2 }) && n.WorkPointMm.X == 5003);

            BarEnd postEnd = ends.Single(e => e.Bar.ElementId == 3 && e.RawMm.Z == 0);
            Assert.Null(postEnd.CutWithElementId);
            Assert.Equal(0.0, postEnd.GapMm, 6);
            DetectedNode post = nodes.Single(n => n.AllElementIds.Contains(3) && n.WorkPointMm.Z == 0);
            Assert.Equal(NodeStatus.Untyped, post.Status);
        }

        [Fact]
        public void Detect_GapKJoint_GroupsBothDiagonalsOnTheSameChordWithinTheFaceReach()
        {
            Vec3 P(double x, double z) => new Vec3(x, 0, z);
            List<DetectorBar> Joint(double gapMm) => new List<DetectorBar>
            {
                new DetectorBar(1, P(0, 0), P(6000, 0), SyntheticTruss.ChordType, SyntheticTruss.ChordDepth),
                new DetectorBar(2, P(3000, 0), P(1000, 2000), SyntheticTruss.DiagonalType, SyntheticTruss.DiagonalDepth),
                new DetectorBar(3, P(3000 + gapMm, 0), P(5000 + gapMm, 2000), SyntheticTruss.DiagonalType, SyntheticTruss.DiagonalDepth),
            };
            // 34 mm de excentricidad (como en el cordón superior del Hangar): un solo nudo, con la segunda barra a más de 5 mm del origen del marco.
            DetectedNode joint = NodeDetector.Detect(Joint(34)).Single(n => n.ChordContinuous);
            Assert.Equal(2, joint.MemberElementIds.Count);
            Assert.Equal(3017.0, joint.WorkPointMm.X, 0.5);
            Assert.Single(joint.Members, m => !m.ReachesNode);
            // A 200 mm son dos nudos distintos del mismo cordón.
            Assert.Equal(2, NodeDetector.Detect(Joint(200)).Count(n => n.ChordContinuous && n.MemberElementIds.Count == 1));
        }

        [Fact]
        public void Detect_HangarTruss_FindsTheTenCentralNodesWithTheirThreeDiagonals()
        {
            List<DetectedNode> nodes = NodeDetector.Detect(HangarTruss.Bars());

            // 53 barras: 10 nudos del cordón central completos (cordón atravesando + 3 diagonales), 6 tríos de diagonales donde
            // el cordón no se seleccionó, 20 parejas en los cordones superior e inferior (tampoco seleccionados), el empalme
            // 1249515/1249516 (dos barras paralelas) y 16 extremos.
            Assert.Equal(53, nodes.Count);
            var central = nodes.Where(n => n.ChordContinuous && n.MemberElementIds.Count == 3).ToList();
            Assert.Equal(10, central.Count);
            Assert.All(central, n => Assert.Equal(NodeStatus.Detected, n.Status));
            Assert.All(central, n => Assert.Equal(17423.0, n.WorkPointMm.Z, 0.5));
            Assert.All(central, n => Assert.All(n.Members, m => Assert.True(m.ReachesNode)));
            Assert.Equal(6, nodes.Count(n => !n.ChordContinuous && n.AllElementIds.Count == 3));
            Assert.Equal(20, nodes.Count(n => !n.ChordContinuous && n.AllElementIds.Count == 2 && n.Status == NodeStatus.Detected));
            Assert.Single(nodes, n => n.AllElementIds.Count == 2 && n.Status == NodeStatus.Untyped && n.AllElementIds.Contains(1249515));
            Assert.Equal(16, nodes.Count(n => n.AllElementIds.Count == 1));
            Assert.DoesNotContain(nodes, n => n.Status == NodeStatus.Offset || n.Status == NodeStatus.AmbiguousChord);

            // El Detalle D: los ángulos de la Fase 7 (136,9 / 44,4 / −135,6) y lo que se queda corta cada diagonal.
            DetectedNode detalleD = central.Single(n => n.MemberElementIds.Contains(HangarTruss.DetalleDUpLeft));
            Assert.Equal(HangarTruss.DetalleDChord, detalleD.ChordElementId);
            Assert.Equal(new[] { HangarTruss.DetalleDUpLeft, HangarTruss.DetalleDUpRight, HangarTruss.DetalleDLower }, detalleD.MemberElementIds.OrderBy(i => i).ToArray());
            Assert.Equal(-11870.0, detalleD.WorkPointMm.X, 1.0);
            Assert.Equal(136.9, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDUpLeft).AngleDeg, 0.2);
            Assert.Equal(44.4, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDUpRight).AngleDeg, 0.2);
            Assert.Equal(-135.6, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDLower).AngleDeg, 0.2);
            Assert.Equal(84.5, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDUpLeft).EndGapMm, 1.0);
            Assert.Equal(19.6, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDUpRight).EndGapMm, 1.0);
            Assert.Equal(48.1, detalleD.Members.Single(m => m.ElementId == HangarTruss.DetalleDLower).EndGapMm, 1.0);
            Assert.Contains("2 +Y", detalleD.Signature());
            Assert.Contains("1 -Y", detalleD.Signature());

            // El nudo simétrico del mismo cordón (el de la Fase 7 en espejo): dos diagonales colineales que se encuentran 30 mm
            // por encima del eje y la tercera que arranca 4 mm por debajo.
            DetectedNode mirror = central.Single(n => n.MemberElementIds.Contains(HangarTruss.MirrorUpLeft));
            Assert.Equal(HangarTruss.DetalleDChord, mirror.ChordElementId);
            Assert.Equal(-6740.5, mirror.WorkPointMm.X, 1.0);
            Assert.Equal(135.0, mirror.Members.Single(m => m.ElementId == HangarTruss.MirrorUpLeft).AngleDeg, 0.3);
            Assert.Equal(44.4, mirror.Members.Single(m => m.ElementId == HangarTruss.MirrorUpRight).AngleDeg, 0.3);
            Assert.Equal(-44.4, mirror.Members.Single(m => m.ElementId == HangarTruss.MirrorLower).AngleDeg, 0.3);
            Assert.Contains("1 -Y", mirror.Signature());

            // Los nombres siguen la X global y son estables.
            Assert.Equal("N1", nodes[0].Name);
            Assert.Equal("N53", nodes[52].Name);
            Assert.Equal(nodes.Select(n => n.Name), NodeDetector.Detect(HangarTruss.Bars()).Select(n => n.Name));
        }

        [Fact]
        public void Detect_HangarTruss_WithTheOldRuleTheDiagonalsStayedLoose()
        {
            // Lo que pasó en el PC (resultados-fase-8.md, 8-4): sin alcance de cara (todo a 10 mm), 97 nudos y ninguno con
            // tres barras. Se conserva como prueba de regresión del diagnóstico.
            var raw = new NodeDetectorOptions { FaceReachMm = 0.001 };
            List<DetectedNode> nodes = NodeDetector.Detect(HangarTruss.Bars(), raw);
            Assert.Equal(97, nodes.Count);
            Assert.DoesNotContain(nodes, n => n.MemberElementIds.Count >= 2);
        }

        [Fact]
        public void Detect_HangarTruss8b_ReproducesTheFiftyNineNodesOfThePc()
        {
            // La selección de la ronda 8b (resultados-fase-8b.md, 8b-4): 56 barras con el cordón central entero y sin cordones
            // superior ni inferior, con los ejes reales que imprimió el sondeo 18. En el PC: 59 nudos, 16 con cordón atravesando y
            // tres diagonales, 20 parejas en K sin cordón y 23 untyped (14 extremos de tramos, el empalme 1250938/1250939 y 8
            // extremos lejanos de diagonales).
            List<DetectedNode> nodes = NodeDetector.Detect(HangarTruss8b.Bars());
            Assert.Equal(59, nodes.Count);
            var central = nodes.Where(n => n.ChordContinuous && n.MemberElementIds.Count == 3).ToList();
            Assert.Equal(16, central.Count);
            Assert.All(central, n => Assert.Equal(NodeStatus.Detected, n.Status));
            Assert.All(central, n => Assert.Equal(17423.0, n.WorkPointMm.Z, 0.5));
            Assert.All(central, n => Assert.All(n.Members, m => Assert.True(m.ReachesNode)));
            Assert.Equal(20, nodes.Count(n => !n.ChordContinuous && n.AllElementIds.Count == 2 && n.Status == NodeStatus.Detected));
            Assert.Equal(23, nodes.Count(n => n.Status == NodeStatus.Untyped));
            Assert.Single(nodes, n => n.Status == NodeStatus.Untyped && n.AllElementIds.Count == 2 && n.AllElementIds.Contains(HangarTruss8b.SpliceLeft));
            Assert.Equal(22, nodes.Count(n => n.AllElementIds.Count == 1));
            Assert.DoesNotContain(nodes, n => n.Status == NodeStatus.Offset || n.Status == NodeStatus.AmbiguousChord);

            // N4 del PC (el gemelo del Detalle D): 136,9 / 44,4 / −135,6 y lo que se queda corta cada diagonal hasta el punto de
            // trabajo (84,5 / 19,6 / 48,1 mm, lo mismo que HangarTruss). El sondeo 18 imprime 86,2 / 20,4 / 47,4 porque mide hasta
            // el corte de cada barra con el cordón, y el punto de trabajo es la media de los tres cortes (hasta 3,4 mm entre sí).
            DetectedNode n4 = central.Single(n => n.MemberElementIds.Contains(HangarTruss8b.DetalleDUpLeft));
            Assert.Equal("N4", n4.Name);
            Assert.Equal(HangarTruss8b.DetalleDChord, n4.ChordElementId);
            Assert.Equal(new[] { HangarTruss8b.DetalleDUpLeft, HangarTruss8b.DetalleDUpRight, HangarTruss8b.DetalleDLower }, n4.MemberElementIds.OrderBy(i => i).ToArray());
            Assert.Equal(-11870.0, n4.WorkPointMm.X, 1.0);
            Assert.Equal(136.9, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDUpLeft).AngleDeg, 0.2);
            Assert.Equal(44.4, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDUpRight).AngleDeg, 0.2);
            Assert.Equal(-135.6, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDLower).AngleDeg, 0.2);
            Assert.Equal(84.5, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDUpLeft).EndGapMm, 1.0);
            Assert.Equal(19.6, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDUpRight).EndGapMm, 1.0);
            Assert.Equal(48.1, n4.Members.Single(m => m.ElementId == HangarTruss8b.DetalleDLower).EndGapMm, 1.0);

            // N7 del PC: el nudo siguiente del mismo cordón, en espejo (135 / 44,4 / −44,4).
            DetectedNode n7 = central.Single(n => n.MemberElementIds.Contains(HangarTruss8b.MirrorUpLeft));
            Assert.Equal("N7", n7.Name);
            Assert.Equal(HangarTruss8b.DetalleDChord, n7.ChordElementId);
            Assert.Equal(-6740.5, n7.WorkPointMm.X, 1.0);
            Assert.Equal(135.0, n7.Members.Single(m => m.ElementId == HangarTruss8b.MirrorUpLeft).AngleDeg, 0.3);
            Assert.Equal(44.4, n7.Members.Single(m => m.ElementId == HangarTruss8b.MirrorUpRight).AngleDeg, 0.3);
            Assert.Equal(-44.4, n7.Members.Single(m => m.ElementId == HangarTruss8b.MirrorLower).AngleDeg, 0.3);

            // Los dos nudos del tramo HSS3X3 (N53 y N56 en el PC) y los nombres, estables y por la X global.
            Assert.Equal(2, central.Count(n => n.ChordElementId == HangarTruss8b.SmallChord));
            Assert.Equal("N1", nodes[0].Name);
            Assert.Equal("N59", nodes[58].Name);
            Assert.Equal(nodes.Select(n => n.Name), NodeDetector.Detect(HangarTruss8b.Bars()).Select(n => n.Name));
        }
    }
}
