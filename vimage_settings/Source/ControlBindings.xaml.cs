using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ControlBindings.xaml
    /// </summary>
    public partial class ControlBindings : UserControl
    {
        public List<ControlItem> CustomActionBindings = [];

        public ControlBindings()
        {
            InitializeComponent();

            if (App.vimageConfig == null)
                return;
            for (int i = 0; i < App.vimageConfig.Controls.Count; i++)
            {
                var item = new ControlItem(App.vimageConfig.ControlNames[i], App.vimageConfig.Controls[i]);
                _ = ControlsPanel.Children.Add(item);
            }
            CustomActionBindings = [];
            for (int i = 0; i < App.vimageConfig.CustomActionBindings.Count; i++)
            {
                AddCustomActionBinding(i);
            }
        }
        public void AddCustomActionBinding(int index)
        {
            if (App.vimageConfig.CustomActionBindings[index] is CustomActionBinding cab)
            {
                var item = new ControlItem(cab.name, cab.bindings);
                _ = ControlsPanel.Children.Add(item);
                CustomActionBindings.Add(item);
            }
        }
        public void RemoveCustomActionBinding(int index)
        {
            ControlsPanel.Children.Remove(CustomActionBindings[index]);
            CustomActionBindings.RemoveAt(index);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            // Reset Controls to Default
            App.vimageConfig.SetDefaultControls();

            foreach (ControlItem item in ControlsPanel.Children)
                item.UpdateBindings();
        }
    }
}
