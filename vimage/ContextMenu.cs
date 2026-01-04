using System;
using System.Collections.Generic;
using System.Windows.Forms;
using vimage.Common;
using vimage.Display;
using Action = vimage.Common.Action;

namespace vimage
{
    internal class ContextMenu : ContextMenuStrip
    {
        private readonly ImageViewer ImageViewer;
        public int Setting = -1;
        private List<string> Items_General = [];
        private List<string> Items_Animation = [];

        private Dictionary<string, ContextMenuFunc> FuncByName = [];

        public int FileNameItem = -1;
        public string FileNameCurrent = ".";

        private ToolTip? ToolTip;

        public ContextMenu(ImageViewer ImageViewer)
            : base()
        {
            this.ImageViewer = ImageViewer;

            SetupToolTip();
        }

        public void LoadItems(
            List<ContextMenuItem> General,
            List<ContextMenuItem> Animation,
            int AnimationInsertAtIndex
        )
        {
            FuncByName = [];

            // General
            Items_General = [];
            LoadItemsInto(Items_General, General);

            // Animation
            Items_Animation = [.. Items_General];
            List<string> list = [];

            // inserting into submenu?
            int depth = 0;
            if (Items_Animation[AnimationInsertAtIndex].StartsWith(':'))
                depth = Items_Animation[AnimationInsertAtIndex].Split(':').Length - 1;

            LoadItemsInto(list, Animation, depth);
            Items_Animation.InsertRange(AnimationInsertAtIndex, list);
        }

        private void LoadItemsInto(List<string> list, List<ContextMenuItem> items, int depth = 0)
        {
            foreach (var item in items)
            {
                if (ImageViewer.File == "")
                {
                    // Remove certain items if there is no file (looking at clipboard image)
                    if (item.func == null && item.name.StartsWith("Sort"))
                    {
                        // remove Sort By submenu
                        continue;
                    }
                    if (item.func is FuncAction funcAction)
                    {
                        // remove navigation and delete
                        switch (funcAction.Value)
                        {
                            case Action.NextImage:
                            case Action.PrevImage:
                            case Action.Delete:
                                continue;
                        }
                    }
                }

                if (item.children != null && item.children.Count > 0)
                {
                    // Submenu
                    list.Add(VariableAmountOfStrings(depth, ":") + item.name + ":");
                    FuncByName.Add(item.name, new FuncAction(Action.None));
                    LoadItemsInto(list, item.children, depth + 1);
                }
                else
                {
                    // Item
                    if (FuncByName.ContainsKey(item.name))
                        continue;

                    var itemName = item.name;
                    if (itemName.StartsWith("[filename"))
                        FileNameItem = list.Count;
                    if (itemName.Contains("[version]"))
                        itemName = itemName.Replace("[version]", ImageViewer.VERSION_NO);

                    list.Add(VariableAmountOfStrings(depth, ":") + itemName);
                    if (
                        item.func != null
                        && !(item.func is FuncAction action && action.Value == Action.None)
                    )
                        FuncByName.Add(itemName, item.func);
                }
            }
        }

        public void Setup(bool force)
        {
            if (
                !force
                && (
                    (Setting == 0 && ImageViewer.Image is not AnimatedImage)
                    || (Setting == 1 && ImageViewer.Image is AnimatedImage)
                )
            )
                return;

            Items.Clear();
            ShowImageMargin = ImageViewer.Config.ContextMenuShowMargin;

            List<string> items;
            if (ImageViewer.Image is AnimatedImage)
            {
                Setting = 1;
                items = Items_Animation;
            }
            else
            {
                Setting = 0;
                items = Items_General;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ToolStripItem? item = null;
                string name = items[i];
                bool itemClickable = true;

                if (name.Length > 0 && name.LastIndexOf(':') == name.Length - 1)
                {
                    // non-clickable item?
                    name = name[..^1];
                    itemClickable = false;
                }

                if (name.StartsWith(':'))
                {
                    // sub item
                    if (Items[Items.Count - 1] is ToolStripDropDownItem dropDownItem)
                    {
                        if (dropDownItem.DropDown is ToolStripDropDownMenu dropDownMenu)
                        {
                            dropDownMenu.ShowImageMargin = ImageViewer
                                .Config
                                .ContextMenuShowMarginSub;
                        }
                        name = name[1..];
                        while (name.StartsWith(':'))
                        {
                            if (
                                dropDownItem.DropDownItems.Count > 0
                                && dropDownItem.DropDownItems[dropDownItem.DropDownItems.Count - 1]
                                    is ToolStripDropDownItem subDropDownitem
                            )
                            {
                                dropDownItem = subDropDownitem;
                            }
                            name = name[1..];
                        }

                        item = dropDownItem.DropDownItems.Add(name);
                    }
                }
                else
                {
                    // item
                    item = Items.Add(name);
                }
                if (name.Equals("-"))
                    continue;

                if (item is not null)
                {
                    if (itemClickable)
                        item.Click += ContexMenuItemClicked;
                    item.Name = name;
                }
            }

            GetItemByFunc(Action.VisitWebsite)?.BackColor = System.Drawing.Color.CornflowerBlue;

            RefreshItems();
        }

