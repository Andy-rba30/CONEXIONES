using System.Linq;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Types;

namespace MotorConexiones.Revit.Operations
{
    /// <summary><c>conn_list_types</c>: tipos de conexión disponibles y cuándo usar cada uno.</summary>
    public sealed class TypesOperation : IOperation
    {
        public string Name => "types";
        public bool RequiresDocument => false;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            var types = ConnectionTypeRegistry.All.Select(t => new
            {
                type_name = t.Name,
                description = t.Description
            }).ToList();

            return ApiResponse.Success(Name, new { connection_types = types }, context.Warnings);
        }
    }
}
