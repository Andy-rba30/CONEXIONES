using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using SpecValidationResult = MotorConexiones.Core.Validation.ValidationResult;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana "Previsualización de conexión" del botón de la cinta (el único camino del add-in con ventanas).
    /// Izquierda: croquis 2D con cotas. Derecha: tabla editable y contorno. Abajo: errores y avisos del validador,
    /// token abreviado y botones Recargar, Guardar JSON, Validar, Crear y Cancelar. No modifica el modelo: si la persona
    /// pulsa Crear, <see cref="CreateRequested"/> queda en verdadero y el comando crea al cerrarse la ventana.
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        private readonly PreviewSession _session;
        private readonly ObservableCollection<FieldRow> _rows = new ObservableCollection<FieldRow>();
        private readonly ObservableCollection<IssueRow> _issues = new ObservableCollection<IssueRow>();
        private bool _refreshing;

        public PreviewWindow(PreviewSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            InitializeComponent();
            FieldsGrid.ItemsSource = _rows;
            IssuesList.ItemsSource = _issues;
            FileText.Text = "Archivo: " + _session.FilePath;
            Loaded += (_, _) =>
            {
                RefreshAll();
                Canvas.Fit();
            };
        }

        /// <summary>Verdadero si la persona pulsó Crear con la validación en verde.</summary>
        public bool CreateRequested { get; private set; }

        private void RefreshAll(string? statusOverride = null, bool statusIsError = false)
        {
            _refreshing = true;
            try
            {
                _rows.Clear();
                foreach (SpecField field in _session.Fields)
                {
                    _rows.Add(new FieldRow(field));
                }
                OutlineBox.Text = _session.OutlineText;
                SummaryText.Text = _session.Summary();
                Canvas.Sketch = _session.Sketch;
                SketchNotesText.Text = _session.Sketch != null && _session.Sketch.Notes.Count > 0
                    ? string.Join(" · ", _session.Sketch.Notes)
                    : string.Empty;
                UpdateStatus(statusOverride, statusIsError);
            }
            finally
            {
                _refreshing = false;
            }
        }

        private void UpdateStatus(string? statusOverride = null, bool statusIsError = false)
        {
            SpecValidationResult? validation = _session.Validation;
            _issues.Clear();
            if (validation != null)
            {
                foreach (ApiError error in validation.Errors) _issues.Add(new IssueRow("Error", error));
                foreach (ApiError warning in validation.Warnings) _issues.Add(new IssueRow("Aviso", warning));
            }

            int errors = validation?.Errors.Count ?? 0;
            int warnings = validation?.Warnings.Count ?? 0;
            if (statusOverride != null)
            {
                StatusText.Text = statusOverride;
                StatusText.Foreground = statusIsError ? ErrorBrush : InfoBrush;
            }
            else if (_session.CanCreate)
            {
                StatusText.Text = "Validación correcta" + (warnings > 0 ? " con " + warnings + " aviso(s). Revísalos antes de crear." : ". Puedes crear la conexión.");
                StatusText.Foreground = OkBrush;
            }
            else
            {
                StatusText.Text = errors + " error(es) y " + warnings + " aviso(s). Corrige los errores en la tabla (o en el archivo y pulsa Recargar) para poder crear.";
                StatusText.Foreground = ErrorBrush;
            }

            TokenText.Text = "validation_token: " + _session.TokenShort;
            CreateButton.IsEnabled = _session.CanCreate;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is FieldRow row && !row.IsEditable)
            {
                e.Cancel = true;
            }
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_refreshing || e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not FieldRow row) return;
            string text = (e.EditingElement as TextBox)?.Text ?? row.Value;
            if (string.Equals(text, row.OriginalValue, StringComparison.Ordinal)) return;

            // Fuera del evento de edición para que el DataGrid termine su propio ciclo antes de reconstruir las filas.
            Dispatcher.BeginInvoke(new Action(() => ApplyEdit(row, text)));
        }

        private void ApplyEdit(FieldRow row, string text)
        {
            if (!_session.TrySetField(row.Path, text, out string error))
            {
                row.Value = row.OriginalValue;
                UpdateStatus("No se aplicó el cambio en '" + row.Label + "': " + error, statusIsError: true);
                JsonLineLogger.Write(new { @event = "ribbon_preview_edit_rejected", path = row.Path, value = text, error });
                return;
            }
            JsonLineLogger.Write(new { @event = "ribbon_preview_edit", path = row.Path, value = text, is_valid = _session.CanCreate });
            RefreshAll();
            Canvas.HighlightPath = row.Path;
        }

        private void OnFieldSelected(object sender, SelectionChangedEventArgs e)
        {
            Canvas.HighlightPath = (FieldsGrid.SelectedItem as FieldRow)?.Path;
        }

        private void OnApplyOutline(object sender, RoutedEventArgs e)
        {
            if (!_session.TrySetOutline(OutlineBox.Text, out string error))
            {
                UpdateStatus("El contorno no se aplicó: " + error, statusIsError: true);
                return;
            }
            RefreshAll();
            Canvas.HighlightPath = "gusset.width_mm";
        }

        private void OnFit(object sender, RoutedEventArgs e)
        {
            Canvas.Fit();
        }

        private void OnReload(object sender, RoutedEventArgs e)
        {
            try
            {
                _session.Reload();
                RefreshAll("Archivo recargado del disco: " + _session.FilePath);
                Canvas.Fit();
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo recargar el archivo: " + ex.Message, statusIsError: true);
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = _session.Save();
                UpdateStatus("JSON guardado en " + path + " (el original no se ha tocado).");
                JsonLineLogger.Write(new { @event = "ribbon_preview_saved", path });
            }
            catch (Exception ex)
            {
                UpdateStatus("No se pudo guardar el JSON: " + ex.Message, statusIsError: true);
            }
        }

        private void OnValidate(object sender, RoutedEventArgs e)
        {
            _session.Refresh();
            RefreshAll();
        }

        private void OnCreate(object sender, RoutedEventArgs e)
        {
            // Se vuelve a validar justo antes de crear: el token que se usa es el recién calculado.
            _session.Refresh();
            RefreshAll();
            if (!_session.CanCreate) return;
            CreateRequested = true;
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            CreateRequested = false;
            DialogResult = false;
            Close();
        }

        /// <summary>Fila de la tabla; <see cref="Value"/> es lo único que se edita.</summary>
        public sealed class FieldRow : INotifyPropertyChanged
        {
            private string _value;

            public FieldRow(SpecField field)
            {
                Section = field.Section;
                Label = field.Hint != null ? field.Label + "  (" + field.Hint + ")" : field.Label;
                Path = field.Path;
                IsEditable = field.IsEditable;
                OriginalValue = field.Value;
                _value = field.Value;
            }

            public string Section { get; }
            public string Label { get; }
            public string Path { get; }
            public bool IsEditable { get; }
            public string OriginalValue { get; }

            public string Value
            {
                get => _value;
                set
                {
                    if (_value == value) return;
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        /// <summary>Fila de la lista de errores y avisos (los mismos que ve la IA).</summary>
        public sealed class IssueRow
        {
            public IssueRow(string severity, ApiError error)
            {
                Severity = severity;
                Code = error.Code;
                Path = error.Path ?? string.Empty;
                Message = error.Message;
                Hint = error.Hint ?? string.Empty;
            }

            public string Severity { get; }
            public string Code { get; }
            public string Path { get; }
            public string Message { get; }
            public string Hint { get; }
        }
    }
}
