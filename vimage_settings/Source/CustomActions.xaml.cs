using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using vimage.Common;

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
            DataContext = App.vimageConfig;

            if (App.vimageConfig == null)
                return;
            LoadItems();
        }

        private void LoadItems()
        {
            if (App.vimageConfig is null) return;
            for (int i = 0; i < App.vimageConfig.CustomActions.Count; i++)
            {
                var item = new CustomActionItem(i, CustomActionItems);
                _ = CustomActionItems.Children.Add(item);
            }
        }

        public void UpdateItemIndices()
        {
            for (int i = 0; i < CustomActionItems.Children.Count; i++)
            {
                if (CustomActionItems.Children[i] is CustomActionItem customActionItem)
                    customActionItem.Index = i;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int index = CustomActionItems.Children.Count;

            if (App.vimageConfig != null)
            {
                App.vimageConfig.CustomActions.Add(new CustomAction { name = "ACTION", func = "" });
                App.vimageConfig.CustomActionBindings.Add(
                    new CustomActionBinding { name = "ACTION", bindings = [] }
                );
            }

            var item = new CustomActionItem(index, CustomActionItems);
            _ = CustomActionItems.Children.Add(item);

            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // update controls tab
                mainWindow.ControlBindings.AddCustomActionBinding(
                    index
                );
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
