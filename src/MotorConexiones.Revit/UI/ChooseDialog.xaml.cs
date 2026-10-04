using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MotorConexiones.Revit.UI
{
    /// <summary>Una opción de <see cref="ChooseDialog"/>.</summary>
    public sealed class ChoiceItem
    {
        public ChoiceItem(string label, object? tag, bool selected = false)
        {
            Label = label;
            Tag = tag;
            Selected = selected;
        }

        public string Label { get; }
        public object? Tag { get; }
        public bool Selected { get; }
    }

    /// <summary>
    /// Diálogo pequeño para elegir de una lista (P11: listas primero), con la opción de pinchar en Revit en su lugar.
    /// Si la persona pulsa "Pinchar en Revit…", <see cref="PickRequested"/> queda en verdadero y el comando hace la
    /// selección con las ventanas cerradas.
    /// </summary>
    public partial class ChooseDialog : Window
    {
        public ChooseDialog(string title, string prompt, IEnumerable<ChoiceItem> items, bool multiSelect, bool allowPick)
        {
            InitializeComponent();
            Title = "MotorConexiones: " + title;
            PromptText.Text = prompt;
            ItemsList.SelectionMode = multiSelect ? SelectionMode.Extended : SelectionMode.Single;
            foreach (ChoiceItem item in items)
            {
                ItemsList.Items.Add(item);
                if (item.Selected) ItemsList.SelectedItems.Add(item);
            }
            PickButton.Visibility = allowPick ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>Opciones elegidas al aceptar.</summary>
        public List<ChoiceItem> Chosen { get; } = new List<ChoiceItem>();

        /// <summary>Verdadero si se pidió pinchar en Revit en vez de elegir de la lista.</summary>
        public bool PickRequested { get; private set; }

        private void OnOk(object sender, RoutedEventArgs e)
        {
            Chosen.Clear();
            Chosen.AddRange(ItemsList.SelectedItems.Cast<ChoiceItem>());
            DialogResult = true;
            Close();
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ItemsList.SelectionMode == SelectionMode.Single && ItemsList.SelectedItem != null) OnOk(sender, e);
        }

        private void OnPick(object sender, RoutedEventArgs e)
        {
            PickRequested = true;
            DialogResult = true;
            Close();
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
