using System;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Conexiones del modelo": abre la ventana que lista las conexiones del add-in y permite borrarlas con
    /// confirmación. Camino del botón de la cinta: aquí sí hay ventanas; las rutas conn_* no cambian.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class ListConnectionsCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document? doc = commandData.Application.ActiveUIDocument?.Document;
            if (doc == null)
            {
                TaskDialog.Show("MotorConexiones", "Abre un proyecto de Revit para ver sus conexiones.");
                return Result.Cancelled;
            }

            try
            {
                var window = new ConnectionsWindow(doc, commandData.Application);
                _ = new WindowInteropHelper(window) { Owner = commandData.Application.MainWindowHandle };
                window.ShowDialog();
                JsonLineLogger.Write(new { @event = "ribbon_connections_window", document = doc.Title, deleted = window.DeletedCount });
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connections_window_failed", error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo abrir la ventana de conexiones:\n" + ex.Message);
                return Result.Failed;
            }
        }
    }
}
