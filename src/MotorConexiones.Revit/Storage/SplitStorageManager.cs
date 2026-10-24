using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using MotorConexiones.Core.Storage;

namespace MotorConexiones.Revit.Storage
{
    /// <summary>
    /// Fase 13 (D8): los cortes de barra (<see cref="SplitRecord"/>) en Extensible Storage, sobre elementos <see cref="DataStorage"/>
    /// propios. Es un esquema <b>nuevo</b> (<c>MotorConexionesSplit</c>, GUID fijo, versión 1): el esquema de las conexiones
    /// (<c>MotorConexionesConnection</c>) no se toca, porque un esquema de Extensible Storage no admite campos nuevos con el
    /// mismo GUID, y así los registros de las conexiones de las fases anteriores se leen sin cambios. El registro va entero en
    /// JSON (<c>RecordJson</c>) más dos campos de búsqueda.
    /// </summary>
    public static class SplitStorageManager
    {
        public static readonly Guid SchemaGuid = new Guid("3D7E9A1C-5B2F-4C8E-9F1A-6D4B2E8C0A7F");
        public const string SchemaName = "MotorConexionesSplit";
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
            builder.SetDocumentation("Fase 13 de MotorConexiones: una barra de armazón partida en dos (tramo corto = el original, tramo largo = el nuevo) y lo necesario para deshacerlo.");
            builder.AddSimpleField("SplitId", typeof(string));
            builder.AddSimpleField("ElementIds", typeof(string));
            builder.AddSimpleField("RecordJson", typeof(string));
            _cachedSchema = builder.Finish();
            return _cachedSchema;
        }

        /// <summary>Guarda (o actualiza) el registro. Llamar dentro de una Transaction.</summary>
        public static DataStorage Save(Document document, SplitRecord record)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (record == null) throw new ArgumentNullException(nameof(record));
            Schema schema = GetOrCreateSchema();
            DataStorage? storage = Find(document, schema, record.SplitId);
            storage ??= DataStorage.Create(document);
            var entity = new Entity(schema);
            entity.Set("SplitId", record.SplitId);
            entity.Set("ElementIds", record.ElementId + "," + record.NewElementId);
            entity.Set("RecordJson", record.ToJson());
            storage.SetEntity(entity);
            return storage;
        }

        public static SplitRecord? Get(Document document, string splitId)
        {
            if (document == null || string.IsNullOrWhiteSpace(splitId)) return null;
            Schema schema = GetOrCreateSchema();
            DataStorage? storage = Find(document, schema, splitId);
            return storage == null ? null : Extract(storage, schema);
        }

        public static IReadOnlyList<SplitRecord> List(Document document)
        {
            var results = new List<SplitRecord>();
            if (document == null) return results;
            Schema schema = GetOrCreateSchema();
            foreach (DataStorage storage in new FilteredElementCollector(document).OfClass(typeof(DataStorage)))
            {
                SplitRecord? record = Extract(storage, schema);
                if (record != null) results.Add(record);
            }
            return results;
        }

        /// <summary>El corte en el que una barra es el original o el tramo nuevo, o nulo.</summary>
        public static SplitRecord? FindByElement(Document document, long elementId) =>
            List(document).FirstOrDefault(r => r.ElementId == elementId || r.NewElementId == elementId);

        /// <summary>Los cortes que pidió una conexión (la cadena de Conectar o el lote).</summary>
        public static List<SplitRecord> ByConnection(Document document, string connectionId) =>
            List(document).Where(r => !string.IsNullOrEmpty(r.ConnectionId) && string.Equals(r.ConnectionId, connectionId, StringComparison.OrdinalIgnoreCase)).ToList();

        /// <summary>Los cortes de un lote (<c>batch_id</c> = <c>plan_id</c>).</summary>
        public static List<SplitRecord> ByBatch(Document document, string batchId) =>
            List(document).Where(r => !string.IsNullOrEmpty(r.BatchId) && string.Equals(r.BatchId, batchId, StringComparison.OrdinalIgnoreCase)).ToList();

        /// <summary>Borra el registro (no toca las barras). Llamar dentro de una Transaction.</summary>
        public static bool Delete(Document document, string splitId)
        {
            if (document == null || string.IsNullOrWhiteSpace(splitId)) return false;
            Schema schema = GetOrCreateSchema();
            DataStorage? storage = Find(document, schema, splitId);
            if (storage == null) return false;
            document.Delete(storage.Id);
            return true;
        }

        private static DataStorage? Find(Document document, Schema schema, string splitId)
        {
            foreach (DataStorage storage in new FilteredElementCollector(document).OfClass(typeof(DataStorage)))
            {
                Entity entity = storage.GetEntity(schema);
                if (entity != null && entity.IsValid() && string.Equals(entity.Get<string>("SplitId"), splitId, StringComparison.OrdinalIgnoreCase)) return storage;
            }
            return null;
        }

        private static SplitRecord? Extract(DataStorage storage, Schema schema)
        {
            Entity entity = storage.GetEntity(schema);
            if (entity == null || !entity.IsValid()) return null;
            try
            {
                return SplitRecord.FromJson(entity.Get<string>("RecordJson"));
            }
            catch
            {
                return null;
            }
        }
    }
}
