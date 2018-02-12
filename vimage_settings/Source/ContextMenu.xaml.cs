using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ContextMenu.xaml
    /// </summary>
    public partial class ContextMenu : UserControl
    {
        public List<ContextMenuItem> Items = new List<ContextMenuItem>();
        public ContextMenuItem CurrentItemSelection = null;

        public ContextMenu()
        {
            InitializeComponent();
            DataContext = App.vimageConfig;

            if (App.vimageConfig == null)
                return;
            LoadItems(App.vimageConfig.ContextMenu, ContextMenuItems_General, ContextMenuItems_GeneralCanvas, ContextMenuItems_GeneralScroll);
            LoadItems(App.vimageConfig.ContextMenu_Animation, ContextMenuItems_Animation, ContextMenuItems_AnimationCanvas, ContextMenuItems_AnimationScroll);
        }

        public void Save()
        {
            App.vimageConfig.ContextMenu.Clear();
            App.vimageConfig.ContextMenu_Animation.Clear();

            SaveContextMenu(App.vimageConfig.ContextMenu, ContextMenuItems_General);
            SaveContextMenu(App.vimageConfig.ContextMenu_Animation, ContextMenuItems_Animation);
        }
        private void SaveContextMenu(List<object> contextMenu, Panel panel)
        {
            int currentSubLevel = 0;
            List<object> currentMenu = contextMenu;
            List<object> prevMenu = null;
            for (int i = 0; i < panel.Children.Count; i++)
            {
                ContextMenuItem item = (ContextMenuItem)panel.Children[i];
                if (i < panel.Children.Count - 1 && ((ContextMenuItem)panel.Children[i + 1]).Indent > item.Indent)
                {
                    // Submenu
                    currentMenu.Add(item.ItemName.Text);
                }
                else if (item.Indent != 0)
                {
                    // Subitem
                    if (item.Indent > currentSubLevel)
                    {
                        // First subitem
                        currentSubLevel = item.Indent;
                        currentMenu.Add(new List<object>());
                        prevMenu = currentMenu;
                        currentMenu = currentMenu[currentMenu.Count - 1] as List<object>;
                    }
                    else if (item.Indent < currentSubLevel)
                    {
                        currentSubLevel = item.Indent;
                        currentMenu = prevMenu;
                    }

                    currentMenu.Add(new { name = item.ItemName.Text, func = item.ItemFunction.Text.Trim() });
                }
                else
                {
                    // Item
                    if (currentSubLevel != 0)
                    {
                        currentSubLevel = 0;
                        currentMenu = contextMenu;
                    }

                    currentMenu.Add(new { name = item.ItemName.Text, func = item.ItemFunction.Text.Trim() });
                }
            }
        }

        private void LoadItems(List<object> items, Panel panel, ContextMenuEditorCanvas canvas, ScrollViewer scroll, int indent = 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                dynamic o = (items[i] as dynamic);

                if (o is List<object>)
                {
                    LoadItems(o, panel, canvas, scroll, indent + 1);
                }
                else
                {
                    ContextMenuItem item = new ContextMenuItem(o is string ? o : o.name, o is string ? "" : o.func, this, panel, canvas, scroll, indent);
                    panel.Children.Add(item);
                    Items.Add(item);

                    Canvas.SetTop(item, item.MinHeight * (panel.Children.Count - 1));
                }
            }
        }

        public void UpdateCustomActions()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].UpdateCustomActions();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Panel panel = Tabs.SelectedIndex == 0 ? ContextMenuItems_General : ContextMenuItems_Animation;
            ContextMenuEditorCanvas canvas = Tabs.SelectedIndex == 0 ? ContextMenuItems_GeneralCanvas : ContextMenuItems_AnimationCanvas;
            ScrollViewer scroll = Tabs.SelectedIndex == 0 ? ContextMenuItems_GeneralScroll : ContextMenuItems_AnimationScroll;

            ContextMenuItem item = new ContextMenuItem("", "", this, panel, canvas, scroll, CurrentItemSelection == null ? 0 : CurrentItemSelection.Indent);
            if (CurrentItemSelection == null)
            {
                panel.Children.Add(item);
                Items.Add(item);

                Canvas.SetTop(item, item.MinHeight * (panel.Children.Count - 1));
            }
            else
            {
                panel.Children.Insert(panel.Children.IndexOf(CurrentItemSelection) + 1, item);
                Items.Insert(Items.IndexOf(CurrentItemSelection) + 1, item);
            }

            CurrentItemSelection.UnselectItem();
            item.SelectItem(true);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            if (App.vimageConfig == null)
                return;

            ContextMenuItems_General.Children.Clear();
            ContextMenuItems_Animation.Children.Clear();
            Items.Clear();
            
            App.vimageConfig.SetDefaultContextMenu();

            LoadItems(App.vimageConfig.ContextMenu, ContextMenuItems_General, ContextMenuItems_GeneralCanvas, ContextMenuItems_GeneralScroll);
            LoadItems(App.vimageConfig.ContextMenu_Animation, ContextMenuItems_Animation, ContextMenuItems_AnimationCanvas, ContextMenuItems_AnimationScroll);
        }

    }
}
