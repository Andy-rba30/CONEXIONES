using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using MotorConexiones.Core.Contract;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Resultados posibles de un nudo en un lote (<c>outcome</c>).</summary>
    public static class BatchOutcome
    {
        /// <summary>Creada sin avisos.</summary>
        public const string Created = "created";
        /// <summary>Creada, pero el nudo tenía avisos del plan (perfil distinto, desvío) o Revit avisó al crear.</summary>
        public const string CreatedWithWarnings = "created_with_warnings";
        /// <summary>La conexión existente se rehízo con el mismo <c>connection_id</c> (<c>replace_existing</c>).</summary>
        public const string Updated = "updated";
        /// <summary>Falló: su grupo se revirtió y el lote siguió (o se detuvo con <c>stop_on_error</c>).</summary>
        public const string Failed = "failed";
        /// <summary>No se intentó: estado que no permite crear, token distinto del plan, nudo inexistente, lote detenido.</summary>
        public const string Skipped = "skipped";
        /// <summary>Se había creado y <c>stop_on_error</c> revirtió el lote entero.</summary>
        public const string RolledBack = "rolled_back";
        /// <summary>Borrada por <c>batch_delete</c>.</summary>
        public const string Deleted = "deleted";

        public static bool IsCreated(string outcome) => outcome == Created || outcome == CreatedWithWarnings || outcome == Updated;
    }

    /// <summary>Lo que pasó con un nudo (o con una conexión, al borrar) dentro de un lote.</summary>
    public sealed class BatchNodeResult
    {
        [JsonPropertyName("node")]
        public string? Node { get; set; }

        [JsonPropertyName("outcome")]
        public string Outcome { get; set; } = BatchOutcome.Skipped;

        [JsonPropertyName("connection_id")]
        public string? ConnectionId { get; set; }

        [JsonPropertyName("template_name")]
        public string? TemplateName { get; set; }

        [JsonPropertyName("orientation")]
        public string? Orientation { get; set; }

        [JsonPropertyName("elements_count")]
        public int ElementsCount { get; set; }

        [JsonPropertyName("restored_members_count")]
        public int RestoredMembersCount { get; set; }

        [JsonPropertyName("duration_ms")]
        public long DurationMs { get; set; }

        /// <summary>Por qué se saltó o falló, en español (vacío si se creó).</summary>
        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("errors")]
        public List<ApiError> Errors { get; set; } = new List<ApiError>();

        [JsonPropertyName("warnings")]
        public List<ApiError> Warnings { get; set; } = new List<ApiError>();

        [JsonIgnore]
        public bool IsCreated => BatchOutcome.IsCreated(Outcome);

        /// <summary>"N4: creada (conexión 0d39233d…, 9 elementos)" / "N7: falló (FABRICATION_FAILED: …)".</summary>
        public string Describe()
        {
            string who = Node ?? ConnectionId ?? "?";
            switch (Outcome)
            {
                case BatchOutcome.Created: return who + ": creada" + Suffix();
                case BatchOutcome.CreatedWithWarnings: return who + ": creada con aviso" + Suffix();
                case BatchOutcome.Updated: return who + ": rehecha" + Suffix();
                case BatchOutcome.Deleted: return who + ": borrada (" + ElementsCount + " elementos, " + RestoredMembersCount + " barras restauradas)";
                case BatchOutcome.RolledBack: return who + ": creada y revertida (stop_on_error)";
                case BatchOutcome.Failed: return who + ": falló (" + FirstError() + ")";
                default: return who + ": saltada (" + (Reason ?? "sin motivo") + ")";
            }
        }

        private string Suffix() => " (conexión " + Short(ConnectionId) + ", " + ElementsCount + " elementos)";

        internal string FirstError()
        {
            ApiError? error = Errors.FirstOrDefault();
            if (error == null) return Reason ?? "error desconocido";
            return error.Code + ": " + BatchReport.Short(error.Message);
        }

        private static string Short(string? id) => string.IsNullOrEmpty(id) ? "?" : id!.Length > 8 ? id.Substring(0, 8) + "…" : id;
    }

    /// <summary>
    /// Informe de un lote (Fase 9, sección 3.5 de la propuesta): por nudo <c>created</c> / <c>created_with_warnings</c> /
    /// <c>updated</c> / <c>failed</c> (con sus errores) / <c>skipped</c> (con su motivo) / <c>rolled_back</c>, cuentas,
    /// tiempos, los <c>connection_id</c> creados y un resumen en español. El mismo informe sirve para <c>batch_delete</c>
    /// (<c>deleted</c> / <c>failed</c>). Es lo que devuelve <c>conn_batch_create</c>, lo que enseña la ventana y lo que el
    /// plan guarda en <c>last_report</c>.
    /// </summary>
    public sealed class BatchReport
    {
        public const string OperationCreate = "batch_create";
        public const string OperationDelete = "batch_delete";

        public const string UndoOne = "one";
        public const string UndoPerNode = "per_node";

        [JsonPropertyName("batch_id")]
        public string BatchId { get; set; } = string.Empty;

        [JsonPropertyName("plan_id")]
        public string? PlanId { get; set; }

        [JsonPropertyName("operation")]
        public string Operation { get; set; } = OperationCreate;

        [JsonPropertyName("document")]
        public string? Document { get; set; }

        [JsonPropertyName("started_utc")]
        public string StartedUtc { get; set; } = DateTime.UtcNow.ToString("o");

        [JsonPropertyName("finished_utc")]
        public string? FinishedUtc { get; set; }

        [JsonPropertyName("duration_ms")]
        public long DurationMs { get; set; }

        [JsonPropertyName("stop_on_error")]
        public bool StopOnError { get; set; }

        /// <summary>Verdadero si <c>stop_on_error</c> revirtió el lote entero.</summary>
        [JsonPropertyName("stopped")]
        public bool Stopped { get; set; }

        /// <summary>Nudo en el que se detuvo el lote (solo con <see cref="Stopped"/>).</summary>
        [JsonPropertyName("stopped_at")]
        public string? StoppedAt { get; set; }

        /// <summary><c>one</c> = grupo exterior asimilado (una entrada de deshacer); <c>per_node</c> = plan B (una por nudo).</summary>
        [JsonPropertyName("undo_entries")]
        public string UndoEntries { get; set; } = UndoOne;

        [JsonPropertyName("nodes")]
        public List<BatchNodeResult> Nodes { get; set; } = new List<BatchNodeResult>();

        // ---- cuentas (se calculan; se serializan para que la IA no tenga que contar) ----

        [JsonPropertyName("created_count")]
        public int CreatedCount => Nodes.Count(n => n.Outcome == BatchOutcome.Created || n.Outcome == BatchOutcome.CreatedWithWarnings);

        [JsonPropertyName("updated_count")]
        public int UpdatedCount => Nodes.Count(n => n.Outcome == BatchOutcome.Updated);

        [JsonPropertyName("with_warnings_count")]
        public int WithWarningsCount => Nodes.Count(n => n.IsCreated && (n.Outcome == BatchOutcome.CreatedWithWarnings || n.Warnings.Count > 0));

        [JsonPropertyName("failed_count")]
        public int FailedCount => Nodes.Count(n => n.Outcome == BatchOutcome.Failed);

        [JsonPropertyName("skipped_count")]
        public int SkippedCount => Nodes.Count(n => n.Outcome == BatchOutcome.Skipped);

        [JsonPropertyName("rolled_back_count")]
        public int RolledBackCount => Nodes.Count(n => n.Outcome == BatchOutcome.RolledBack);

        [JsonPropertyName("deleted_count")]
        public int DeletedCount => Nodes.Count(n => n.Outcome == BatchOutcome.Deleted);

        [JsonPropertyName("deleted_elements_count")]
        public int DeletedElementsCount => Nodes.Where(n => n.Outcome == BatchOutcome.Deleted).Sum(n => n.ElementsCount);

        [JsonPropertyName("restored_members_count")]
        public int RestoredMembersCount => Nodes.Where(n => n.Outcome == BatchOutcome.Deleted).Sum(n => n.RestoredMembersCount);

        /// <summary>Las conexiones que quedaron creadas (o rehechas) por este lote.</summary>
        [JsonPropertyName("connection_ids")]
        public List<string> ConnectionIds => Nodes.Where(n => n.IsCreated && !string.IsNullOrEmpty(n.ConnectionId)).Select(n => n.ConnectionId!).ToList();

        [JsonPropertyName("summary_text")]
        public string SummaryText => Summarize();

        [JsonIgnore]
        public bool IsDelete => Operation == OperationDelete;

        public BatchNodeResult? Find(string node) => Nodes.FirstOrDefault(n => string.Equals(n.Node, node, StringComparison.OrdinalIgnoreCase));

        /// <summary>Cierra el informe: fecha de fin y duración.</summary>
        public void Finish(long durationMs)
        {
            FinishedUtc = DateTime.UtcNow.ToString("o");
            DurationMs = durationMs;
        }

        /// <summary>
        /// "Lote 4ef7dd3d: 15 conexiones creadas (14 con aviso), 1 falló (N7: FABRICATION_FAILED: …), 2 saltadas. Una sola
        /// entrada de deshacer (Ctrl+Z). 12,3 s." / "Lote 4ef7dd3d: 16 conexiones borradas (144 elementos, 48 barras
        /// restauradas). Una sola entrada de deshacer (Ctrl+Z)."
        /// </summary>
        public string Summarize()
        {
            string head = "Lote " + (BatchId.Length > 8 ? BatchId.Substring(0, 8) : BatchId) + ": ";
            var parts = new List<string>();
            if (IsDelete)
            {
                int deleted = DeletedCount;
                parts.Add(deleted == 1
                    ? "1 conexión borrada (" + DeletedElementsCount + " elementos, " + RestoredMembersCount + " barras restauradas)"
                    : deleted + " conexiones borradas (" + DeletedElementsCount + " elementos, " + RestoredMembersCount + " barras restauradas)");
                if (FailedCount > 0) parts.Add(FailedCount + (FailedCount == 1 ? " no se pudo borrar" : " no se pudieron borrar") + " (" + string.Join("; ", Nodes.Where(n => n.Outcome == BatchOutcome.Failed).Select(n => (n.Node ?? n.ConnectionId) + ": " + n.FirstError())) + ")");
            }
            else if (Stopped && CreatedCount + UpdatedCount == 0)
            {
                parts.Add("se detuvo en " + (StoppedAt ?? "?") + " y se revirtió todo (stop_on_error): " + RolledBackCount + (RolledBackCount == 1 ? " conexión revertida" : " conexiones revertidas")
                          + ", " + FailedCount + (FailedCount == 1 ? " fallida" : " fallidas") + " (" + string.Join("; ", Nodes.Where(n => n.Outcome == BatchOutcome.Failed).Select(n => n.Node + ": " + n.FirstError())) + ")"
                          + (SkippedCount > 0 ? ", " + SkippedCount + " sin intentar" : ""));
            }
            else if (Stopped)
            {
                // Plan B (una entrada de deshacer por nudo): no hay grupo exterior que revertir; lo creado antes del fallo se queda.
                int kept = CreatedCount + UpdatedCount;
                parts.Add("se detuvo en " + (StoppedAt ?? "?") + " (stop_on_error): " + kept + (kept == 1 ? " conexión creada antes se queda" : " conexiones creadas antes se quedan")
                          + " (una entrada de deshacer por nudo), " + FailedCount + (FailedCount == 1 ? " fallida" : " fallidas") + " (" + string.Join("; ", Nodes.Where(n => n.Outcome == BatchOutcome.Failed).Select(n => n.Node + ": " + n.FirstError())) + ")"
                          + (SkippedCount > 0 ? ", " + SkippedCount + " sin intentar" : ""));
            }
            else
            {
                int created = CreatedCount;
                int updated = UpdatedCount;
                int warned = WithWarningsCount;
                string createdText = created == 1 ? "1 conexión creada" : created + " conexiones creadas";
                if (warned > 0) createdText += " (" + warned + " con aviso)";
                parts.Add(createdText);
                if (updated > 0) parts.Add(updated + (updated == 1 ? " rehecha" : " rehechas"));
                if (FailedCount > 0)
                {
                    parts.Add(FailedCount + (FailedCount == 1 ? " falló" : " fallaron") + " (" + string.Join("; ", Nodes.Where(n => n.Outcome == BatchOutcome.Failed).Select(n => n.Node + ": " + n.FirstError())) + ")");
                }
                if (SkippedCount > 0) parts.Add(SkippedCount + (SkippedCount == 1 ? " saltada" : " saltadas") + " (" + string.Join("; ", Nodes.Where(n => n.Outcome == BatchOutcome.Skipped).Take(4).Select(n => n.Node + ": " + n.Reason)) + (SkippedCount > 4 ? "; …" : "") + ")");
            }
            string text = head + string.Join(", ", parts) + ".";
            if (!Stopped && (CreatedCount + UpdatedCount + DeletedCount) > 0)
            {
                text += UndoEntries == UndoPerNode
                    ? " Una entrada de deshacer por conexión (batch_single_undo: false)."
                    : " Una sola entrada de deshacer (Ctrl+Z).";
            }
            if (DurationMs > 0) text += " " + (DurationMs / 1000.0).ToString("0.#", CultureInfo.InvariantCulture) + " s.";
            return text;
        }

        public string ToJson(bool indented = true) => JsonSerializer.Serialize(this, indented ? PrettyOptions : CompactOptions);

        public static BatchReport? FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<BatchReport>(json, PrettyOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        internal static string Short(string message)
        {
            string text = (message ?? "").Trim().TrimEnd('.');
            return text.Length <= 120 ? text : text.Substring(0, 117) + "…";
        }

        private static readonly JsonSerializerOptions PrettyOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };

        private static readonly JsonSerializerOptions CompactOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };
    }
}
