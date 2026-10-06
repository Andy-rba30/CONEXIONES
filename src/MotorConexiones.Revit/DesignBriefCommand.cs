using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core;
using MotorConexiones.Core.Brief;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Types;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Operations;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón <b>Encargo para IA</b> de la cinta (ronda 9b, <c>docs/propuestas/encargo-ia-externa.md</c>; programado en la
    /// Fase 10). Con el nudo seleccionado (el cordón y las barras que llegan), calcula lo mismo que <c>conn_get_node_info</c>
    /// (por <see cref="Bridge.Handle"/>, sin ventana), escribe <b>un solo archivo Markdown</b> (decisión P1) con el prompt, los
    /// datos del nudo, el esquema, las secciones 2 a 4 de <c>docs/guide.md</c> y un ejemplo confirmado (la plantilla del
    /// catálogo con la misma cantidad de barras o el Detalle D embebido), lo copia al portapapeles y abre la carpeta. La
    /// persona lo pega en la IA del navegador con la imagen del detalle y aplica el JSON con <b>Ejecutar especificación
    /// JSON</b>. Con un solo tipo de conexión no pregunta el tipo (P3). Sin herramienta MCP (P2). El texto lo escribe el
    /// Core (<see cref="DesignBriefWriter"/>): aquí solo se recogen los datos y se guarda.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class DesignBriefCommand : IExternalCommand
    {
        public const string FolderName = "encargos";

        public static string Folder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MotorConexiones", FolderName);

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument? uidoc = commandData.Application.ActiveUIDocument;
            Document? doc = uidoc?.Document;
            if (doc == null || uidoc == null)
            {
                TaskDialog.Show("MotorConexiones - Encargo para IA", "Abre un proyecto de Revit y selecciona el nudo (el cordón y las barras que llegan).");
                return Result.Cancelled;
            }

            var selected = uidoc.Selection.GetElementIds().Select(id => id.Value).OrderBy(id => id).ToList();
            if (selected.Count < 2)
            {
                TaskDialog.Show("MotorConexiones - Encargo para IA",
                    "Selecciona primero el nudo en Revit: el cordón y todas las barras que llegan a él (al menos 2 barras). Ahora hay " + selected.Count + " elemento(s) seleccionado(s).");
                return Result.Cancelled;
            }

            try
            {
                // 1. Los datos del nudo, con el mismo código que conn_get_node_info (IDs reales, perfiles, ángulos, origen, ejes).
                string request = "{\"element_ids\":[" + string.Join(",", selected) + "]}";
                string response = Bridge.Handle("node_info", request, doc, uidoc);
                using JsonDocument parsed = JsonDocument.Parse(response);
                JsonElement root = parsed.RootElement;
                bool ok = root.TryGetProperty("ok", out JsonElement okEl) && okEl.ValueKind == JsonValueKind.True;
                if (!ok)
                {
                    string error = "No se pudieron leer los datos del nudo.";
                    if (root.TryGetProperty("errors", out JsonElement errors) && errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
                    {
                        JsonElement first = errors[0];
                        error = (first.TryGetProperty("code", out JsonElement code) ? code.GetString() + ": " : "")
                                + (first.TryGetProperty("message", out JsonElement msg) ? msg.GetString() : "")
                                + (first.TryGetProperty("hint", out JsonElement hint) && hint.ValueKind == JsonValueKind.String ? "\n" + hint.GetString() : "");
                    }
                    JsonLineLogger.Write(new { @event = "ribbon_design_brief_failed", selection = selected.Count, error });
                    TaskDialog.Show("MotorConexiones - Encargo para IA", "No se pudo preparar el encargo:\n" + error);
                    return Result.Cancelled;
                }
                string nodeInfoJson = JsonSerializer.Serialize(root.GetProperty("data"), new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                int memberCount = root.GetProperty("data").TryGetProperty("members", out JsonElement members) && members.ValueKind == JsonValueKind.Array ? members.GetArrayLength() : selected.Count;

                // 2. El esquema (ejemplo lleno del único tipo), la guía desplegada y el ejemplo confirmado.
                string example = PickExample(memberCount, out string exampleSource);
                var input = new DesignBriefInput
                {
                    DocumentTitle = doc.Title,
                    ConnectionType = GussetNodeType.Instance.Name,
                    AddinVersion = AddinInfo.Version,
                    CreatedLocal = DateTime.Now,
                    SelectedElementIds = selected,
                    NodeInfoJson = nodeInfoJson,
                    SchemaExampleJson = GussetNodeType.Instance.GetExampleJson(),
                    GuideMarkdown = GuideOperation.ReadGuideMarkdown(),
                    ExampleJson = example,
                    ExampleSource = exampleSource,
                };
                string brief = DesignBriefWriter.Write(input);

                // 3. Archivo, portapapeles y carpeta.
                string folder = Folder;
                Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, DesignBriefWriter.FileNameFor(doc.Title, input.CreatedLocal));
                File.WriteAllText(path, brief, new UTF8Encoding(false));
                bool copied = CopyToClipboard(brief);
                bool opened = OpenFolder(path);
                JsonLineLogger.Write(new { @event = "ribbon_design_brief", file = path, length = brief.Length, selection = selected, members = memberCount, example = exampleSource, clipboard = copied, folder_opened = opened });

                var dialog = new TaskDialog("MotorConexiones - Encargo para IA")
                {
                    MainInstruction = copied ? "Encargo copiado al portapapeles" : "Encargo guardado (no se pudo copiar al portapapeles)",
                    MainContent = "Pégalo en la IA del navegador junto con la imagen del detalle (un recorte del plano) y guarda el JSON que te devuelva como archivo .json; "
                                  + "después, Ejecutar especificación JSON con ese archivo.\n\nArchivo: " + path + "\nEjemplo confirmado: " + exampleSource + ".",
                    CommonButtons = TaskDialogCommonButtons.Close,
                };
                dialog.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_design_brief_failed", selection = selected.Count, error = ex.ToString() });
                TaskDialog.Show("MotorConexiones - Error", "No se pudo escribir el encargo:\n" + ex.Message);
                return Result.Failed;
            }
        }

        /// <summary>La plantilla del catálogo con la misma cantidad de barras (la más reciente), o el Detalle D embebido.</summary>
        private static string PickExample(int memberCount, out string source)
        {
            try
            {
                CatalogConfig config = CatalogConfigLoader.Load();
                CatalogStore store = CatalogConfigLoader.OpenStore(config);
                var warnings = new List<Core.Contract.ApiError>();
                CatalogEntry? entry = store.List(warnings)
                    .Where(e => e.MembersCount == Math.Max(1, memberCount - 1))
                    .OrderByDescending(e => e.CreatedUtc, StringComparer.Ordinal)
                    .FirstOrDefault();
                if (entry != null)
                {
                    CatalogTemplate? template = store.Get(entry.TemplateId);
                    if (template?.SpecTemplate != null)
                    {
                        source = "la plantilla '" + template.Name + "' del catálogo (spec_template: sin IDs, con slot por barra; en tu JSON van los element_id de los Datos del nudo)";
                        return template.SpecTemplate.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
                    }
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_design_brief_catalog_failed", error = ex.ToString() });
            }
            source = DesignBriefWriter.EmbeddedExampleSource;
            return DesignBriefWriter.EmbeddedExampleJson();
        }

        private static bool CopyToClipboard(string text)
        {
            try
            {
                System.Windows.Clipboard.SetText(text);
                return true;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_design_brief_clipboard_failed", error = ex.ToString() });
                return false;
            }
        }

        private static bool OpenFolder(string path)
        {
            try
            {
                Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + path + "\"") { UseShellExecute = true });
                return true;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_design_brief_explorer_failed", error = ex.ToString() });
                return false;
            }
        }
    }
}
