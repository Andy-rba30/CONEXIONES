using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>Opciones únicas de serialización JSON del add-in.</summary>
    public static class JsonOptions
    {
        /// <summary>
        /// Sin sangría, con los acentos tal cual (no <c>é</c>), sin omitir nulos (el sobre lleva <c>data: null</c>)
        /// y tolerante con comentarios y comas finales al leer especificaciones escritas a mano.
        /// </summary>
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };
    }
}
