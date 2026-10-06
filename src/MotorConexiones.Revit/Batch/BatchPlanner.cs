using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>Lo que se pide al planificador (desde <c>conn_batch_plan</c> o desde la ventana).</summary>
    public sealed class BatchPlanInput
    {
        /// <summary>Barras seleccionadas; vacío = las del plan anterior (<see cref="PlanId"/>).</summary>
        public List<long> ElementIds { get; set; } = new List<long>();

        /// <summary>Plantillas a probar; vacío = todas las del catálogo (o las del plan anterior).</summary>
        public List<string> TemplateIds { get; set; } = new List<string>();

        /// <summary>Correcciones nuevas, que se acumulan sobre las del plan anterior.</summary>
        public BatchOverrides? Overrides { get; set; }

        /// <summary>Plan a replanificar (mismo <c>plan_id</c>, misma selección si no se pasa otra).</summary>
        public string? PlanId { get; set; }

        /// <summary>Si se olvidan las correcciones acumuladas y se parte de cero (mismo plan_id).</summary>
        public bool ResetOverrides { get; set; }

        /// <summary>Poner las marcas en la vista activa (por defecto sí).</summary>
        public bool Mark { get; set; } = true;
    }

    /// <summary>Lo que quitó <see cref="BatchPlanner.DiscardAndClean"/>.</summary>
    public sealed class DiscardResult
    {
        /// <summary>Colores restaurados y marcadores borrados de los planes en memoria.</summary>
        public int RemovedMarks { get; set; }

        /// <summary>Otros planes del documento que estaban marcados y se desmarcaron (siguen en memoria).</summary>
        public int OtherPlansUnmarked { get; set; }

        /// <summary>Marcadores que no pertenecían a ningún plan en memoria y se quitaron igualmente.</summary>
        public int OrphanMarkers { get; set; }

        /// <summary>Marcadores de MotorConexiones que quedan en el documento (debe ser 0).</summary>
        public int RemainingMarkers { get; set; }

        public string Describe() =>
            RemovedMarks + " marca(s) quitadas"
            + (OtherPlansUnmarked > 0 ? ", " + OtherPlansUnmarked + " plan(es) más desmarcados" : "")
            + (OrphanMarkers > 0 ? ", " + OrphanMarkers + " marcador(es) huérfanos quitados" : "")
            + "; quedan " + RemainingMarkers + " marcadores en el documento.";
    }

    /// <summary>
    /// Selección → <see cref="RevitModelFacts"/> → <see cref="PlanBuilder"/> (validación con <see cref="ValidationService"/>)
    /// → marcas en el modelo. Lo usan la operación <c>batch_plan</c> y el botón Planificar lote. Las marcas van dentro
    /// de un <see cref="OperationScope"/> (una entrada de deshacer por planificación).
    /// </summary>
    public static class BatchPlanner
    {
        /// <summary>Planifica (o replanifica) y deja el plan en <see cref="PlanRegistry"/>. Lanza <see cref="CatalogException"/> si la petición no sirve.</summary>
        public static BatchPlan Plan(Document document, UIApplication? uiApplication, BatchPlanInput input, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (input == null) throw new ArgumentNullException(nameof(input));
            warnings ??= new List<ApiError>();

            BatchPlan? previous = string.IsNullOrWhiteSpace(input.PlanId) ? null : PlanRegistry.Get(input.PlanId);
            if (!string.IsNullOrWhiteSpace(input.PlanId) && previous == null)
            {
                throw new CatalogException(ErrorCodes.PlanNotFound, "No hay ningún plan con plan_id '" + input.PlanId + "' en memoria (se descartó o Revit se reinició).", "plan_id",
                    "Vuelve a planificar sin plan_id con la selección de la cercha.");
            }

            List<long> ids = input.ElementIds.Count > 0 ? input.ElementIds.Distinct().ToList() : previous?.SelectionIds.ToList() ?? new List<long>();
            if (ids.Count < 2)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest,
                    "Se necesitan al menos 2 barras para planificar (recibidas: " + ids.Count + ").", "element_ids",
                    "Selecciona en Revit los cordones y todas las diagonales y montantes de la cercha, o pasa sus IDs en 'element_ids'.");
            }

            CatalogConfig config = CatalogConfigLoader.Load();
            CatalogStore store = CatalogConfigLoader.OpenStore(config);
            List<string> templateIds = input.TemplateIds.Count > 0 ? input.TemplateIds : previous?.TemplateIds ?? new List<string>();
            List<CatalogTemplate> templates = LoadTemplates(store, templateIds, warnings);

            BatchOverrides overrides = previous == null || input.ResetOverrides ? new BatchOverrides() : previous.Overrides.Clone();
            overrides = overrides.MergeWith(input.Overrides);

            var facts = new RevitModelFacts(document);
            var request = new PlanRequest(facts, (json, spec) => Validate(document, json, spec))
            {
                SelectionIds = ids,
                Templates = templates,
                RequestedTemplateIds = templateIds,
                Overrides = overrides,
                Config = config,
                ExistingConnections = ExistingConnections(document),
                LastReport = previous?.LastReport,
                PlanId = previous?.PlanId,
                CreatedUtc = previous?.CreatedUtc,
                DocumentTitle = document.Title,
            };

            // Las marcas del plan anterior se quitan y se ponen las nuevas en la misma operación (una entrada de deshacer).
            // Cierre de la Fase 8: en un documento solo hay un plan marcado. Si otro plan del mismo documento tenía marcas
            // (en la ronda 8b, el del puente cuando la cinta planificó una selección nueva), se quitan también y se avisa con
            // PLAN_MARKS_REPLACED; ese plan sigue en memoria sin marcas. Si no, al descartar el nuevo quedaban los marcadores
            // del viejo sin color (36 cubos en el paso 8b-6).
            BatchPlan plan;
            string opId = previous?.PlanId ?? Guid.NewGuid().ToString("D");
            List<BatchPlan> othersMarked = input.Mark
                ? OtherMarkedPlans(document, previous?.PlanId)
                : new List<BatchPlan>();
            // Cierre de la 8c: si la operación falla, Revit deshace el grupo y el modelo conserva las marcas viejas; los planes
            // en memoria tienen que seguir diciéndolo (si no, esos cubos quedaban huérfanos para siempre).
            var saved = new List<PlanMarkState>();
            if (previous != null) saved.Add(PlanMarks.Capture(previous));
            saved.AddRange(othersMarked.Select(PlanMarks.Capture));
            try
            {
                using (var scope = new OperationScope(document, uiApplication, "batch_plan", opId, warnings))
                {
                    if ((previous != null && previous.IsMarked) || othersMarked.Count > 0)
                    {
                        using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: quitar marcas del plan"))
                        {
                            if (previous != null && previous.IsMarked) PlanMarks.Remove(document, previous, warnings);
                            foreach (BatchPlan other in othersMarked)
                            {
                                PlanMarks.Remove(document, other, warnings);
                                warnings.Add(new ApiError(ErrorCodes.PlanMarksReplaced,
                                    "Se quitaron las marcas del plan " + other.PlanId + " (sigue en memoria, sin marcas): en un documento solo se marca un plan a la vez.",
                                    "plan_id", "Para volver a verlo, replanifica con su plan_id; para olvidarlo, conn_batch_plan_discard con ese plan_id."));
                            }
                            scope.CommitOrThrow(clean);
                        }
                    }
                    plan = PlanBuilder.Build(request);
                    // Ronda 8c: el color de cada nudo es el de su estado (verde, ámbar, rojo, gris), no el de la paleta por nudo, y
                    // si el catálogo no tiene ninguna plantilla el plan lo dice en español (CATALOG_EMPTY) en vez de salir todo no_match.
                    PlanAdvice.ApplyStatusColors(plan);
                    if (templates.Count == 0) plan.Warnings.Add(PlanAdvice.CatalogEmptyWarning());
                    if (input.Mark)
                    {
                        using (Transaction mark = scope.StartTransaction(document, "MotorConexiones: marcas del plan"))
                        {
                            PlanMarks.Apply(document, plan, warnings);
                            scope.CommitOrThrow(mark);
                        }
                    }
                    scope.Commit();
                }
            }
            catch
            {
                foreach (PlanMarkState state in saved) state.Restore();
                throw;
            }

            PlanRegistry.Put(plan);
            JsonLineLogger.Write(new
            {
                @event = "batch_plan",
                plan_id = plan.PlanId,
                replan = previous != null,
                element_ids = ids.Count,
                templates = templates.Select(t => t.TemplateId).ToList(),
                summary = plan.Summary(),
                marked = plan.IsMarked,
                overrides = overrides.ToJson(),
            });
            return plan;
        }

        /// <summary>Quita las marcas de un plan y lo olvida. Devuelve cuántas marcas se quitaron.</summary>
        public static int Discard(Document document, UIApplication? uiApplication, BatchPlan plan, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            int removed = 0;
            PlanMarkState saved = PlanMarks.Capture(plan);
            try
            {
                using (var scope = new OperationScope(document, uiApplication, "batch_plan_discard", plan.PlanId, warnings))
                {
                    using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: descartar plan"))
                    {
                        removed = PlanMarks.Remove(document, plan, warnings);
                        scope.CommitOrThrow(clean);
                    }
                    scope.Commit();
                }
            }
            catch
            {
                saved.Restore();
                throw;
            }
            PlanRegistry.Remove(plan.PlanId);
            JsonLineLogger.Write(new { @event = "batch_plan_discard", plan_id = plan.PlanId, removed_marks = removed });
            return removed;
        }

        /// <summary>
        /// <b>Descartar plan</b> desde la ventana (cierre de la ronda 8c): quita las marcas de este plan, las de cualquier otro
        /// plan marcado del mismo documento y los marcadores huérfanos (de planes que el add-in ya no recuerda), y olvida el
        /// plan. En la ronda 8c, Descartar desde la ventana quitó los colores pero dejó 33 cubos en gris: eran marcadores de
        /// otro plan que el registro ya no tenía por marcado. Ahora, tras Descartar, no queda <b>ningún</b> cubo ni rombo en el
        /// documento, igual que <c>conn_batch_plan_discard</c> con <c>all: true</c>; los demás planes siguen en memoria sin
        /// marcas. Una sola entrada de deshacer; si falla, el modelo y los planes quedan como estaban.
        /// </summary>
        public static DiscardResult DiscardAndClean(Document document, UIApplication? uiApplication, BatchPlan plan, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var result = new DiscardResult();
            List<BatchPlan> others = OtherMarkedPlans(document, plan.PlanId);
            var saved = new List<PlanMarkState> { PlanMarks.Capture(plan) };
            saved.AddRange(others.Select(PlanMarks.Capture));
            try
            {
                using (var scope = new OperationScope(document, uiApplication, "batch_plan_discard", plan.PlanId, warnings))
                {
                    using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: descartar plan y quitar todas las marcas"))
                    {
                        result.RemovedMarks = PlanMarks.Remove(document, plan, warnings);
                        foreach (BatchPlan other in others)
                        {
                            result.RemovedMarks += PlanMarks.Remove(document, other, warnings);
                            result.OtherPlansUnmarked++;
                        }
                        result.OrphanMarkers = PlanMarks.RemoveAll(document, warnings);
                        scope.CommitOrThrow(clean);
                    }
                    scope.Commit();
                }
            }
            catch
            {
                foreach (PlanMarkState state in saved) state.Restore();
                throw;
            }
            PlanRegistry.Remove(plan.PlanId);
            result.RemainingMarkers = PlanMarks.MarkerIds(document).Count;
            JsonLineLogger.Write(new
            {
                @event = "batch_plan_discard",
                plan_id = plan.PlanId,
                from = "window",
                removed_marks = result.RemovedMarks,
                other_plans_unmarked = result.OtherPlansUnmarked,
                orphan_markers = result.OrphanMarkers,
                remaining_markers = result.RemainingMarkers,
            });
            return result;
        }

        /// <summary>Los demás planes marcados del mismo documento (los que <c>PLAN_MARKS_REPLACED</c> desmarca).</summary>
        private static List<BatchPlan> OtherMarkedPlans(Document document, string? exceptPlanId) =>
            PlanRegistry.All().Where(p => p.IsMarked && (exceptPlanId == null || !string.Equals(p.PlanId, exceptPlanId, StringComparison.OrdinalIgnoreCase))
                                          && string.Equals(p.Document, document.Title, StringComparison.OrdinalIgnoreCase)).ToList();

        /// <summary>
        /// Quita todas las marcas de MotorConexiones del documento (también las de planes olvidados) y vacía el registro.
        /// Devuelve cuántos marcadores había antes (cierre de la Fase 8: antes solo contaba los huérfanos y el paso 8b-7 dijo
        /// <c>removed_markers: 0</c> tras quitar los 36 de un plan en memoria).
        /// </summary>
        public static int DiscardAll(Document document, UIApplication? uiApplication, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            int markers = PlanMarks.MarkerIds(document).Count;
            int orphans;
            using (var scope = new OperationScope(document, uiApplication, "batch_plan_discard", "all", warnings))
            {
                using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: quitar todas las marcas"))
                {
                    foreach (BatchPlan plan in PlanRegistry.All()) PlanMarks.Remove(document, plan, warnings);
                    orphans = PlanMarks.RemoveAll(document, warnings);
                    scope.CommitOrThrow(clean);
                }
                scope.Commit();
            }
            foreach (BatchPlan plan in PlanRegistry.All()) PlanRegistry.Remove(plan.PlanId);
            JsonLineLogger.Write(new { @event = "batch_plan_discard_all", markers, orphans });
            return markers;
        }

        /// <summary>La misma validación que <c>conn_validate</c>, para cada nudo del plan.</summary>
        internal static PlanValidation Validate(Document document, string specJson, ConnectionSpec spec)
        {
            ModelValidation validation = ValidationService.Validate(document, specJson, spec);
            return new PlanValidation(validation.IsValid, validation.Result.ValidationToken, validation.Result.Errors, validation.Result.Warnings);
        }

        /// <summary>Las conexiones del add-in que ya hay en el modelo (P8), con su cordón, sus barras y su lote (Fase 9).</summary>
        internal static List<ExistingConnection> ExistingConnections(Document document)
        {
            var connections = new List<ExistingConnection>();
            foreach (ConnectionRecord record in ConnectionStorageManager.ListConnections(document))
            {
                try
                {
                    ConnectionSpec? spec = ConnectionSpec.FromJson(record.SpecJson);
                    if (spec == null) continue;
                    var ids = new List<long>();
                    if (spec.Members != null) ids.AddRange(spec.Members.Select(m => m.ElementId));
                    if (spec.Node?.ElementIds != null) ids.AddRange(spec.Node.ElementIds);
                    connections.Add(new ExistingConnection(record.ConnectionId, spec.Source?.BatchId, spec.Chord?.ElementId ?? 0, ids));
                }
                catch
                {
                    // Un registro ilegible no impide planificar.
                }
            }
            return connections;
        }

        private static List<CatalogTemplate> LoadTemplates(CatalogStore store, List<string> templateIds, List<ApiError> warnings)
        {
            var templates = new List<CatalogTemplate>();
            if (templateIds.Count == 0)
            {
                foreach (CatalogEntry entry in store.List(warnings))
                {
                    try
                    {
                        CatalogTemplate? template = store.Get(entry.TemplateId);
                        if (template != null) templates.Add(template);
                    }
                    catch (CatalogException error)
                    {
                        warnings.Add(error.Error);
                    }
                }
                return templates;
            }
            foreach (string id in templateIds)
            {
                CatalogTemplate? template;
                try
                {
                    template = store.Get(id) ?? store.FindByName(id);
                }
                catch (CatalogException error)
                {
                    warnings.Add(error.Error);
                    continue;
                }
                if (template == null)
                {
                    throw new CatalogException(ErrorCodes.TemplateNotFound, "No existe la plantilla '" + id + "' en " + store.Folder + ".", "template_ids",
                        "Usa conn_catalog_list para ver las plantillas disponibles.");
                }
                templates.Add(template);
            }
            return templates;
        }

        /// <summary>
        /// Las barras del plan tal como las ve la detección (la selección y las que mencionan las correcciones), leídas del
        /// modelo, para dibujar el mapa de la cercha en la ventana (ronda 8c). Solo lee; las que no existan se saltan.
        /// </summary>
        public static List<DetectorBar> BarsOf(Document document, BatchPlan plan)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var facts = new RevitModelFacts(document);
            var bars = new List<DetectorBar>();
            foreach (long id in plan.SelectionIds.Concat(plan.Nodes.SelectMany(n => n.ElementIds)).Concat(plan.Overrides.ReferencedElementIds()).Distinct())
            {
                try
                {
                    if (!facts.ElementExists(id) || !facts.IsStructuralMember(id)) continue;
                    MemberModelFacts? member = facts.GetMemberFacts(id);
                    if (member == null || member.CurveStartMm.DistanceTo(member.CurveEndMm) < 1e-6) continue;
                    bars.Add(DetectorBar.FromFacts(member));
                }
                catch
                {
                    // Una barra ilegible no deja sin mapa a las demás.
                }
            }
            return bars;
        }

        /// <summary>El alzado de la cercha del plan (ronda 8c, V1).</summary>
        public static TrussMap MapOf(Document document, BatchPlan plan) => TrussMap.Build(plan, BarsOf(document, plan));

        /// <summary>
        /// Todo lo que la ventana no modal necesita del modelo, leído de una vez en contexto válido (cierre de la ronda 8c):
        /// el plan, el mapa y el tipo de cada barra. Si el mapa no se puede calcular, la ventana sale sin mapa y el motivo va
        /// al log; nunca impide abrirla.
        /// </summary>
        public static PlanSnapshot SnapshotOf(Document document, BatchPlan plan)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            List<DetectorBar> bars;
            TrussMap? map = null;
            try
            {
                bars = BarsOf(document, plan);
                map = TrussMap.Build(plan, bars);
            }
            catch (Exception ex)
            {
                bars = new List<DetectorBar>();
                JsonLineLogger.Write(new { @event = "ribbon_batch_map_failed", plan_id = plan.PlanId, error = ex.ToString() });
            }
            var types = new Dictionary<long, string>();
            foreach (DetectorBar bar in bars)
            {
                if (!string.IsNullOrEmpty(bar.TypeName)) types[bar.ElementId] = bar.TypeName!;
            }
            return new PlanSnapshot(plan, map, types);
        }

        /// <summary>Los datos del plan para la respuesta del puente y para la ventana.</summary>
        public static object PlanToData(BatchPlan plan, bool includeSpecs)
        {
            return new
            {
                plan_id = plan.PlanId,
                document = plan.Document,
                created_utc = plan.CreatedUtc,
                updated_utc = plan.UpdatedUtc,
                selection_count = plan.SelectionIds.Count,
                templates = plan.Templates,
                summary = plan.Summary(),
                description = plan.Describe(),
                ready_count = plan.ReadyCount,
                creatable_count = plan.CreatableCount,
                created_count = plan.CreatedCount,
                failed_count = plan.FailedCount,
                created_in_batch_count = plan.CreatedInBatchCount,
                has_batch_connections = plan.HasBatchConnections,
                last_report = plan.LastReport == null ? null : BatchCreator.ReportToData(plan.LastReport, null, false),
                summary_text = PlanAdvice.SummaryText(plan),
                visible_count = PlanAdvice.VisibleCount(plan),
                hidden_text = PlanAdvice.HiddenText(plan),
                is_marked = plan.IsMarked,
                marked_view_id = plan.MarkedViewId,
                marks = new { element_count = plan.MarkedElementIds.Count, marker_element_ids = plan.MarkerElementIds },
                overrides = System.Text.Json.JsonDocument.Parse(plan.Overrides.ToJson()).RootElement.Clone(),
                unused_element_ids = plan.UnusedElementIds,
                nodes = plan.Nodes.Select(n => NodeToData(n, includeSpecs, plan)).ToList(),
            };
        }

        /// <summary>
        /// Los datos de un nudo. Ronda 8c: añade <c>status_text</c> (estado en español con icono), <c>advice</c> (qué hacer) y
        /// <c>visible_by_default</c>; <c>color_name</c> / <c>color_rgb</c> pasan a ser los del estado (nulos en los nudos ocultos,
        /// que no se marcan). <paramref name="plan"/> solo hace falta para el consejo con el catálogo vacío.
        /// </summary>
        public static object NodeToData(PlanNode node, bool includeSpec, BatchPlan? plan = null)
        {
            bool visible = PlanAdvice.VisibleByDefault(node);
            return new
            {
                name = node.Name,
                status = node.Status,
                status_detail = node.StatusDetail,
                status_text = PlanAdvice.StatusText(node, plan),
                advice = PlanAdvice.Advice(node, plan),
                actions = PlanAdvice.Actions(node, plan).Select(a => new { key = a.Key, label = a.Label }).ToList(),
                visible_by_default = visible,
                work_point_mm = node.WorkPointMm,
                chord_element_id = node.ChordElementId,
                chord_continuous = node.ChordContinuous,
                chord_type_name = node.ChordTypeName,
                through_element_ids = node.ThroughElementIds,
                member_element_ids = node.MemberElementIds,
                element_ids = node.ElementIds,
                members = node.Members.Select(m => new { element_id = m.ElementId, angle_deg = m.AngleDeg, side = m.Side, type_name = m.TypeName, reaches_node = m.ReachesNode, end_gap_mm = m.EndGapMm }).ToList(),
                signature = node.Signature,
                is_manual = node.IsManual,
                template_id = node.TemplateId,
                template_name = node.TemplateName,
                orientation = node.Orientation,
                is_mirrored = node.IsMirrored,
                max_deviation_deg = node.MaxDeviationDeg,
                match = node.Match == null ? null : (object)System.Text.Json.JsonDocument.Parse(node.Match.ToJsonString()).RootElement.Clone(),
                attempts = node.Attempts,
                spec = includeSpec && node.Spec != null ? (object)System.Text.Json.JsonDocument.Parse(node.Spec.ToJsonString()).RootElement.Clone() : null,
                has_spec_override = node.HasSpecOverride,
                is_valid = node.IsValid,
                validation_token = node.ValidationToken,
                errors = node.Errors,
                warnings = node.Warnings,
                errors_count = node.Errors.Count,
                warnings_count = node.Warnings.Count,
                existing_connection_id = node.ExistingConnectionId,
                existing_batch_id = node.ExistingBatchId,
                replaces_existing = node.ReplacesExisting,
                created_connection_id = node.CreatedConnectionId,
                color_name = visible ? PlanAdvice.ColorName(node) : null,
                color_rgb = visible ? PlanAdvice.ColorRgb(node) : null,
                is_marked = node.IsMarked,
                marker_element_id = node.MarkerElementId,
            };
        }
    }
}
