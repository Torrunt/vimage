using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ControlItem.xaml
    /// </summary>
    public partial class ControlItem : UserControl
    {
        public List<string> Controls = [];
        private bool CanRecordMouseButton = false;
        private bool JustRecordedCombo = false;
        private readonly List<string> KeysHeld = [];

        public ControlItem()
        {
            InitializeComponent();
        }

        public ControlItem(string name, List<string> controls)
        {
            InitializeComponent();

            ControlName.Content = Helpers.SplitCamelCase(name);
            Controls = controls;
            UpdateBindings();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Controls.Clear();
            ControlSetting.Text = "";
        }

        public void UpdateBindings()
        {
            ControlSetting.Text = string.Join(", ", Controls);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = ConvertWindowsKey(e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == null)
                return;

            if (KeysHeld.Count == 0 || KeysHeld[^1] != key)
            {
                KeysHeld.Add(key);
                if (KeysHeld.Count > 2)
                    KeysHeld.RemoveAt(0);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var key = ConvertWindowsKey(e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == null)
                return;

            _ = KeysHeld.Remove(key);
            RecordControl(key);
        }

        private void ControlSetting_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!ControlSetting.IsFocused)
                return;
            if (!CanRecordMouseButton)
            {
                CanRecordMouseButton = true;
                return;
            }
            e.Handled = true;

            // Record Mouse Button Press
            var button = e.ChangedButton switch
            {
                MouseButton.Left => "MOUSELEFT",
                MouseButton.Right => "MOUSERIGHT",
                MouseButton.Middle => "MOUSEMIDDLE",
                MouseButton.XButton1 => "MOUSE4",
                MouseButton.XButton2 => "MOUSE5",
                _ => null,
            };
            if (button != null)
                RecordControl(button);
            ControlSetting.ReleaseMouseCapture();
        }

        private void ControlSetting_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!ControlSetting.IsFocused)
                return;
            e.Handled = true;

            // Record Mouse Wheel Direction
            RecordControl(e.Delta > 0 ? "SCROLLUP" : "SCROLLDOWN");
        }

        private static string? ConvertWindowsKey(Key keyCode)
        {
            var key = keyCode.ToString().ToUpper();
            if (key is null)
                return null;

            // Record Key Press
            if (
                key.Equals("SCROLL")
                || key.Equals("NUMLOCK")
                || key.Equals("CAPITAL")
                || key.Equals("LWIN")
                || key.Equals("RWIN")
            )
                return null;

            // fix up some weird names KeyEventArgs gives
            key = key switch
            {
                "OEMOPENBRACKETS" => "[",
                "OEM3" => "`",
                "OEM6" => "]",
                "OEM5" => "\\",
                "OEM1" => ";",
                "OEM7" => "'",
                "OEMMINUS" => "MINUS",
                "OEMPLUS" => "PLUS",
                _ => key,
            };

            // fix number keys (remove D from D#)
            if (key.Length == 2 && key[0] == 'D')
                key = key[1..];

            return key;
        }

        private void RecordControl(string input)
        {
            if (JustRecordedCombo)
            {
                // Don't record any more controls until all buttons are released
                if (KeysHeld.Count > 0)
                    return;
                JustRecordedCombo = false;
                return;
            }

            int i = Controls.IndexOf(input);

            // Key Combo? (eg: CTRL+C)
            if (KeysHeld.Count > 0 && KeysHeld[^1] != input)
            {
                var combo = string.Join("+", KeysHeld) + "+" + input;
                if (Controls.IndexOf(combo) != -1)
                    return;
                Controls.Add(combo);
                UpdateBindings();
                JustRecordedCombo = true;
                return;
            }

            if (i != -1)
                return;
            Controls.Add(input);
            UpdateBindings();
        }

        private void ControlSetting_GotFocus(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;
            window.PreviewKeyDown += OnKeyDown;
            window.PreviewKeyUp += OnKeyUp;
            ControlSetting.PreviewMouseUp += ControlSetting_MouseUp;
            ControlSetting.PreviewMouseWheel += ControlSetting_MouseWheel;
            CanRecordMouseButton = false;
        }

        private void ControlSetting_LostFocus(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window == null)
                return;
            window.PreviewKeyDown -= OnKeyDown;
            window.PreviewKeyUp -= OnKeyUp;
            ControlSetting.PreviewMouseUp -= ControlSetting_MouseUp;
            ControlSetting.PreviewMouseWheel -= ControlSetting_MouseWheel;
        }
    }
}
