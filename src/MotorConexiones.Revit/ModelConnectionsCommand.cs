using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Transactions;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Conexiones del modelo" de la cinta (Fase 6): abre <see cref="ConnectionsWindow"/> con la lista de
    /// conexiones del add-in y borra la que la persona elija, con confirmación, usando el mismo servicio y el mismo
    /// <see cref="OperationScope"/> que <c>conn_delete</c> (una operación = un TransactionGroup; error = rollback).
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class ModelConnectionsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication app = commandData.Application;
            Document? doc = app.ActiveUIDocument?.Document;
            if (doc == null)
            {
                TaskDialog.Show("MotorConexiones", "Abre un proyecto de Revit para ver sus conexiones.");
                return Result.Cancelled;
            }

            try
            {
                var window = new ConnectionsWindow(doc, id => Delete(doc, app, id), app.MainWindowHandle);
                window.ShowDialog();
                JsonLineLogger.Write(new { @event = "connections_window_closed", deleted = window.DeletedCount });
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "connections_window_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error",
                    "No se pudo abrir la ventana de conexiones:\n" + ex.Message +
                    "\n\nEl detalle está en el registro (%LOCALAPPDATA%\\MotorConexiones\\log).");
                return Result.Failed;
            }
        }

        /// <summary>Borra una conexión igual que <c>conn_delete</c>; devuelve el resultado en español para la ventana.</summary>
        private static DeleteOutcome Delete(Document doc, UIApplication app, string connectionId)
        {
            if (doc.IsReadOnly)
            {
                return new DeleteOutcome(false, "El documento es de solo lectura: no se puede borrar.");
            }
            if (doc.IsModifiable)
            {
                return new DeleteOutcome(false, "Revit tiene una transacción abierta (REVIT_BUSY): cierra la operación en curso y repite.");
            }

            var warnings = new List<ApiError>();
            ConnectionRecord? deleted = null;
            try
            {
                using (var scope = new OperationScope(doc, app, "delete_ribbon", connectionId, warnings))
                {
                    using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Borrar " + connectionId))
                    {
                        bool found = ConnectionCreationService.DeleteConnection(doc, connectionId, warnings, out deleted);
                        if (!found)
                        {
                            return new DeleteOutcome(false, "No existe la conexión " + connectionId + " (quizá ya se borró).");
                        }
                        scope.CommitOrThrow(tx);
                    }
                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "delete_ribbon_failed", connection_id = connectionId, error = ex.ToString(), warnings = warnings.Select(w => w.Code).ToList() });
                return new DeleteOutcome(false, "No se pudo borrar: " + ex.Message + " Se deshizo todo (rollback completo).");
            }

            int deletedElements = deleted?.CreatedElementIds?.Count ?? 0;
            int restored = deleted?.ModifiedMembers?.Count ?? 0;
            JsonLineLogger.Write(new
            {
                @event = "delete_ribbon",
                connection_id = connectionId,
                deleted_elements = deletedElements,
                restored_members = restored,
                warnings = warnings.Select(w => w.Code).ToList(),
            });
            string warningText = warnings.Count == 0 ? "" : " Avisos: " + string.Join("; ", warnings.Select(w => w.Code).Distinct()) + ".";
            return new DeleteOutcome(true,
                "Borrada " + connectionId + ": " + deletedElements + " elementos eliminados y " + restored + " barras restauradas." + warningText,
                deletedElements, restored);
        }
    }
}
