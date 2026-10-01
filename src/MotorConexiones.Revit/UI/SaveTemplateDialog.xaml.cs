using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MotorConexiones.Core.Catalog;
using MotorConexiones.Core.Contract;
using MotorConexiones.Core.Storage;
using MotorConexiones.Revit.Catalog;

namespace MotorConexiones.Revit.UI
{
    /// <summary>
    /// Diálogo pequeño de "Guardar en catálogo" (Fase 7): nombre, descripción, etiquetas, política de perfil y copia a la
    /// carpeta compartida. Con una lista de conexiones del modelo, también cuál de ellas se guarda como plantilla.
    /// </summary>
    public partial class SaveTemplateDialog : Window
    {
        private readonly string? _sharedFolder;

        public SaveTemplateDialog(CatalogConfig config, string defaultName, IReadOnlyList<ConnectionRecord>? connections)
        {
            InitializeComponent();
            config ??= CatalogConfig.Default;
            NameBox.Text = defaultName ?? string.Empty;
            _sharedFolder = CatalogConfigLoader.ResolveSharedFolder(config);

            string policy = ProfilePolicy.Normalize(config.DefaultProfilePolicy);
            PolicyBox.SelectedIndex = policy == ProfilePolicy.Require ? 1 : policy == ProfilePolicy.Ignore ? 2 : 0;

            if (string.IsNullOrEmpty(_sharedFolder))
            {
                SharedBox.IsEnabled = false;
                SharedFolderText.Text = "No hay shared_catalog_folder en config\\catalog.json (por ejemplo la carpeta catalog\\ del repositorio).";
            }
            else
            {
                SharedFolderText.Text = _sharedFolder;
            }

            if (connections != null)
            {
                ConnectionPanel.Visibility = System.Windows.Visibility.Visible;
                foreach (ConnectionRecord record in connections.OrderBy(r => r.CreatedUtc, StringComparer.Ordinal))
                {
                    ConnectionBox.Items.Add(new ConnectionItem(record));
                }
                if (ConnectionBox.Items.Count > 0) ConnectionBox.SelectedIndex = 0;
            }
            NameBox.Focus();
            NameBox.SelectAll();
        }

        /// <summary>Lo que escribió la persona (válido solo si el diálogo devolvió verdadero).</summary>
        public TemplateMetadata Metadata { get; private set; } = new TemplateMetadata();

        /// <summary>Conexión elegida (solo cuando se abrió con la lista de conexiones).</summary>
        public string? SelectedConnectionId { get; private set; }

        public bool Overwrite { get; private set; }
        public bool CopyToShared { get; private set; }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            string name = (NameBox.Text ?? string.Empty).Trim();
            if (name.Length == 0)
            {
                ErrorText.Text = "Escribe un nombre para la plantilla.";
                NameBox.Focus();
                return;
            }
            if (ConnectionPanel.Visibility == System.Windows.Visibility.Visible)
            {
                if (ConnectionBox.SelectedItem is not ConnectionItem item)
                {
                    ErrorText.Text = "Elige la conexión del modelo que quieres guardar como plantilla.";
                    return;
                }
                SelectedConnectionId = item.ConnectionId;
            }

            Metadata = new TemplateMetadata
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(DescriptionBox.Text) ? null : DescriptionBox.Text.Trim(),
                Tags = (TagsBox.Text ?? string.Empty).Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).Where(t => t.Length > 0).ToList(),
                ProfilePolicy = (PolicyBox.SelectedItem as ComboBoxItem)?.Tag as string ?? ProfilePolicy.Warn,
            };
            Overwrite = OverwriteBox.IsChecked == true;
            CopyToShared = SharedBox.IsEnabled && SharedBox.IsChecked == true;
            DialogResult = true;
            Close();
        }

        /// <summary>Una conexión del modelo en la lista desplegable.</summary>
        public sealed class ConnectionItem
        {
            public ConnectionItem(ConnectionRecord record)
            {
                ConnectionId = record.ConnectionId;
                string members = "";
                try
                {
                    ConnectionSpec? spec = ConnectionSpec.FromJson(record.SpecJson);
                    if (spec != null)
                    {
                        var parts = new List<string>();
                        if (spec.Chord != null) parts.Add("cordón " + spec.Chord.ElementId);
                        if (spec.Members != null) parts.AddRange(spec.Members.Select(m => (m.Role ?? "barra") + " " + m.ElementId));
                        members = (spec.Source?.Drawing != null ? spec.Source.Drawing + ": " : "") + string.Join(", ", parts);
                    }
                }
                catch
                {
                    members = "(especificación no legible)";
                }
                string created = record.CreatedUtc;
                if (DateTime.TryParse(record.CreatedUtc, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime utc))
                {
                    created = utc.ToLocalTime().ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }
                Text = created + " · " + members + " · " + record.ConnectionId;
            }

            public string ConnectionId { get; }
            public string Text { get; }
        }
    }
}
