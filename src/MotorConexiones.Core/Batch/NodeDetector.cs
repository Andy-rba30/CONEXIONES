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
        /// <summary>Fase 9: la conexión del nudo se creó con el lote (<c>created_connection_id</c>); sus marcas se quitaron.</summary>
        public const string Created = "created";
        /// <summary>Fase 9: el lote intentó crearlo y falló (su grupo se revirtió); conserva especificación y token para reintentar.</summary>
        public const string Failed = "failed";

        /// <summary>Verdadero si el nudo tiene cordón y barras y se le puede intentar aplicar una plantilla.</summary>
        public static bool CanMatch(string status) => status == Detected || status == Ready || status == Invalid || status == NoMatch;
    }

    /// <summary>Una barra tal como la ve la detección: eje en mm, tipo y canto del perfil.</summary>
    public sealed class DetectorBar
    {
        public DetectorBar(long elementId, Vec3 startMm, Vec3 endMm, string? typeName, double depthMm = 0.0)
        {
            ElementId = elementId;
            StartMm = startMm;
            EndMm = endMm;
            TypeName = typeName;
            DepthMm = depthMm > 0 ? depthMm : 0.0;
        }

        public long ElementId { get; }
        public Vec3 StartMm { get; }
        public Vec3 EndMm { get; }
        public string? TypeName { get; }

        /// <summary>
        /// Canto del perfil en mm (la mayor de las dos medidas de la sección, ronda 8b): decide hasta qué distancia del eje
        /// vecino un extremo cortado en la cara del cordón se considera del mismo nudo. 0 si el modelo no lo da.
        /// </summary>
        public double DepthMm { get; }

        public double LengthMm => StartMm.DistanceTo(EndMm);
        public Vec3 Direction => (EndMm - StartMm).Normalized();

        /// <summary>Pendiente respecto a la horizontal en grados (0 = horizontal), como <c>MemberInfo.SlopeDegrees</c>.</summary>
        public double SlopeDeg => Units.UnitConverter.RadiansToDegrees(Math.Asin(Math.Min(1.0, Math.Abs(Direction.Z))));

        public static DetectorBar FromFacts(MemberModelFacts facts) =>
            new DetectorBar(facts.ElementId, facts.CurveStartMm, facts.CurveEndMm, facts.TypeName, Math.Max(facts.WidthMm, facts.HeightMm));
    }

    /// <summary>Una barra que llega a un nudo, con su ángulo con signo en el marco canónico.</summary>
    public sealed class DetectedMember
    {
        public DetectedMember(long elementId, double angleDeg, string side, string? typeName, bool reachesNode, double endGapMm = 0.0)
        {
            ElementId = elementId;
            AngleDeg = angleDeg;
            Side = side;
            TypeName = typeName;
            ReachesNode = reachesNode;
            EndGapMm = endGapMm;
        }

        public long ElementId { get; }
        public double AngleDeg { get; }
        public string Side { get; }
        public string? TypeName { get; }
        public bool ReachesNode { get; }

        /// <summary>
        /// Distancia del extremo real de la barra (su <c>LocationCurve</c>) al punto de trabajo del nudo, en mm (ronda 8b).
        /// 0 cuando la barra llega al eje; unos 20 a 90 mm cuando termina en la cara del cordón.
        /// </summary>
        public double EndGapMm { get; }
    }

    /// <summary>
    /// Un extremo de barra tal como lo usa la agrupación (ronda 8b): el punto real del eje y el punto efectivo, que es el
    /// corte del eje de la barra con el eje de la barra vecina (el cordón que pasa por delante o una barra que llega al
    /// mismo nudo) cuando el extremo queda a menos del alcance de cara de ese eje.
    /// </summary>
    public sealed class BarEnd
    {
        public BarEnd(DetectorBar bar, Vec3 rawMm, Vec3 effectiveMm, long? cutWithElementId, bool neighbourPassesThrough)
        {
            Bar = bar;
            RawMm = rawMm;
            EffectiveMm = effectiveMm;
            CutWithElementId = cutWithElementId;
            NeighbourPassesThrough = neighbourPassesThrough;
        }

        public DetectorBar Bar { get; }

        /// <summary>Extremo real de la <c>LocationCurve</c>.</summary>
        public Vec3 RawMm { get; }

        /// <summary>Punto con el que se agrupa: el corte con el eje vecino, o el extremo real si no hay vecino.</summary>
        public Vec3 EffectiveMm { get; }

        /// <summary>Barra con cuyo eje se cortó el extremo (nulo si se usa el extremo real).</summary>
        public long? CutWithElementId { get; }

        /// <summary>Verdadero si la barra vecina sigue más allá del corte por los dos lados (es un cordón que atraviesa).</summary>
        public bool NeighbourPassesThrough { get; }

        /// <summary>Cuánto se movió el extremo (0 si llega al eje).</summary>
        public double GapMm => RawMm.DistanceTo(EffectiveMm);
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
        /// <summary>Medio canto que se supone cuando el perfil no trae medidas (40 mm: un HSS de 80).</summary>
        public const double DefaultHalfDepthMm = 40.0;

        /// <summary>Dos ejes que se cruzan con menos de este ángulo se tratan como paralelos (empalmes, cordones con quiebro).</summary>
        public const double MinCrossingAngleDeg = 5.0;

        /// <summary>Distancia para agrupar extremos de barras en un nudo (10 mm).</summary>
        public double ClusterMm { get; set; } = 10.0;

        /// <summary>Distancia máxima del eje de una barra al punto de trabajo para "atraviesa el nudo" (5 mm).</summary>
        public double AxisMaxDistanceMm { get; set; } = 5.0;

        /// <summary>
        /// Alcance de cara fijo en mm (ronda 8b): hasta qué distancia del eje vecino se admite el extremo de una barra
        /// cortada en la cara del cordón. 0 = según el canto: medio canto de cada barra más <see cref="ClusterMm"/>.
        /// </summary>
        public double FaceReachMm { get; set; } = 0.0;

        /// <summary>Medio canto de una barra (o el supuesto si no se conoce).</summary>
        public double HalfDepthMm(DetectorBar? bar) => bar != null && bar.DepthMm > 0 ? bar.DepthMm / 2.0 : DefaultHalfDepthMm;

        /// <summary>Alcance de cara entre una barra y su vecina: fijo si está configurado; si no, medio canto de cada una más la agrupación.</summary>
        public double FaceReach(DetectorBar bar, DetectorBar? neighbour) =>
            FaceReachMm > 0 ? FaceReachMm : HalfDepthMm(bar) + HalfDepthMm(neighbour) + Math.Max(ClusterMm, 0.0);

        public static NodeDetectorOptions FromConfig(CatalogConfig config) => new NodeDetectorOptions
        {
            ClusterMm = config.NodeClusterMm > 0 ? config.NodeClusterMm : 10.0,
            AxisMaxDistanceMm = config.NodeAxisMaxDistanceMm > 0 ? config.NodeAxisMaxDistanceMm : 5.0,
            FaceReachMm = config.NodeFaceReachMm > 0 ? config.NodeFaceReachMm : 0.0,
        };
    }

    /// <summary>
    /// Detección de nudos (sección 3.2 de la propuesta): lleva cada extremo de barra al corte de su eje con el eje vecino
    /// (ronda 8b: las diagonales reales terminan en la cara del cordón, no en su eje), agrupa esos puntos, busca las
    /// barras que llegan y las que atraviesan cada grupo, elige el cordón, calcula el marco canónico y los ángulos con
    /// signo, y da nombre a los nudos. Pura geometría de segmentos: se prueba en la nube con cerchas sintéticas y con la
    /// cercha del Hangar reconstruida de los resultados del PC.
    /// </summary>
    public static class NodeDetector
    {
        /// <summary>Detecta los nudos de un conjunto de barras. Las barras con eje nulo se ignoran.</summary>
        public static List<DetectedNode> Detect(IReadOnlyList<DetectorBar> bars, NodeDetectorOptions? options = null)
        {
            if (bars == null) throw new ArgumentNullException(nameof(bars));
            options ??= new NodeDetectorOptions();
            var valid = bars.Where(b => b.LengthMm > 1e-6).GroupBy(b => b.ElementId).Select(g => g.First()).ToList();
            var byId = valid.ToDictionary(b => b.ElementId, b => b);

            // 1. Extremos efectivos: cada extremo va al corte de su eje con el eje vecino (cordón o barra del mismo nudo).
            List<BarEnd> ends = EffectiveEnds(valid, options);

            // 2. Agrupar (unión de componentes): extremos efectivos a menos de ClusterMm, y además los que cortan al MISMO
            //    cordón que atraviesa a menos del alcance de cara a lo largo de él (nudos en K con excentricidad).
            int[] parent = Enumerable.Range(0, ends.Count).ToArray();
            int Find(int i)
            {
                while (parent[i] != i)
                {
                    parent[i] = parent[parent[i]];
                    i = parent[i];
                }
                return i;
            }
            for (int i = 0; i < ends.Count; i++)
            {
                for (int j = i + 1; j < ends.Count; j++)
                {
                    if (ends[i].Bar.ElementId == ends[j].Bar.ElementId) continue;
                    double distance = ends[i].EffectiveMm.DistanceTo(ends[j].EffectiveMm);
                    bool together = distance <= options.ClusterMm;
                    long cutWith = ends[i].CutWithElementId ?? 0;
                    if (!together && cutWith != 0 && ends[j].CutWithElementId == cutWith
                        && ends[i].NeighbourPassesThrough && ends[j].NeighbourPassesThrough && byId.TryGetValue(cutWith, out DetectorBar? chord))
                    {
                        together = distance <= Math.Max(options.FaceReach(ends[i].Bar, chord), options.FaceReach(ends[j].Bar, chord));
                    }
                    if (!together) continue;
                    int a = Find(i), b = Find(j);
                    if (a != b) parent[a] = b;
                }
            }
            var clusters = new Dictionary<int, List<int>>();
            for (int i = 0; i < ends.Count; i++)
            {
                int root = Find(i);
                if (!clusters.TryGetValue(root, out List<int>? list))
                {
                    list = new List<int>();
                    clusters[root] = list;
                }
                list.Add(i);
            }

            // 3. Un nudo por grupo: punto de trabajo = media de los extremos efectivos; barras que llegan = las de esos extremos.
            var nodes = new List<DetectedNode>();
            foreach (List<int> group in clusters.Values)
            {
                Vec3 sum = Vec3.Zero;
                foreach (int i in group) sum += ends[i].EffectiveMm;
                Vec3 center = sum * (1.0 / group.Count);
                var arriving = group.Select(i => ends[i].Bar.ElementId).Distinct().ToList();
                nodes.Add(BuildNode("", center, arriving, valid, options));
            }

            // 7. Nombres estables: por la coordenada de mayor extensión, después las otras (sección 3.2, punto 7).
            Name(nodes);
            return nodes;
        }

        /// <summary>Los dos extremos efectivos de cada barra (ver <see cref="BarEnd"/>), buscando el eje vecino entre <paramref name="bars"/>.</summary>
        public static List<BarEnd> EffectiveEnds(IReadOnlyList<DetectorBar> bars, NodeDetectorOptions options)
        {
            if (bars == null) throw new ArgumentNullException(nameof(bars));
            options ??= new NodeDetectorOptions();
            var ends = new List<BarEnd>(bars.Count * 2);
            foreach (DetectorBar bar in bars)
            {
                if (bar.LengthMm <= 1e-6) continue;
                ends.Add(SnapEnd(bar, bar.StartMm, bars, options));
                ends.Add(SnapEnd(bar, bar.EndMm, bars, options));
            }
            return ends;
        }

        /// <summary>
        /// Extremo efectivo de una barra (ronda 8b). Se busca entre las demás barras la vecina cuyo eje corta al de la barra
        /// (a menos de <c>AxisMaxDistanceMm</c>, como exige <see cref="NodeFrame"/>), con el corte sobre la vecina o como
        /// mucho un alcance más allá de su extremo, y con el extremo real a menos del alcance de cara de ese eje. Gana la
        /// vecina que atraviesa (cordón) y, a igualdad, la de eje más cercano. Los ejes casi paralelos (empalmes) no cuentan.
        /// Sin vecina, el extremo efectivo es el real.
        /// </summary>
        public static BarEnd SnapEnd(DetectorBar bar, Vec3 endMm, IReadOnlyList<DetectorBar> bars, NodeDetectorOptions options)
        {
            if (bar == null) throw new ArgumentNullException(nameof(bar));
            options ??= new NodeDetectorOptions();
            var best = new BarEnd(bar, endMm, endMm, null, false);
            if (bar.LengthMm <= 1e-6) return best;
            Vec3 u = bar.Direction;
            double minSin = Math.Sin(NodeDetectorOptions.MinCrossingAngleDeg * Math.PI / 180.0);
            int bestRank = int.MaxValue;
            double bestDistance = double.MaxValue;
            foreach (DetectorBar other in bars)
            {
                if (other.ElementId == bar.ElementId || other.LengthMm <= 1e-6) continue;
                Vec3 v = other.Direction;
                if (u.Cross(v).Length < minSin) continue;
                if (!ClosestPoints(bar.StartMm, u, other.StartMm, v, out Vec3 p, out Vec3 q, out _, out double tOther)) continue;
                if (p.DistanceTo(q) > options.AxisMaxDistanceMm) continue;

                double reach = options.FaceReach(bar, other);
                double distance = DistanceToLine(endMm, other.StartMm, v);
                if (distance > reach) continue;
                if (tOther < -reach || tOther > other.LengthMm + reach) continue;
                Vec3 cut = (p + q) * 0.5;
                if (cut.DistanceTo(endMm) > NodeReach.DefaultEndToleranceMm) continue;

                bool through = tOther > reach && tOther < other.LengthMm - reach;
                // Si la vecina termina ahí y es la propia barra la que sigue más de un alcance de cara más allá del corte, la
                // barra pasa de largo (un cordón que sobresale en el extremo de la cercha): su extremo real se queda donde está.
                // (Una diagonal que sobrepasa unos milímetros el punto donde se junta con otra sí se lleva al corte.)
                double beyond = (cut - endMm).Dot(u) * (endMm.DistanceTo(bar.StartMm) <= 1e-9 ? 1.0 : -1.0);
                if (!through && beyond > reach) continue;
                int rank = through ? 0 : 1;
                if (rank < bestRank || (rank == bestRank && distance < bestDistance - 1e-9))
                {
                    best = new BarEnd(bar, endMm, cut, other.ElementId, through);
                    bestRank = rank;
                    bestDistance = distance;
                }
            }
            return best;
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

            // Barras que llegan (un extremo en el nudo, aunque sea en la cara del cordón) y barras que atraviesan (eje a
            // menos de AxisMaxDistanceMm del punto y el punto dentro del tramo, a más de un alcance de cara de sus extremos).
            var arriving = new List<DetectorBar>();
            foreach (long id in barIds.Distinct())
            {
                if (byId.TryGetValue(id, out DetectorBar? bar)) arriving.Add(bar);
            }
            foreach (DetectorBar bar in allBars)
            {
                bool listed = arriving.Any(a => a.ElementId == bar.ElementId);
                // Una barra de la lista (llega por su extremo efectivo) solo atraviesa si sigue más de un alcance de cara por
                // los dos lados: es el cordón incluido a mano (nudo añadido, merge, split). Una que no está en la lista
                // atraviesa con que pase el punto más de la agrupación por los dos lados (cordón que sobresale en el extremo).
                if (!PassesThrough(bar, workPointMm, options, listed)) continue;
                node.ThroughBarIds.Add(bar.ElementId);
                arriving.RemoveAll(a => a.ElementId == bar.ElementId);
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
                double gap = Math.Min(bar.StartMm.DistanceTo(workPointMm), bar.EndMm.DistanceTo(workPointMm));
                node.Members.Add(new DetectedMember(bar.ElementId, Math.Round(NodeFrame.SignedAngleDeg(ux, uy), 2), NodeFrame.SideOf(uy), bar.TypeName, reaches, Math.Round(gap, 1)));
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
        /// Atraviesa el nudo con las opciones de la detección: eje a menos de <c>AxisMaxDistanceMm</c> y el punto dentro del
        /// tramo. Con <paramref name="arrives"/> (la barra llega al nudo por un extremo efectivo) hace falta que siga más de un
        /// alcance de cara por los dos lados: una barra que termina en la cara del cordón, aunque sobrepase su eje, llega, no
        /// atraviesa. Sin él basta con la agrupación: un cordón que sobresale unos centímetros en el extremo de la cercha atraviesa.
        /// </summary>
        public static bool PassesThrough(DetectorBar bar, Vec3 pointMm, NodeDetectorOptions options, bool arrives = true)
        {
            options ??= new NodeDetectorOptions();
            double margin = arrives ? Math.Max(options.FaceReach(bar, null), options.ClusterMm) : Math.Max(options.ClusterMm, 1.0);
            return IsThrough(bar, pointMm, options.AxisMaxDistanceMm, margin);
        }

        /// <summary>Distancia de un punto a la recta que pasa por <paramref name="origin"/> con dirección unitaria <paramref name="direction"/>.</summary>
        public static double DistanceToLine(Vec3 point, Vec3 origin, Vec3 direction)
        {
            Vec3 w = point - origin;
            return point.DistanceTo(origin + direction * w.Dot(direction));
        }

        /// <summary>
        /// Puntos más cercanos entre dos rectas (origen y dirección unitaria): <paramref name="p"/> sobre la primera y
        /// <paramref name="q"/> sobre la segunda, con sus parámetros. Falso si son paralelas.
        /// </summary>
        public static bool ClosestPoints(Vec3 originA, Vec3 u, Vec3 originB, Vec3 v, out Vec3 p, out Vec3 q, out double tA, out double tB)
        {
            Vec3 w = originA - originB;
            double b = u.Dot(v);
            double d = u.Dot(w);
            double e = v.Dot(w);
            double denominator = 1.0 - b * b;
            if (denominator < 1e-9)
            {
                p = originA;
                q = originB;
                tA = 0;
                tB = 0;
                return false;
            }
            tA = (b * e - d) / denominator;
            tB = (e - b * d) / denominator;
            p = originA + u * tA;
            q = originB + v * tB;
            return true;
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
