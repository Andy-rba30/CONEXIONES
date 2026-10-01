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
    /// <c>conn_update</c>: reemplaza una conexión existente de forma atómica conservando su <c>connection_id</c>.
    /// Exige un <c>validation_token</c> válido y se ejecuta dentro de un <see cref="OperationScope"/>.
    /// </summary>
    public sealed class UpdateOperation : IOperation
    {
        public string Name => "update";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;

            string connectionId = "";
            if (context.TryGet("connection_id", out var idEl) && idEl.ValueKind == JsonValueKind.String)
            {
                connectionId = idEl.GetString() ?? "";
            }

            if (string.IsNullOrWhiteSpace(connectionId))
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.InvalidRequest,
                    "connection_id es requerido para actualizar.",
                    "connection_id",
                    "Pasa el connection_id de la conexión que deseas actualizar."));
            }

            var existingRecord = ConnectionStorageManager.GetConnection(doc, connectionId);
            if (existingRecord == null)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ElementNotFound,
                    "No se encontró la conexión '" + connectionId + "' en el modelo.",
                    "connection_id",
                    "Usa conn_list para verificar los IDs existentes."));
            }

            string token = "";
            if (context.TryGet("validation_token", out var tokenEl) && tokenEl.ValueKind == JsonValueKind.String)
            {
                token = tokenEl.GetString() ?? "";
            }

            if (string.IsNullOrWhiteSpace(token))
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ValidationTokenInvalid,
                    "validation_token es obligatorio para actualizar una conexión.",
                    "validation_token",
                    "Llama a conn_validate con la nueva especificación para obtener el token."));
            }

            string rawSpecJson;
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
                    "spec",
                    "Corrige la sintaxis JSON."));
            }

            if (spec == null)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.SchemaInvalid, "Especificación nula."));
            }

            // Verificar token
            var modelFacts = new RevitModelFacts(doc);
            string expectedToken = ValidationTokenGenerator.GenerateToken(spec, modelFacts);

            if (!string.Equals(token.Trim(), expectedToken.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ValidationTokenInvalid,
                    "El validation_token no es válido para la nueva especificación o el modelo ha cambiado.",
                    "validation_token",
                    "Vuelve a llamar a conn_validate para obtener un token fresco."));
            }

            ConnectionRecord updatedRecord;
            using (var scope = new OperationScope(doc, context.UIApplication, Name, connectionId, context.Warnings))
            {
                using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Actualizar " + connectionId))
                {
                    try
                    {
                        updatedRecord = ConnectionCreationService.UpdateConnection(doc, context.UIDocument, connectionId, spec, rawSpecJson, context.Warnings);
                        scope.CommitOrThrow(tx);
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse.Failure(Name, new ApiError(
                            ErrorCodes.InternalError,
                            "Fallo al actualizar la conexión: " + ex.Message,
                            "",
                            "Se realizó rollback completo."),
                            context.Warnings);
                    }
                }
                scope.Commit();
            }

            var data = new
            {
                connection_id = updatedRecord.ConnectionId,
                spec_version = updatedRecord.SpecVersion,
                connection_type = updatedRecord.ConnectionType,
                created_element_ids = updatedRecord.CreatedElementIds,
                created_elements_count = updatedRecord.CreatedElementIds.Count,
                updated_utc = DateTime.UtcNow.ToString("o")
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
