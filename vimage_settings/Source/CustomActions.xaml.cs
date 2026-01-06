using System.Windows;
using System.Windows.Controls;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for CustomActions.xaml
    /// </summary>
    public partial class CustomActions : UserControl
    {
        public CustomActions()
        {
            InitializeComponent();
            DataContext = App.Config;

            if (App.Config == null)
                return;
            LoadItems();
        }

        private void LoadItems()
        {
            if (App.Config is null)
                return;
            for (int i = 0; i < App.Config.CustomActions.Count; i++)
            {
                var item = new CustomActionRow(i, CustomActionItems);
                _ = CustomActionItems.Children.Add(item);
            }
        }

        public void UpdateItemIndices()
        {
            for (int i = 0; i < CustomActionItems.Children.Count; i++)
            {
                if (CustomActionItems.Children[i] is CustomActionRow customActionItem)
                    customActionItem.Index = i;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (App.Config == null)
                return;

            int index = CustomActionItems.Children.Count;

            var actionName = "ACTION";

            App.Config.CustomActions.Add(new vimage.Common.CustomActionItem(actionName, ""));
            App.Config.CustomActionBindings.Add(actionName, []);

            var item = new CustomActionRow(index, CustomActionItems);
            _ = CustomActionItems.Children.Add(item);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // update controls tab
                mainWindow.ControlBindings.AddCustomActionBinding(actionName, []);
                // update context menu function list
                mainWindow.ContextMenuEditor.UpdateCustomActions();
            }
        }

        private void CommandList_Click(object sender, RoutedEventArgs e)
        {
            new CommandsList().Show();
        }
    }
}
