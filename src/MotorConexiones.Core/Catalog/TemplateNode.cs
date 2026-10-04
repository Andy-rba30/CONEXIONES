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
        public TemplateNodeMember(long elementId, double ux, double uy, string? typeName, double widthMm, bool connectsToNode)
        {
            ElementId = elementId;
            Ux = ux;
            Uy = uy;
            AngleDeg = NodeFrame.SignedAngleDeg(ux, uy);
            Side = NodeFrame.SideOf(uy);
            TypeName = typeName;
            WidthMm = widthMm;
            ConnectsToNode = connectsToNode;
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
        public bool ConnectsToNode { get; }
    }

    /// <summary>
    /// El nudo visto por el catálogo (sin Revit): cordón, marco canónico y barras con su ángulo con signo. Se construye
    /// desde <see cref="IModelFacts"/>, en Revit o en las pruebas, con la misma regla que validar y crear.
    /// </summary>
    public sealed class TemplateNode
    {
        private TemplateNode(long chordElementId, MemberModelFacts chord, NodeFrame frame, List<TemplateNodeMember> members)
        {
            ChordElementId = chordElementId;
            ChordTypeName = chord.TypeName;
            ChordStartMm = chord.CurveStartMm;
            ChordEndMm = chord.CurveEndMm;
            Frame = frame;
            Members = members;
        }

        public long ChordElementId { get; }
        public string ChordTypeName { get; }
        public Vec3 ChordStartMm { get; }
        public Vec3 ChordEndMm { get; }
        public NodeFrame Frame { get; }
        public IReadOnlyList<TemplateNodeMember> Members { get; }

        /// <summary>IDs del nudo: el cordón y después las barras, en el orden recibido.</summary>
        public List<long> AllElementIds
        {
            get
            {
                var ids = new List<long> { ChordElementId };
                ids.AddRange(Members.Select(m => m.ElementId));
                return ids;
            }
        }

        public TemplateNodeMember? Find(long elementId) => Members.FirstOrDefault(m => m.ElementId == elementId);

        /// <summary>
        /// Lee el nudo de los hechos del modelo. El plano de la cercha lo define el cordón y la barra menos paralela a
        /// él; como el marco es canónico, el resultado no depende de qué barra sea. Lanza <see cref="CatalogException"/>
        /// con <c>ELEMENT_NOT_FOUND</c>, <c>ELEMENT_NOT_A_MEMBER</c>, <c>NODE_NEEDS_TWO_MEMBERS</c> o los códigos del nudo.
        /// </summary>
        public static TemplateNode FromModelFacts(IModelFacts facts, long chordElementId, IEnumerable<long> memberElementIds)
        {
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            MemberModelFacts chord = Require(facts, chordElementId, "chord_element_id");

            var memberFacts = new List<MemberModelFacts>();
            foreach (long id in memberElementIds ?? Enumerable.Empty<long>())
            {
                if (id == chordElementId || memberFacts.Any(m => m.ElementId == id)) continue;
                memberFacts.Add(Require(facts, id, "element_ids"));
            }
            if (memberFacts.Count == 0)
            {
                throw new CatalogException(ErrorCodes.NodeNeedsTwoMembers,
                    "Hace falta el cordón y al menos una barra que llegue al nudo.", "element_ids",
                    "Selecciona el cordón y las diagonales o montantes del nudo.");
            }

            // Barra que define el plano: la menos paralela al cordón.
            Vec3 chordDirection = (chord.CurveEndMm - chord.CurveStartMm).Normalized();
            MemberModelFacts planeMember = memberFacts
                .OrderByDescending(m => chordDirection.Cross((m.CurveEndMm - m.CurveStartMm).Normalized()).Length)
                .First();

            NodeFrame frame;
            try
            {
                frame = NodeFrame.Compute(chord.CurveStartMm, chord.CurveEndMm, planeMember.CurveStartMm, planeMember.CurveEndMm);
            }
            catch (NodeGeometryException error)
            {
                throw new CatalogException(error.Code, error.Message, "element_ids", error.Hint);
            }

            var members = new List<TemplateNodeMember>();
            foreach (MemberModelFacts m in memberFacts)
            {
                var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(frame, m.CurveStartMm, m.CurveEndMm, frame.Origin);
                bool reaches = NodeReach.MemberReachesNode(m.CurveStartMm, m.CurveEndMm, frame.Origin);
                members.Add(new TemplateNodeMember(m.ElementId, ux, uy, m.TypeName, m.WidthMm, reaches));
            }
            return new TemplateNode(chordElementId, chord, frame, members);
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
