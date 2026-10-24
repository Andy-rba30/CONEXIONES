using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Model;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón <b>Partir barra</b> (Fase 13, D6 a D8): con el cordón y la diagonal seleccionados, parte la diagonal a la distancia del
    /// Detalle B (330 mm desde la cara del cordón por el eje de la barra, más la pila de planchas; <c>choices.json</c>) como
    /// <c>conn_split_member</c>; con un tramo ya partido seleccionado, deshace el corte como <c>conn_unsplit</c>. Los cuadros son los del
    /// botón de la cinta (permitidos); la IA usa las operaciones del MCP.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SplitMemberCommand : IExternalCommand
    {
        private const string Title = "Partir barra";

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApplication = commandData.Application;
            UIDocument? uidoc = uiApplication.ActiveUIDocument;
            if (uidoc?.Document == null)
            {
                TaskDialog.Show(Title, "Abre un modelo de Revit primero.");
                return Result.Cancelled;
            }
            Document doc = uidoc.Document;
            var facts = new RevitModelFacts(doc);
            List<long> ids = uidoc.Selection.GetElementIds().Select(id => id.Value).Where(id => IsBar(facts, id)).Distinct().OrderBy(id => id).ToList();

            // 1. Deshacer: alguna barra seleccionada es un tramo de un corte del add-in.
            SplitRecord? split = ids.Select(id => SplitStorageManager.FindByElement(doc, id)).FirstOrDefault(r => r != null);
            if (split != null) return Unsplit(uiApplication, doc, split);

            if (ids.Count < 2)
            {
                TaskDialog.Show(Title,
                    "Selecciona el cordón y la diagonal que quieres partir (al menos 2 barras de armazón). Ahora hay " + ids.Count + " barra(s).\n\n" +
                    "Para deshacer un corte, selecciona uno de sus dos tramos y vuelve a pulsar Partir barra.");
                return Result.Cancelled;
            }

            // 2. El nudo: el cordón y las barras que llegan (misma regla que Conectar).
            TemplateNode node;
            try
            {
                List<TemplateNode> nodes = SelectionNodes.Resolve(facts, ids, null, out int pointCount);
                if (pointCount > 1)
                {
                    TaskDialog.Show(Title, "La selección forma " + pointCount + " nudos: selecciona el cordón y la diagonal de un solo nudo (Planificar lote parte las diagonales de una cercha entera).");
                    return Result.Cancelled;
                }
                node = nodes.FirstOrDefault(n => !n.IsSplice) ?? throw new CatalogException(ErrorCodes.NodeAxesParallel, "Las barras seleccionadas son colineales (un empalme): no hay cordón desde cuya cara medir el corte.", "element_ids", "Selecciona el cordón y una diagonal que llegue a él con ángulo.");
            }
            catch (CatalogException ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_split_failed", selection = ids, code = ex.Error.Code, error = ex.Error.Message });
                TaskDialog.Show(Title, "No se pudo leer el nudo (" + ex.Error.Code + "):\n" + ex.Error.Message + (ex.Error.Hint != null ? "\n\n" + ex.Error.Hint : ""));
                return Result.Cancelled;
            }

            PanelChoices choices = PanelChoicesLoader.Load(LimitsConfigLoader.Load());
            double offset = choices.DiagonalSpliceOffsetMm;
            List<long> diagonals = node.Members.Select(m => m.ElementId).ToList();
            if (diagonals.Count > 3)
            {
                TaskDialog.Show(Title, "Hay " + diagonals.Count + " barras sobre el cordón " + node.ChordElementId + ": selecciona el cordón y como mucho 3 diagonales.");
                return Result.Cancelled;
            }

            // 3. Elegir la diagonal (y la distancia) con un cuadro del botón.
            while (true)
            {
                var dialog = new TaskDialog(Title)
                {
                    MainInstruction = "Partir a " + Format(offset) + " mm de la cara del cordón " + node.ChordElementId + " (Detalle B)",
                    MainContent = "El corte va por el eje de la barra: cara del cordón + " + Format(offset) + " + espesor de la plancha (+ mitad del hueco). El tramo corto conserva el id de la barra; el largo es un elemento nuevo. Deshacer: selecciona un tramo y pulsa Partir barra.",
                    AllowCancellation = true,
                    CommonButtons = TaskDialogCommonButtons.Cancel,
                };
                var previews = new Dictionary<TaskDialogResult, SplitPreview>();
                TaskDialogCommandLinkId[] links = { TaskDialogCommandLinkId.CommandLink1, TaskDialogCommandLinkId.CommandLink2, TaskDialogCommandLinkId.CommandLink3 };
                TaskDialogResult[] results = { TaskDialogResult.CommandLink1, TaskDialogResult.CommandLink2, TaskDialogResult.CommandLink3 };
                for (int i = 0; i < diagonals.Count; i++)
                {
                    try
                    {
                        SplitPreview preview = SplitService.Preview(doc, diagonals[i], node.ChordElementId, offset);
                        SplitService.EnsureNotInUseForSplit(doc, diagonals[i], node.ChordElementId);
                        previews[results[i]] = preview;
                        dialog.AddCommandLink(links[i], "Partir la barra " + diagonals[i] + " (" + preview.MemberType + ")", preview.Describe());
                    }
                    catch (CatalogException ex)
                    {
                        dialog.AddCommandLink(links[i], "La barra " + diagonals[i] + " no se puede partir", ex.Error.Message);
                    }
                }
                dialog.AddCommandLink(TaskDialogCommandLinkId.CommandLink4, "Otra distancia desde la cara…", "Ahora " + Format(offset) + " mm (diagonal_splice_offset_mm de choices.json).");
                TaskDialogResult result = dialog.Show();
                if (result == TaskDialogResult.CommandLink4)
                {
                    double? other = AskDistance(uiApplication, offset);
                    if (!other.HasValue) return Result.Cancelled;
                    offset = other.Value;
                    continue;
                }
                if (!previews.TryGetValue(result, out SplitPreview? chosen) || chosen == null)
                {
                    return Result.Cancelled;
                }
                return Split(uiApplication, doc, chosen);
            }
        }

        private static Result Split(UIApplication uiApplication, Document doc, SplitPreview preview)
        {
            var warnings = new List<ApiError>();
            SplitRecord record;
            try
            {
                record = SplitService.Split(doc, uiApplication, preview, null, null, warnings);
            }
            catch (CatalogException ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_split_failed", element_id = preview.ElementId, code = ex.Error.Code, error = ex.Error.Message });
                TaskDialog.Show(Title, "No se pudo partir la barra " + preview.ElementId + " (" + ex.Error.Code + "):\n" + ex.Error.Message + (ex.Error.Hint != null ? "\n\n" + ex.Error.Hint : "") + "\n\nSe deshizo todo: la barra sigue entera.");
                return Result.Failed;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_split_failed", element_id = preview.ElementId, error = ex.ToString() });
                TaskDialog.Show(Title, "No se pudo partir la barra " + preview.ElementId + ":\n" + ex.Message + "\n\nSe deshizo todo: la barra sigue entera.");
                return Result.Failed;
            }
            JsonLineLogger.Write(new { @event = "ribbon_split", element_id = record.ElementId, new_element_id = record.NewElementId, split_id = record.SplitId, mechanism = record.Mechanism, warnings = warnings.Select(w => w.Code).ToList() });
            string warningText = warnings.Count == 0 ? "" : "\nAdvertencias: " + string.Join("; ", warnings.Select(w => w.Code).Distinct());
            var done = new TaskDialog(Title)
            {
                MainInstruction = "Barra partida en dos.",
                MainContent =
                    "Tramo corto (del lado del cordón, conserva el id): " + record.ElementId + "\n" +
                    "Tramo largo (elemento nuevo): " + record.NewElementId + "\n" +
                    "Corte a " + Format(record.SplitDistanceMm) + " mm del eje del cordón por el eje de la barra (" + Format(record.OffsetFromChordFaceMm) + " desde la cara).\n" +
                    "Mecanismo: " + record.Mechanism + ".\n\n" +
                    "Siguiente: selecciona los dos tramos y pulsa Conectar para el empalme del Detalle B; para deshacer el corte, selecciona un tramo y pulsa Partir barra." + warningText,
                CommonButtons = TaskDialogCommonButtons.Close,
            };
            done.Show();
            return Result.Succeeded;
        }

        private static Result Unsplit(UIApplication uiApplication, Document doc, SplitRecord split)
        {
            var ask = new TaskDialog(Title)
            {
                MainInstruction = "La barra " + split.ElementId + " está partida (tramo nuevo " + split.NewElementId + "): ¿deshacer el corte?",
                MainContent = "Se borra el tramo " + split.NewElementId + " y la barra " + split.ElementId + " recupera su curva y sus extensiones. Solo se puede si ninguna conexión usa los tramos (si no, bórralas antes con Conexiones del modelo).",
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
                DefaultButton = TaskDialogResult.Yes,
            };
            if (ask.Show() != TaskDialogResult.Yes) return Result.Cancelled;
            var warnings = new List<ApiError>();
            bool complete;
            try
            {
                complete = SplitService.Unsplit(doc, uiApplication, split, warnings);
            }
            catch (CatalogException ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_unsplit_failed", split_id = split.SplitId, code = ex.Error.Code, error = ex.Error.Message });
                TaskDialog.Show(Title, "No se pudo deshacer el corte (" + ex.Error.Code + "):\n" + ex.Error.Message + (ex.Error.Hint != null ? "\n\n" + ex.Error.Hint : ""));
                return Result.Failed;
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_unsplit_failed", split_id = split.SplitId, error = ex.ToString() });
                TaskDialog.Show(Title, "No se pudo deshacer el corte:\n" + ex.Message + "\n\nSe deshizo todo.");
                return Result.Failed;
            }
            JsonLineLogger.Write(new { @event = "ribbon_unsplit", split_id = split.SplitId, element_id = split.ElementId, new_element_id = split.NewElementId, complete, warnings = warnings.Select(w => w.Code).ToList() });
            TaskDialog.Show(Title, "Corte deshecho: el tramo " + split.NewElementId + " se borró y la barra " + split.ElementId + " recuperó su curva" + (complete ? "." : " (parcial: " + string.Join("; ", warnings.Select(w => w.Message)) + ")."));
            return Result.Succeeded;
        }

        /// <summary>Cuadro mínimo (del botón de la cinta) para otra distancia desde la cara del cordón; nulo si se cancela.</summary>
        private static double? AskDistance(UIApplication uiApplication, double current)
        {
            var box = new System.Windows.Controls.TextBox { Text = Format(current), Margin = new Thickness(0, 4, 0, 8), MinWidth = 120 };
            var ok = new Button { Content = "Aceptar", IsDefault = true, MinWidth = 80, Margin = new Thickness(0, 0, 6, 0) };
            var cancel = new Button { Content = "Cancelar", IsCancel = true, MinWidth = 80 };
            var buttons = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            var panel = new StackPanel { Margin = new Thickness(12) };
            panel.Children.Add(new TextBlock { Text = "Distancia desde la cara del cordón hasta la primera plancha, en mm (Detalle B: 330):", TextWrapping = TextWrapping.Wrap });
            panel.Children.Add(box);
            panel.Children.Add(buttons);
            var window = new Window
            {
                Title = Title + " · distancia",
                Content = panel,
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 360,
            };
            _ = new WindowInteropHelper(window) { Owner = uiApplication.MainWindowHandle };
            double? value = null;
            ok.Click += (_, _) =>
            {
                string text = box.Text.Trim().Replace(',', '.');
                if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed) && parsed > 0)
                {
                    value = parsed;
                    window.DialogResult = true;
                    window.Close();
                }
                else
                {
                    box.ToolTip = "Escribe un número mayor que cero (por ejemplo 330).";
                    box.Focus();
                    box.SelectAll();
                }
            };
            box.Focus();
            box.SelectAll();
            window.ShowDialog();
            return value;
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

        private static string Format(double value) => value.ToString("0.#", CultureInfo.InvariantCulture);
    }
}
