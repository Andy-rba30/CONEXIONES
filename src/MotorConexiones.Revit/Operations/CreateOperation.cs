using System;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_create</c>: crea la conexión física en el modelo. Exige un <c>validation_token</c> válido
    /// emitido previamente por <c>conn_validate</c>. Se ejecuta de manera atómica dentro de un <see cref="OperationScope"/>.
    /// </summary>
    public sealed class CreateOperation : IOperation
    {
        public string Name => "create";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;

            // 1. Obtener validation_token
            string token = "";
            if (context.TryGet("validation_token", out var tokenEl) && tokenEl.ValueKind == JsonValueKind.String)
            {
                token = tokenEl.GetString() ?? "";
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ValidationTokenInvalid,
                    "validation_token es obligatorio para crear una conexión.",
                    "validation_token",
                    "Llama primero a conn_validate para validar la especificación y obtener el token."));
            }

            // 2. Extraer especificación
            string rawSpecJson;
            if (context.TryGet("spec", out var specEl) && specEl.ValueKind == JsonValueKind.Object)
            {
                rawSpecJson = specEl.GetRawText();
            }
            else
            {
                // Si la especificación viene en la raíz excluyendo el token
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
                    "spec",
                    "Corrige la sintaxis JSON."));
            }

            if (spec == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SchemaInvalid, "Especificación nula."));
            }

            // 3. Verificar token contra el modelo
            var modelFacts = new RevitModelFacts(doc);
            string expectedToken = ValidationTokenGenerator.GenerateToken(spec, modelFacts);

            if (!string.Equals(token.Trim(), expectedToken.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ValidationTokenInvalid,
                    "El validation_token no es válido o el modelo ha cambiado desde que se realizó la validación.",
                    "validation_token",
                    "Vuelve a llamar a conn_validate para revalidar el estado actual del modelo y obtener un nuevo token."));
            }

            // 4. Ejecución atómica en OperationScope
            string opId = Guid.NewGuid().ToString("D");
            ConnectionRecord record;
            var snapshot = ConnectionCreationService.Snapshot(doc);

            using (var scope = new OperationScope(doc, context.UIApplication, Name, opId, context.Warnings))
            {
                using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Crear " + (spec.Source?.Drawing ?? "Nudo")))
                {
                    try
                    {
                        record = ConnectionCreationService.CreateConnection(doc, context.UIDocument, spec, rawSpecJson, opId, context.Warnings);
                        scope.CommitOrThrow(tx);
                        using (Transaction adopt = scope.StartTransaction(doc, "MotorConexiones: registrar elementos"))
                        {
                            ConnectionCreationService.AdoptNewElements(doc, record, snapshot, context.Warnings);
                            scope.CommitOrThrow(adopt);
                        }
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse.Failure(Name, new ApiError(
                            ErrorCodes.InternalError,
                            "Fallo al modelar la conexión: " + ex.Message,
                            "",
                            "Revisa el registro y los miembros seleccionados. Se realizó rollback completo."),
                            context.Warnings);
                    }
                }

                scope.Commit();
            }

            var data = new
            {
                connection_id = record.ConnectionId,
                spec_version = record.SpecVersion,
                connection_type = record.ConnectionType,
                created_element_ids = record.CreatedElementIds,
                created_elements_count = record.CreatedElementIds.Count,
                created_utc = record.CreatedUtc,
                backend = record.BackendName
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
