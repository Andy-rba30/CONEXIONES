using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Estados de un nudo detectado (sección 3.3 de <c>docs/prompts/fase-8.md</c>).</summary>
    public static class NodeStatus
    {
        /// <summary>Casa con una plantilla, valida y tiene token.</summary>
        public const string Ready = "ready";
        /// <summary>Casa pero la validación da errores.</summary>
        public const string Invalid = "invalid";
        /// <summary>Ninguna plantilla casa todas sus ranuras.</summary>
        public const string NoMatch = "no_match";
        /// <summary>Más de una barra atraviesa el nudo: hace falta elegir el cordón.</summary>
        public const string AmbiguousChord = "ambiguous_chord";
        /// <summary>Los ejes del cordón y de la barra no se cortan (más de 5 mm).</summary>
        public const string Offset = "offset";
        /// <summary>Una sola barra llega y ninguna atraviesa: empalme o extremo suelto.</summary>
        public const string Untyped = "untyped";
        /// <summary>Alguna barra ya está en una conexión del add-in (P8: se salta).</summary>
        public const string AlreadyConnected = "already_connected";
        /// <summary>Excluido por la persona.</summary>
        public const string Excluded = "excluded";
        /// <summary>Todavía sin casar (estado intermedio de la detección).</summary>
        public const string Detected = "detected";

        /// <summary>Verdadero si el nudo tiene cordón y barras y se le puede intentar aplicar una plantilla.</summary>
        public static bool CanMatch(string status) => status == Detected || status == Ready || status == Invalid || status == NoMatch;
    }

    /// <summary>Una barra tal como la ve la detección: eje en mm y tipo.</summary>
    public sealed class DetectorBar
    {
        public DetectorBar(long elementId, Vec3 startMm, Vec3 endMm, string? typeName)
        {
            ElementId = elementId;
            StartMm = startMm;
            EndMm = endMm;
            TypeName = typeName;
        }

        public long ElementId { get; }
        public Vec3 StartMm { get; }
        public Vec3 EndMm { get; }
        public string? TypeName { get; }
        public double LengthMm => StartMm.DistanceTo(EndMm);
        public Vec3 Direction => (EndMm - StartMm).Normalized();

        /// <summary>Pendiente respecto a la horizontal en grados (0 = horizontal), como <c>MemberInfo.SlopeDegrees</c>.</summary>
        public double SlopeDeg => Units.UnitConverter.RadiansToDegrees(Math.Asin(Math.Min(1.0, Math.Abs(Direction.Z))));

        public static DetectorBar FromFacts(MemberModelFacts facts) => new DetectorBar(facts.ElementId, facts.CurveStartMm, facts.CurveEndMm, facts.TypeName);
    }

    /// <summary>Una barra que llega a un nudo, con su ángulo con signo en el marco canónico.</summary>
    public sealed class DetectedMember
    {
        public DetectedMember(long elementId, double angleDeg, string side, string? typeName, bool reachesNode)
        {
            ElementId = elementId;
            AngleDeg = angleDeg;
            Side = side;
            TypeName = typeName;
            ReachesNode = reachesNode;
        }

        public long ElementId { get; }
        public double AngleDeg { get; }
        public string Side { get; }
        public string? TypeName { get; }
        public bool ReachesNode { get; }
    }

    /// <summary>Un nudo detectado: punto de trabajo, cordón, barras y estado geométrico.</summary>
    public sealed class DetectedNode
    {
        public DetectedNode(string name, Vec3 workPointMm)
        {
            Name = name;
            WorkPointMm = workPointMm;
        }

        /// <summary><c>N1</c>, <c>N2</c>… (P12).</summary>
        public string Name { get; set; }

        public Vec3 WorkPointMm { get; set; }

        /// <summary>Cordón elegido (0 si no hay ninguno reconocible).</summary>
        public long ChordElementId { get; set; }

        /// <summary>Verdadero si el cordón atraviesa el nudo; falso si es una barra que llega (extremo de cercha).</summary>
        public bool ChordContinuous { get; set; }

        /// <summary>Todas las barras que atraviesan el nudo (más de una = <c>ambiguous_chord</c>).</summary>
        public List<long> ThroughBarIds { get; } = new List<long>();

        /// <summary>IDs de las barras que llegan al nudo (sin el cordón), en el orden recibido.</summary>
        public List<long> MemberElementIds { get; } = new List<long>();

        /// <summary>Barras con ángulo, solo cuando hay marco (<see cref="Frame"/>).</summary>
        public List<DetectedMember> Members { get; } = new List<DetectedMember>();

        public NodeFrame? Frame { get; set; }

        /// <summary>Estado geométrico: <c>detected</c>, <c>ambiguous_chord</c>, <c>offset</c> o <c>untyped</c>.</summary>
        public string Status { get; set; } = NodeStatus.Detected;

        /// <summary>Explicación corta del estado (en español).</summary>
        public string? StatusDetail { get; set; }

        /// <summary>Verdadero si lo añadió la persona (<c>add_node</c>) o salió de un <c>merge</c>/<c>split</c>.</summary>
        public bool IsManual { get; set; }

        public List<long> AllElementIds
        {
            get
            {
                var ids = new List<long>();
                if (ChordElementId != 0) ids.Add(ChordElementId);
                ids.AddRange(MemberElementIds.Where(id => id != ChordElementId));
                return ids;
            }
        }

        /// <summary>Firma legible: barras por lado y ángulos ("2 +Y (135, 45) · 1 -Y (-135)").</summary>
        public string Signature()
        {
            if (Members.Count == 0) return MemberElementIds.Count + " barra(s) sin marco";
            string Side(string side)
            {
                var angles = Members.Where(m => m.Side == side).Select(m => m.AngleDeg.ToString("0", CultureInfo.InvariantCulture)).ToList();
                return angles.Count == 0 ? "" : angles.Count + " " + side + " (" + string.Join(", ", angles) + ")";
            }
            return string.Join(" · ", new[] { Side("+Y"), Side("-Y") }.Where(s => s.Length > 0));
        }
    }

    /// <summary>Opciones de la detección (de <c>config/catalog.json</c>, decisión P7).</summary>
    public sealed class NodeDetectorOptions
    {
        /// <summary>Distancia para agrupar extremos de barras en un nudo (10 mm).</summary>
        public double ClusterMm { get; set; } = 10.0;

        /// <summary>Distancia máxima del eje de una barra al punto de trabajo para "atraviesa el nudo" (5 mm).</summary>
        public double AxisMaxDistanceMm { get; set; } = 5.0;

        public static NodeDetectorOptions FromConfig(CatalogConfig config) => new NodeDetectorOptions
        {
            ClusterMm = config.NodeClusterMm > 0 ? config.NodeClusterMm : 10.0,
            AxisMaxDistanceMm = config.NodeAxisMaxDistanceMm > 0 ? config.NodeAxisMaxDistanceMm : 5.0,
        };
    }

    /// <summary>
    /// Detección de nudos (sección 3.2 de la propuesta): agrupa los extremos de las barras, busca las que llegan y las
    /// que atraviesan cada grupo, elige el cordón, calcula el marco canónico y los ángulos con signo, y da nombre a los
    /// nudos. Pura geometría de segmentos: se prueba en la nube con cerchas sintéticas.
    /// </summary>
    public static class NodeDetector
    {
        /// <summary>Detecta los nudos de un conjunto de barras. Las barras con eje nulo se ignoran.</summary>
        public static List<DetectedNode> Detect(IReadOnlyList<DetectorBar> bars, NodeDetectorOptions? options = null)
        {
            if (bars == null) throw new ArgumentNullException(nameof(bars));
            options ??= new NodeDetectorOptions();
            var valid = bars.Where(b => b.LengthMm > 1e-6).GroupBy(b => b.ElementId).Select(g => g.First()).ToList();

            // 1. Puntos candidatos: todos los extremos, agrupados a menos de ClusterMm (unión de componentes).
            var endpoints = new List<(Vec3 point, long barId)>();
            foreach (DetectorBar bar in valid)
            {
                endpoints.Add((bar.StartMm, bar.ElementId));
                endpoints.Add((bar.EndMm, bar.ElementId));
            }
            int[] parent = Enumerable.Range(0, endpoints.Count).ToArray();
            int Find(int i)
            {
                while (parent[i] != i)
                {
                    parent[i] = parent[parent[i]];
                    i = parent[i];
                }
                return i;
            }
            for (int i = 0; i < endpoints.Count; i++)
            {
                for (int j = i + 1; j < endpoints.Count; j++)
                {
                    if (endpoints[i].point.DistanceTo(endpoints[j].point) <= options.ClusterMm)
                    {
                        int a = Find(i), b = Find(j);
                        if (a != b) parent[a] = b;
                    }
                }
            }
            var clusters = new Dictionary<int, List<int>>();
            for (int i = 0; i < endpoints.Count; i++)
            {
                int root = Find(i);
                if (!clusters.TryGetValue(root, out List<int>? list))
                {
                    list = new List<int>();
                    clusters[root] = list;
                }
                list.Add(i);
            }

            // 2. Un nudo por grupo: punto de trabajo = media de los extremos; barras que llegan = las de esos extremos.
            var nodes = new List<DetectedNode>();
            foreach (List<int> group in clusters.Values)
            {
                Vec3 sum = Vec3.Zero;
                foreach (int i in group) sum += endpoints[i].point;
                Vec3 center = sum * (1.0 / group.Count);
                var arriving = group.Select(i => endpoints[i].barId).Distinct().ToList();
                nodes.Add(BuildNode("", center, arriving, valid, options));
            }

            // 7. Nombres estables: por la coordenada de mayor extensión, después las otras (sección 3.2, punto 7).
            Name(nodes);
            return nodes;
        }

        /// <summary>
        /// Construye (o reconstruye) un nudo a partir de sus barras: cordón = la que atraviesa (o la más horizontal de
        /// las que llegan), marco canónico y ángulos. <paramref name="forcedChordId"/> fija el cordón (corrección manual).
        /// </summary>
        public static DetectedNode BuildNode(string name, Vec3 workPointMm, IReadOnlyList<long> barIds, IReadOnlyList<DetectorBar> allBars,
            NodeDetectorOptions options, long? forcedChordId = null, bool manual = false)
        {
            if (allBars == null) throw new ArgumentNullException(nameof(allBars));
            options ??= new NodeDetectorOptions();
            var byId = allBars.GroupBy(b => b.ElementId).ToDictionary(g => g.Key, g => g.First());
            var node = new DetectedNode(name, workPointMm) { IsManual = manual };

            // Barras que llegan (un extremo en el nudo) y barras que atraviesan (eje cerca, punto dentro del tramo).
            var arriving = new List<DetectorBar>();
            foreach (long id in barIds.Distinct())
            {
                if (byId.TryGetValue(id, out DetectorBar? bar)) arriving.Add(bar);
            }
            double endTolerance = Math.Max(options.ClusterMm, 1.0);
            foreach (DetectorBar bar in allBars)
            {
                if (arriving.Any(a => a.ElementId == bar.ElementId)) continue;
                if (IsThrough(bar, workPointMm, options.AxisMaxDistanceMm, endTolerance)) node.ThroughBarIds.Add(bar.ElementId);
            }
            // Una barra "que llega" también puede atravesar si su extremo está a más de la tolerancia de agrupación
            // (nudo añadido a mano con el cordón incluido en la lista): se trata como cordón que atraviesa.
            foreach (DetectorBar bar in arriving.ToList())
            {
                bool endsHere = bar.StartMm.DistanceTo(workPointMm) <= endTolerance || bar.EndMm.DistanceTo(workPointMm) <= endTolerance;
                if (!endsHere && IsThrough(bar, workPointMm, options.AxisMaxDistanceMm, endTolerance))
                {
                    node.ThroughBarIds.Add(bar.ElementId);
                    arriving.Remove(bar);
                }
            }

            // 4. Cordón.
            DetectorBar? chord = null;
            if (forcedChordId.HasValue && byId.TryGetValue(forcedChordId.Value, out DetectorBar? forced))
            {
                chord = forced;
                node.ChordContinuous = node.ThroughBarIds.Contains(forced.ElementId);
                arriving.RemoveAll(a => a.ElementId == forced.ElementId);
            }
            else if (node.ThroughBarIds.Count == 1)
            {
                chord = byId[node.ThroughBarIds[0]];
                node.ChordContinuous = true;
            }
            else if (node.ThroughBarIds.Count > 1)
            {
                node.Status = NodeStatus.AmbiguousChord;
                node.StatusDetail = "Atraviesan el nudo " + node.ThroughBarIds.Count + " barras (" + string.Join(", ", node.ThroughBarIds) + "): elige el cordón.";
                chord = node.ThroughBarIds.Select(id => byId[id]).OrderBy(b => b.SlopeDeg).ThenByDescending(b => b.LengthMm).First();
                node.ChordContinuous = true;
            }
            else if (arriving.Count >= 2)
            {
                chord = arriving.OrderBy(b => b.SlopeDeg).ThenByDescending(b => b.LengthMm).First();
                arriving.Remove(chord);
                node.ChordContinuous = false;
            }

            node.ChordElementId = chord?.ElementId ?? 0;
            node.MemberElementIds.AddRange(arriving.Select(a => a.ElementId));

            if (chord == null || arriving.Count == 0)
            {
                node.Status = NodeStatus.Untyped;
                node.StatusDetail = chord == null
                    ? "Una sola barra llega y ninguna atraviesa: empalme o extremo suelto."
                    : "Solo hay cordón: ninguna diagonal ni montante llega al nudo.";
                return node;
            }

            // 5. Marco canónico con el cordón y la barra menos paralela a él; ángulos con signo de cada barra.
            Vec3 chordDirection = chord.Direction;
            DetectorBar planeBar = arriving.OrderByDescending(b => chordDirection.Cross(b.Direction).Length).First();
            if (chordDirection.Cross(planeBar.Direction).Length < 1e-6)
            {
                node.Status = NodeStatus.Untyped;
                node.StatusDetail = "Todas las barras son paralelas al cordón: no definen el plano de la cercha.";
                return node;
            }
            try
            {
                node.Frame = NodeFrame.Compute(chord.StartMm, chord.EndMm, planeBar.StartMm, planeBar.EndMm);
            }
            catch (NodeGeometryException error)
            {
                node.Status = NodeStatus.Offset;
                node.StatusDetail = error.Message;
                return node;
            }

            foreach (DetectorBar bar in arriving)
            {
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(node.Frame, bar.StartMm, bar.EndMm, node.Frame.Origin);
                bool reaches = NodeReach.MemberReachesNode(bar.StartMm, bar.EndMm, node.Frame.Origin);
                node.Members.Add(new DetectedMember(bar.ElementId, Math.Round(NodeFrame.SignedAngleDeg(ux, uy), 2), NodeFrame.SideOf(uy), bar.TypeName, reaches));
            }
            if (node.Status == NodeStatus.Detected) node.StatusDetail = null;
            return node;
        }

        /// <summary>Una barra atraviesa el punto si su eje pasa a menos de <paramref name="axisMm"/> y el punto cae dentro del tramo, lejos de sus extremos.</summary>
        public static bool IsThrough(DetectorBar bar, Vec3 pointMm, double axisMm, double endToleranceMm)
        {
            Vec3 v = bar.EndMm - bar.StartMm;
            double length = v.Length;
            if (length < 1e-9) return false;
            Vec3 u = v * (1.0 / length);
            double projection = (pointMm - bar.StartMm).Dot(u);
            if (projection <= endToleranceMm || projection >= length - endToleranceMm) return false;
            Vec3 closest = bar.StartMm + u * projection;
            return closest.DistanceTo(pointMm) <= axisMm;
        }

        /// <summary>
        /// Da nombre <c>N1…</c> a los nudos: por la coordenada global en la que la cercha se extiende más, después por las
        /// otras dos (todas redondeadas al mm). Los nudos que ya tienen nombre lo conservan.
        /// </summary>
        public static void Name(List<DetectedNode> nodes)
        {
            if (nodes.Count == 0) return;
            double[] spread =
            {
                nodes.Max(n => n.WorkPointMm.X) - nodes.Min(n => n.WorkPointMm.X),
                nodes.Max(n => n.WorkPointMm.Y) - nodes.Min(n => n.WorkPointMm.Y),
                nodes.Max(n => n.WorkPointMm.Z) - nodes.Min(n => n.WorkPointMm.Z),
            };
            int[] order = Enumerable.Range(0, 3).OrderByDescending(i => spread[i]).ThenBy(i => i).ToArray();
            double Coord(DetectedNode n, int axis) => Math.Round(axis == 0 ? n.WorkPointMm.X : axis == 1 ? n.WorkPointMm.Y : n.WorkPointMm.Z, 0);
            var sorted = nodes.OrderBy(n => Coord(n, order[0])).ThenBy(n => Coord(n, order[1])).ThenBy(n => Coord(n, order[2])).ToList();
            var used = new HashSet<string>(nodes.Where(n => n.Name.Length > 0).Select(n => n.Name), StringComparer.OrdinalIgnoreCase);
            int next = 1;
            foreach (DetectedNode node in sorted)
            {
                if (node.Name.Length > 0) continue;
                while (used.Contains("N" + next)) next++;
                node.Name = "N" + next;
                used.Add(node.Name);
                next++;
            }
            nodes.Sort((a, b) => NameNumber(a.Name).CompareTo(NameNumber(b.Name)));
        }

        /// <summary>Siguiente nombre libre (<c>N7</c> si ya hay N1..N6).</summary>
        public static string NextName(IEnumerable<string> existing)
        {
            var used = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);
            int next = 1;
            while (used.Contains("N" + next)) next++;
            return "N" + next;
        }

        private static double NameNumber(string name)
        {
            string digits = new string(name.Skip(1).TakeWhile(char.IsDigit).ToArray());
            double number = double.TryParse(digits, NumberStyles.Integer, CultureInfo.InvariantCulture, out double n) ? n : double.MaxValue;
            // "N5-2" (de un split) va justo después de N5.
            int dash = name.IndexOf('-');
            if (dash > 0 && int.TryParse(name.Substring(dash + 1), out int suffix)) number += suffix / 1000.0;
            return number;
        }
    }
}
