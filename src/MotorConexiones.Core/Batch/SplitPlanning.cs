using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>
    /// Fase 13 (decisión D5): el corte de cada diagonal con <c>splice_hint</c> como <b>paso previo</b> del nudo en K en el plan, y el
    /// empalme del Detalle B como nudo <b>previsto</b> (<c>N5B</c>) que se valida con los extremos calculados antes de partir. Puro
    /// Core: el punto de corte lo da <see cref="SplitPoint"/>; los dos tramos se simulan con <see cref="SplitPreviewFacts"/> (el
    /// original queda como tramo corto, el largo lleva un id provisional negativo hasta que Revit lo cree).
    /// </summary>
    public static class SplitPlanning
    {
        /// <summary>Sufijo del nudo previsto del empalme B: <c>N5</c> → <c>N5B</c> (y <c>N5B2</c> con dos diagonales).</summary>
        public const string SpliceSuffix = "B";

        /// <summary>El id provisional del tramo largo en el plan (negativo: no existe hasta que Revit parte la barra).</summary>
        public static long VirtualLongSegmentId(long elementId) => -Math.Abs(elementId);

        public static bool IsVirtualId(long id) => id < 0;

        /// <summary>
        /// Los pasos de corte de un nudo en K: uno por barra con <c>splice_hint</c> que llega al nudo. Un paso que no se puede
        /// calcular (barra paralela, barra más corta que el 330 más la pila) sale con <see cref="PlanSplitStep.Error"/> y no se ejecuta.
        /// </summary>
        public static List<PlanSplitStep> StepsFor(ConnectionSpec spec, PlanNode node, IModelFacts facts, PanelChoices choices)
        {
            var steps = new List<PlanSplitStep>();
            if (spec == null || node == null || facts == null || !spec.IsType("direct_weld") || spec.Members == null) return steps;
            choices ??= PanelChoices.Default;
            int index = 0;
            foreach (MemberSpec member in spec.Members)
            {
                double? offset = member.SpliceHint?.OffsetFromChordFaceMm;
                if (!offset.HasValue || offset.Value <= 0) continue;
                PlanMember? planMember = node.Members.FirstOrDefault(m => m.ElementId == member.ElementId);
                if (planMember != null && !planMember.ReachesNode) continue;
                index++;
                var step = new PlanSplitStep
                {
                    ElementId = member.ElementId,
                    ChordElementId = member.ChordElementId ?? spec.Chord?.ElementId ?? node.ChordElementId,
                    OffsetFromChordFaceMm = offset.Value,
                    Detail = string.IsNullOrWhiteSpace(member.SpliceHint!.Detail) ? "B" : member.SpliceHint.Detail!,
                    SpliceNode = node.Name + SpliceSuffix + (index == 1 ? "" : index.ToString(CultureInfo.InvariantCulture)),
                };
                MemberModelFacts? bar = facts.GetMemberFacts(member.ElementId);
                EndPlateDetailDefaults? detail = bar != null ? choices.Starter?.EndPlate?.Find(bar.TypeName) : null;
                double thickness = detail?.ThicknessMm ?? SplitPoint.DefaultPlateThicknessMm;
                double gap = Math.Max(0.0, choices.Starter?.EndPlate?.GapMm ?? 0.0);
                try
                {
                    SplitPointResult? result = SplitPoint.FromSpec(spec, member, facts, thickness, gap);
                    if (result == null || bar == null)
                    {
                        step.Error = "La barra " + member.ElementId + " no está en el modelo: no se puede calcular el punto de corte.";
                    }
                    else
                    {
                        SplitPoint.Segments(bar, result); // lanza si el corte cae fuera de la barra
                        step.SplitPointMm = result.SplitPointArray;
                        step.SplitDistanceMm = Math.Round(result.SplitDistanceMm, 1);
                        step.FaceDistanceMm = Math.Round(result.FaceDistanceMm, 1);
                        step.FirstPlateDistanceMm = Math.Round(result.FirstPlateDistanceMm, 1);
                        step.PlateThicknessMm = Math.Round(thickness, 3);
                        step.GapMm = gap;
                        step.ThetaDeg = Math.Round(result.ThetaDeg, 1);
                    }
                }
                catch (NodeGeometryException error)
                {
                    step.Error = error.Message;
                }
                steps.Add(step);
            }
            return steps;
        }

        /// <summary>
        /// El nudo previsto del empalme B de un paso: clase <c>splice</c>, estado <c>after_split</c>, con el arranque <c>end_plate</c> por
        /// perfil (Detalle B para HSS2-1/2) escrito sobre los dos tramos calculados (el largo con id provisional) y validado con esos
        /// extremos. Sin token: el de verdad sale al crear, sobre los tramos reales.
        /// </summary>
        public static PlanNode PredictSplice(PlanNode kNode, PlanSplitStep step, IModelFacts facts, PanelChoices choices, IReadOnlyList<CatalogTemplate>? templates, LimitsConfig? limits, string planId)
        {
            if (kNode == null) throw new ArgumentNullException(nameof(kNode));
            if (step == null) throw new ArgumentNullException(nameof(step));
            if (facts == null) throw new ArgumentNullException(nameof(facts));
            choices ??= PanelChoices.Default;
            limits ??= LimitsConfig.Default;
            long longId = VirtualLongSegmentId(step.ElementId);
            MemberModelFacts? bar = facts.GetMemberFacts(step.ElementId);
            var node = new PlanNode
            {
                Name = step.SpliceNode,
                Status = NodeStatus.AfterSplit,
                Kind = NodeKind.Splice,
                ChordElementId = step.ElementId,
                ChordContinuous = false,
                ChordTypeName = bar?.TypeName,
                MemberElementIds = new List<long> { longId },
                ElementIds = new List<long> { step.ElementId, longId },
                SpliceElementId = null,
                WorkPointMm = step.SplitPointMm ?? kNode.WorkPointMm,
                ConnectionType = "end_plate",
                IsStarter = true,
                IsManual = kNode.IsManual,
                Signature = "empalme previsto de la diagonal " + step.ElementId,
                ColorIndex = kNode.ColorIndex,
                ColorName = kNode.ColorName,
                ColorRgb = kNode.ColorRgb,
            };
            node.Members.Add(new PlanMember { ElementId = longId, AngleDeg = 0.0, Side = "chord", TypeName = bar?.TypeName, ReachesNode = true, EndGapMm = 0.0, AxisOffsetMm = 0.0 });
            if (step.Error != null || bar == null || step.SplitPointMm == null)
            {
                node.StatusDetail = "No se puede prever el empalme: " + (step.Error ?? "la barra no está en el modelo") + ".";
                node.Errors.Add(new ApiError(ErrorCodes.SplitFailed, step.Error ?? "La barra " + step.ElementId + " no está en el modelo.", "nodes[" + kNode.Name + "].split_steps", "Revisa la barra y offset_from_chord_face_mm."));
                return node;
            }
            try
            {
                var preview = new SplitPreviewFacts(facts, bar, new Vec3(step.SplitPointMm[0], step.SplitPointMm[1], step.SplitPointMm[2]), longId);
                TemplateNode templateNode = TemplateNode.FromModelFacts(preview, step.ElementId, new[] { longId });
                StarterResult starter = SpecStarter.ForSplice(templateNode, choices, templates);
                JsonObject spec = TemplateJson.ParseObject(starter.SpecJson, "spec");
                if (spec["source"] is JsonObject source)
                {
                    source["batch_id"] = planId;
                    source["drawing"] = SpecStarter.DrawingName + " · Detalle " + starter.DetailName + " tras partir " + step.ElementId + " a " + step.OffsetFromChordFaceMm.ToString("0", CultureInfo.InvariantCulture);
                }
                node.Spec = spec;
                node.TemplateName = starter.TemplateName;
                node.StatusDetail = "Empalme " + starter.DetailName + " de la diagonal " + step.ElementId + ": se crea después de partirla a " + step.OffsetFromChordFaceMm.ToString("0", CultureInfo.InvariantCulture)
                                    + " (el tramo largo saldrá con un id nuevo; " + longId + " es provisional). " + starter.DefaultsSource + ".";
                ConnectionSpec? parsed = ConnectionSpec.FromJson(node.SpecJson);
                if (parsed != null)
                {
                    NodeFrame frame = SpecFrame.Compute(parsed, preview);
                    ValidationResult validation = SpecValidator.Validate(node.SpecJson, parsed, preview.WithFrame(frame), limits);
                    node.Errors.AddRange(validation.Errors);
                    node.Warnings.AddRange(validation.Warnings);
                    node.IsValid = validation.IsValid;
                    if (!validation.IsValid)
                    {
                        node.StatusDetail += " No valida con los extremos calculados: " + string.Join(", ", validation.Errors.Select(e => e.Code).Distinct()) + ".";
                    }
                }
            }
            catch (CatalogException error)
            {
                node.Errors.Add(error.Error);
                node.StatusDetail = "No se pudo escribir el arranque del empalme B: " + error.Error.Message;
            }
            catch (NodeGeometryException error)
            {
                node.Errors.Add(new ApiError(error.Code, error.Message, "nodes[" + kNode.Name + "].split_steps", error.Hint));
                node.StatusDetail = "No se pudo prever el empalme B: " + error.Message;
            }
            return node;
        }

        /// <summary>
        /// Los hechos del modelo con una barra partida <b>sin tocar el modelo</b>: el original queda como tramo corto (del extremo del
        /// lado del cordón al punto de corte) y el tramo largo responde a un id provisional negativo. Lo demás pasa tal cual.
        /// </summary>
        public sealed class SplitPreviewFacts : IModelFacts
        {
            private readonly IModelFacts _inner;
            private readonly MemberModelFacts _shortSegment;
            private readonly MemberModelFacts _longSegment;
            private readonly NodeFrame? _frame;

            public SplitPreviewFacts(IModelFacts inner, MemberModelFacts bar, Vec3 splitPointMm, long longId, NodeFrame? frame = null)
            {
                _inner = inner ?? throw new ArgumentNullException(nameof(inner));
                if (bar == null) throw new ArgumentNullException(nameof(bar));
                _frame = frame;
                int near = bar.CurveStartMm.DistanceTo(splitPointMm) <= bar.CurveEndMm.DistanceTo(splitPointMm) ? 0 : 1;
                // El extremo "cercano" es el que queda más cerca del punto de corte medido desde el lado del cordón: el corte parte la
                // barra en dos tramos; el corto conserva el extremo del lado del cordón.
                Vec3 nearEnd = near == 0 ? bar.CurveStartMm : bar.CurveEndMm;
                Vec3 farEnd = near == 0 ? bar.CurveEndMm : bar.CurveStartMm;
                _shortSegment = Clone(bar, bar.ElementId, near == 0 ? nearEnd : splitPointMm, near == 0 ? splitPointMm : nearEnd);
                _longSegment = Clone(bar, longId, near == 0 ? splitPointMm : farEnd, near == 0 ? farEnd : splitPointMm);
                _longSegment.UniqueId = bar.UniqueId + "-split";
            }

            public long ShortId => _shortSegment.ElementId;
            public long LongId => _longSegment.ElementId;

            public SplitPreviewFacts WithFrame(NodeFrame frame) => new SplitPreviewFacts(this, frame);

            private SplitPreviewFacts(SplitPreviewFacts other, NodeFrame frame)
            {
                _inner = other._inner;
                _shortSegment = other._shortSegment;
                _longSegment = other._longSegment;
                _frame = frame;
            }

            public string ProjectUniqueId => _inner.ProjectUniqueId;

            public bool ElementExists(long elementId) => elementId == _longSegment.ElementId || _inner.ElementExists(elementId);

            public bool IsStructuralMember(long elementId) => elementId == _longSegment.ElementId || _inner.IsStructuralMember(elementId);

            public MemberModelFacts? GetMemberFacts(long elementId)
            {
                MemberModelFacts? facts;
                if (elementId == _longSegment.ElementId) facts = Clone(_longSegment, _longSegment.ElementId, _longSegment.CurveStartMm, _longSegment.CurveEndMm);
                else if (elementId == _shortSegment.ElementId) facts = Clone(_shortSegment, _shortSegment.ElementId, _shortSegment.CurveStartMm, _shortSegment.CurveEndMm);
                else return _inner.GetMemberFacts(elementId);
                if (_frame != null)
                {
                    // Como RevitModelFacts con marco: el ángulo con signo y la llegada al nudo medidos en el marco del empalme.
                    var (ux, uy) = ConnectionGeometry.GetMemberDirection2D(_frame, facts.CurveStartMm, facts.CurveEndMm, _frame.Origin);
                    facts.AngleInPlaneDeg = Math.Round(NodeFrame.SignedAngleDeg(ux, uy), 1);
                    facts.ConnectsToNode = NodeReach.MemberReachesNode(facts.CurveStartMm, facts.CurveEndMm, _frame.Origin);
                    facts.AxisDistanceMm = Math.Round(NodeReach.AxisDistanceMm(facts.CurveStartMm, facts.CurveEndMm, _frame.Origin), 2);
                    facts.EndGapMm = Math.Round(NodeReach.EndGapMm(facts.CurveStartMm, facts.CurveEndMm, _frame.Origin), 2);
                    facts.AxisOffsetMm = 0.0;
                }
                return facts;
            }

            public IReadOnlyList<string> GetAvailableProfileNames() => _inner.GetAvailableProfileNames();

            public bool CheckClashWithForeignMember(long foreignMemberId, Vec3 startMm, Vec3 endMm) =>
                foreignMemberId == _longSegment.ElementId ? false : _inner.CheckClashWithForeignMember(foreignMemberId, startMm, endMm);

            private static MemberModelFacts Clone(MemberModelFacts source, long id, Vec3 start, Vec3 end) => new MemberModelFacts
            {
                ElementId = id,
                UniqueId = source.UniqueId,
                FamilyName = source.FamilyName,
                TypeName = source.TypeName,
                WidthMm = source.WidthMm,
                HeightMm = source.HeightMm,
                ThicknessMm = source.ThicknessMm,
                SectionRotationDeg = source.SectionRotationDeg,
                CurveStartMm = start,
                CurveEndMm = end,
                AngleInPlaneDeg = source.AngleInPlaneDeg,
                ConnectsToNode = true,
            };
        }
    }
}
