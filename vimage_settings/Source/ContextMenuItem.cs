using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vimage;

namespace vimage_settings
{
    public partial class ContextMenuItem : UserControl
    {
        private ConfigWindow ConfigWindow;
        public bool Submenu = false;
        public int Subitem = 0;
        private bool FocusedItem = false;

        private const int SUB_ITEM_INDENT = 20;

        public ContextMenuItem(string name, string func)
        {
            InitializeComponent();

            // func drop down list
            comboBox_Function.Items.Add("-");
            for (int i = 0; i < MenuFuncs.FUNCS.Length; i++)
                comboBox_Function.Items.Add(MenuFuncs.WithSpaces(MenuFuncs.FUNCS[i]));
            comboBox_Function.SelectedIndex = 0;

            // name
            textBox_Name.Text = name;

            // func
            func = MenuFuncs.WithSpaces(func);
            int fundIndex = comboBox_Function.Items.IndexOf(func);
            if (fundIndex != -1)
                comboBox_Function.SelectedIndex = fundIndex;
        }

        public string GetName() { return textBox_Name.Text; }
        public string GetFunc() { return comboBox_Function.Text; }

        public void SetSubmenu(bool submenu)
        {
            if (Submenu == submenu)
                return;
            Submenu = submenu;

            if (Submenu)
            {
                comboBox_Function.Visible = false;
                textBox_Name.Width += comboBox_Function.Width + 3;
            }
            else
            {
                textBox_Name.Width -= comboBox_Function.Width + 3;
                comboBox_Function.Visible = true;
            }
        }
        public void SetSubitem(int subitem)
        {
            if (Subitem == subitem)
                return;

            int index = ConfigWindow.GetContextMenuList().IndexOf(this);

            // Exit out if trying to indent too far, or indent the first item
            if (subitem > Subitem && (index <= 0 || (index > 0 && ConfigWindow.GetContextMenuList()[index - 1].Subitem < Subitem)))
                return;

            // If submenu, reset to normal item for now
            bool wasSubmenu = false;
            if (Submenu)
            {
                SetSubmenu(false);
                wasSubmenu = true;
            }

            // Indent
            int indent = SUB_ITEM_INDENT * (subitem - Subitem);
            int indentHalf = indent / 2;

            textBox_Name.Location = new System.Drawing.Point(textBox_Name.Location.X + indent, textBox_Name.Location.Y);
            comboBox_Function.Location = new System.Drawing.Point(comboBox_Function.Location.X + indentHalf, comboBox_Function.Location.Y);
            textBox_Name.Width -= indentHalf;
            comboBox_Function.Width -= indentHalf;

            if (wasSubmenu)
                SetSubmenu(true);

            if (subitem > Subitem)
            {
                // Move right

                // Make item above a submenu if it's not already
                if (index > 0 && !ConfigWindow.GetContextMenuList()[index - 1].Submenu && ConfigWindow.GetContextMenuList()[index - 1].Subitem < subitem)
                    ConfigWindow.GetContextMenuList()[index-1].SetSubmenu(true);
            }
            else
            {
                // Move left

                // If item directly above is a submenu, change it to a normal item
                if (index > 0 && ConfigWindow.GetContextMenuList()[index - 1].Submenu && ConfigWindow.GetContextMenuList()[index - 1].Subitem >= subitem)
                    ConfigWindow.GetContextMenuList()[index - 1].SetSubmenu(false);
                // If item below is subitem, change to submenu
                if (!Submenu && index < ConfigWindow.GetContextMenuList().Count - 1 && ConfigWindow.GetContextMenuList()[index + 1].Subitem == subitem + 1)
                    SetSubmenu(true);
            }

            comboBox_Function.Select(0, 0);

            int originalDepth = Subitem;
            Subitem = subitem;

            // Indent subitems
            if (Submenu && wasSubmenu)
            {
                for (int i = index + 1; i < ConfigWindow.GetContextMenuList().Count - 1; i++)
                {
                    if (ConfigWindow.GetContextMenuList()[i].Subitem != originalDepth + 1)
                        break;
                    wasSubmenu = ConfigWindow.GetContextMenuList()[i].Submenu;
                    ConfigWindow.GetContextMenuList()[i].SetSubitem(Subitem + 1);
                    if (wasSubmenu)
                        break;
                }
            }
        }

        public void AddConfigWindowReference(ConfigWindow configWindow)
        {
            ConfigWindow = configWindow;
        }

