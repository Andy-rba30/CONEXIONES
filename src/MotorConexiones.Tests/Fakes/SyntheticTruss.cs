using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Tests.Fakes
{
    /// <summary>
    /// Cercha sintética para probar la detección de nudos y el plan sin Revit (Fase 8). Está en el plano XZ como la del
    /// Hangar (Y = −17195,8) y todas las coordenadas van en mm:
    /// <list type="bullet">
    /// <item>Cordón central (ID 100) de X = 0 a 8000 en Z = 17423, con cuatro nudos del tipo del Detalle D en X = 1000,
    /// 3000, 5000 y 7000: dos diagonales arriba (135° y 45°) y una abajo. En la mitad izquierda la de abajo va a
    /// −135° (como la plantilla); en la derecha, a −45° (nudo en espejo, <c>mirror_x</c>).</item>
    /// <item>Cordón superior (ID 101) en Z = 18837 dibujado <b>al revés</b> (de X = 8700 a −700) y cordón inferior (ID 102)
    /// en Z = 16009: en sus nudos solo llega una diagonal (sin encaje con una plantilla de tres barras). Sobresalen 286 mm
    /// de los últimos nudos: más que el alcance de cara, así que atraviesan (un cordón que sobresaliera menos contaría
    /// como barra que llega: extremo de cercha).</item>
    /// <item>En el nudo de X = 3000 cruza además una barra en Y (ID 150): dos barras atraviesan → <c>ambiguous_chord</c>.</item>
    /// <item>En el nudo de X = 5000 la diagonal inferior termina 6 mm por encima del eje (se agrupa igual).</item>
    /// <item>En el nudo de X = 7000 la diagonal inferior se queda 120 mm corta (85 mm del eje del cordón, más que el
    /// alcance de cara de la ronda 8b: 38,1 + 31,75 + 10 = 79,85 mm): no se agrupa (queda suelta) y el nudo tiene solo
    /// dos barras.</item>
    /// <item>Barra suelta lejos de todo (ID 160) y un nudo con desfase en el cordón inferior (X = 4000): un montante
    /// (ID 170) a 8,5 mm fuera del plano y una diagonal (ID 171) a 0,5 mm al otro lado se agrupan, el cordón pasa a 4 mm
    /// del centro pero el montante dista 8,5 mm del eje del cordón → <c>offset</c>.</item>
    /// </list>
    /// </summary>
    public static class SyntheticTruss
    {
        public const double Y = -17195.8;
        public const double ChordZ = 17423.0;
        public const double UpperZ = ChordZ + 1414.0;
        public const double LowerZ = ChordZ - 1414.0;

        public const long CentralChord = 100;
        public const long UpperChord = 101;
        public const long LowerChord = 102;
        public const long CrossingBar = 150;
        public const long LooseBar = 160;
        public const long OffsetPost = 170;
        public const long OffsetDiagonal = 171;

        /// <summary>X de los cuatro nudos del cordón central.</summary>
        public static readonly double[] NodeX = { 1000.0, 3000.0, 5000.0, 7000.0 };

        /// <summary>IDs de las barras de cada nudo central: arriba-izquierda, arriba-derecha, abajo (en ese orden).</summary>
        public static long UpLeft(int node) => 200 + node * 10;
        public static long UpRight(int node) => 201 + node * 10;
        public static long Lower(int node) => 202 + node * 10;

        public const string ChordType = "HSS3X3X1/4";
        public const string DiagonalType = "HSS2-1-2X2-1-2X3-16 64x64";

        /// <summary>Canto del perfil (como lo lee <c>RevitModelFacts</c>): HSS3X3 = 76,2 mm; HSS 2-1/2 = 63,5 mm.</summary>
        public const double ChordDepth = 76.2;
        public const double DiagonalDepth = 63.5;

        /// <summary>Barra con el canto que corresponde a su tipo.</summary>
        public static DetectorBar Bar(long id, Vec3 start, Vec3 end, string type) =>
            new DetectorBar(id, start, end, type, type == ChordType ? ChordDepth : DiagonalDepth);

        public static List<DetectorBar> Bars()
        {
            var bars = new List<DetectorBar>
            {
                Bar(CentralChord, P(0, ChordZ), P(8000, ChordZ), ChordType),
                Bar(UpperChord, P(8700, UpperZ), P(-700, UpperZ), ChordType),
                Bar(LowerChord, P(-700, LowerZ), P(8700, LowerZ), ChordType),
                Bar(CrossingBar, new Vec3(3000, Y - 1000, ChordZ), new Vec3(3000, Y + 1000, ChordZ), ChordType),
                Bar(LooseBar, new Vec3(20000, Y, 0), new Vec3(21000, Y, 500), DiagonalType),
                Bar(OffsetPost, new Vec3(4000, Y + 8.5, LowerZ), new Vec3(4000, Y + 8.5, LowerZ + 900), DiagonalType),
                Bar(OffsetDiagonal, new Vec3(4000, Y - 0.5, LowerZ), new Vec3(4800, Y - 0.5, LowerZ + 800), DiagonalType),
            };
            for (int i = 0; i < NodeX.Length; i++)
            {
                double x = NodeX[i];
                bars.Add(Bar(UpLeft(i), P(x, ChordZ), P(x - 1414, UpperZ), DiagonalType));
                bars.Add(Bar(UpRight(i), P(x, ChordZ), P(x + 1414, UpperZ), DiagonalType));
                bool left = i < 2;
                double lowerX = left ? x - 1414 : x + 1414;
                Vec3 end = P(x, ChordZ);
                if (i == 2) end = P(x, ChordZ + 6.0);        // 6 mm: se agrupa
                if (i == 3) end = P(x + 84.85, ChordZ - 84.85); // 120 mm a lo largo de la diagonal (85 mm del eje): no se agrupa
                bars.Add(Bar(Lower(i), P(lowerX, LowerZ), end, DiagonalType));
            }
            return bars;
        }

        public static Vec3 P(double x, double z) => new Vec3(x, Y, z);

        /// <summary>Hechos del modelo para la cercha sintética.</summary>
        public static SyntheticTrussFacts Facts(IEnumerable<DetectorBar>? bars = null) => new SyntheticTrussFacts(bars ?? Bars());
    }

    /// <summary>
    /// <see cref="IModelFacts"/> sobre una lista de barras (la cercha sintética o la del Hangar de <see cref="HangarTruss"/>).
    /// Como <c>RevitModelFacts</c>, el ángulo con signo de cada barra se mide respecto al marco del nudo que se le pase con
    /// <see cref="WithFrame"/> (hacia fuera desde el punto de trabajo); sin marco, es la pendiente respecto a la horizontal.
    /// Ancho y alto de la sección = el canto de la barra (<c>DetectorBar.DepthMm</c>).
    /// </summary>
    public sealed class SyntheticTrussFacts : IModelFacts
    {
        private readonly Dictionary<long, DetectorBar> _bars;
        private readonly NodeFrame? _frame;

        public SyntheticTrussFacts(IEnumerable<DetectorBar> bars, NodeFrame? frame = null)
        {
            _bars = bars.ToDictionary(b => b.ElementId, b => b);
            _frame = frame;
        }

        public string ProjectUniqueId => "synthetic-truss-0000-0000-0000-000000000001";

        public SyntheticTrussFacts WithFrame(NodeFrame frame) => new SyntheticTrussFacts(_bars.Values, frame);

        public bool ElementExists(long elementId) => _bars.ContainsKey(elementId);

        public bool IsStructuralMember(long elementId) => _bars.ContainsKey(elementId);

        public MemberModelFacts? GetMemberFacts(long elementId)
        {
            if (!_bars.TryGetValue(elementId, out DetectorBar? bar)) return null;
            bool isChord = bar.TypeName == SyntheticTruss.ChordType || bar.DepthMm > 70;
            double depth = bar.DepthMm > 0 ? bar.DepthMm : (isChord ? SyntheticTruss.ChordDepth : SyntheticTruss.DiagonalDepth);
            double angle;
            bool reaches = true;
            if (_frame != null)
            {
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(_frame, bar.StartMm, bar.EndMm, _frame.Origin);
                angle = NodeFrame.SignedAngleDeg(ux, uy);
                reaches = NodeReach.MemberReachesNode(bar.StartMm, bar.EndMm, _frame.Origin);
            }
            else
            {
                angle = bar.SlopeDeg;
            }
            return new MemberModelFacts
            {
                ElementId = elementId,
                UniqueId = "synthetic-" + elementId,
                FamilyName = isChord ? "HSS-Square" : "HSS-Square-64x64",
                TypeName = bar.TypeName ?? string.Empty,
                WidthMm = depth,
                HeightMm = depth,
                ThicknessMm = isChord ? 6.35 : 4.76,
                CurveStartMm = bar.StartMm,
                CurveEndMm = bar.EndMm,
                AngleInPlaneDeg = Math.Round(angle, 1),
                ConnectsToNode = reaches,
            };
        }

        public IReadOnlyList<string> GetAvailableProfileNames() => new[]
        {
            SyntheticTruss.ChordType, SyntheticTruss.DiagonalType, "HSS2-1/2X2-1/2X3/16", "HSS4X4X3/8", HangarTruss.BigChordType, "W12X26",
        };

        public bool CheckClashWithForeignMember(long foreignMemberId, Vec3 startMm, Vec3 endMm) => false;
    }
}
