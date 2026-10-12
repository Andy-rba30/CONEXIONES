using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Una plantilla que casa entera con el nudo, en su mejor orientación.</summary>
    public sealed class TemplateCandidate
    {
        public TemplateCandidate(CatalogTemplate template, TemplateMatch match)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            Match = match ?? throw new ArgumentNullException(nameof(match));
        }

        public CatalogTemplate Template { get; }
        public TemplateMatch Match { get; }
        public string TemplateId => Template.TemplateId;
        public string Name => Template.Name;
        public string OrientationName => Match.OrientationName;
        public bool IsMirrored => Match.Orientation != TemplateOrientation.Same;
        public double MaxDeviationDeg => Match.MaxDeviationDeg;
        public double Score => Match.Score;

        /// <summary>"Nudo tipico Detalle D · en espejo (mirror_x) · desvío máx. 1,2°".</summary>
        public string Describe() => Name + " · " + TemplateChooser.OrientationText(Match.Orientation) + " · desvío máx. " + TemplateChooser.Degrees(MaxDeviationDeg);
    }

    /// <summary>
    /// Lo que devuelve <see cref="TemplateChooser.Propose"/>: las plantillas que encajan, ordenadas de mejor a peor, la
    /// elegida (la primera) y, si ninguna encaja, los intentos que lo explican. <see cref="SummaryText"/> es la frase en
    /// español que enseñan la cabecera de la previsualización y <c>conn_catalog_apply</c>.
    /// </summary>
    public sealed class TemplateProposal
    {
        public TemplateProposal(IReadOnlyList<TemplateCandidate> candidates, IReadOnlyList<string> attempts, int templatesCount, IReadOnlyList<double> memberAnglesDeg, string summaryText)
        {
            Candidates = candidates ?? new List<TemplateCandidate>();
            Attempts = attempts ?? new List<string>();
            TemplatesCount = templatesCount;
            MemberAnglesDeg = memberAnglesDeg ?? new List<double>();
            SummaryText = summaryText ?? string.Empty;
        }

        /// <summary>Plantillas que casan todas sus ranuras, de mejor a peor (suma de desvíos, desvío máximo, la más reciente).</summary>
        public IReadOnlyList<TemplateCandidate> Candidates { get; }

        /// <summary>La mejor candidata, o nula si ninguna encaja (o el catálogo está vacío).</summary>
        public TemplateCandidate? Chosen => Candidates.Count > 0 ? Candidates[0] : null;

        public bool HasChoice => Chosen != null;

        /// <summary>Un intento por orientación de cada plantilla que no encajó ("Detalle D → same: ranura 2 (...) sin barra").</summary>
        public IReadOnlyList<string> Attempts { get; }

        /// <summary>Cuántas plantillas se probaron.</summary>
        public int TemplatesCount { get; }

        public bool IsCatalogEmpty => TemplatesCount == 0;

        /// <summary>Ángulos con signo de las barras del nudo, para los textos.</summary>
        public IReadOnlyList<double> MemberAnglesDeg { get; }

        /// <summary>Frase en español: la propuesta y las otras que encajan, o por qué ninguna encaja.</summary>
        public string SummaryText { get; }

        /// <summary>Candidata por <c>template_id</c>, o nula.</summary>
        public TemplateCandidate? Find(string? templateId) =>
            string.IsNullOrWhiteSpace(templateId) ? null : Candidates.FirstOrDefault(c => string.Equals(c.TemplateId, templateId!.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Fase 11: casa un nudo con <b>todas</b> las plantillas del catálogo (las cuatro orientaciones de cada una, con
    /// <see cref="TemplateMatcher.MatchAll"/>) y propone la mejor. Es la regla que el plan de lote aplicaba nudo a nudo en
    /// <c>PlanBuilder</c>, extraída aquí para que el botón Conectar, el panel (Cambiar…) y <c>conn_catalog_apply</c> sin
    /// <c>template_id</c> compartan la misma. Puro Core: se prueba sin Revit.
    /// </summary>
    public static class TemplateChooser
    {
        /// <summary>
        /// <paramref name="requireAllMembers"/> (por defecto sí): una plantilla solo cuenta como candidata si además de casar
        /// todas sus ranuras no deja ninguna barra del nudo sin ranura (con 3 barras seleccionadas una plantilla de 2 dejaría
        /// una barra sin conectar). El plan de lote pasa <c>false</c> y conserva la regla de la Fase 8 (la barra sobrante se
        /// anota en <c>unassigned_members</c>).
        /// </summary>
        public static TemplateProposal Propose(TemplateNode node, IReadOnlyList<CatalogTemplate>? templates, CatalogConfig? config = null, bool requireAllMembers = true)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            templates ??= new List<CatalogTemplate>();
            var angles = node.Members.Select(m => Math.Round(m.AngleDeg, 1)).ToList();

            var candidates = new List<(TemplateCandidate Candidate, int Index)>();
            var attempts = new List<string>();
            var failed = new List<string>();
            for (int i = 0; i < templates.Count; i++)
            {
                CatalogTemplate template = templates[i];
                if (template == null) continue;
                TemplateMatch best = TemplateMatcher.Match(template, node);
                if (best.IsComplete && (!requireAllMembers || best.UnassignedMembers.Count == 0))
                {
                    candidates.Add((new TemplateCandidate(template, best), i));
                }
                else
                {
                    attempts.AddRange(TemplateMatcher.MatchAll(template, node).Select(m => template.Name + " → " + m.Describe()));
                    failed.Add(template.Name + " → " + ShortFailure(best));
                }
            }

            // Fase 12: primero la plantilla que deja menos barras sin ranura y casa más ranuras (una de 2 barras no gana a la de 3
            // en un nudo de tres barras solo por sumar menos desvío); después, como siempre, el menor desvío.
            List<TemplateCandidate> ordered = candidates
                .OrderBy(c => c.Candidate.Match.UnassignedMembers.Count)
                .ThenByDescending(c => c.Candidate.Match.MatchedCount)
                .ThenBy(c => c.Candidate.Score)
                .ThenBy(c => c.Candidate.MaxDeviationDeg)
                .ThenByDescending(c => c.Candidate.Template.CreatedUtc ?? string.Empty, StringComparer.Ordinal)
                .ThenBy(c => c.Index)
                .Select(c => c.Candidate)
                .ToList();

            string summary = Summarize(ordered, failed, templates.Count, angles);
            return new TemplateProposal(ordered, attempts, templates.Count, angles, summary);
        }

        /// <summary>
        /// Fase 12: las plantillas que pueden casar con un nudo por su clase: en un empalme (<see cref="TemplateNode.IsSplice"/>)
        /// las <c>end_plate</c> cuyo cordón sea del mismo perfil (o sin perfil); en un nudo, las <c>gusset_node</c> y
        /// <c>direct_weld</c> (se casan por ángulos). El orden se conserva.
        /// </summary>
        public static List<CatalogTemplate> ForNode(IEnumerable<CatalogTemplate>? templates, TemplateNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            var result = new List<CatalogTemplate>();
            foreach (CatalogTemplate template in templates ?? Enumerable.Empty<CatalogTemplate>())
            {
                if (template == null) continue;
                string type = string.IsNullOrWhiteSpace(template.ConnectionType) ? "gusset_node" : template.ConnectionType.Trim().ToLowerInvariant();
                if (node.IsSplice)
                {
                    if (type != "end_plate") continue;
                    string? profile = template.ChordPattern?.Profile;
                    if (!string.IsNullOrWhiteSpace(profile) && !string.IsNullOrWhiteSpace(node.ChordTypeName) && !Validation.ProfileMatcher.Matches(profile, node.ChordTypeName)) continue;
                    result.Add(template);
                }
                else if (type == "gusset_node" || type == "direct_weld")
                {
                    result.Add(template);
                }
            }
            return result;
        }

        /// <summary>Texto en español de una orientación: "igual", "en espejo (mirror_x)"...</summary>
        public static string OrientationText(TemplateOrientation orientation) => orientation switch
        {
            TemplateOrientation.MirrorX => "en espejo (mirror_x)",
            TemplateOrientation.MirrorY => "en espejo (mirror_y)",
            TemplateOrientation.Both => "en espejo (both)",
            _ => "igual",
        };

        /// <summary>Grados con una cifra decimal y coma: "1,2°".</summary>
        public static string Degrees(double degrees) => degrees.ToString("0.0", CultureInfo.InvariantCulture).Replace('.', ',') + "°";

        /// <summary>Ángulo con signo y signo explícito: "+45,0°", "−135,6°".</summary>
        public static string SignedDegrees(double degrees) => (degrees < 0 ? "−" : "+") + Degrees(Math.Abs(degrees));

        private static string Summarize(List<TemplateCandidate> ordered, List<string> failed, int templatesCount, List<double> angles)
        {
            if (templatesCount == 0)
            {
                return "El catálogo no tiene ninguna plantilla: no hay nada que proponer. Crea primero un nudo y guárdalo como plantilla (Catálogo).";
            }
            if (ordered.Count > 0)
            {
                TemplateCandidate chosen = ordered[0];
                string others = ordered.Count == 1
                    ? "ninguna"
                    : string.Join(", ", ordered.Skip(1).Select(c => c.Name + " (" + OrientationText(c.Match.Orientation) + ", " + Degrees(c.MaxDeviationDeg) + ")"));
                return "Plantilla propuesta: " + chosen.Describe() + ". Otras que encajan: " + others + ".";
            }
            string anglesText = angles.Count == 0 ? "sin barras" : string.Join(", ", angles.Take(angles.Count - 1).Select(SignedDegrees)) + (angles.Count > 1 ? " y " : "") + SignedDegrees(angles[angles.Count - 1]);
            return "Ninguna de las " + templatesCount + " plantilla" + (templatesCount == 1 ? "" : "s") + " encaja con " + angles.Count + " barra" + (angles.Count == 1 ? "" : "s")
                   + " a " + anglesText + ": " + string.Join("; ", failed) + ".";
        }

        /// <summary>"falta barra para la ranura 2 (diagonal −135,6°)" o "sobran barras 123, 456".</summary>
        private static string ShortFailure(TemplateMatch best)
        {
            var parts = new List<string>();
            foreach (SlotAssignment a in best.Assignments.Where(a => !a.IsAssigned))
            {
                parts.Add("falta barra para la ranura " + a.Slot + " (" + (a.Role ?? "barra") + " " + SignedDegrees(a.TemplateAngleDeg) + ")");
            }
            if (best.UnassignedMembers.Count > 0) parts.Add("sobran barras " + string.Join(", ", best.UnassignedMembers));
            if (parts.Count == 0) parts.Add("no casa (" + best.OrientationName + ")");
            return string.Join(", ", parts) + " [" + best.OrientationName + "]";
        }
    }
}