        private void button_Delete_Click(object sender, EventArgs e)
        {
            if (ConfigWindow.GetContextMenuList().Count == 1)
                RemoveItemFocus();
            else
            {
                int index = ConfigWindow.GetContextMenuList().IndexOf(this);
                ConfigWindow.GetContextMenuList()[index != 0 ? index - 1 : index + 1].GiveItemFocus();
            }

            int scrollValue = ConfigWindow.GetContextMenuPanelScrollValue();
            ConfigWindow.GetContextMenuList().Remove(this);
            Parent.Controls.Remove(this);
            ConfigWindow.RefreshContextMenuItems(scrollValue);
        }

        private void button_Up_Click(object sender, EventArgs e)
        {
            GiveItemFocus();

            int Index = ConfigWindow.GetContextMenuList().IndexOf(this);
            if (Index == 0)
                return;
            Index = Index - 1;

            ConfigWindow.GetContextMenuList().Remove(this);
            ConfigWindow.GetContextMenuList().Insert(Index, this);
            ConfigWindow.RefreshContextMenuItems();

            // Still a submenu / submenu item?
            if (Submenu)
            {
                SetSubmenu(false);
                if (ConfigWindow.GetContextMenuList().Count >= Index)
                {
                    ConfigWindow.GetContextMenuList()[Index + 1].SetSubitem(Subitem);
                    ConfigWindow.GetContextMenuList()[Index + 1].SetSubmenu(true);
                }
            }
            else if (ConfigWindow.GetContextMenuList().Count >= Index)
            {
                if (ConfigWindow.GetContextMenuList()[Index + 1].Submenu)
                {
                    SetSubitem(ConfigWindow.GetContextMenuList()[Index + 1].Subitem);
                    SetSubmenu(true);

                    ConfigWindow.GetContextMenuList()[Index + 1].SetSubmenu(false);
                    ConfigWindow.GetContextMenuList()[Index + 1].SetSubitem(ConfigWindow.GetContextMenuList()[Index + 1].Subitem + 1);
                }
                else if (ConfigWindow.GetContextMenuList()[Index + 1].Subitem > Subitem)
                {
                    SetSubitem(Subitem + 1);
                }
            }
        }
        private void button_Down_Click(object sender, EventArgs e)
        {
            GiveItemFocus();

            int Index = ConfigWindow.GetContextMenuList().IndexOf(this);
            if (Index == ConfigWindow.GetContextMenuList().Count - 1)
                return;
            Index = Index + 1;

            ConfigWindow.GetContextMenuList().Remove(this);
            ConfigWindow.GetContextMenuList().Insert(Index, this);
            ConfigWindow.RefreshContextMenuItems();

            // Still a submenu / submenu item?
            if (Submenu)
            {
                SetSubmenu(false);
                if (ConfigWindow.GetContextMenuList().Count >= Index)
                {
                    ConfigWindow.GetContextMenuList()[Index - 1].SetSubitem(Subitem);
                    ConfigWindow.GetContextMenuList()[Index - 1].SetSubmenu(true);
                }
                SetSubitem(Subitem + 1);
            }
            else if (ConfigWindow.GetContextMenuList().Count >= Index)
            {
                if (ConfigWindow.GetContextMenuList()[Index - 1].Submenu)
                {
                    SetSubmenu(true);

                    ConfigWindow.GetContextMenuList()[Index - 1].SetSubmenu(false);
                }
                else if (ConfigWindow.GetContextMenuList()[Index - 1].Subitem < Subitem)
                {
                    SetSubitem(ConfigWindow.GetContextMenuList()[Index - 1].Subitem);
                }
            }
        }

        private void button_Left_Click(object sender, EventArgs e)
        {
            GiveItemFocus();

            if (Subitem == 0)
                return;
            SetSubitem(Subitem - 1);
        }

        private void button_Right_Click(object sender, EventArgs e)
        {
            GiveItemFocus();

            if (Subitem >= 3)
                return;
            SetSubitem(Subitem + 1);
        }

        public void GiveItemFocus()
        {
            if (FocusedItem)
                return;

            if (ConfigWindow.ContextMenuItemFocused != null)
                ConfigWindow.ContextMenuItemFocused.RemoveItemFocus();

            FocusedItem = true;
            ConfigWindow.ContextMenuItemFocused = this;

            BackColor = System.Drawing.Color.DeepSkyBlue;
        }
        public void RemoveItemFocus()
        {
            if (!FocusedItem)
                return;

            FocusedItem = false;
            ConfigWindow.ContextMenuItemFocused = null;

            BackColor = System.Drawing.Color.Transparent;
        }

        private void ContextMenuItem_Click(object sender, EventArgs e)
        {
            GiveItemFocus();
        }
        private void textBox_Name_Click(object sender, EventArgs e)
        {
            GiveItemFocus();
        }
        private void comboBox_Function_Click(object sender, EventArgs e)
        {
            GiveItemFocus();
        }

    }
}
