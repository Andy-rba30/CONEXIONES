using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>
    /// Una plantilla del catálogo (sección 2.1 de la propuesta): la especificación de una conexión <b>sin IDs de
    /// elementos</b> más un patrón de barras que dice cómo casar cada ranura (<c>slot</c>) con una barra del nudo real.
    /// Se guarda como un archivo JSON (<c>&lt;template_id&gt;.json</c>) en la carpeta del catálogo.
    /// </summary>
    public sealed class CatalogTemplate
    {
        public const string CurrentCatalogVersion = "1.0";

        private static readonly JsonSerializerOptions FileOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        [JsonPropertyName("catalog_version")]
        public string CatalogVersion { get; set; } = CurrentCatalogVersion;

        /// <summary>GUID estable; es también el nombre del archivo.</summary>
        [JsonPropertyName("template_id")]
        public string TemplateId { get; set; } = Guid.NewGuid().ToString("D");

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("connection_type")]
        public string ConnectionType { get; set; } = "gusset_node";

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("created_utc")]
        public string CreatedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("origin")]
        public TemplateOrigin Origin { get; set; } = new TemplateOrigin();

        [JsonPropertyName("chord_pattern")]
        public ChordPattern ChordPattern { get; set; } = new ChordPattern();

        [JsonPropertyName("member_pattern")]
        public List<MemberPatternSlot> MemberPattern { get; set; } = new List<MemberPatternSlot>();

        [JsonPropertyName("matching")]
        public MatchingOptions Matching { get; set; } = new MatchingOptions();

        /// <summary>
        /// La especificación sin <c>node</c>, sin <c>chord.element_id</c>, sin <c>members[].element_id</c> (cada barra lleva
        /// <c>slot</c>) y sin dudas abiertas. El contorno de la cartela va en mm en el marco canónico del nudo de origen.
        /// </summary>
        [JsonPropertyName("spec_template")]
        public JsonObject? SpecTemplate { get; set; }

        public string ToJson() => JsonSerializer.Serialize(this, FileOptions);

        /// <summary>Lee una plantilla; devuelve nulo si el texto no es JSON o no es un objeto de plantilla.</summary>
        public static CatalogTemplate? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                CatalogTemplate? template = JsonSerializer.Deserialize<CatalogTemplate>(json, FileOptions);
                if (template == null || string.IsNullOrWhiteSpace(template.TemplateId) || template.SpecTemplate == null) return null;
                template.Tags ??= new List<string>();
                template.MemberPattern ??= new List<MemberPatternSlot>();
                template.Origin ??= new TemplateOrigin();
                template.ChordPattern ??= new ChordPattern();
                template.Matching ??= new MatchingOptions();
                return template;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>Resumen de una línea para listas y ventanas: "3 barras: diagonal 136,9° +Y · ...".</summary>
        public string DescribePattern()
        {
            var parts = new List<string>();
            foreach (MemberPatternSlot slot in MemberPattern)
            {
                parts.Add(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0} {1:0.0}° {2}",
                    slot.Role ?? "barra", slot.AngleDeg, slot.Side).Replace('.', ','));
            }
            return MemberPattern.Count + " barra(s): " + string.Join(" · ", parts);
        }
    }

    /// <summary>De dónde salió la plantilla (informativo).</summary>
    public sealed class TemplateOrigin
    {
        [JsonPropertyName("connection_id")]
        public string? ConnectionId { get; set; }

        [JsonPropertyName("document")]
        public string? Document { get; set; }

        [JsonPropertyName("drawing")]
        public string? Drawing { get; set; }

        /// <summary>IDs del nudo de origen (cordón y barras en el orden de las ranuras), solo para poder volver a él.</summary>
        [JsonPropertyName("element_ids")]
        public List<long> ElementIds { get; set; } = new List<long>();
    }

    /// <summary>Lo que la plantilla espera del cordón.</summary>
    public sealed class ChordPattern
    {
        [JsonPropertyName("profile")]
        public string? Profile { get; set; }

        [JsonPropertyName("continuous")]
        public bool Continuous { get; set; } = true;

        [JsonPropertyName("profile_policy")]
        public string ProfilePolicy { get; set; } = Catalog.ProfilePolicy.Warn;
    }

    /// <summary>Una ranura de la plantilla: a qué barra del nudo real corresponde (por ángulo) y qué perfil espera.</summary>
    public sealed class MemberPatternSlot
    {
        [JsonPropertyName("slot")]
        public int Slot { get; set; }

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        /// <summary>Ángulo con signo desde +X del marco canónico del nudo de origen, en [−180°, 180°).</summary>
        [JsonPropertyName("angle_deg")]
        public double AngleDeg { get; set; }

        /// <summary><c>+Y</c> o <c>-Y</c>: lado del cordón.</summary>
        [JsonPropertyName("side")]
        public string Side { get; set; } = "+Y";

        /// <summary>Perfil escrito en la especificación de origen (rótulo del plano).</summary>
        [JsonPropertyName("profile")]
        public string? Profile { get; set; }

        /// <summary>Nombre del tipo en el modelo de origen (por si el rótulo y el tipo no coinciden literalmente).</summary>
        [JsonPropertyName("model_type_name")]
        public string? ModelTypeName { get; set; }

        [JsonPropertyName("profile_policy")]
        public string ProfilePolicy { get; set; } = Catalog.ProfilePolicy.Warn;
    }

    /// <summary>Tolerancias de casado propias de la plantilla (por defecto las de <c>config/catalog.json</c>).</summary>
    public sealed class MatchingOptions
    {
        [JsonPropertyName("angle_tolerance_deg")]
        public double AngleToleranceDeg { get; set; } = 10.0;

        [JsonPropertyName("allow_mirror")]
        public bool AllowMirror { get; set; } = true;
    }

    /// <summary>Fallo del catálogo con el error accionable del sobre común.</summary>
    public sealed class CatalogException : Exception
    {
        public CatalogException(string code, string message, string? path = null, string? hint = null) : base(message)
        {
            Error = new Contract.ApiError(code, message, path, hint);
        }

        public CatalogException(Contract.ApiError error) : base(error.Message)
        {
            Error = error;
        }

        public Contract.ApiError Error { get; }
    }
}
