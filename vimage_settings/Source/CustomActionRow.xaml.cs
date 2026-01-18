using System.Windows;
using System.Windows.Controls;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for CustomActionRow.xaml
    /// </summary>
    public partial class CustomActionRow : UserControl
    {
        private readonly StackPanel ParentPanel;
        public int Index;
        private string ActionName;

        public CustomActionRow(int index, StackPanel parentPanel)
        {
            InitializeComponent();

            Index = index;
            ParentPanel = parentPanel;

            ActionName = App.Config?.CustomActions[Index].Name ?? "";
            ItemName.Text = ActionName;
            ItemAction.Text = App.Config?.CustomActions[Index].Func;

            ItemName.TextChanged += ItemName_TextChanged;
            ItemAction.TextChanged += ItemAction_TextChanged;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (App.Config is not null)
            {
                App.Config.CustomActions.RemoveAt(Index);
                App.Config.CustomActionBindings.Remove(ActionName);
            }

            ParentPanel?.Children.Remove(this);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // update controls tab
                mainWindow.ControlBindings.RemoveCustomActionBinding(Index);

                // update item indices
                mainWindow.CustomActions.UpdateItemIndices();
                // update context menu function list
                mainWindow.ContextMenuEditor.UpdateCustomActions();
            }
        }

        private void ItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.Config == null)
                return;

            var previousName = ActionName;
            var binding = App.Config.CustomActionBindings[previousName];

            App.Config.CustomActions[Index] = new CustomActionItem(ItemName.Text, ItemAction.Text);
            ActionName = ItemName.Text;

            // update control binding
            App.Config.CustomActionBindings.Remove(previousName);
            App.Config.CustomActionBindings.Add(ActionName, binding);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // update controls tab
                mainWindow.ControlBindings.CustomActionBindings[Index].ControlName.Content =
                    ItemName.Text;
                // update context menu function list
                mainWindow.ContextMenuEditor.UpdateCustomActions();
            }
        }

        private void ItemAction_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.Config == null)
                return;
            App.Config.CustomActions[Index] = new CustomActionItem(ItemName.Text, ItemAction.Text);
        }
    }
}
