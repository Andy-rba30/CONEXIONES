using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.Batch
{
    /// <summary>
    /// Puente entre la ventana <b>no modal</b> del plan y la API de Revit (cierre de la ronda 8c, mejora C7 de la propuesta).
    /// Una ventana que se queda abierta vive fuera de los comandos de la cinta, y fuera de un comando Revit no admite
    /// llamadas a su API (ni leer el modelo, ni transacciones, ni pinchar). Por eso la ventana no toca nada por su cuenta:
    /// encola aquí el trabajo (<see cref="Run"/>) y levanta el <see cref="ExternalEvent"/>; Revit lo ejecuta en su propio
    /// hilo, en contexto válido, en cuanto queda libre (nada más soltar el ratón si la persona estaba orbitando). Por aquí
    /// pasa todo lo que replanifica, marca, descarta, encuadra o pide pinchar en el modelo; la ventana solo pinta.
    /// El evento se crea una sola vez, dentro del comando de la cinta (<see cref="EnsureCreated"/>), porque
    /// <c>ExternalEvent.Create</c> solo funciona en contexto válido de la API.
    /// </summary>
    public static class PlanEvents
    {
        private static readonly object Gate = new object();
        private static readonly Queue<Action<UIApplication>> Pending = new Queue<Action<UIApplication>>();
        private static ExternalEvent? _event;

        /// <summary>Verdadero cuando el evento ya existe (se creó en un comando de la cinta).</summary>
        public static bool IsCreated
        {
            get
            {
                lock (Gate) return _event != null;
            }
        }

        /// <summary>Crea el evento si no existe. Solo desde un comando de la cinta (contexto válido de la API).</summary>
        public static void EnsureCreated()
        {
            lock (Gate)
            {
                if (_event != null) return;
                _event = ExternalEvent.Create(new Handler());
            }
        }

        /// <summary>
        /// Encola el trabajo y pide a Revit que lo ejecute. Devuelve falso, con el motivo en español, si el evento no existe
        /// o Revit no acepta peticiones ahora (por ejemplo, con un cuadro de diálogo suyo abierto); en ese caso el trabajo
        /// se quita de la cola y no se ejecutará.
        /// </summary>
        public static bool Run(Action<UIApplication> work, out string? reason)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));
            ExternalEvent? external;
            lock (Gate)
            {
                external = _event;
                if (external != null) Pending.Enqueue(work);
            }
            if (external == null)
            {
                reason = "La ventana del plan no está conectada con Revit: ciérrala y vuelve a pulsar Planificar lote.";
                return false;
            }

            ExternalEventRequest request;
            try
            {
                request = external.Raise();
            }
            catch (Exception ex)
            {
                Forget(work);
                reason = "Revit no aceptó la petición: " + ex.Message;
                return false;
            }
            switch (request)
            {
                case ExternalEventRequest.Accepted:
                case ExternalEventRequest.Pending:
                    reason = null;
                    return true;
                case ExternalEventRequest.Denied:
                    Forget(work);
                    reason = "Revit no acepta peticiones ahora (¿hay un cuadro de diálogo o una herramienta de Revit en marcha?). Termina eso y vuelve a intentarlo.";
                    return false;
                default:
                    Forget(work);
                    reason = "Revit tardó demasiado en aceptar la petición (" + request + "). Vuelve a intentarlo.";
                    return false;
            }
        }

        private static void Forget(Action<UIApplication> work)
        {
            lock (Gate)
            {
                if (Pending.Count == 0) return;
                var kept = new List<Action<UIApplication>>();
                while (Pending.Count > 0)
                {
                    Action<UIApplication> item = Pending.Dequeue();
                    if (!ReferenceEquals(item, work)) kept.Add(item);
                }
                foreach (Action<UIApplication> item in kept) Pending.Enqueue(item);
            }
        }

        /// <summary>Vacía la cola en contexto válido. Un trabajo que falle no impide los siguientes; el fallo queda en el log.</summary>
        private sealed class Handler : IExternalEventHandler
        {
            public void Execute(UIApplication app)
            {
                while (true)
                {
                    Action<UIApplication>? work;
                    lock (Gate)
                    {
                        if (Pending.Count == 0) return;
                        work = Pending.Dequeue();
                    }
                    try
                    {
                        work(app);
                    }
                    catch (Exception ex)
                    {
                        JsonLineLogger.Write(new { @event = "plan_event_failed", error = ex.ToString() });
                    }
                }
            }

            public string GetName() => "MotorConexiones: ventana del plan de lote";
        }
    }
}
