using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace MotorConexiones.Core.Brief
{
    /// <summary>Lo que hace falta para escribir el encargo para una IA externa (ronda 9b de <c>docs/propuestas/encargo-ia-externa.md</c>).</summary>
    public sealed class DesignBriefInput
    {
        public string DocumentTitle { get; set; } = string.Empty;
        public string ConnectionType { get; set; } = "gusset_node";
        public string AddinVersion { get; set; } = AddinInfo.Version;
        public DateTime CreatedLocal { get; set; } = DateTime.Now;

        /// <summary>Barras seleccionadas (cordón y barras), solo informativo.</summary>
        public List<long> SelectedElementIds { get; set; } = new List<long>();

        /// <summary>El JSON de <c>data</c> de <c>conn_get_node_info</c>, tal cual (IDs, perfiles reales, ángulos, origen, ejes).</summary>
        public string NodeInfoJson { get; set; } = string.Empty;

        /// <summary>El <c>data.example</c> de <c>conn_get_schema</c> del tipo.</summary>
        public string SchemaExampleJson { get; set; } = string.Empty;

        /// <summary>El contenido de <c>docs/guide.md</c> desplegado (se copian las secciones 2, 3 y 4).</summary>
        public string GuideMarkdown { get; set; } = string.Empty;

        /// <summary>Un JSON que ya se creó bien en Revit: la plantilla más parecida del catálogo o el fixture embebido.</summary>
        public string ExampleJson { get; set; } = string.Empty;

        /// <summary>De dónde sale el ejemplo ("plantilla 'Nudo tipico Detalle D' del catálogo" / "el Detalle D de la Fase 3, embebido en el add-in").</summary>
        public string ExampleSource { get; set; } = string.Empty;
    }

    /// <summary>
    /// Escribe el encargo para una IA externa en un solo Markdown (decisión P1 de la propuesta): el prompt ya redactado, los
    /// datos del nudo, el esquema, las reglas de lectura de la guía y un ejemplo confirmado. La persona lo pega en el
    /// navegador con la imagen del detalle, recibe el JSON y lo aplica con <b>Ejecutar especificación JSON</b>. Puro Core:
    /// el botón de la cinta solo recoge los datos y lo guarda.
    /// </summary>
    public static class DesignBriefWriter
    {
        /// <summary>El prompt de la sección 3 de la propuesta, tal cual.</summary>
        public const string Prompt =
            "Eres el diseñador de una conexión de acero para un add-in de Revit. Tu única salida es un JSON que cumple el esquema " +
            "adjunto (sección \"Esquema\"); no añadas campos que no estén en él. Lee el detalle que te adjunto como imagen siguiendo las " +
            "reglas de la sección \"Cómo leer un detalle\". Usa SOLO los element_id, perfiles y ángulos de la sección \"Datos del nudo\": " +
            "los ángulos y las posiciones salen del modelo, no del dibujo; si el plano trae un ángulo, ponlo en expected_angle_deg. El " +
            "cordón es chord_element_id. Transcribe cada cadena de cotas completa en dimension_chains con su total. Lo que no se lea " +
            "bien va en uncertain_fields con path, reason y user_confirmed_value: null, y el campo apuntado queda null. Espesores y " +
            "diámetros en pulgadas van en mm en el campo *_mm y con el texto del plano tal cual en *_label. La sección \"Ejemplo " +
            "confirmado\" es un JSON que ya se creó bien en Revit: imítalo en forma, no en valores. Antes del JSON dame una tabla " +
            "corta: cordón, cartela (espesor y contorno), cada barra con su unión, pernos, soldaduras, retiros y tus dudas. Después, el " +
            "JSON completo en un solo bloque de código, listo para guardar como archivo .json. Si más adelante te pego errores del " +
            "validador, corrige solo lo que digan y devuélveme el JSON entero otra vez.";

        /// <summary>Nombre del recurso embebido con el Detalle D confirmado (<c>docs/fixtures/detalle-D-confirmado.json</c>).</summary>
        public const string EmbeddedExampleResource = "MotorConexiones.Core.Brief.detalle-D-confirmado.json";

        public const string EmbeddedExampleSource = "el Detalle D de la Fase 3 (docs/fixtures/detalle-D-confirmado.json), embebido en el add-in";

        /// <summary>Secciones de la guía que se copian: desde "## 2." hasta antes de "## 5.".</summary>
        public const string GuideFirstSection = "## 2.";
        public const string GuideStopSection = "## 5.";

        public static string Write(DesignBriefInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var text = new StringBuilder();
            string title = string.IsNullOrWhiteSpace(input.DocumentTitle) ? "(sin documento)" : input.DocumentTitle.Trim();
            text.Append("# Encargo para una IA externa: conexión ").Append(input.ConnectionType).Append(" en ").Append(title).AppendLine();
            text.AppendLine();
            text.Append("Generado por MotorConexiones ").Append(input.AddinVersion).Append(" el ")
                .Append(input.CreatedLocal.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)).Append('.');
            if (input.SelectedElementIds.Count > 0) text.Append(" Barras seleccionadas en Revit: ").Append(string.Join(", ", input.SelectedElementIds)).Append('.');
            text.AppendLine();
            text.AppendLine();
            text.AppendLine("**Cómo usarlo**: pega este texto entero en la IA del navegador **junto con la imagen del detalle** (un recorte del plano: la IA");
            text.AppendLine("necesita verla; este archivo no la lleva). Guarda el JSON que te devuelva como archivo `.json` y aplícalo en Revit con el botón");
            text.AppendLine("**Ejecutar especificación JSON** (previsualización con cotas, validación y Crear). Si el validador rechaza algo, pégale a la IA el");
            text.AppendLine("error literal de la ventana y vuelve a aplicar el JSON corregido.");
            text.AppendLine();
            text.AppendLine("## Prompt");
            text.AppendLine();
            text.AppendLine(Prompt);
            text.AppendLine();
            text.AppendLine("## Datos del nudo (conn_get_node_info, del modelo de Revit)");
            text.AppendLine();
            AppendJson(text, input.NodeInfoJson, "(no se pudieron leer los datos del nudo)");
            text.AppendLine();
            text.AppendLine("## Esquema (ejemplo lleno de conn_get_schema para " + input.ConnectionType + ")");
            text.AppendLine();
            AppendJson(text, input.SchemaExampleJson, "(sin esquema)");
            text.AppendLine();
            text.AppendLine("## Cómo leer un detalle (docs/guide.md, secciones 2, 3 y 4)");
            text.AppendLine();
            text.AppendLine(ExtractGuideSections(input.GuideMarkdown).TrimEnd());
            text.AppendLine();
            text.AppendLine("## Ejemplo confirmado (" + (string.IsNullOrWhiteSpace(input.ExampleSource) ? "un JSON que ya se creó bien en Revit" : input.ExampleSource.Trim()) + ")");
            text.AppendLine();
            AppendJson(text, input.ExampleJson, "(sin ejemplo)");
            text.AppendLine();
            text.AppendLine("## Qué devolver");
            text.AppendLine();
            text.AppendLine("1. La tabla corta (cordón, cartela, cada barra con su unión, pernos, soldaduras, retiros y dudas).");
            text.AppendLine("2. El JSON completo en un solo bloque de código, con los `element_id` de la sección \"Datos del nudo\".");
            return text.ToString();
        }

        /// <summary>
        /// Las secciones 2, 3 y 4 de la guía (desde la línea que empieza por <see cref="GuideFirstSection"/> hasta antes de la
        /// que empieza por <see cref="GuideStopSection"/>). Si no se encuentran, la guía entera con una nota.
        /// </summary>
        public static string ExtractGuideSections(string? guideMarkdown)
        {
            if (string.IsNullOrWhiteSpace(guideMarkdown)) return "(la guía docs/guide.md no está disponible: usa las reglas del esquema y del ejemplo)";
            string[] lines = guideMarkdown!.Replace("\r\n", "\n").Split('\n');
            int start = Array.FindIndex(lines, l => l.StartsWith(GuideFirstSection, StringComparison.Ordinal));
            if (start < 0) return "(la guía no tiene la sección 2; se copia entera)\n\n" + guideMarkdown.Trim();
            int stop = Array.FindIndex(lines, start + 1, l => l.StartsWith(GuideStopSection, StringComparison.Ordinal));
            if (stop < 0) stop = lines.Length;
            return string.Join("\n", lines.Skip(start).Take(stop - start)).Trim();
        }

        /// <summary><c>encargo-&lt;documento&gt;-AAAAMMDD-HHMM.md</c>, sin caracteres que un nombre de archivo no admita.</summary>
        public static string FileNameFor(string? documentTitle, DateTime when)
        {
            string name = (documentTitle ?? string.Empty).Trim();
            if (name.EndsWith(".rvt", StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - 4);
            var builder = new StringBuilder();
            foreach (char c in name)
            {
                char clean = char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_';
                if (clean == '_' && builder.Length > 0 && builder[builder.Length - 1] == '_') continue;
                builder.Append(clean);
            }
            string safe = builder.ToString().Trim('_');
            if (safe.Length == 0) safe = "documento";
            if (safe.Length > 60) safe = safe.Substring(0, 60);
            return "encargo-" + safe + "-" + when.ToString("yyyyMMdd-HHmm", CultureInfo.InvariantCulture) + ".md";
        }

        /// <summary>El Detalle D confirmado, embebido en el Core (para cuando el catálogo no tiene plantillas).</summary>
        public static string EmbeddedExampleJson()
        {
            using (Stream? stream = typeof(DesignBriefWriter).Assembly.GetManifestResourceStream(EmbeddedExampleResource))
            {
                if (stream == null) throw new InvalidOperationException("Falta el recurso embebido " + EmbeddedExampleResource + ".");
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static void AppendJson(StringBuilder text, string? json, string empty)
        {
            text.AppendLine("```json");
            text.AppendLine(string.IsNullOrWhiteSpace(json) ? empty : json!.Trim());
            text.AppendLine("```");
        }
    }
}
