using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ControlItem.xaml
    /// </summary>
    public partial class ControlItem : UserControl
    {
        public List<int> Controls = [];
        private bool CanRecordMouseButton = false;
        private readonly List<int> KeysHeld = [];

        public ControlItem()
        {
            InitializeComponent();
        }

        public ControlItem(string name, List<int> controls)
        {
            InitializeComponent();

            ControlName.Content = name;
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
            ControlSetting.Text = Config.ControlsToString(Controls);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            int key = ConvertWindowsKey(e.Key == Key.System ? e.SystemKey : e.Key);
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

            int key = ConvertWindowsKey(e.Key == Key.System ? e.SystemKey : e.Key);
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
            int button = e.ChangedButton switch
            {
                MouseButton.Left => 0,
                MouseButton.Right => 1,
                MouseButton.Middle => 2,
                MouseButton.XButton1 => 3,
                MouseButton.XButton2 => 4,
                _ => -1,
            };
            RecordControl(button + Config.MouseCodeOffset);
            ControlSetting.ReleaseMouseCapture();
        }

        private void ControlSetting_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!ControlSetting.IsFocused)
                return;
            e.Handled = true;

            // Record Mouse Wheel Direction
            int bind = -1;
            if (e.Delta > 0)
                bind = Config.MOUSE_SCROLL_UP;
            else if (e.Delta < 0)
                bind = Config.MOUSE_SCROLL_DOWN;

            RecordControl(bind);
        }

        private static int ConvertWindowsKey(Key keyCode)
        {
            var key = keyCode.ToString().ToUpper();
            if (key is null)
                return -1;

            // Record Key Press
            if (
                key.Equals("SCROLL")
                || key.Equals("NUMLOCK")
                || key.Equals("CAPITAL")
                || key.Equals("LWIN")
                || key.Equals("RWIN")
            )
                return -1;

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

            return (int)Config.StringToKey(key);
        }

        private void RecordControl(int bind, bool canBeKeyCombo = true)
        {
            if (bind == -1)
                return;
            int i = Controls.IndexOf(bind);
            if (!(i == -1 || (i > 1 && Controls[i - 2] == -2)))
                return;

            if (canBeKeyCombo)
            {
                if (KeysHeld.Count > 0 && KeysHeld[^1] != bind)
                {
                    // Key Combo? (eg: CTRL+C)
                    int c = KeysHeld[^1];

                    if (i != -1 && Controls.IndexOf(c) != -1)
                        return;
                    Controls.Add(-2);
                    Controls.Add(c);
                }
                else if (i != -1)
                    return;
            }
            Controls.Add(bind);
            UpdateBindings();
        }

        private void ControlSetting_GotFocus(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
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
            Window window = Window.GetWindow(this);
            if (window == null)
                return;
            window.PreviewKeyDown -= OnKeyDown;
            window.PreviewKeyUp -= OnKeyUp;
            ControlSetting.PreviewMouseUp -= ControlSetting_MouseUp;
            ControlSetting.PreviewMouseWheel -= ControlSetting_MouseWheel;
        }
    }
}
