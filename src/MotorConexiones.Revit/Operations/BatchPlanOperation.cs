using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_batch_plan</c> (Fase 8): barras seleccionadas (o <c>element_ids</c>) + plantillas → plan con los nudos
    /// detectados, su plantilla casada, su especificación instanciada, su validación y su token, más las marcas en la
    /// vista activa. No crea ninguna conexión. Con <c>plan_id</c> replanifica el mismo plan acumulando <c>overrides</c>.
    /// </summary>
    public sealed class BatchPlanOperation : IOperation
    {
        public string Name => "batch_plan";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true; // solo las marcas (overrides de vista y marcadores), nunca acero

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            var input = new BatchPlanInput();
            if (context.TryGet("plan_id", out var planEl) && planEl.ValueKind == JsonValueKind.String) input.PlanId = (planEl.GetString() ?? "").Trim();
            if (context.TryGet("element_ids", out var idsEl) && idsEl.ValueKind == JsonValueKind.Array)
            {
                input.ElementIds = NodeInspector.ResolveIds(context.Request, null);
            }
            else if (string.IsNullOrEmpty(input.PlanId))
            {
                input.ElementIds = NodeInspector.ResolveIds(context.Request, context.UIDocument);
            }
            if (context.TryGet("template_ids", out var templatesEl) && templatesEl.ValueKind == JsonValueKind.Array)
            {
                input.TemplateIds = templatesEl.EnumerateArray().Where(t => t.ValueKind == JsonValueKind.String)
                    .Select(t => (t.GetString() ?? "").Trim()).Where(t => t.Length > 0).ToList();
            }
            else if (context.TryGet("template_id", out var templateEl) && templateEl.ValueKind == JsonValueKind.String)
            {
                string one = (templateEl.GetString() ?? "").Trim();
                if (one.Length > 0) input.TemplateIds.Add(one);
            }
            if (context.TryGet("mark", out var markEl) && (markEl.ValueKind == JsonValueKind.False || markEl.ValueKind == JsonValueKind.True)) input.Mark = markEl.ValueKind == JsonValueKind.True;
            if (context.TryGet("reset_overrides", out var resetEl) && resetEl.ValueKind == JsonValueKind.True) input.ResetOverrides = true;
            bool includeSpecs = !(context.TryGet("include_specs", out var specsEl) && specsEl.ValueKind == JsonValueKind.False);

            try
            {
                if (context.TryGet("overrides", out var overridesEl)) input.Overrides = BatchOverrides.FromJson(overridesEl);
                if (context.TryGet("replace_existing", out var replaceEl) && replaceEl.ValueKind == JsonValueKind.True)
                {
                    input.Overrides ??= new BatchOverrides();
                    input.Overrides.ReplaceExisting = true;
                }
                var warnings = new List<ApiError>(context.Warnings);
                BatchPlan plan = BatchPlanner.Plan(doc, context.UIApplication, input, warnings);
                warnings.AddRange(plan.Warnings);
                return ApiResponse.Success(Name, BatchPlanner.PlanToData(plan, includeSpecs), warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
        }
    }

    /// <summary><c>conn_batch_plan_get</c>: el plan en memoria (<c>plan_id</c> o el último), sin tocar el modelo.</summary>
    public sealed class BatchPlanGetOperation : IOperation
    {
        public string Name => "batch_plan_get";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            string? planId = context.TryGet("plan_id", out var planEl) && planEl.ValueKind == JsonValueKind.String ? planEl.GetString() : null;
            bool includeSpecs = !(context.TryGet("include_specs", out var specsEl) && specsEl.ValueKind == JsonValueKind.False);
            BatchPlan? plan = PlanRegistry.Get(planId);
            if (plan == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.PlanNotFound,
                    string.IsNullOrWhiteSpace(planId) ? "No hay ningún plan en memoria." : "No hay ningún plan con plan_id '" + planId + "' en memoria.",
                    "plan_id", "Planifica con conn_batch_plan (la selección de la cercha y, si quieres, template_ids)."), context.Warnings);
            }
            if (context.TryGet("node", out var nodeEl) && nodeEl.ValueKind == JsonValueKind.String)
            {
                PlanNode? node = plan.Find(nodeEl.GetString() ?? "");
                if (node == null)
                {
                    return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "El plan no tiene ningún nudo llamado '" + nodeEl.GetString() + "'.", "node",
                        "Usa los nombres de nodes[].name: " + string.Join(", ", plan.Nodes.Select(n => n.Name)) + "."), context.Warnings);
                }
                return ApiResponse.Success(Name, new { plan_id = plan.PlanId, node = BatchPlanner.NodeToData(node, true, plan) }, context.Warnings);
            }
            return ApiResponse.Success(Name, BatchPlanner.PlanToData(plan, includeSpecs), context.Warnings);
        }
    }

    /// <summary><c>conn_batch_plan_discard</c>: quita las marcas del modelo y olvida el plan (o todos con <c>all: true</c>).</summary>
    public sealed class BatchPlanDiscardOperation : IOperation
    {
        public string Name => "batch_plan_discard";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            var warnings = new List<ApiError>(context.Warnings);
            bool all = context.TryGet("all", out var allEl) && allEl.ValueKind == JsonValueKind.True;
            if (all)
            {
                int plans = PlanRegistry.Count;
                int markers = BatchPlanner.DiscardAll(doc, context.UIApplication, warnings);
                return ApiResponse.Success(Name, new { discarded_plans = plans, removed_markers = markers, remaining_markers = PlanMarks.MarkerIds(doc).Count }, warnings);
            }
            string? planId = context.TryGet("plan_id", out var planEl) && planEl.ValueKind == JsonValueKind.String ? planEl.GetString() : null;
            BatchPlan? plan = PlanRegistry.Get(planId);
            if (plan == null)
            {
                int orphan = PlanMarks.MarkerIds(doc).Count;
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.PlanNotFound,
                    (string.IsNullOrWhiteSpace(planId) ? "No hay ningún plan en memoria." : "No hay ningún plan con plan_id '" + planId + "' en memoria.")
                    + (orphan > 0 ? " Quedan " + orphan + " marcador(es) de planes olvidados." : ""),
                    "plan_id", orphan > 0 ? "Pasa all: true para quitar todas las marcas de MotorConexiones del modelo." : "No hay nada que descartar."), warnings);
            }
            int removed = BatchPlanner.Discard(doc, context.UIApplication, plan, warnings);
            return ApiResponse.Success(Name, new
            {
                discarded_plan_id = plan.PlanId,
                removed_marks = removed,
                remaining_plans = PlanRegistry.Count,
                remaining_markers = PlanMarks.MarkerIds(doc).Count,
            }, warnings);
        }
    }
}
