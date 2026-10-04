using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Threading;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Editing;
using MotorConexiones.Core.Validation;
using MotorConexiones.Revit.Logging;
using SpecValidationResult = MotorConexiones.Core.Validation.ValidationResult;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana "Previsualización de conexión" (Fase 6): croquis 2D con cotas a la izquierda, tabla editable a la
    /// derecha y errores, avisos, token y botones abajo. Solo dibuja y edita lo que le da <see cref="PreviewSession"/>;
    /// no toca el modelo: si la persona pulsa Crear, la ventana se cierra con <c>DialogResult = true</c> y el comando
    /// de la cinta crea la conexión con el servicio de siempre. Se abre modal desde el comando externo (hilo de Revit).
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private readonly PreviewSession _session;
        private readonly ObservableCollection<FieldRow> _rows = new ObservableCollection<FieldRow>();
        private readonly ObservableCollection<MessageRow> _messages = new ObservableCollection<MessageRow>();
        private bool _syncingRows;

        public PreviewWindow(PreviewSession session, IntPtr ownerHandle)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            InitializeComponent();
            if (ownerHandle != IntPtr.Zero)
            {
                new WindowInteropHelper(this).Owner = ownerHandle;
            }

            var view = new ListCollectionView(_rows);
            view.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FieldRow.Group)));
            FieldsGrid.ItemsSource = view;
            MessagesGrid.ItemsSource = _messages;

            RefreshAll("Especificación leída" + (_session.FilePath != null ? " de " + _session.FilePath : "") + ".");
        }

        /// <summary>Recalcula todo desde la sesión y vuelve a pintar croquis, tabla, mensajes y estado.</summary>
        private void RefreshAll(string? statusNote)
        {
            try
            {
                _session.Refresh();
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "preview_refresh_failed", error = error.ToString() });
                StatusText.Text = "No se pudo recalcular la previsualización: " + error.Message;
                return;
            }

            SyncRows();
            Sketch.Model = _session.LastSketch;
            SketchNotes.Text = _session.LastSketch == null ? "" : string.Join(" ", _session.LastSketch.Notes);
            SketchNotes.Visibility = string.IsNullOrEmpty(SketchNotes.Text) ? Visibility.Collapsed : Visibility.Visible;

            _messages.Clear();
            foreach (ApiError nodeError in _session.NodeErrors) _messages.Add(new MessageRow("Nudo", nodeError));
            SpecValidationResult? validation = _session.LastValidation;
            if (validation != null)
            {
                foreach (ApiError error in validation.Errors) _messages.Add(new MessageRow("Error", error));
                foreach (ApiError warning in validation.Warnings) _messages.Add(new MessageRow("Aviso", warning));
            }

            int errors = validation?.Errors.Count ?? 0;
            int warnings = validation?.Warnings.Count ?? 0;
            CreateButton.IsEnabled = _session.CanCreate;
            ValidationTitle.Text = "Validación: " + errors + (errors == 1 ? " error" : " errores") + ", " + warnings + (warnings == 1 ? " aviso" : " avisos") + " · " + _session.TokenSummary();
            StatusText.Text = _session.CanCreate
                ? "Validación en verde: puedes crear la conexión."
                : "Corrige los errores de la lista para habilitar Crear.";
            DetailText.Text = statusNote ?? "";
            Title = "MotorConexiones · Previsualización de conexión" + (_session.Spec.Source?.Drawing != null ? " · " + _session.Spec.Source.Drawing : "");
        }

        /// <summary>Actualiza los valores en sitio si las filas son las mismas; si cambió la estructura, reconstruye la tabla.</summary>
        private void SyncRows()
        {
            List<SpecField> fields = _session.Fields();
            _syncingRows = true;
            try
            {
                bool sameShape = fields.Count == _rows.Count;
                if (sameShape)
                {
                    for (int i = 0; i < fields.Count; i++)
                    {
                        if (fields[i].Path != _rows[i].Path)
                        {
                            sameShape = false;
                            break;
                        }
                    }
                }

                if (sameShape)
                {
                    for (int i = 0; i < fields.Count; i++)
                    {
                        _rows[i].Value = fields[i].Value;
                    }
                }
                else
                {
                    _rows.Clear();
                    foreach (SpecField field in fields) _rows.Add(new FieldRow(field));
                }
            }
            finally
            {
                _syncingRows = false;
            }
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            if (e.Row.Item is FieldRow row && row.IsReadOnly)
            {
                e.Cancel = true;
                DetailText.Text = "'" + row.Label + "' es un dato leído del modelo o de la especificación: no se edita aquí.";
            }
        }

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_syncingRows || e.EditAction != DataGridEditAction.Commit) return;
            if (e.Row.Item is not FieldRow row) return;
            string previous = row.Value;
            string text = (e.EditingElement as TextBox)?.Text ?? previous;
            if (text == previous) return;

            // Se aplica después de que el DataGrid termine su propio commit, para no recargar la tabla a mitad de la edición.
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => ApplyEdit(row, previous, text)));
        }

        private void ApplyEdit(FieldRow row, string previous, string text)
        {
            SpecEditResult result;
            try
            {
                result = _session.ApplyEdit(row.Path, text);
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "preview_edit_failed", path = row.Path, error = error.ToString() });
                result = SpecEditResult.Failure("Error inesperado al aplicar el valor: " + error.Message);
            }

            if (!result.Ok)
            {
                _syncingRows = true;
                row.Value = previous;
                _syncingRows = false;
                DetailText.Text = row.Label + ": " + result.Error;
                StatusText.Text = "El valor no se aplicó.";
                return;
            }

            RefreshAll(result.Note ?? ("'" + row.Label + "' = " + text + ". Croquis y validación actualizados."));
        }

        private void OnFitClick(object sender, RoutedEventArgs e)
        {
            Sketch.Fit();
        }

        private void OnReloadClick(object sender, RoutedEventArgs e)
        {
            string? error = _session.Reload();
            if (error != null)
            {
                DetailText.Text = error;
                StatusText.Text = "No se recargó.";
                return;
            }
            RefreshAll("Recargado desde " + _session.FilePath + ".");
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = _session.SaveCorrected();
                DetailText.Text = "Guardado en " + path + " (el archivo original no cambia).";
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "preview_save_failed", error = error.ToString() });
                DetailText.Text = "No se pudo guardar: " + error.Message;
            }
        }

        private void OnValidateClick(object sender, RoutedEventArgs e)
        {
            RefreshAll("Validación repetida contra el modelo.");
        }

        private void OnCreateClick(object sender, RoutedEventArgs e)
        {
            if (!_session.CanCreate)
            {
                DetailText.Text = "La validación no está en verde: no se puede crear.";
                return;
            }
            // Fijar DialogResult cierra la ventana modal; no hace falta Close().
            DialogResult = true;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        /// <summary>Resumen corto de lo que se va a crear (para el diálogo final del comando).</summary>
        public static string Summary(ConnectionSpec spec)
        {
            int members = spec.Members?.Count ?? 0;
            int knifePlates = spec.Members?.Count(m => string.Equals(m.Attachment?.Type, "bolted_knife_plate", StringComparison.OrdinalIgnoreCase)) ?? 0;
            int bolts = spec.Members?.Where(m => m.Attachment?.Bolts != null).Sum(m => m.Attachment!.Bolts!.Rows.GetValueOrDefault(0) * m.Attachment!.Bolts!.Columns.GetValueOrDefault(0)) ?? 0;
            return "Barras: " + members + " · Placas cuchilla: " + knifePlates + " · Pernos: " + bolts;
        }
    }
}
