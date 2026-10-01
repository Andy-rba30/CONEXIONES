using System;
using System.Linq;
using System.Text.Json;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Types;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_get_schema</c>: JSON Schema de un tipo de conexión más un ejemplo lleno.</summary>
    public sealed class SchemaOperation : IOperation
    {
        public string Name => "schema";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            string typeName = "gusset_node";
            if (context.TryGet("type", out var typeEl) && typeEl.ValueKind == JsonValueKind.String)
            {
                typeName = typeEl.GetString() ?? "gusset_node";
            }

            IConnectionType? connType = ConnectionTypeRegistry.Find(typeName);
            if (connType == null)
            {
                var registered = string.Join(", ", ConnectionTypeRegistry.All.Select(t => t.Name));
                return ApiResponse.Failure(Name, new ApiError(
                    ErrorCodes.UnknownOperation,
                    "El tipo de conexión '" + typeName + "' no está registrado.",
                    "type",
                    "Tipos disponibles: " + registered));
            }

            string schemaJson = connType.GetSchemaJson();
            object schemaObj;
            try
            {
                using var doc = JsonDocument.Parse(schemaJson);
                schemaObj = doc.RootElement.Clone();
            }
            catch
            {
                schemaObj = schemaJson;
            }

            string exampleJson = connType.GetExampleJson();
            object exampleObj;
            try
            {
                using var doc = JsonDocument.Parse(exampleJson);
                exampleObj = doc.RootElement.Clone();
            }
            catch
            {
                exampleObj = exampleJson;
            }

            var data = new
            {
                connection_type = connType.Name,
                description = connType.Description,
                json_schema = schemaObj,
                example = exampleObj
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }
    }
}
