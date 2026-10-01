using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using Xunit;

namespace MotorConexiones.Tests
{
    public class ConnectionGeometryTests
    {
        [Fact]
        public void GetGussetOutline_ExtractsPointsCorrectly()
        {
            var gusset = new GussetSpec
            {
                Outline = new GussetOutline
                {
                    Mode = "polygon",
                    PointsMm = new List<double[]>
                    {
                        new[] { -175.0, 280.0 },
                        new[] { 245.0, 280.0 },
                        new[] { 315.0, 210.0 }
                    }
                }
            };

            var outline = ConnectionGeometry.GetGussetOutline(gusset);
            Assert.Equal(3, outline.Count);
            Assert.Equal(-175.0, outline[0].X);
            Assert.Equal(280.0, outline[0].Y);
            Assert.Equal(245.0, outline[1].X);
            Assert.Equal(280.0, outline[1].Y);
            Assert.Equal(315.0, outline[2].X);
            Assert.Equal(210.0, outline[2].Y);
        }

        [Fact]
        public void ComputeKnifePlateCorners_CalculatesFourVerticesInOrder()
        {
            // Miembro orientado a lo largo de X positivo (ux=1, uy=0)
            double ux = 1.0;
            double uy = 0.0;
            double endSetback = 180.0;

            var plate = new KnifePlateSpec
            {
                LengthMm = 200.0,
                WidthMm = 100.0,
                InsertionMm = 80.0
            };

            // distStart = 180 - (200 - 80) = 60
            // distEnd = 180 + 80 = 260
            // halfWidth = 50
            // vx = -uy = 0, vy = ux = 1
            var corners = ConnectionGeometry.ComputeKnifePlateCorners(ux, uy, endSetback, plate);

            Assert.Equal(4, corners.Count);
            // Corner 0: (distStart*ux - halfWidth*vx, distStart*uy - halfWidth*vy) = (60, -50)
            Assert.Equal(60.0, corners[0].X, precision: 3);
            Assert.Equal(-50.0, corners[0].Y, precision: 3);

            // Corner 1: (distStart*ux + halfWidth*vx, distStart*uy + halfWidth*vy) = (60, 50)
            Assert.Equal(60.0, corners[1].X, precision: 3);
            Assert.Equal(50.0, corners[1].Y, precision: 3);

            // Corner 2: (distEnd*ux + halfWidth*vx, distEnd*uy + halfWidth*vy) = (260, 50)
            Assert.Equal(260.0, corners[2].X, precision: 3);
            Assert.Equal(50.0, corners[2].Y, precision: 3);

            // Corner 3: (distEnd*ux - halfWidth*vx, distEnd*uy - halfWidth*vy) = (260, -50)
            Assert.Equal(260.0, corners[3].X, precision: 3);
            Assert.Equal(-50.0, corners[3].Y, precision: 3);
        }

        [Fact]
        public void ComputeBoltPositions_CalculatesSingleLinePattern()
        {
            double ux = 1.0;
            double uy = 0.0;
            double endSetback = 180.0;

            var plate = new KnifePlateSpec
            {
                LengthMm = 200.0,
                WidthMm = 100.0,
                InsertionMm = 80.0
            };

            var bolts = new BoltPatternSpec
            {
                Rows = 2,
                Columns = 1,
                FirstRowFromPlateEndMm = 30.0,
                SpacingMm = 50.0
            };

            // distStart = 60
            // firstRowDist = 60 + 30 = 90
            // Row 0: along = 90
            // Row 1: along = 140
            var positions = ConnectionGeometry.ComputeBoltPositions(ux, uy, endSetback, plate, bolts);

            Assert.Equal(2, positions.Count);
            Assert.Equal(90.0, positions[0].X, precision: 3);
            Assert.Equal(0.0, positions[0].Y, precision: 3);

            Assert.Equal(140.0, positions[1].X, precision: 3);
            Assert.Equal(0.0, positions[1].Y, precision: 3);
        }

        [Fact]
        public void ComputeWeldLines_CalculatesWeldedSlotLines()
        {
            double ux = 1.0;
            double uy = 0.0;
            double endSetback = 180.0;

            var member = new MemberSpec
            {
                Attachment = new AttachmentSpec
                {
                    Type = "welded_slot",
                    SlotLengthMm = 150.0,
                    Weld = new WeldSpec
                    {
                        SizeMm = 6.0
                    }
                }
            };

            var welds = ConnectionGeometry.ComputeWeldLines(ux, uy, endSetback, member);

            Assert.Equal(2, welds.Count);
            // Weld 1: start (180, 32), end (330, 32), size 6.0
            Assert.Equal(180.0, welds[0].Start.X, precision: 3);
            Assert.Equal(32.0, welds[0].Start.Y, precision: 3);
            Assert.Equal(330.0, welds[0].End.X, precision: 3);
            Assert.Equal(32.0, welds[0].End.Y, precision: 3);
            Assert.Equal(6.0, welds[0].SizeMm);

            // Weld 2: start (180, -32), end (330, -32), size 6.0
            Assert.Equal(180.0, welds[1].Start.X, precision: 3);
            Assert.Equal(-32.0, welds[1].Start.Y, precision: 3);
            Assert.Equal(330.0, welds[1].End.X, precision: 3);
            Assert.Equal(-32.0, welds[1].End.Y, precision: 3);
            Assert.Equal(6.0, welds[1].SizeMm);
        }

        [Fact]
        public void GetMemberDirection2D_ReturnsUnitVectorPointingAwayFromWorkPoint()
        {
            var frame = NodeFrame.Compute(new Vec3(0, 0, 0), new Vec3(100, 0, 0), new Vec3(0, 0, 0), new Vec3(0, 100, 0));
            var workPoint = Vec3.Zero;
            var curveStart = new Vec3(10.0, 10.0, 0.0);
            var curveEnd = new Vec3(100.0, 100.0, 0.0);

            var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, curveStart, curveEnd, workPoint);

            // Direction should be (1/sqrt(2), 1/sqrt(2)) ~ (0.7071, 0.7071)
            double expected = 1.0 / Math.Sqrt(2.0);
            Assert.Equal(expected, ux, precision: 4);
            Assert.Equal(expected, uy, precision: 4);
        }
    }
}
