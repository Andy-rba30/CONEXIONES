using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Fabrication;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit
{
    /// <summary>
    /// Botón "Ejecutar especificación JSON" en la cinta Ribbon de Revit.
    /// Es el único sitio del add-in donde se interactúa visualmente con el usuario mediante diálogos.
    /// Permite seleccionar un archivo JSON de conexión, lo valida contra el modelo y lo crea atómicamente.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public sealed class RunSpecCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument? uidoc = commandData.Application.ActiveUIDocument;
            Document? doc = uidoc?.Document;

            if (doc == null)
            {
                TaskDialog.Show("MotorConexiones", "Por favor, abre un proyecto de Revit con una cercha o pórtico estructural.");
                return Result.Cancelled;
            }

            if (doc.IsReadOnly)
            {
                TaskDialog.Show("MotorConexiones", "El documento activo es de solo lectura. Ábrelo con permiso de modificación.");
                return Result.Cancelled;
            }

            // 1. Diálogo de selección de archivo JSON
            string? filePath = ShowOpenFileDialog();
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return Result.Cancelled;
            }

            string rawJson;
            try
            {
                rawJson = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MotorConexiones - Error", "No se pudo leer el archivo seleccionado: " + ex.Message);
                return Result.Failed;
            }

            ConnectionSpec? spec;
            try
            {
                spec = ConnectionSpec.FromJson(rawJson);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MotorConexiones - Error JSON", "El archivo no contiene un JSON válido:\n" + ex.Message);
                return Result.Failed;
            }

            if (spec == null)
            {
                TaskDialog.Show("MotorConexiones - Error", "La especificación no pudo ser deserializada.");
                return Result.Failed;
            }

            // 2. Resolver miembros seleccionados en Revit si no vienen en la especificación
            var selectedIds = uidoc?.Selection?.GetElementIds()?.Select(id => id.Value).ToList() ?? new List<long>();
            if ((spec.Node?.ElementIds == null || spec.Node.ElementIds.Count == 0) && selectedIds.Count >= 2)
            {
                spec.Node = new NodeRef { ElementIds = selectedIds };
                if (spec.Chord == null || spec.Chord.ElementId <= 0)
                {
                    spec.Chord = new ChordSpec { ElementId = selectedIds[0], Continuous = true };
                }
                rawJson = spec.ToJson();
            }

            // 3. Dudas sin confirmar: el add-in nunca inventa valores (sección 5.6 del encargo). Se para y se explica.
            if (spec.UncertainFields != null && spec.UncertainFields.Any(u => u.UserConfirmedValue == null))
            {
                var unconfirmed = spec.UncertainFields.Where(u => u.UserConfirmedValue == null).ToList();
                string details = string.Join("\n", unconfirmed.Select(u => "• " + u.Path + ": " + u.Reason));
                var uncertaintyDialog = new TaskDialog("MotorConexiones - Dudas sin resolver")
                {
                    MainInstruction = "La especificación tiene dudas sin confirmar y no se puede crear.",
                    MainContent = details + "\n\nRellena user_confirmed_value en cada entrada de uncertain_fields del archivo JSON y vuelve a ejecutarlo.",
                    CommonButtons = TaskDialogCommonButtons.Close,
                };
                uncertaintyDialog.Show();
                return Result.Cancelled;
            }

            // 4. Validar especificación contra el modelo (marco con la misma regla que las operaciones del MCP)
            NodeFrame? frame = null;
            try
            {
                frame = NodeInspector.ResolveNode(doc, spec).Frame;
            }
            catch (NodeInspectionException)
            {
                // El validador informará de los IDs que falten.
            }

            LimitsConfig limits = LoadLimitsConfig();
            var modelFacts = new RevitModelFacts(doc, frame);
            ValidationResult validationResult = SpecValidator.Validate(rawJson, spec, modelFacts, limits);

            if (!validationResult.IsValid)
            {
                string errorList = string.Join("\n\n", validationResult.Errors.Select(e =>
                    $"• [{e.Code}] {e.Message}\n  Campo: {e.Path}\n  Sugerencia: {e.Hint}"));

                var errDialog = new TaskDialog("MotorConexiones - Errores de Validación")
                {
                    MainInstruction = "La especificación no superó las comprobaciones de validación.",
                    MainContent = errorList,
                    CommonButtons = TaskDialogCommonButtons.Close
                };
                errDialog.Show();
                return Result.Failed;
            }

            // 5. Diálogo de confirmación con resumen de la conexión
            int knifePlatesCount = spec.Members?.Count(m => string.Equals(m.Attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase)) ?? 0;
            int totalBolts = spec.Members?.Where(m => m.Attachment?.Bolts != null)
                .Sum(m => m.Attachment!.Bolts!.Rows * m.Attachment!.Bolts!.Columns) ?? 0;

            string summaryText =
                $"Tipo de conexión: {spec.ConnectionType}\n" +
                $"Plano de origen: {spec.Source?.Drawing ?? "Detalle"}\n" +
                $"Cordón: ElementId [{spec.Chord?.ElementId}] ({spec.Chord?.Profile})\n" +
                $"Cartela: Espesor {spec.Gusset?.ThicknessMm} mm ({spec.Gusset?.ThicknessLabel}), ancho {spec.Gusset?.WidthMm} mm, alto {spec.Gusset?.HeightMm} mm\n" +
                $"Miembros a conectar: {spec.Members?.Count ?? 0}\n" +
                $"Placas cuchilla: {knifePlatesCount}\n" +
                $"Pernos totales: {totalBolts}\n" +
                $"Advertencias: {validationResult.Warnings.Count}\n" +
                $"Token de validación: {(validationResult.ValidationToken != null ? validationResult.ValidationToken.Substring(0, 16) + "..." : "Generado")}\n\n" +
                "¿Desea crear esta conexión de forma atómica en el modelo?";

            var confirmDialog = new TaskDialog("MotorConexiones - Confirmar Creación")
            {
                MainInstruction = "¿Desea modelar la conexión en el modelo activo?",
                MainContent = summaryText,
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No
            };

            if (confirmDialog.Show() != TaskDialogResult.Yes)
            {
                return Result.Cancelled;
            }

            // 6. Creación atómica dentro de OperationScope
            var warnings = new List<ApiError>();
            string opId = Guid.NewGuid().ToString("D");
            ConnectionRecord createdRecord;

            try
            {
                using (var scope = new OperationScope(doc, commandData.Application, "run_spec_ribbon", opId, warnings))
                {
                    using (Transaction tx = scope.StartTransaction(doc, "MotorConexiones: Crear " + (spec.Source?.Drawing ?? "Conexión")))
                    {
                        createdRecord = ConnectionCreationService.CreateConnection(doc, uidoc, spec, rawJson, opId, warnings);
                        scope.CommitOrThrow(tx);
                    }
                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("MotorConexiones - Error en Modelado",
                    "Ocurrió un error al crear la geometría de la conexión:\n\n" + ex.Message +
                    "\n\nSe ha realizado un rollback completo.");
                return Result.Failed;
            }

            // 7. Diálogo de éxito final
            string backendName = createdRecord.BackendName;
            string warningText = warnings.Count == 0 ? "" : "\nAdvertencias: " + string.Join("; ", warnings.Select(w => w.Code));
            var successDialog = new TaskDialog("MotorConexiones - Éxito")
            {
                MainInstruction = "Conexión modelada correctamente en el modelo.",
                MainContent =
                    $"ID de conexión: {createdRecord.ConnectionId}\n" +
                    $"Elementos geométricos creados: {createdRecord.CreatedElementIds.Count}\n" +
                    $"Miembros modificados (setbacks): {createdRecord.ModifiedMembers.Count}\n" +
                    $"Backend de fabricación utilizado: {backendName}{warningText}\n" +
                    "La conexión ha quedado registrada en Extensible Storage y puede ser consultada o borrada con las herramientas conn_* del MCP.",
                CommonButtons = TaskDialogCommonButtons.Close
            };
            successDialog.Show();

            return Result.Succeeded;
        }

        private static string? ShowOpenFileDialog()
        {
            try
            {
                // Localizar System.Windows.Forms por reflexión para no añadir dependencias pesadas de SDK en csproj
                Assembly? formsAsm = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "System.Windows.Forms")
                                     ?? Assembly.Load("System.Windows.Forms");

                Type? dialogType = formsAsm?.GetType("System.Windows.Forms.OpenFileDialog");
                if (dialogType != null)
                {
                    dynamic dialog = Activator.CreateInstance(dialogType)!;
                    dialog.Title = "Seleccionar especificación JSON de conexión";
                    dialog.Filter = "Archivos JSON (*.json)|*.json|Todos los archivos (*.*)|*.*";
                    dialog.Multiselect = false;

                    // Carpeta inicial: la última usada por Windows; si no, Documentos.
                    string defaultDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    if (Directory.Exists(defaultDir))
                    {
                        dialog.InitialDirectory = defaultDir;
                        dialog.RestoreDirectory = true;
                    }

                    dynamic result = dialog.ShowDialog();
                    if (result.ToString() == "OK")
                    {
                        return dialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "open_file_dialog_failed", error = ex.ToString() });
            }

            return null;
        }

        private static LimitsConfig LoadLimitsConfig()
        {
            try
            {
                string addinFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                string limitsPath = Path.Combine(addinFolder, "config", "limits.json");
                if (File.Exists(limitsPath))
                {
                    return LimitsConfig.LoadFromFile(limitsPath);
                }
            }
            catch { }

            return LimitsConfig.Default;
        }
    }
}
