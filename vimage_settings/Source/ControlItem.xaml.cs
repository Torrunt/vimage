using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using vimage;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ControlItem.xaml
    /// </summary>
    public partial class ControlItem : UserControl
    {
        public List<int> Controls;
        private bool CanRecordMouseButton = false;
        private readonly List<int> KeysHeld = new List<int>();

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
            if (KeysHeld.Count == 0 || KeysHeld[KeysHeld.Count - 1] != key)
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
            int button = -1;
            switch (e.ChangedButton)
            {
                case MouseButton.Left: button = (int)SFML.Window.Mouse.Button.Left; break;
                case MouseButton.Right: button = (int)SFML.Window.Mouse.Button.Right; break;
                case MouseButton.Middle: button = (int)SFML.Window.Mouse.Button.Middle; break;
                case MouseButton.XButton1: button = (int)SFML.Window.Mouse.Button.XButton1; break;
                case MouseButton.XButton2: button = (int)SFML.Window.Mouse.Button.XButton2; break;
            }

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
        private int ConvertWindowsKey(Key keyCode)
        {
            string key = keyCode.ToString().ToUpper();

            // Record Key Press
            if (key.Equals("SCROLL") || key.Equals("NUMLOCK") || key.Equals("CAPITAL") ||
                key.Equals("LWIN") || key.Equals("RWIN"))
                return -1;

            // fix up some weird names KeyEventArgs gives
            switch (key)
            {
                case "OEMOPENBRACKETS": key = "["; break;
                case "OEM3": key = "`"; break;
                case "OEM6": key = "]"; break;
                case "OEM5": key = "\\"; break;
                case "OEM1": key = ";"; break;
                case "OEM7": key = "'"; break;
                case "OEMMINUS": key = "MINUS"; break;
                case "OEMPLUS": key = "PLUS"; break;
            }

            // fix number keys (remove D from D#)
            if (key.Length == 2 && key[0] == 'D')
                key = key.Remove(0, 1);

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
                if (KeysHeld.Count > 0 && KeysHeld[KeysHeld.Count - 1] != bind)
                {
                    // Key Combo? (eg: CTRL+C)
                    int c = KeysHeld[KeysHeld.Count - 1];

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
