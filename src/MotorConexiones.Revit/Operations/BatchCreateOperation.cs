using System.Collections.Generic;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Batch;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_batch_create</c> (Fase 9): crea las conexiones de un plan nudo a nudo con los <c>validation_token</c> que
    /// devolvió <c>conn_batch_plan</c>. Cada nudo es una operación atómica propia; el lote entero es una entrada de deshacer
    /// (<see cref="BatchCreator"/>). Devuelve el informe por nudo. Con <c>stop_on_error</c> y un fallo, <c>ok: false</c> con
    /// <c>BATCH_STOPPED</c> y el informe en <c>data</c>.
    /// </summary>
    public sealed class BatchCreateOperation : IOperation
    {
        public string Name => "batch_create";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            BatchCreateRequest request;
            try
            {
                request = BatchCreateRequest.FromJson(context.Request);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
            BatchPlan? plan = PlanRegistry.Get(request.PlanId);
            if (plan == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.PlanNotFound,
                    "No hay ningún plan con plan_id '" + request.PlanId + "' en memoria (se descartó o Revit se reinició): no se crea nada.",
                    "plan_id", "Vuelve a planificar con conn_batch_plan y usa su plan_id y sus tokens."), context.Warnings);
            }
            var warnings = new List<ApiError>(context.Warnings);
            BatchReport report;
            try
            {
                report = BatchCreator.Create(doc, context.UIApplication, plan, request, warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, warnings);
            }
            object data = BatchCreator.ReportToData(report, plan, request.IncludeSpecs);
            if (report.Stopped)
            {
                var response = ApiResponse.Failure(Name, new ApiError(ErrorCodes.BatchStopped,
                    "El lote se detuvo en " + report.StoppedAt + " y se revirtió entero (stop_on_error): " + report.SummaryText,
                    "nodes[" + report.StoppedAt + "]", "Mira data.nodes para ver por qué falló; corrige o excluye ese nudo y vuelve a crear."), warnings);
                response.Data = data;
                return response;
            }
            return ApiResponse.Success(Name, data, warnings);
        }
    }

    /// <summary>
    /// <c>conn_batch_delete</c> (Fase 9): borra todas las conexiones del lote (<c>batch_id</c> = el <c>plan_id</c> que las
    /// creó), una a una con las garantías de <c>conn_delete</c>, en una sola entrada de deshacer. Devuelve el informe.
    /// </summary>
    public sealed class BatchDeleteOperation : IOperation
    {
        public string Name => "batch_delete";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            string batchId = "";
            if (context.TryGet("batch_id", out JsonElement batchEl) && batchEl.ValueKind == JsonValueKind.String) batchId = (batchEl.GetString() ?? "").Trim();
            if (batchId.Length == 0 && context.TryGet("plan_id", out JsonElement planEl) && planEl.ValueKind == JsonValueKind.String) batchId = (planEl.GetString() ?? "").Trim();
            if (batchId.Length == 0)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "Falta batch_id: el lote que se quiere borrar (es el plan_id del plan que lo creó).", "batch_id",
                    "conn_list enseña el batch_id de cada conexión y data.batches cuenta cuántas hay por lote."), context.Warnings);
            }
            var warnings = new List<ApiError>(context.Warnings);
            BatchReport report;
            try
            {
                report = BatchCreator.DeleteBatch(doc, context.UIApplication, batchId, warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, warnings);
            }
            BatchPlan? plan = PlanRegistry.Get(batchId);
            return ApiResponse.Success(Name, BatchCreator.ReportToData(report, plan, false), warnings);
        }
    }
}
