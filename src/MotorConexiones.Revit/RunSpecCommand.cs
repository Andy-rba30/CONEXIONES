using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Transactions;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Ejecutar especificación JSON" de la cinta. Es el camino del add-in donde se permiten ventanas (las rutas
    /// conn_* del MCP no las tienen). Elige el archivo JSON, abre la ventana de previsualización (croquis con cotas,
    /// tabla editable, validación) y, si la persona pulsa Crear con la validación en verde, crea la conexión con el
    /// token recién calculado: <see cref="RibbonCreation"/> (una operación atómica, registro en Extensible Storage y
    /// diálogo final con el <c>connection_id</c>), el mismo código que usa el botón Catálogo desde la Fase 7.
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
                TaskDialog.Show("MotorConexiones", "Por favor, abre un proyecto de Revit con una cercha o pórtico estructural.");
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

            // 2. Si el JSON no trae los IDs del nudo y hay barras seleccionadas, se usan (sin tocar el resto del texto).
            ConnectionSpec? spec = null;
            try
            {
                spec = ConnectionSpec.FromJson(rawJson);
            }
            catch (Exception)
            {
                // La ventana mostrará el error de lectura y permitirá Recargar tras corregir el archivo.
            }

            var selectedIds = uidoc?.Selection?.GetElementIds()?.Select(id => id.Value).OrderBy(id => id).ToList() ?? new List<long>();
            if (spec != null && (spec.Node?.ElementIds == null || spec.Node.ElementIds.Count == 0) && selectedIds.Count >= 2)
            {
                if (SpecEditor.TrySetElementIds(rawJson, selectedIds, out string withIds, out string idsError))
                {
                    rawJson = withIds;
                }
                else
                {
                    JsonLineLogger.Write(new { @event = "ribbon_selection_ignored", error = idsError });
                }
            }

            // 3. Ventana de previsualización (modal sobre Revit, dentro del comando: sin ExternalEvent).
            PreviewSession session;
            bool createRequested;
            try
            {
                session = new PreviewSession(doc, uidoc, filePath!, rawJson);
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_preview_opened",
                    file = filePath,
                    is_valid = session.CanCreate,
                    errors = session.Validation?.Errors.Select(e => e.Code).ToList(),
                    sketch_pieces = session.Sketch?.PieceCount ?? 0,
                });

                var window = new PreviewWindow(session);
                _ = new WindowInteropHelper(window) { Owner = commandData.Application.MainWindowHandle };
                window.ShowDialog();
                createRequested = window.CreateRequested;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_preview_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo abrir la ventana de previsualización:\n" + ex.Message);
                return Result.Failed;
            }

            if (!createRequested)
            {
                return Result.Cancelled;
            }

            // 4. Crear con el JSON y el token tal como quedaron en la ventana (la ventana ya volvió a validar al pulsar Crear).
            // Desde la Fase 7 el código de crear es compartido con el botón Catálogo (RibbonCreation).
            return RibbonCreation.CreateFromSession(commandData.Application, doc, uidoc, session, "run_spec_ribbon");
        }

        private static string? ShowOpenFileDialog()
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
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
