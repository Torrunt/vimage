using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ControlBindings.xaml
    /// </summary>
    public partial class ControlBindings : UserControl
    {
        public List<ControlItem> CustomActionBindings;

        public ControlBindings()
        {
            InitializeComponent();

            if (App.vimageConfig == null)
                return;
            for (int i = 0; i < App.vimageConfig.Controls.Count; i++)
            {
                ControlItem item = new ControlItem(App.vimageConfig.ControlNames[i], App.vimageConfig.Controls[i]);
                ControlsPanel.Children.Add(item);
            }
            CustomActionBindings = new List<ControlItem>();
            for (int i = 0; i < App.vimageConfig.CustomActionBindings.Count; i++)
            {
                AddCustomActionBinding(i);
            }
        }
        public void AddCustomActionBinding(int index)
        {
            ControlItem item = new ControlItem((App.vimageConfig.CustomActionBindings[index] as dynamic).name, (App.vimageConfig.CustomActionBindings[index] as dynamic).bindings);
            ControlsPanel.Children.Add(item);
            CustomActionBindings.Add(item);
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
