using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MotorConexiones.Core.Storage
{
    /// <summary>
    /// Registro persistido en Extensible Storage (DataStorage) por cada conexión creada.
    /// Contiene la especificación completa, los elementos creados y los valores originales para restauración.
    /// </summary>
    public sealed class ConnectionRecord
    {
        [JsonPropertyName("connection_id")]
        public string ConnectionId { get; set; } = Guid.NewGuid().ToString("D");

        [JsonPropertyName("spec_version")]
        public string SpecVersion { get; set; } = "1.0";

        [JsonPropertyName("connection_type")]
        public string ConnectionType { get; set; } = "gusset_node";

        [JsonPropertyName("spec_json")]
        public string SpecJson { get; set; } = "{}";

        [JsonPropertyName("created_element_ids")]
        public List<long> CreatedElementIds { get; set; } = new List<long>();

        [JsonPropertyName("modified_members")]
        public List<ModifiedMemberRecord> ModifiedMembers { get; set; } = new List<ModifiedMemberRecord>();

        [JsonPropertyName("created_utc")]
        public string CreatedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        }

        public static ConnectionRecord? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonSerializer.Deserialize<ConnectionRecord>(json);
        }
    }
}
