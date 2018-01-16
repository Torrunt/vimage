using System.Collections.Generic;
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
            DataContext = App.vimageConfig;

            if (App.vimageConfig == null)
                return;
            LoadItems();
        }

        private void LoadItems()
        {
            for (int i = 0; i < App.vimageConfig.CustomActions.Count; i++)
            {
                CustomActionItem item = new CustomActionItem(i, CustomActionItems);
                CustomActionItems.Children.Add(item);
            }
        }
        public void UpdateItemIndices()
        {
            for (int i = 0; i < CustomActionItems.Children.Count; i++)
                (CustomActionItems.Children[i] as CustomActionItem).Index = i;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            int index = CustomActionItems.Children.Count;

            App.vimageConfig.CustomActions.Add(new { name = "ACTION", func = "" });
            App.vimageConfig.CustomActionBindings.Add(new { name = "ACTION", bindings = new List<int>() });

            CustomActionItem item = new CustomActionItem(index, CustomActionItems);
            CustomActionItems.Children.Add(item);

            // update controls tab
            (Application.Current.MainWindow as MainWindow).ControlBindings.AddCustomActionBinding(index);
            // update context menu function list
            (Application.Current.MainWindow as MainWindow).ContextMenuEditor.UpdateCustomActions();
        }

    }
}
