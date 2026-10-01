using System.Collections.Generic;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>Contexto que recibe cada operación del puente.</summary>
    public sealed class OperationContext
    {
        public OperationContext(string operation, JsonElement request, Document? document, UIDocument? uiDocument)
        {
            Operation = operation;
            Request = request;
            Document = document;
            UIDocument = uiDocument;
        }

        public string Operation { get; }
        public JsonElement Request { get; }
        public Document? Document { get; }
        public UIDocument? UIDocument { get; }
        public UIApplication? UIApplication => UIDocument?.Application;

        /// <summary>Advertencias acumuladas (fallos de Revit suprimidos, diálogos cancelados...).</summary>
        public List<ApiError> Warnings { get; } = new List<ApiError>();

        /// <summary>Lee una propiedad opcional de la petición.</summary>
        public bool TryGet(string name, out JsonElement value)
        {
            if (Request.ValueKind == JsonValueKind.Object && Request.TryGetProperty(name, out value)) return true;
            value = default;
            return false;
        }

        public double GetDouble(string name, double defaultValue)
        {
            return TryGet(name, out var value) && value.ValueKind == JsonValueKind.Number ? value.GetDouble() : defaultValue;
        }
    }

    /// <summary>Una operación del puente (<c>ping</c>, <c>probe_plate_b</c>, y en fases siguientes <c>validate</c>, <c>create</c>...).</summary>
    public interface IOperation
    {
        string Name { get; }

        /// <summary>Si es verdadero y no hay documento abierto, el puente responde <c>NO_DOCUMENT</c> sin llamar.</summary>
        bool RequiresDocument { get; }

        /// <summary>Si es verdadero y el documento está ocupado o es de solo lectura, el puente responde <c>REVIT_BUSY</c>.</summary>
        bool ModifiesModel { get; }

        ApiResponse Execute(OperationContext context);
    }
}
