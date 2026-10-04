using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
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
                ConnectedMembers = ConnectedMembers(document),
                PlanId = previous?.PlanId,
                CreatedUtc = previous?.CreatedUtc,
                DocumentTitle = document.Title,
            };

            // Las marcas del plan anterior se quitan y se ponen las nuevas en la misma operación (una entrada de deshacer).
            BatchPlan plan;
            string opId = previous?.PlanId ?? Guid.NewGuid().ToString("D");
            using (var scope = new OperationScope(document, uiApplication, "batch_plan", opId, warnings))
            {
                if (previous != null && previous.IsMarked)
                {
                    using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: quitar marcas del plan"))
                    {
                        PlanMarks.Remove(document, previous, warnings);
                        scope.CommitOrThrow(clean);
                    }
                }
                plan = PlanBuilder.Build(request);
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
            using (var scope = new OperationScope(document, uiApplication, "batch_plan_discard", plan.PlanId, warnings))
            {
                using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: descartar plan"))
                {
                    removed = PlanMarks.Remove(document, plan, warnings);
                    scope.CommitOrThrow(clean);
                }
                scope.Commit();
            }
            PlanRegistry.Remove(plan.PlanId);
            JsonLineLogger.Write(new { @event = "batch_plan_discard", plan_id = plan.PlanId, removed_marks = removed });
            return removed;
        }

        /// <summary>Quita todas las marcas de MotorConexiones del documento (también las de planes olvidados) y vacía el registro.</summary>
        public static int DiscardAll(Document document, UIApplication? uiApplication, List<ApiError> warnings)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            int markers;
            using (var scope = new OperationScope(document, uiApplication, "batch_plan_discard", "all", warnings))
            {
                using (Transaction clean = scope.StartTransaction(document, "MotorConexiones: quitar todas las marcas"))
                {
                    foreach (BatchPlan plan in PlanRegistry.All()) PlanMarks.Remove(document, plan, warnings);
                    markers = PlanMarks.RemoveAll(document, warnings);
                    scope.CommitOrThrow(clean);
                }
                scope.Commit();
            }
            foreach (BatchPlan plan in PlanRegistry.All()) PlanRegistry.Remove(plan.PlanId);
            JsonLineLogger.Write(new { @event = "batch_plan_discard_all", markers });
            return markers;
        }

        /// <summary>La misma validación que <c>conn_validate</c>, para cada nudo del plan.</summary>
        internal static PlanValidation Validate(Document document, string specJson, ConnectionSpec spec)
        {
            ModelValidation validation = ValidationService.Validate(document, specJson, spec);
            return new PlanValidation(validation.IsValid, validation.Result.ValidationToken, validation.Result.Errors, validation.Result.Warnings);
        }

        /// <summary>Barras que ya están en una conexión del add-in (P8).</summary>
        internal static Dictionary<long, string> ConnectedMembers(Document document)
        {
            var map = new Dictionary<long, string>();
            foreach (ConnectionRecord record in ConnectionStorageManager.ListConnections(document))
            {
                try
                {
                    ConnectionSpec? spec = ConnectionSpec.FromJson(record.SpecJson);
                    if (spec == null) continue;
                    var ids = new List<long>();
                    if (spec.Chord != null) ids.Add(spec.Chord.ElementId);
                    if (spec.Members != null) ids.AddRange(spec.Members.Select(m => m.ElementId));
                    if (spec.Node?.ElementIds != null) ids.AddRange(spec.Node.ElementIds);
                    foreach (long id in ids.Where(i => i > 0).Distinct())
                    {
                        if (!map.ContainsKey(id)) map[id] = record.ConnectionId;
                    }
                }
                catch
                {
                    // Un registro ilegible no impide planificar.
                }
            }
            return map;
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
                is_marked = plan.IsMarked,
                marked_view_id = plan.MarkedViewId,
                marks = new { element_count = plan.MarkedElementIds.Count, marker_element_ids = plan.MarkerElementIds },
                overrides = System.Text.Json.JsonDocument.Parse(plan.Overrides.ToJson()).RootElement.Clone(),
                unused_element_ids = plan.UnusedElementIds,
                nodes = plan.Nodes.Select(n => NodeToData(n, includeSpecs)).ToList(),
            };
        }

        public static object NodeToData(PlanNode node, bool includeSpec)
        {
            return new
            {
                name = node.Name,
                status = node.Status,
                status_detail = node.StatusDetail,
                work_point_mm = node.WorkPointMm,
                chord_element_id = node.ChordElementId,
                chord_continuous = node.ChordContinuous,
                chord_type_name = node.ChordTypeName,
                through_element_ids = node.ThroughElementIds,
                member_element_ids = node.MemberElementIds,
                element_ids = node.ElementIds,
                members = node.Members.Select(m => new { element_id = m.ElementId, angle_deg = m.AngleDeg, side = m.Side, type_name = m.TypeName, reaches_node = m.ReachesNode }).ToList(),
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
                replaces_existing = node.ReplacesExisting,
                color_name = node.CanBeMarked ? node.ColorName : null,
                color_rgb = node.CanBeMarked ? node.ColorRgb : null,
                is_marked = node.IsMarked,
                marker_element_id = node.MarkerElementId,
            };
        }
    }
}
