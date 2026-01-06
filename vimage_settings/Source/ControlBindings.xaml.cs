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

            if (App.Config == null)
                return;
            foreach (var c in App.Config.Controls)
                ControlsPanel.Children.Add(new ControlItem(c.Key.ToString(), c.Value));

            CustomActionBindings = [];
            foreach (var c in App.Config.CustomActionBindings)
                AddCustomActionBinding(c.Key, c.Value);
        }

        public void AddCustomActionBinding(string actionName, List<string> bindings)
        {
            var item = new ControlItem(actionName, bindings);
            _ = ControlsPanel.Children.Add(item);
            CustomActionBindings.Add(item);
        }

        public void RemoveCustomActionBinding(int index)
        {
            ControlsPanel.Children.Remove(CustomActionBindings[index]);
            CustomActionBindings.RemoveAt(index);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            if (App.Config == null)
                return;

            // Reset Controls to Default
            var defaultConfig = new Config();
            App.Config.Controls = new Dictionary<Action, List<string>>(defaultConfig.Controls);

            foreach (ControlItem item in ControlsPanel.Children)
                item.UpdateBindings();
        }
    }
}
