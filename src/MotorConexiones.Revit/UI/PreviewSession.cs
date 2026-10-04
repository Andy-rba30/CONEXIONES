using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Node;
using MotorConexiones.Revit.Services;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Estado de la ventana de previsualización: la especificación en edición, su JSON, el nudo leído del modelo, el
    /// croquis y la última validación. Todo lo que calcula viene del Core (<see cref="SketchBuilder"/>,
    /// <see cref="SpecFieldCatalog"/>, <see cref="SpecFieldEditor"/>, <see cref="SpecValidator"/>); de Revit solo
    /// lee el documento (nunca lo modifica). Vive únicamente en el camino del botón de la cinta.
    /// </summary>
    public sealed class PreviewSession
    {
        public PreviewSession(Document document, string? filePath, ConnectionSpec spec, string rawJson, LimitsConfig limits)
        {
            Document = document ?? throw new ArgumentNullException(nameof(document));
            FilePath = filePath;
            Spec = spec ?? throw new ArgumentNullException(nameof(spec));
            RawJson = rawJson ?? spec.ToJson();
            Limits = limits ?? LimitsConfig.Default;
        }

        public Document Document { get; }

        /// <summary>Archivo JSON de origen (null si la especificación no vino de un archivo).</summary>
        public string? FilePath { get; }

        public ConnectionSpec Spec { get; private set; }

        /// <summary>JSON compacto de la especificación actual: es lo que firma el token y lo que se guarda en el modelo.</summary>
        public string RawJson { get; private set; }

        public LimitsConfig Limits { get; }

        public ValidationResult? LastValidation { get; private set; }
        public SketchModel? LastSketch { get; private set; }
        public SketchNodeInput? LastNode { get; private set; }

        /// <summary>Paquete de pernos por barra empernada (lo mismo que creará el add-in), para mostrarlo bajo el croquis.</summary>
        public List<(long MemberId, BoltStack Stack)> BoltStacks { get; } = new List<(long, BoltStack)>();

        /// <summary>Problemas al leer el nudo del modelo (IDs que faltan, ejes que no se cortan…), además de los del validador.</summary>
        public List<ApiError> NodeErrors { get; } = new List<ApiError>();

        /// <summary>Crear solo con validación en verde y token.</summary>
        public bool CanCreate => LastValidation != null && LastValidation.IsValid && !string.IsNullOrEmpty(LastValidation.ValidationToken);

        /// <summary>Recalcula nudo, croquis y validación con la especificación actual.</summary>
        public void Refresh()
        {
            NodeErrors.Clear();
            NodeFrame? frame = null;
            try
            {
                frame = NodeInspector.ResolveNode(Document, Spec).Frame;
            }
            catch (NodeInspectionException error)
            {
                NodeErrors.Add(error.Error);
            }
            catch (Exception error)
            {
                NodeErrors.Add(new ApiError(ErrorCodes.InternalError, "No se pudo leer el nudo del modelo: " + error.Message));
            }

            var facts = new RevitModelFacts(Document, frame);
            LastNode = SketchNodeInput.FromModelFacts(Spec, facts);
            LastSketch = SketchBuilder.Build(Spec, LastNode);
            LastValidation = SpecValidator.Validate(RawJson, Spec, facts, Limits);

            BoltStacks.Clear();
            if (Spec.Members != null)
            {
                foreach (MemberSpec member in Spec.Members)
                {
                    if (member.Attachment?.Bolts == null || !string.Equals(member.Attachment.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase)) continue;
                    try
                    {
                        BoltStacks.Add((member.ElementId, ConnectionCreationService.ComputeBoltStack(Spec, member, Limits)));
                    }
                    catch (ArgumentException)
                    {
                        // Espesores o diámetro no válidos: el validador ya lo dice; no hay paquete que mostrar.
                    }
                }
            }
        }

        /// <summary>Texto de una línea por paquete de pernos, en mm, para la nota bajo el croquis.</summary>
        public string BoltStacksText()
        {
            var parts = new List<string>();
            foreach ((long memberId, BoltStack stack) in BoltStacks)
            {
                parts.Add("Pernos de la barra " + memberId + ": agarre " + Core.Sketch.SketchFormat.Mm(stack.GripMm) + " mm (cartela "
                    + Core.Sketch.SketchFormat.Mm(stack.GussetThicknessMm) + " + placa cuchilla " + Core.Sketch.SketchFormat.Mm(stack.KnifeThicknessMm)
                    + "), longitud " + Core.Sketch.SketchFormat.Mm(stack.BoltLengthMm) + " mm (agarre + " + Core.Sketch.SketchFormat.Mm(stack.LengthAdditionMm)
                    + " AISC 7-15, redondeado a " + Core.Sketch.SketchFormat.Mm(stack.LengthIncrementMm) + "); cabeza en la cara exterior de la placa cuchilla, lado +Z de la cartela.");
            }
            return string.Join(" ", parts);
        }

        /// <summary>Filas de la tabla con los valores actuales.</summary>
        public List<SpecField> Fields()
        {
            return SpecFieldCatalog.Build(Spec, LastNode);
        }

        /// <summary>Aplica un valor escrito en la tabla; si se acepta, actualiza el JSON y recalcula todo.</summary>
        public SpecEditResult ApplyEdit(string path, string? text)
        {
            SpecEditResult result = SpecFieldEditor.Apply(Spec, path, text);
            if (result.Ok)
            {
                RawJson = Spec.ToJson();
                Refresh();
                JsonLineLogger.Write(new { @event = "preview_edit", path, value = text, note = result.Note });
            }
            return result;
        }

        /// <summary>Vuelve a leer el archivo del disco (correcciones hechas desde el chat). Devuelve el error, o null si fue bien.</summary>
        public string? Reload()
        {
            if (string.IsNullOrWhiteSpace(FilePath)) return "La especificación no vino de un archivo: no hay nada que recargar.";
            if (!File.Exists(FilePath)) return "El archivo ya no existe: " + FilePath;
            try
            {
                string json = File.ReadAllText(FilePath!);
                ConnectionSpec? spec = ConnectionSpec.FromJson(json);
                if (spec == null) return "El archivo está vacío o no es una especificación.";
                Spec = spec;
                RawJson = json;
                Refresh();
                JsonLineLogger.Write(new { @event = "preview_reload", file = FilePath });
                return null;
            }
            catch (Exception error)
            {
                return "No se pudo leer el archivo: " + error.Message;
            }
        }

        /// <summary>Ruta donde se guarda la copia corregida: junto al original con sufijo <c>-corregido.json</c>, nunca encima.</summary>
        public string CorrectedPath()
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "conexion-corregido.json");
            }
            string folder = Path.GetDirectoryName(FilePath!) ?? "";
            string name = Path.GetFileNameWithoutExtension(FilePath!);
            if (name.EndsWith("-corregido", StringComparison.OrdinalIgnoreCase))
            {
                name = name.Substring(0, name.Length - "-corregido".Length);
            }
            return Path.Combine(folder, name + "-corregido.json");
        }

        /// <summary>Escribe el JSON corregido con sangría y devuelve la ruta.</summary>
        public string SaveCorrected()
        {
            string path = CorrectedPath();
            File.WriteAllText(path, Spec.ToJson(indented: true) + Environment.NewLine, new System.Text.UTF8Encoding(false));
            JsonLineLogger.Write(new { @event = "preview_saved", file = path });
            return path;
        }

        /// <summary>Token abreviado para la barra de estado.</summary>
        public string TokenSummary()
        {
            string? token = LastValidation?.ValidationToken;
            if (string.IsNullOrEmpty(token)) return "sin token";
            return "token " + token!.Substring(0, Math.Min(12, token.Length)) + "…";
        }
    }
}
