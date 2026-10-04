using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Catalog;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana "Catálogo" del botón de la cinta (Fase 7): lista las plantillas con búsqueda, aplica una a la selección
    /// (abre <see cref="PreviewWindow"/> con la especificación instanciada; si allí se pulsa Crear, el comando crea al
    /// cerrarse esta ventana), guarda una plantilla desde una conexión del modelo y borra plantillas. Con
    /// <c>pickOnly</c> solo elige una plantilla (botón "Abrir del catálogo" de la previsualización).
    /// </summary>
    public partial class CatalogWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x33, 0x33, 0x33));

        private readonly Document _document;
        private readonly UIApplication? _uiApplication;
        private readonly bool _pickOnly;
        private readonly ObservableCollection<TemplateRow> _rows = new ObservableCollection<TemplateRow>();
        private List<CatalogEntry> _entries = new List<CatalogEntry>();
        private CatalogConfig _config = CatalogConfig.Default;
        private CatalogStore? _store;

        public CatalogWindow(Document document, UIApplication? uiApplication, bool pickOnly)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _uiApplication = uiApplication;
            _pickOnly = pickOnly;
            InitializeComponent();
            TemplatesGrid.ItemsSource = _rows;
            if (pickOnly)
            {
                Title = "MotorConexiones: elegir una plantilla del catálogo";
                ApplyButton.Content = "Elegir esta plantilla";
                ApplyButton.ToolTip = "Aplica la plantilla elegida al nudo de la ventana de previsualización.";
                SaveFromConnectionButton.Visibility = System.Windows.Visibility.Collapsed;
                IntroText.Text = "Elige la plantilla que quieres aplicar al nudo abierto en la previsualización. Nada se crea: solo se sustituye la especificación de la ventana.";
            }
            Loaded += (_, _) => LoadRows();
        }

        /// <summary>Plantilla elegida en modo <c>pickOnly</c>.</summary>
        public CatalogTemplate? SelectedTemplate { get; private set; }

        /// <summary>Si la persona pulsó Crear en la previsualización de una plantilla aplicada: el comando crea con esta sesión.</summary>
        public PreviewSession? PendingSession { get; private set; }

        public string? PendingTemplateId { get; private set; }

        // ---- lista ----

        private void LoadRows(string? status = null, bool isError = false)
        {
            _rows.Clear();
            _entries = new List<CatalogEntry>();
            var warnings = new List<ApiError>();
            try
            {
                _config = CatalogConfigLoader.Load();
                _store = CatalogConfigLoader.OpenStore(_config);
                _entries = _store.List(warnings).ToList();
                FolderText.Text = _store.Folder;
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo leer el catálogo: " + ex.Message, ErrorBrush);
                return;
            }
            ApplyFilter();

            if (status != null)
            {
                SetStatus(status, isError ? ErrorBrush : OkBrush);
            }
            else
            {
                string text = _entries.Count == 0
                    ? "No hay plantillas en " + _store.Folder + ". Guarda una desde una conexión del modelo o desde la ventana de previsualización."
                    : _entries.Count + " plantilla(s) en " + _store.Folder + ".";
                if (warnings.Count > 0) text += " " + warnings.Count + " archivo(s) ilegible(s): " + string.Join("; ", warnings.Select(w => w.Message));
                SetStatus(text, warnings.Count > 0 ? ErrorBrush : InfoBrush);
            }
        }

        private void ApplyFilter()
        {
            string filter = (SearchBox.Text ?? string.Empty).Trim();
            _rows.Clear();
            foreach (CatalogEntry entry in _entries)
            {
                if (filter.Length == 0 || Matches(entry, filter)) _rows.Add(new TemplateRow(entry));
            }
            UpdateButtons();
        }

        private static bool Matches(CatalogEntry entry, string filter)
        {
            string haystack = string.Join(" ", entry.Name, entry.Description ?? "", string.Join(" ", entry.Tags), entry.Pattern, entry.ChordProfile ?? "", entry.OriginDrawing ?? "", entry.TemplateId);
            return haystack.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void SetStatus(string text, Brush brush)
        {
            StatusText.Text = text;
            StatusText.Foreground = brush;
        }

        private void UpdateButtons()
        {
            bool selected = TemplatesGrid.SelectedItem is TemplateRow;
            ApplyButton.IsEnabled = selected;
            DeleteButton.IsEnabled = selected && !_pickOnly;
        }

        private TemplateRow? SelectedRow => TemplatesGrid.SelectedItem as TemplateRow;

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateButtons();
            TemplateRow? row = SelectedRow;
            DetailText.Text = row == null
                ? "Selecciona una plantilla para ver su descripción, su origen y su archivo."
                : (string.IsNullOrEmpty(row.Description) ? "(sin descripción)" : row.Description)
                  + "\n" + row.Pattern
                  + "\nOrigen: " + (row.Origin.Length > 0 ? row.Origin : "(desconocido)")
                  + "\ntemplate_id: " + row.TemplateId + "  ·  " + row.File;
        }

        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedRow != null) OnApply(sender, e);
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadRows();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ---- aplicar ----

        private void OnApply(object sender, RoutedEventArgs e)
        {
            TemplateRow? row = SelectedRow;
            if (row == null || _store == null) return;

            CatalogTemplate? template;
            try
            {
                template = _store.Get(row.TemplateId);
            }
            catch (CatalogException ex)
            {
                SetStatus("No se pudo leer la plantilla: " + ex.Error.Message, ErrorBrush);
                return;
            }
            if (template == null)
            {
                LoadRows("La plantilla '" + row.Name + "' ya no está en el catálogo.", isError: true);
                return;
            }

            if (_pickOnly)
            {
                SelectedTemplate = template;
                DialogResult = true;
                Close();
                return;
            }

            UIDocument? uidoc = _uiApplication?.ActiveUIDocument;
            List<long> ids = uidoc?.Selection?.GetElementIds()?.Select(id => id.Value).OrderBy(id => id).ToList() ?? new List<long>();
            if (ids.Count < 2)
            {
                SetStatus("Selecciona en Revit el cordón y todas las barras que llegan al nudo (al menos 2 elementos de armazón estructural) y vuelve a pulsar Aplicar.", ErrorBrush);
                return;
            }

            CatalogApplyResult result;
            try
            {
                result = CatalogService.Apply(_document, template, ids, null, null, _config);
            }
            catch (CatalogException ex)
            {
                SetStatus("La plantilla '" + template.Name + "' no se pudo aplicar: " + ex.Error.Message + (ex.Error.Hint != null ? " " + ex.Error.Hint : ""), ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_catalog_apply_failed", template_id = template.TemplateId, element_ids = ids, code = ex.Error.Code, error = ex.Error.Message });
                return;
            }
            catch (Exception ex)
            {
                SetStatus("La plantilla no se pudo aplicar: " + ex.Message, ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_catalog_apply_failed", template_id = template.TemplateId, element_ids = ids, error = ex.ToString() });
                return;
            }

            string summary = string.Format(CultureInfo.InvariantCulture,
                "Plantilla '{0}' casada en orientación {1} (desvío máximo {2:0.0}°): {3}.",
                template.Name, result.Match.OrientationName, result.Match.MaxDeviationDeg, result.Match.Describe());
            JsonLineLogger.Write(new
            {
                @event = "ribbon_catalog_apply",
                template_id = template.TemplateId,
                element_ids = ids,
                orientation = result.Match.OrientationName,
                is_valid = result.Validation.IsValid,
                error_codes = result.Validation.Result.Errors.Select(err => err.Code).ToArray(),
            });

            string virtualPath = CatalogService.VirtualFilePath(template, result.Node.ChordElementId);
            string title = "Plantilla '" + template.Name + "' aplicada al nudo del cordón " + result.Node.ChordElementId + " (" + result.Match.OrientationName + ")";
            var session = new PreviewSession(_document, uidoc, virtualPath, result.Instantiation.SpecJson, isVirtualFile: true, title: title);
            var preview = new PreviewWindow(session) { Owner = this };
            preview.ShowDialog();

            if (preview.CreateRequested && session.CanCreate)
            {
                PendingSession = session;
                PendingTemplateId = template.TemplateId;
                DialogResult = true;
                Close();
                return;
            }
            SetStatus(summary + " No se creó nada.", InfoBrush);
        }

        // ---- guardar desde una conexión del modelo ----

        private void OnSaveFromConnection(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<ConnectionRecord> records;
            try
            {
                records = ConnectionStorageManager.ListConnections(_document);
            }
            catch (Exception ex)
            {
                SetStatus("No se pudieron leer las conexiones del modelo: " + ex.Message, ErrorBrush);
                return;
            }
            if (records.Count == 0)
            {
                SetStatus("El modelo no tiene conexiones creadas por MotorConexiones: crea una primero (botón Ejecutar especificación JSON) o guarda la plantilla desde la ventana de previsualización.", ErrorBrush);
                return;
            }

            var dialog = new SaveTemplateDialog(_config, "Nudo típico", records) { Owner = this };
            if (dialog.ShowDialog() != true || dialog.SelectedConnectionId == null || _store == null) return;

            try
            {
                if (!CatalogService.TryBuildFromConnection(_document, dialog.SelectedConnectionId, dialog.Metadata, _config, out CatalogTemplate? template, out ModelValidation? validation) || template == null)
                {
                    string detail = validation == null ? "" : string.Join("; ", validation.Result.Errors.Select(err => err.Code + " " + err.Message));
                    SetStatus("La especificación guardada de esa conexión ya no valida contra el modelo: " + detail, ErrorBrush);
                    return;
                }
                string? file = SaveAskingToOverwrite(_store, template, dialog.Overwrite, CatalogConfigLoader.ResolveSharedFolder(_config), dialog.CopyToShared, out string? sharedFile);
                if (file == null)
                {
                    SetStatus("No se guardó la plantilla.", InfoBrush);
                    return;
                }
                JsonLineLogger.Write(new { @event = "ribbon_catalog_save", template_id = template.TemplateId, name = template.Name, connection_id = dialog.SelectedConnectionId, file, shared_file = sharedFile });
                LoadRows("Plantilla '" + template.Name + "' guardada en " + file + (sharedFile != null ? " y copiada a " + sharedFile : "") + ".");
                SelectRow(template.TemplateId);
            }
            catch (CatalogException ex)
            {
                SetStatus("No se pudo guardar la plantilla: " + ex.Error.Message + (ex.Error.Hint != null ? " " + ex.Error.Hint : ""), ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_catalog_save_failed", code = ex.Error.Code, error = ex.Error.Message });
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo guardar la plantilla: " + ex.Message, ErrorBrush);
                JsonLineLogger.Write(new { @event = "ribbon_catalog_save_failed", error = ex.ToString() });
            }
        }

        /// <summary>
        /// Guarda y, si ya existe una plantilla con ese nombre, pregunta si se sustituye. Devuelve la ruta o nulo si la
        /// persona dijo que no. La usan esta ventana y la de previsualización.
        /// </summary>
        internal static string? SaveAskingToOverwrite(CatalogStore store, CatalogTemplate template, bool overwrite, string? sharedFolder, bool copyToShared, out string? sharedFile)
        {
            try
            {
                return CatalogService.Save(store, template, overwrite, sharedFolder, copyToShared, out sharedFile);
            }
            catch (CatalogException ex) when (ex.Error.Code == ErrorCodes.TemplateExists)
            {
                var confirm = new TaskDialog("MotorConexiones - Plantilla existente")
                {
                    MainInstruction = "Ya existe una plantilla llamada '" + template.Name + "'. ¿Sustituirla?",
                    MainContent = ex.Error.Message + "\nSi la sustituyes conserva su template_id; si no, cancela y elige otro nombre.",
                    CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
                    DefaultButton = TaskDialogResult.No,
                };
                if (confirm.Show() != TaskDialogResult.Yes)
                {
                    sharedFile = null;
                    return null;
                }
                return CatalogService.Save(store, template, true, sharedFolder, copyToShared, out sharedFile);
            }
        }

        private void SelectRow(string templateId)
        {
            TemplateRow? row = _rows.FirstOrDefault(r => string.Equals(r.TemplateId, templateId, StringComparison.OrdinalIgnoreCase));
            if (row == null) return;
            TemplatesGrid.SelectedItem = row;
            TemplatesGrid.ScrollIntoView(row);
        }

        // ---- eliminar ----

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            TemplateRow? row = SelectedRow;
            if (row == null || _store == null) return;

            var confirm = new TaskDialog("MotorConexiones - Confirmar borrado")
            {
                MainInstruction = "¿Eliminar la plantilla '" + row.Name + "' del catálogo?",
                MainContent = "Se borra el archivo " + row.File + ". Las conexiones ya creadas con ella no cambian.",
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
                DefaultButton = TaskDialogResult.No,
            };
            if (confirm.Show() != TaskDialogResult.Yes) return;

            try
            {
                bool deleted = _store.Delete(row.TemplateId, out string path);
                JsonLineLogger.Write(new { @event = "ribbon_catalog_delete", template_id = row.TemplateId, name = row.Name, file = path, deleted });
                LoadRows(deleted ? "Plantilla '" + row.Name + "' eliminada (" + path + ")." : "La plantilla ya no existía.", isError: !deleted);
            }
            catch (Exception ex)
            {
                SetStatus("No se pudo eliminar la plantilla: " + ex.Message, ErrorBrush);
            }
        }

        /// <summary>Fila de la lista (solo lectura).</summary>
        public sealed class TemplateRow
        {
            public TemplateRow(CatalogEntry entry)
            {
                TemplateId = entry.TemplateId;
                Name = entry.Name;
                Description = entry.Description ?? string.Empty;
                MembersCount = entry.MembersCount;
                ChordProfile = entry.ChordProfile ?? string.Empty;
                Pattern = entry.Pattern;
                Tags = string.Join(", ", entry.Tags);
                File = entry.File;
                Origin = string.Join(" · ", new[] { entry.OriginDrawing, entry.OriginDocument }.Where(t => !string.IsNullOrWhiteSpace(t)));
                CreatedLocal = entry.CreatedUtc;
                if (DateTime.TryParse(entry.CreatedUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime utc))
                {
                    CreatedLocal = utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
            }

            public string TemplateId { get; }
            public string Name { get; }
            public string Description { get; }
            public int MembersCount { get; }
            public string ChordProfile { get; }
            public string Pattern { get; }
            public string Tags { get; }
            public string CreatedLocal { get; }
            public string File { get; }
            public string Origin { get; }
        }
    }
}
