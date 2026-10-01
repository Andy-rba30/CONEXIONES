using System;
using System.Collections.Generic;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using MotorConexiones.Core.Storage;

namespace MotorConexiones.Revit.Storage
{
    /// <summary>
    /// Gestiona la persistencia de conexiones en el modelo mediante Extensible Storage sobre elementos <see cref="DataStorage"/>.
    /// Nombre de esquema: <c>MotorConexionesConnection</c> (GUID fijo, versión 1).
    /// </summary>
    public static class ConnectionStorageManager
    {
        public static readonly Guid SchemaGuid = new Guid("8A6C4D2E-3F1B-4E5A-9C7D-2E4F6A8B0C1D");
        public const string SchemaName = "MotorConexionesConnection";
        public const string VendorId = "MCNX";

        private static Schema? _cachedSchema;

        public static Schema GetOrCreateSchema()
        {
            if (_cachedSchema != null) return _cachedSchema;

            Schema schema = Schema.Lookup(SchemaGuid);
            if (schema != null)
            {
                _cachedSchema = schema;
                return schema;
            }

            var builder = new SchemaBuilder(SchemaGuid);
            builder.SetSchemaName(SchemaName);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);
            builder.SetVendorId(VendorId);
            builder.SetDocumentation("Almacena la especificación y metadatos de conexiones creadas por MotorConexiones.");

            builder.AddSimpleField("ConnectionId", typeof(string));
            builder.AddSimpleField("SpecVersion", typeof(string));
            builder.AddSimpleField("ConnectionType", typeof(string));
            builder.AddSimpleField("SpecJson", typeof(string));
            builder.AddSimpleField("CreatedElementIdsJson", typeof(string));
            builder.AddSimpleField("ModifiedMembersJson", typeof(string));
            builder.AddSimpleField("CreatedUtc", typeof(string));
            builder.AddSimpleField("Backend", typeof(string));

            _cachedSchema = builder.Finish();
            return _cachedSchema;
        }

        public static DataStorage SaveConnection(Document document, ConnectionRecord record)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (record == null) throw new ArgumentNullException(nameof(record));

            Schema schema = GetOrCreateSchema();

            // Buscar si ya existe un DataStorage con este ConnectionId (para actualización)
            DataStorage? storage = FindDataStorage(document, schema, record.ConnectionId);
            if (storage == null)
            {
                storage = DataStorage.Create(document);
            }

            Entity entity = new Entity(schema);
            entity.Set("ConnectionId", record.ConnectionId ?? string.Empty);
            entity.Set("SpecVersion", record.SpecVersion ?? "1.0");
            entity.Set("ConnectionType", record.ConnectionType ?? "gusset_node");
            entity.Set("SpecJson", record.SpecJson ?? "{}");
            entity.Set("CreatedElementIdsJson", JsonSerializer.Serialize(record.CreatedElementIds ?? new List<long>()));
            entity.Set("ModifiedMembersJson", JsonSerializer.Serialize(record.ModifiedMembers ?? new List<ModifiedMemberRecord>()));
            entity.Set("CreatedUtc", record.CreatedUtc ?? DateTime.UtcNow.ToString("o"));
            entity.Set("Backend", record.BackendName ?? string.Empty);

            storage.SetEntity(entity);
            return storage;
        }

        public static ConnectionRecord? GetConnection(Document document, string connectionId)
        {
            if (document == null || string.IsNullOrWhiteSpace(connectionId)) return null;

            Schema schema = GetOrCreateSchema();
            DataStorage? storage = FindDataStorage(document, schema, connectionId);
            if (storage == null) return null;

            return ExtractRecord(storage, schema);
        }

        public static IReadOnlyList<ConnectionRecord> ListConnections(Document document)
        {
            var results = new List<ConnectionRecord>();
            if (document == null) return results;

            Schema schema = GetOrCreateSchema();
            var collector = new FilteredElementCollector(document).OfClass(typeof(DataStorage));

            foreach (DataStorage storage in collector)
            {
                ConnectionRecord? record = ExtractRecord(storage, schema);
                if (record != null)
                {
                    results.Add(record);
                }
            }

            return results;
        }

        public static bool DeleteConnection(Document document, string connectionId, out ConnectionRecord? deletedRecord)
        {
            deletedRecord = null;
            if (document == null || string.IsNullOrWhiteSpace(connectionId)) return false;

            Schema schema = GetOrCreateSchema();
            DataStorage? storage = FindDataStorage(document, schema, connectionId);
            if (storage == null) return false;

            deletedRecord = ExtractRecord(storage, schema);
            document.Delete(storage.Id);
            return true;
        }

        private static DataStorage? FindDataStorage(Document document, Schema schema, string connectionId)
        {
            var collector = new FilteredElementCollector(document).OfClass(typeof(DataStorage));
            foreach (DataStorage storage in collector)
            {
                Entity entity = storage.GetEntity(schema);
                if (entity != null && entity.IsValid())
                {
                    string id = entity.Get<string>("ConnectionId");
                    if (string.Equals(id, connectionId, StringComparison.OrdinalIgnoreCase))
                    {
                        return storage;
                    }
                }
            }
            return null;
        }

        private static ConnectionRecord? ExtractRecord(DataStorage storage, Schema schema)
        {
            Entity entity = storage.GetEntity(schema);
            if (entity == null || !entity.IsValid()) return null;

            try
            {
                string connectionId = entity.Get<string>("ConnectionId");
                string specVersion = entity.Get<string>("SpecVersion");
                string connectionType = entity.Get<string>("ConnectionType");
                string specJson = entity.Get<string>("SpecJson");
                string createdElementIdsJson = entity.Get<string>("CreatedElementIdsJson");
                string modifiedMembersJson = entity.Get<string>("ModifiedMembersJson");
                string createdUtc = entity.Get<string>("CreatedUtc");
                string backend = entity.Get<string>("Backend");

                var elementIds = !string.IsNullOrWhiteSpace(createdElementIdsJson)
                    ? JsonSerializer.Deserialize<List<long>>(createdElementIdsJson) ?? new List<long>()
                    : new List<long>();

                var modifiedMembers = !string.IsNullOrWhiteSpace(modifiedMembersJson)
                    ? JsonSerializer.Deserialize<List<ModifiedMemberRecord>>(modifiedMembersJson) ?? new List<ModifiedMemberRecord>()
                    : new List<ModifiedMemberRecord>();

                return new ConnectionRecord
                {
                    ConnectionId = connectionId,
                    SpecVersion = specVersion,
                    ConnectionType = connectionType,
                    SpecJson = specJson,
                    CreatedElementIds = elementIds,
                    ModifiedMembers = modifiedMembers,
                    CreatedUtc = createdUtc,
                    BackendName = backend
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