        public void RefreshItems()
        {
            if (FileNameItem != -1 && FileNameCurrent != ImageViewer.File)
            {
                FileNameCurrent = ImageViewer.File;
                var item = Items_General[FileNameItem];
                if (item.Contains("[filename]"))
                {
                    // File Name
                    var fileNameItem = Items[item];
                    fileNameItem?.Text = item.Replace(
                        "[filename]",
                        ImageViewer.File == ""
                            ? "Clipboard Image"
                            : ImageViewer.File[(ImageViewer.File.LastIndexOf('\\') + 1)..]
                    );
                }
                else if (item.Contains("[filename"))
                {
                    // File Name (trimmed)
                    int a = item.IndexOf("[filename.") + 10;
                    int b = item.IndexOf(']');
                    if (int.TryParse(item[a..b], out int nameLength))
                    {
                        var fileName =
                            ImageViewer.File == ""
                                ? "Clipboard Image"
                                : ImageViewer.File[(ImageViewer.File.LastIndexOf('\\') + 1)..];
                        var extension =
                            ImageViewer.File == "" ? "" : fileName[fileName.LastIndexOf('.')..];
                        if (
                            nameLength >= fileName.Length - 6
                            || fileName.LastIndexOf('.') <= nameLength
                        )
                            nameLength = fileName.Length;

                        var fileNameItem = Items[item];
                        if (fileNameItem is not null)
                        {
                            fileNameItem.Text =
                                (a > 10 ? item[..(a - 10)] : "")
                                + (
                                    fileName.Length > nameLength
                                        ? fileName[..nameLength] + ".." + extension
                                        : fileName
                                )
                                + (b < item.Length - 1 ? item[(b + 1)..] : "");
                            fileNameItem.ToolTipText = fileName.Length > nameLength ? fileName : "";
                            fileNameItem.MouseEnter += ItemMouseEnter;
                            fileNameItem.MouseLeave += ItemMouseLeave;
                        }
                    }
                }
            }

            if (
                !ImageViewer.Config.ContextMenuShowMargin
                && !ImageViewer.Config.ContextMenuShowMarginSub
            )
                return;

            GetItemByFunc(Action.Flip)?.Checked = ImageViewer.FlippedX;
            GetItemByFunc(Action.FitToMonitorHeight)?.Checked = ImageViewer.FitToMonitorHeight;
            GetItemByFunc(Action.FitToMonitorWidth)?.Checked = ImageViewer.FitToMonitorWidth;
            GetItemByFunc(Action.ToggleSmoothing)?.Checked = ImageViewer.Smoothing();
            GetItemByFunc(Action.ToggleBackground)?.Checked =
                ImageViewer.BackgroundsForImagesWithTransparency;
            GetItemByFunc(Action.ToggleLock)?.Checked = ImageViewer.Locked;
            GetItemByFunc(Action.ToggleAlwaysOnTop)?.Checked = ImageViewer.AlwaysOnTop;
            GetItemByFunc(Action.ToggleTitleBar)?.Checked = ImageViewer.Config.ShowTitleBar;
            GetItemByFunc(Action.SortName)?.Checked = ImageViewer.SortImagesBy == SortBy.Name;
            GetItemByFunc(Action.SortDate)?.Checked = ImageViewer.SortImagesBy == SortBy.Date;
            GetItemByFunc(Action.SortDateModified)?.Checked =
                ImageViewer.SortImagesBy == SortBy.DateModified;
            GetItemByFunc(Action.SortDateCreated)?.Checked =
                ImageViewer.SortImagesBy == SortBy.DateCreated;
            GetItemByFunc(Action.SortSize)?.Checked = ImageViewer.SortImagesBy == SortBy.Size;
            GetItemByFunc(Action.SortAscending)?.Checked =
                ImageViewer.SortImagesByDir == SortDirection.Ascending;
            GetItemByFunc(Action.SortDescending)?.Checked =
                ImageViewer.SortImagesByDir == SortDirection.Descending;
        }

