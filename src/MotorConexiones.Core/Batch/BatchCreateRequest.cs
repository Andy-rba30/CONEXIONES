using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Validation;

namespace MotorConexiones.Core.Batch
{
    /// <summary>Un nudo que se pide crear: su nombre en el plan, el token de <c>conn_batch_plan</c> y, opcionalmente, una especificación propia.</summary>
    public sealed class BatchCreateItem
    {
        public BatchCreateItem(string node, string validationToken, JsonObject? spec = null)
        {
            Node = (node ?? string.Empty).Trim();
            ValidationToken = (validationToken ?? string.Empty).Trim();
            Spec = spec;
        }

        /// <summary>Nombre del nudo (<c>N4</c>).</summary>
        public string Node { get; }

        /// <summary>El <c>validation_token</c> que devolvió el plan para ese nudo (sin él no se crea nada).</summary>
        public string ValidationToken { get; }

        /// <summary>Especificación que sustituye a la del plan solo para esta creación (opcional).</summary>
        public JsonObject? Spec { get; }
    }

    /// <summary>
    /// Lo que recibe <c>conn_batch_create</c> (Fase 9, sección 3.5 de la propuesta): el plan y la lista de nudos con su token,
    /// tal como salieron de <c>conn_batch_plan</c>. Se lee del JSON de la petición; cualquier cosa que falte o sobre es
    /// <c>INVALID_REQUEST</c> antes de tocar el modelo. La ventana de la cinta construye la misma petición con los nudos
    /// listos del plan.
    /// </summary>
    public sealed class BatchCreateRequest
    {
        private static readonly HashSet<string> KnownKeys = new HashSet<string>(StringComparer.Ordinal)
        {
            "plan_id", "nodes", "stop_on_error", "include_specs",
        };

        public BatchCreateRequest(string planId, IEnumerable<BatchCreateItem> items, bool stopOnError = false)
        {
            PlanId = (planId ?? string.Empty).Trim();
            Items = (items ?? Enumerable.Empty<BatchCreateItem>()).ToList();
            StopOnError = stopOnError;
        }

        public string PlanId { get; }

        /// <summary>Nudos pedidos, en el orden en que se crearán.</summary>
        public IReadOnlyList<BatchCreateItem> Items { get; }

        /// <summary>P9: con <c>true</c>, el primer nudo que falle revierte el lote entero; por defecto <c>false</c>.</summary>
        public bool StopOnError { get; }

        /// <summary>Si la respuesta lleva la especificación de cada nudo creado (por defecto no: pesa 3 KB por nudo).</summary>
        public bool IncludeSpecs { get; set; }

        /// <summary>Petición con todos los nudos creables de un plan (<c>ready</c> y <c>failed</c>) y sus tokens: la que hace el botón Crear N conexiones.</summary>
        public static BatchCreateRequest ForPlan(BatchPlan plan, bool stopOnError = false)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            var items = plan.Nodes.Where(BatchRunner.CanBeCreated).Select(n => new BatchCreateItem(n.Name, n.ValidationToken ?? string.Empty));
            return new BatchCreateRequest(plan.PlanId, items, stopOnError);
        }

        /// <summary>Lee la petición del JSON de <c>conn_batch_create</c>. Lanza <see cref="CatalogException"/> con <c>INVALID_REQUEST</c>.</summary>
        public static BatchCreateRequest FromJson(JsonElement request)
        {
            if (request.ValueKind != JsonValueKind.Object)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "La petición de batch_create debe ser un objeto JSON.", "",
                    "Envía {\"plan_id\": \"...\", \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"...\"}]}.");
            }
            foreach (JsonProperty property in request.EnumerateObject())
            {
                if (!KnownKeys.Contains(property.Name))
                {
                    throw new CatalogException(ErrorCodes.InvalidRequest, "'" + property.Name + "' no es una clave de batch_create.", property.Name,
                        "Claves admitidas: plan_id, nodes, stop_on_error, include_specs.");
                }
            }

            string planId = request.TryGetProperty("plan_id", out JsonElement planEl) && planEl.ValueKind == JsonValueKind.String ? (planEl.GetString() ?? "").Trim() : "";
            if (planId.Length == 0)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "Falta plan_id: el lote se crea a partir de un plan de conn_batch_plan.", "plan_id",
                    "Planifica con conn_batch_plan y pasa su plan_id y los nudos con sus tokens.");
            }

            if (!request.TryGetProperty("nodes", out JsonElement nodesEl) || nodesEl.ValueKind != JsonValueKind.Array || nodesEl.GetArrayLength() == 0)
            {
                throw new CatalogException(ErrorCodes.InvalidRequest, "Falta nodes: la lista de nudos a crear, cada uno con su validation_token del plan.", "nodes",
                    "Ejemplo: \"nodes\": [{\"node\": \"N4\", \"validation_token\": \"<token de conn_batch_plan>\"}]. Sin token no se crea nada.");
            }

            var items = new List<BatchCreateItem>();
            int index = 0;
            foreach (JsonElement item in nodesEl.EnumerateArray())
            {
                string path = "nodes[" + index + "]";
                index++;
                if (item.ValueKind == JsonValueKind.String)
                {
                    throw new CatalogException(ErrorCodes.InvalidRequest, "El nudo '" + item.GetString() + "' viene sin validation_token.", path,
                        "Cada nudo va como {\"node\": \"" + item.GetString() + "\", \"validation_token\": \"...\"}; el token lo devolvió conn_batch_plan.");
                }
                if (item.ValueKind != JsonValueKind.Object)
                {
                    throw new CatalogException(ErrorCodes.InvalidRequest, "Cada elemento de nodes debe ser un objeto {node, validation_token}.", path);
                }
                string name = item.TryGetProperty("node", out JsonElement nameEl) && nameEl.ValueKind == JsonValueKind.String ? (nameEl.GetString() ?? "").Trim() : "";
                if (name.Length == 0 && item.TryGetProperty("name", out JsonElement altEl) && altEl.ValueKind == JsonValueKind.String) name = (altEl.GetString() ?? "").Trim();
                if (name.Length == 0)
                {
                    throw new CatalogException(ErrorCodes.InvalidRequest, "Un elemento de nodes no dice qué nudo es (falta 'node').", path + ".node",
                        "Usa los nombres de nodes[].name del plan (N1, N2…).");
                }
                string token = item.TryGetProperty("validation_token", out JsonElement tokenEl) && tokenEl.ValueKind == JsonValueKind.String ? (tokenEl.GetString() ?? "").Trim() : "";
                if (token.Length == 0)
                {
                    throw new CatalogException(ErrorCodes.InvalidRequest, "El nudo " + name + " viene sin validation_token.", path + ".validation_token",
                        "Pasa el validation_token que conn_batch_plan (o conn_batch_plan_get) devolvió para " + name + ". Sin token no se crea nada.");
                }
                JsonObject? spec = null;
                if (item.TryGetProperty("spec", out JsonElement specEl) && specEl.ValueKind == JsonValueKind.Object)
                {
                    spec = JsonNode.Parse(specEl.GetRawText()) as JsonObject;
                }
                items.Add(new BatchCreateItem(name, token, spec));
            }

            bool stop = request.TryGetProperty("stop_on_error", out JsonElement stopEl) && stopEl.ValueKind == JsonValueKind.True;
            bool includeSpecs = request.TryGetProperty("include_specs", out JsonElement specsEl) && specsEl.ValueKind == JsonValueKind.True;
            return new BatchCreateRequest(planId, items, stop) { IncludeSpecs = includeSpecs };
        }
    }
}
