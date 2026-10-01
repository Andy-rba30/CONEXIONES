using System;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_delete</c>: borra los elementos geométricos creados por el add-in, restaura los retiros
    /// de extremo de los miembros a su estado original y elimina el registro de Extensible Storage.
    /// Nunca borra elementos ajenos a la conexión.
    /// </summary>
    public sealed class DeleteOperation : IOperation
    {
        public string Name => "delete";
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
                    "connection_id es requerido para borrar una conexión.",
                    "connection_id",
                    "Usa conn_list para ver las conexiones existentes."));
            }

            var record = ConnectionStorageManager.GetConnection(doc, connectionId);
            if (record == null)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ElementNotFound,
                    "No se encontró la conexión con ID '" + connectionId + "'.",
                    "connection_id",
                    "Verifica los IDs disponibles con conn_list."));
            }

            ConnectionRecord? deletedRecord = null;
            using (var scope = new OperationScope(doc, context.UIApplication, Name, connectionId, context.Warnings))
            {
                using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Borrar " + connectionId))
                {
                    try
                    {
                        bool success = ConnectionCreationService.DeleteConnection(doc, connectionId, context.Warnings, out deletedRecord);
                        if (!success)
                        {
                            return ApiResponse.Failure(Name, new ApiError(
                                ErrorCodes.InternalError,
                                "No se pudo borrar la conexión '" + connectionId + "'."));
                        }
                        scope.CommitOrThrow(tx);
                    }
                    catch (Exception ex)
                    {
                        return ApiResponse.Failure(Name, new ApiError(
                            ErrorCodes.InternalError,
                            "Fallo al borrar la conexión: " + ex.Message,
                            "",
                            "Se realizó rollback completo."),
                            context.Warnings);
                    }
                }
                scope.Commit();
            }

            var data = new
            {
                deleted_connection_id = connectionId,
                deleted_elements_count = deletedRecord?.CreatedElementIds?.Count ?? 0,
                restored_members_count = deletedRecord?.ModifiedMembers?.Count ?? 0
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