        private void ContexMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is not ToolStripItem item)
                return;

            if (
                item is ToolStripDropDownItem toolStripDropDownItem
                && !toolStripDropDownItem.HasDropDownItems
            )
                Close();

            var func = FuncByName[item.Name ?? ""];
            if (func is FuncString funcString)
                ImageViewer.DoCustomAction(funcString.Value);
            else if (func is FuncAction funcAction)
                ImageViewer.DoAction(funcAction.Value);
        }

        /// <summary>returns the ToolStripMenuItem based on the name of the function.</summary>
        public ToolStripMenuItem? GetItemByFunc(Action func)
        {
            return GetItemByFuncFrom(func, Items);
        }

        private ToolStripMenuItem? GetItemByFuncFrom(
            Action func,
            ToolStripItemCollection collection
        )
        {
            for (int i = 0; i < collection.Count; i++)
            {
                var name = collection[i].Name;
                if (name is null || name == "")
                    continue;
                FuncByName.TryGetValue(name, out var currentFunc);
                if (currentFunc is FuncAction action && action.Value == func)
                    return collection[i] as ToolStripMenuItem;

                if (
                    collection[i] is ToolStripDropDownItem toolStripDropDownItem
                    && toolStripDropDownItem.DropDownItems.Count > 0
                )
                {
                    var item = GetItemByFuncFrom(func, toolStripDropDownItem.DropDownItems);
                    if (item != null)
                        return item;
                }
            }
            return null;
        }

        private static string VariableAmountOfStrings(int amount, string s)
        {
            if (amount == 0)
                return "";

            string str = "";
            for (int i = 0; i < amount; i++)
                str += s;
            return str;
        }

        private void SetupToolTip()
        {
            ShowItemToolTips = false;
            ToolTip = new ToolTip { UseAnimation = true, UseFading = true };
            if (SystemInformation.HighContrast)
            {
                ToolTip.BackColor = System.Drawing.Color.FromArgb(26, 255, 255);
                ToolTip.ForeColor = System.Drawing.Color.Black;
            }
            else
                ToolTip.BackColor = System.Drawing.Color.FromArgb(196, 225, 255);
            ToolTip.OwnerDraw = true;
            ToolTip.Draw += new DrawToolTipEventHandler(ToolTipDraw);
        }

        private void ToolTipDraw(object? sender, DrawToolTipEventArgs e)
        {
            if (ToolTip is null)
                return;
            var bounds = e.Bounds;
            bounds.Height -= 1;
            var newArgs = new DrawToolTipEventArgs(
                e.Graphics,
                e.AssociatedWindow,
                e.AssociatedControl,
                bounds,
                e.ToolTipText,
                ToolTip.BackColor,
                ToolTip.ForeColor,
                e.Font
            );
            newArgs.DrawBackground();
            newArgs.DrawText(TextFormatFlags.VerticalCenter);
        }

        private void ItemMouseEnter(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem item || item.Owner is null)
                return;
            ToolTip?.Show(
                item.ToolTipText,
                item.Owner,
                item.Bounds.Location.X + 8,
                item.Bounds.Location.Y + 1
            );
        }

        private void ItemMouseLeave(object? sender, EventArgs e)
        {
            if (sender is not ToolStripMenuItem item || item.Owner is null)
                return;
            ToolTip?.Hide(item.Owner);
        }
    }
}
