using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Cierre de la Fase 13 (0.13.1): el registro del corte con los extremos exactos en pies (compatible con los registros de la
    /// 0.13.0) y la garantía del Core de que el punto de corte que llega al add-in está sobre el eje de la barra aunque el marco
    /// del nudo lo haya dado fuera de su plano (la diagonal 1249632 del Hangar: 0,011 mm en Y).
    /// </summary>
    public class Fase13CierreTests
    {
        [Fact]
        public void SplitRecord_Of_0_13_0_WithoutFeet_StillLoads()
        {
            const string json = "{\"split_id\":\"02116c8a-c904-4cbf-8f5a-a3f4d60e3513\",\"element_id\":1249632,\"new_element_id\":1322620,"
                + "\"chord_element_id\":1245533,\"original_start_mm\":[-9277.8,-17195.8,19960.1],\"original_end_mm\":[-6770.4,-17195.8,17452.7],"
                + "\"split_point_mm\":[-8909,-17195.8,19591.3],\"offset_from_chord_face_mm\":330,\"split_distance_mm\":483.2,"
                + "\"original_extensions_feet\":{},\"mechanism\":\"copy_element\",\"approximated_parameters\":[],\"connection_id\":null,"
                + "\"batch_id\":null,\"created_utc\":\"2026-10-31T15:50:24.6139035Z\"}";
            SplitRecord? record = SplitRecord.FromJson(json);
            Assert.NotNull(record);
            Assert.Null(record!.OriginalStartFeet);
            Assert.Null(record.OriginalEndFeet);
            Assert.Equal(1322620, record.NewElementId);
            Assert.Equal(-17195.8, record.OriginalStartMm[1]);
        }

        [Fact]
        public void SplitRecord_RoundTrips_TheExactFeet()
        {
            var record = new SplitRecord
            {
                ElementId = 1249632,
                NewElementId = 1322620,
                OriginalStartFeet = new[] { -30.438976377952755, -56.41666666666667, 65.48589238845144 },
                OriginalEndFeet = new[] { -22.212598425196852, -56.41666666666667, 57.25948162729659 },
            };
            string json = record.ToJson();
            Assert.Contains("\"original_start_feet\"", json);
            SplitRecord? back = SplitRecord.FromJson(json);
            Assert.NotNull(back);
            Assert.Equal(record.OriginalStartFeet, back!.OriginalStartFeet);
            Assert.Equal(record.OriginalEndFeet, back.OriginalEndFeet);
        }

        [Fact]
        public void Segments_PutTheSplitPointOnTheBarAxis_EvenIfTheFramePointIsOffPlane()
        {
            var bar = new MemberModelFacts { ElementId = 1249632, CurveStartMm = new Vec3(-9277.8, -17195.8, 19960.1), CurveEndMm = new Vec3(-6770.4, -17195.8, 17452.7) };
            // Lo que dio el marco del nudo en Revit el 2026-10-31: el punto de corte 0,011 mm fuera del plano de la diagonal.
            var fromFrame = new SplitPointResult(1249632, 143.7, 473.7, 483.2, new Vec3(-9250.7, -17195.8, 19933.0), new Vec3(-8909.0, -17195.789, 19591.3), new Vec3(0.7071, 0.0, -0.7071), 45.0);
            var (shortStart, shortEnd, longStart, longEnd) = SplitPoint.Segments(bar, fromFrame);
            Assert.Equal(0, SplitPoint.NearEndIndex(bar, fromFrame.CutPointMm));
            Assert.True(shortEnd.Equals(longStart), "los dos tramos comparten el punto de corte");
            Assert.Equal(bar.CurveStartMm, shortStart);
            Assert.Equal(bar.CurveEndMm, longEnd);
            Vec3 axis = (bar.CurveEndMm - bar.CurveStartMm).Normalized();
            Vec3 toSplit = shortEnd - bar.CurveStartMm;
            double offAxis = (toSplit - axis * toSplit.Dot(axis)).Length;
            Assert.True(offAxis < 1e-9, "el punto de corte queda a " + offAxis + " mm del eje");
            Assert.Equal(-17195.8, shortEnd.Y, 9);
            Assert.InRange(shortStart.DistanceTo(shortEnd), 521.0, 522.2);
        }
    }
}
