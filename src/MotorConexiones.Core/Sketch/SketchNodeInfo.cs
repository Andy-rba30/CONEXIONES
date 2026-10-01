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
        public SketchMemberInfo(long elementId, double ux, double uy, double widthMm, string? typeName, double angleInPlaneDeg, bool isApproximate)
        {
            ElementId = elementId;
            Ux = ux;
            Uy = uy;
            WidthMm = widthMm;
            TypeName = typeName;
            AngleInPlaneDeg = angleInPlaneDeg;
            IsApproximate = isApproximate;
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
            ChordTypeName = chordTypeName;
            Members = members ?? new List<SketchMemberInfo>();
            LocalYInGlobal = localYInGlobal;
            Note = note;
        }

        public double ChordWidthMm { get; }
        public string? ChordTypeName { get; }
        public IReadOnlyList<SketchMemberInfo> Members { get; }

        /// <summary>Eje Y local expresado en coordenadas globales (para la leyenda); nulo si no hay marco.</summary>
        public Vec3? LocalYInGlobal { get; }

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
        /// Lee el nudo de los hechos del modelo: marco con <see cref="NodeFrame.Compute"/> (cordón + primer miembro, la
        /// misma regla que validar y crear) y dirección de cada barra con <see cref="ConnectionGeometry.GetMemberDirection2D"/>.
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

            NodeFrame frame;
            try
            {
                frame = NodeFrame.Compute(chord.CurveStartMm, chord.CurveEndMm, first.CurveStartMm, first.CurveEndMm);
            }
            catch (NodeGeometryException error)
            {
                return FromSpecAngles(spec, "No se pudo calcular el sistema local (" + error.Code + "): direcciones aproximadas por el ángulo de la especificación.");
            }

            var members = new List<SketchMemberInfo>();
            string? note = null;
            List<MemberSpec> memberSpecs = spec.Members ?? new List<MemberSpec>();
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
                members.Add(new SketchMemberInfo(memberSpec.ElementId, ux, uy, width, memberFacts.TypeName, angle, false));
            }

            double chordWidth = chord.WidthMm > 0 ? chord.WidthMm : DefaultChordWidthMm;
            return new SketchNodeInfo(chordWidth, chord.TypeName, members, frame.Y, note);
        }

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
            return new SketchNodeInfo(DefaultChordWidthMm, spec.Chord?.Profile, members, null,
                note ?? "Direcciones aproximadas: el nudo no se leyó del modelo.");
        }

        private static SketchMemberInfo ApproximateMember(ConnectionSpec spec, int index)
        {
            MemberSpec member = spec.Members[index];
            bool vertical = string.Equals(member.Role, "vertical", StringComparison.OrdinalIgnoreCase);
            double angle = member.ExpectedAngleDeg ?? (vertical ? 90.0 : 45.0);
            double radians = UnitConverter.DegreesToRadians(angle);
            double ux = Math.Cos(radians);
            double uy = Math.Sin(radians);
            // Reparto: 0 → (+x, +y), 1 → (−x, +y), 2 → (+x, −y), 3 → (−x, −y), y vuelta a empezar.
            if (index % 2 == 1) ux = -ux;
            if ((index / 2) % 2 == 1) uy = -uy;
            return new SketchMemberInfo(member.ElementId, ux, uy, DefaultMemberWidthMm, member.Profile, NodeFrame.SignedAngleDeg(ux, uy), true);
        }
    }
}
