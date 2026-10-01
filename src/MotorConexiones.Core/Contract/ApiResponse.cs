using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Sobre común de todas las respuestas del add-in (sección 10 del encargo):
    /// <c>{ ok, data, errors, warnings, meta }</c>. Siempre se serializa completo, con <c>data: null</c>
    /// cuando no hay datos, para que la IA reciba una forma estable.
    /// </summary>
    public sealed class ApiResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        [JsonPropertyName("data")]
        public object? Data { get; set; }

        [JsonPropertyName("errors")]
        public List<ApiError> Errors { get; set; } = new List<ApiError>();

        [JsonPropertyName("warnings")]
        public List<ApiError> Warnings { get; set; } = new List<ApiError>();

        [JsonPropertyName("meta")]
        public ApiMeta Meta { get; set; } = new ApiMeta();

        public static ApiResponse Success(string operation, object? data, IEnumerable<ApiError>? warnings = null)
        {
            var response = new ApiResponse { Ok = true, Data = data, Meta = { Operation = operation } };
            if (warnings != null) response.Warnings.AddRange(warnings);
            return response;
        }

        public static ApiResponse Failure(string operation, ApiError error, IEnumerable<ApiError>? warnings = null)
        {
            var response = new ApiResponse { Ok = false, Data = null, Meta = { Operation = operation } };
            response.Errors.Add(error);
            if (warnings != null) response.Warnings.AddRange(warnings);
            return response;
        }

        public static ApiResponse Failure(string operation, IEnumerable<ApiError> errors, IEnumerable<ApiError>? warnings = null)
        {
            var response = new ApiResponse { Ok = false, Data = null, Meta = { Operation = operation } };
            response.Errors.AddRange(errors);
            if (warnings != null) response.Warnings.AddRange(warnings);
            return response;
        }

        public string ToJson() => JsonSerializer.Serialize(this, JsonOptions.Default);
    }
}
