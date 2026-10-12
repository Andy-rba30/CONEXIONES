using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;

namespace MotorConexiones.Revit.Node
{
    /// <summary>
    /// Fase 12: qué nudos hay en una selección de barras, con la misma regla que el plan de lote (<see cref="NodeDetector"/>):
    /// un nudo en K (clase <c>node</c>), un empalme (clase <c>splice</c>: dos tramos colineales) o los dos en el mismo punto
    /// (decisión D5: dos conexiones, el empalme primero). Lo usan Conectar, <c>conn_get_node_info</c> y
    /// <c>conn_catalog_apply</c> para leer el nudo (<see cref="TemplateNode"/>) con su excentricidad, sus desvíos y, en un
    /// nudo en K sobre un empalme, el otro tramo del cordón.
    /// </summary>
    public static class SelectionNodes
    {
        /// <summary>Los nudos detectados en las barras indicadas (los IDs que no son barras con eje se ignoran).</summary>
        public static List<DetectedNode> Detect(IModelFacts facts, IEnumerable<long> ids)
        {
            var bars = new List<DetectorBar>();
            foreach (long id in ids.Distinct())
            {
                try
                {
                    if (!facts.ElementExists(id) || !facts.IsStructuralMember(id)) continue;
                    MemberModelFacts? member = facts.GetMemberFacts(id);
                    if (member != null && member.CurveStartMm.DistanceTo(member.CurveEndMm) > 1e-6) bars.Add(DetectorBar.FromFacts(member));
                }
                catch
                {
                    // Un elemento que no se puede leer no es una barra.
                }
            }
            return NodeDetector.Detect(bars, NodeDetectorOptions.FromConfig(CatalogConfigLoader.Load()));
        }

        /// <summary>Los nudos de verdad: con cordón, con barras y en un estado casable (incluye los empalmes).</summary>
        public static List<DetectedNode> Real(IEnumerable<DetectedNode> nodes) =>
            nodes.Where(n => n.ChordElementId != 0 && n.MemberElementIds.Count >= 1 && NodeStatus.CanMatch(n.Status)).ToList();

        /// <summary>Cuántos puntos distintos ocupan los nudos de verdad (un nudo en K y su empalme cuentan como uno).</summary>
        public static int PointCount(IEnumerable<DetectedNode> nodes) => Real(nodes).Count(n => n.Companion == null);

        /// <summary>El nudo del catálogo a partir del detectado: cordón, barras, otro tramo del empalme y si el cordón atraviesa.</summary>
        public static TemplateNode ToTemplateNode(IModelFacts facts, DetectedNode node) =>
            TemplateNode.FromModelFacts(facts, node.ChordElementId, node.MemberElementIds, node.SpliceElementId, node.ChordContinuous);

        /// <summary>
        /// Los nudos de una selección como <see cref="TemplateNode"/>, el empalme antes que su nudo en K (se crea primero).
        /// Si el detector no ve ningún nudo de verdad (o se fuerza el cordón), la regla de siempre: el cordón más horizontal
        /// y el resto como barras. <paramref name="pointCount"/> dice cuántos puntos distintos hay (más de uno = una cercha).
        /// </summary>
        public static List<TemplateNode> Resolve(IModelFacts facts, IReadOnlyList<long> ids, long? chordElementId, out int pointCount)
        {
            var list = new List<TemplateNode>();
            List<DetectedNode> detected = Detect(facts, ids);
            List<DetectedNode> real = Real(detected);
            pointCount = PointCount(detected);
            if (chordElementId.HasValue || real.Count == 0)
            {
                long chord = chordElementId ?? TemplateNode.ChooseChord(facts, ids);
                list.Add(TemplateNode.FromModelFacts(facts, chord, ids.Where(id => id != chord)));
                if (pointCount == 0) pointCount = 1;
                return list;
            }
            foreach (DetectedNode node in real.OrderBy(n => n.Kind == NodeKind.Splice ? 0 : 1).ThenBy(n => NodeDetector.NameNumber(n.Name)))
            {
                try
                {
                    list.Add(ToTemplateNode(facts, node));
                }
                catch (CatalogException)
                {
                    // Un nudo que el catálogo no puede leer (ejes que no se cortan) no entra; el que llama decide.
                }
            }
            if (list.Count == 0)
            {
                long chord = TemplateNode.ChooseChord(facts, ids);
                list.Add(TemplateNode.FromModelFacts(facts, chord, ids.Where(id => id != chord)));
            }
            return list;
        }

        /// <summary>El primer nudo de la lista cuya clase casa con el tipo de conexión (empalme ↔ <c>end_plate</c>); si no hay, el primero.</summary>
        public static TemplateNode ForType(IReadOnlyList<TemplateNode> nodes, string? connectionType)
        {
            if (nodes == null || nodes.Count == 0) throw new ArgumentException("Sin nudos.", nameof(nodes));
            bool wantsSplice = string.Equals(connectionType, EndPlateValidator.TypeName, StringComparison.OrdinalIgnoreCase);
            return nodes.FirstOrDefault(n => n.IsSplice == wantsSplice) ?? nodes[0];
        }
    }
}
