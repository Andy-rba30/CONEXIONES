using System;
using System.Collections.Generic;
using System.Globalization;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Units;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Sketch
{
    /// <summary>Una barra del nudo tal como la necesita el croquis: dirección 2D en el sistema local y ancho del perfil.</summary>
    public sealed class SketchMemberInput
    {
        public SketchMemberInput(long elementId, double ux, double uy, double widthMm, string? profile, bool fromModel)
        {
            ElementId = elementId;
            double len = Math.Sqrt(ux * ux + uy * uy);
            if (len < 1e-9)
            {
                ux = 1.0;
                uy = 0.0;
                len = 1.0;
            }
            Ux = ux / len;
            Uy = uy / len;
            WidthMm = widthMm > 0 ? widthMm : GussetNodeSketch.DefaultMemberWidthMm;
            Profile = profile;
            FromModel = fromModel;
        }

        public long ElementId { get; }

        /// <summary>Dirección unitaria desde el punto de trabajo hacia fuera, en el plano local (X cordón, Y perpendicular).</summary>
        public double Ux { get; }
        public double Uy { get; }

        /// <summary>Ancho del perfil en el plano del croquis.</summary>
        public double WidthMm { get; }

        /// <summary>Nombre del tipo en el modelo (o el perfil del contrato si no hay modelo).</summary>
        public string? Profile { get; }

        /// <summary>Verdadero si la dirección salió de la curva de ubicación del modelo; falso si es esquemática.</summary>
        public bool FromModel { get; }

        /// <summary>Ángulo agudo con el cordón, en grados (0 a 90), como se lee en el plano.</summary>
        public double AngleToChordDeg => UnitConverter.RadiansToDegrees(Math.Atan2(Math.Abs(Uy), Math.Abs(Ux)));

        /// <summary>Ángulo en el plano medido desde +X (0 a 180), el mismo que devuelve <c>conn_get_node_info</c>.</summary>
        public double AngleInPlaneDeg => UnitConverter.RadiansToDegrees(Math.Atan2(Math.Abs(Uy), Ux));
    }

    /// <summary>
    /// Datos del nudo que el croquis necesita y que no están en la especificación: direcciones reales de las barras en
    /// el plano de la cercha y anchos de perfil. Se construyen desde <see cref="IModelFacts"/> (Revit o simulado) o, si
    /// no hay modelo, de forma esquemática a partir de los ángulos del plano (<c>expected_angle_deg</c>).
    /// </summary>
    public sealed class SketchNodeInput
    {
        private SketchNodeInput(double chordWidthMm, string? chordProfile, bool isSchematic)
        {
            ChordWidthMm = chordWidthMm > 0 ? chordWidthMm : GussetNodeSketch.DefaultChordWidthMm;
            ChordProfile = chordProfile;
            IsSchematic = isSchematic;
        }

        public double ChordWidthMm { get; }
        public string? ChordProfile { get; }

        /// <summary>Verdadero si no se pudo calcular el sistema local desde el modelo.</summary>
        public bool IsSchematic { get; }

        /// <summary>
        /// Componente Z global del eje Y local (0 si es esquemático). Si es negativa, el eje Y del croquis apunta hacia
        /// abajo en el modelo y el dibujo se ve girado 180° respecto a la vista real; las coordenadas del contrato se
        /// dibujan tal cual.
        /// </summary>
        public double LocalYGlobalZ { get; private set; }

        public List<SketchMemberInput> Members { get; } = new List<SketchMemberInput>();

        /// <summary>Explicaciones en español de lo que no salió del modelo.</summary>
        public List<string> Notes { get; } = new List<string>();

        public SketchMemberInput? Find(long elementId) => Members.Find(m => m.ElementId == elementId);

        /// <summary>
        /// Construye el nudo desde los hechos del modelo: el sistema local con la misma regla que el add-in
        /// (<see cref="NodeFrame.Compute"/> con el cordón y el primer miembro) y cada dirección con
        /// <see cref="ConnectionGeometry.GetMemberDirection2D"/>. Si falta algo, cae al modo esquemático y lo anota.
        /// </summary>
        public static SketchNodeInput FromModelFacts(ConnectionSpec spec, IModelFacts? facts)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (facts == null) return Schematic(spec, "No hay modelo: direcciones esquemáticas según expected_angle_deg.");

            long chordId = spec.Chord?.ElementId ?? 0;
            MemberModelFacts? chordFacts = chordId > 0 ? facts.GetMemberFacts(chordId) : null;
            MemberModelFacts? firstFacts = null;
            if (spec.Members != null && spec.Members.Count > 0)
            {
                firstFacts = facts.GetMemberFacts(spec.Members[0].ElementId);
            }

            if (chordFacts == null || firstFacts == null)
            {
                return Schematic(spec, "El cordón o el primer miembro no están en el modelo: croquis esquemático.");
            }

            NodeFrame frame;
            try
            {
                frame = NodeFrame.Compute(chordFacts.CurveStartMm, chordFacts.CurveEndMm, firstFacts.CurveStartMm, firstFacts.CurveEndMm);
            }
            catch (NodeGeometryException error)
            {
                return Schematic(spec, "No se pudo calcular el sistema local del nudo (" + error.Code + "): croquis esquemático.");
            }
            catch (InvalidOperationException)
            {
                return Schematic(spec, "Una barra del modelo tiene longitud cero: croquis esquemático.");
            }

            double chordWidth = chordFacts.WidthMm > 0 ? chordFacts.WidthMm : ProfileDimensions.WidthOrDefault(spec.Chord?.Profile, GussetNodeSketch.DefaultChordWidthMm);
            var input = new SketchNodeInput(chordWidth, chordFacts.TypeName ?? spec.Chord?.Profile, isSchematic: false)
            {
                LocalYGlobalZ = frame.Y.Z,
            };
            if (frame.Y.Z < -0.5)
            {
                input.Notes.Add("En el modelo el eje Y local apunta hacia abajo (Z global negativa): el croquis se ve girado 180° respecto a la vista real. Las coordenadas del contrato se dibujan tal cual.");
            }

            if (spec.Members != null)
            {
                int index = 0;
                foreach (MemberSpec member in spec.Members)
                {
                    MemberModelFacts? mf = facts.GetMemberFacts(member.ElementId);
                    if (mf == null)
                    {
                        SketchMemberInput schematic = SchematicMember(member, index, spec.Members);
                        input.Members.Add(schematic);
                        input.Notes.Add("La barra " + member.ElementId + " no está en el modelo: dirección esquemática.");
                    }
                    else
                    {
                        var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, mf.CurveStartMm, mf.CurveEndMm, frame.Origin);
                        double width = mf.WidthMm > 0 ? mf.WidthMm : ProfileDimensions.WidthOrDefault(member.Profile, GussetNodeSketch.DefaultMemberWidthMm);
                        input.Members.Add(new SketchMemberInput(member.ElementId, ux, uy, width, mf.TypeName ?? member.Profile, fromModel: true));
                    }
                    index++;
                }
            }
            return input;
        }

        /// <summary>
        /// Nudo esquemático sin modelo: cordón horizontal, montantes verticales y diagonales con el ángulo del plano
        /// (<c>expected_angle_deg</c>, 45° si falta), alternando cuadrantes para que no se superpongan.
        /// </summary>
        public static SketchNodeInput Schematic(ConnectionSpec spec, string? note = null)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            double chordWidth = ProfileDimensions.WidthOrDefault(spec.Chord?.Profile, GussetNodeSketch.DefaultChordWidthMm);
            var input = new SketchNodeInput(chordWidth, spec.Chord?.Profile, isSchematic: true);
            if (!string.IsNullOrWhiteSpace(note)) input.Notes.Add(note!);
            if (spec.Members != null)
            {
                for (int i = 0; i < spec.Members.Count; i++)
                {
                    input.Members.Add(SchematicMember(spec.Members[i], i, spec.Members));
                }
            }
            return input;
        }

        private static SketchMemberInput SchematicMember(MemberSpec member, int index, IList<MemberSpec> all)
        {
            double width = ProfileDimensions.WidthOrDefault(member.Profile, GussetNodeSketch.DefaultMemberWidthMm);
            if (string.Equals(member.Role, "vertical", StringComparison.OrdinalIgnoreCase))
            {
                return new SketchMemberInput(member.ElementId, 0.0, 1.0, width, member.Profile, fromModel: false);
            }

            double angle = member.ExpectedAngleDeg.GetValueOrDefault(45.0);
            angle = Math.Abs(angle) % 180.0;
            if (angle > 90.0) angle = 180.0 - angle;
            if (angle < 5.0) angle = 45.0;
            double rad = UnitConverter.DegreesToRadians(angle);
            double c = Math.Cos(rad);
            double s = Math.Sin(rad);

            // Cuadrante según el orden entre las diagonales: 1.ª arriba-derecha, 2.ª abajo-izquierda, 3.ª arriba-izquierda, 4.ª abajo-derecha.
            int diagonalIndex = 0;
            for (int i = 0; i < index && i < all.Count; i++)
            {
                if (!string.Equals(all[i].Role, "vertical", StringComparison.OrdinalIgnoreCase)) diagonalIndex++;
            }
            switch (diagonalIndex % 4)
            {
                case 0: return new SketchMemberInput(member.ElementId, c, s, width, member.Profile, fromModel: false);
                case 1: return new SketchMemberInput(member.ElementId, -c, -s, width, member.Profile, fromModel: false);
                case 2: return new SketchMemberInput(member.ElementId, -c, s, width, member.Profile, fromModel: false);
                default: return new SketchMemberInput(member.ElementId, c, -s, width, member.Profile, fromModel: false);
            }
        }
    }

    /// <summary>Ancho de un perfil a partir de su nombre, para el croquis cuando el modelo no da parámetros.</summary>
    public static class ProfileDimensions
    {
        /// <summary>
        /// <c>HSS3X3X1/4</c> → 76,2; <c>HSS2-1/2X2-1/2X3/16</c> → 63,5; <c>HSS2-1-2X2-1-2X3-16 64x64</c> → 64 (sufijo métrico).
        /// Devuelve falso si el nombre no se entiende.
        /// </summary>
        public static bool TryParseWidthMm(string? profile, out double widthMm)
        {
            widthMm = 0.0;
            if (string.IsNullOrWhiteSpace(profile)) return false;
            string s = profile!.Trim().ToUpperInvariant();

            // Sufijo métrico de los tipos de Revit: "HSS2-1-2X2-1-2X3-16 64x64" → 64 (dos números y nada más).
            int space = s.LastIndexOf(' ');
            if (space > 0 && space + 1 < s.Length)
            {
                string[] parts = s.Substring(space + 1).Trim().Split('X');
                if (parts.Length == 2
                    && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double metricWidth)
                    && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                    && metricWidth > 0)
                {
                    widthMm = metricWidth;
                    return true;
                }
            }
            s = s.Replace(" ", "");

            int hss = s.IndexOf("HSS", StringComparison.Ordinal);
            if (hss < 0) return false;
            string rest = s.Substring(hss + 3);
            string[] imperial = rest.Split('X');
            if (imperial.Length < 2) return false;
            string first = imperial[0].Trim();
            if (LabelParser.TryParseLabelToMm(first + "\"", out double mm) && mm > 0)
            {
                widthMm = mm;
                return true;
            }
            return false;
        }

        public static double WidthOrDefault(string? profile, double fallbackMm)
        {
            return TryParseWidthMm(profile, out double mm) ? mm : fallbackMm;
        }
    }
}
