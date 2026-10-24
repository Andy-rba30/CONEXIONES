using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using System.Text.Json.Nodes;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Catalog;
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
        private readonly Dictionary<int, JsonObject> _attachmentStash = new Dictionary<int, JsonObject>();
        private PanelChoices? _choices;

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

        // ---- Fase 11: lo que el botón Conectar deja en la sesión para la cabecera del panel ----

        /// <summary>El nudo leído del modelo (cordón, barras, ángulos); lo pone Conectar para poder cambiar de plantilla.</summary>
        public TemplateNode? Node { get; set; }

        /// <summary>Las plantillas que encajan, de mejor a peor, y la frase de la propuesta; nulo si no se calculó.</summary>
        public TemplateProposal? Proposal { get; set; }

        /// <summary>Configuración del catálogo con la que se instancia al cambiar de plantilla.</summary>
        public CatalogConfig? CatalogConfig { get; set; }

        /// <summary>Verdadero si la especificación es un arranque (ninguna plantilla encajó).</summary>
        public bool IsStarter { get; set; }

        /// <summary>Texto de la cabecera del panel (propuesta o arranque); nulo = sin cabecera (archivo JSON de siempre).</summary>
        public string? HeaderText { get; set; }

        /// <summary>Aviso corto (por ejemplo, el nudo ya tiene una conexión del add-in); nulo si no hay.</summary>
        public string? HeaderWarning { get; set; }

        /// <summary>template_id de la plantilla aplicada ahora (para la miniatura de la cabecera), o nulo.</summary>
        public string? AppliedTemplateId => Spec?.Source?.TemplateId;

        /// <summary>Las listas de los desplegables (config\choices.json, con los diámetros filtrados por limits.json); se leen una vez.</summary>
        public PanelChoices Choices => _choices ??= PanelChoicesLoader.Load(_limits);

        /// <summary>La ruta del plano guardada en la especificación (<c>source.drawing_image</c>), o nula.</summary>
        public string? DrawingImagePath => string.IsNullOrWhiteSpace(Spec?.Source?.DrawingImage) ? null : Spec!.Source!.DrawingImage!.Trim();

        /// <summary>IDs del nudo (cordón y barras) tal como están en la especificación, para el Encargo para IA.</summary>
        public List<long> ElementIds
        {
            get
            {
                var ids = new List<long>();
                if (Spec?.Chord != null && Spec.Chord.ElementId > 0) ids.Add(Spec.Chord.ElementId);
                if (Spec?.Members != null) ids.AddRange(Spec.Members.Select(m => m.ElementId).Where(id => id > 0));
                if (Spec?.Node?.ElementIds != null) ids.AddRange(Spec.Node.ElementIds.Where(id => id > 0));
                return ids.Distinct().OrderBy(id => id).ToList();
            }
        }

        /// <summary>Escribe <c>source.drawing_image</c> (vacío o nulo = quitar la clave) sin tocar el resto del JSON.</summary>
        public bool TrySetDrawingImage(string? path, out string error)
        {
            if (!SpecEditor.TrySetValue(RawJson, "source.drawing_image", path ?? string.Empty, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>Un desplegable con rótulo: escribe el valor en mm y el rótulo a la vez (nunca deja un LABEL_VALUE_MISMATCH a medias).</summary>
        public bool TrySetLabeledValue(string pathMm, string pathLabel, string label, out string error)
        {
            if (!SpecEditor.TrySetLabeledValue(RawJson, pathMm, pathLabel, label, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>
        /// Cambia el tipo de unión de una barra creando el bloque que falta con los valores por defecto de <c>choices.json</c>;
        /// el bloque anterior se guarda en memoria (solo en esta sesión) para volver atrás con sus valores.
        /// </summary>
        public bool TrySetAttachmentType(int memberIndex, string type, out string error)
        {
            _attachmentStash.TryGetValue(memberIndex, out JsonObject? restored);
            if (!SpecEditor.TrySetAttachmentType(RawJson, memberIndex, type, Choices.Starter, restored, out string json, out JsonObject? previous, out error)) return false;
            if (previous != null) _attachmentStash[memberIndex] = previous;
            if (!string.Equals(json, RawJson, StringComparison.Ordinal)) SetJson(json);
            return true;
        }

        /// <summary>
        /// Fase 12 (decisión D4): el desplegable Tipo de Cambiar…: fuerza el arranque de otro tipo (cartela rectangular, soldadura
        /// directa o placa de extremo) sobre el nudo leído. Las plantillas del catálogo sirven de valores por defecto (D10).
        /// </summary>
        public bool TryApplyStarterType(string connectionType, IReadOnlyList<CatalogTemplate>? templates, out string error)
        {
            error = string.Empty;
            if (Node == null)
            {
                error = "No hay nudo leído del modelo en esta sesión: usa Abrir del catálogo.";
                return false;
            }
            try
            {
                StarterResult result = SpecStarter.ForType(connectionType, Node, Choices, templates);
                string? drawingImage = DrawingImagePath;
                SetJson(result.SpecJson);
                if (drawingImage != null && DrawingImagePath == null) TrySetDrawingImage(drawingImage, out _);
                IsStarter = true;
                HeaderText = result.HeaderText;
                return true;
            }
            catch (CatalogException ex)
            {
                error = ex.Error.Message + (ex.Error.Hint != null ? " " + ex.Error.Hint : "");
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>Cambiar… de la cabecera: vuelve a instanciar la especificación con otra candidata de la propuesta.</summary>
        public bool TryApplyCandidate(TemplateCandidate candidate, out string error)
        {
            error = string.Empty;
            if (candidate == null || Node == null)
            {
                error = "No hay nudo leído del modelo en esta sesión: usa Abrir del catálogo.";
                return false;
            }
            try
            {
                InstantiationResult result = TemplateInstantiator.Instantiate(candidate.Template, candidate.Match, Node, CatalogConfig ?? Core.Catalog.CatalogConfig.Default);
                string? drawingImage = DrawingImagePath;
                SetJson(result.SpecJson);
                // Un plano abierto a mano se conserva si la plantilla no trae el suyo.
                if (drawingImage != null && DrawingImagePath == null) TrySetDrawingImage(drawingImage, out _);
                IsStarter = false;
                HeaderText = "Plantilla aplicada: " + candidate.Describe() + "." + (Proposal != null && Proposal.Candidates.Count > 1 ? " Otras que encajan: " + (Proposal.Candidates.Count - 1) + "." : "");
                return true;
            }
            catch (CatalogException ex)
            {
                error = ex.Error.Message;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public string RawJson { get; private set; }
        public ConnectionSpec? Spec { get; private set; }
        public string? ParseError { get; private set; }
        public ValidationResult? Validation { get; private set; }
        public CoreSketch? Sketch { get; private set; }
        public List<SpecField> Fields { get; private set; }

        public string OutlineText => SpecEditor.FormatOutline(Spec?.Gusset?.Outline);

        // ---- Fase 13: cartela automática y la cadena de partir la diagonal ----

        /// <summary>Verdadero si el JSON lleva <c>gusset.outline.mode = "auto"</c> (los puntos se calculan, no se escriben).</summary>
        public bool IsAutoOutline => SpecEditor.IsAutoOutlineJson(RawJson);

        /// <summary>Los puntos calculados del contorno automático (los de la última validación; si no la hay, se calculan del modelo). Vacío si no es automático.</summary>
        public List<double[]> AutoOutlinePoints
        {
            get
            {
                if (Validation?.AutoOutline != null) return Validation.AutoOutline.PointsMm;
                if (Spec == null || !Core.Geometry2D.AutoOutline.IsAuto(Spec)) return new List<double[]>();
                try
                {
                    return Core.Geometry2D.AutoOutline.Resolve(Spec, new RevitModelFacts(_document), _limits.AutoOutlineMaxVertices)?.PointsMm ?? new List<double[]>();
                }
                catch (Exception)
                {
                    return new List<double[]>();
                }
            }
        }

        /// <summary>El contorno calculado como texto de solo lectura para el cuadro de Avanzado ("x; y" por línea).</summary>
        public string AutoOutlineText
        {
            get
            {
                List<double[]> points = AutoOutlinePoints;
                if (points.Count == 0) return "(cartela automática: todavía sin puntos calculados; valida primero)";
                var sb = new StringBuilder();
                foreach (double[] p in points) sb.Append(SpecEditor.FormatNumber(p[0])).Append("; ").Append(SpecEditor.FormatNumber(p[1])).AppendLine();
                return sb.ToString().TrimEnd();
            }
        }

        /// <summary>Contorno: <c>auto</c> (quita los puntos) o <c>polygon</c> (fija los calculados). Escribe por <see cref="SpecEditor"/>.</summary>
        public bool TrySetOutlineMode(string mode, out string error)
        {
            string wanted = (mode ?? string.Empty).Trim().ToLowerInvariant();
            IReadOnlyList<double[]>? points = wanted == Core.Geometry2D.AutoOutline.PolygonMode && IsAutoOutline ? AutoOutlinePoints : null;
            if (!SpecEditor.TrySetOutlineMode(RawJson, wanted, points, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>Convertir en polígono: los puntos calculados pasan a <c>points_mm</c> y el modo a <c>polygon</c>.</summary>
        public bool TryConvertAutoToPolygon(out string error)
        {
            if (!SpecEditor.ConvertAutoToPolygon(RawJson, AutoOutlinePoints, out string json, out error)) return false;
            SetJson(json);
            return true;
        }

        /// <summary>Las diagonales que la especificación manda partir (<c>direct_weld</c> con <c>splice_hint</c>): barra, cordón y distancia desde la cara.</summary>
        public List<SplitTarget> SplitTargets
        {
            get
            {
                var targets = new List<SplitTarget>();
                if (Spec?.Members == null || !Spec.IsType(DirectWeldValidator.TypeName)) return targets;
                foreach (MemberSpec member in Spec.Members)
                {
                    double? offset = member.SpliceHint?.OffsetFromChordFaceMm;
                    if (!offset.HasValue || offset.Value <= 0 || member.ElementId <= 0) continue;
                    long chord = member.ChordElementId ?? Spec.Chord?.ElementId ?? 0;
                    if (chord <= 0) continue;
                    targets.Add(new SplitTarget(member.ElementId, chord, offset.Value));
                }
                return targets;
            }
        }

        /// <summary>Verdadero si la ventana ofrece la casilla de partir la diagonal (D9).</summary>
        public bool OffersSplit => SplitTargets.Count > 0;

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
            // Fase 12: resumen por tipo.
            if (Spec.IsType("direct_weld"))
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0} · soldadura directa · cordón {1}{2} · {3} barra(s) cortadas contra la cara, soldadas en todo el contorno{4}",
                    Spec.Source?.Drawing ?? Spec.ConnectionType, Spec.Chord?.Profile ?? "?",
                    Spec.Node?.EccentricityMm is double ecc && ecc > 0.05 ? " · excentricidad " + ecc.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " mm" : "",
                    members,
                    Spec.Members != null && Spec.Members.Any(m => m.SpliceHint != null) ? " · empalme Detalle B a " + (Spec.Members.First(m => m.SpliceHint != null).SpliceHint!.OffsetFromChordFaceMm ?? 0).ToString("0", System.Globalization.CultureInfo.InvariantCulture) + " mm" : "");
            }
            if (Spec.IsType("end_plate"))
            {
                return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                    "{0} · placa de extremo (empalme) · tramos {1} y {2} · plancha {3} {4} × {5} con esquinas de {6} · {7} pernos {8} alrededor del tubo, tuerca a ambos lados",
                    Spec.Source?.Drawing ?? Spec.ConnectionType, Spec.Chord?.ElementId ?? 0, members > 0 ? Spec.Members![0].ElementId : 0,
                    SketchText.Thickness(Spec.Plate?.ThicknessLabel, Spec.Plate?.ThicknessMm), Spec.Plate?.WidthMm ?? 0, Spec.Plate?.HeightMm ?? 0, Spec.Plate?.CornerCutMm ?? 0,
                    Core.Geometry3D.BoltRing.Count(Spec.Bolts), Spec.Bolts?.DiameterLabel ?? "");
            }
            int knife = Spec.Members?.Count(m => string.Equals(m.Attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase)) ?? 0;
            int bolts = Spec.Members?.Where(m => m.Attachment?.Bolts != null).Sum(m => (m.Attachment!.Bolts!.Rows ?? 0) * (m.Attachment.Bolts.Columns ?? 0)) ?? 0;
            return string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "{0} · cordón {1} · cartela {2} · {3} barras, {4} placas cuchilla, {5} pernos",
                Spec.Source?.Drawing ?? Spec.ConnectionType, Spec.Chord?.Profile ?? "?",
                SketchText.Thickness(Spec.Gusset?.ThicknessLabel, Spec.Gusset?.ThicknessMm), members, knife, bolts);
        }
    }

    /// <summary>Fase 13 (D9): una diagonal que se parte antes de crear el nudo en K: barra, cordón desde cuya cara se mide y distancia (330).</summary>
    public sealed class SplitTarget
    {
        public SplitTarget(long elementId, long chordElementId, double offsetFromChordFaceMm)
        {
            ElementId = elementId;
            ChordElementId = chordElementId;
            OffsetFromChordFaceMm = offsetFromChordFaceMm;
        }

        public long ElementId { get; }
        public long ChordElementId { get; }
        public double OffsetFromChordFaceMm { get; }
    }
}
