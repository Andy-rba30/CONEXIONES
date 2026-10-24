using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Storage
{
    /// <summary>
    /// Fase 13 (decisión D8): lo que se guarda en Extensible Storage por cada barra partida con <c>conn_split_member</c> (o
    /// con el botón Partir barra, o por la cadena de Conectar y el lote) para poder deshacerlo: la barra original (el tramo
    /// corto, que conserva el id), el elemento nuevo (el tramo largo), la curva original, la distancia, el cordón y quién lo
    /// pidió. <c>conn_unsplit</c> y Borrar borran el nuevo y devuelven la curva, solo si ninguna conexión usa los tramos.
    /// </summary>
    public sealed class SplitRecord
    {
        [JsonPropertyName("split_id")]
        public string SplitId { get; set; } = Guid.NewGuid().ToString("D");

        /// <summary>La barra partida: conserva su id y se queda con el tramo corto (del lado del cordón).</summary>
        [JsonPropertyName("element_id")]
        public long ElementId { get; set; }

        /// <summary>El elemento nuevo: el tramo largo.</summary>
        [JsonPropertyName("new_element_id")]
        public long NewElementId { get; set; }

        /// <summary>El cordón desde cuya cara se midió la distancia.</summary>
        [JsonPropertyName("chord_element_id")]
        public long ChordElementId { get; set; }

        [JsonPropertyName("original_start_mm")]
        public double[] OriginalStartMm { get; set; } = new double[3];

        [JsonPropertyName("original_end_mm")]
        public double[] OriginalEndMm { get; set; } = new double[3];

        [JsonPropertyName("split_point_mm")]
        public double[] SplitPointMm { get; set; } = new double[3];

        /// <summary>La distancia del Detalle B (330) de la cara del cordón a la primera plancha, por el eje.</summary>
        [JsonPropertyName("offset_from_chord_face_mm")]
        public double OffsetFromChordFaceMm { get; set; }

        /// <summary>Del corte del eje con el eje del cordón al punto de corte de los elementos, por el eje.</summary>
        [JsonPropertyName("split_distance_mm")]
        public double SplitDistanceMm { get; set; }

        /// <summary>Extensiones (Start/End Extension, pies) del original antes de partir, por índice de extremo, para restaurarlas.</summary>
        [JsonPropertyName("original_extensions_feet")]
        public Dictionary<string, double> OriginalExtensionsFeet { get; set; } = new Dictionary<string, double>();

        /// <summary>Cómo se partió: <c>copy_element</c> (CopyElement + LocationCurve) o <c>new_family_instance</c> (la reserva).</summary>
        [JsonPropertyName("mechanism")]
        public string Mechanism { get; set; } = string.Empty;

        /// <summary>Parámetros del original que el tramo nuevo no conserva (vacío si todo igual).</summary>
        [JsonPropertyName("approximated_parameters")]
        public List<string> ApproximatedParameters { get; set; } = new List<string>();

        /// <summary>La conexión que pidió el corte (la cadena de Conectar o el lote), o nulo si fue el botón / la herramienta suelta.</summary>
        [JsonPropertyName("connection_id")]
        public string? ConnectionId { get; set; }

        /// <summary>El lote (<c>plan_id</c>) que pidió el corte, o nulo.</summary>
        [JsonPropertyName("batch_id")]
        public string? BatchId { get; set; }

        [JsonPropertyName("created_utc")]
        public string CreatedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        /// <summary>Los dos tramos: el original (corto) y el nuevo (largo).</summary>
        [JsonIgnore]
        public IEnumerable<long> SegmentIds => new[] { ElementId, NewElementId };

        public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        public static SplitRecord? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize<SplitRecord>(json);
        }
    }
}
