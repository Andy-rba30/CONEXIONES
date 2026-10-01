using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using Xunit;

namespace MotorConexiones.Tests
{
    public class NodeFrameTests
    {
        private static void AssertVec(Vec3 expected, Vec3 actual, double tolerance = 1e-9)
        {
            Assert.True(expected.DistanceTo(actual) < tolerance, $"esperado {expected}, obtenido {actual}");
        }

        [Fact]
        public void HorizontalChordWithDiagonalInVerticalPlane_GivesAxesOfSection7()
        {
            // Cordón sobre el eje X global, diagonal a 45° dentro del plano XZ: la cercha es vertical.
            var chordStart = new Vec3(0, 0, 0);
            var chordEnd = new Vec3(6000, 0, 0);
            var memberStart = new Vec3(1000, 0, 0);
            var memberEnd = new Vec3(2000, 0, 1000);

            var frame = NodeFrame.Compute(chordStart, chordEnd, memberStart, memberEnd);

            AssertVec(new Vec3(1000, 0, 0), frame.Origin);
            AssertVec(Vec3.UnitX, frame.X);
            // Marco canónico (Fase 7): en una cercha vertical +Y local apunta hacia arriba (+Z global) y Z = X × Y.
            AssertVec(Vec3.UnitZ, frame.Y);
            AssertVec(frame.X.Cross(frame.Y), frame.Z);
            AssertVec(new Vec3(0, -1, 0), frame.Z);
            Assert.False(frame.ChordReversed);
            Assert.Equal(0.0, frame.AxisDistanceMm, 6);
        }

        [Fact]
        public void VerticalTrussWithModelNoise_ChoosesZByPositiveY()
        {
            // Caso real de la Fase 1 (HANGAR_PRUEBA): cordón dibujado hacia -X con 2e-6 de desvío en Y, diagonal en el
            // plano XZ. Marco canónico: X hacia +X global (el cordón está "al revés"), Y hacia arriba, Z = X × Y = -Y global.
            var frame = NodeFrame.Compute(
                new Vec3(236570.5, -41238.5, 17423.0), new Vec3(226452.6, -41238.48, 17423.0),
                new Vec3(231605.2, -41238.3, 19960.1), new Vec3(234112.6, -41238.3, 17452.7));
            Assert.True(frame.X.X > 0.999, $"X = {frame.X}");
            Assert.True(frame.Y.Z > 0.999, $"Y = {frame.Y}");
            Assert.True(frame.Z.Y < -0.999, $"Z = {frame.Z}");
            Assert.True(frame.ChordReversed);
            AssertVec(frame.X.Cross(frame.Y), frame.Z);
            Assert.True(frame.AxisDistanceMm < 1.0);
        }

        [Fact]
        public void HorizontalTruss_ZPointsUp()
        {
            // Cercha horizontal (plano XY): Z debe apuntar hacia arriba (componente global Z positiva).
            var frame = NodeFrame.Compute(new Vec3(0, 0, 3000), new Vec3(5000, 0, 3000), new Vec3(2000, 0, 3000), new Vec3(2500, -800, 3000));
            AssertVec(Vec3.UnitZ, frame.Z);
            AssertVec(Vec3.UnitX, frame.X);
            AssertVec(Vec3.UnitY, frame.Y);
            AssertVec(new Vec3(2000, 0, 3000), frame.Origin);
        }

        [Fact]
        public void ChordDirectionReversed_GivesTheSameCanonicalFrame()
        {
            // El mismo nudo con el cordón dibujado en los dos sentidos: el marco canónico no cambia (solo ChordReversed).
            var forward = NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(5000, 0, 0), new Vec3(2000, 0, 0), new Vec3(2500, 800, 0));
            var backward = NodeFrame.Compute(new Vec3(5000, 0, 0), new Vec3(0, 0, 0), new Vec3(2000, 0, 0), new Vec3(2500, 800, 0));
            AssertVec(Vec3.UnitX, backward.X);
            AssertVec(Vec3.UnitY, backward.Y);
            AssertVec(Vec3.UnitZ, backward.Z);
            AssertVec(forward.X, backward.X);
            AssertVec(forward.Y, backward.Y);
            AssertVec(forward.Z, backward.Z);
            AssertVec(forward.Origin, backward.Origin);
            Assert.False(forward.ChordReversed);
            Assert.True(backward.ChordReversed);
        }

