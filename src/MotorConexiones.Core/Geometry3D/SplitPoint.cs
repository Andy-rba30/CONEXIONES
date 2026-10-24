using System;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>Dónde se parte la diagonal para el empalme del Detalle B (Fase 13, decisión D7), medido por el eje de la barra.</summary>
    public sealed class SplitPointResult
    {
        public SplitPointResult(long elementId, double faceDistanceMm, double firstPlateDistanceMm, double splitDistanceMm, Vec3 cutPointMm, Vec3 splitPointMm, Vec3 directionOutward, double thetaDeg)
        {
            ElementId = elementId;
            FaceDistanceMm = faceDistanceMm;
            FirstPlateDistanceMm = firstPlateDistanceMm;
            SplitDistanceMm = splitDistanceMm;
            CutPointMm = cutPointMm;
            SplitPointMm = splitPointMm;
            DirectionOutward = directionOutward;
            ThetaDeg = thetaDeg;
        }

        public long ElementId { get; }

        /// <summary>Del corte del eje de la barra con el eje del cordón a la cara del cordón, por el eje de la barra: (canto/2)/|sen θ|.</summary>
        public double FaceDistanceMm { get; }

        /// <summary>Del corte con el eje del cordón a la cara de la primera plancha del Detalle B: cara + offset_from_chord_face_mm (330).</summary>
        public double FirstPlateDistanceMm { get; }

        /// <summary>Del corte con el eje del cordón al punto donde se parten los dos elementos de Revit: el centro de la pila (330 + espesor + gap/2).</summary>
        public double SplitDistanceMm { get; }

        /// <summary>El corte del eje de la barra con el eje del cordón, en el marco global (mm).</summary>
        public Vec3 CutPointMm { get; }

        /// <summary>El punto de corte de los dos elementos, en el marco global (mm).</summary>
        public Vec3 SplitPointMm { get; }

        /// <summary>Dirección unitaria de la barra desde el cordón hacia fuera (hacia el tramo largo).</summary>
        public Vec3 DirectionOutward { get; }

        /// <summary>Inclinación de la barra respecto al cordón, en grados (0 a 90).</summary>
        public double ThetaDeg { get; }

        /// <summary>Del punto de corte a la cara del cordón (lo que mide el tramo corto por su eje, sin contar lo que entra en el cordón).</summary>
        public double ShortSegmentLengthFromFaceMm => SplitDistanceMm - FaceDistanceMm;

        public double[] SplitPointArray => new[] { Math.Round(SplitPointMm.X, 1), Math.Round(SplitPointMm.Y, 1), Math.Round(SplitPointMm.Z, 1) };
    }

    /// <summary>
    /// Fase 13 (D7): el 330 del Detalle B se mide <b>por el eje de la diagonal</b>, desde el punto donde ese eje cruza la <b>cara</b>
    /// del cordón hasta la cara de la primera plancha; el corte de los dos elementos de Revit queda en el centro de la pila de las
    /// dos planchas (330 + espesor + gap/2), de modo que el arranque <c>SpecStarter.ForSplice</c> retire cada tramo lo justo. La
    /// cara está a (canto del cordón en el plano / 2) / |sen θ| del corte del eje de la barra con el eje del cordón (medido sobre el
    /// corte propio de la barra, con su desvío de eje). En el nudo en K del Hangar: θ = 44,4°, cara a 101,6 → 145,2 por el eje;
    /// corte a 145,2 + 330 + 9,525 ≈ 485 mm. Puro Core: se prueba en la nube.
    /// </summary>
    public static class SplitPoint
    {
        /// <summary>Espesor de la plancha del Detalle B cuando la especificación no lo dice (3/8").</summary>
        public const double DefaultPlateThicknessMm = 9.525;

        /// <summary>
        /// El cálculo a partir de la geometría del marco: <paramref name="ux"/>, <paramref name="uy"/> es la dirección de la barra en el
        /// plano local (hacia fuera), <paramref name="axisOffsetMm"/> su desvío sobre el eje del cordón, <paramref name="chordWidthMm"/>
        /// el lado del cordón en el plano de la cercha. Lanza si la barra es paralela al cordón (no hay cara que cruzar).
        /// </summary>
        public static SplitPointResult Compute(long elementId, NodeFrame frame, double ux, double uy, double axisOffsetMm, double chordWidthMm,
            double offsetFromChordFaceMm, double plateThicknessMm, double gapMm)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            double length = Math.Sqrt(ux * ux + uy * uy);
            if (length < 1e-9) throw new ArgumentException("La dirección de la barra es nula.", nameof(ux));
            ux /= length;
            uy /= length;
            double sin = Math.Abs(uy);
            if (sin < Math.Sin(Batch.NodeDetectorOptions.MinCrossingAngleDeg * Math.PI / 180.0))
            {
                throw new NodeGeometryException(Validation.ErrorCodes.NodeAxesParallel,
                    "La barra es paralela al cordón: su eje no cruza la cara y no hay dónde medir el empalme del Detalle B.",
                    "El corte de la diagonal solo vale para barras que llegan al cordón con un ángulo (más de 5°).");
            }
            double theta = Math.Asin(Math.Min(1.0, sin)) * 180.0 / Math.PI;
            double half = (chordWidthMm > 0 ? chordWidthMm : Sketch.SketchNodeInfo.DefaultChordWidthMm) / 2.0;
            double face = half / sin;
            double offset = Math.Max(0.0, offsetFromChordFaceMm);
            double thickness = plateThicknessMm > 0 ? plateThicknessMm : DefaultPlateThicknessMm;
            double firstPlate = face + offset;
            double split = firstPlate + thickness + Math.Max(0.0, gapMm) / 2.0;
            Vec3 cut = frame.Origin + frame.X * axisOffsetMm;
            Vec3 direction = (frame.X * ux + frame.Y * uy).Normalized();
            Vec3 splitPoint = cut + direction * split;
            return new SplitPointResult(elementId, face, firstPlate, split, cut, splitPoint, direction, theta);
        }

        /// <summary>
        /// El punto de corte de una barra de una especificación <c>direct_weld</c> con <c>splice_hint</c>, leída del modelo con la regla
        /// única del marco (<see cref="SpecFrame"/>). Nulo si la barra no lleva <c>splice_hint</c> o no está en el modelo.
        /// </summary>
        public static SplitPointResult? FromSpec(ConnectionSpec spec, MemberSpec member, IModelFacts facts, double plateThicknessMm = DefaultPlateThicknessMm, double gapMm = 0.0)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (member == null) throw new ArgumentNullException(nameof(member));
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            double? offset = member.SpliceHint?.OffsetFromChordFaceMm;
            if (!offset.HasValue || offset.Value <= 0) return null;
            NodeFrame frame = SpecFrame.Compute(spec, facts);
            long chordId = member.ChordElementId ?? spec.Chord?.ElementId ?? 0;
            MemberModelFacts? chord = facts.GetMemberFacts(chordId) ?? (spec.Chord != null ? facts.GetMemberFacts(spec.Chord.ElementId) : null);
            MemberModelFacts? bar = facts.GetMemberFacts(member.ElementId);
            if (chord == null || bar == null) return null;
            return FromAxes(member.ElementId, frame, chord, bar, offset.Value, plateThicknessMm, gapMm);
        }

        /// <summary>El punto de corte a partir del marco del nudo y los ejes de la barra y del cordón (el add-in y el plan).</summary>
        public static SplitPointResult FromAxes(long elementId, NodeFrame frame, MemberModelFacts chord, MemberModelFacts bar, double offsetFromChordFaceMm,
            double plateThicknessMm = DefaultPlateThicknessMm, double gapMm = 0.0)
        {
            if (frame == null) throw new ArgumentNullException(nameof(frame));
            if (chord == null) throw new ArgumentNullException(nameof(chord));
            if (bar == null) throw new ArgumentNullException(nameof(bar));
            var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, bar.CurveStartMm, bar.CurveEndMm, frame.Origin);
            Vec3 chordDirection = (chord.CurveEndMm - chord.CurveStartMm).Normalized();
            double axisOffset = 0.0;
            double s = NodeFrame.ChordCutParameter(chord.CurveStartMm, chordDirection, bar.CurveStartMm, bar.CurveEndMm, out double distance);
            if (!double.IsNaN(s) && distance <= NodeFrame.MaxAxisDistanceMm)
            {
                Vec3 cut = chord.CurveStartMm + chordDirection * s;
                axisOffset = (cut - frame.Origin).Dot(frame.X);
                if (Math.Abs(axisOffset) < 0.05) axisOffset = 0.0;
            }
            double chordWidth = chord.WidthMm > 0 ? chord.WidthMm : Math.Max(chord.HeightMm, 0.0);
            return Compute(elementId, frame, ux, uy, axisOffset, chordWidth, offsetFromChordFaceMm, plateThicknessMm, gapMm);
        }

        /// <summary>
        /// Qué extremo de la barra queda del lado del cordón (D6: el tramo corto conserva el elemento original): 0 si es el inicio
        /// de su línea de ubicación, 1 si es el final.
        /// </summary>
        public static int NearEndIndex(MemberModelFacts bar, Vec3 cutPointMm) =>
            bar.CurveStartMm.DistanceTo(cutPointMm) <= bar.CurveEndMm.DistanceTo(cutPointMm) ? 0 : 1;

        /// <summary>
        /// Los dos tramos de la barra partida en <paramref name="result"/>: el corto (del extremo del lado del cordón al punto de corte,
        /// conserva el id) y el largo (del punto de corte al otro extremo, el elemento nuevo). Lanza si el punto de corte cae fuera
        /// de la barra (barra más corta que el 330 más la pila).
        /// </summary>
        public static (Vec3 ShortStart, Vec3 ShortEnd, Vec3 LongStart, Vec3 LongEnd) Segments(MemberModelFacts bar, SplitPointResult result)
        {
            if (bar == null) throw new ArgumentNullException(nameof(bar));
            if (result == null) throw new ArgumentNullException(nameof(result));
            int near = NearEndIndex(bar, result.CutPointMm);
            Vec3 nearEnd = near == 0 ? bar.CurveStartMm : bar.CurveEndMm;
            Vec3 farEnd = near == 0 ? bar.CurveEndMm : bar.CurveStartMm;
            double total = nearEnd.DistanceTo(farEnd);
            double alongFromNear = (result.SplitPointMm - nearEnd).Dot((farEnd - nearEnd).Normalized());
            if (alongFromNear <= 1.0 || alongFromNear >= total - 1.0)
            {
                throw new NodeGeometryException(Validation.ErrorCodes.SplitFailed,
                    string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "El punto de corte queda a {0:0.0} mm del extremo del lado del cordón y la barra {1} mide {2:0.0} mm: no se puede partir.", alongFromNear, bar.ElementId, total),
                    "Comprueba offset_from_chord_face_mm (330 por defecto) y que la barra sea la diagonal entera.");
            }
            Vec3 split = nearEnd + (farEnd - nearEnd).Normalized() * alongFromNear;
            return near == 0 ? (nearEnd, split, split, farEnd) : (split, nearEnd, farEnd, split);
        }
    }
}
