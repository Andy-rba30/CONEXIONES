using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Registro explícito de incertidumbre o duda en la interpretación del plano.
    /// Si user_confirmed_value es null, impide la generación del validation_token.
    /// </summary>
    public sealed class UncertainField
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("user_confirmed_value")]
        public object? UserConfirmedValue { get; set; }
    }
}
