using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Win32;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Transactions;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Ejecutar especificación JSON" de la cinta. Es uno de los dos sitios del add-in con ventanas (el otro es
    /// <see cref="ModelConnectionsCommand"/>); las rutas que usa la IA (<see cref="Bridge"/>) no muestran ninguna.
    /// Flujo (Fase 6): elegir el archivo JSON → ventana de previsualización con croquis 2D, tabla editable y
    /// validación → si la persona pulsa Crear con la validación en verde, crea la conexión con
    /// <see cref="ConnectionCreationService"/> en una operación atómica y muestra el <c>connection_id</c>.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class RunSpecCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument? uidoc = commandData.Application.ActiveUIDocument;
            Document? doc = uidoc?.Document;

            if (doc == null)
            {
                TaskDialog.Show("MotorConexiones", "Abre un proyecto de Revit con la cercha antes de ejecutar una especificación.");
                return Result.Cancelled;
            }

            if (doc.IsReadOnly)
            {
                TaskDialog.Show("MotorConexiones", "El documento activo es de solo lectura. Ábrelo con permiso de modificación.");
                return Result.Cancelled;
            }

            // 1. Archivo JSON.
            string? filePath = ShowOpenFileDialog();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return Result.Cancelled;
            }

            string rawJson;
            try
            {
                rawJson = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MotorConexiones - Error", "No se pudo leer el archivo seleccionado: " + ex.Message);
                return Result.Failed;
            }

            ConnectionSpec? spec;
            try
            {
                spec = ConnectionSpec.FromJson(rawJson);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MotorConexiones - Error JSON", "El archivo no contiene un JSON válido:\n" + ex.Message);
                return Result.Failed;
            }

            if (spec == null)
            {
                TaskDialog.Show("MotorConexiones - Error", "El archivo está vacío o no es una especificación de conexión.");
                return Result.Failed;
            }

            // 2. Si la especificación no trae IDs, se toman de la selección actual de Revit (como hasta ahora).
            var selectedIds = uidoc?.Selection?.GetElementIds()?.Select(id => id.Value).ToList() ?? new List<long>();
            if ((spec.Node?.ElementIds == null || spec.Node.ElementIds.Count == 0) && selectedIds.Count >= 2)
            {
                spec.Node = new NodeRef { ElementIds = selectedIds };
                if (spec.Chord == null || spec.Chord.ElementId <= 0)
                {
                    spec.Chord = new ChordSpec { ElementId = selectedIds[0], Continuous = true };
                }
                rawJson = spec.ToJson();
            }

            // 3. Ventana de previsualización (croquis + tabla + validación). Modal, en el hilo de Revit, sin ExternalEvent.
            LimitsConfig limits = LimitsConfigLoader.Load();
            var session = new PreviewSession(doc, filePath, spec, rawJson, limits);
            JsonLineLogger.Write(new { @event = "preview_window_opened", file = filePath, selected_ids = selectedIds.Count });

            bool? accepted;
            try
            {
                var window = new PreviewWindow(session, commandData.Application.MainWindowHandle);
                accepted = window.ShowDialog();
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "preview_window_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error",
                    "No se pudo abrir la ventana de previsualización:\n" + ex.Message +
                    "\n\nEl detalle está en el registro (%LOCALAPPDATA%\\MotorConexiones\\log). No se ha tocado el modelo.");
                return Result.Failed;
            }

            if (accepted != true)
            {
                JsonLineLogger.Write(new { @event = "preview_window_cancelled", file = filePath });
                return Result.Cancelled;
            }

            if (!session.CanCreate || session.LastValidation == null)
            {
                TaskDialog.Show("MotorConexiones", "La validación no está en verde: no se crea nada.");
                return Result.Cancelled;
            }

            spec = session.Spec;
            rawJson = session.RawJson;
            string token = session.LastValidation.ValidationToken ?? "";
            JsonLineLogger.Write(new { @event = "preview_window_accepted", file = filePath, token_prefix = token.Length >= 12 ? token.Substring(0, 12) : token });

            // 4. Creación atómica dentro de OperationScope: exactamente lo que hacía el botón antes de la Fase 6.
            var warnings = new List<ApiError>();
            string opId = Guid.NewGuid().ToString("D");
            ConnectionRecord createdRecord;
            var snapshot = ConnectionCreationService.Snapshot(doc);

            try
            {
                using (var scope = new OperationScope(doc, commandData.Application, "run_spec_ribbon", opId, warnings))
                {
                    using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Crear " + (spec.Source?.Drawing ?? "Conexión")))
                    {
                        createdRecord = ConnectionCreationService.CreateConnection(doc, uidoc, spec, rawJson, opId, warnings);
                        scope.CommitOrThrow(tx);
                    }
                    using (Transaction adopt = scope.StartTransaction(doc, "MotorConexiones: registrar elementos"))
                    {
                        ConnectionCreationService.AdoptNewElements(doc, createdRecord, snapshot, warnings);
                        scope.CommitOrThrow(adopt);
                    }
                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "run_spec_ribbon_failed", error = ex.ToString(), warnings = warnings.Select(w => w.Code).ToList() });
                TaskDialog.Show("MotorConexiones - Error en Modelado",
                    "Ocurrió un error al crear la geometría de la conexión:\n\n" + ex.Message +
                    "\n\nSe ha realizado un rollback completo." +
                    (warnings.Count > 0 ? "\nAvisos de Revit: " + string.Join("; ", warnings.Select(w => w.Message)) : ""));
                return Result.Failed;
            }

            JsonLineLogger.Write(new
            {
                @event = "run_spec_ribbon_created",
                connection_id = createdRecord.ConnectionId,
                created_elements = createdRecord.CreatedElementIds.Count,
                modified_members = createdRecord.ModifiedMembers.Count,
                backend = createdRecord.BackendName,
                warnings = warnings.Select(w => w.Code).ToList(),
            });

            // 5. Diálogo final con el connection_id.
            string warningText = warnings.Count == 0 ? "" : "\nAvisos: " + string.Join("; ", warnings.Select(w => w.Code).Distinct());
            var successDialog = new TaskDialog("MotorConexiones - Conexión creada")
            {
                MainInstruction = "Conexión modelada correctamente.",
                MainContent =
                    $"ID de conexión: {createdRecord.ConnectionId}\n" +
                    PreviewWindow.Summary(spec) + "\n" +
                    $"Elementos creados: {createdRecord.CreatedElementIds.Count}\n" +
                    $"Barras retiradas: {createdRecord.ModifiedMembers.Count}\n" +
                    $"Backend: {createdRecord.BackendName}{warningText}\n\n" +
                    "La conexión queda registrada en el modelo. Puedes borrarla con el botón \"Conexiones del modelo\" o con conn_delete desde la IA.",
                CommonButtons = TaskDialogCommonButtons.Close
            };
            successDialog.Show();

            return Result.Succeeded;
        }

        private static string? ShowOpenFileDialog()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Title = "Seleccionar especificación JSON de conexión",
                    Filter = "Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*",
                    Multiselect = false,
                    CheckFileExists = true,
                };
                string defaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Directory.Exists(defaultDir))
                {
                    dialog.InitialDirectory = defaultDir;
                    dialog.RestoreDirectory = true;
                }
                return dialog.ShowDialog() == true ? dialog.FileName : null;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "open_file_dialog_failed", error = ex.ToString() });
                return null;
            }
        }
    }
}
