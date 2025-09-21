using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace vimage
{
    internal class ContextMenu : ContextMenuStrip
    {
        private readonly ImageViewer ImageViewer;
        public int Setting = -1;
        private List<string> Items_General;
        private List<string> Items_Animation;

        private Dictionary<string, dynamic> FuncByName;

        public int FileNameItem = -1;
        public string FileNameCurrent = ".";

        private ToolTip ToolTip;

        public ContextMenu(ImageViewer ImageViewer)
            : base()
        {
            this.ImageViewer = ImageViewer;

            SetupToolTip();
        }

        public void LoadItems(
            List<object> General,
            List<object> Animation,
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

        private void LoadItemsInto(List<string> list, List<object> items, int depth = 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (ImageViewer.File == "")
                {
                    // Remove certain items if there is no file (looking at clipboard image)
                    if (items[i] is string)
                    {
                        // remove Sort By submenu
                        if ((items[i] as string).IndexOf("Sort") == 0)
                        {
                            i++;
                            if (i < items.Count - 1 && (items[i + 1] as dynamic).name == "-")
                                i++;
                            continue;
                        }
                    }
                    else
                    {
                        // remove navigation and delete
                        switch ((items[i] as dynamic).func)
                        {
                            case Action.NextImage:
                            case Action.PrevImage:
                            case Action.Delete:
                                continue;
                        }
                    }
                }

                if (items[i] is string)
                {
                    // Submenu
                    list.Add(VariableAmountOfStrings(depth, ":") + items[i] + ":");
                    FuncByName.Add(items[i] as string, Action.None);

                    i++;
                    LoadItemsInto(list, items[i] as List<object>, depth + 1);
                }
                else
                {
                    // Item
                    if (!FuncByName.ContainsKey((items[i] as dynamic).name))
                    {
                        string itemName = (items[i] as dynamic).name;
                        if (itemName.IndexOf("[filename") == 0)
                            FileNameItem = list.Count;
                        if (itemName.Contains("[version]"))
                            itemName = itemName.Replace("[version]", ImageViewer.VERSION_NO);

                        list.Add(VariableAmountOfStrings(depth, ":") + itemName);
                        if (!itemName.Equals("-"))
                            FuncByName.Add(itemName, (items[i] as dynamic).func);
                    }
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
                ToolStripItem item = null;
                string name = items[i];
                bool itemClickable = true;

                if (name.Length > 0 && name.LastIndexOf(":") == name.Length - 1)
                {
                    // non-clickable item?
                    name = name.Substring(0, name.Length - 1);
                    itemClickable = false;
                }

                if (name.StartsWith(':'))
                {
                    // sub item
                    var dropDownItem = Items[Items.Count - 1] as ToolStripDropDownItem;
                    ((ToolStripDropDownMenu)dropDownItem.DropDown).ShowImageMargin = ImageViewer
                        .Config
                        .ContextMenuShowMarginSub;
                    name = name[1..];
                    while (name.StartsWith(':'))
                    {
                        if (dropDownItem.DropDownItems.Count > 0)
                            dropDownItem =
                                dropDownItem.DropDownItems[dropDownItem.DropDownItems.Count - 1]
                                as ToolStripDropDownItem;
                        name = name[1..];
                    }

                    item = dropDownItem.DropDownItems.Add(name);
                }
                else
                {
                    // item
                    item = Items.Add(name);
                }
                if (name.Equals("-"))
                    continue;

                if (itemClickable)
                    item.Click += ContexMenuItemClicked;

                item.Name = name;
            }

            var websiteItem = GetItemByFunc(Action.VisitWebsite);
            if (websiteItem != null)
                websiteItem.BackColor = System.Drawing.Color.CornflowerBlue;

            RefreshItems();
        }

        public void RefreshItems()
        {
            if (FileNameItem != -1 && FileNameCurrent != ImageViewer.File)
            {
                FileNameCurrent = ImageViewer.File;
                if (Items_General[FileNameItem].Contains("[filename]"))
                {
                    // File Name
                    Items[Items_General[FileNameItem]].Text = Items_General[FileNameItem]
                        .Replace(
                            "[filename]",
                            ImageViewer.File == ""
                                ? "Clipboard Image"
                                : ImageViewer.File.Substring(ImageViewer.File.LastIndexOf('\\') + 1)
                        );
                }
                else if (Items_General[FileNameItem].Contains("[filename"))
                {
                    // File Name (trimmed)
                    int a = Items_General[FileNameItem].IndexOf("[filename.") + 10;
                    int b = Items_General[FileNameItem].IndexOf("]");
                    if (
                        int.TryParse(
                            Items_General[FileNameItem].Substring(a, b - a),
                            out int nameLength
                        )
                    )
                    {
                        string fileName =
                            ImageViewer.File == ""
                                ? "Clipboard Image"
                                : ImageViewer.File.Substring(
                                    ImageViewer.File.LastIndexOf('\\') + 1
                                );
                        string extension =
                            ImageViewer.File == ""
                                ? ""
                                : fileName.Substring(fileName.LastIndexOf("."));
                        if (
                            nameLength >= fileName.Length - 6
                            || fileName.LastIndexOf(".") <= nameLength
                        )
                            nameLength = fileName.Length;
                        Items[Items_General[FileNameItem]].Text =
                            (a > 10 ? Items_General[FileNameItem].Substring(0, a - 10) : "")
                            + (
                                fileName.Length > nameLength
                                    ? fileName.Substring(0, nameLength) + ".." + extension
                                    : fileName
                            )
                            + (
                                b < Items_General[FileNameItem].Length - 1
                                    ? Items_General[FileNameItem].Substring(b + 1)
                                    : ""
                            );
                        Items[Items_General[FileNameItem]].ToolTipText =
                            fileName.Length > nameLength ? fileName : "";
                        Items[Items_General[FileNameItem]].MouseEnter += ItemMouseEnter;
                        Items[Items_General[FileNameItem]].MouseLeave += ItemMouseLeave;
                    }
                }
            }

            if (
                !ImageViewer.Config.ContextMenuShowMargin
                && !ImageViewer.Config.ContextMenuShowMarginSub
            )
                return;

            ToolStripMenuItem item;

            item = GetItemByFunc(Action.Flip);
            if (item != null)
                item.Checked = ImageViewer.FlippedX;

            item = GetItemByFunc(Action.FitToMonitorHeight);
            if (item != null)
                item.Checked = ImageViewer.FitToMonitorHeight;

            item = GetItemByFunc(Action.FitToMonitorWidth);
            if (item != null)
                item.Checked = ImageViewer.FitToMonitorWidth;

            item = GetItemByFunc(Action.ToggleSmoothing);
            if (item != null)
                item.Checked = ImageViewer.Smoothing();

            item = GetItemByFunc(Action.ToggleBackground);
            if (item != null)
                item.Checked = ImageViewer.BackgroundsForImagesWithTransparency;

            item = GetItemByFunc(Action.ToggleLock);
            if (item != null)
                item.Checked = ImageViewer.Locked;

            item = GetItemByFunc(Action.ToggleAlwaysOnTop);
            if (item != null)
                item.Checked = ImageViewer.AlwaysOnTop;

            item = GetItemByFunc(Action.ToggleTitleBar);
            if (item != null)
                item.Checked = ImageViewer.Config.Setting_ShowTitleBar;

            item = GetItemByFunc(Action.SortName);
            if (item != null)
                item.Checked = ImageViewer.SortImagesBy == SortBy.Name;

            item = GetItemByFunc(Action.SortDate);
            if (item != null)
                item.Checked = ImageViewer.SortImagesBy == SortBy.Date;

            item = GetItemByFunc(Action.SortDateModified);
            if (item != null)
                item.Checked = ImageViewer.SortImagesBy == SortBy.DateModified;

            item = GetItemByFunc(Action.SortDateCreated);
            if (item != null)
                item.Checked = ImageViewer.SortImagesBy == SortBy.DateCreated;

            item = GetItemByFunc(Action.SortSize);
            if (item != null)
                item.Checked = ImageViewer.SortImagesBy == SortBy.Size;

            item = GetItemByFunc(Action.SortAscending);
            if (item != null)
                item.Checked = ImageViewer.SortImagesByDir == SortDirection.Ascending;

            item = GetItemByFunc(Action.SortDescending);
            if (item != null)
                item.Checked = ImageViewer.SortImagesByDir == SortDirection.Descending;
        }

        private void ContexMenuItemClicked(object sender, EventArgs e)
        {
            var item = sender as ToolStripItem;

            if (!(item as ToolStripDropDownItem).HasDropDownItems)
                Close();

            object func = FuncByName[item.Name];
            if (func is string @funcName)
            {
                for (int i = 0; i < ImageViewer.Config.CustomActions.Count; i++)
                {
                    if ((ImageViewer.Config.CustomActions[i] as dynamic).name == @funcName)
                        ImageViewer.DoCustomAction(
                            (ImageViewer.Config.CustomActions[i] as dynamic).func
                        );
                }
            }
            else
                ImageViewer.DoAction((Action)func);
        }

        /// <summary>returns the ToolStripMenuItem based on the name of the function.</summary>
        public ToolStripMenuItem GetItemByFunc(Action func)
        {
            return GetItemByFuncFrom(func, Items);
        }

        private ToolStripMenuItem GetItemByFuncFrom(Action func, ToolStripItemCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Name == "")
                    continue;
                object currentFunc = FuncByName[collection[i].Name];
                if (currentFunc is Action action && action == func)
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

        private void ToolTipDraw(object sender, DrawToolTipEventArgs e)
        {
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

        private void ItemMouseEnter(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            ToolTip.Show(
                item.ToolTipText,
                item.Owner,
                item.Bounds.Location.X + 8,
                item.Bounds.Location.Y + 1
            );
        }

        private void ItemMouseLeave(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            ToolTip.Hide(item.Owner);
        }
    }
}
