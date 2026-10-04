using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.DB;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Logging;
using MotorConexiones.Revit.Storage;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Resultado de borrar una conexión desde la ventana.</summary>
    public sealed class DeleteOutcome
    {
        public DeleteOutcome(bool ok, string message, int deletedElements = 0, int restoredMembers = 0)
        {
            Ok = ok;
            Message = message;
            DeletedElements = deletedElements;
            RestoredMembers = restoredMembers;
        }

        public bool Ok { get; }
        public string Message { get; }
        public int DeletedElements { get; }
        public int RestoredMembers { get; }
    }

    /// <summary>Fila de la lista de conexiones.</summary>
    public sealed class ConnectionRow
    {
        public ConnectionRow(ConnectionRecord record)
        {
            Id = record.ConnectionId;
            Type = record.ConnectionType;
            ElementsCount = record.CreatedElementIds?.Count ?? 0;
            Backend = record.BackendName;
            Created = FormatDate(record.CreatedUtc);
            Drawing = "";
            Members = "";
            try
            {
                ConnectionSpec? spec = ConnectionSpec.FromJson(record.SpecJson);
                if (spec != null)
                {
                    Drawing = spec.Source?.Drawing ?? "";
                    var ids = new List<long>();
                    if (spec.Chord != null && spec.Chord.ElementId > 0) ids.Add(spec.Chord.ElementId);
                    if (spec.Members != null) ids.AddRange(spec.Members.Select(m => m.ElementId));
                    if (spec.Node?.ElementIds != null) ids.AddRange(spec.Node.ElementIds);
                    Members = string.Join(", ", ids.Distinct().Select(i => i.ToString(CultureInfo.InvariantCulture)));
                }
            }
            catch
            {
                Members = "(especificación ilegible)";
            }
        }

        public string Id { get; }
        public string Type { get; }
        public string Drawing { get; }
        public string Created { get; }
        public int ElementsCount { get; }
        public string Members { get; }
        public string Backend { get; }

        private static string FormatDate(string? utc)
        {
            if (DateTime.TryParse(utc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime parsed))
            {
                return parsed.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            }
            return utc ?? "";
        }
    }

    /// <summary>
    /// Ventana "Conexiones del modelo" (Fase 6, sección 3.2): lista las conexiones del add-in en el documento y permite
    /// borrar una con confirmación. El borrado lo ejecuta el comando de la cinta a través de <c>deleter</c>
    /// (OperationScope + ConnectionCreationService.DeleteConnection, lo mismo que conn_delete); la ventana solo lista.
    /// </summary>
    public partial class ConnectionsWindow : Window
    {
        private readonly Document _document;
        private readonly Func<string, DeleteOutcome> _deleter;
        private readonly ObservableCollection<ConnectionRow> _rows = new ObservableCollection<ConnectionRow>();

        public ConnectionsWindow(Document document, Func<string, DeleteOutcome> deleter, IntPtr ownerHandle)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _deleter = deleter ?? throw new ArgumentNullException(nameof(deleter));
            InitializeComponent();
            if (ownerHandle != IntPtr.Zero)
            {
                new WindowInteropHelper(this).Owner = ownerHandle;
            }
            ConnectionsGrid.ItemsSource = _rows;
            Refresh(null);
        }

        /// <summary>Conexiones borradas en esta sesión de la ventana (para el registro del comando).</summary>
        public int DeletedCount { get; private set; }

        private void Refresh(string? note)
        {
            _rows.Clear();
            try
            {
                IReadOnlyList<ConnectionRecord> records = ConnectionStorageManager.ListConnections(_document);
                foreach (ConnectionRecord record in records.OrderByDescending(r => r.CreatedUtc, StringComparer.Ordinal))
                {
                    _rows.Add(new ConnectionRow(record));
                }
                // Primero lo que acaba de pasar (p. ej. "Borrada …"); después cuántas quedan.
                string remaining = _rows.Count == 0
                    ? (note != null ? "No queda ninguna conexión en el documento." : "No hay conexiones de MotorConexiones en este documento.")
                    : (note != null ? "Quedan " : "") + _rows.Count + (_rows.Count == 1 ? " conexión" : " conexiones") + " en el documento.";
                StatusText.Text = note != null ? note + " " + remaining : remaining;
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "connections_window_list_failed", error = error.ToString() });
                StatusText.Text = "No se pudo leer el almacenamiento del modelo: " + error.Message;
            }
            DeleteButton.IsEnabled = ConnectionsGrid.SelectedItem is ConnectionRow;
        }

        private void OnSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DeleteButton.IsEnabled = ConnectionsGrid.SelectedItem is ConnectionRow;
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            Refresh(null);
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (ConnectionsGrid.SelectedItem is not ConnectionRow row) return;

            MessageBoxResult answer = MessageBox.Show(this,
                "¿Borrar la conexión " + row.Id + "?\n\n" +
                "Se eliminan sus " + row.ElementsCount + " elementos (placas, pernos y soldaduras creados por el add-in) y las barras " +
                row.Members + " recuperan su extensión original. No se borra nada que el add-in no haya creado.",
                "MotorConexiones · Confirmar borrado", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (answer != MessageBoxResult.Yes) return;

            DeleteOutcome outcome;
            try
            {
                outcome = _deleter(row.Id);
            }
            catch (Exception error)
            {
                JsonLineLogger.Write(new { @event = "connections_window_delete_failed", connection_id = row.Id, error = error.ToString() });
                outcome = new DeleteOutcome(false, "Error inesperado: " + error.Message);
            }

            if (outcome.Ok) DeletedCount++;
            Refresh(outcome.Message);
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            // Fijar DialogResult cierra la ventana modal; no hace falta Close().
            DialogResult = DeletedCount > 0;
        }
    }
}
