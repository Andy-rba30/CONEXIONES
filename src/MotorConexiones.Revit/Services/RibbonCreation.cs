using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Transactions;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit.Services
{
    /// <summary>
    /// Crear desde la cinta (lo que hacía el botón "Ejecutar especificación JSON" desde la Fase 3, compartido en la
    /// Fase 7 con el botón Catálogo): una operación atómica con el token recién calculado por la ventana, adopción de
    /// los elementos de Advance Steel, registro y diálogo final con el <c>connection_id</c>. Solo en el camino de los
    /// botones: las rutas <c>conn_*</c> no pasan por aquí.
    /// </summary>
    internal static class RibbonCreation
    {
        public static Result CreateFromSession(UIApplication uiApplication, Document doc, UIDocument? uidoc, PreviewSession session, string operationName)
        {
            if (!session.CanCreate || session.Spec == null)
            {
                TaskDialog.Show("MotorConexiones", "La especificación dejó de ser válida: vuelve a abrir la ventana y valida de nuevo.");
                return Result.Cancelled;
            }

            ConnectionSpec spec = session.Spec;
            string rawJson = session.RawJson;
            string token = session.Validation?.ValidationToken ?? string.Empty;
            var warnings = new List<ApiError>();
            string opId = Guid.NewGuid().ToString("D");
            ConnectionRecord createdRecord;
            var snapshot = ConnectionCreationService.Snapshot(doc);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                using (var scope = new OperationScope(doc, uiApplication, operationName, opId, warnings))
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
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_create_failed",
                    operation = operationName,
                    file = session.FilePath,
                    template_id = spec.Source?.TemplateId,
                    error = ex.ToString(),
                    warnings = warnings.Select(w => w.Code).ToList(),
                    duration_ms = stopwatch.ElapsedMilliseconds,
                });
                TaskDialog.Show("MotorConexiones - Error en Modelado",
                    "Ocurrió un error al crear la geometría de la conexión:\n\n" + ex.Message +
                    "\n\nSe ha realizado un rollback completo.");
                return Result.Failed;
            }

            JsonLineLogger.Write(new
            {
                @event = "ribbon_create",
                operation = operationName,
                file = session.FilePath,
                template_id = spec.Source?.TemplateId,
                connection_id = createdRecord.ConnectionId,
                created_elements = createdRecord.CreatedElementIds.Count,
                modified_members = createdRecord.ModifiedMembers.Count,
                backend = createdRecord.BackendName,
                validation_token_prefix = token.Length >= 16 ? token.Substring(0, 16) : token,
                warnings = warnings.Select(w => w.Code).ToList(),
                duration_ms = stopwatch.ElapsedMilliseconds,
            });

            string warningText = warnings.Count == 0 ? "" : "\nAdvertencias: " + string.Join("; ", warnings.Select(w => w.Code).Distinct());
            string templateText = string.IsNullOrEmpty(spec.Source?.TemplateId) ? "" : $"Plantilla del catálogo: {spec.Source!.TemplateId}\n";
            var successDialog = new TaskDialog("MotorConexiones - Éxito")
            {
                MainInstruction = "Conexión modelada correctamente en el modelo.",
                MainContent =
                    $"ID de conexión: {createdRecord.ConnectionId}\n" +
                    templateText +
                    $"Elementos geométricos creados: {createdRecord.CreatedElementIds.Count}\n" +
                    $"Miembros modificados (retiros): {createdRecord.ModifiedMembers.Count}\n" +
                    $"Backend de fabricación utilizado: {createdRecord.BackendName}{warningText}\n" +
                    "La conexión ha quedado registrada en Extensible Storage. Se puede ver y borrar con el botón \"Conexiones del modelo\" o con las herramientas conn_* del MCP.",
                CommonButtons = TaskDialogCommonButtons.Close
            };
            successDialog.Show();

            return Result.Succeeded;
        }
    }
}
