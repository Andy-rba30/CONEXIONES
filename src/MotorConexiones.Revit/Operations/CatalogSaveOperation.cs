using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_catalog_save</c>: guarda una plantilla desde una conexión creada (<c>connection_id</c>) o desde una
    /// especificación (<c>spec</c>) que valide contra el modelo abierto. Necesita el modelo para medir los ángulos reales.
    /// </summary>
    public sealed class CatalogSaveOperation : IOperation
    {
        public string Name => "catalog_save";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            CatalogConfig config = CatalogConfigLoader.Load();

            var metadata = new TemplateMetadata
            {
                Name = ReadString(context, "name") ?? "",
                Description = ReadString(context, "description"),
                TemplateId = ReadString(context, "template_id"),
                ProfilePolicy = ReadString(context, "profile_policy"),
            };
            if (context.TryGet("tags", out var tagsEl) && tagsEl.ValueKind == JsonValueKind.Array)
            {
                metadata.Tags = tagsEl.EnumerateArray().Where(t => t.ValueKind == JsonValueKind.String).Select(t => t.GetString() ?? "").ToList();
            }
            if (context.TryGet("angle_tolerance_deg", out var tolEl) && tolEl.ValueKind == JsonValueKind.Number) metadata.AngleToleranceDeg = tolEl.GetDouble();
            if (context.TryGet("allow_mirror", out var mirrorEl) && (mirrorEl.ValueKind == JsonValueKind.True || mirrorEl.ValueKind == JsonValueKind.False)) metadata.AllowMirror = mirrorEl.GetBoolean();
            bool overwrite = ReadBool(context, "overwrite");
            bool copyToShared = ReadBool(context, "copy_to_shared");
            if (!string.IsNullOrWhiteSpace(metadata.ProfilePolicy) && !ProfilePolicy.IsValid(metadata.ProfilePolicy))
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest,
                    "profile_policy debe ser warn, require o ignore.", "profile_policy", "Omite el campo para usar la política de config\\catalog.json."), context.Warnings);
            }

            string? connectionId = ReadString(context, "connection_id");
            string? rawSpec = null;
            if (context.TryGet("spec", out var specEl) && specEl.ValueKind == JsonValueKind.Object) rawSpec = specEl.GetRawText();
            if (string.IsNullOrWhiteSpace(connectionId) && rawSpec == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest,
                    "Pasa connection_id (una conexión creada por el add-in) o spec (una especificación que valide).", "connection_id",
                    "Usa conn_list para ver los connection_id, o pasa la especificación completa en spec."), context.Warnings);
            }

            try
            {
                CatalogTemplate? template;
                ModelValidation? validation;
                bool ok;
                if (!string.IsNullOrWhiteSpace(connectionId))
                {
                    ok = CatalogService.TryBuildFromConnection(doc, connectionId!.Trim(), metadata, config, out template, out validation);
                }
                else
                {
                    ok = CatalogService.TryBuildFromSpec(doc, rawSpec!, null, metadata, config, out template, out ModelValidation specValidation);
                    validation = specValidation;
                }
                if (!ok || template == null)
                {
                    var errors = new List<ApiError>
                    {
                        new ApiError(ErrorCodes.TemplateSpecInvalid,
                            "La especificación no valida contra el modelo abierto: no se guarda como plantilla.", rawSpec != null ? "spec" : "connection_id",
                            "Corrige los errores que siguen (los mismos de conn_validate) y vuelve a intentarlo."),
                    };
                    if (validation != null) errors.AddRange(validation.Result.Errors);
                    var failedWarnings = new List<ApiError>(context.Warnings);
                    if (validation != null) failedWarnings.AddRange(validation.Result.Warnings);
                    return ApiResponse.Failure(Name, errors, failedWarnings);
                }

                CatalogStore store = CatalogConfigLoader.OpenStore(config);
                string? sharedFolder = CatalogConfigLoader.ResolveSharedFolder(config);
                string file = CatalogService.Save(store, template, overwrite, sharedFolder, copyToShared, out string? sharedFile);
                var warnings = new List<ApiError>(context.Warnings);
                if (validation != null) warnings.AddRange(validation.Result.Warnings);
                if (copyToShared && sharedFile == null)
                {
                    warnings.Add(new ApiError(ErrorCodes.CatalogFolderUnavailable, "No hay shared_catalog_folder en config\\catalog.json: no se hizo la copia compartida.",
                        "copy_to_shared", "Configura shared_catalog_folder (por ejemplo la carpeta catalog\\ del repositorio)."));
                }
                JsonLineLogger.Write(new { @event = "catalog_save", template_id = template.TemplateId, name = template.Name, file, shared_file = sharedFile, from_connection = connectionId });

                var data = new
                {
                    template_id = template.TemplateId,
                    name = template.Name,
                    file,
                    shared_file = sharedFile,
                    members_count = template.MemberPattern.Count,
                    chord_profile = template.ChordPattern.Profile,
                    member_pattern = CatalogService.PatternToData(template),
                    matching = new { angle_tolerance_deg = template.Matching.AngleToleranceDeg, allow_mirror = template.Matching.AllowMirror },
                    origin = new { connection_id = template.Origin.ConnectionId, document = template.Origin.Document, drawing = template.Origin.Drawing, element_ids = template.Origin.ElementIds },
                };
                return ApiResponse.Success(Name, data, warnings);
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
        }

        private static string? ReadString(OperationContext context, string name)
        {
            return context.TryGet(name, out var el) && el.ValueKind == JsonValueKind.String ? el.GetString() : null;
        }

        private static bool ReadBool(OperationContext context, string name)
        {
            return context.TryGet(name, out var el) && el.ValueKind == JsonValueKind.True;
        }
    }
}
