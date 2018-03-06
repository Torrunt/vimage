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
        private bool RecordedKeyCombo = false;
        private bool CanRecordMouseButton = false;

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
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (key == Key.System || key == Key.LeftShift || key == Key.RightShift || key == Key.LeftCtrl || key == Key.RightCtrl ||
                key == Key.LeftAlt || key == Key.RightAlt || key == Key.LWin || key == Key.RWin)
                return;
            RecordKey(e.Key == Key.System ? e.SystemKey : e.Key);
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (RecordedKeyCombo)
                return;
            Key key = e.Key == Key.System ? e.SystemKey : e.Key;
            // Only record CTRL, SHIFT and ALT as single keys on key up (in case they are part of a key combo)
            if (key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftShift || key == Key.RightShift ||
                key == Key.LeftAlt || key == Key.RightAlt)
                RecordKey(key, false);
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

            // Update Control and Text Box
            int bind = button + Config.MouseCodeOffset;
            if (Controls.IndexOf(bind) == -1)
            {
                Controls.Add(bind);
                UpdateBindings();
            }
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

            // Update Control and Text Box
            if (Controls.IndexOf(bind) == -1)
            {
                Controls.Add(bind);
                UpdateBindings();
            }
        }
        private void RecordKey(Key keyCode, bool canBeKeyCombo = true)
        {
            string key = keyCode.ToString().ToUpper();

            RecordedKeyCombo = false;
            // Record Key Press
            if (key.Equals("SCROLL") || key.Equals("NUMLOCK") || key.Equals("CAPITAL") ||
                key.Equals("LWIN") || key.Equals("RWIN"))
                return;

            // fix up some weird names KeyEventArgs gives
            switch (key)
            {
                case "OEMOPENBRACKETS": key = "["; break;
                case "OEM3": key = "`"; break;
                case "OEM6": key = "]"; break;
                case "OEM5": key = "\\"; break;
                case "OEM1": key = ";"; break;
                case "OEM7": key = "'"; break;
            }

            if (key.Length == 2 && key[0] == 'D')
                key = key.Remove(0, 1);

            // Update Control and Text Box
            int bind = (int)Config.StringToKey(key);
            int i = Controls.IndexOf(bind);
            if (i == -1 || (i > 1 && Controls[i-2] == -2))
            {
                if (canBeKeyCombo)
                {
                    // Key Combo? (eg: CTRL+C)
                    int c = -1;
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        c = (int)SFML.Window.Keyboard.Key.LControl;
                    else if (Keyboard.IsKeyDown(Key.RightCtrl))
                        c = (int)SFML.Window.Keyboard.Key.RControl;
                    else if (Keyboard.IsKeyDown(Key.LeftShift))
                        c = (int)SFML.Window.Keyboard.Key.LShift;
                    else if (Keyboard.IsKeyDown(Key.RightShift))
                        c = (int)SFML.Window.Keyboard.Key.RShift;
                    else if (Keyboard.IsKeyDown(Key.LeftAlt))
                        c = (int)SFML.Window.Keyboard.Key.LAlt;
                    else if (Keyboard.IsKeyDown(Key.RightAlt))
                        c = (int)SFML.Window.Keyboard.Key.RAlt;
                    if (c != -1 && c != bind)
                    {
                        if (i != -1 && Controls.IndexOf(c) != -1)
                            return;
                        Controls.Add(-2);
                        Controls.Add(c);
                        RecordedKeyCombo = true;
                    }
                    else if (i != -1)
                        return;
                }
                Controls.Add(bind);
                UpdateBindings();
            }
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
