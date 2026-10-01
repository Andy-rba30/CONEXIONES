using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// Borra los DirectShape creados por <c>probe_plate_b</c> (los reconoce por <c>ApplicationId</c> = MotorConexiones
    /// y <c>ApplicationDataId</c> plate:/bolt:). Nunca borra nada que el add-in no haya creado.
    /// </summary>
    public sealed class ProbeDeleteBOperation : IOperation
    {
        public string Name => "probe_delete_b";
        public bool RequiresDocument => true;
        public bool ModifiesModel => true;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            List<ElementId> targets = new FilteredElementCollector(doc)
                .OfClass(typeof(DirectShape))
                .Cast<DirectShape>()
                .Where(shape => shape.ApplicationId == DirectShapeBackend.ApplicationId
                                && (shape.ApplicationDataId.StartsWith("plate:", StringComparison.Ordinal)
                                    || shape.ApplicationDataId.StartsWith("bolt:", StringComparison.Ordinal)))
                .Select(shape => shape.Id)
                .ToList();

            if (targets.Count == 0)
            {
                return ApiResponse.Success(Name, new { deleted = 0, element_ids = new List<long>() }, context.Warnings);
            }

            ICollection<ElementId> deleted;
            try
            {
                using var scope = new OperationScope(doc, context.UIApplication, Name, "sondeo", context.Warnings);
                using (Transaction transaction = scope.StartTransaction(doc, "Borrar placa y pernos de prueba"))
                {
                    deleted = doc.Delete(targets);
                    scope.CommitOrThrow(transaction);
                }
                scope.Commit();
            }
            catch (Exception error)
            {
                return ApiResponse.Failure(Name, new ApiError(ErrorCodes.FabricationFailed,
                    "No se pudieron borrar los elementos de prueba: " + error.Message), context.Warnings);
            }

            return ApiResponse.Success(Name, new
            {
                deleted = deleted.Count,
                element_ids = targets.Select(id => id.Value).ToList(),
            }, context.Warnings);
        }
    }
}