        [Fact]
        public void HangarNode_CanonicalFrameMirrorsXRelativeToPhases3To6()
        {
            // Nudo del Detalle D en el Hangar (resultados de la Fase 3): cordón hacia -X global. Hasta la Fase 6 el marco
            // era X = -X global, Y = +Z, Z = +Y; ahora X = +X global, Y = +Z, Z = -Y global.
            var frame = NodeFrame.Compute(
                new Vec3(-4437.3, -17195.7, 17423.0), new Vec3(-14397.6, -17195.8, 17423.0),
                new Vec3(-14536.8, -17195.8, 19918.8), new Vec3(-11930.6, -17195.8, 17481.8));
            Assert.True(frame.X.X > 0.999, $"X = {frame.X}");
            Assert.True(frame.Y.Z > 0.999, $"Y = {frame.Y}");
            Assert.True(frame.Z.Y < -0.999, $"Z = {frame.Z}");
            Assert.True(frame.ChordReversed);
            Assert.Equal(-11867.7, frame.Origin.X, 0);
        }

        [Theory]
        [InlineData(1.0, 1.0, 45.0)]
        [InlineData(-1.0, 1.0, 135.0)]
        [InlineData(-1.0, -1.0, -135.0)]
        [InlineData(1.0, -1.0, -45.0)]
        [InlineData(0.0, 1.0, 90.0)]
        [InlineData(-1.0, 0.0, -180.0)]
        public void SignedAngle_IsMeasuredFromPlusXInMinus180To180(double ux, double uy, double expected)
        {
            Assert.Equal(expected, NodeFrame.SignedAngleDeg(ux, uy), 6);
        }

        [Theory]
        [InlineData(45.0, 45.0)]
        [InlineData(135.0, 45.0)]
        [InlineData(-45.0, 45.0)]
        [InlineData(-135.6, 44.4)]
        [InlineData(90.0, 90.0)]
        [InlineData(-180.0, 0.0)]
        [InlineData(370.0, 10.0)]
        public void AngleToChord_FoldsToTheInclinationADrawingWrites(double signed, double expected)
        {
            Assert.Equal(expected, NodeFrame.AngleToChordDeg(signed), 6);
        }

        [Fact]
        public void AngleDifference_TakesTheShortWayAround()
        {
            Assert.Equal(10.0, NodeFrame.AngleDifferenceDeg(-175.0, 175.0), 6);
            Assert.Equal(0.0, NodeFrame.AngleDifferenceDeg(180.0, -180.0), 6);
            Assert.Equal(90.0, NodeFrame.AngleDifferenceDeg(45.0, -45.0), 6);
        }

        [Fact]
        public void AxesNotIntersecting_ThrowsWithDistance()
        {
            // El miembro pasa 12 mm por encima del cordón.
            var error = Assert.Throws<NodeGeometryException>(() =>
                NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(6000, 0, 0), new Vec3(1000, 0, 12), new Vec3(1000, 1000, 12)));
            Assert.Equal(ErrorCodes.NodeAxesNotIntersecting, error.Code);
            Assert.Contains("12.0 mm", error.Message);
        }

        [Fact]
        public void AxesWithinFiveMillimeters_UsesMidpoint()
        {
            var frame = NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(6000, 0, 0), new Vec3(1000, 0, 4), new Vec3(1000, 1000, 4));
            AssertVec(new Vec3(1000, 0, 2), frame.Origin);
            Assert.Equal(4.0, frame.AxisDistanceMm, 6);
        }

        [Fact]
        public void ParallelAxes_Throw()
        {
            var error = Assert.Throws<NodeGeometryException>(() =>
                NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(6000, 0, 0), new Vec3(0, 100, 0), new Vec3(6000, 100, 0)));
            Assert.Equal(ErrorCodes.NodeAxesParallel, error.Code);
        }

        [Fact]
        public void ToLocalAndToGlobal_AreInverse()
        {
            var frame = NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(6000, 0, 0), new Vec3(1000, 0, 0), new Vec3(2000, 0, 1000));
            var global = frame.ToGlobal(250, -130, 4.7625);
            var local = frame.ToLocal(global);
            AssertVec(new Vec3(250, -130, 4.7625), local, 1e-9);
        }
    }
}
