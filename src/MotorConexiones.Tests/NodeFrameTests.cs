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
            // X × d con d = (1,0,1)/√2 da (0,-1,0)/√2; como su componente Z es 0 se elige signo con Y positiva.
            AssertVec(Vec3.UnitY, frame.Z);
            AssertVec(frame.Z.Cross(frame.X), frame.Y);
            AssertVec(new Vec3(0, 0, -1), frame.Y);
            Assert.Equal(0.0, frame.AxisDistanceMm, 6);
        }

        [Fact]
        public void VerticalTrussWithModelNoise_ChoosesZByPositiveY()
        {
            // Caso real de la Fase 1 (HANGAR_PRUEBA): cordón hacia -X con 2e-6 de desvío en Y, diagonal en el plano XZ.
            // Con tolerancia 1e-9 la normal salía (0, -1, 0); debe salir (0, +1, 0).
            var frame = NodeFrame.Compute(
                new Vec3(236570.5, -41238.5, 17423.0), new Vec3(226452.6, -41238.48, 17423.0),
                new Vec3(231605.2, -41238.3, 19960.1), new Vec3(234112.6, -41238.3, 17452.7));
            Assert.True(frame.Z.Y > 0.999, $"Z = {frame.Z}");
            Assert.True(frame.X.X < -0.999, $"X = {frame.X}");
            AssertVec(frame.Z.Cross(frame.X), frame.Y);
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
        public void ChordDirectionReversed_FlipsXButKeepsZUp()
        {
            var frame = NodeFrame.Compute(new Vec3(5000, 0, 0), new Vec3(0, 0, 0), new Vec3(2000, 0, 0), new Vec3(2500, 800, 0));
            AssertVec(-Vec3.UnitX, frame.X);
            AssertVec(Vec3.UnitZ, frame.Z);
            AssertVec(-Vec3.UnitY, frame.Y);
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
