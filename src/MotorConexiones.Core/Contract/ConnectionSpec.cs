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

        [JsonPropertyName("gusset")]
        public GussetSpec? Gusset { get; set; }

        [JsonPropertyName("members")]
        public List<MemberSpec> Members { get; set; } = new List<MemberSpec>();

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
    }
}
