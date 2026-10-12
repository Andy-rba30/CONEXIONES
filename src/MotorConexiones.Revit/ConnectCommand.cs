using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Batch;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Batch;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.UI;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón <b>Conectar</b> de la cinta (Fase 11, decisiones D1 a D3): con las barras del nudo seleccionadas (el cordón y las
    /// que llegan) lee el nudo con la misma regla que <c>conn_get_node_info</c> y <c>conn_catalog_apply</c>
    /// (<see cref="TemplateNode.FromModelFacts"/>: cordón el más horizontal, marco canónico, ángulos con signo), casa
    /// <b>todas</b> las plantillas del catálogo (<see cref="TemplateChooser"/>) y abre la previsualización con la mejor ya
    /// aplicada, sin pedir ningún archivo JSON. Si ninguna encaja, abre igual una <b>especificación de arranque</b> con cartela
    /// rectangular (<see cref="SpecStarter"/>) y lo dice en la cabecera. Con una sola barra seleccionada pide pinchar el cordón y
    /// las demás barras en Revit (Esc termina); con cero, un cuadro corto. Si la selección parece una cercha (más de un nudo con
    /// cordón) lo dice y ofrece abrir Planificar lote (D1). <b>No crea nada al pulsarse</b>: solo abre la ventana; Crear hace lo
    /// mismo que los demás botones (<see cref="RibbonCreation"/>). Este botón es un camino con ventanas; las rutas conn_* no cambian.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class ConnectCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApplication = commandData.Application;
            UIDocument? uidoc = uiApplication.ActiveUIDocument;
            Document? doc = uidoc?.Document;
            if (doc == null || uidoc == null)
            {
                TaskDialog.Show(RibbonCreation.TitleConnect, "Abre un proyecto de Revit y selecciona el nudo: el cordón y las barras que llegan a él.");
                return Result.Cancelled;
            }
            if (doc.IsReadOnly)
            {
                TaskDialog.Show(RibbonCreation.TitleConnect, "El documento activo es de solo lectura. Ábrelo con permiso de modificación.");
                return Result.Cancelled;
            }

            var stopwatch = Stopwatch.StartNew();
            var selected = uidoc.Selection.GetElementIds().Select(id => id.Value).Distinct().OrderBy(id => id).ToList();
            var facts = new RevitModelFacts(doc);
            List<long> ids = selected.Where(id => IsBar(facts, id)).ToList();
            int ignored = selected.Count - ids.Count;

            // D3: con una barra, pinchar el cordón y las demás barras en Revit (Esc termina); con cero, cuadro corto.
            if (ids.Count == 1)
            {
                List<long> picked = PickMore(uidoc, ids[0]);
                if (picked.Count == 0)
                {
                    JsonLineLogger.Write(new { @event = "ribbon_connect_cancelled", selection = selected.Count, reason = "pick_cancelled" });
                    return Result.Cancelled;
                }
                ids = ids.Concat(picked).Distinct().OrderBy(id => id).ToList();
            }
            if (ids.Count < 2)
            {
                TaskDialog.Show(RibbonCreation.TitleConnect,
                    "Selecciona primero el nudo en Revit: el cordón y todas las barras que llegan a él (al menos 2 barras de armazón estructural). Ahora hay "
                    + ids.Count + " barra(s)" + (ignored > 0 ? " y " + ignored + " elemento(s) que no son barras" : "") + ".\n\nCon una sola barra seleccionada, Conectar te pide pinchar las demás en Revit.");
                return Result.Cancelled;
            }

            // D1: si la selección da más de un nudo con cordón, es una cercha: Planificar lote.
            int nodeCount = CountNodes(facts, ids);
            if (nodeCount > 1)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_truss", selection = ids.Count, nodes = nodeCount });
                var truss = new TaskDialog(RibbonCreation.TitleConnect)
                {
                    MainInstruction = "Esto parece una cercha (" + nodeCount + " nudos): usa Planificar lote",
                    MainContent = "Conectar es para un nudo suelto: el cordón y las barras que llegan a él. La selección tiene " + ids.Count + " barras que forman " + nodeCount + " nudos con cordón.",
                    AllowCancellation = true,
                    CommonButtons = TaskDialogCommonButtons.Cancel,
                };
                truss.AddCommandLink(TaskDialogCommandLinkId.CommandLink1, "Abrir Planificar lote con esta selección", "Detecta los nudos, casa cada uno con el catálogo y crea el lote desde su ventana.");
                truss.AddCommandLink(TaskDialogCommandLinkId.CommandLink2, "Cancelar y seleccionar solo un nudo", "Pincha el cordón y las barras de un solo nudo y vuelve a pulsar Conectar.");
                truss.DefaultButton = TaskDialogResult.CommandLink1;
                if (truss.Show() == TaskDialogResult.CommandLink1)
                {
                    return new BatchPlanCommand().Execute(commandData, ref message, elements);
                }
                return Result.Cancelled;
            }

            // Reconocer el nudo: la misma regla que conn_get_node_info y conn_catalog_apply.
            TemplateNode node;
            try
            {
                long chord = TemplateNode.ChooseChord(facts, ids);
                node = TemplateNode.FromModelFacts(facts, chord, ids.Where(id => id != chord));
            }
            catch (CatalogException ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_failed", selection = ids, code = ex.Error.Code, error = ex.Error.Message });
                TaskDialog.Show(RibbonCreation.TitleConnect, "No se pudo leer el nudo (" + ex.Error.Code + "):\n" + ex.Error.Message + (ex.Error.Hint != null ? "\n\n" + ex.Error.Hint : ""));
                return Result.Cancelled;
            }

            PreviewSession session;
            TemplateProposal proposal;
            string mode;
            try
            {
                CatalogConfig config = CatalogConfigLoader.Load();
                CatalogStore store = CatalogConfigLoader.OpenStore(config);
                var warnings = new List<ApiError>();
                List<CatalogTemplate> templates = LoadTemplates(store, warnings);
                proposal = TemplateChooser.Propose(node, templates, config);

                string json;
                string title;
                string header;
                string virtualPath;
                bool starter;
                if (proposal.Chosen != null)
                {
                    TemplateCandidate chosen = proposal.Chosen;
                    InstantiationResult result = TemplateInstantiator.Instantiate(chosen.Template, chosen.Match, node, config);
                    json = result.SpecJson;
                    title = "Conectar · nudo del cordón " + node.ChordElementId + " · " + chosen.Name;
                    header = proposal.SummaryText + (proposal.Candidates.Count > 1 ? " Pulsa Cambiar… para elegir otra." : "")
                             + (result.Warnings.Count > 0 ? " Avisos: " + string.Join(", ", result.Warnings.Select(w => w.Code).Distinct()) + "." : "");
                    virtualPath = CatalogService.VirtualFilePath(chosen.Template, node.ChordElementId);
                    starter = false;
                    mode = "template";
                }
                else
                {
                    // D2 y D10: especificación de arranque con los valores de la plantilla del mismo cordón o de choices.json.
                    PanelChoices choices = PanelChoicesLoader.Load(LimitsConfigLoader.Load());
                    StarterResult result = SpecStarter.FromNode(node, choices, templates);
                    json = result.SpecJson;
                    title = "Conectar · nudo del cordón " + node.ChordElementId + " · especificación de arranque";
                    header = proposal.SummaryText + " " + result.HeaderText;
                    string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    virtualPath = Path.Combine(documents, "MotorConexiones", "conectar-nudo-" + node.ChordElementId + ".json");
                    starter = true;
                    mode = proposal.IsCatalogEmpty ? "starter_empty_catalog" : "starter";
                }

                session = new PreviewSession(doc, uidoc, virtualPath, json, isVirtualFile: true, title: title)
                {
                    Node = node,
                    Proposal = proposal,
                    CatalogConfig = config,
                    IsStarter = starter,
                    HeaderText = header,
                    HeaderWarning = ExistingConnectionNote(doc, node) ?? (warnings.Count > 0 ? string.Join(" · ", warnings.Select(w => w.Message)) : null),
                };
                JsonLineLogger.Write(new
                {
                    @event = "ribbon_connect",
                    document = doc.Title,
                    selection = selected.Count,
                    ignored,
                    members = node.Members.Count,
                    chord_element_id = node.ChordElementId,
                    templates = proposal.TemplatesCount,
                    candidates = proposal.Candidates.Count,
                    proposal = proposal.Chosen == null ? null : new { template_id = proposal.Chosen.TemplateId, name = proposal.Chosen.Name, orientation = proposal.Chosen.OrientationName, max_deviation_deg = Math.Round(proposal.Chosen.MaxDeviationDeg, 2) },
                    starter,
                    mode,
                    is_valid = session.CanCreate,
                    errors = session.Validation?.Errors.Select(e => e.Code).Distinct().ToList(),
                    duration_ms = stopwatch.ElapsedMilliseconds,
                });
            }
            catch (CatalogException ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_failed", selection = ids, code = ex.Error.Code, error = ex.Error.Message });
                TaskDialog.Show(RibbonCreation.TitleConnect, "No se pudo preparar la conexión (" + ex.Error.Code + "):\n" + ex.Error.Message + (ex.Error.Hint != null ? "\n\n" + ex.Error.Hint : ""));
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_failed", selection = ids, error = ex.ToString() });
                TaskDialog.Show(RibbonCreation.TitleError, "No se pudo preparar la conexión:\n" + ex.Message);
                return Result.Failed;
            }

            // La previsualización (modal, como siempre: es el cuadro del botón de la cinta).
            bool createRequested;
            try
            {
                var window = new PreviewWindow(session);
                _ = new WindowInteropHelper(window) { Owner = uiApplication.MainWindowHandle };
                window.ShowDialog();
                createRequested = window.CreateRequested;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_failed", selection = ids, error = ex.ToString(), stage = "window" });
                TaskDialog.Show(RibbonCreation.TitleError, "No se pudo abrir la ventana de previsualización:\n" + ex.Message);
                return Result.Failed;
            }
            if (!createRequested)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_cancelled", selection = ids.Count, reason = "window_closed", mode });
                return Result.Cancelled;
            }
            return RibbonCreation.CreateFromSession(uiApplication, doc, uidoc, session, "connect_ribbon");
        }

        private static bool IsBar(IModelFacts facts, long id)
        {
            try
            {
                if (!facts.ElementExists(id) || !facts.IsStructuralMember(id)) return false;
                MemberModelFacts? member = facts.GetMemberFacts(id);
                return member != null && member.CurveStartMm.DistanceTo(member.CurveEndMm) > 1e-6;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Pinchar el cordón y las barras que faltan, una a una, hasta Esc. Devuelve lo pinchado (vacío si se canceló sin pinchar nada).</summary>
        private static List<long> PickMore(UIDocument uidoc, long already)
        {
            var picked = new List<long>();
            while (true)
            {
                Reference? reference = PlanPicker.PickOne(uidoc, "Conectar: pincha el cordón y las barras del nudo (ya hay " + (1 + picked.Count) + "; Esc termina)");
                if (reference == null) break;
                long id = reference.ElementId.Value;
                if (id != already && !picked.Contains(id)) picked.Add(id);
            }
            JsonLineLogger.Write(new { @event = "ribbon_connect_pick", already, picked });
            return picked;
        }

        /// <summary>Cuántos nudos con cordón y barras forma la selección (la regla del detector del plan de lote).</summary>
        private static int CountNodes(RevitModelFacts facts, List<long> ids)
        {
            try
            {
                var bars = new List<DetectorBar>();
                foreach (long id in ids)
                {
                    MemberModelFacts? member = facts.GetMemberFacts(id);
                    if (member != null) bars.Add(DetectorBar.FromFacts(member));
                }
                List<DetectedNode> nodes = NodeDetector.Detect(bars, NodeDetectorOptions.FromConfig(CatalogConfigLoader.Load()));
                return nodes.Count(n => n.ChordElementId != 0 && n.MemberElementIds.Count >= 1 && NodeStatus.CanMatch(n.Status));
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_detect_failed", error = ex.Message });
                return 1;
            }
        }

        private static List<CatalogTemplate> LoadTemplates(CatalogStore store, List<ApiError> warnings)
        {
            var templates = new List<CatalogTemplate>();
            foreach (CatalogEntry entry in store.List(warnings))
            {
                try
                {
                    CatalogTemplate? template = store.Get(entry.TemplateId);
                    if (template != null) templates.Add(template);
                }
                catch (CatalogException error)
                {
                    warnings.Add(error.Error);
                }
            }
            return templates;
        }

        /// <summary>Si el nudo ya tiene una conexión del add-in (mismo cordón y alguna barra), una frase para la cabecera; si no, nulo.</summary>
        private static string? ExistingConnectionNote(Document doc, TemplateNode node)
        {
            try
            {
                var planNode = new PlanNode { ChordElementId = node.ChordElementId, MemberElementIds = node.Members.Select(m => m.ElementId).ToList() };
                ExistingConnection? existing = BatchPlanner.ExistingConnections(doc).FirstOrDefault(c => c.Covers(planNode));
                return existing == null ? null : "Este nudo ya tiene la conexión " + existing.ConnectionId + " del add-in: si creas otra se solaparán (bórrala antes con Conexiones del modelo).";
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_connect_existing_failed", error = ex.Message });
                return null;
            }
        }
    }
}
