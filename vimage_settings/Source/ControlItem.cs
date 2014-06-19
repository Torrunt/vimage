using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vimage;

namespace vimage_settings
{
    public partial class ControlItem : UserControl
    {
        // <summary>Reference to a control in the config.</summary>
        private List<int> Control;

        private bool TextBoxHasFocus = false;

        public ControlItem(string name, List<int> control)
        {
            InitializeComponent();
            
            Dock = DockStyle.Top;

            Control = control;

            label_Name.Text = name;
            textBox_Binding.Text = Config.ControlsToString(Control);
        }

        public void UpdateBindings()
        {
            textBox_Binding.Text = Config.ControlsToString(Control);
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            // Clear Control
            textBox_Binding.Clear();
            Control.Clear();
        }

        private void control_OnKeyDown(object sender, KeyEventArgs e)
        {
            // Record Key Press
            string name = ((TextBox)sender).Name.Replace("textBox_", "");
            string key = e.KeyCode.ToString().ToUpper();

            if (key.Equals("SCROLL") || key.Equals("NUMLOCK") || key.Equals("CAPITAL") ||
                key.Equals("LWIN") || key.Equals("RWIN"))
                return;

            // fix up some weird names KeyEventArgs gives
            switch (key)
            {
                case "OEMOPENBRACKETS": key = "["; break;
                case "OEM6": key = "]"; break;
                case "OEM5": key = "\\"; break;
                case "OEM1": key = ";"; break;
                case "OEM7": key = "'"; break;
            }

            key = key.Replace("OEM", ""); // eg: OEMTILDE to TILDE
            key = key.Replace("KEY", ""); // eg: CONTROLKEY to CONTROL

            // Update Control and Text Box
            int bind = (int)Config.StringToKey(key);
            if (Control.IndexOf(bind) == -1)
            {
                Control.Add(bind);
                textBox_Binding.Text = Config.ControlsToString(Control);
            }
        }
        private void control_OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!TextBoxHasFocus)
            {
                TextBoxHasFocus = true;
                return;
            }

            // Record Mouse Button Press
            string name = ((TextBox)sender).Name.Replace("textBox_", "");
            string button = e.Button.ToString().ToUpper();

            // Update Control and Text Box
            int bind = Config.StringToMouseButton("MOUSE" + button);
            if (Control.IndexOf(bind) == -1)
            {
                Control.Add(bind);
                textBox_Binding.Text = Config.ControlsToString(Control);
            }
        }

        private void control_OnLoseFocus(object sender, EventArgs e)
        {
            TextBoxHasFocus = false;
        }
    }
}
