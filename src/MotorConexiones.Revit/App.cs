using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using MotorConexiones.Core;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Logging;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Punto de entrada del add-in: pone el panel "MotorConexiones" en la pestaña <b>ARBA</b> de la persona (la misma
    /// que crean sus otros add-ins de C# con <c>CreateRibbonTab("ARBA")</c>, según la respuesta del instalador en la
    /// Fase 6) con los botones "Ejecutar especificación JSON", "Conexiones del modelo", "Catálogo" (Fase 7), "Planificar lote"
    /// (Fase 8) y "Encargo para IA" (ronda 9b, Fase 10). Si ARBA no se puede usar, el panel va a la pestaña de reserva
    /// "Conexiones". En ambos casos queda anotado en el registro. Fase 10: registra también el manejador de clics de las
    /// etiquetas del lienzo (<see cref="PlanLabels.EnsureHandlerRegistered"/>). El puente con el MCP es <see cref="Bridge"/>,
    /// que no depende de esta clase.
    /// </summary>
    public sealed class App : IExternalApplication
    {
        /// <summary>Pestaña preferida: la de la persona. La crea el primer add-in que arranca; los demás la reutilizan.</summary>
        public const string PreferredTabName = "ARBA";

        /// <summary>Pestaña de reserva si ARBA no se puede crear ni reutilizar.</summary>
        public const string FallbackTabName = "Conexiones";

        /// <summary>Nombre histórico (Fases 1 a 5) que otros scripts citan; ahora es la reserva.</summary>
        public const string TabName = FallbackTabName;

        public const string PanelName = "MotorConexiones";

        /// <summary>Pestaña en la que quedó el panel al arrancar (ARBA o Conexiones); vacío si falló.</summary>
        public static string ActiveTabName { get; private set; } = string.Empty;

        public Result OnStartup(UIControlledApplication application)
        {
            // Las DLL del add-in (Core) se resuelven desde la carpeta del propio add-in.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromAddinFolder;

            try
            {
                RibbonPanel panel = CreatePanel(application);
                string assemblyPath = Assembly.GetExecutingAssembly().Location;

                var runSpec = new PushButtonData(
                    "MotorConexiones_RunSpec",
                    "Ejecutar\nespecificación JSON",
                    assemblyPath,
                    typeof(RunSpecCommand).FullName)
                {
                    ToolTip = "Lee una especificación JSON de conexión, la dibuja con cotas, deja corregir valores, valida y crea en el modelo.",
                    LongDescription = "MotorConexiones " + AddinInfo.Version + ". Abre la ventana de previsualización: croquis 2D a la izquierda, tabla editable a la derecha, errores y botones abajo. La IA usa el servidor MCP, no este botón.",
                };
                panel.AddItem(runSpec);

                var listConnections = new PushButtonData(
                    "MotorConexiones_ListConnections",
                    "Conexiones\ndel modelo",
                    assemblyPath,
                    typeof(ListConnectionsCommand).FullName)
                {
                    ToolTip = "Lista las conexiones creadas por MotorConexiones en el documento y permite borrar una (como conn_list y conn_delete).",
                    LongDescription = "Borrar quita solo lo que creó el add-in y devuelve a las barras sus extensiones originales.",
                };
                panel.AddItem(listConnections);

                var catalog = new PushButtonData(
                    "MotorConexiones_Catalog",
                    "Catálogo",
                    assemblyPath,
                    typeof(CatalogCommand).FullName)
                {
                    ToolTip = "Plantillas de conexión con nombre: aplicar una a las barras seleccionadas, guardar una desde una conexión del modelo o borrarla (como conn_catalog_*).",
                    LongDescription = "Fase 7. Aplicar abre la ventana de previsualización con la especificación instanciada en el nudo (casado por ángulos, también en espejo); Crear hace lo mismo que el botón Ejecutar especificación JSON.",
                };
                panel.AddItem(catalog);

                var batchPlan = new PushButtonData(
                    "MotorConexiones_BatchPlan",
                    "Planificar\nlote",
                    assemblyPath,
                    typeof(BatchPlanCommand).FullName)
                {
                    ToolTip = "Detecta los nudos de la cercha seleccionada (basta pinchar una barra: la selección asistida añade las que la tocan), casa cada uno con las plantillas del catálogo, valida nudo a nudo y marca los nudos en el modelo con colores, marcadores y etiquetas pinchables (como conn_batch_plan). Desde la ventana, Crear N conexiones crea el lote (como conn_batch_create).",
                    LongDescription = "Fases 8 a 10. Pincha una barra de la cercha (o selecciónala entera) y pulsa; sin selección, reabre el último plan. En la ventana: Ver en Revit, excluir, cordón, barras, añadir nudo, completar selección, plantilla, editar nudo, descartar, Crear N conexiones (cada nudo por separado, una sola entrada de deshacer) y Borrar el lote. Pinchar la etiqueta de un nudo en la vista elige su fila.",
                };
                panel.AddItem(batchPlan);

                var designBrief = new PushButtonData(
                    "MotorConexiones_DesignBrief",
                    "Encargo\npara IA",
                    assemblyPath,
                    typeof(DesignBriefCommand).FullName)
                {
                    ToolTip = "Con el nudo seleccionado (cordón y barras), escribe el encargo para una IA externa (prompt, datos del nudo, esquema, reglas de lectura y un ejemplo confirmado) en un solo archivo .md, lo copia al portapapeles y abre la carpeta.",
                    LongDescription = "Ronda 9b (Fase 10). Pega el encargo en la IA del navegador junto con la imagen del detalle; guarda el JSON que devuelva y aplícalo con Ejecutar especificación JSON. No usa el MCP ni toca el modelo.",
                };
                panel.AddItem(designBrief);

                // Fase 10 (V3): el manejador de clics de las etiquetas del lienzo se registra una vez por sesión de Revit.
                // Si fallara, las etiquetas se ven igual y no responden al clic; el botón Planificar lote lo reintenta.
                bool labelHandler = PlanLabels.EnsureHandlerRegistered(out string? labelHandlerError);

                JsonLineLogger.Write(new
                {
                    @event = "startup",
                    addin_version = AddinInfo.Version,
                    revit_version = application.ControlledApplication.VersionNumber,
                    revit_build = application.ControlledApplication.VersionBuild,
                    ribbon_tab = ActiveTabName,
                    assembly = assemblyPath,
                    label_handler = labelHandler,
                    label_handler_error = labelHandlerError,
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
        /// Panel "MotorConexiones" en ARBA: se intenta crear la pestaña (si ya existe, Revit lanza
        /// <c>ArgumentException</c> y se reutiliza) y después el panel. El orden de carga de los add-ins no importa:
        /// quien arranca primero crea la pestaña y los demás la reutilizan, que es lo que ya hacen ColumnRebar,
        /// BeamRebar, RetainingWallRebar y RotarNorte en el PC de la persona. Si falla, reserva en "Conexiones".
        /// </summary>
        private static RibbonPanel CreatePanel(UIControlledApplication application)
        {
            bool preferredExisted = !TryCreateTab(application, PreferredTabName, out string tabError);
            try
            {
                RibbonPanel panel = application.CreateRibbonPanel(PreferredTabName, PanelName);
                ActiveTabName = PreferredTabName;
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_panel_created",
                    tab = PreferredTabName,
                    panel = PanelName,
                    tab_already_existed = preferredExisted,
                    create_tab_error = tabError,
                });
                return panel;
            }
            catch (Exception preferredError)
            {
                bool fallbackExisted = !TryCreateTab(application, FallbackTabName, out string fallbackTabError);
                RibbonPanel panel = application.CreateRibbonPanel(FallbackTabName, PanelName);
                ActiveTabName = FallbackTabName;
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_panel_created",
                    tab = FallbackTabName,
                    panel = PanelName,
                    tab_already_existed = fallbackExisted,
                    create_tab_error = fallbackTabError,
                    preferred_tab = PreferredTabName,
                    preferred_tab_error = preferredError.GetType().Name + ": " + preferredError.Message,
                });
                return panel;
            }
        }

        /// <summary>Crea la pestaña; devuelve falso (sin lanzar) si ya existía o Revit no la dejó crear.</summary>
        private static bool TryCreateTab(UIControlledApplication application, string tabName, out string error)
        {
            error = string.Empty;
            try
            {
                application.CreateRibbonTab(tabName);
                return true;
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException ex)
            {
                // ArgumentException: la pestaña ya existe (otro add-in o una recarga). InvalidOperationException: demasiadas pestañas.
                error = ex.GetType().Name + ": " + ex.Message;
                return false;
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
