using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Resultado de validar la especificación de un nudo (lo que devuelve <c>conn_validate</c>).</summary>
    public sealed class PlanValidation
    {
        public PlanValidation(bool isValid, string? token, IEnumerable<ApiError>? errors, IEnumerable<ApiError>? warnings)
        {
            IsValid = isValid && !string.IsNullOrEmpty(token);
            Token = IsValid ? token : null;
            if (errors != null) Errors.AddRange(errors);
            if (warnings != null) Warnings.AddRange(warnings);
        }

        public bool IsValid { get; }
        public string? Token { get; }
        public List<ApiError> Errors { get; } = new List<ApiError>();
        public List<ApiError> Warnings { get; } = new List<ApiError>();
    }

    /// <summary>Valida una especificación instanciada contra el modelo: en Revit, <c>ValidationService</c>; en las pruebas, <c>SpecValidator</c>.</summary>
    public delegate PlanValidation PlanValidator(string specJson, ConnectionSpec spec);

    /// <summary>Lo que hace falta para construir un plan.</summary>
    public sealed class PlanRequest
    {
        public PlanRequest(IModelFacts facts, PlanValidator validator)
        {
            Facts = facts ?? throw new ArgumentNullException(nameof(facts));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public IModelFacts Facts { get; }
        public PlanValidator Validator { get; }

        /// <summary>Barras seleccionadas (los elementos que no sean barras con eje se ignoran).</summary>
        public List<long> SelectionIds { get; set; } = new List<long>();

        /// <summary>Plantillas a probar, en orden de preferencia (a igual desvío gana la primera).</summary>
        public List<CatalogTemplate> Templates { get; set; } = new List<CatalogTemplate>();

        /// <summary>IDs de plantilla pedidos (vacío = todas); solo informativo, se guarda en el plan.</summary>
        public List<string> RequestedTemplateIds { get; set; } = new List<string>();

        public BatchOverrides Overrides { get; set; } = new BatchOverrides();
        public CatalogConfig Config { get; set; } = CatalogConfig.Default;

        /// <summary>Barras que ya están en una conexión del add-in: ID → <c>connection_id</c>.</summary>
        public Dictionary<long, string> ConnectedMembers { get; set; } = new Dictionary<long, string>();

        /// <summary>Para replanificar: el <c>plan_id</c> del plan anterior (si no, uno nuevo).</summary>
        public string? PlanId { get; set; }

        public string? CreatedUtc { get; set; }
        public string? DocumentTitle { get; set; }
    }

    /// <summary>
    /// Construye el plan (sección 3.2 y 3.3 de la propuesta): detecta los nudos, aplica las correcciones, casa cada nudo
    /// con las plantillas, instancia la especificación con <c>source.batch_id</c> y la valida con el validador recibido.
    /// Puro Core: se prueba en la nube con una cercha sintética; en Revit lo llama <c>BatchPlanner</c>.
    /// </summary>
    public static class PlanBuilder
    {
        public static BatchPlan Build(PlanRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var plan = new BatchPlan
            {
                PlanId = string.IsNullOrWhiteSpace(request.PlanId) ? Guid.NewGuid().ToString("D") : request.PlanId!,
                Document = request.DocumentTitle,
                SelectionIds = request.SelectionIds.Distinct().ToList(),
                TemplateIds = request.RequestedTemplateIds.ToList(),
                Overrides = request.Overrides.Clone(),
            };
            if (!string.IsNullOrWhiteSpace(request.CreatedUtc)) plan.CreatedUtc = request.CreatedUtc!;
            plan.UpdatedUtc = DateTime.UtcNow.ToString("o");
            foreach (CatalogTemplate template in request.Templates) plan.Templates[template.TemplateId] = template.Name;

            // 1. Barras: la selección más las que mencionan las correcciones.
            var bars = new List<DetectorBar>();
            var ignored = new List<long>();
            var missing = new List<long>();
            foreach (long id in plan.SelectionIds.Concat(request.Overrides.ReferencedElementIds()).Distinct())
            {
                if (bars.Any(b => b.ElementId == id)) continue;
                MemberModelFacts? facts = request.Facts.ElementExists(id) && request.Facts.IsStructuralMember(id) ? request.Facts.GetMemberFacts(id) : null;
                if (facts == null || facts.CurveStartMm.DistanceTo(facts.CurveEndMm) < 1e-6)
                {
                    if (plan.SelectionIds.Contains(id)) ignored.Add(id);
                    else missing.Add(id);
                    continue;
                }
                bars.Add(DetectorBar.FromFacts(facts));
            }
            if (ignored.Count > 0)
            {
                plan.Warnings.Add(new ApiError(ErrorCodes.ElementNotAMember,
                    ignored.Count + " elemento(s) de la selección no son barras de armazón estructural con eje y se ignoran: " + string.Join(", ", ignored) + ".",
                    "element_ids", "Selecciona solo cordones, diagonales y montantes."));
            }
            if (missing.Count > 0)
            {
                plan.Warnings.Add(new ApiError(ErrorCodes.ElementNotFound,
                    "Las correcciones mencionan barras que no existen o no son armazón estructural: " + string.Join(", ", missing) + ".",
                    "overrides", "Revisa los IDs de add_node, add_member, chord y split."));
            }

            // 2. Detección y correcciones de forma (merge, split, add_node, chord, barras).
            var options = NodeDetectorOptions.FromConfig(request.Config);
            List<DetectedNode> nodes = NodeDetector.Detect(bars, options);
            ApplyShapeOverrides(nodes, bars, options, request.Overrides, plan);
            NodeDetector.Name(nodes);

            // 3. Nudo a nudo: estado, casado, instanciación y validación.
            int colorIndex = 0;
            foreach (DetectedNode detected in nodes)
            {
                PlanNode node = ToPlanNode(detected, bars);
                plan.Nodes.Add(node);
                if (request.Overrides.Exclude.Contains(node.Name, StringComparer.OrdinalIgnoreCase))
                {
                    node.Status = NodeStatus.Excluded;
                    node.StatusDetail = "Excluido por la persona.";
                    continue;
                }
                foreach (long id in node.ElementIds)
                {
                    if (request.ConnectedMembers.TryGetValue(id, out string? connectionId))
                    {
                        node.ExistingConnectionId = connectionId;
                        break;
                    }
                }
                if (node.ExistingConnectionId != null)
                {
                    if (!request.Overrides.ReplaceExisting)
                    {
                        node.Status = NodeStatus.AlreadyConnected;
                        node.StatusDetail = "Ya tiene la conexión " + node.ExistingConnectionId + " (se salta; replace_existing: true para rehacerla).";
                        continue;
                    }
                    node.ReplacesExisting = true;
                }
                if (node.CanBeMarked)
                {
                    node.ColorIndex = colorIndex;
                    node.ColorName = PlanPalette.NameOf(colorIndex);
                    var (r, g, b) = PlanPalette.RgbOf(colorIndex);
                    node.ColorRgb = new[] { (int)r, (int)g, (int)b };
                    colorIndex++;
                }
                if (!NodeStatus.CanMatch(node.Status)) continue;

                try
                {
                    MatchAndValidate(request, plan, detected, node);
                }
                catch (CatalogException error)
                {
                    node.Status = NodeStatus.Invalid;
                    node.StatusDetail = error.Error.Message;
                    node.Errors.Add(error.Error);
                }
            }

            // 4. Barras que no quedaron en ningún nudo con cordón y barras.
            var used = new HashSet<long>(plan.Nodes.Where(n => n.Status != NodeStatus.Untyped).SelectMany(n => n.ElementIds));
            plan.UnusedElementIds = bars.Select(b => b.ElementId).Where(id => !used.Contains(id)).ToList();
            return plan;
        }

        /// <summary>
        /// Punto de trabajo de un nudo dado solo por sus barras (añadido a mano o separado): el extremo que más barras
        /// comparten (por llegada o por paso) y la media de los extremos que lo rodean.
        /// </summary>
        public static Vec3 WorkPointFor(IReadOnlyList<DetectorBar> barsOfNode, NodeDetectorOptions options, Vec3? near = null)
        {
            if (barsOfNode.Count == 0) return Vec3.Zero;
            var candidates = barsOfNode.SelectMany(b => new[] { b.StartMm, b.EndMm }).ToList();
            Vec3 best = candidates[0];
            int bestScore = -1;
            double bestDistance = double.MaxValue;
            foreach (Vec3 candidate in candidates)
            {
                int score = 0;
                foreach (DetectorBar bar in barsOfNode)
                {
                    bool ends = bar.StartMm.DistanceTo(candidate) <= options.ClusterMm || bar.EndMm.DistanceTo(candidate) <= options.ClusterMm;
                    if (ends || NodeDetector.IsThrough(bar, candidate, options.AxisMaxDistanceMm, options.ClusterMm)) score++;
                }
                // A igual número de barras, el candidato más cercano al punto de referencia (el nudo que se separa).
                double distance = near.HasValue ? candidate.DistanceTo(near.Value) : 0.0;
                if (score > bestScore || (score == bestScore && distance < bestDistance - 1e-9))
                {
                    bestScore = score;
                    bestDistance = distance;
                    best = candidate;
                }
            }
            var around = candidates.Where(c => c.DistanceTo(best) <= options.ClusterMm).ToList();
            Vec3 sum = Vec3.Zero;
            foreach (Vec3 p in around) sum += p;
            return sum * (1.0 / around.Count);
        }

        private static void ApplyShapeOverrides(List<DetectedNode> nodes, List<DetectorBar> bars, NodeDetectorOptions options, BatchOverrides overrides, BatchPlan plan)
        {
            DetectedNode? Find(string name) => nodes.FirstOrDefault(n => string.Equals(n.Name, name, StringComparison.OrdinalIgnoreCase));
            void Missing(string what, string name) => plan.Warnings.Add(new ApiError(ErrorCodes.InvalidRequest,
                "La corrección " + what + " nombra el nudo '" + name + "', que no existe en el plan.", "overrides." + what, "Usa los nombres de nodes[].name."));

            // merge: cada grupo se funde en el primero.
            foreach (List<string> group in overrides.Merge)
            {
                var found = group.Select(Find).ToList();
                if (found.Any(m => m == null) || found.Count < 2)
                {
                    foreach (string name in group.Where(n => Find(n) == null)) Missing("merge", name);
                    continue;
                }
                List<DetectedNode> members = found.Select(m => m!).ToList();
                var ids = members.SelectMany(m => m.AllElementIds.Concat(m.ThroughBarIds)).Distinct().ToList();
                Vec3 sum = Vec3.Zero;
                foreach (DetectedNode m in members) sum += m.WorkPointMm;
                DetectedNode merged = NodeDetector.BuildNode(members[0].Name, sum * (1.0 / members.Count), ids, bars, options, null, manual: true);
                int index = nodes.IndexOf(members[0]);
                foreach (DetectedNode m in members) nodes.Remove(m);
                nodes.Insert(Math.Min(index, nodes.Count), merged);
            }

            // split: un nudo en varios.
            foreach (var pair in overrides.Split)
            {
                DetectedNode? node = Find(pair.Key);
                if (node == null)
                {
                    Missing("split", pair.Key);
                    continue;
                }
                int index = nodes.IndexOf(node);
                nodes.Remove(node);
                int part = 1;
                foreach (List<long> group in pair.Value)
                {
                    var groupBars = bars.Where(b => group.Contains(b.ElementId)).ToList();
                    if (groupBars.Count == 0) continue;
                    string name = part == 1 ? node.Name : node.Name + "-" + part;
                    nodes.Insert(Math.Min(index, nodes.Count), NodeDetector.BuildNode(name, WorkPointFor(groupBars, options, node.WorkPointMm), group, bars, options, null, manual: true));
                    index++;
                    part++;
                }
            }

            // add_node: un nudo que no se detectó, dado por sus barras.
            foreach (var pair in overrides.AddNode)
            {
                var groupBars = bars.Where(b => pair.Value.Contains(b.ElementId)).ToList();
                if (groupBars.Count == 0)
                {
                    plan.Warnings.Add(new ApiError(ErrorCodes.InvalidRequest, "add_node '" + pair.Key + "' no tiene ninguna barra legible.", "overrides.add_node." + pair.Key));
                    continue;
                }
                DetectedNode? existing = Find(pair.Key);
                if (existing != null) nodes.Remove(existing);
                string name = pair.Key.Trim().Length > 0 ? pair.Key.Trim() : NodeDetector.NextName(nodes.Select(n => n.Name));
                nodes.Add(NodeDetector.BuildNode(name, WorkPointFor(groupBars, options), pair.Value, bars, options, null, manual: true));
            }

            // chord, remove_member, add_member: se reconstruye el nudo con sus barras corregidas.
            var touched = overrides.Chord.Keys.Concat(overrides.RemoveMember.Keys).Concat(overrides.AddMember.Keys).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            foreach (string name in touched)
            {
                DetectedNode? node = Find(name);
                if (node == null)
                {
                    Missing("chord/remove_member/add_member", name);
                    continue;
                }
                var ids = node.AllElementIds.Concat(node.ThroughBarIds).Distinct().ToList();
                if (overrides.RemoveMember.TryGetValue(name, out List<long>? removed) && removed != null) ids.RemoveAll(removed.Contains);
                if (overrides.AddMember.TryGetValue(name, out List<long>? added) && added != null) foreach (long id in added) if (!ids.Contains(id)) ids.Add(id);
                long? forcedChord = null;
                if (overrides.Chord.TryGetValue(name, out long chordId) && chordId != 0)
                {
                    forcedChord = chordId;
                    if (!ids.Contains(chordId)) ids.Add(chordId);
                }
                DetectedNode rebuilt = NodeDetector.BuildNode(node.Name, node.WorkPointMm, ids, bars, options, forcedChord, manual: node.IsManual || forcedChord.HasValue);
                if (forcedChord.HasValue && !bars.Any(b => b.ElementId == forcedChord.Value))
                {
                    rebuilt.Status = NodeStatus.AmbiguousChord;
                    rebuilt.StatusDetail = "El cordón indicado (" + forcedChord.Value + ") no es una barra legible del modelo.";
                }
                nodes[nodes.IndexOf(node)] = rebuilt;
            }
        }

        private static PlanNode ToPlanNode(DetectedNode detected, List<DetectorBar> bars)
        {
            var node = new PlanNode
            {
                Name = detected.Name,
                Status = detected.Status,
                StatusDetail = detected.StatusDetail,
                WorkPointMm = new[] { Math.Round(detected.WorkPointMm.X, 1), Math.Round(detected.WorkPointMm.Y, 1), Math.Round(detected.WorkPointMm.Z, 1) },
                ChordElementId = detected.ChordElementId,
                ChordContinuous = detected.ChordContinuous,
                ChordTypeName = bars.FirstOrDefault(b => b.ElementId == detected.ChordElementId)?.TypeName,
                ThroughElementIds = detected.ThroughBarIds.ToList(),
                MemberElementIds = detected.MemberElementIds.ToList(),
                ElementIds = detected.AllElementIds,
                Signature = detected.Signature(),
                IsManual = detected.IsManual,
            };
            foreach (DetectedMember member in detected.Members)
            {
                node.Members.Add(new PlanMember
                {
                    ElementId = member.ElementId,
                    AngleDeg = Math.Round(member.AngleDeg, 1),
                    Side = member.Side,
                    TypeName = member.TypeName,
                    ReachesNode = member.ReachesNode,
                });
            }
            return node;
        }

        private static void MatchAndValidate(PlanRequest request, BatchPlan plan, DetectedNode detected, PlanNode node)
        {
            string? specJson = null;
            ConnectionSpec? spec = null;

            // Especificación editada a mano: sustituye a la plantilla solo en este nudo.
            if (request.Overrides.Spec.TryGetValue(node.Name, out JsonObject? overrideSpec))
            {
                JsonObject copy = TemplateJson.Clone(overrideSpec);
                if (copy["source"] is not JsonObject source)
                {
                    source = new JsonObject();
                    copy["source"] = source;
                }
                source["batch_id"] = plan.PlanId;
                node.Spec = copy;
                node.HasSpecOverride = true;
                specJson = node.SpecJson;
                try
                {
                    spec = ConnectionSpec.FromJson(specJson);
                }
                catch (Exception error)
                {
                    node.Status = NodeStatus.Invalid;
                    node.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La especificación editada del nudo no se puede leer: " + error.Message, "overrides.spec." + node.Name));
                    return;
                }
                node.TemplateId = spec?.Source?.TemplateId;
                if (node.TemplateId != null && plan.Templates.TryGetValue(node.TemplateId, out string? name)) node.TemplateName = name;
                node.StatusDetail = "Especificación editada a mano.";
            }
            else
            {
                // Plantillas candidatas: la indicada para el nudo o todas las pedidas.
                List<CatalogTemplate> candidates = request.Templates;
                if (request.Overrides.Template.TryGetValue(node.Name, out string? wanted))
                {
                    if (wanted == null)
                    {
                        node.Status = NodeStatus.NoMatch;
                        node.StatusDetail = "Sin plantilla por decisión de la persona (template: null).";
                        return;
                    }
                    candidates = request.Templates.Where(t => string.Equals(t.TemplateId, wanted, StringComparison.OrdinalIgnoreCase)).ToList();
                    if (candidates.Count == 0)
                    {
                        node.Status = NodeStatus.NoMatch;
                        node.StatusDetail = "La plantilla '" + wanted + "' no está entre las del plan.";
                        return;
                    }
                }
                if (candidates.Count == 0)
                {
                    node.Status = NodeStatus.NoMatch;
                    node.StatusDetail = "No hay plantillas en el catálogo (o ninguna de las pedidas existe).";
                    return;
                }

                TemplateNode templateNode = TemplateNode.FromModelFacts(request.Facts, detected.ChordElementId, detected.MemberElementIds);
                CatalogTemplate? best = null;
                TemplateMatch? bestMatch = null;
                foreach (CatalogTemplate template in candidates)
                {
                    TemplateMatch match = TemplateMatcher.Match(template, templateNode);
                    if (!match.IsComplete)
                    {
                        node.Attempts.AddRange(TemplateMatcher.MatchAll(template, templateNode).Select(m => template.Name + " → " + m.Describe()));
                        continue;
                    }
                    if (bestMatch == null || match.Score < bestMatch.Score - 1e-9)
                    {
                        best = template;
                        bestMatch = match;
                    }
                }
                if (best == null || bestMatch == null)
                {
                    node.Status = NodeStatus.NoMatch;
                    node.StatusDetail = "Ninguna plantilla casa todas sus ranuras con las barras del nudo.";
                    return;
                }

                InstantiationResult instantiation = TemplateInstantiator.Instantiate(best, bestMatch, templateNode, request.Config, plan.PlanId);
                node.TemplateId = best.TemplateId;
                node.TemplateName = best.Name;
                node.Orientation = bestMatch.OrientationName;
                node.IsMirrored = bestMatch.Orientation != TemplateOrientation.Same;
                node.MaxDeviationDeg = Math.Round(bestMatch.MaxDeviationDeg, 2);
                node.Match = MatchToJson(bestMatch);
                node.Attempts.Clear();
                node.Warnings.AddRange(instantiation.Warnings);
                node.Spec = TemplateJson.ParseObject(instantiation.SpecJson, "spec");
                specJson = instantiation.SpecJson;
                spec = instantiation.Spec;
            }

            if (spec == null || specJson == null)
            {
                node.Status = NodeStatus.Invalid;
                node.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid, "La especificación del nudo quedó vacía.", "overrides.spec." + node.Name));
                return;
            }

            PlanValidation validation = request.Validator(specJson, spec);
            node.Errors.AddRange(validation.Errors);
            node.Warnings.AddRange(validation.Warnings);
            node.IsValid = validation.IsValid;
            node.ValidationToken = validation.Token;
            node.Status = validation.IsValid ? NodeStatus.Ready : NodeStatus.Invalid;
            if (!validation.IsValid && node.StatusDetail == null)
            {
                node.StatusDetail = validation.Errors.Count + " error(es) de validación: " + string.Join(", ", validation.Errors.Select(e => e.Code).Distinct()) + ".";
            }
        }

        /// <summary>El <c>match</c> de <c>conn_catalog_apply</c> como objeto JSON, para el plan.</summary>
        public static JsonObject MatchToJson(TemplateMatch match)
        {
            var assignments = new JsonArray();
            foreach (SlotAssignment a in match.Assignments)
            {
                assignments.Add(new JsonObject
                {
                    ["slot"] = a.Slot,
                    ["role"] = a.Role,
                    ["element_id"] = a.ElementId,
                    ["template_angle_deg"] = Math.Round(a.TemplateAngleDeg, 2),
                    ["model_angle_deg"] = a.ModelAngleDeg.HasValue ? Math.Round(a.ModelAngleDeg.Value, 2) : (double?)null,
                    ["deviation_deg"] = a.IsAssigned ? Math.Round(a.DeviationDeg, 2) : (double?)null,
                    ["side"] = a.Member?.Side,
                    ["model_type_name"] = a.Member?.TypeName,
                    ["template_profile"] = a.TemplateProfile,
                    ["profile_policy"] = a.ProfilePolicy,
                });
            }
            var unassigned = new JsonArray();
            foreach (long id in match.UnassignedMembers) unassigned.Add(id);
            return new JsonObject
            {
                ["orientation"] = match.OrientationName,
                ["is_complete"] = match.IsComplete,
                ["matched_count"] = match.MatchedCount,
                ["score_deg"] = Math.Round(match.Score, 2),
                ["max_deviation_deg"] = Math.Round(match.MaxDeviationDeg, 2),
                ["assignments"] = assignments,
                ["unassigned_members"] = unassigned,
                ["description"] = match.Describe(),
            };
        }
    }
}
