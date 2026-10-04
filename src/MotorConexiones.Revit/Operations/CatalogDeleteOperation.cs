using System.Text.Json;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_catalog_delete</c>: borra el archivo de una plantilla (la IA pide confirmación antes).</summary>
    public sealed class CatalogDeleteOperation : IOperation
    {
        public string Name => "catalog_delete";
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
                if (template == null || !store.Delete(templateId.Trim(), out string path))
                {
                    return ApiResponse.Failure(Name, new ApiError(ErrorCodes.TemplateNotFound,
                        "No existe la plantilla '" + templateId + "' en " + store.Folder + ".", "template_id",
                        "Usa conn_catalog_list para ver las plantillas disponibles."), context.Warnings);
                }
                JsonLineLogger.Write(new { @event = "catalog_delete", template_id = template.TemplateId, name = template.Name, file = path });
                return ApiResponse.Success(Name, new { deleted_template_id = template.TemplateId, name = template.Name, file = path }, context.Warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
        }
    }
}
