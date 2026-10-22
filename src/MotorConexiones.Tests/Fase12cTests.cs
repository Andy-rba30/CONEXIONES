using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Tests.Fakes;
using Xunit;

namespace MotorConexiones.Tests
{
    /// <summary>
    /// Ronda 12c: lo que el sondeo 27 (ronda 12b) dijo del perfil del cordón superior del Hangar, hecho regla: el tipo
    /// HSS12X8X1/2 guarda "Ht" = 304,8 y "b" = 203,2 y el ejemplar va girado 90°, así que en el plano de la cercha queda el 8"
    /// (<c>WidthMm</c>) y perpendicular el 12" (<c>HeightMm</c>); la huella del tubo en la plancha, el croquis y el arranque
    /// del empalme ya no adivinan con el mayor y el menor; y las soldaduras se encadenan para <c>WeldLine</c>.
    /// </summary>
    public class SectionOrientationTests
    {
        [Theory]
        [InlineData("HSS12X8X1/2", 304.8, 203.2, 12.7)]
        [InlineData("HSS8X4X1/2", 203.2, 101.6, 12.7)]
        [InlineData("HSS3X3X1/4", 76.2, 76.2, 6.35)]
        [InlineData("HSS2-1-2X2-1-2X3-16 64x64", 63.5, 63.5, 4.7625)]
        [InlineData("HSS2-1/2X2-1/2X3/16", 63.5, 63.5, 4.7625)]
        public void HssDesignation_ReadsDepthThenWidthThenWall(string typeName, double depth, double width, double wall)
        {
            Assert.True(HssDesignation.TryParseMm(typeName, out double d, out double w, out double t));
            Assert.Equal(depth, d, 1e-3);
            Assert.Equal(width, w, 1e-3);
            Assert.Equal(wall, t, 1e-3);
        }

        [Fact]
        public void HssDesignation_FallsBackToTheMetricSuffixAndRejectsOtherShapes()
        {
            Assert.True(HssDesignation.TryParseMm("HSS 64x64", out double d, out double w, out double t));
            Assert.Equal(64.0, d);
            Assert.Equal(64.0, w);
            Assert.Equal(0.0, t);
            Assert.True(HssDesignation.TryParseMm("HSS 100x50x4", out d, out w, out t));
            Assert.Equal(100.0, d);
            Assert.Equal(50.0, w);
            Assert.Equal(4.0, t);
            Assert.False(HssDesignation.TryParseMm("W12X26", out _, out _, out _));
            Assert.False(HssDesignation.TryParseMm("", out _, out _, out _));
            Assert.False(HssDesignation.TryParseMm(null, out _, out _, out _));
        }

        [Theory]
        [InlineData(90.0, 203.2, 304.8)]     // el cordón superior del Hangar: Cross-Section Rotation 90 (sondeo 27)
        [InlineData(-90.0, 203.2, 304.8)]
        [InlineData(270.0, 203.2, 304.8)]
        [InlineData(0.0, 304.8, 203.2)]      // sin girar: el canto nominal en el plano vertical de la barra
        [InlineData(180.0, 304.8, 203.2)]    // el cordón central y las diagonales del Hangar van a ±180: nada cambia
        [InlineData(-180.0, 304.8, 203.2)]
        [InlineData(30.0, 304.8, 203.2)]     // un giro que no es múltiplo de 90 se redondea al más cercano
        [InlineData(60.0, 203.2, 304.8)]
        [InlineData(double.NaN, 304.8, 203.2)]
        public void SectionOrientation_PutsTheNominalDepthInPlaneUnlessRotated(double rotation, double inPlane, double outOfPlane)
        {
            var (a, b) = SectionOrientation.Oriented(304.8, 203.2, rotation);
            Assert.Equal(inPlane, a, 1e-9);
            Assert.Equal(outOfPlane, b, 1e-9);
            Assert.Equal(inPlane != 304.8, SectionOrientation.IsRotated(rotation));
        }

        [Fact]
        public void SectionOrientation_IsHarmlessOnASquare()
        {
            Assert.Equal((63.5, 63.5), SectionOrientation.Oriented(63.5, 63.5, 90.0));
            Assert.Equal((76.2, 76.2), SectionOrientation.Oriented(76.2, 76.2, 180.0));
        }

        private static MemberModelFacts Chord(long id, double inPlaneMm, double outOfPlaneMm, string type = "HSS12X8X1/2") => new MemberModelFacts
        {
            ElementId = id,
            UniqueId = "f-" + id,
            FamilyName = "HSS-Hollow Structural Section",
            TypeName = type,
            WidthMm = inPlaneMm,
            HeightMm = outOfPlaneMm,
            ThicknessMm = 11.81,
            CurveStartMm = new Vec3(0, 0, 0),
            CurveEndMm = new Vec3(5000, 0, 0),
        };

