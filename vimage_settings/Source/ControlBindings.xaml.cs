using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using vimage.Common;
using Action = vimage.Common.Action;

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
            foreach (var action in Enum.GetValues<Action>())
            {
                if (action == Action.None || action == Action.Custom)
                    continue;
                _ = App.Config.Controls.TryGetValue(action, out var controls);
                ControlsPanel.Children.Add(new ControlItem(action.ToString(), controls ?? []));
            }

            CustomActionBindings = [];
            foreach (var customAction in App.Config.CustomActions)
            {
                _ = App.Config.CustomActionBindings.TryGetValue(
                    customAction.Name,
                    out var controls
                );
                AddCustomActionBinding(customAction.Name, controls ?? []);
            }
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
