using System;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Catálogo" de la cinta (Fase 7): lista las plantillas, aplica una a la selección (abre la ventana de
    /// previsualización con la especificación instanciada y, si la persona pulsa Crear, crea), guarda una plantilla desde
    /// una conexión del modelo y borra plantillas. Camino de los botones: aquí sí hay ventanas; las rutas conn_* no cambian.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class CatalogCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument? uidoc = commandData.Application.ActiveUIDocument;
            Document? doc = uidoc?.Document;
            if (doc == null)
            {
                TaskDialog.Show("MotorConexiones", "Abre un proyecto de Revit para usar el catálogo de conexiones.");
                return Result.Cancelled;
            }

            CatalogWindow window;
            try
            {
                window = new CatalogWindow(doc, commandData.Application, pickOnly: false);
                _ = new WindowInteropHelper(window) { Owner = commandData.Application.MainWindowHandle };
                window.ShowDialog();
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_catalog_window",
                    document = doc.Title,
                    applied_template_id = window.PendingTemplateId,
                    create_requested = window.PendingSession != null,
                });
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_catalog_window_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo abrir la ventana del catálogo:\n" + ex.Message);
                return Result.Failed;
            }

            if (window.PendingSession == null)
            {
                return Result.Succeeded;
            }

            // Crear con las ventanas ya cerradas, en el comando: una operación atómica, igual que el botón de la Fase 6.
            return RibbonCreation.CreateFromSession(commandData.Application, doc, uidoc, window.PendingSession, "catalog_ribbon");
        }
    }
}
