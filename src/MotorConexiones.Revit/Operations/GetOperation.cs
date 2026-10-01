using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_get</c>: devuelve la especificación guardada y los metadatos de una conexión por su <c>connection_id</c>.
    /// </summary>
    public sealed class GetOperation : IOperation
    {
        public string Name => "get";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

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
                    "connection_id es requerido para obtener la conexión.",
                    "connection_id",
                    "Consulta conn_list para ver los IDs existentes."));
            }

            var record = ConnectionStorageManager.GetConnection(doc, connectionId);
            if (record == null)
            {
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.ElementNotFound,
                    "No se encontró ninguna conexión con ID '" + connectionId + "'.",
                    "connection_id",
                    "Usa conn_list para verificar las conexiones guardadas en el modelo."));
            }

            object specObject;
            try
            {
                using var docJson = JsonDocument.Parse(record.SpecJson);
                specObject = docJson.RootElement.Clone();
            }
            catch
            {
                specObject = record.SpecJson;
            }

            var data = new
            {
                connection_id = record.ConnectionId,
                spec_version = record.SpecVersion,
                connection_type = record.ConnectionType,
                created_utc = record.CreatedUtc,
                created_element_ids = record.CreatedElementIds,
                created_elements_count = record.CreatedElementIds.Count,
                spec = specObject
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
