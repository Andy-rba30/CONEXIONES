using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Ejecutar especificación JSON". Es el único sitio del add-in donde se permite una ventana.
    /// En la Fase 1 solo confirma que el add-in está cargado; la lectura del JSON llega en la Fase 3.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class RunSpecCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application.Application;
            var dialog = new TaskDialog("MotorConexiones")
            {
                MainInstruction = "MotorConexiones " + AddinInfo.Version + " está cargado.",
                MainContent =
                    "Revit " + app.VersionNumber + " (" + app.VersionBuild + ").\n" +
                    "La ejecución de especificaciones JSON desde este botón llega en la Fase 3.\n" +
                    "Mientras tanto, la IA usa las herramientas conn_* del servidor MCP.",
                CommonButtons = TaskDialogCommonButtons.Close,
            };
            dialog.Show();
            return Result.Succeeded;
        }
    }
}
