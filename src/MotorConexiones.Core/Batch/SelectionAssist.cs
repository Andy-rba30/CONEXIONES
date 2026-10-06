using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using MotorConexiones.Core.Geometry3D;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Por qué la selección asistida añadió una barra.</summary>
    public static class AddedBarReason
    {
        /// <summary>Cordón que pasa de largo por el extremo de una barra ya elegida (el extremo se corta con su eje).</summary>
        public const string Chord = "chord";

        /// <summary>Barra que llega (por un extremo) a una barra ya elegida.</summary>
        public const string Member = "member";

        /// <summary>Otro tramo del mismo cordón: paralelo y pegado al extremo de uno ya elegido (empalme).</summary>
        public const string Splice = "splice";
    }

    /// <summary>Una barra que la selección asistida añadió, con su motivo y la barra que la trajo.</summary>
    public sealed class AddedBar
    {
        public AddedBar(long elementId, string? typeName, string reason, long touchesElementId, int round)
        {
            ElementId = elementId;
            TypeName = typeName;
            Reason = reason;
            TouchesElementId = touchesElementId;
            Round = round;
        }

        public long ElementId { get; }
        public string? TypeName { get; }

        /// <summary><see cref="AddedBarReason"/>.</summary>
        public string Reason { get; }

        /// <summary>La barra ya elegida a la que esta llega (o por la que pasa de largo).</summary>
        public long TouchesElementId { get; }

        /// <summary>Ronda en la que entró (1 = toca la selección original).</summary>
        public int Round { get; }

        public string ReasonText
        {
            get
            {
                switch (Reason)
                {
                    case AddedBarReason.Chord: return "cordón que pasa de largo por " + TouchesElementId;
                    case AddedBarReason.Splice: return "otro tramo del cordón " + TouchesElementId + " (empalme)";
                    default: return "llega a " + TouchesElementId;
                }
            }
        }

        public string Describe() => ElementId + (string.IsNullOrEmpty(TypeName) ? "" : " " + TypeName) + " (" + ReasonText + ")";
    }

    /// <summary>Opciones de la selección asistida (mejora C6).</summary>
    public sealed class SelectionAssistOptions
    {
        /// <summary>Distancia máxima de los dos extremos de una barra al plano de la cercha para añadirla (50 mm).</summary>
        public double PlaneToleranceMm { get; set; } = 50.0;

        /// <summary>Dos normales que difieren menos de esto (grados) votan el mismo plano.</summary>
        public double NormalClusterDeg { get; set; } = 3.0;

        /// <summary>Tope de barras (selección más añadidas): una cercha tiene decenas; un piso entero de vigas, cientos.</summary>
        public int MaxBars { get; set; } = 400;
    }

    /// <summary>Lo que devolvió la selección asistida.</summary>
    public sealed class SelectionExpansion
    {
        /// <summary>Las barras seleccionadas que existen como barras con eje, en el orden recibido.</summary>
        public List<long> OriginalIds { get; } = new List<long>();

        /// <summary>IDs de la selección que no son barras con eje (se ignoran).</summary>
        public List<long> IgnoredIds { get; } = new List<long>();

        public List<AddedBar> Added { get; } = new List<AddedBar>();

        /// <summary>Barras que tocan la selección pero tienen un extremo fuera del plano de la cercha (correas, riostras).</summary>
        public List<long> SkippedOutOfPlane { get; } = new List<long>();

        /// <summary>Normal del plano de la cercha usado para filtrar (nula si nunca se pudo decidir: solo tramos paralelos).</summary>
        public Vec3? PlaneNormal { get; set; }

        /// <summary>Punto del plano (media de los extremos de la selección).</summary>
        public Vec3 PlaneOrigin { get; set; }

        public int Rounds { get; set; }

        /// <summary>Verdadero si se paró en <see cref="SelectionAssistOptions.MaxBars"/> y puede quedar cercha sin añadir.</summary>
        public bool LimitReached { get; set; }

        public int ChordCount => Added.Count(a => a.Reason == AddedBarReason.Chord);
        public int MemberCount => Added.Count(a => a.Reason == AddedBarReason.Member);
        public int SpliceCount => Added.Count(a => a.Reason == AddedBarReason.Splice);

        /// <summary>La selección original más las añadidas, en ese orden y sin repetir.</summary>
        public List<long> AllIds => OriginalIds.Concat(Added.Select(a => a.ElementId)).Distinct().ToList();

        public List<long> AddedIds => Added.Select(a => a.ElementId).ToList();

        /// <summary>
        /// "Se añadieron 8 barras que tocan la selección: 2 cordones que pasan de largo, 5 barras que llegan y 1 tramo de
        /// cordón. 3 barras fuera del plano de la cercha no se añadieron." / "La selección ya está completa: ninguna barra
        /// más la toca."
        /// </summary>
        public string SummaryText()
        {
            var sentences = new List<string>();
            if (OriginalIds.Count == 0)
            {
                sentences.Add("Ninguna de las barras seleccionadas es una barra de armazón estructural con eje.");
            }
            else if (Added.Count == 0)
            {
                sentences.Add("La selección ya está completa: ninguna barra más la toca.");
            }
            else
            {
                var parts = new List<string>();
                int chords = ChordCount, members = MemberCount, splices = SpliceCount;
                if (chords > 0) parts.Add(chords + (chords == 1 ? " cordón que pasa de largo" : " cordones que pasan de largo"));
                if (members > 0) parts.Add(members + (members == 1 ? " barra que llega" : " barras que llegan"));
                if (splices > 0) parts.Add(splices + (splices == 1 ? " tramo de cordón" : " tramos de cordón"));
                string list = parts.Count <= 1 ? string.Join("", parts) : string.Join(", ", parts.Take(parts.Count - 1)) + " y " + parts[parts.Count - 1];
                sentences.Add((Added.Count == 1 ? "Se añadió 1 barra que toca la selección" : "Se añadieron " + Added.Count + " barras que tocan la selección")
                              + ": " + list + ".");
            }
            if (SkippedOutOfPlane.Count > 0)
            {
                sentences.Add(SkippedOutOfPlane.Count == 1
                    ? "1 barra fuera del plano de la cercha no se añadió."
                    : SkippedOutOfPlane.Count + " barras fuera del plano de la cercha no se añadieron.");
            }
            if (IgnoredIds.Count > 0)
            {
                sentences.Add(IgnoredIds.Count + (IgnoredIds.Count == 1 ? " elemento de la selección no es una barra con eje." : " elementos de la selección no son barras con eje."));
            }
            if (LimitReached) sentences.Add("Se paró en el tope de barras: puede quedar cercha sin añadir (revisa la selección).");
            return string.Join(" ", sentences);
        }
    }

    /// <summary>Lo que la respuesta y el plan guardan de la selección asistida (<c>selection_expansion</c>): solo cuentas, IDs y el texto.</summary>
    public sealed class SelectionExpansionSummary
    {
        [JsonPropertyName("requested_count")]
        public int RequestedCount { get; set; }

        [JsonPropertyName("added_count")]
        public int AddedCount { get; set; }

        [JsonPropertyName("chord_count")]
        public int ChordCount { get; set; }

        [JsonPropertyName("member_count")]
        public int MemberCount { get; set; }

        [JsonPropertyName("splice_count")]
        public int SpliceCount { get; set; }

        [JsonPropertyName("added_element_ids")]
        public List<long> AddedElementIds { get; set; } = new List<long>();

        [JsonPropertyName("skipped_out_of_plane")]
        public List<long> SkippedOutOfPlane { get; set; } = new List<long>();

        [JsonPropertyName("ignored_element_ids")]
        public List<long> IgnoredElementIds { get; set; } = new List<long>();

        [JsonPropertyName("limit_reached")]
        public bool LimitReached { get; set; }

        [JsonPropertyName("rounds")]
        public int Rounds { get; set; }

        [JsonPropertyName("summary_text")]
        public string SummaryText { get; set; } = string.Empty;

        public static SelectionExpansionSummary From(SelectionExpansion expansion)
        {
            if (expansion == null) throw new ArgumentNullException(nameof(expansion));
            return new SelectionExpansionSummary
            {
                RequestedCount = expansion.OriginalIds.Count + expansion.IgnoredIds.Count,
                AddedCount = expansion.Added.Count,
                ChordCount = expansion.ChordCount,
                MemberCount = expansion.MemberCount,
                SpliceCount = expansion.SpliceCount,
                AddedElementIds = expansion.AddedIds,
                SkippedOutOfPlane = expansion.SkippedOutOfPlane.ToList(),
                IgnoredElementIds = expansion.IgnoredIds.ToList(),
                LimitReached = expansion.LimitReached,
                Rounds = expansion.Rounds,
                SummaryText = expansion.SummaryText(),
            };
        }
    }

    /// <summary>
    /// Selección asistida (mejora C6 de <c>docs/propuestas/flujo-intuitivo.md</c>, Fase 10): a partir de una o varias barras,
    /// añade las que las tocan con la <b>misma regla del detector</b> (ronda 8b): el extremo de una barra se lleva al corte de
    /// su eje con el eje de la otra cuando los ejes se cortan y el extremo queda a menos del alcance de cara de ese eje
    /// (<see cref="NodeDetector.SnapEnd"/>). Repite por rondas hasta que nada más se toca (la cercha entera) y se queda en
    /// el <b>plano de la cercha</b>: una barra con un extremo fuera del plano (correa, riostra entre cerchas) no entra. Con
    /// una sola barra el plano no se conoce: lo deciden por votación las barras que la tocan. Pura geometría de
    /// segmentos: se prueba en la nube; en Revit, <c>BatchPlanner.ExpandSelection</c> le pasa todas las barras del documento.
    /// </summary>
    public static class SelectionAssist
    {
        public static SelectionExpansion Expand(IReadOnlyList<long> selectedIds, IReadOnlyList<DetectorBar> candidates,
            NodeDetectorOptions? detector = null, SelectionAssistOptions? options = null)
        {
            if (selectedIds == null) throw new ArgumentNullException(nameof(selectedIds));
            if (candidates == null) throw new ArgumentNullException(nameof(candidates));
            detector ??= new NodeDetectorOptions();
            options ??= new SelectionAssistOptions();

            var result = new SelectionExpansion();
            var byId = new Dictionary<long, DetectorBar>();
            foreach (DetectorBar bar in candidates)
            {
                if (bar.LengthMm > 1e-6 && !byId.ContainsKey(bar.ElementId)) byId[bar.ElementId] = bar;
            }

            var accepted = new List<DetectorBar>();
            var acceptedIds = new HashSet<long>();
            foreach (long id in selectedIds.Distinct())
            {
                if (byId.TryGetValue(id, out DetectorBar? bar))
                {
                    accepted.Add(bar);
                    acceptedIds.Add(id);
                    result.OriginalIds.Add(id);
                }
                else
                {
                    result.IgnoredIds.Add(id);
                }
            }
            if (accepted.Count == 0) return result;

            result.PlaneOrigin = Centroid(accepted);
            result.PlaneNormal = PlaneOf(accepted);
            var skipped = new HashSet<long>();
            List<DetectorBar> frontier = accepted.ToList();
            double minSin = Math.Sin(NodeDetectorOptions.MinCrossingAngleDeg * Math.PI / 180.0);

            while (frontier.Count > 0 && !result.LimitReached)
            {
                result.Rounds++;
                var touching = new List<(DetectorBar Bar, string Reason, DetectorBar Touches)>();
                var seen = new HashSet<long>();
                foreach (DetectorBar bar in frontier)
                {
                    foreach (DetectorBar candidate in byId.Values)
                    {
                        if (acceptedIds.Contains(candidate.ElementId) || seen.Contains(candidate.ElementId) || skipped.Contains(candidate.ElementId)) continue;
                        if (!NearBy(candidate, bar, detector)) continue;
                        string? reason = Touches(candidate, bar, detector, minSin);
                        if (reason == null) continue;
                        seen.Add(candidate.ElementId);
                        touching.Add((candidate, reason, bar));
                    }
                }

                // Sin plano todavía (una barra sola, o solo tramos paralelos): lo votan las barras que tocan la selección.
                if (result.PlaneNormal == null && touching.Count > 0)
                {
                    result.PlaneNormal = Vote(touching.Where(t => t.Reason != AddedBarReason.Splice).Select(t => (t.Touches.Direction, t.Bar.Direction)).ToList(), minSin, options.NormalClusterDeg);
                }

                var added = new List<DetectorBar>();
                foreach (var (candidate, reason, touches) in touching)
                {
                    if (result.PlaneNormal.HasValue && !InPlane(candidate, result.PlaneNormal.Value, result.PlaneOrigin, options.PlaneToleranceMm))
                    {
                        skipped.Add(candidate.ElementId);
                        result.SkippedOutOfPlane.Add(candidate.ElementId);
                        continue;
                    }
                    if (accepted.Count >= options.MaxBars)
                    {
                        result.LimitReached = true;
                        break;
                    }
                    accepted.Add(candidate);
                    acceptedIds.Add(candidate.ElementId);
                    added.Add(candidate);
                    result.Added.Add(new AddedBar(candidate.ElementId, candidate.TypeName, reason, touches.ElementId, result.Rounds));
                }
                frontier = added;
            }
            return result;
        }

        /// <summary>
        /// Motivo por el que <paramref name="candidate"/> toca a <paramref name="bar"/>, o nulo si no la toca:
        /// <see cref="AddedBarReason.Chord"/> si un extremo de <paramref name="bar"/> se corta con el eje de la candidata y
        /// esta pasa de largo; <see cref="AddedBarReason.Member"/> si un extremo de cualquiera de las dos se corta con el eje
        /// de la otra; <see cref="AddedBarReason.Splice"/> si son paralelas y un extremo de una está pegado a un extremo de la otra.
        /// </summary>
        public static string? Touches(DetectorBar candidate, DetectorBar bar, NodeDetectorOptions detector, double minSin)
        {
            if (candidate == null || bar == null || candidate.ElementId == bar.ElementId) return null;
            var pair = new[] { candidate, bar };
            if (candidate.Direction.Cross(bar.Direction).Length < minSin)
            {
                double reach = detector.FaceReach(candidate, bar);
                foreach (Vec3 a in new[] { bar.StartMm, bar.EndMm })
                {
                    foreach (Vec3 b in new[] { candidate.StartMm, candidate.EndMm })
                    {
                        if (a.DistanceTo(b) <= reach) return AddedBarReason.Splice;
                    }
                }
                return null;
            }
            foreach (Vec3 end in new[] { bar.StartMm, bar.EndMm })
            {
                BarEnd snapped = NodeDetector.SnapEnd(bar, end, pair, detector);
                if (snapped.CutWithElementId == candidate.ElementId) return snapped.NeighbourPassesThrough ? AddedBarReason.Chord : AddedBarReason.Member;
            }
            foreach (Vec3 end in new[] { candidate.StartMm, candidate.EndMm })
            {
                BarEnd snapped = NodeDetector.SnapEnd(candidate, end, pair, detector);
                if (snapped.CutWithElementId == bar.ElementId) return AddedBarReason.Member;
            }
            return null;
        }

        /// <summary>
        /// Filtro rápido antes de la geometría fina: las cajas envolventes de las dos barras, ampliadas con la distancia
        /// máxima que admite <see cref="NodeDetector.SnapEnd"/> (el alcance de cara y la tolerancia de extremo), se solapan.
        /// </summary>
        public static bool NearBy(DetectorBar candidate, DetectorBar bar, NodeDetectorOptions detector)
        {
            double margin = Math.Max(detector.FaceReach(candidate, bar), NodeReach.DefaultEndToleranceMm);
            return Overlaps(candidate.StartMm.X, candidate.EndMm.X, bar.StartMm.X, bar.EndMm.X, margin)
                && Overlaps(candidate.StartMm.Y, candidate.EndMm.Y, bar.StartMm.Y, bar.EndMm.Y, margin)
                && Overlaps(candidate.StartMm.Z, candidate.EndMm.Z, bar.StartMm.Z, bar.EndMm.Z, margin);
        }

        private static bool Overlaps(double a0, double a1, double b0, double b1, double margin) =>
            Math.Max(a0, a1) + margin >= Math.Min(b0, b1) && Math.Min(a0, a1) - margin <= Math.Max(b0, b1);

        /// <summary>Verdadero si los dos extremos están a menos de <paramref name="toleranceMm"/> del plano.</summary>
        public static bool InPlane(DetectorBar bar, Vec3 normal, Vec3 origin, double toleranceMm) =>
            Math.Abs((bar.StartMm - origin).Dot(normal)) <= toleranceMm && Math.Abs((bar.EndMm - origin).Dot(normal)) <= toleranceMm;

        /// <summary>Normal del plano que contiene a las barras, o nula si todas son paralelas (una sola, o tramos de un cordón).</summary>
        public static Vec3? PlaneOf(IReadOnlyList<DetectorBar> bars)
        {
            double minSin = Math.Sin(NodeDetectorOptions.MinCrossingAngleDeg * Math.PI / 180.0);
            var pairs = new List<(Vec3, Vec3)>();
            for (int i = 0; i < bars.Count; i++)
            {
                for (int j = i + 1; j < bars.Count; j++) pairs.Add((bars[i].Direction, bars[j].Direction));
            }
            return Vote(pairs, minSin, 3.0);
        }

        /// <summary>
        /// Votación del plano: cada pareja de direcciones no paralelas propone la normal de su plano; gana la normal con más
        /// votos (las que difieren menos de <paramref name="clusterDeg"/> cuentan juntas). Nula si ninguna pareja vale.
        /// </summary>
        public static Vec3? Vote(IReadOnlyList<(Vec3 A, Vec3 B)> pairs, double minSin, double clusterDeg)
        {
            var clusters = new List<(Vec3 Normal, int Votes)>();
            double cosCluster = Math.Cos(clusterDeg * Math.PI / 180.0);
            foreach (var (a, b) in pairs)
            {
                Vec3 cross = a.Cross(b);
                if (cross.Length < minSin) continue;
                Vec3 normal = cross.Normalized();
                int found = -1;
                for (int i = 0; i < clusters.Count; i++)
                {
                    if (Math.Abs(clusters[i].Normal.Dot(normal)) >= cosCluster)
                    {
                        found = i;
                        break;
                    }
                }
                if (found < 0) clusters.Add((normal, 1));
                else clusters[found] = (clusters[found].Normal, clusters[found].Votes + 1);
            }
            if (clusters.Count == 0) return null;
            return clusters.OrderByDescending(c => c.Votes).First().Normal;
        }

        private static Vec3 Centroid(IReadOnlyList<DetectorBar> bars)
        {
            Vec3 sum = Vec3.Zero;
            foreach (DetectorBar bar in bars) sum += bar.StartMm + bar.EndMm;
            return bars.Count == 0 ? Vec3.Zero : sum * (1.0 / (2.0 * bars.Count));
        }
    }
}
