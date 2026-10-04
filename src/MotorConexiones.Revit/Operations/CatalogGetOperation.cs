using System.Text.Json;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_catalog_get</c>: una plantilla completa por su <c>template_id</c>.</summary>
    public sealed class CatalogGetOperation : IOperation
    {
        public string Name => "catalog_get";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            string templateId = "";
            if (context.TryGet("template_id", out var idEl) && idEl.ValueKind == JsonValueKind.String) templateId = idEl.GetString() ?? "";
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "template_id es obligatorio.", "template_id",
                    "Consulta conn_catalog_list para ver los IDs."), context.Warnings);
            }

            try
            {
                CatalogStore store = CatalogConfigLoader.OpenStore(CatalogConfigLoader.Load());
                CatalogTemplate? template = store.Get(templateId.Trim());
                if (template == null)
                {
                    return ApiResponse.Failure(Name, new ApiError(ErrorCodes.TemplateNotFound,
                        "No existe la plantilla '" + templateId + "' en " + store.Folder + ".", "template_id",
                        "Usa conn_catalog_list para ver las plantillas disponibles."), context.Warnings);
                }
                using var doc = JsonDocument.Parse(template.ToJson());
                var data = new
                {
                    template_id = template.TemplateId,
                    name = template.Name,
                    file = store.PathFor(template.TemplateId),
                    pattern = template.DescribePattern(),
                    template = doc.RootElement.Clone(),
                };
                return ApiResponse.Success(Name, data, context.Warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
        }
    }
}
