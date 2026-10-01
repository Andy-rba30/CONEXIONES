using MotorConexiones.Core.Geometry3D;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>Coordenadas reales del nudo de HANGAR_PRUEBA_sondeo.rvt (resultados-fase-3.md).</summary>
    public class NodeReachTests
    {
        private static readonly Vec3 WorkPoint = new Vec3(-11867.7, -17195.8, 17423.0);

        [Fact]
        public void ContinuousChord_WithWorkPointInsideSpan_Reaches()
        {
            // Cordón 1249510: 9960 mm, el nudo está a 2,5 m de un extremo y 7,4 m del otro. Falló en la ronda 3b.
            Assert.True(NodeReach.MemberReachesNode(new Vec3(-4437.3, -17195.7, 17423.0), new Vec3(-14397.6, -17195.8, 17423.0), WorkPoint));
        }

        [Fact]
        public void Diagonal_EndingNearNode_Reaches()
        {
            // Diagonal 1249630: su extremo queda a 86 mm del punto de trabajo.
            Assert.True(NodeReach.MemberReachesNode(new Vec3(-14536.8, -17195.8, 19918.8), new Vec3(-11930.6, -17195.8, 17481.8), WorkPoint));
        }

        [Fact]
        public void Member_OnSameAxisButFarAway_DoesNotReach()
        {
            // Misma recta que la diagonal, pero termina 1,2 m antes del nudo.
            var start = new Vec3(-14536.8, -17195.8, 19918.8);
            var dir = (new Vec3(-11930.6, -17195.8, 17481.8) - start).Normalized();
            var end = start + dir * (start.DistanceTo(new Vec3(-11930.6, -17195.8, 17481.8)) - 1200.0);
            Assert.False(NodeReach.MemberReachesNode(start, end, WorkPoint));
        }

        [Fact]
        public void Member_WhoseAxisMissesTheNode_DoesNotReach()
        {
            Assert.False(NodeReach.MemberReachesNode(new Vec3(-14536.8, -17195.8 + 12.0, 19918.8), new Vec3(-11930.6, -17195.8 + 12.0, 17481.8), WorkPoint));
            Assert.True(NodeReach.AxisDistanceMm(new Vec3(-14536.8, -17195.8 + 12.0, 19918.8), new Vec3(-11930.6, -17195.8 + 12.0, 17481.8), WorkPoint) > 11.0);
        }
    }
}
