using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Units;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>Lo que el croquis necesita saber de una barra: dirección en el plano local, ancho del perfil y tipo.</summary>
    public sealed class SketchMemberInfo
    {
        public SketchMemberInfo(long elementId, double ux, double uy, double widthMm, string? typeName, double angleInPlaneDeg, bool isApproximate, double axisOffsetMm = 0.0, double depthMm = 0.0)
        {
            ElementId = elementId;
            Ux = ux;
            Uy = uy;
            WidthMm = widthMm;
            TypeName = typeName;
            AngleInPlaneDeg = angleInPlaneDeg;
            IsApproximate = isApproximate;
            AxisOffsetMm = axisOffsetMm;
            DepthMm = depthMm > 0 ? depthMm : widthMm;
        }

        public long ElementId { get; }
        /// <summary>Dirección unitaria de la barra en el plano local, del punto de trabajo hacia fuera.</summary>
        public double Ux { get; }
        public double Uy { get; }
        /// <summary>Ancho del perfil en el plano (mm).</summary>
        public double WidthMm { get; }
        /// <summary>Nombre del tipo en el modelo (p. ej. <c>HSS2-1-2X2-1-2X3-16 64x64</c>), si se conoce.</summary>
        public string? TypeName { get; }
        /// <summary>
        /// Ángulo con signo desde +X local en grados, en [−180°, 180°) (Fase 7: la misma regla que
        /// <c>conn_get_node_info</c>; +Y = arriba en cerchas verticales). <see cref="NodeFrame.AngleToChordDeg"/> da la
        /// inclinación sin signo que escribe un plano.
        /// </summary>
        public double AngleInPlaneDeg { get; }
        /// <summary>Verdadero si la dirección no salió del modelo sino del ángulo escrito en la especificación.</summary>
        public bool IsApproximate { get; }
        /// <summary>
        /// Fase 12 (opción A): dónde corta el eje de la barra al eje del cordón, medido desde el punto de trabajo a lo largo
        /// de +X (con signo). 0 cuando concurre: la barra se dibuja desde (<see cref="AxisOffsetMm"/>, 0), no desde el origen.
        /// </summary>
        public double AxisOffsetMm { get; }
        /// <summary>Fase 12: el otro lado de la sección (para la placa de extremo: ancho × alto del tubo).</summary>
        public double DepthMm { get; }
    }

    /// <summary>
    /// Datos del nudo para el croquis, sin Revit: anchos de perfil y direcciones de las barras en el sistema local
    /// (sección 7 del encargo). Se construye desde <see cref="IModelFacts"/> (en Revit o en las pruebas) o, si el
    /// nudo no se puede leer, desde los ángulos de la especificación (aproximado).
    /// </summary>
    public sealed class SketchNodeInfo
    {
        /// <summary>Ancho supuesto para una barra HSS2-1/2 cuando el modelo no lo dice.</summary>
        public const double DefaultMemberWidthMm = 63.5;
        /// <summary>Ancho supuesto para un cordón HSS3X3 cuando el modelo no lo dice.</summary>
        public const double DefaultChordWidthMm = 76.2;

        public SketchNodeInfo(double chordWidthMm, string? chordTypeName, IReadOnlyList<SketchMemberInfo> members, Vec3? localYInGlobal, string? note)
        {
            ChordWidthMm = chordWidthMm > 0 ? chordWidthMm : DefaultChordWidthMm;
            ChordDepthMm = ChordWidthMm;
            ChordTypeName = chordTypeName;
            Members = members ?? new List<SketchMemberInfo>();
            LocalYInGlobal = localYInGlobal;
            Note = note;
        }

        public double ChordWidthMm { get; }
        /// <summary>Fase 12: el otro lado de la sección del cordón (el 12" del HSS12X8, perpendicular al plano); por defecto el ancho.</summary>
        public double ChordDepthMm { get; set; }
        public string? ChordTypeName { get; }
        public IReadOnlyList<SketchMemberInfo> Members { get; }
        /// <summary>Eje Y local expresado en coordenadas globales (para la leyenda); nulo si no hay marco.</summary>
        public Vec3? LocalYInGlobal { get; }
        /// <summary>Fase 12: la excentricidad medida del nudo (distancia entre los cortes extremos), 0 si concurren.</summary>
        public double EccentricityMm { get; set; }
        /// <summary>Aviso para la persona cuando algo es aproximado.</summary>
        public string? Note { get; }
        public bool IsApproximate => Note != null;

        public SketchMemberInfo? Find(long elementId)
        {
            foreach (var member in Members)
            {
                if (member.ElementId == elementId) return member;
            }
            return null;
        }

        /// <summary>
        /// Lee el nudo de los hechos del modelo: marco con <see cref="NodeFrame.Compute(Vec3, Vec3, Vec3, Vec3, IEnumerable{ValueTuple{Vec3, Vec3}})"/>
        /// (cordón + todas las barras: el punto de trabajo a mitad entre los cortes, Fase 12) y dirección de cada barra con
        /// <see cref="ConnectionGeometry.GetMemberDirection2D"/>; cada barra lleva además el corte de su eje con el del cordón.
        /// Si falta algo, cae a <see cref="FromSpecAngles"/> con una nota.
        /// </summary>
        public static SketchNodeInfo FromModelFacts(ConnectionSpec spec, IModelFacts? facts)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (facts == null) return FromSpecAngles(spec, "No hay datos del modelo: direcciones aproximadas por el ángulo de la especificación.");
            MemberModelFacts? chord = spec.Chord != null ? facts.GetMemberFacts(spec.Chord.ElementId) : null;
            MemberModelFacts? first = spec.Members != null && spec.Members.Count > 0 ? facts.GetMemberFacts(spec.Members[0].ElementId) : null;
            if (chord == null || first == null)
            {
                return FromSpecAngles(spec, "El cordón o la primera barra no están en el modelo: direcciones aproximadas por el ángulo de la especificación.");
            }
            List<MemberSpec> memberSpecs = spec.Members ?? new List<MemberSpec>();
            NodeFrame frame;
            try
            {
                frame = FrameOf(spec, facts, chord, first, memberSpecs);
            }
            catch (NodeGeometryException error)
            {
                return FromSpecAngles(spec, "No se pudo calcular el sistema local (" + error.Code + "): direcciones aproximadas por el ángulo de la especificación.");
            }
            var members = new List<SketchMemberInfo>();
            string? note = null;
            Vec3 chordDirection = (chord.CurveEndMm - chord.CurveStartMm).Normalized();
            for (int i = 0; i < memberSpecs.Count; i++)
            {
                MemberSpec memberSpec = memberSpecs[i];
                MemberModelFacts? memberFacts = facts.GetMemberFacts(memberSpec.ElementId);
                if (memberFacts == null)
                {
                    members.Add(ApproximateMember(spec, i));
                    note = "La barra " + memberSpec.ElementId + " no está en el modelo: su dirección es aproximada.";
                    continue;
                }
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, memberFacts.CurveStartMm, memberFacts.CurveEndMm, frame.Origin);
                double angle = NodeFrame.SignedAngleDeg(ux, uy);
                double width = memberFacts.WidthMm > 0 ? memberFacts.WidthMm : DefaultMemberWidthMm;
                double depth = memberFacts.HeightMm > 0 ? memberFacts.HeightMm : width;
                // Fase 12: el corte del eje de la barra con el eje del cordón, con signo a lo largo de +X (0 si concurre).
                double offset = 0.0;
                double s = NodeFrame.ChordCutParameter(chord.CurveStartMm, chordDirection, memberFacts.CurveStartMm, memberFacts.CurveEndMm, out double distance);
                if (!double.IsNaN(s) && distance <= NodeFrame.MaxAxisDistanceMm)
                {
                    Vec3 cut = chord.CurveStartMm + chordDirection * s;
                    offset = (cut - frame.Origin).Dot(frame.X);
                    if (Math.Abs(offset) < 0.05) offset = 0.0;
                }
                members.Add(new SketchMemberInfo(memberSpec.ElementId, ux, uy, width, memberFacts.TypeName, angle, false, Math.Round(offset, 2), depth));
            }
            double chordWidth = chord.WidthMm > 0 ? chord.WidthMm : DefaultChordWidthMm;
            return new SketchNodeInfo(chordWidth, chord.TypeName, members, frame.Y, note)
            {
                ChordDepthMm = chord.HeightMm > 0 ? chord.HeightMm : chordWidth,
                EccentricityMm = Math.Round(frame.EccentricityMm, 2),
            };
        }

        /// <summary>Fase 12: el marco del nudo con la regla única de <see cref="SpecFrame"/>.</summary>
        public static NodeFrame FrameOf(ConnectionSpec spec, IModelFacts facts, MemberModelFacts chord, MemberModelFacts first, IReadOnlyList<MemberSpec> memberSpecs)
        {
            var others = new List<(Vec3 Start, Vec3 End)>();
            for (int i = 1; i < memberSpecs.Count; i++)
            {
                MemberModelFacts? other = facts.GetMemberFacts(memberSpecs[i].ElementId);
                if (other != null) others.Add((other.CurveStartMm, other.CurveEndMm));
            }
            return SpecFrame.Compute(spec.IsType("end_plate"), chord.CurveStartMm, chord.CurveEndMm, first.CurveStartMm, first.CurveEndMm, others);
        }

        /// <summary>El punto de empalme de dos tramos: a mitad entre los dos extremos más cercanos.</summary>
        public static Vec3 SplicePoint(MemberModelFacts a, MemberModelFacts b) => SpecFrame.SplicePoint(a.CurveStartMm, a.CurveEndMm, b.CurveStartMm, b.CurveEndMm);

        /// <summary>
        /// Sin modelo: cada barra toma su <c>expected_angle_deg</c> (o 90° si es montante, 45° si no) y se reparte
        /// por cuadrantes para que no se superpongan. Solo para ver algo cuando el nudo no se puede leer.
        /// </summary>
        public static SketchNodeInfo FromSpecAngles(ConnectionSpec spec, string? note = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            var members = new List<SketchMemberInfo>();
            for (int i = 0; i < (spec.Members?.Count ?? 0); i++)
            {
                members.Add(ApproximateMember(spec, i));
            }
            double chordWidth = TryWidth(spec.Chord?.Profile) ?? DefaultChordWidthMm;
            return new SketchNodeInfo(chordWidth, spec.Chord?.Profile, members, null,
                note ?? "Direcciones aproximadas: el nudo no se leyó del modelo.")
            {
                ChordDepthMm = TryDepth(spec.Chord?.Profile) ?? chordWidth,
                EccentricityMm = spec.Node?.EccentricityMm ?? 0.0,
            };
        }

        /// <summary>Lado menor de un perfil HSS por su designación (el que queda en el plano de la cercha), en mm; nulo si no se reconoce.</summary>
        public static double? TryWidth(string? profile)
        {
            if (string.IsNullOrWhiteSpace(profile) || !Validation.ProfileMatcher.TryParseHssDimensions(profile!, out double w, out double h, out _) || w <= 0) return null;
            return UnitConverter.InchesToMm(h > 0 ? Math.Min(w, h) : w);
        }

        /// <summary>Lado mayor de un perfil HSS por su designación, en mm; nulo si no se reconoce.</summary>
        public static double? TryDepth(string? profile)
        {
            if (string.IsNullOrWhiteSpace(profile) || !Validation.ProfileMatcher.TryParseHssDimensions(profile!, out double w, out double h, out _) || w <= 0) return null;
            return UnitConverter.InchesToMm(Math.Max(w, h));
        }

        private static SketchMemberInfo ApproximateMember(ConnectionSpec spec, int index)
        {
            MemberSpec member = spec.Members[index];
            bool vertical = string.Equals(member.Role, "vertical", StringComparison.OrdinalIgnoreCase);
            bool chordRole = string.Equals(member.Role, "chord", StringComparison.OrdinalIgnoreCase);
            double angle = member.ExpectedAngleDeg ?? (vertical ? 90.0 : chordRole ? 0.0 : 45.0);
            double radians = UnitConverter.DegreesToRadians(angle);
            double ux = Math.Cos(radians);
            double uy = Math.Sin(radians);
            bool signed = member.ExpectedAngleDeg.HasValue && (member.ExpectedAngleDeg.Value < 0 || member.ExpectedAngleDeg.Value > 90);
            if (chordRole)
            {
                ux = 1.0;
                uy = 0.0;
            }
            else if (spec.IsType("direct_weld") && !signed)
            {
                // Fase 12: en un nudo soldado todas las barras van al mismo lado (desde abajo, −Y): se alternan −x / +x.
                uy = -Math.Abs(uy);
                if (index % 2 == 0) ux = -Math.Abs(ux);
            }
            else if (!signed)
            {
                // Reparto: 0 → (+x, +y), 1 → (−x, +y), 2 → (+x, −y), 3 → (−x, −y), y vuelta a empezar.
                if (index % 2 == 1) ux = -ux;
                if ((index / 2) % 2 == 1) uy = -uy;
            }
            double width = TryWidth(member.Profile) ?? DefaultMemberWidthMm;
            return new SketchMemberInfo(member.ElementId, ux, uy, width, member.Profile, NodeFrame.SignedAngleDeg(ux, uy), true, member.AxisOffsetMm ?? 0.0, TryDepth(member.Profile) ?? width);
        }
    }
}
