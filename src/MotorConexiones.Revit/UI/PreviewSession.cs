using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Geometry3D;
using MotorConexiones.Core.Sketch;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Node;
using CoreSketch = MotorConexiones.Core.Sketch.Sketch;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Estado de la ventana de previsualización: el JSON actual (la única fuente de verdad), su especificación, la
    /// validación contra el modelo (la misma que ve la IA), el croquis y las filas de la tabla. No sabe de WPF: la
    /// ventana le pide cosas y dibuja lo que devuelve. No modifica el modelo; crear se hace fuera, en el comando.
    /// </summary>
    public sealed class PreviewSession
    {
        private readonly Document _document;
        private readonly LimitsConfig _limits;

        public PreviewSession(Document document, UIDocument? uiDocument, string filePath, string rawJson, bool isVirtualFile = false, string? title = null)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            UIDocument = uiDocument;
            FilePath = filePath ?? string.Empty;
            IsVirtualFile = isVirtualFile;
            Title = title;
            _limits = LimitsConfigLoader.Load();
            RawJson = rawJson ?? string.Empty;
            Fields = new List<SpecField>();
            SetJson(RawJson);
        }

        public UIDocument? UIDocument { get; }

        /// <summary>Documento sobre el que se valida (lo usan los botones del catálogo de la ventana).</summary>
        public Document Document => _document;

        /// <summary>Los límites con los que se valida (<c>config\limits.json</c> desplegado).</summary>
        public LimitsConfig Limits => _limits;

        /// <summary>Archivo abierto. Nunca se escribe encima: <see cref="Save"/> usa el sufijo <c>-corregido</c>.</summary>
        public string FilePath { get; }

        /// <summary>
        /// Verdadero cuando la especificación no viene de un archivo (plantilla del catálogo aplicada, Fase 7): no se puede
        /// recargar y <see cref="Save"/> escribe directamente en <see cref="FilePath"/> (en Documentos).
        /// </summary>
        public bool IsVirtualFile { get; }

        /// <summary>Texto de cabecera para un archivo virtual (por ejemplo la plantilla y el nudo).</summary>
        public string? Title { get; }

        public string RawJson { get; private set; }
        public ConnectionSpec? Spec { get; private set; }
        public string? ParseError { get; private set; }
        public ValidationResult? Validation { get; private set; }
        public CoreSketch? Sketch { get; private set; }
        public List<SpecField> Fields { get; private set; }

        public string OutlineText => SpecEditor.FormatOutline(Spec?.Gusset?.Outline);

        public bool CanCreate => Validation != null && Validation.IsValid && !string.IsNullOrEmpty(Validation.ValidationToken);

        public string TokenShort => Validation?.ValidationToken is string token && token.Length >= 16
            ? token.Substring(0, 16) + "…"
            : "(sin token: corrige los errores)";

        /// <summary>Vuelve a leer el archivo del disco (correcciones hechas desde el chat o un editor).</summary>
        public void Reload()
        {
            if (IsVirtualFile)
            {
                throw new InvalidOperationException("Esta especificación salió del catálogo y no viene de un archivo: no hay nada que recargar (usa Guardar JSON para escribirla en Documentos).");
            }
            SetJson(File.ReadAllText(FilePath));
        }

        /// <summary>Sustituye el JSON, vuelve a leer la especificación, valida y reconstruye el croquis.</summary>
        public void SetJson(string json)
        {
            RawJson = json ?? string.Empty;
            try
            {
                Spec = ConnectionSpec.FromJson(RawJson);
                ParseError = Spec == null ? "El archivo está vacío." : null;
            }
            catch (Exception ex)
            {
                Spec = null;
                ParseError = ex.Message;
            }
            Fields = Spec != null ? SpecEditor.ListFields(Spec) : new List<SpecField>();
            Refresh();
        }

        /// <summary>Valida y redibuja con el JSON actual.</summary>
        public void Refresh()
        {
            Validate();
            BuildSketch();
        }

        public bool TrySetField(string path, string text, out string error)
        {
            if (!SpecEditor.TrySetValue(RawJson, path, text, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        public bool TrySetOutline(string pointsText, out string error)
        {
            if (!SpecEditor.TrySetOutline(RawJson, pointsText, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>
        /// Ancho (X) o alto (Y) de la cartela desde su cota (doble clic en el croquis, ronda 6b): escribe width_mm/height_mm y
        /// estira el contorno en ese eje alrededor del punto de trabajo para que mida el valor nuevo.
        /// </summary>
        public bool TrySetGussetSize(bool width, double valueMm, out string error)
        {
            if (!SpecEditor.TrySetGussetSize(RawJson, width, valueMm, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>
        /// La misma validación que <c>conn_validate</c>: marco del nudo con <see cref="NodeInspector.ResolveNode"/>,
        /// hechos del modelo con <see cref="RevitModelFacts"/> y <see cref="SpecValidator"/> con <c>config\limits.json</c>.
        /// Los fallos del nudo que el validador no cubre (ejes que no se cortan, menos de dos barras) se añaden como errores.
        /// </summary>
        public ValidationResult Validate()
        {
            var result = new ValidationResult();
            if (Spec == null)
            {
                result.Errors.Add(new ApiError(ErrorCodes.SchemaInvalid,
                    "El archivo no contiene un JSON válido: " + (ParseError ?? "sin detalle"), "",
                    "Corrige el archivo en un editor y pulsa Recargar."));
                Validation = result;
                return result;
            }

            NodeFrame? frame = null;
            ApiError? nodeError = null;
            try
            {
                frame = NodeInspector.ResolveNode(_document, Spec).Frame;
            }
            catch (NodeInspectionException ex)
            {
                nodeError = ex.Error;
            }
            catch (Exception ex)
            {
                nodeError = new ApiError(ErrorCodes.InternalError, "No se pudo leer el nudo del modelo: " + ex.Message, "node", "Revisa los element_ids.");
            }

            var facts = new RevitModelFacts(_document, frame);
            result = SpecValidator.Validate(RawJson, Spec, facts, _limits);

            if (nodeError != null && !result.Errors.Any(e => e.Code == nodeError.Code))
            {
                result.Errors.Add(nodeError);
                result.ValidationToken = null;
            }

            Validation = result;
            return result;
        }

        /// <summary>Croquis en Core con los datos del nudo leídos del modelo (direcciones y anchos de perfil).</summary>
        public CoreSketch BuildSketch()
        {
            if (Spec == null)
            {
                var empty = new CoreSketch();
                empty.Notes.Add("Sin croquis: el JSON no se puede leer.");
                Sketch = empty;
                return empty;
            }

            SketchNodeInfo nodeInfo;
            try
            {
                nodeInfo = SketchNodeInfo.FromModelFacts(Spec, new RevitModelFacts(_document, null));
            }
            catch (Exception ex)
            {
                nodeInfo = SketchNodeInfo.FromSpecAngles(Spec, "No se pudo leer el nudo del modelo (" + ex.Message + "): direcciones aproximadas.");
            }

            try
            {
                // Los mismos límites que la validación: la longitud de perno del croquis es la que se creará.
                Sketch = SketchBuilder.Build(Spec, nodeInfo, null, _limits);
            }
            catch (Exception ex)
            {
                var failed = new CoreSketch();
                failed.Notes.Add("El croquis no se pudo construir: " + ex.Message);
                Sketch = failed;
            }
            return Sketch;
        }

        /// <summary>Ruta donde se guarda el JSON corregido: junto al original, con sufijo <c>-corregido.json</c>, nunca encima.</summary>
        public string GetCorrectedPath()
        {
            string directory = Path.GetDirectoryName(FilePath) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(FilePath);
            const string suffix = "-corregido";
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) name = name.Substring(0, name.Length - suffix.Length);
            string target = Path.Combine(directory, name + suffix + ".json");
            if (string.Equals(Path.GetFullPath(target), Path.GetFullPath(FilePath), StringComparison.OrdinalIgnoreCase))
            {
                target = Path.Combine(directory, name + suffix + "-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".json");
            }
            return target;
        }

        /// <summary>Escribe el JSON actual con sangría (UTF-8 sin BOM) en <see cref="GetCorrectedPath"/> y devuelve la ruta.</summary>
        public string Save()
        {
            string target = IsVirtualFile ? FilePath : GetCorrectedPath();
            string? directory = Path.GetDirectoryName(target);
            if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
            File.WriteAllText(target, SpecEditor.ToPrettyJson(RawJson), new UTF8Encoding(false));
            return target;
        }

        /// <summary>Resumen corto para la barra de estado: piezas que se crearían.</summary>
        public string Summary()
        {
            if (Spec == null) return "JSON no legible.";
            int members = Spec.Members?.Count ?? 0;
            int knife = Spec.Members?.Count(m => string.Equals(m.Attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase)) ?? 0;
            int bolts = Spec.Members?.Where(m => m.Attachment?.Bolts != null).Sum(m => (m.Attachment!.Bolts!.Rows ?? 0) * (m.Attachment.Bolts.Columns ?? 0)) ?? 0;
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0} · cordón {1} · cartela {2} · {3} barras, {4} placas cuchilla, {5} pernos",
                Spec.Source?.Drawing ?? Spec.ConnectionType, Spec.Chord?.Profile ?? "?",
                SketchText.Thickness(Spec.Gusset?.ThicknessLabel, Spec.Gusset?.ThicknessMm), members, knife, bolts);
        }
    }
}
