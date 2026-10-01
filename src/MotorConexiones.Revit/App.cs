using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using MotorConexiones.Core;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Punto de entrada del add-in: crea la pestaña "Conexiones" con el botón "Ejecutar especificación JSON"
    /// y registra el arranque. El puente con el MCP es <see cref="Bridge"/>, que no depende de esta clase.
    /// </summary>
    public sealed class App : IExternalApplication
    {
        public const string TabName = "Conexiones";
        public const string PanelName = "MotorConexiones";

        public Result OnStartup(UIControlledApplication application)
        {
            // Las DLL del add-in (Core) se resuelven desde la carpeta del propio add-in.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromAddinFolder;

            try
            {
                try
                {
                    application.CreateRibbonTab(TabName);
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    // La pestaña ya existe (otro add-in o una recarga): se reutiliza.
                }

                RibbonPanel panel = application.CreateRibbonPanel(TabName, PanelName);
                var button = new PushButtonData(
                    "MotorConexiones_RunSpec",
                    "Ejecutar\nespecificación JSON",
                    Assembly.GetExecutingAssembly().Location,
                    typeof(RunSpecCommand).FullName)
                {
                    ToolTip = "Lee una especificación JSON de conexión, la valida y la crea en el modelo (disponible en la Fase 3).",
                    LongDescription = "MotorConexiones " + AddinInfo.Version + ". Este es el único botón del add-in; la IA usa el servidor MCP.",
                };
                panel.AddItem(button);

                JsonLineLogger.Write(new
                {
                    @event = "startup",
                    addin_version = AddinInfo.Version,
                    revit_version = application.ControlledApplication.VersionNumber,
                    revit_build = application.ControlledApplication.VersionBuild,
                    assembly = Assembly.GetExecutingAssembly().Location,
                });
                return Result.Succeeded;
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "startup_failed", error = error.ToString() });
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            AppDomain.CurrentDomain.AssemblyResolve -= ResolveFromAddinFolder;
            JsonLineLogger.Write(new { @event = "shutdown" });
            return Result.Succeeded;
        }

        private static Assembly? ResolveFromAddinFolder(object? sender, ResolveEventArgs args)
        {
            try
            {
                string folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                string name = new AssemblyName(args.Name).Name ?? "";
                string candidate = Path.Combine(folder, name + ".dll");
                return File.Exists(candidate) ? Assembly.LoadFrom(candidate) : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
