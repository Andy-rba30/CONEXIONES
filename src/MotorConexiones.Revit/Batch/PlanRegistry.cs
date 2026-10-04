using System;
using System.Collections.Generic;
using System.Linq;
using MotorConexiones.Core.Batch;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Planes en memoria del add-in (<c>plan_id</c> → plan) mientras Revit siga abierto. Un plan vive hasta que se
    /// descarta (<c>batch_plan_discard</c> o el botón Descartar) o se cierra Revit; las marcas del modelo llevan el
    /// <c>plan_id</c> en sus marcadores para poder limpiarlas aunque el plan ya no esté aquí.
    /// </summary>
    public static class PlanRegistry
    {
        private static readonly object Gate = new object();
        private static readonly Dictionary<string, BatchPlan> Plans = new Dictionary<string, BatchPlan>(StringComparer.OrdinalIgnoreCase);
        private static string? _lastPlanId;

        public static void Put(BatchPlan plan)
        {
            if (plan == null) throw new ArgumentNullException(nameof(plan));
            lock (Gate)
            {
                Plans[plan.PlanId] = plan;
                _lastPlanId = plan.PlanId;
            }
        }

        /// <summary>El plan pedido o, sin <paramref name="planId"/>, el último planificado. Nulo si no hay.</summary>
        public static BatchPlan? Get(string? planId)
        {
            lock (Gate)
            {
                string? key = string.IsNullOrWhiteSpace(planId) ? _lastPlanId : planId!.Trim();
                return key != null && Plans.TryGetValue(key, out BatchPlan? plan) ? plan : null;
            }
        }

        /// <summary>El último plan de un documento (por título), para que el botón de la cinta lo reabra.</summary>
        public static BatchPlan? LastFor(string? documentTitle)
        {
            lock (Gate)
            {
                if (_lastPlanId != null && Plans.TryGetValue(_lastPlanId, out BatchPlan? last)
                    && (documentTitle == null || string.Equals(last.Document, documentTitle, StringComparison.OrdinalIgnoreCase)))
                {
                    return last;
                }
                return Plans.Values.Where(p => documentTitle == null || string.Equals(p.Document, documentTitle, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => p.UpdatedUtc, StringComparer.Ordinal).FirstOrDefault();
            }
        }

        public static bool Remove(string planId)
        {
            lock (Gate)
            {
                bool removed = Plans.Remove(planId);
                if (string.Equals(_lastPlanId, planId, StringComparison.OrdinalIgnoreCase))
                {
                    _lastPlanId = Plans.Values.OrderByDescending(p => p.UpdatedUtc, StringComparer.Ordinal).FirstOrDefault()?.PlanId;
                }
                return removed;
            }
        }

        public static IReadOnlyList<BatchPlan> All()
        {
            lock (Gate)
            {
                return Plans.Values.OrderByDescending(p => p.UpdatedUtc, StringComparer.Ordinal).ToList();
            }
        }

        public static int Count
        {
            get
            {
                lock (Gate) return Plans.Count;
            }
        }
    }
}
