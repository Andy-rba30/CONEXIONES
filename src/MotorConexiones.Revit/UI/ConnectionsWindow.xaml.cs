using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Services;
using MotorConexiones.Revit.Storage;
using MotorConexiones.Revit.Transactions;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Ventana "Conexiones del modelo" del segundo botón de la cinta: lista las conexiones del add-in guardadas en
    /// Extensible Storage (como <c>conn_list</c>) y borra una con confirmación (como <c>conn_delete</c>): los mismos
    /// servicios, una operación = un <c>TransactionGroup</c>, sin tocar <c>Bridge</c>.
    /// </summary>
    public partial class ConnectionsWindow : Window
    {
        private static readonly Brush OkBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x1E, 0x7E, 0x34));
        private static readonly Brush ErrorBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xB0, 0x1E, 0x1E));
        private static readonly Brush InfoBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x33, 0x33, 0x33));

        private readonly Document _document;
        private readonly UIApplication _uiApplication;
        private readonly ObservableCollection<ConnectionRow> _rows = new ObservableCollection<ConnectionRow>();

        public ConnectionsWindow(Document document, UIApplication uiApplication)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _uiApplication = uiApplication ?? throw new ArgumentNullException(nameof(uiApplication));
            InitializeComponent();
            ConnectionsGrid.ItemsSource = _rows;
            Loaded += (_, _) => LoadRows();
        }

        /// <summary>Cuántas conexiones se borraron desde esta ventana (para el registro del comando).</summary>
        public int DeletedCount { get; private set; }

        private void LoadRows(string? status = null, bool isError = false)
        {
            _rows.Clear();
            IReadOnlyList<ConnectionRecord> records;
            try
            {
                records = ConnectionStorageManager.ListConnections(_document);
            }
            catch (Exception ex)
            {
                SetStatus("No se pudieron leer las conexiones: " + ex.Message, ErrorBrush);
                return;
            }

            foreach (ConnectionRecord record in records.OrderBy(r => r.CreatedUtc, StringComparer.Ordinal))
            {
                _rows.Add(new ConnectionRow(record));
            }

            if (status != null)
            {
                SetStatus(status, isError ? ErrorBrush : OkBrush);
            }
            else
            {
                SetStatus(_rows.Count == 0
                    ? "No hay conexiones de MotorConexiones en el documento '" + _document.Title + "'."
                    : _rows.Count + " conexión(es) en el documento '" + _document.Title + "'. Selecciona una para borrarla.", InfoBrush);
            }
            DeleteButton.IsEnabled = ConnectionsGrid.SelectedItem != null;
        }

        private void SetStatus(string text, Brush brush)
        {
            StatusText.Text = text;
            StatusText.Foreground = brush;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = ConnectionsGrid.SelectedItem != null && !_document.IsReadOnly;
        }

        private void OnRefresh(object sender, RoutedEventArgs e)
        {
            LoadRows();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnDelete(object sender, RoutedEventArgs e)
        {
            if (ConnectionsGrid.SelectedItem is not ConnectionRow row) return;

            if (_document.IsReadOnly)
            {
                SetStatus("El documento es de solo lectura: no se puede borrar.", ErrorBrush);
                return;
            }

            var confirm = new TaskDialog("MotorConexiones - Confirmar borrado")
            {
                MainInstruction = "¿Borrar la conexión " + row.ConnectionId + "?",
                MainContent = "Se borrarán " + row.ElementsCount + " elemento(s) creados por el add-in y se restaurarán " + row.ModifiedCount +
                              " barra(s) a su extensión original (" + row.Members + "). No se toca ningún elemento ajeno. " +
                              "Después se puede deshacer con Deshacer de Revit.",
                CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No,
                DefaultButton = TaskDialogResult.No,
            };
            if (confirm.Show() != TaskDialogResult.Yes) return;

            var stopwatch = Stopwatch.StartNew();
            var warnings = new List<ApiError>();
            ConnectionRecord? deleted = null;
            try
            {
                using (var scope = new OperationScope(_document, _uiApplication, "ribbon_delete", row.ConnectionId, warnings))
                {
                    using (Transaction tx = scope.StartTransaction(_document, "MotorConexiones: Borrar " + row.ConnectionId))
                    {
                        if (!ConnectionCreationService.DeleteConnection(_document, row.ConnectionId, warnings, out deleted))
                        {
                            throw new InvalidOperationException("La conexión ya no existe en el documento.");
                        }
                        scope.CommitOrThrow(tx);
                    }
                    scope.Commit();
                }
            }
            catch (Exception ex)
            {
                JsonLineLogger.Write(new { @event = "ribbon_delete_failed", connection_id = row.ConnectionId, error = ex.ToString(), warnings = warnings.Select(w => w.Code).ToList() });
                LoadRows("No se pudo borrar " + row.ConnectionId + ": " + ex.Message + " (se deshizo todo).", isError: true);
                return;
            }

            DeletedCount++;
            JsonLineLogger.Write(new
            {
                @event = "ribbon_delete",
                connection_id = row.ConnectionId,
                deleted_elements = deleted?.CreatedElementIds.Count ?? 0,
                restored_members = deleted?.ModifiedMembers.Count ?? 0,
                duration_ms = stopwatch.ElapsedMilliseconds,
                warnings = warnings.Select(w => w.Code).ToList(),
            });
            string warningText = warnings.Count == 0 ? "" : " Avisos: " + string.Join(", ", warnings.Select(w => w.Code).Distinct()) + ".";
            LoadRows("Borrada " + row.ConnectionId + ": " + (deleted?.CreatedElementIds.Count ?? 0) + " elemento(s) eliminados y " +
                     (deleted?.ModifiedMembers.Count ?? 0) + " barra(s) restauradas en " + stopwatch.ElapsedMilliseconds + " ms." + warningText);
        }

        /// <summary>Fila de la lista (solo lectura).</summary>
        public sealed class ConnectionRow
        {
            public ConnectionRow(ConnectionRecord record)
            {
                ConnectionId = record.ConnectionId;
                ConnectionType = record.ConnectionType;
                ElementsCount = record.CreatedElementIds.Count;
                ModifiedCount = record.ModifiedMembers.Count;
                Backend = record.BackendName;
                CreatedLocal = FormatLocal(record.CreatedUtc);
                Members = DescribeMembers(record.SpecJson);
            }

            public string ConnectionId { get; }
            public string ConnectionType { get; }
            public string CreatedLocal { get; }
            public int ElementsCount { get; }
            public int ModifiedCount { get; }
            public string Members { get; }
            public string Backend { get; }

            private static string FormatLocal(string createdUtc)
            {
                if (DateTime.TryParse(createdUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime utc))
                {
                    return utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                return createdUtc;
            }

            private static string DescribeMembers(string specJson)
            {
                try
                {
                    ConnectionSpec? spec = ConnectionSpec.FromJson(specJson);
                    if (spec == null) return "";
                    var parts = new List<string>();
                    if (spec.Chord != null) parts.Add("cordón " + spec.Chord.ElementId);
                    if (spec.Members != null) parts.AddRange(spec.Members.Select(m => (m.Role ?? "barra") + " " + m.ElementId));
                    string? drawing = spec.Source?.Drawing;
                    return (string.IsNullOrEmpty(drawing) ? "" : drawing + ": ") + string.Join(", ", parts);
                }
                catch
                {
                    return "(especificación no legible)";
                }
            }
        }
    }
}
