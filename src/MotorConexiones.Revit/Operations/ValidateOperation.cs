using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_validate</c>: valida la especificación contra el esquema, las 10 reglas del contrato y el modelo activo de Revit.
    /// Si no hay errores, genera y devuelve el <c>validation_token</c> SHA-256 requerido para la creación.
    /// </summary>
    public sealed class ValidateOperation : IOperation
    {
        public string Name => "validate";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            string rawSpecJson;

            // Extraer el JSON de la especificación
            if (context.TryGet("spec", out var specEl) && specEl.ValueKind == JsonValueKind.Object)
            {
                rawSpecJson = specEl.GetRawText();
            }
            else
            {
                rawSpecJson = context.Request.GetRawText();
            }

            ConnectionSpec? spec = null;
            try
            {
                spec = ConnectionSpec.FromJson(rawSpecJson);
            }
            catch (Exception ex)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.SchemaInvalid,
                    "No se pudo deserializar la especificación: " + ex.Message,
                    "",
                    "Verifica que el JSON cumpla con la estructura requerida."));
            }

            // Marco del nudo con la misma regla que preview y create (cordón + primer miembro de members).
            NodeFrame? frame = null;
            if (spec != null)
            {
                try
                {
                    frame = NodeInspector.ResolveNode(doc, spec).Frame;
                }
                catch (NodeInspectionException)
                {
                    // Los IDs inexistentes los reporta el validador con ELEMENT_NOT_FOUND; aquí solo falta el marco.
                }
            }

            // Cargar límites configurables
            LimitsConfig limits = LimitsConfigLoader.Load();

            // Modelo desacoplado sobre Revit
            var modelFacts = new RevitModelFacts(doc, frame);

            // Validar exhaustivamente
            ValidationResult result = SpecValidator.Validate(rawSpecJson, spec, modelFacts, limits);

            object? calculatedValues = null;
            if (frame != null)
            {
                calculatedValues = new
                {
                    origin_mm = new[] { Math.Round(frame.Origin.X, 1), Math.Round(frame.Origin.Y, 1), Math.Round(frame.Origin.Z, 1) },
                    axis_distance_mm = Math.Round(frame.AxisDistanceMm, 2),
                    frame_x = new[] { Math.Round(frame.X.X, 4), Math.Round(frame.X.Y, 4), Math.Round(frame.X.Z, 4) },
                    frame_y = new[] { Math.Round(frame.Y.X, 4), Math.Round(frame.Y.Y, 4), Math.Round(frame.Y.Z, 4) },
                    frame_z = new[] { Math.Round(frame.Z.X, 4), Math.Round(frame.Z.Y, 4), Math.Round(frame.Z.Z, 4) }
                };
            }

            // Ronda 6b: agarre y longitud de cada grupo de pernos, para que la IA vea lo que se creará.
            var boltStacks = new System.Collections.Generic.List<object>();
            if (spec?.Members != null)
            {
                double gussetThickness = spec.Gusset?.ThicknessMm ?? 9.525;
                foreach (var member in spec.Members)
                {
                    var plate = member.Attachment?.Plate;
                    if (plate == null) continue;
                    var stack = MotorConexiones.Core.Geometry3D.BoltStack.Compute(gussetThickness, plate, member.Attachment?.Bolts, limits);
                    boltStacks.Add(new
                    {
                        member_element_id = member.ElementId,
                        gusset_face = stack.FaceLabel,
                        grip_mm = Math.Round(stack.GripMm, 3),
                        bolt_length_mm = Math.Round(stack.BoltLengthMm, 3),
                        length_source = stack.LengthFromSpec ? "spec" : "computed_from_grip",
                    });
                }
            }

            var data = new
            {
                is_valid = result.IsValid,
                validation_token = result.ValidationToken,
                errors_count = result.Errors.Count,
                warnings_count = result.Warnings.Count,
                calculated_values = calculatedValues,
                bolt_stacks = boltStacks
            };

            // Propagar advertencias acumuladas
            var allWarnings = new System.Collections.Generic.List<ApiError>(context.Warnings);
            allWarnings.AddRange(result.Warnings);

            if (!result.IsValid)
            {
                return ApiResponse.Failure(Name, result.Errors, allWarnings);
            }

            return ApiResponse.Success(Name, data, allWarnings);
        }
    }
}
