using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.Operations
{
    /// <summary>
    /// <c>conn_list</c>: lista todas las conexiones creadas por el add-in almacenadas en el modelo activo. Fase 9: cada
    /// conexión lleva su <c>batch_id</c> (<c>source.batch_id</c>, el plan que la creó), <c>batches</c> cuenta cuántas hay por
    /// lote y, con <c>batch_id</c> en la petición, solo se listan las de ese lote.
    /// </summary>
    public sealed class ListOperation : IOperation
    {
        public string Name => "list";
        public bool RequiresDocument => true;
        public bool ModifiesModel => false;

        public ApiResponse Execute(OperationContext context)
        {
            Document doc = context.Document!;
            string? filter = context.TryGet("batch_id", out JsonElement batchEl) && batchEl.ValueKind == JsonValueKind.String ? (batchEl.GetString() ?? "").Trim() : null;
            if (string.IsNullOrEmpty(filter)) filter = null;
            var records = ConnectionStorageManager.ListConnections(doc);

            var all = records.Select(r =>
            {
                var (templateId, batchId) = SourceOf(r.SpecJson);
                return new
                {
                    connection_id = r.ConnectionId,
                    spec_version = r.SpecVersion,
                    connection_type = r.ConnectionType,
                    created_elements_count = r.CreatedElementIds.Count,
                    backend = r.BackendName,
                    created_utc = r.CreatedUtc,
                    template_id = templateId,
                    batch_id = batchId,
                };
            }).ToList();
            var batches = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in all.Where(i => i.batch_id != null))
            {
                batches[item.batch_id!] = batches.TryGetValue(item.batch_id!, out int count) ? count + 1 : 1;
            }
            var items = filter == null ? all : all.Where(i => string.Equals(i.batch_id, filter, StringComparison.OrdinalIgnoreCase)).ToList();

            var data = new
            {
                connections_count = items.Count,
                total_count = all.Count,
                batch_id = filter,
                batches,
                connections = items
            };

            return ApiResponse.Success(Name, data, context.Warnings);
        }

        /// <summary>Plantilla (<c>source.template_id</c>, Fase 7) y lote (<c>source.batch_id</c>, Fase 9) de los que salió la conexión.</summary>
        private static (string? TemplateId, string? BatchId) SourceOf(string specJson)
        {
            try
            {
                SourceInfo? source = ConnectionSpec.FromJson(specJson)?.Source;
                return (source?.TemplateId, string.IsNullOrWhiteSpace(source?.BatchId) ? null : source!.BatchId);
            }
            catch
            {
                return (null, null);
            }
        }
    }
}
