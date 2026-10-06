using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Catalog
{
    /// <summary>
    /// Configuración del catálogo de plantillas, leída de <c>config/catalog.json</c> (editable sin recompilar):
    /// carpetas, tolerancias de casado y política de perfil por defecto, y las tolerancias de la detección de nudos
    /// (<c>node_*</c>, Fase 8).
    /// </summary>
    public sealed class CatalogConfig
    {
        /// <summary>Carpeta por defecto del catálogo del usuario (la misma de <c>log\</c>), con la variable sin expandir.</summary>
        public const string DefaultCatalogFolder = "%LOCALAPPDATA%\\MotorConexiones\\catalogo";

        [JsonPropertyName("schema_version")]
        public int SchemaVersion { get; set; } = 1;

        /// <summary>Carpeta donde viven las plantillas (un JSON por plantilla). Admite variables de entorno (%LOCALAPPDATA%).</summary>
        [JsonPropertyName("catalog_folder")]
        public string CatalogFolder { get; set; } = DefaultCatalogFolder;

        /// <summary>
        /// Carpeta compartida opcional (por ejemplo <c>catalog\</c> del repositorio o una carpeta de red): "Guardar en
        /// catálogo" ofrece copiar ahí la plantilla y <c>deploy.ps1</c> copia a la carpeta del usuario las que falten.
        /// </summary>
        [JsonPropertyName("shared_catalog_folder")]
        public string? SharedCatalogFolder { get; set; }

        /// <summary>Diferencia máxima de ángulo (grados) para casar una barra del nudo con una ranura de la plantilla (P7: 10°).</summary>
        [JsonPropertyName("angle_tolerance_deg")]
        public double AngleToleranceDeg { get; set; } = 10.0;

        /// <summary>A partir de esta desviación (grados) respecto a la plantilla se avisa con TEMPLATE_ANGLE_DEVIATION.</summary>
        [JsonPropertyName("angle_deviation_warning_deg")]
        public double AngleDeviationWarningDeg { get; set; } = 5.0;

        /// <summary>Si las plantillas se prueban también reflejadas (P6: sí).</summary>
        [JsonPropertyName("allow_mirror")]
        public bool AllowMirror { get; set; } = true;

        /// <summary>Política de perfil por defecto al guardar una plantilla: <c>warn</c> (P3), <c>require</c> o <c>ignore</c>.</summary>
        [JsonPropertyName("default_profile_policy")]
        public string DefaultProfilePolicy { get; set; } = ProfilePolicy.Warn;

        /// <summary>Fase 8: distancia para agrupar extremos de barras en un nudo (P7: 10 mm).</summary>
        [JsonPropertyName("node_cluster_mm")]
        public double NodeClusterMm { get; set; } = 10.0;

        /// <summary>Fase 8: distancia máxima del eje de una barra al punto de trabajo para "atraviesa el nudo" (5 mm).</summary>
        [JsonPropertyName("node_axis_max_distance_mm")]
        public double NodeAxisMaxDistanceMm { get; set; } = 5.0;

        /// <summary>
        /// Ronda 8b: alcance de cara fijo en mm para agrupar un extremo cortado en la cara del cordón con el corte de su eje.
        /// 0 (por defecto) = según el canto del perfil: medio canto de cada barra más <see cref="NodeClusterMm"/>.
        /// </summary>
        [JsonPropertyName("node_face_reach_mm")]
        public double NodeFaceReachMm { get; set; } = 0.0;

        /// <summary>
        /// Fase 9: <c>true</c> (por defecto) = el lote se crea dentro de un grupo exterior que se asimila al final (una sola
        /// entrada de deshacer, con un grupo por nudo anidado); <c>false</c> = plan B de la propuesta 4.1: un grupo por nudo
        /// y una entrada de deshacer por nudo (si el sondeo 20 dijera que los grupos anidados no conviven con Advance Steel).
        /// </summary>
        [JsonPropertyName("batch_single_undo")]
        public bool BatchSingleUndo { get; set; } = true;

        /// <summary>
        /// Fase 10 (V3): <c>true</c> (por defecto) = al marcar un plan se pone en la vista una etiqueta pinchable con el
        /// número de cada nudo visible; <c>false</c> = solo colores y marcadores, sin recompilar (por si las etiquetas del
        /// lienzo dieran problemas en el PC).
        /// </summary>
        [JsonPropertyName("plan_labels")]
        public bool PlanLabels { get; set; } = true;

        /// <summary>
        /// Fase 10 (V2): <c>true</c> (por defecto) = al marcar un plan se dibuja en cada nudo listo (y fallido o que no valida)
        /// una cartela fantasma: un sólido transparente con el contorno de su cartela, del color de su estado, que Crear
        /// sustituye por acero y Descartar quita; <c>false</c> = solo colores, marcadores y etiquetas.
        /// </summary>
        [JsonPropertyName("plan_ghosts")]
        public bool PlanGhosts { get; set; } = true;

        public static CatalogConfig Default => new CatalogConfig();

        public static CatalogConfig LoadFromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return Default;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            };
            CatalogConfig? parsed = JsonSerializer.Deserialize<CatalogConfig>(json, options);
            if (parsed == null) return Default;
            if (string.IsNullOrWhiteSpace(parsed.CatalogFolder)) parsed.CatalogFolder = DefaultCatalogFolder;
            if (!ProfilePolicy.IsValid(parsed.DefaultProfilePolicy)) parsed.DefaultProfilePolicy = ProfilePolicy.Warn;
            if (parsed.AngleToleranceDeg <= 0) parsed.AngleToleranceDeg = 10.0;
            if (parsed.AngleDeviationWarningDeg < 0) parsed.AngleDeviationWarningDeg = 5.0;
            return parsed;
        }

        public static CatalogConfig LoadFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return Default;
            try
            {
                return LoadFromJson(File.ReadAllText(filePath));
            }
            catch
            {
                return Default;
            }
        }

        /// <summary>Expande las variables de entorno de una carpeta (<c>%LOCALAPPDATA%</c>) y quita espacios sobrantes.</summary>
        public static string ExpandFolder(string? folder)
        {
            string text = (folder ?? string.Empty).Trim();
            if (text.Length == 0) return string.Empty;
            try
            {
                return Environment.ExpandEnvironmentVariables(text);
            }
            catch
            {
                return text;
            }
        }
    }

    /// <summary>Qué hacer cuando el perfil del nudo no es el de la plantilla (decisión P3: <c>warn</c> por defecto).</summary>
    public static class ProfilePolicy
    {
        /// <summary>Aviso <c>TEMPLATE_PROFILE_DIFFERS</c> y se escribe el perfil del modelo.</summary>
        public const string Warn = "warn";

        /// <summary>Se conserva el perfil de la plantilla: <c>conn_validate</c> dará <c>PROFILE_MISMATCH</c> si no coincide.</summary>
        public const string Require = "require";

        /// <summary>Se escribe el perfil del modelo sin avisar.</summary>
        public const string Ignore = "ignore";

        public static bool IsValid(string? policy) =>
            string.Equals(policy, Warn, StringComparison.OrdinalIgnoreCase)
            || string.Equals(policy, Require, StringComparison.OrdinalIgnoreCase)
            || string.Equals(policy, Ignore, StringComparison.OrdinalIgnoreCase);

        public static string Normalize(string? policy, string fallback = Warn)
        {
            string clean = (policy ?? string.Empty).Trim().ToLowerInvariant();
            return IsValid(clean) ? clean : fallback;
        }
    }
}
