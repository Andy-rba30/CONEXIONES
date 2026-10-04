using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_list</c>: lista todas las conexiones creadas por el add-in almacenadas en el modelo activo.
    /// </summary>
    public sealed class ListOperation : IOperation
    {
        public string Name => "list";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            var records = ConnectionStorageManager.ListConnections(doc);

            var items = records.Select(r => new
            {
                connection_id = r.ConnectionId,
                spec_version = r.SpecVersion,
                connection_type = r.ConnectionType,
                created_elements_count = r.CreatedElementIds.Count,
                backend = r.BackendName,
                created_utc = r.CreatedUtc,
                template_id = TemplateIdOf(r.SpecJson),
            }).ToList();

            var data = new
            {
                connections_count = items.Count,
                connections = items
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }

        /// <summary>Plantilla de la que salió la conexión (<c>source.template_id</c>, Fase 7) o nulo.</summary>
        private static string? TemplateIdOf(string specJson)
        {
            try
            {
                return MotorConexiones.Core.Contract.ConnectionSpec.FromJson(specJson)?.Source?.TemplateId;
            }
            catch
            {
                return null;
            }
        }
    }
}
