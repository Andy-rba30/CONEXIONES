using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Fase 10, mejora C6: a partir de una barra, la selección asistida añade las que la tocan con la regla del detector
    /// (ronda 8b) y se queda en el plano de la cercha; una selección completa no añade nada.
    /// </summary>
    public class SelectionAssistTests
    {
        private static readonly long[] OutOfTruss = { SyntheticTruss.CrossingBar, SyntheticTruss.LooseBar, SyntheticTruss.OffsetPost };

        [Fact]
        public void Expand_FromOneDiagonal_AddsTheWholeSyntheticTruss()
        {
            List<DetectorBar> bars = SyntheticTruss.Bars();
            SelectionExpansion expansion = SelectionAssist.Expand(new[] { SyntheticTruss.UpLeft(0) }, bars);

            Assert.Equal(new[] { SyntheticTruss.UpLeft(0) }, expansion.OriginalIds);
            Assert.Empty(expansion.IgnoredIds);
            var expected = bars.Select(b => b.ElementId).Where(id => id != SyntheticTruss.UpLeft(0) && !OutOfTruss.Contains(id)).OrderBy(id => id).ToList();
            Assert.Equal(expected, expansion.AddedIds.OrderBy(id => id).ToList());
            // La primera ronda trae los dos cordones por los que pasa la diagonal (el central por abajo, el superior por arriba)
            // y las otras dos barras de su nudo (sus ejes se cortan en el punto de trabajo: se tocan como en el detector).
            Assert.Equal(new[] { SyntheticTruss.CentralChord, SyntheticTruss.UpperChord, SyntheticTruss.UpRight(0), SyntheticTruss.Lower(0) },
                expansion.Added.Where(a => a.Round == 1).Select(a => a.ElementId).OrderBy(id => id).ToArray());
            Assert.Equal(AddedBarReason.Chord, expansion.Added.Single(a => a.ElementId == SyntheticTruss.CentralChord).Reason);
            Assert.Equal(AddedBarReason.Chord, expansion.Added.Single(a => a.ElementId == SyntheticTruss.UpperChord).Reason);
            Assert.Equal(AddedBarReason.Member, expansion.Added.Single(a => a.ElementId == SyntheticTruss.UpRight(0)).Reason);
            Assert.Equal(3, expansion.ChordCount);
            Assert.Equal(expected.Count - 3, expansion.MemberCount);
            Assert.Equal(0, expansion.SpliceCount);
            Assert.NotNull(expansion.PlaneNormal);
            Assert.Equal(1.0, System.Math.Abs(expansion.PlaneNormal!.Value.Dot(Vec3.UnitY)), 3);
            Assert.False(expansion.LimitReached);
            // La barra que cruza en Y por el nudo de X = 3000 "pasa de largo" por el extremo de sus diagonales (es el caso
            // ambiguous_chord del plan), pero tiene los extremos a 1 m del plano de la cercha: fuera.
            Assert.Equal(new[] { SyntheticTruss.CrossingBar }, expansion.SkippedOutOfPlane);
            Assert.Equal("Se añadieron 15 barras que tocan la selección: 3 cordones que pasan de largo y 12 barras que llegan. 1 barra fuera del plano de la cercha no se añadió.", expansion.SummaryText());
            Assert.Equal(16, expansion.AllIds.Count);
            Assert.Equal(SyntheticTruss.UpLeft(0), expansion.AllIds[0]);
        }

        [Fact]
        public void Expand_FromTheChordAlone_VotesThePlaneAndAddsTheDiagonals()
        {
            List<DetectorBar> bars = SyntheticTruss.Bars();
            SelectionExpansion expansion = SelectionAssist.Expand(new[] { SyntheticTruss.CentralChord }, bars);

            // Con una sola barra no hay plano: lo votan las diez diagonales que llegan al cordón (plano XZ, normal Y).
            Assert.NotNull(expansion.PlaneNormal);
            Assert.Equal(1.0, System.Math.Abs(expansion.PlaneNormal!.Value.Dot(Vec3.UnitY)), 3);
            var expected = bars.Select(b => b.ElementId).Where(id => id != SyntheticTruss.CentralChord && !OutOfTruss.Contains(id)).OrderBy(id => id).ToList();
            Assert.Equal(expected, expansion.AddedIds.OrderBy(id => id).ToList());
            // La barra que cruza en Y (ambiguous_chord) se queda fuera del plano; la suelta no toca nada; el montante
            // desplazado 8,5 mm no se corta con el eje del cordón inferior (más de 5 mm).
            Assert.DoesNotContain(SyntheticTruss.CrossingBar, expansion.AddedIds);
            Assert.Equal(new[] { SyntheticTruss.CrossingBar }, expansion.SkippedOutOfPlane);
            Assert.DoesNotContain(SyntheticTruss.OffsetPost, expansion.AddedIds);
            Assert.Contains(SyntheticTruss.OffsetDiagonal, expansion.AddedIds);
            // Las once diagonales que terminan en el cordón (la de abajo del cuarto nudo se queda 85 mm corta: no) llegan en la
            // primera ronda; los cordones superior e inferior, en la segunda, como cordones.
            Assert.Equal(11, expansion.Added.Count(a => a.Round == 1));
            Assert.All(expansion.Added.Where(a => a.Round == 1), a => Assert.Equal(AddedBarReason.Member, a.Reason));
            Assert.Equal(AddedBarReason.Chord, expansion.Added.Single(a => a.ElementId == SyntheticTruss.UpperChord).Reason);
            Assert.Equal(AddedBarReason.Chord, expansion.Added.Single(a => a.ElementId == SyntheticTruss.LowerChord).Reason);
            Assert.True(expansion.Rounds >= 3);
        }

        [Fact]
        public void Expand_LeavesOutBarsOutOfThePlane()
        {
            List<DetectorBar> bars = SyntheticTruss.Bars();
            // Una riostra que llega al nudo de X = 1000 desde fuera del plano de la cercha (en Y): la toca, pero no es de la cercha.
            bars.Add(SyntheticTruss.Bar(900, SyntheticTruss.P(1000, SyntheticTruss.ChordZ), new Vec3(1000, SyntheticTruss.Y + 2500, SyntheticTruss.ChordZ + 1200), SyntheticTruss.DiagonalType));
            SelectionExpansion expansion = SelectionAssist.Expand(new[] { SyntheticTruss.UpLeft(0), SyntheticTruss.UpRight(0) }, bars);

            Assert.DoesNotContain(900, expansion.AddedIds);
            Assert.Equal(new[] { SyntheticTruss.CrossingBar, 900L }, expansion.SkippedOutOfPlane.OrderBy(id => id).ToArray());
            Assert.EndsWith("2 barras fuera del plano de la cercha no se añadieron.", expansion.SummaryText());
            Assert.Contains(SyntheticTruss.CentralChord, expansion.AddedIds);
        }

        [Fact]
        public void Expand_WithTheWholeTruss_AddsNothing()
        {
            List<DetectorBar> bars = SyntheticTruss.Bars();
            var all = bars.Select(b => b.ElementId).ToList();
            SelectionExpansion expansion = SelectionAssist.Expand(all, bars);

            Assert.Empty(expansion.Added);
            Assert.Equal(all, expansion.AllIds);
            Assert.Equal("La selección ya está completa: ninguna barra más la toca.", expansion.SummaryText());
        }

        [Fact]
        public void Expand_IgnoresIdsThatAreNotBarsAndSaysSo()
        {
            SelectionExpansion expansion = SelectionAssist.Expand(new long[] { 999999, 888888 }, SyntheticTruss.Bars());
            Assert.Empty(expansion.OriginalIds);
            Assert.Equal(new long[] { 999999, 888888 }, expansion.IgnoredIds);
            Assert.StartsWith("Ninguna de las barras seleccionadas es una barra de armazón estructural con eje.", expansion.SummaryText());

            SelectionExpansion mixed = SelectionAssist.Expand(new long[] { SyntheticTruss.CentralChord, 999999 }, SyntheticTruss.Bars());
            Assert.Equal(new[] { SyntheticTruss.CentralChord }, mixed.OriginalIds);
            Assert.EndsWith("1 elemento de la selección no es una barra con eje.", mixed.SummaryText());
        }

        [Fact]
        public void Expand_StopsAtTheLimitAndWarns()
        {
            SelectionExpansion expansion = SelectionAssist.Expand(new[] { SyntheticTruss.UpLeft(0) }, SyntheticTruss.Bars(), null, new SelectionAssistOptions { MaxBars = 4 });
            Assert.True(expansion.LimitReached);
            Assert.Equal(3, expansion.Added.Count);
            Assert.EndsWith("Se paró en el tope de barras: puede quedar cercha sin añadir (revisa la selección).", expansion.SummaryText());
        }

        [Fact]
        public void Expand_FromOneDiagonalOfTheHangar8bTruss_RecoversTheWholeSelectionAndTheSamePlan()
        {
            List<DetectorBar> bars = HangarTruss8b.Bars();
            SelectionExpansion expansion = SelectionAssist.Expand(new[] { HangarTruss8b.DetalleDUpLeft }, bars);

            // Las 56 barras de la 8b: las 48 diagonales y los 8 tramos del cordón central (por las diagonales que terminan en
            // ellos y, el último, por el empalme 1250938/1250939).
            Assert.Equal(56, expansion.AllIds.Count);
            Assert.Equal(bars.Select(b => b.ElementId).OrderBy(id => id), expansion.AllIds.OrderBy(id => id));
            Assert.Contains(expansion.Added, a => a.ElementId == HangarTruss8b.SmallChord && (a.Reason == AddedBarReason.Splice || a.Reason == AddedBarReason.Chord));
            Assert.Empty(expansion.SkippedOutOfPlane);
            Assert.False(expansion.LimitReached);
            Assert.Equal(1.0, System.Math.Abs(expansion.PlaneNormal!.Value.Dot(Vec3.UnitY)), 2);

            // El plan con la selección recuperada es el de siempre: 59 nudos y 16 listos.
            PlanRequest request = BatchPlanTests.Request(bars: bars);
            request.SelectionIds = expansion.AllIds;
            BatchPlan plan = PlanBuilder.Build(request);
            Assert.Equal(59, plan.Nodes.Count);
            Assert.Equal(16, plan.ReadyCount);
        }

        [Fact]
        public void Touches_FollowsTheDetectorRule()
        {
            var options = new NodeDetectorOptions();
            double minSin = System.Math.Sin(NodeDetectorOptions.MinCrossingAngleDeg * System.Math.PI / 180.0);
            List<DetectorBar> bars = SyntheticTruss.Bars();
            DetectorBar chord = bars.Single(b => b.ElementId == SyntheticTruss.CentralChord);
            DetectorBar diagonal = bars.Single(b => b.ElementId == SyntheticTruss.UpLeft(0));
            DetectorBar farDiagonal = bars.Single(b => b.ElementId == SyntheticTruss.Lower(3));
            DetectorBar loose = bars.Single(b => b.ElementId == SyntheticTruss.LooseBar);

            Assert.Equal(AddedBarReason.Chord, SelectionAssist.Touches(chord, diagonal, options, minSin));
            Assert.Equal(AddedBarReason.Member, SelectionAssist.Touches(diagonal, chord, options, minSin));
            Assert.Null(SelectionAssist.Touches(farDiagonal, chord, options, minSin));
            Assert.Null(SelectionAssist.Touches(loose, chord, options, minSin));
            Assert.Null(SelectionAssist.Touches(chord, chord, options, minSin));

            // Empalme: dos tramos paralelos pegados por los extremos.
            var left = new DetectorBar(1, new Vec3(0, 0, 0), new Vec3(5000, 0, 0), SyntheticTruss.ChordType, 76.2);
            var right = new DetectorBar(2, new Vec3(5000, 0, 0), new Vec3(9000, 0, 0), SyntheticTruss.ChordType, 76.2);
            var apart = new DetectorBar(3, new Vec3(5300, 0, 0), new Vec3(9000, 0, 0), SyntheticTruss.ChordType, 76.2);
            Assert.Equal(AddedBarReason.Splice, SelectionAssist.Touches(right, left, options, minSin));
            Assert.Null(SelectionAssist.Touches(apart, left, options, minSin));
        }
    }
}
