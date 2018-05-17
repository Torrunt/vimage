using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace vimage
{
    class ContextMenu : ContextMenuStrip
    {
        private ImageViewer ImageViewer;
        public int Setting = -1;
        private List<string> Items_General;
        private List<string> Items_Animation;

        private Dictionary<string, dynamic> FuncByName;

        public int FileNameItem = -1;

        public ContextMenu(ImageViewer ImageViewer)
            : base()
        {
            this.ImageViewer = ImageViewer;
        }

        public void LoadItems(List<object> General, List<object> Animation, int AnimationInsertAtIndex)
        {
            FuncByName = new Dictionary<string, dynamic>();

            // General
            Items_General = new List<string>();
            LoadItemsInto(Items_General, General);

            // Animation
            Items_Animation = new List<string>(Items_General);
            List<string> list = new List<string>();

            // inserting into submenu?
            int depth = 0;
            if (Items_Animation[AnimationInsertAtIndex].IndexOf(":") == 0)
                depth = Items_Animation[AnimationInsertAtIndex].Split(':').Length - 1;

            LoadItemsInto(list, Animation, depth);
            Items_Animation.InsertRange(AnimationInsertAtIndex, list);
        }
        private void LoadItemsInto(List<string> list, List<object> items, int depth = 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] is string)
                {
                    // Submenu
                    list.Add(VariableAmountOfStrings(depth, ":") + items[i] + ":");
                    FuncByName.Add(items[i] as string, Action.None);

                    i++;
                    LoadItemsInto(list, (items[i] as List<object>), depth + 1);
                }
                else
                {
                    // Item
                    if (!FuncByName.ContainsKey((items[i] as dynamic).name))
                    {
                        string itemName = (items[i] as dynamic).name;
                        if (itemName.Contains("[filename]"))
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
            if (!force && ((Setting == 0 && !(ImageViewer.Image is AnimatedImage)) || (Setting == 1 && ImageViewer.Image is AnimatedImage)))
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

                if (name.IndexOf(":") == 0)
                {
                    // sub item
                    ToolStripDropDownItem dropDownItem = Items[Items.Count - 1] as ToolStripDropDownItem;
                    ((ToolStripDropDownMenu)dropDownItem.DropDown).ShowImageMargin = ImageViewer.Config.ContextMenuShowMarginSub;
                    name = name.Substring(1);
                    while (name.IndexOf(":") == 0)
                    {
                        if (dropDownItem.DropDownItems.Count > 0)
                            dropDownItem = dropDownItem.DropDownItems[dropDownItem.DropDownItems.Count - 1] as ToolStripDropDownItem;
                        name = name.Substring(1);
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

            ToolStripMenuItem websiteItem = GetItemByFunc(Action.VisitWebsite);
            if (websiteItem != null)
                websiteItem.BackColor = System.Drawing.Color.CornflowerBlue;

            RefreshItems();
        }

        public void RefreshItems()
        {
            if (FileNameItem != -1)
                Items[Items_General[FileNameItem]].Text = Items_General[FileNameItem].Replace("[filename]", ImageViewer.File.Substring(ImageViewer.File.LastIndexOf('\\') + 1));

            if (!ImageViewer.Config.ContextMenuShowMargin && !ImageViewer.Config.ContextMenuShowMarginSub)
                return;

            ToolStripMenuItem item;

            item = GetItemByFunc(Action.Flip);
            if (item != null) item.Checked = ImageViewer.FlippedX;

            item = GetItemByFunc(Action.FitToMonitorHeight);
            if (item != null) item.Checked = ImageViewer.FitToMonitorHeight;

            item = GetItemByFunc(Action.FitToMonitorWidth);
            if (item != null) item.Checked = ImageViewer.FitToMonitorWidth;

            item = GetItemByFunc(Action.ToggleSmoothing);
            if (item != null) item.Checked = ImageViewer.Smoothing();

            item = GetItemByFunc(Action.ToggleMipmapping);
            if (item != null) item.Checked = ImageViewer.Mipmapping();

            item = GetItemByFunc(Action.ToggleBackground);
            if (item != null) item.Checked = ImageViewer.BackgroundsForImagesWithTransparency;

            item = GetItemByFunc(Action.ToggleLock);
            if (item != null) item.Checked = ImageViewer.Locked;

            item = GetItemByFunc(Action.ToggleAlwaysOnTop);
            if (item != null) item.Checked = ImageViewer.AlwaysOnTop;

            item = GetItemByFunc(Action.ToggleTitleBar);
            if (item != null) item.Checked = ImageViewer.Config.Setting_ShowTitleBar;

            item = GetItemByFunc(Action.SortName);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Name;

            item = GetItemByFunc(Action.SortDate);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Date;

            item = GetItemByFunc(Action.SortDateModified);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateModified;

            item = GetItemByFunc(Action.SortDateCreated);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateCreated;

            item = GetItemByFunc(Action.SortSize);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Size;

            item = GetItemByFunc(Action.SortAscending);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Ascending;

            item = GetItemByFunc(Action.SortDescending);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Descending;
        }

        private void ContexMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;

            if (!(item as ToolStripDropDownItem).HasDropDownItems)
                Close();

            object func = FuncByName[item.Name];
            if (func is string)
            {
                for (int i = 0; i < ImageViewer.Config.CustomActions.Count; i++)
                {
                    if ((ImageViewer.Config.CustomActions[i] as dynamic).name == (string)func)
                        ImageViewer.DoCustomAction((ImageViewer.Config.CustomActions[i] as dynamic).func);
                }
            }
            else
                ImageViewer.DoAction((Action)func);
        }

        /// <summary>returns the ToolStripMenuItem based on the name of the function.</summary>
        public ToolStripMenuItem GetItemByFunc(Action func)
        {
            ToolStripMenuItem item = null;
            item = GetItemByFuncFrom(func, ImageViewer.Config.ContextMenu, Items);
            if (item == null)
                item = GetItemByFuncFrom(func, ImageViewer.Config.ContextMenu_Animation, Items);

            return item;
        }
        private ToolStripMenuItem GetItemByFuncFrom(Action func, List<object> list, ToolStripItemCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                int a = 0;
                for (int c = 0; c < list.Count; c++)
                {
                    if (list[c] is string)
                    {
                        // Submenu
                        ToolStripDropDownItem submenu = (collection[(list[c] as string)] as ToolStripDropDownItem);
                        c++;
                        ToolStripMenuItem item = GetItemByFuncFrom(func, (list[c] as List<object>), submenu.DropDownItems);
                        if (item != null)
                            return item;
                    }
                    else if ((list[c] as dynamic).func is Action && (list[c] as dynamic).func == func)
                        return collection[a] as ToolStripMenuItem;

                    if (!(list[c] is string))
                        a++;
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

    }
}
