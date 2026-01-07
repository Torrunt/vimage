using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using vimage.Common;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for ContextMenu.xaml
    /// </summary>
    public partial class ContextMenu : UserControl
    {
        public List<ContextMenuRow> Items = [];
        public ContextMenuRow? CurrentItemSelection = null;

        public ContextMenu()
        {
            InitializeComponent();
            DataContext = App.Config;

            if (App.Config == null)
                return;
            LoadItems(
                App.Config.ContextMenu,
                ContextMenuItems_General,
                ContextMenuItems_GeneralCanvas,
                ContextMenuItems_GeneralScroll
            );
            LoadItems(
                App.Config.ContextMenu_Animation,
                ContextMenuItems_Animation,
                ContextMenuItems_AnimationCanvas,
                ContextMenuItems_AnimationScroll
            );
        }

        public void Save()
        {
            if (App.Config == null)
                return;
            App.Config.ContextMenu.Clear();
            App.Config.ContextMenu_Animation.Clear();

            App.Config.ContextMenu = GetSavedContextMenu(ContextMenuItems_General);
            App.Config.ContextMenu_Animation = GetSavedContextMenu(ContextMenuItems_Animation);
        }

        private static List<ContextMenuItem> GetSavedContextMenu(Panel panel)
        {
            List<ContextMenuItem> contextMenu = [];
            var stack = new Stack<(int indent, ContextMenuItem item)>();
            foreach (var child in panel.Children)
            {
                if (child is not ContextMenuRow row)
                    continue;

                var item = row.GetAsContextMenuItem();
                int indent = row.Indent;

                while (stack.Count > 0 && stack.Peek().indent >= indent)
                    stack.Pop(); // pop until we find the correct parent level

                if (stack.Count > 0)
                {
                    // Sub item
                    var previousItem = stack.Peek().item;
                    previousItem.Children ??= [];
                    previousItem.Children.Add(item);
                }
                else
                    contextMenu.Add(item);

                stack.Push((indent, item));
            }

            return contextMenu;
        }

        private void LoadItems(
            List<ContextMenuItem> items,
            Panel panel,
            ContextMenuEditorCanvas canvas,
            ScrollViewer scroll,
            int indent = 0
        )
        {
            foreach (var item in items)
            {
                var row = new ContextMenuRow(
                    item.Name,
                    item.Func,
                    this,
                    panel,
                    canvas,
                    scroll,
                    indent
                );
                _ = panel.Children.Add(row);
                Items.Add(row);

                Canvas.SetTop(row, row.MinHeight * (panel.Children.Count - 1));

                if (item.Children != null && item.Children.Count > 0)
                    LoadItems(item.Children, panel, canvas, scroll, indent + 1);
            }
        }

        public void UpdateCustomActions()
        {
            for (int i = 0; i < Items.Count; i++)
                Items[i].UpdateCustomActions();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var panel =
                Tabs.SelectedIndex == 0 ? ContextMenuItems_General : ContextMenuItems_Animation;
            var canvas =
                Tabs.SelectedIndex == 0
                    ? ContextMenuItems_GeneralCanvas
                    : ContextMenuItems_AnimationCanvas;
            var scroll =
                Tabs.SelectedIndex == 0
                    ? ContextMenuItems_GeneralScroll
                    : ContextMenuItems_AnimationScroll;

            var item = new ContextMenuRow(
                "",
                new ActionEnum(Action.None),
                this,
                panel,
                canvas,
                scroll,
                CurrentItemSelection == null ? 0 : CurrentItemSelection.Indent
            );
            if (CurrentItemSelection == null)
            {
                _ = panel.Children.Add(item);
                Items.Add(item);

                Canvas.SetTop(item, item.MinHeight * (panel.Children.Count - 1));
            }
            else
            {
                panel.Children.Insert(panel.Children.IndexOf(CurrentItemSelection) + 1, item);
                Items.Insert(Items.IndexOf(CurrentItemSelection) + 1, item);
            }

            CurrentItemSelection?.UnselectItem();
            item.SelectItem(true);
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            if (App.Config == null)
                return;

            ContextMenuItems_General.Children.Clear();
            ContextMenuItems_Animation.Children.Clear();
            Items.Clear();

            var defaultConfig = new Config();
            App.Config.ContextMenu = [.. defaultConfig.ContextMenu];
            App.Config.ContextMenu_Animation = [.. defaultConfig.ContextMenu_Animation];

            LoadItems(
                App.Config.ContextMenu,
                ContextMenuItems_General,
                ContextMenuItems_GeneralCanvas,
                ContextMenuItems_GeneralScroll
            );
            LoadItems(
                App.Config.ContextMenu_Animation,
                ContextMenuItems_Animation,
                ContextMenuItems_AnimationCanvas,
                ContextMenuItems_AnimationScroll
            );
        }
    }
}
