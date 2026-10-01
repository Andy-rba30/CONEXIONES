using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_catalog_apply</c>: plantilla + barras de un nudo (o la selección) → especificación instanciada, casado y
    /// el resultado de <c>conn_validate</c> con su token. La IA pasa <c>spec</c> y <c>validation_token</c> tal cual a <c>conn_create</c>.
    /// </summary>
    public sealed class CatalogApplyOperation : IOperation
    {
        public string Name => "catalog_apply";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            CatalogConfig config = CatalogConfigLoader.Load();

            string templateId = "";
            if (context.TryGet("template_id", out var idEl) && idEl.ValueKind == JsonValueKind.String) templateId = (idEl.GetString() ?? "").Trim();
            string templateName = "";
            if (context.TryGet("template_name", out var nameEl) && nameEl.ValueKind == JsonValueKind.String) templateName = (nameEl.GetString() ?? "").Trim();
            if (templateId.Length == 0 && templateName.Length == 0)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest, "template_id es obligatorio (o template_name).", "template_id",
                    "Consulta conn_catalog_list para ver las plantillas."), context.Warnings);
            }

            long? chordId = null;
            if (context.TryGet("chord_element_id", out var chordEl) && chordEl.ValueKind == JsonValueKind.Number && chordEl.TryGetInt64(out long chordValue)) chordId = chordValue;
            TemplateOrientation? forced = null;
            if (context.TryGet("orientation", out var orEl) && orEl.ValueKind == JsonValueKind.String)
            {
                string text = (orEl.GetString() ?? "").Trim();
                if (text.Length > 0 && !string.Equals(text, "auto", System.StringComparison.OrdinalIgnoreCase))
                {
                    if (!TemplateOrientations.TryParse(text, out TemplateOrientation parsed))
                    {
                        return ApiResponse.Failure(Name, new ApiError(ErrorCodes.InvalidRequest,
                            "orientation debe ser auto, same, mirror_x, mirror_y o both.", "orientation", "Omite el campo para que se pruebe en las cuatro."), context.Warnings);
                    }
                    forced = parsed;
                }
            }
            List<long> ids = NodeInspector.ResolveIds(context.Request, context.UIDocument);

            try
            {
                CatalogStore store = CatalogConfigLoader.OpenStore(config);
                CatalogTemplate? template = templateId.Length > 0 ? store.Get(templateId) : store.FindByName(templateName);
                if (template == null)
                {
                    return ApiResponse.Failure(Name, new ApiError(ErrorCodes.TemplateNotFound,
                        "No existe la plantilla '" + (templateId.Length > 0 ? templateId : templateName) + "' en " + store.Folder + ".", "template_id",
                        "Usa conn_catalog_list para ver las plantillas disponibles."), context.Warnings);
                }

                CatalogApplyResult result;
                try
                {
                    result = CatalogService.Apply(doc, template, ids, chordId, forced, config);
                }
                catch (CatalogException ex) when (ex.Error.Code == ErrorCodes.TemplateNoMatch)
                {
                    // Sin encaje: se devuelven los intentos para que la IA (o la persona) vea qué ranura no encontró barra.
                    var facts = new RevitModelFacts(doc);
                    object? attempts = null;
                    try
                    {
                        long chord = chordId ?? TemplateNode.ChooseChord(facts, ids);
                        TemplateNode node = TemplateNode.FromModelFacts(facts, chord, ids.Where(i => i != chord));
                        attempts = TemplateMatcher.MatchAll(template, node, null, null, forced).Select(CatalogService.MatchToData).ToList();
                    }
                    catch (CatalogException)
                    {
                        // Si el nudo tampoco se puede leer, basta el error principal.
                    }
                    ApiResponse failure = ApiResponse.Failure(Name, ex.Error, context.Warnings);
                    failure.Data = new { template_id = template.TemplateId, name = template.Name, attempts };
                    JsonLineLogger.Write(new { @event = "catalog_apply_no_match", template_id = template.TemplateId, element_ids = ids });
                    return failure;
                }

                ModelValidation validation = result.Validation;
                var warnings = new List<ApiError>(context.Warnings);
                warnings.AddRange(result.Instantiation.Warnings);
                warnings.AddRange(validation.Result.Warnings);
                using var specDoc = JsonDocument.Parse(result.Instantiation.SpecJson);
                var data = new
                {
                    template_id = template.TemplateId,
                    name = template.Name,
                    node = new { chord_element_id = result.Node.ChordElementId, element_ids = result.Node.AllElementIds, chord_direction_reversed = result.Node.Frame.ChordReversed },
                    match = CatalogService.MatchToData(result.Match),
                    spec = specDoc.RootElement.Clone(),
                    is_valid = validation.IsValid,
                    validation_token = validation.Result.ValidationToken,
                    errors_count = validation.Result.Errors.Count,
                    warnings_count = warnings.Count,
                    calculated_values = ValidationService.CalculatedValues(validation.Frame),
                    bolt_stacks = ValidationService.BoltStacks(validation.Spec, validation.Limits),
                };
                JsonLineLogger.Write(new
                {
                    @event = "catalog_apply",
                    template_id = template.TemplateId,
                    element_ids = ids,
                    orientation = result.Match.OrientationName,
                    is_valid = validation.IsValid,
                    error_codes = validation.Result.Errors.Select(e => e.Code).ToArray(),
                });

                var response = new ApiResponse { Ok = validation.IsValid, Data = data, Meta = { Operation = Name } };
                response.Errors.AddRange(validation.Result.Errors);
                response.Warnings.AddRange(warnings);
                return response;
            }
            catch (CatalogException ex)
            {
                return ApiResponse.Failure(Name, ex.Error, context.Warnings);
            }
        }
    }
}
