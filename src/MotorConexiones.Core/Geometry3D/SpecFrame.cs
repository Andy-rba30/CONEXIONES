using System;
using System.Collections.Generic;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;

namespace MotorConexiones.Core.Geometry3D
{
    /// <summary>
    /// Fase 12: <b>la</b> regla para calcular el marco del nudo de una especificación, la misma en validar, previsualizar,
    /// crear y dibujar (Core, Revit y pruebas): cordón = <c>chord.element_id</c>; con un <c>end_plate</c> o cuando la
    /// primera barra es colineal con el cordón, el marco colineal del empalme con origen entre los extremos que se juntan;
    /// si no, el marco canónico con <b>todas</b> las barras (el punto de trabajo a mitad entre los cortes, opción A).
    /// </summary>
    public static class SpecFrame
    {
        /// <summary>Lanza <see cref="NodeGeometryException"/> si los ejes no definen el plano o no se cortan.</summary>
        public static NodeFrame Compute(ConnectionSpec spec, IModelFacts facts)
        {
            if (spec == null) throw new ArgumentNullException(nameof(spec));
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            if (spec.Chord == null || spec.Members == null || spec.Members.Count == 0)
            {
                throw new NodeGeometryException(Validation.ErrorCodes.NodeNeedsTwoMembers, "La especificación necesita el cordón y al menos una barra.", "Rellena chord.element_id y members[*].element_id.");
            }
            MemberModelFacts chord = facts.GetMemberFacts(spec.Chord.ElementId)
                ?? throw new NodeGeometryException(Validation.ErrorCodes.ElementNotFound, "El cordón " + spec.Chord.ElementId + " no está en el modelo.", "Comprueba chord.element_id.");
            MemberModelFacts first = facts.GetMemberFacts(spec.Members[0].ElementId)
                ?? throw new NodeGeometryException(Validation.ErrorCodes.ElementNotFound, "La barra " + spec.Members[0].ElementId + " no está en el modelo.", "Comprueba members[0].element_id.");
            var others = new List<(Vec3 Start, Vec3 End)>();
            for (int i = 1; i < spec.Members.Count; i++)
            {
                MemberModelFacts? other = facts.GetMemberFacts(spec.Members[i].ElementId);
                if (other != null) others.Add((other.CurveStartMm, other.CurveEndMm));
            }
            return Compute(spec.IsType("end_plate"), chord.CurveStartMm, chord.CurveEndMm, first.CurveStartMm, first.CurveEndMm, others);
        }

        /// <summary>La misma regla a partir de los ejes (para el add-in, que lee <c>MemberInfo</c> en vez de hechos).</summary>
        public static NodeFrame Compute(bool isEndPlate, Vec3 chordStart, Vec3 chordEnd, Vec3 firstStart, Vec3 firstEnd, IEnumerable<(Vec3 Start, Vec3 End)>? others)
        {
            Vec3 chordDirection = (chordEnd - chordStart).Normalized();
            Vec3 firstDirection = (firstEnd - firstStart).Normalized();
            double minSin = Math.Sin(5.0 * Math.PI / 180.0);
            if (isEndPlate || chordDirection.Cross(firstDirection).Length < minSin)
            {
                return NodeFrame.ForCollinear(chordStart, chordEnd, SplicePoint(chordStart, chordEnd, firstStart, firstEnd));
            }
            return NodeFrame.Compute(chordStart, chordEnd, firstStart, firstEnd, others);
        }

        /// <summary>El punto de empalme de dos tramos: a mitad entre los dos extremos más cercanos.</summary>
        public static Vec3 SplicePoint(Vec3 aStart, Vec3 aEnd, Vec3 bStart, Vec3 bEnd)
        {
            Vec3 bestA = aStart, bestB = bStart;
            double best = double.MaxValue;
            foreach (Vec3 pa in new[] { aStart, aEnd })
            {
                foreach (Vec3 pb in new[] { bStart, bEnd })
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
    }
}
