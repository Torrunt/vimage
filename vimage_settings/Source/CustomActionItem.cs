using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vimage;

namespace vimage_settings
{
    public partial class CustomActionItem : UserControl
    {
        private ConfigWindow ConfigWindow;

        public CustomActionItem(string name, string func)
        {
            InitializeComponent();

            Dock = DockStyle.Top;

            // name
            textBox_Name.Text = name;

            // func
            textBox_Func.Text = func;
        }

        public void AddConfigWindowReference(ConfigWindow configWindow) { ConfigWindow = configWindow; }

        public string GetName() { return textBox_Name.Text; }
        public string GetFunc() { return textBox_Func.Text; }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            int index = ConfigWindow.CustomActionItems.IndexOf(this);

            int scrollValue = ConfigWindow.GetContextMenuPanelScrollValue();
            ConfigWindow.CustomActionItems.Remove(this);
            Parent.Controls.Remove(this);
            ConfigWindow.RefreshCustomAcionItems(scrollValue);
        }
    }
}
