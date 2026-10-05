using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Paleta fija de las marcas (un color por nudo; se repite a partir del 13).</summary>
    public static class PlanPalette
    {
        private static readonly (string Name, byte R, byte G, byte B)[] Colors =
        {
            ("rojo", 230, 25, 75), ("verde", 60, 180, 75), ("azul", 0, 130, 200), ("naranja", 245, 130, 48),
            ("morado", 145, 30, 180), ("cian", 70, 240, 240), ("magenta", 240, 50, 230), ("lima", 210, 245, 60),
            ("rosa", 250, 190, 212), ("turquesa", 0, 128, 128), ("marrón", 154, 99, 36), ("amarillo", 255, 225, 25),
        };

        public static int Count => Colors.Length;

        public static string NameOf(int index) => Colors[Mod(index)].Name;

        public static (byte R, byte G, byte B) RgbOf(int index)
        {
            var c = Colors[Mod(index)];
            return (c.R, c.G, c.B);
        }

        private static int Mod(int index) => ((index % Colors.Length) + Colors.Length) % Colors.Length;
    }

    /// <summary>Una barra de un nudo del plan.</summary>
    public sealed class PlanMember
    {
        [JsonPropertyName("element_id")]
        public long ElementId { get; set; }

        [JsonPropertyName("angle_deg")]
        public double AngleDeg { get; set; }

        [JsonPropertyName("side")]
        public string Side { get; set; } = "+Y";

        [JsonPropertyName("type_name")]
        public string? TypeName { get; set; }

        [JsonPropertyName("reaches_node")]
        public bool ReachesNode { get; set; } = true;

        /// <summary>Distancia del extremo real de la barra al punto de trabajo (ronda 8b): 0 si llega al eje, 20 a 90 mm si termina en la cara del cordón.</summary>
        [JsonPropertyName("end_gap_mm")]
        public double EndGapMm { get; set; }
    }

    /// <summary>Un nudo del plan: detección, casado, especificación y validación.</summary>
    public sealed class PlanNode
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = NodeStatus.Detected;

        [JsonPropertyName("status_detail")]
        public string? StatusDetail { get; set; }

        [JsonPropertyName("work_point_mm")]
        public double[] WorkPointMm { get; set; } = new double[3];

        [JsonPropertyName("chord_element_id")]
        public long ChordElementId { get; set; }

        [JsonPropertyName("chord_continuous")]
        public bool ChordContinuous { get; set; }

        [JsonPropertyName("chord_type_name")]
        public string? ChordTypeName { get; set; }

        /// <summary>Barras que atraviesan el nudo (más de una en <c>ambiguous_chord</c>).</summary>
        [JsonPropertyName("through_element_ids")]
        public List<long> ThroughElementIds { get; set; } = new List<long>();

        /// <summary>Barras del nudo sin el cordón.</summary>
        [JsonPropertyName("member_element_ids")]
        public List<long> MemberElementIds { get; set; } = new List<long>();

        /// <summary>Cordón y barras.</summary>
        [JsonPropertyName("element_ids")]
        public List<long> ElementIds { get; set; } = new List<long>();

        [JsonPropertyName("members")]
        public List<PlanMember> Members { get; set; } = new List<PlanMember>();

        [JsonPropertyName("signature")]
        public string Signature { get; set; } = string.Empty;

        [JsonPropertyName("is_manual")]
        public bool IsManual { get; set; }

        [JsonPropertyName("template_id")]
        public string? TemplateId { get; set; }

        [JsonPropertyName("template_name")]
        public string? TemplateName { get; set; }

        /// <summary><c>same</c>, <c>mirror_x</c>, <c>mirror_y</c>, <c>both</c> o nulo si no casó.</summary>
        [JsonPropertyName("orientation")]
        public string? Orientation { get; set; }

        [JsonPropertyName("is_mirrored")]
        public bool IsMirrored { get; set; }

        [JsonPropertyName("max_deviation_deg")]
        public double? MaxDeviationDeg { get; set; }

        /// <summary>Asignación ranura → barra del casado elegido (el <c>match</c> de <c>conn_catalog_apply</c>).</summary>
        [JsonPropertyName("match")]
        public JsonObject? Match { get; set; }

        /// <summary>Con <c>no_match</c>: el mejor intento de cada plantilla y orientación.</summary>
        [JsonPropertyName("attempts")]
        public List<string> Attempts { get; set; } = new List<string>();

        /// <summary>Especificación instanciada (o la editada a mano) con IDs reales y <c>source.batch_id</c>.</summary>
        [JsonPropertyName("spec")]
        public JsonObject? Spec { get; set; }

        [JsonPropertyName("has_spec_override")]
        public bool HasSpecOverride { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("validation_token")]
        public string? ValidationToken { get; set; }

        [JsonPropertyName("errors")]
        public List<ApiError> Errors { get; set; } = new List<ApiError>();

        [JsonPropertyName("warnings")]
        public List<ApiError> Warnings { get; set; } = new List<ApiError>();

        /// <summary>Conexión del add-in que ya toca alguna barra del nudo.</summary>
        [JsonPropertyName("existing_connection_id")]
        public string? ExistingConnectionId { get; set; }

        /// <summary>Verdadero si se planificó para rehacer la conexión existente (<c>replace_existing</c>).</summary>
        [JsonPropertyName("replaces_existing")]
        public bool ReplacesExisting { get; set; }

        [JsonPropertyName("color_index")]
        public int ColorIndex { get; set; }

        [JsonPropertyName("color_name")]
        public string ColorName { get; set; } = string.Empty;

        [JsonPropertyName("color_rgb")]
        public int[] ColorRgb { get; set; } = new int[3];

        /// <summary>Verdadero si el nudo lleva marcas en el modelo.</summary>
        [JsonPropertyName("is_marked")]
        public bool IsMarked { get; set; }

        [JsonPropertyName("marker_element_id")]
        public long? MarkerElementId { get; set; }

        [JsonIgnore]
        public string SpecJson => Spec == null ? string.Empty : Spec.ToJsonString(BatchPlan.PrettyOptions);

        [JsonIgnore]
        public bool IsReady => Status == NodeStatus.Ready && !string.IsNullOrEmpty(ValidationToken);

        [JsonIgnore]
        public bool CanBeMarked => Status != NodeStatus.Excluded && Status != NodeStatus.Untyped && Status != NodeStatus.AlreadyConnected;

        /// <summary>Texto corto para la tabla de la ventana y para el chat.</summary>
        public string Describe()
        {
            string template = TemplateName ?? TemplateId ?? "-";
            string orientation = Orientation == null ? "" : " (" + Orientation + ")";
            return Name + ": " + Status + " · cordón " + ChordElementId + " · barras " + string.Join(", ", MemberElementIds)
                   + " · plantilla " + template + orientation
                   + (Errors.Count > 0 ? " · " + Errors.Count + " error(es)" : "")
                   + (Warnings.Count > 0 ? " · " + Warnings.Count + " aviso(s)" : "");
        }
    }

    /// <summary>
    /// El plan de un lote (sección 3.1 de la propuesta): nudos detectados y casados con su validación y su token,
    /// las correcciones acumuladas y las marcas puestas en el modelo. Es lo que devuelve <c>conn_batch_plan</c>, lo que
    /// enseña la ventana de la cinta y lo que la Fase 9 recibirá para crear.
    /// </summary>
    public sealed class BatchPlan
    {
        public static readonly JsonSerializerOptions PrettyOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };

        [JsonPropertyName("plan_id")]
        public string PlanId { get; set; } = Guid.NewGuid().ToString("D");

        [JsonPropertyName("created_utc")]
        public string CreatedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("updated_utc")]
        public string UpdatedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("document")]
        public string? Document { get; set; }

        /// <summary>Barras seleccionadas al planificar (sin las que añaden las correcciones).</summary>
        [JsonPropertyName("selection_ids")]
        public List<long> SelectionIds { get; set; } = new List<long>();

        /// <summary>Plantillas pedidas (vacío = todas las del catálogo).</summary>
        [JsonPropertyName("template_ids")]
        public List<string> TemplateIds { get; set; } = new List<string>();

        /// <summary>Plantillas que se probaron de verdad (id → nombre).</summary>
        [JsonPropertyName("templates")]
        public Dictionary<string, string> Templates { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("overrides")]
        public BatchOverrides Overrides { get; set; } = new BatchOverrides();

        [JsonPropertyName("nodes")]
        public List<PlanNode> Nodes { get; set; } = new List<PlanNode>();

        /// <summary>Barras que no quedaron en ningún nudo con cordón y barras (sueltas o solo en nudos sin tipo).</summary>
        [JsonPropertyName("unused_element_ids")]
        public List<long> UnusedElementIds { get; set; } = new List<long>();

        /// <summary>Avisos de la planificación (plantillas ilegibles, barras que no se pudieron leer...).</summary>
        [JsonPropertyName("warnings")]
        public List<ApiError> Warnings { get; set; } = new List<ApiError>();

        // ---- marcas (las rellena el add-in) ----

        [JsonPropertyName("is_marked")]
        public bool IsMarked { get; set; }

        [JsonPropertyName("marked_view_id")]
        public long? MarkedViewId { get; set; }

        /// <summary>Barras con override de gráficos en la vista marcada.</summary>
        [JsonPropertyName("marked_element_ids")]
        public List<long> MarkedElementIds { get; set; } = new List<long>();

        /// <summary>Marcadores (DirectShape) creados en el modelo.</summary>
        [JsonPropertyName("marker_element_ids")]
        public List<long> MarkerElementIds { get; set; } = new List<long>();

        [JsonIgnore]
        public int ReadyCount => Nodes.Count(n => n.Status == NodeStatus.Ready);

        public PlanNode? Find(string name) => Nodes.FirstOrDefault(n => string.Equals(n.Name, name, StringComparison.OrdinalIgnoreCase));

        /// <summary>Cuenta por estado, para el resumen de la respuesta y de la ventana.</summary>
        public Dictionary<string, int> Summary()
        {
            var summary = new Dictionary<string, int>();
            foreach (PlanNode node in Nodes)
            {
                summary[node.Status] = summary.TryGetValue(node.Status, out int count) ? count + 1 : 1;
            }
            return summary;
        }

        /// <summary>Resumen de una línea ("8 nudos: 6 ready, 1 no_match, 1 untyped").</summary>
        public string Describe()
        {
            var parts = Summary().OrderByDescending(p => p.Value).ThenBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Value + " " + p.Key);
            return Nodes.Count + " nudo(s): " + string.Join(", ", parts) + ".";
        }

        public string ToJson(bool indented = true) => JsonSerializer.Serialize(this, indented ? PrettyOptions : CompactOptions);

        public static BatchPlan? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                BatchPlan? plan = JsonSerializer.Deserialize<BatchPlan>(json, PrettyOptions);
                if (plan == null || string.IsNullOrWhiteSpace(plan.PlanId)) return null;
                plan.Overrides ??= new BatchOverrides();
                plan.Nodes ??= new List<PlanNode>();
                return plan;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static readonly JsonSerializerOptions CompactOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };
    }
}
