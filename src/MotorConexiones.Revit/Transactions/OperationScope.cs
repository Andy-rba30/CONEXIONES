using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit.Transactions
{
    /// <summary>
    /// Una operación = un <see cref="TransactionGroup"/> "MotorConexiones: &lt;operación&gt; &lt;id&gt;".
    /// Dentro se abren las <see cref="Transaction"/> necesarias con <see cref="StartTransaction"/>, que ya llevan
    /// <c>SetForcedModalHandling(false)</c> y un <see cref="IFailuresPreprocessor"/> que guarda las advertencias en
    /// <see cref="Warnings"/> y hace rollback ante errores. Mientras el ámbito está abierto, cualquier diálogo de
    /// Revit se cancela y se anota como <c>REVIT_DIALOG_SUPPRESSED</c>. Si no se llama a <see cref="Commit"/>,
    /// <see cref="Dispose"/> deshace el grupo completo (atómico).
    /// </summary>
    public sealed class OperationScope : IDisposable
    {
        private readonly TransactionGroup _group;
        private readonly UIApplication? _uiApplication;
        private bool _committed;
        private bool _disposed;

        public OperationScope(Document document, UIApplication? uiApplication, string operation, string connectionId, List<ApiError> warnings)
        {
            Warnings = warnings ?? throw new ArgumentNullException(nameof(warnings));
            _uiApplication = uiApplication;
            _group = new TransactionGroup(document, "MotorConexiones: " + operation + " " + connectionId);
            _group.Start();
            if (_uiApplication != null)
            {
                _uiApplication.DialogBoxShowing += OnDialogBoxShowing;
            }
        }

        public List<ApiError> Warnings { get; }

        /// <summary>
        /// Definición de la advertencia de Revit "Beam or Brace is slightly off axis and may cause inaccuracies." (cierre de la ronda
        /// 13b, fase-13.md 8.2): Revit la lanza sobre el <b>tramo nuevo</b> al partir una barra que ya se sale un poco de un plano
        /// vertical (la diagonal 1249631 del Hangar: 0,006 mm en Y en 3,5 m, el sondeo 30 v2); el add-in copia el eje exacto, así que
        /// no añade nada. Solo se filtra en la transacción del corte (<c>SplitService.Split</c>), nunca en las demás.
        /// </summary>
        public const string SlightlyOffAxisDefinition = "7cba70c0-2107-421e-9fe6-d2a2e05ead62";

        /// <summary>
        /// Abre una transacción sin ventanas dentro del grupo. El llamador la confirma con <c>Commit()</c>.
        /// <paramref name="informativeDefinitions"/>: Guids de definiciones de advertencias de Revit que, <b>solo en esta transacción</b>,
        /// se quedan en el log (<c>filtered: true</c>) sin salir como <c>REVIT_WARNING</c>; la de "solo visible en nivel Fino" se
        /// filtra siempre.
        /// </summary>
        public Transaction StartTransaction(Document document, string name, params string[] informativeDefinitions)
        {
            var transaction = new Transaction(document, name);
            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetForcedModalHandling(false);
            options.SetClearAfterRollback(true);
            options.SetFailuresPreprocessor(new FailureCollector(Warnings, informativeDefinitions));
            transaction.SetFailureHandlingOptions(options);
            transaction.Start();
            return transaction;
        }

        /// <summary>Confirma una transacción y comprueba que Revit no la haya deshecho por un error.</summary>
        public void CommitOrThrow(Transaction transaction)
        {
            TransactionStatus status = transaction.Commit();
            if (status != TransactionStatus.Committed)
            {
                throw new InvalidOperationException(
                    "Revit no confirmó la transacción '" + transaction.GetName() + "' (estado " + status + "). " +
                    "Mira las advertencias devueltas para ver el error de Revit.");
            }
        }

        /// <summary>Funde el grupo en una sola entrada de deshacer.</summary>
        public void Commit()
        {
            if (_group.HasStarted() && !_group.HasEnded())
            {
                _group.Assimilate();
            }
            _committed = true;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_uiApplication != null)
            {
                _uiApplication.DialogBoxShowing -= OnDialogBoxShowing;
            }
            try
            {
                if (!_committed && _group.HasStarted() && !_group.HasEnded())
                {
                    _group.RollBack();
                }
            }
            finally
            {
                _group.Dispose();
            }
        }

        private void OnDialogBoxShowing(object? sender, DialogBoxShowingEventArgs args)
        {
            string id = args.DialogId ?? "";
            string detail = "";
            if (args is TaskDialogShowingEventArgs taskDialog)
            {
                detail = taskDialog.Message ?? "";
            }
            else if (args is MessageBoxShowingEventArgs messageBox)
            {
                detail = messageBox.Message ?? "";
            }

            bool cancelled = TryOverride(args, (int)TaskDialogResult.Cancel) || TryOverride(args, (int)TaskDialogResult.Ok);
            Warnings.Add(new ApiError(
                ErrorCodes.RevitDialogSuppressed,
                "Revit intentó mostrar el diálogo '" + id + "'" + (detail.Length > 0 ? ": " + detail : "") +
                (cancelled ? " (cancelado automáticamente)." : " (no se pudo cancelar)."),
                hint: "Las rutas del MCP no muestran ventanas; revisa el mensaje y corrige la especificación si hace falta."));
        }

        private static bool TryOverride(DialogBoxShowingEventArgs args, int result)
        {
            try
            {
                return args.OverrideResult(result);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Guarda las advertencias de Revit y deshace si hay errores, sin diálogos.</summary>
        private sealed class FailureCollector : IFailuresPreprocessor
        {
            /// <summary>
            /// Definición de la advertencia de Revit "The created elements are only visible in Detail Level: Fine."
            /// (cierre de la ronda 12d, fase-12.md 10.5): la lanza por cada elemento de Advance Steel al confirmar (2 en un nudo en K,
            /// 8 en un empalme), es informativa (las soldaduras y las planchas se dibujan en nivel Fino) y ya está explicada en la
            /// guía y el README. Queda en el log como <c>revit_failure</c> con <c>filtered: true</c> y no sale como <c>REVIT_WARNING</c>.
            /// Se casa por el Guid de la definición, que no depende del idioma de Revit.
            /// </summary>
            private const string OnlyVisibleInFineDefinition = "73c984d7-da5a-4cd2-b83b-1c2c345d216b";

            private readonly List<ApiError> _warnings;
            private readonly HashSet<string> _informative;

            public FailureCollector(List<ApiError> warnings, IEnumerable<string>? informativeDefinitions = null)
            {
                _warnings = warnings;
                _informative = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { OnlyVisibleInFineDefinition };
                if (informativeDefinitions != null)
                {
                    foreach (string definition in informativeDefinitions)
                    {
                        if (!string.IsNullOrWhiteSpace(definition)) _informative.Add(definition);
                    }
                }
            }

            public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
            {
                bool hasErrors = false;
                foreach (FailureMessageAccessor failure in failuresAccessor.GetFailureMessages())
                {
                    string text;
                    try
                    {
                        text = failure.GetDescriptionText();
                    }
                    catch
                    {
                        text = "(sin descripción)";
                    }

                    FailureSeverity severity = failure.GetSeverity();
                    string elements = DescribeElements(failure);
                    string definition = DescribeDefinition(failure);
                    // Solo las advertencias informativas conocidas se filtran del aviso ("solo visible en nivel Fino" siempre; las que
                    // pida la transacción, como "slightly off axis" en el corte), y solo si son advertencias (nunca un error).
                    bool filtered = severity == FailureSeverity.Warning && _informative.Contains(definition);
                    // Ronda 12d (fase-12.md 9.2.b): la 12c devolvió un REVIT_WARNING por cada elemento de Advance Steel (2 en el nudo
                    // en K, 8 en el empalme) y ni el cuadro ni el log decían qué advertía Revit. Cada fallo queda en el log con su
                    // texto, su severidad y los elementos a los que apunta, también el filtrado (fase-12.md 10.3).
                    JsonLineLogger.Write(new
                    {
                        @event = "revit_failure",
                        severity = severity.ToString(),
                        text,
                        elements,
                        definition,
                        filtered,
                        transaction = DescribeTransaction(failuresAccessor),
                    });

                    if (filtered)
                    {
                        continue;
                    }

                    if (severity == FailureSeverity.Warning)
                    {
                        _warnings.Add(new ApiError(ErrorCodes.RevitWarning,
                            "Advertencia de Revit: " + text + (elements.Length > 0 ? " [elementos: " + elements + "]" : "")));
                    }
                    else
                    {
                        hasErrors = true;
                        _warnings.Add(new ApiError(ErrorCodes.RevitError, "Error de Revit: " + text + (elements.Length > 0 ? " [elementos: " + elements + "]" : ""),
                            hint: "La operación se deshizo completa."));
                    }
                }

                failuresAccessor.DeleteAllWarnings();
                return hasErrors ? FailureProcessingResult.ProceedWithRollBack : FailureProcessingResult.Continue;
            }

            /// <summary>Los ids de los elementos a los que apunta el fallo, separados por comas (vacío si no hay o no se pueden leer).</summary>
            private static string DescribeElements(FailureMessageAccessor failure)
            {
                try
                {
                    var ids = new List<string>();
                    foreach (ElementId id in failure.GetFailingElementIds())
                    {
                        ids.Add(id.Value.ToString());
                    }
                    return string.Join(", ", ids);
                }
                catch
                {
                    return "";
                }
            }

            private static string DescribeDefinition(FailureMessageAccessor failure)
            {
                try
                {
                    return failure.GetFailureDefinitionId().Guid.ToString();
                }
                catch
                {
                    return "";
                }
            }

            private static string DescribeTransaction(FailuresAccessor failuresAccessor)
            {
                try
                {
                    return failuresAccessor.GetTransactionName();
                }
                catch
                {
                    return "";
                }
            }
        }
    }
}