        [Fact]
        public void Footprint_PutsTheOutOfPlaneSideAcrossThePlateAndTheInPlaneSideUp()
        {
            var (_, spec) = Fase12Fixtures.Load("detalle-C");
            // El Hangar (girado 90°): 203,2 en el plano, 304,8 perpendicular → 304,8 a lo ancho de la plancha, 203,2 a lo alto.
            TubeFootprint hangar = EndPlateValidator.Footprint(Chord(1, 203.2, 304.8), Chord(2, 203.2, 304.8), spec);
            Assert.Equal(304.8, hangar.WidthMm, 1e-9);
            Assert.Equal(203.2, hangar.HeightMm, 1e-9);
            // El mismo tubo sin girar (el 12" en el plano): la huella se da la vuelta; antes salía igual que la de arriba.
            TubeFootprint upright = EndPlateValidator.Footprint(Chord(1, 304.8, 203.2), Chord(2, 304.8, 203.2), spec);
            Assert.Equal(203.2, upright.WidthMm, 1e-9);
            Assert.Equal(304.8, upright.HeightMm, 1e-9);
            // Un solo hecho vale; sin hechos, el perfil escrito con el canto a lo ancho.
            Assert.Equal(304.8, EndPlateValidator.Footprint(null, Chord(2, 203.2, 304.8), spec).WidthMm, 1e-9);
            TubeFootprint fromProfile = EndPlateValidator.Footprint(null, null, spec);
            Assert.Equal(304.8, fromProfile.WidthMm, 1e-3);
            Assert.Equal(203.2, fromProfile.HeightMm, 1e-3);
            // Con la huella real del Hangar, los 12 pernos del C quedan a más de 37 mm de la cara del tubo (7.2.c de fase-12.md).
            foreach (BoltPosition b in BoltRing.Positions(spec.Plate, spec.Bolts))
            {
                Assert.InRange(hangar.DistanceTo(b.X, b.Y), 37.0, 40.0);
            }
        }

        [Fact]
        public void EndPlateSketch_DrawsTheTubeWithTheOutOfPlaneSideAcross()
        {
            var (_, spec) = Fase12Fixtures.Load("detalle-C");
            var members = new List<SketchMemberInfo> { new SketchMemberInfo(1245531, 1, 0, 203.2, "HSS12X8X1/2", 0, false, 0.0, 304.8) };
            var hangar = new SketchNodeInfo(203.2, "HSS12X8X1/2", members, null, null) { ChordDepthMm = 304.8 };
            SketchPolygon tube = EndPlateSketch.Build(spec, hangar).Polygons.Single(p => p.Kind == SketchKind.ChordEdge);
            Assert.Equal(304.8, tube.Points.Max(p => p.X) - tube.Points.Min(p => p.X), 1e-9);
            Assert.Equal(203.2, tube.Points.Max(p => p.Y) - tube.Points.Min(p => p.Y), 1e-9);
            var upright = new SketchNodeInfo(304.8, "HSS12X8X1/2", members, null, null) { ChordDepthMm = 203.2 };
            tube = EndPlateSketch.Build(spec, upright).Polygons.Single(p => p.Kind == SketchKind.ChordEdge);
            Assert.Equal(203.2, tube.Points.Max(p => p.X) - tube.Points.Min(p => p.X), 1e-9);
            Assert.Equal(304.8, tube.Points.Max(p => p.Y) - tube.Points.Min(p => p.Y), 1e-9);
        }

        [Fact]
        public void Starter_ForSplice_ScalesThePlateWithTheOutOfPlaneSideAcross()
        {
            PanelChoices choices = Fase11Fixtures.RepoChoices();
            // Un HSS10X6X1/2 (sin detalle en choices.json) girado como el cordón del Hangar: 152,4 en el plano, 254 perpendicular.
            var rotated = new FakeModelFacts();
            rotated.Members.Clear();
            rotated.Members[1] = Chord(1, 152.4, 254.0, "HSS10X6X1/2");
            rotated.Members[2] = Chord(2, 152.4, 254.0, "HSS10X6X1/2");
            rotated.Members[2].CurveStartMm = new Vec3(5000, 0, 0);
            rotated.Members[2].CurveEndMm = new Vec3(10000, 0, 0);
            StarterResult wide = SpecStarter.ForSplice(TemplateNode.FromModelFacts(rotated, 1, new long[] { 2 }), choices);
            Assert.Equal("I escalado", wide.DetailName);
            Assert.Equal(380.0, wide.Spec.Plate!.WidthMm);   // 254 + 2 × (37 + 25) = 378 → 380 a lo ancho
            Assert.Equal(280.0, wide.Spec.Plate.HeightMm);   // 152,4 + 124 = 276,4 → 280 a lo alto
            // El mismo tubo sin girar: la plancha se da la vuelta (antes salía 380 × 280 en los dos casos).
            var upright = new FakeModelFacts();
            upright.Members.Clear();
            upright.Members[1] = Chord(1, 254.0, 152.4, "HSS10X6X1/2");
            upright.Members[2] = Chord(2, 254.0, 152.4, "HSS10X6X1/2");
            upright.Members[2].CurveStartMm = new Vec3(5000, 0, 0);
            upright.Members[2].CurveEndMm = new Vec3(10000, 0, 0);
            StarterResult tall = SpecStarter.ForSplice(TemplateNode.FromModelFacts(upright, 1, new long[] { 2 }), choices);
            Assert.Equal(280.0, tall.Spec.Plate!.WidthMm);
            Assert.Equal(380.0, tall.Spec.Plate.HeightMm);
            // El Hangar de siempre no cambia: Detalle C por el nombre del perfil, 430 × 330.
            StarterResult c = SpecStarter.ForSplice(TemplateNode.FromModelFacts(HangarTruss8c.Facts(), HangarTruss8c.FirstSpliceLeft, new[] { HangarTruss8c.FirstSpliceRight }), choices);
            Assert.Equal(430.0, c.Spec.Plate!.WidthMm);
            Assert.Equal(330.0, c.Spec.Plate.HeightMm);
        }

