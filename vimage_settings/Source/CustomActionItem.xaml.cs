using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;

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

            ItemName.Text = (App.vimageConfig.CustomActions[Index] as dynamic).name;
            ItemAction.Text = (App.vimageConfig.CustomActions[Index] as dynamic).func;

            ItemName.TextChanged += ItemName_TextChanged;
            ItemAction.TextChanged += ItemAction_TextChanged;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            App.vimageConfig.CustomActions.RemoveAt(Index);
            App.vimageConfig.CustomActionBindings.RemoveAt(Index);

            ParentPanel?.Children.Remove(this);

            // update controls tab
            (Application.Current.MainWindow as MainWindow).ControlBindings.RemoveCustomActionBinding(Index);

            // update item indices
            (Application.Current.MainWindow as MainWindow).CustomActions.UpdateItemIndices();
            // update context menu function list
            (Application.Current.MainWindow as MainWindow).ContextMenuEditor.UpdateCustomActions();
        }

        private void ItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.vimageConfig == null)
                return;
            App.vimageConfig.CustomActions[Index] = new { name = ItemName.Text, func = ItemAction.Text };

            // update control binding
            List<int> bindings = (App.vimageConfig.CustomActionBindings[Index] as dynamic).bindings;
            App.vimageConfig.CustomActionBindings[Index] = new { name = ItemName.Text, bindings = bindings };

            // update controls tab
            (Application.Current.MainWindow as MainWindow).ControlBindings.CustomActionBindings[Index].ControlName.Content = ItemName.Text;
            // update context menu function list
            (Application.Current.MainWindow as MainWindow).ContextMenuEditor.UpdateCustomActions();
        }
        private void ItemAction_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (App.vimageConfig == null)
                return;
            App.vimageConfig.CustomActions[Index] = new { name = ItemName.Text, func = ItemAction.Text };
        }
    }
}
