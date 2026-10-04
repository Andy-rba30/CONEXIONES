using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using MotorConexiones.Core;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Punto de entrada del add-in. Desde la Fase 6 los dos botones ("Ejecutar especificación JSON" y "Conexiones del
    /// modelo") van en el panel "Conexiones" de la pestaña "ARBA", que crean los add-ins de C# de la persona
    /// (respuesta del instalador, 2026-10-04): <c>CreateRibbonTab("ARBA")</c> dentro de try/catch (si ya existe, se
    /// reutiliza), se busca el panel con <c>GetRibbonPanels("ARBA")</c> y se crea si falta. Si cualquier paso falla,
    /// reserva a la pestaña "Conexiones" de antes y lo anota en el log. No se tocan los paneles IA, Acero, Metrados ni
    /// Encofrado. El puente con el MCP es <see cref="Bridge"/>, que no depende de esta clase.
    /// </summary>
    public sealed class App : IExternalApplication
    {
        /// <summary>Pestaña de la persona donde viven sus add-ins.</summary>
        public const string ArbaTabName = "ARBA";

        /// <summary>Panel propio dentro de ARBA.</summary>
        public const string ArbaPanelName = "Conexiones";

        /// <summary>Reserva si ARBA falla (lo que había hasta la Fase 5).</summary>
        public const string TabName = "Conexiones";
        public const string PanelName = "MotorConexiones";

        public Result OnStartup(UIControlledApplication application)
        {
            // Las DLL del add-in (Core) se resuelven desde la carpeta del propio add-in.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromAddinFolder;

            try
            {
                RibbonPanel panel = CreatePanel(application, out string tab, out string panelName, out string? arbaError);
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                var runSpec = new PushButtonData(
                    "MotorConexiones_RunSpec",
                    "Ejecutar\nespecificación JSON",
                    assemblyPath,
                    typeof(RunSpecCommand).FullName)
                {
                    ToolTip = "Lee una especificación JSON de conexión, la dibuja con cotas, deja corregir valores, valida y crea la conexión en el modelo.",
                    LongDescription = "MotorConexiones " + AddinInfo.Version + ". Ventana de previsualización 2D (Fase 6). La IA usa el servidor MCP, sin ventanas.",
                };
                var modelConnections = new PushButtonData(
                    "MotorConexiones_ModelConnections",
                    "Conexiones\ndel modelo",
                    assemblyPath,
                    typeof(ModelConnectionsCommand).FullName)
                {
                    ToolTip = "Lista las conexiones creadas por MotorConexiones en el documento y permite borrar una (restaura las barras).",
                    LongDescription = "Lo mismo que conn_list y conn_delete, sin la IA.",
                };
                SetIcons(runSpec, RibbonIcons.RunSpec);
                SetIcons(modelConnections, RibbonIcons.ModelConnections);

                var items = new List<RibbonItem?> { panel.AddItem(runSpec), panel.AddItem(modelConnections) };
                TrySetVisible(panel);

                JsonLineLogger.Write(new
                {
                    @event = "startup",
                    addin_version = AddinInfo.Version,
                    revit_version = application.ControlledApplication.VersionNumber,
                    revit_build = application.ControlledApplication.VersionBuild,
                    assembly = assemblyPath,
                    ribbon_tab = tab,
                    ribbon_panel = panelName,
                    ribbon_buttons = items.Count,
                    arba_error = arbaError,
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

        /// <summary>
        /// Panel "Conexiones" de la pestaña "ARBA"; si no se puede, panel "MotorConexiones" de la pestaña "Conexiones".
        /// El orden de carga de los add-ins no importa: si ARBA ya existe, CreateRibbonTab lanza y se ignora; si todavía
        /// no existe, la creamos nosotros y los demás add-ins harán lo mismo al cargar (así lo hacen ellos entre sí).
        /// </summary>
        private static RibbonPanel CreatePanel(UIControlledApplication application, out string tab, out string panelName, out string? arbaError)
        {
            arbaError = null;
            try
            {
                try
                {
                    application.CreateRibbonTab(ArbaTabName);
                }
                catch (Exception)
                {
                    // Ya creada por otro add-in de ARBA (o por una recarga): se reutiliza.
                }

                RibbonPanel? existing = FindPanel(application, ArbaTabName, ArbaPanelName);
                RibbonPanel panel = existing ?? application.CreateRibbonPanel(ArbaTabName, ArbaPanelName);
                tab = ArbaTabName;
                panelName = ArbaPanelName;
                JsonLineLogger.Write(new { @event = "ribbon_arba", tab, panel = panelName, reused_panel = existing != null });
                return panel;
            }
            catch (Exception error)
            {
                arbaError = error.ToString();
                JsonLineLogger.Write(new { @event = "ribbon_arba_failed", error = error.ToString(), fallback_tab = TabName });
            }

            try
            {
                application.CreateRibbonTab(TabName);
            }
            catch (Exception)
            {
                // La pestaña de reserva ya existe.
            }
            tab = TabName;
            panelName = PanelName;
            return FindPanel(application, TabName, PanelName) ?? application.CreateRibbonPanel(TabName, PanelName);
        }

        private static RibbonPanel? FindPanel(UIControlledApplication application, string tabName, string panelName)
        {
            try
            {
                IList<RibbonPanel> panels = application.GetRibbonPanels(tabName);
                if (panels == null) return null;
                foreach (RibbonPanel panel in panels)
                {
                    if (string.Equals(panel.Name, panelName, StringComparison.OrdinalIgnoreCase)) return panel;
                }
            }
            catch (Exception)
            {
                // La pestaña no existe todavía o Revit no la expone: se crea el panel.
            }
            return null;
        }

        private static void SetIcons(ButtonData button, Func<int, System.Windows.Media.ImageSource?> draw)
        {
            try
            {
                System.Windows.Media.ImageSource? large = draw(32);
                System.Windows.Media.ImageSource? small = draw(16);
                if (large != null) button.LargeImage = large;
                if (small != null) button.Image = small;
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "ribbon_icon_failed", button = button.Name, error = error.Message });
            }
        }

        private static void TrySetVisible(RibbonPanel panel)
        {
            try
            {
                if (!panel.Visible) panel.Visible = true;
            }
            catch (Exception)
            {
                // Algunas versiones no permiten cambiarlo durante OnStartup; el panel nuevo ya es visible por defecto.
            }
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
