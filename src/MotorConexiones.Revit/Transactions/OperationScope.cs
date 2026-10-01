using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Validation;

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

        /// <summary>Abre una transacción sin ventanas dentro del grupo. El llamador la confirma con <c>Commit()</c>.</summary>
        public Transaction StartTransaction(Document document, string name)
        {
            var transaction = new Transaction(document, name);
            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetForcedModalHandling(false);
            options.SetClearAfterRollback(true);
            options.SetFailuresPreprocessor(new FailureCollector(Warnings));
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
            private readonly List<ApiError> _warnings;

            public FailureCollector(List<ApiError> warnings)
            {
                _warnings = warnings;
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

                    if (failure.GetSeverity() == FailureSeverity.Warning)
                    {
                        _warnings.Add(new ApiError(ErrorCodes.RevitWarning, "Advertencia de Revit: " + text));
                    }
                    else
                    {
                        hasErrors = true;
                        _warnings.Add(new ApiError(ErrorCodes.RevitError, "Error de Revit: " + text,
                            hint: "La operación se deshizo completa."));
                    }
                }

                failuresAccessor.DeleteAllWarnings();
                return hasErrors ? FailureProcessingResult.ProceedWithRollBack : FailureProcessingResult.Continue;
            }
        }
    }
}