        [Fact]
        public void TheSyntheticHangarFacts_AlreadyFollowTheConvention()
        {
            // El fixture K y el simulador ya daban 203,2 en el plano y 304,8 perpendicular: el add-in se alinea con ellos.
            MemberModelFacts chord = KNode55.Facts().GetMemberFacts(KNode55.Chord)!;
            Assert.Equal(203.2, chord.WidthMm, 1e-6);
            Assert.Equal(304.8, chord.HeightMm, 1e-6);
            Assert.Equal(0.0, chord.SectionRotationDeg);
            Assert.Equal(152.4, ValidationCommon.ChordDepthMm(new ConnectionSpec { Chord = new ChordSpec { ElementId = KNode55.Chord } }, KNode55.Facts()) / 2.0, 1e-6);
        }
    }

    /// <summary>Ronda 12c: las cadenas de segmentos que se convierten en <c>WeldLine</c> de Advance Steel.</summary>
    public class WeldChainTests
    {
        private static WeldLine2D Segment(double x1, double y1, double x2, double y2, double size = 5.0) =>
            new WeldLine2D(new BoltPosition(x1, y1), new BoltPosition(x2, y2), size);

        [Fact]
        public void AClosedContour_IsOneClosedChainWithoutTheRepeatedPoint()
        {
            double hw = 152.4, hh = 101.6;
            var contour = new List<WeldLine2D>
            {
                Segment(-hw, -hh, hw, -hh, 6), Segment(hw, -hh, hw, hh, 6), Segment(hw, hh, -hw, hh, 6), Segment(-hw, hh, -hw, -hh, 6),
            };
            WeldChain chain = Assert.Single(WeldChain.Build(contour));
            Assert.True(chain.IsClosed);
            Assert.Equal(4, chain.Points.Count);
            Assert.Equal(4, chain.Segments.Count);
            Assert.Equal(6.0, chain.SizeMm);
            Assert.Equal(-hw, chain.Points[0].X);
            Assert.Equal(-hw, chain.Points[3].X);
            Assert.Equal(hh, chain.Points[3].Y);
        }

        [Fact]
        public void TwoSlotSides_AreTwoOpenChains_AndASingleLineIsOne()
        {
            var slot = new List<WeldLine2D> { Segment(0, 32, 150, 32), Segment(0, -32, 150, -32) };
            List<WeldChain> chains = WeldChain.Build(slot);
            Assert.Equal(2, chains.Count);
            Assert.All(chains, c => Assert.False(c.IsClosed));
            Assert.All(chains, c => Assert.Equal(2, c.Points.Count));
            WeldChain one = Assert.Single(WeldChain.Build(new[] { Segment(-45, -101.6, 45, -101.6) }));
            Assert.False(one.IsClosed);
            Assert.Equal(90.0, one.Points[1].X - one.Points[0].X, 1e-9);
        }

        [Fact]
        public void ReversedSegments_JoinByEitherEnd_AndTinyOnesAreDropped()
        {
            // El segundo segmento viene al revés y el tercero vuelve al principio: una cadena cerrada de tres lados.
            var triangle = new List<WeldLine2D> { Segment(0, 0, 100, 0), Segment(100, 80, 100, 0), Segment(100, 80, 0, 0), Segment(0, 0, 0.2, 0) };
            WeldChain chain = Assert.Single(WeldChain.Build(triangle));
            Assert.True(chain.IsClosed);
            Assert.Equal(3, chain.Points.Count);
            Assert.Equal(3, chain.Segments.Count);
            Assert.Empty(WeldChain.Build(null));
            Assert.Empty(WeldChain.Build(new List<WeldLine2D>()));
        }
    }
}
