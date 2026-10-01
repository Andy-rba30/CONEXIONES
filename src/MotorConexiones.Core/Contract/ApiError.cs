using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>Error o advertencia accionable: código estable, ruta del campo, mensaje en español y sugerencia.</summary>
    public sealed class ApiError
    {
        public ApiError(string code, string message, string? path = null, string? hint = null)
        {
            Code = code;
            Message = message;
            Path = path;
            Hint = hint;
        }

        [JsonPropertyName("code")]
        public string Code { get; }

        [JsonPropertyName("path")]
        public string? Path { get; }

        [JsonPropertyName("message")]
        public string Message { get; }

        [JsonPropertyName("hint")]
        public string? Hint { get; }
    }
}
