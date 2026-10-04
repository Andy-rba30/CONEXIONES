using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Types;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Operations;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Punto de entrada estático para el MCP. <c>revit_mcp/conexiones.py</c> lo localiza por reflexión y lo llama
    /// desde un manejador de pyRevit Routes, que ya corre en contexto de la API de Revit. Nunca lanza: toda
    /// excepción se convierte en el sobre común con <c>ok:false</c>.
    /// </summary>
    public static class Bridge
    {
        private static readonly Dictionary<string, IOperation> Operations = new Dictionary<string, IOperation>(StringComparer.Ordinal);

        static Bridge()
        {
            if (ConnectionTypeRegistry.Find("gusset_node") == null)
            {
                ConnectionTypeRegistry.Register(GussetNodeType.Instance);
            }

            Register(new PingOperation());
            Register(new GuideOperation());
            Register(new TypesOperation());
            Register(new SchemaOperation());
            Register(new NodeInfoOperation());
            Register(new FindProfileOperation());
            Register(new ValidateOperation());
            Register(new PreviewOperation());
            Register(new CreateOperation());
            Register(new ListOperation());
            Register(new GetOperation());
            Register(new UpdateOperation());
            Register(new DeleteOperation());
            // Fase 7: catálogo de plantillas.
            Register(new CatalogListOperation());
            Register(new CatalogGetOperation());
            Register(new CatalogSaveOperation());
            Register(new CatalogDeleteOperation());
            Register(new CatalogApplyOperation());
            // Fase 8: detección de nudos y plan de lote (no crea nada).
            Register(new BatchPlanOperation());
            Register(new BatchPlanGetOperation());
            Register(new BatchPlanDiscardOperation());
        }

        public static void Register(IOperation operation) => Operations[operation.Name] = operation;

        public static IReadOnlyList<string> OperationNames => Operations.Keys.OrderBy(k => k, StringComparer.Ordinal).ToList();

        /// <summary>
        /// Atiende una operación. <paramref name="requestJson"/> puede ser nulo o vacío (equivale a <c>{}</c>).
        /// Devuelve siempre el sobre común serializado.
        /// </summary>
        public static string Handle(string operation, string requestJson, Document doc, UIDocument uidoc)
        {
            var stopwatch = Stopwatch.StartNew();
            string name = operation ?? "";
            ApiResponse response;
            OperationContext? context = null;
            try
            {
                response = HandleCore(name, requestJson, doc, uidoc, out context);
            }
            catch (Exception error)
            {
                response = ApiResponse.Failure(name, new ApiError(
                    ErrorCodes.InternalError,
                    "Error no controlado en el add-in: " + error.GetType().Name + ": " + error.Message,
                    hint: "Mira el registro en " + JsonLineLogger.CurrentFile),
                    context?.Warnings);
                JsonLineLogger.Write(new { @event = "exception", operation = name, error = error.ToString() });
            }

            stopwatch.Stop();
            response.Meta.Operation = name;
            response.Meta.DurationMs = stopwatch.ElapsedMilliseconds;

            string json;
            try
            {
                json = response.ToJson();
            }
            catch (Exception error)
            {
                json = ApiResponse.Failure(name, new ApiError(ErrorCodes.InternalError,
                    "No se pudo serializar la respuesta: " + error.Message)).ToJson();
            }

            JsonLineLogger.Write(new
            {
                @event = "handle",
                operation = name,
                request_summary = Summarize(requestJson),
                ok = response.Ok,
                error_codes = response.Errors.Select(e => e.Code).ToArray(),
                warning_codes = response.Warnings.Select(w => w.Code).ToArray(),
                duration_ms = response.Meta.DurationMs,
            });
            return json;
        }

        private static ApiResponse HandleCore(string name, string requestJson, Document? doc, UIDocument? uidoc, out OperationContext? context)
        {
            context = null;
            if (!Operations.TryGetValue(name, out var operation))
            {
                return ApiResponse.Failure(name, new ApiError(
                    ErrorCodes.UnknownOperation,
                    "La operación '" + name + "' no existe en el add-in.",
                    hint: "Operaciones disponibles: " + string.Join(", ", OperationNames) + "."));
            }

            JsonElement request;
            try
            {
                string text = string.IsNullOrWhiteSpace(requestJson) ? "{}" : requestJson;
                using var document = JsonDocument.Parse(text, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
                request = document.RootElement.Clone();
            }
            catch (JsonException error)
            {
                return ApiResponse.Failure(name, new ApiError(
                    ErrorCodes.InvalidRequest,
                    "La petición no es JSON válido: " + error.Message,
                    hint: "Envía un objeto JSON, por ejemplo {\"element_ids\": [111, 222]}."));
            }

            if (request.ValueKind != JsonValueKind.Object)
            {
                return ApiResponse.Failure(name, new ApiError(ErrorCodes.InvalidRequest,
                    "La petición debe ser un objeto JSON.", hint: "Envía {} si la operación no necesita datos."));
            }

            context = new OperationContext(name, request, doc, uidoc);

            if (operation.RequiresDocument && doc == null)
            {
                return ApiResponse.Failure(name, new ApiError(ErrorCodes.NoDocument,
                    "No hay ningún documento abierto en Revit.", hint: "Abre el modelo y vuelve a intentarlo."));
            }

            if (operation.ModifiesModel && doc != null)
            {
                if (doc.IsReadOnly)
                {
                    return ApiResponse.Failure(name, new ApiError(ErrorCodes.RevitBusy,
                        "El documento es de solo lectura.", hint: "Abre el modelo con permiso de escritura."));
                }
                if (doc.IsModifiable)
                {
                    return ApiResponse.Failure(name, new ApiError(ErrorCodes.RevitBusy,
                        "Revit ya tiene una transacción abierta: no se puede modificar el modelo ahora.",
                        hint: "Termina la orden en curso en Revit (o cierra el diálogo abierto) y repite la llamada."));
                }
            }

            return operation.Execute(context);
        }

        private static string Summarize(string? requestJson)
        {
            if (string.IsNullOrEmpty(requestJson)) return "";
            string text = requestJson!.Replace('\n', ' ').Replace('\r', ' ');
            return text.Length <= 200 ? text : text.Substring(0, 200) + "...";
        }
    }
}
