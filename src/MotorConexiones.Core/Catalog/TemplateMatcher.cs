using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Las cuatro orientaciones en que se prueba una plantilla (sección 3.3 de la propuesta).</summary>
    public enum TemplateOrientation
    {
        /// <summary>Tal cual.</summary>
        Same,
        /// <summary>Reflejada en X (x → −x): el nudo simétrico de la otra mitad de la cercha.</summary>
        MirrorX,
        /// <summary>Reflejada en Y (y → −y): el mismo nudo en el otro cordón (barras hacia abajo).</summary>
        MirrorY,
        /// <summary>Las dos reflexiones.</summary>
        Both,
    }

    public static class TemplateOrientations
    {
        public static string ToName(TemplateOrientation orientation) => orientation switch
        {
            TemplateOrientation.MirrorX => "mirror_x",
            TemplateOrientation.MirrorY => "mirror_y",
            TemplateOrientation.Both => "both",
            _ => "same",
        };

        public static bool TryParse(string? text, out TemplateOrientation orientation)
        {
            switch ((text ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "same": orientation = TemplateOrientation.Same; return true;
                case "mirror_x": orientation = TemplateOrientation.MirrorX; return true;
                case "mirror_y": orientation = TemplateOrientation.MirrorY; return true;
                case "both": orientation = TemplateOrientation.Both; return true;
                default: orientation = TemplateOrientation.Same; return false;
            }
        }

        public static readonly TemplateOrientation[] All =
        {
            TemplateOrientation.Same, TemplateOrientation.MirrorX, TemplateOrientation.MirrorY, TemplateOrientation.Both,
        };
    }

    /// <summary>Una ranura de la plantilla y la barra del nudo que le tocó (o ninguna).</summary>
    public sealed class SlotAssignment
    {
        public SlotAssignment(MemberPatternSlot slot, double templateAngleDeg, TemplateNodeMember? member)
        {
            Slot = slot.Slot;
            Role = slot.Role;
            TemplateProfile = slot.Profile;
            ProfilePolicy = slot.ProfilePolicy;
            TemplateAngleDeg = templateAngleDeg;
            Member = member;
            DeviationDeg = member != null ? NodeFrame.AngleDifferenceDeg(member.AngleDeg, templateAngleDeg) : double.NaN;
        }

        public int Slot { get; }
        public string? Role { get; }
        public string? TemplateProfile { get; }
        public string ProfilePolicy { get; }

        /// <summary>Ángulo de la ranura ya transformado a la orientación probada.</summary>
        public double TemplateAngleDeg { get; }

        public TemplateNodeMember? Member { get; }
        public long? ElementId => Member?.ElementId;
        public double? ModelAngleDeg => Member?.AngleDeg;

        /// <summary>Diferencia entre el ángulo real y el de la plantilla (NaN si la ranura quedó sin barra).</summary>
        public double DeviationDeg { get; }

        public bool IsAssigned => Member != null;
    }

    /// <summary>Resultado de casar una plantilla con un nudo en una orientación.</summary>
    public sealed class TemplateMatch
    {
        public TemplateMatch(TemplateOrientation orientation, List<SlotAssignment> assignments, List<long> unassignedMembers)
        {
            Orientation = orientation;
            Assignments = assignments;
            UnassignedMembers = unassignedMembers;
        }

        public TemplateOrientation Orientation { get; }
        public string OrientationName => TemplateOrientations.ToName(Orientation);
        public IReadOnlyList<SlotAssignment> Assignments { get; }

        /// <summary>Barras del nudo que no corresponden a ninguna ranura.</summary>
        public IReadOnlyList<long> UnassignedMembers { get; }

        public IEnumerable<int> UnmatchedSlots => Assignments.Where(a => !a.IsAssigned).Select(a => a.Slot);
        public int MatchedCount => Assignments.Count(a => a.IsAssigned);
        public bool IsComplete => Assignments.Count > 0 && Assignments.All(a => a.IsAssigned);

        /// <summary>Suma de desviaciones de las ranuras casadas (menor es mejor).</summary>
        public double Score => Assignments.Where(a => a.IsAssigned).Sum(a => a.DeviationDeg);

        public double MaxDeviationDeg => Assignments.Where(a => a.IsAssigned).Select(a => a.DeviationDeg).DefaultIfEmpty(0.0).Max();

        /// <summary>Texto corto para errores y ventanas.</summary>
        public string Describe()
        {
            var parts = new List<string>();
            foreach (SlotAssignment a in Assignments)
            {
                parts.Add(a.IsAssigned
                    ? string.Format(System.Globalization.CultureInfo.InvariantCulture, "ranura {0} ({1} {2:0.0}°) → barra {3} ({4:0.0}°, desvío {5:0.0}°)",
                        a.Slot, a.Role ?? "barra", a.TemplateAngleDeg, a.ElementId, a.ModelAngleDeg, a.DeviationDeg)
                    : string.Format(System.Globalization.CultureInfo.InvariantCulture, "ranura {0} ({1} {2:0.0}°) sin barra",
                        a.Slot, a.Role ?? "barra", a.TemplateAngleDeg));
            }
            if (UnassignedMembers.Count > 0) parts.Add("barras sobrantes: " + string.Join(", ", UnassignedMembers));
            return OrientationName + ": " + string.Join("; ", parts);
        }
    }

    /// <summary>
    /// Casa una plantilla con un nudo (sección 3.3 de la propuesta): prueba las orientaciones permitidas, en cada una
    /// asigna barras a ranuras por ángulo más cercano dentro de la tolerancia (sin repetir barra ni ranura) y se queda
    /// con la que casa más ranuras con menor suma de desviaciones. Puro Core, se prueba sin Revit.
    /// </summary>
    public static class TemplateMatcher
    {
        /// <summary>Mejor casado (completo si lo hay; si no, el mejor intento, con <see cref="TemplateMatch.IsComplete"/> falso).</summary>
        public static TemplateMatch Match(CatalogTemplate template, TemplateNode node, double? angleToleranceDeg = null, bool? allowMirror = null, TemplateOrientation? forced = null)
        {
            return MatchAll(template, node, angleToleranceDeg, allowMirror, forced)
                .OrderByDescending(m => m.MatchedCount)
                .ThenBy(m => m.Score)
                .ThenBy(m => Array.IndexOf(TemplateOrientations.All, m.Orientation))
                .First();
        }

        /// <summary>Un intento por orientación permitida (para explicar un "sin encaje").</summary>
        public static List<TemplateMatch> MatchAll(CatalogTemplate template, TemplateNode node, double? angleToleranceDeg = null, bool? allowMirror = null, TemplateOrientation? forced = null)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (node == null) throw new ArgumentNullException(nameof(node));
            double tolerance = angleToleranceDeg ?? template.Matching?.AngleToleranceDeg ?? 10.0;
            bool mirror = allowMirror ?? template.Matching?.AllowMirror ?? true;

            IEnumerable<TemplateOrientation> orientations = forced.HasValue
                ? new[] { forced.Value }
                : mirror ? TemplateOrientations.All : new[] { TemplateOrientation.Same };

            var results = new List<TemplateMatch>();
            foreach (TemplateOrientation orientation in orientations)
            {
                results.Add(MatchOrientation(template, node, orientation, tolerance));
            }
            return results;
        }

        /// <summary>Ángulo de una ranura visto en una orientación.</summary>
        public static double TransformAngle(double angleDeg, TemplateOrientation orientation) => orientation switch
        {
            TemplateOrientation.MirrorX => NodeFrame.NormalizeDeg(180.0 - angleDeg),
            TemplateOrientation.MirrorY => NodeFrame.NormalizeDeg(-angleDeg),
            TemplateOrientation.Both => NodeFrame.NormalizeDeg(angleDeg - 180.0),
            _ => NodeFrame.NormalizeDeg(angleDeg),
        };

        /// <summary>Un punto del plano local (contorno de la cartela) visto en una orientación.</summary>
        public static (double X, double Y) TransformPoint(double x, double y, TemplateOrientation orientation) => orientation switch
        {
            TemplateOrientation.MirrorX => (-x, y),
            TemplateOrientation.MirrorY => (x, -y),
            TemplateOrientation.Both => (-x, -y),
            _ => (x, y),
        };

        private static TemplateMatch MatchOrientation(CatalogTemplate template, TemplateNode node, TemplateOrientation orientation, double tolerance)
        {
            List<MemberPatternSlot> slots = template.MemberPattern.OrderBy(s => s.Slot).ToList();
            double[] angles = slots.Select(s => TransformAngle(s.AngleDeg, orientation)).ToArray();
            List<TemplateNodeMember> members = node.Members.ToList();

            // Búsqueda exhaustiva con poda: pocas ranuras y pocas barras (un nudo tiene 2 a 6).
            int[] best = new int[slots.Count];
            for (int i = 0; i < best.Length; i++) best[i] = -1;
            int bestMatched = -1;
            double bestScore = double.MaxValue;
            int[] current = new int[slots.Count];
            bool[] used = new bool[members.Count];

            void Search(int slotIndex, int matched, double score)
            {
                if (slotIndex == slots.Count)
                {
                    if (matched > bestMatched || (matched == bestMatched && score < bestScore - 1e-9))
                    {
                        bestMatched = matched;
                        bestScore = score;
                        Array.Copy(current, best, current.Length);
                    }
                    return;
                }
                // Poda: ni aun casando todas las ranuras restantes se mejora.
                if (matched + (slots.Count - slotIndex) < bestMatched) return;

                for (int m = 0; m < members.Count; m++)
                {
                    if (used[m]) continue;
                    double deviation = NodeFrame.AngleDifferenceDeg(members[m].AngleDeg, angles[slotIndex]);
                    if (deviation > tolerance + 1e-9) continue;
                    used[m] = true;
                    current[slotIndex] = m;
                    Search(slotIndex + 1, matched + 1, score + deviation);
                    used[m] = false;
                }
                current[slotIndex] = -1;
                Search(slotIndex + 1, matched, score);
            }

            Search(0, 0, 0.0);

            var assignments = new List<SlotAssignment>();
            var assignedMembers = new HashSet<long>();
            for (int i = 0; i < slots.Count; i++)
            {
                TemplateNodeMember? member = best[i] >= 0 ? members[best[i]] : null;
                if (member != null) assignedMembers.Add(member.ElementId);
                assignments.Add(new SlotAssignment(slots[i], angles[i], member));
            }
            List<long> unassigned = members.Where(m => !assignedMembers.Contains(m.ElementId)).Select(m => m.ElementId).ToList();
            return new TemplateMatch(orientation, assignments, unassigned);
        }
    }
}
