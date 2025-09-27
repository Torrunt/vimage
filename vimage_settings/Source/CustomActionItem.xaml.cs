using System.Windows;
using System.Windows.Controls;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for CustomActionItem.xaml
    /// </summary>
    public partial class CustomActionItem : UserControl
    {
        private readonly StackPanel ParentPanel;
        public int Index;

        public CustomActionItem()
        {
            InitializeComponent();
        }
        public CustomActionItem(int index, StackPanel parentPanel)
        {
            InitializeComponent();

            Index = index;
            ParentPanel = parentPanel;

            ItemName.Text = App.vimageConfig?.CustomActions[Index].name;
            ItemAction.Text = App.vimageConfig?.CustomActions[Index].func;

            ItemName.TextChanged += ItemName_TextChanged;
            ItemAction.TextChanged += ItemAction_TextChanged;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (App.vimageConfig is not null)
            {
                App.vimageConfig.CustomActions.RemoveAt(Index);
                App.vimageConfig.CustomActionBindings.RemoveAt(Index);
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
            if (App.vimageConfig == null)
                return;
            App.vimageConfig.CustomActions[Index] = new CustomAction { name = ItemName.Text, func = ItemAction.Text };

            // update control binding
            var bindings = App.vimageConfig.CustomActionBindings[Index].bindings;
            App.vimageConfig.CustomActionBindings[Index] = new CustomActionBinding { name = ItemName.Text, bindings = bindings };

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // update controls tab
                mainWindow.ControlBindings.CustomActionBindings[Index].ControlName.Content = ItemName.Text;
                // update context menu function list
                mainWindow.ContextMenuEditor.UpdateCustomActions();
            }
        }
        private void ItemAction_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.vimageConfig == null)
                return;
            App.vimageConfig.CustomActions[Index] = new CustomAction { name = ItemName.Text, func = ItemAction.Text };
        }
    }
}
