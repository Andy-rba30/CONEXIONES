using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>Una barra del nudo tal como la ve el catálogo: dirección en el plano local canónico y ángulo con signo.</summary>
    public sealed class TemplateNodeMember
    {
        public TemplateNodeMember(long elementId, double ux, double uy, string? typeName, double widthMm, bool connectsToNode,
            double axisOffsetMm = 0.0, double depthMm = 0.0, double endGapMm = 0.0, long? chordElementId = null, double thicknessMm = 0.0)
        {
            ElementId = elementId;
            Ux = ux;
            Uy = uy;
            AngleDeg = NodeFrame.SignedAngleDeg(ux, uy);
            Side = NodeFrame.SideOf(uy);
            TypeName = typeName;
            WidthMm = widthMm;
            ConnectsToNode = connectsToNode;
            AxisOffsetMm = axisOffsetMm;
            DepthMm = depthMm > 0 ? depthMm : widthMm;
            EndGapMm = endGapMm;
            ChordElementId = chordElementId;
            ThicknessMm = thicknessMm;
        }

        public long ElementId { get; }

        /// <summary>Dirección unitaria en el plano local, del punto de trabajo hacia fuera.</summary>
        public double Ux { get; }
        public double Uy { get; }

        /// <summary>Ángulo con signo desde +X en [−180°, 180°).</summary>
        public double AngleDeg { get; }

        /// <summary><c>+Y</c> o <c>-Y</c>.</summary>
        public string Side { get; }

        public string? TypeName { get; }
        public double WidthMm { get; }

        /// <summary>Fase 12: llega al nudo (regla de los 5 mm, o la de medio canto con excentricidad).</summary>
        public bool ConnectsToNode { get; }

        /// <summary>
        /// Fase 12 (opción A): dónde corta el eje de la barra al eje del cordón, medido desde el punto de trabajo a lo largo de
        /// +X del marco canónico, con signo. 0 cuando concurre.
        /// </summary>
        public double AxisOffsetMm { get; }

        /// <summary>Fase 12: el otro lado de la sección (canto perpendicular al plano); igual al ancho si no se conoce.</summary>
        public double DepthMm { get; }

        /// <summary>Fase 12: distancia del extremo más cercano de la barra al punto de trabajo (0 si llega al eje).</summary>
        public double EndGapMm { get; }

        /// <summary>
        /// Fase 12 (decisión D5): en un nudo en K sobre un empalme, el tramo del cordón al que llega el eje de esta barra cuando
        /// no es el cordón del nudo; nulo si llega al cordón.
        /// </summary>
        public long? ChordElementId { get; }

        /// <summary>Fase 12: espesor de pared del perfil (mm; 0 si no se conoce).</summary>
        public double ThicknessMm { get; }
    }

    /// <summary>
    /// El nudo visto por el catálogo (sin Revit): cordón, marco canónico y barras con su ángulo con signo. Se construye
    /// desde <see cref="IModelFacts"/>, en Revit o en las pruebas, con la misma regla que validar y crear. Fase 12: el
    /// punto de trabajo queda a mitad entre los cortes de todas las barras (opción A), cada barra lleva su desvío, un
    /// <b>empalme</b> (dos tramos colineales) tiene <see cref="IsSplice"/> y un nudo en K sobre un empalme conoce el otro
    /// tramo (<see cref="SpliceElementId"/>).
    /// </summary>
    public sealed class TemplateNode
    {
        private TemplateNode(long chordElementId, MemberModelFacts chord, NodeFrame frame, List<TemplateNodeMember> members)
        {
            ChordElementId = chordElementId;
            ChordTypeName = chord.TypeName;
            ChordWidthMm = chord.WidthMm;
            ChordDepthMm = chord.HeightMm > 0 ? chord.HeightMm : chord.WidthMm;
            ChordThicknessMm = chord.ThicknessMm;
            ChordStartMm = chord.CurveStartMm;
            ChordEndMm = chord.CurveEndMm;
            Frame = frame;
            Members = members;
        }

        public long ChordElementId { get; }
        public string ChordTypeName { get; }

        /// <summary>Fase 11: ancho del perfil del cordón en el plano (mm; 0 si el modelo no lo dice).</summary>
        public double ChordWidthMm { get; }

        /// <summary>Fase 12: el otro lado de la sección del cordón (mm; el ancho si no se conoce).</summary>
        public double ChordDepthMm { get; }

        /// <summary>Fase 12: espesor de pared del cordón (mm; 0 si no se conoce).</summary>
        public double ChordThicknessMm { get; }

        public Vec3 ChordStartMm { get; }
        public Vec3 ChordEndMm { get; }
        public NodeFrame Frame { get; }
        public IReadOnlyList<TemplateNodeMember> Members { get; }

        /// <summary>Fase 12: verdadero si el cordón atraviesa el nudo; falso si termina en él (empalme, extremo de cercha).</summary>
        public bool ChordContinuous { get; private set; } = true;

        /// <summary>Fase 12: verdadero si el nudo es un empalme: el cordón y la única barra son dos tramos colineales.</summary>
        public bool IsSplice { get; private set; }

        /// <summary>Fase 12: en un nudo en K sobre un empalme, el otro tramo del cordón (nulo si no lo hay).</summary>
        public long? SpliceElementId { get; private set; }

        /// <summary>Fase 12: distancia del extremo del cordón al punto de trabajo cuando el cordón termina en el nudo (0 si atraviesa).</summary>
        public double ChordEndGapMm { get; private set; }

        /// <summary>Fase 12: cuánto se separan los cortes extremos de las barras sobre el eje del cordón (0 si concurren).</summary>
        public double EccentricityMm => Frame.EccentricityMm;

        /// <summary>IDs del nudo: el cordón, el otro tramo si lo hay (Fase 12) y después las barras, en el orden recibido.</summary>
        public List<long> AllElementIds
        {
            get
            {
                var ids = new List<long> { ChordElementId };
                if (SpliceElementId.HasValue && SpliceElementId.Value > 0) ids.Add(SpliceElementId.Value);
                ids.AddRange(Members.Select(m => m.ElementId).Where(id => !ids.Contains(id)));
                return ids;
            }
        }

        public TemplateNodeMember? Find(long elementId) => Members.FirstOrDefault(m => m.ElementId == elementId);

        /// <summary>Verdadero si todas las barras están en el mismo lado del cordón (un nudo en K).</summary>
        public bool AllMembersOneSide => Members.Count > 0 && Members.Select(m => m.Side).Distinct().Count() == 1;

        /// <summary>
        /// Lee el nudo de los hechos del modelo. El plano de la cercha lo define el cordón y la barra menos paralela a
        /// él; como el marco es canónico, el resultado no depende de qué barra sea. Fase 12: el origen queda a mitad entre
        /// los cortes de todas las barras (opción A); si la única barra es colineal con el cordón el nudo es un empalme;
        /// <paramref name="spliceElementId"/> es el otro tramo cuando el nudo en K está sobre un empalme (D5) y
        /// <paramref name="chordContinuous"/> dice si el cordón atraviesa. Lanza <see cref="CatalogException"/> con
        /// <c>ELEMENT_NOT_FOUND</c>, <c>ELEMENT_NOT_A_MEMBER</c>, <c>NODE_NEEDS_TWO_MEMBERS</c> o los códigos del nudo.
        /// </summary>
        public static TemplateNode FromModelFacts(IModelFacts facts, long chordElementId, IEnumerable<long> memberElementIds, long? spliceElementId = null, bool chordContinuous = true)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            MemberModelFacts chord = Require(facts, chordElementId, "chord_element_id");

            var memberFacts = new List<MemberModelFacts>();
            foreach (long id in memberElementIds ?? Enumerable.Empty<long>())
            {
                if (id == chordElementId || memberFacts.Any(m => m.ElementId == id)) continue;
                if (spliceElementId.HasValue && id == spliceElementId.Value) continue;
                memberFacts.Add(Require(facts, id, "element_ids"));
            }
            if (memberFacts.Count == 0)
            {
                throw new CatalogException(ErrorCodes.NodeNeedsTwoMembers,
                    "Hace falta el cordón y al menos una barra que llegue al nudo.", "element_ids",
                    "Selecciona el cordón y las diagonales o montantes del nudo.");
            }

            Vec3 chordDirection = (chord.CurveEndMm - chord.CurveStartMm).Normalized();
            double minSin = Math.Sin(Batch.NodeDetectorOptions.MinCrossingAngleDeg * Math.PI / 180.0);
            MemberModelFacts planeMember = memberFacts
                .OrderByDescending(m => chordDirection.Cross((m.CurveEndMm - m.CurveStartMm).Normalized()).Length)
                .First();
            bool collinear = chordDirection.Cross((planeMember.CurveEndMm - planeMember.CurveStartMm).Normalized()).Length < minSin;

            // Empalme: el cordón y un solo tramo colineal. Marco colineal con origen entre los dos extremos que se juntan.
            if (collinear)
            {
                if (memberFacts.Count != 1)
                {
                    throw new CatalogException(ErrorCodes.NodeAxesParallel, "Todas las barras son paralelas al cordón y hay más de un tramo: no es un empalme ni un nudo.", "element_ids",
                        "Un empalme son exactamente dos tramos colineales; un nudo necesita una barra que no sea paralela al cordón.");
                }
                MemberModelFacts other = memberFacts[0];
                Vec3 splicePoint = SplicePoint(chord, other);
                NodeFrame spliceFrame = NodeFrame.ForCollinear(chord.CurveStartMm, chord.CurveEndMm, splicePoint);
                var (sx, sy) = ConnectionGeometry.GetMemberDirection2D(spliceFrame, other.CurveStartMm, other.CurveEndMm, splicePoint);
                double otherGap = NodeReach.EndGapMm(other.CurveStartMm, other.CurveEndMm, splicePoint);
                var spliceMembers = new List<TemplateNodeMember>
                {
                    new TemplateNodeMember(other.ElementId, sx, sy, other.TypeName, other.WidthMm, true, 0.0, other.HeightMm, Math.Round(otherGap, 2), null, other.ThicknessMm),
                };
                return new TemplateNode(chordElementId, chord, spliceFrame, spliceMembers)
                {
                    IsSplice = true,
                    ChordContinuous = false,
                    ChordEndGapMm = Math.Round(NodeReach.EndGapMm(chord.CurveStartMm, chord.CurveEndMm, splicePoint), 2),
                };
            }

            NodeFrame frame;
            try
            {
                var others = memberFacts.Where(m => !ReferenceEquals(m, planeMember)).Select(m => (m.CurveStartMm, m.CurveEndMm)).ToList();
                frame = NodeFrame.Compute(chord.CurveStartMm, chord.CurveEndMm, planeMember.CurveStartMm, planeMember.CurveEndMm, others);
            }
            catch (NodeGeometryException error)
            {
                throw new CatalogException(error.Code, error.Message, "element_ids", error.Hint);
            }

            MemberModelFacts? splice = spliceElementId.HasValue && spliceElementId.Value > 0 ? facts.GetMemberFacts(spliceElementId.Value) : null;
            double maxOffset = Math.Max(NodeReach.DefaultAxisToleranceMm, Math.Max(chord.WidthMm, chord.HeightMm) / 2.0);
            var members = new List<TemplateNodeMember>();
            foreach (MemberModelFacts m in memberFacts)
            {
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, m.CurveStartMm, m.CurveEndMm, frame.Origin);
                double offset = 0.0;
                Vec3 cut = frame.Origin;
                double s = NodeFrame.ChordCutParameter(chord.CurveStartMm, chordDirection, m.CurveStartMm, m.CurveEndMm, out double distance);
                if (!double.IsNaN(s) && distance <= NodeFrame.MaxAxisDistanceMm)
                {
                    cut = chord.CurveStartMm + chordDirection * s;
                    offset = (cut - frame.Origin).Dot(frame.X);
                    if (Math.Abs(offset) < 0.05) offset = 0.0;
                }
                bool reaches = NodeReach.MemberReachesNode(m.CurveStartMm, m.CurveEndMm, frame.Origin)
                               || NodeReach.MemberReachesNodeEccentric(m.CurveStartMm, m.CurveEndMm, frame.Origin, maxOffset);
                double gap = NodeReach.EndGapMm(m.CurveStartMm, m.CurveEndMm, frame.Origin);
                long? reachesOther = splice != null && ReachesOtherSegment(chord, splice, cut, ux, frame) ? splice.ElementId : (long?)null;
                members.Add(new TemplateNodeMember(m.ElementId, ux, uy, m.TypeName, m.WidthMm, reaches, Math.Round(offset, 2), m.HeightMm, Math.Round(gap, 2), reachesOther, m.ThicknessMm));
            }
            return new TemplateNode(chordElementId, chord, frame, members)
            {
                ChordContinuous = chordContinuous && splice == null,
                SpliceElementId = splice?.ElementId,
                ChordEndGapMm = chordContinuous && splice == null ? 0.0 : Math.Round(NodeReach.EndGapMm(chord.CurveStartMm, chord.CurveEndMm, frame.Origin), 2),
            };
        }

        /// <summary>El punto de empalme de dos tramos: a mitad entre los dos extremos más cercanos.</summary>
        public static Vec3 SplicePoint(MemberModelFacts a, MemberModelFacts b)
        {
            Vec3 bestA = a.CurveStartMm, bestB = b.CurveStartMm;
            double best = double.MaxValue;
            foreach (Vec3 pa in new[] { a.CurveStartMm, a.CurveEndMm })
            {
                foreach (Vec3 pb in new[] { b.CurveStartMm, b.CurveEndMm })
                {
                    double d = pa.DistanceTo(pb);
                    if (d < best)
                    {
                        best = d;
                        bestA = pa;
                        bestB = pb;
                    }
                }
            }
            return (bestA + bestB) * 0.5;
        }

        /// <summary>
        /// Decisión D5: la barra llega al otro tramo si el corte de su eje con el eje del cordón cae sobre ese tramo y no sobre el
        /// cordón; si cae justo en el empalme (concurrente), decide hacia dónde sale la barra: hacia el lado del otro tramo.
        /// </summary>
        private static bool ReachesOtherSegment(MemberModelFacts chord, MemberModelFacts other, Vec3 cut, double ux, NodeFrame frame)
        {
            bool onChord = OnSegment(chord, cut);
            bool onOther = OnSegment(other, cut);
            if (onChord && !onOther) return false;
            if (onOther && !onChord) return true;
            // Ambiguo (en el punto de empalme): el lado hacia el que sale la barra. El cordón sigue hacia el lado de su extremo lejano.
            Vec3 chordFar = chord.CurveStartMm.DistanceTo(frame.Origin) >= chord.CurveEndMm.DistanceTo(frame.Origin) ? chord.CurveStartMm : chord.CurveEndMm;
            double chordSide = (chordFar - frame.Origin).Dot(frame.X);
            return Math.Sign(ux) != Math.Sign(chordSide) && Math.Abs(ux) > 1e-6;
        }

        private static bool OnSegment(MemberModelFacts bar, Vec3 point)
        {
            Vec3 v = bar.CurveEndMm - bar.CurveStartMm;
            double length = v.Length;
            if (length < 1e-9) return false;
            double t = (point - bar.CurveStartMm).Dot(v) / (length * length);
            return t > 1e-3 && t < 1.0 - 1e-3;
        }

        /// <summary>
        /// Elige el cordón entre varios IDs con la regla de <c>conn_get_node_info</c>: el más horizontal y, a igualdad,
        /// el más largo. Lanza <see cref="CatalogException"/> si algún ID no es una barra.
        /// </summary>
        public static long ChooseChord(IModelFacts facts, IEnumerable<long> elementIds)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            var candidates = new List<MemberModelFacts>();
            foreach (long id in elementIds ?? Enumerable.Empty<long>())
            {
                if (candidates.Any(c => c.ElementId == id)) continue;
                candidates.Add(Require(facts, id, "element_ids"));
            }
            if (candidates.Count == 0)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "No hay barras entre las que elegir el cordón.", "element_ids",
                    "Selecciona las barras del nudo o pasa sus IDs en 'element_ids'.");
            }
            return candidates
                .OrderBy(c => Math.Abs((c.CurveEndMm - c.CurveStartMm).Normalized().Z))
                .ThenByDescending(c => c.CurveStartMm.DistanceTo(c.CurveEndMm))
                .First().ElementId;
        }

        private static MemberModelFacts Require(IModelFacts facts, long id, string path)
        {
            if (!facts.ElementExists(id))
            {
                throw new CatalogException(ErrorCodes.ElementNotFound, "No existe ningún elemento con id " + id + ".", path,
                    "Selecciona los miembros del nudo en Revit y usa sus IDs.");
            }
            if (!facts.IsStructuralMember(id))
            {
                throw new CatalogException(ErrorCodes.ElementNotAMember, "El elemento " + id + " no es armazón estructural.", path,
                    "Solo se admiten vigas, diagonales y montantes de la categoría Armazón estructural.");
            }
            MemberModelFacts? member = facts.GetMemberFacts(id);
            if (member == null || member.CurveStartMm.DistanceTo(member.CurveEndMm) < 1e-6)
            {
                throw new CatalogException(ErrorCodes.ElementNotAMember, "El elemento " + id + " no tiene línea de ubicación.", path,
                    "Los miembros del nudo deben ser barras con eje (LocationCurve).");
            }
            return member;
        }
    }
}
