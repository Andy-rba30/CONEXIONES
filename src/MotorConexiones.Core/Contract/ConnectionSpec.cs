using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Contract
{
    /// <summary>
    /// Modelo raíz del contrato v1 para especificaciones de conexiones de acero.
    /// </summary>
    public sealed class ConnectionSpec
    {
        [JsonPropertyName("spec_version")]
        public string SpecVersion { get; set; } = "1.0";

        [JsonPropertyName("connection_type")]
        public string ConnectionType { get; set; } = "gusset_node";

        [JsonPropertyName("source")]
        public SourceInfo? Source { get; set; }

        [JsonPropertyName("node")]
        public NodeRef Node { get; set; } = new NodeRef();

        [JsonPropertyName("chord")]
        public ChordSpec? Chord { get; set; }

        /// <summary>La cartela (<c>gusset_node</c>). Fase 12: se omite si es nula (los otros tipos no la llevan); una cartela válida siempre la tiene.</summary>
        [JsonPropertyName("gusset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GussetSpec? Gusset { get; set; }

        [JsonPropertyName("members")]
        public List<MemberSpec> Members { get; set; } = new List<MemberSpec>();

        /// <summary>Fase 12, <c>end_plate</c>: <c>splice</c> (dos tramos de la misma barra). <c>to_support</c> queda previsto, no programado.</summary>
        [JsonPropertyName("kind")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Kind { get; set; }

        /// <summary>Fase 12, <c>end_plate</c>: la plancha (igual en los dos tramos).</summary>
        [JsonPropertyName("plate")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public EndPlateSpec? Plate { get; set; }

        /// <summary>Fase 12, <c>end_plate</c>: los pernos alrededor del tubo, por cara.</summary>
        [JsonPropertyName("bolts")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public RingBoltSpec? Bolts { get; set; }

        /// <summary>Fase 12, <c>end_plate</c>: holgura entre las dos planchas (0 = en contacto, decisión D2).</summary>
        [JsonPropertyName("gap_mm")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? GapMm { get; set; }

        [JsonPropertyName("dimension_chains")]
        public List<DimensionChain> DimensionChains { get; set; } = new List<DimensionChain>();

        [JsonPropertyName("uncertain_fields")]
        public List<UncertainField> UncertainFields { get; set; } = new List<UncertainField>();

        public static ConnectionSpec? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize<ConnectionSpec>(json, JsonOptions.Default);
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, JsonOptions.Default);
        }

        /// <summary>Verdadero si <c>connection_type</c> es el indicado (sin distinguir mayúsculas).</summary>
        public bool IsType(string typeName) => string.Equals(ConnectionType, typeName, StringComparison.OrdinalIgnoreCase);
    }
}
