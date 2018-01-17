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

        private Dictionary<string, string> FuncByName;

        public ContextMenu(ImageViewer ImageViewer)
            : base()
        {
            this.ImageViewer = ImageViewer;
        }

        public void LoadItems(List<object> General, List<object> Animation, int AnimationInsertAtIndex)
        {
            FuncByName = new Dictionary<string, string>();

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
                    FuncByName.Add(items[i] as string, items[i] as string);

                    i++;
                    LoadItemsInto(list, (items[i] as List<object>), depth + 1);
                }
                else
                {
                    // Item
                    if (((items[i] as dynamic).func as string).Equals(MenuFuncs.VERSION_NAME))
                    {
                        list.Add(ImageViewer.VERSION_NAME);
                        FuncByName.Add(ImageViewer.VERSION_NAME, (items[i] as dynamic).func);
                    }
                    else
                    {
                        list.Add(VariableAmountOfStrings(depth, ":") + (items[i] as dynamic).name);
                        if (!((items[i] as dynamic).name as string).Equals("-"))
                            FuncByName.Add((items[i] as dynamic).name, (items[i] as dynamic).func);
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

                if (name.LastIndexOf(":") == name.Length - 1)
                {
                    // non-clickable item?
                    name = name.Substring(0, name.Length - 1);
                    itemClickable = false;
                }

                if (name.IndexOf(":") == 0)
                {
                    // sub item
                    ToolStripDropDownItem dropDownItem = Items[Items.Count - 1] as ToolStripDropDownItem;
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

            if (Items.ContainsKey(ImageViewer.VERSION_NAME))
                ((ToolStripMenuItem)Items[ImageViewer.VERSION_NAME]).BackColor = System.Drawing.Color.CornflowerBlue;

            RefreshItems();
        }

        public void RefreshItems()
        {
            ToolStripMenuItem item;

            item = GetItemByFunc(MenuFuncs.FLIP);
            if (item != null) item.Checked = ImageViewer.FlippedX;

            item = GetItemByFunc(MenuFuncs.FIT_TO_HEIGHT);
            if (item != null) item.Checked = ImageViewer.FitToMonitorHeight;

            item = GetItemByFunc(MenuFuncs.TOGGLE_SMOOTHING);
            if (item != null) item.Checked = ImageViewer.Smoothing();

            item = GetItemByFunc(MenuFuncs.TOGGLE_BACKGROUND);
            if (item != null) item.Checked = ImageViewer.BackgroundsForImagesWithTransparency;

            item = GetItemByFunc(MenuFuncs.ALWAYS_ON_TOP);
            if (item != null) item.Checked = ImageViewer.AlwaysOnTop;


            item = GetItemByFunc(MenuFuncs.SORT_NAME);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Name;

            item = GetItemByFunc(MenuFuncs.SORT_DATE);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Date;

            item = GetItemByFunc(MenuFuncs.SORT_DATE_MODIFIED);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateModified;

            item = GetItemByFunc(MenuFuncs.SORT_DATE_CREATED);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateCreated;

            item = GetItemByFunc(MenuFuncs.SORT_SIZE);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Size;

            item = GetItemByFunc(MenuFuncs.SORT_ASCENDING);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Ascending;

            item = GetItemByFunc(MenuFuncs.SORT_DESCENDING);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Descending;
        }

        private void ContexMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;

            if (!(item as ToolStripDropDownItem).HasDropDownItems)
                Close();

            string func = FuncByName[item.Name];
            switch (func)
            {
                case MenuFuncs.CLOSE: ImageViewer.CloseNextTick = true; return;

                case MenuFuncs.NEXT_IMAGE: ImageViewer.NextImage(); return;
                case MenuFuncs.PREV_IMAGE: ImageViewer.PrevImage(); return;

                case MenuFuncs.SORT_NAME: ImageViewer.ChangeSortBy(SortBy.Name); return;
                case MenuFuncs.SORT_DATE: ImageViewer.ChangeSortBy(SortBy.Date); return;
                case MenuFuncs.SORT_DATE_MODIFIED: ImageViewer.ChangeSortBy(SortBy.DateModified); return;
                case MenuFuncs.SORT_DATE_CREATED: ImageViewer.ChangeSortBy(SortBy.DateCreated); return;
                case MenuFuncs.SORT_SIZE: ImageViewer.ChangeSortBy(SortBy.Size); return;
                case MenuFuncs.SORT_ASCENDING: ImageViewer.ChangeSortByDirection(SortDirection.Ascending); return;
                case MenuFuncs.SORT_DESCENDING: ImageViewer.ChangeSortByDirection(SortDirection.Descending); return;

                case MenuFuncs.NEXT_FRAME: ImageViewer.NextFrame(); return;
                case MenuFuncs.PREV_FRAME: ImageViewer.PrevFrame(); return;
                case MenuFuncs.TOGGLE_ANIMATION: ImageViewer.ToggleAnimation(); return;

                case MenuFuncs.ROTATE_CLOCKWISE: ImageViewer.RotateImage((int)ImageViewer.Image.Rotation + 90); return;
                case MenuFuncs.ROTATE_ANTICLOCKWISE: ImageViewer.RotateImage((int)ImageViewer.Image.Rotation - 90); return;
                case MenuFuncs.FLIP: ImageViewer.FlipImage(); return;
                case MenuFuncs.FIT_TO_HEIGHT: ImageViewer.ToggleFitToMonitor(Config.HEIGHT); return;
                case MenuFuncs.FIT_TO_WIDTH: ImageViewer.ToggleFitToMonitor(Config.WIDTH); return;
                case MenuFuncs.FIT_TO_AUTO: ImageViewer.ToggleFitToMonitor(Config.AUTO); return;
                case MenuFuncs.RESET_IMAGE: ImageViewer.ResetImage(); return;
                case MenuFuncs.TOGGLE_SMOOTHING: ImageViewer.ToggleSmoothing(); return;
                case MenuFuncs.TOGGLE_BACKGROUND: ImageViewer.ToggleBackground(); return;
                case MenuFuncs.ALWAYS_ON_TOP: ImageViewer.ToggleAlwaysOnTop(); return;

                case MenuFuncs.OPEN_FILE_LOCATION: ImageViewer.OpenFileAtLocation(); return;
                case MenuFuncs.DELETE: ImageViewer.DeleteFile(); return;
                case MenuFuncs.COPY: ImageViewer.CopyFile(); return;
                case MenuFuncs.COPY_AS_IMAGE: ImageViewer.CopyAsImage(); return;
                case MenuFuncs.OPEN_DUPLICATE: ImageViewer.OpenDuplicateWindow(); return;
                case MenuFuncs.RANDOM_IMAGE: ImageViewer.RandomImage(); return;

                case MenuFuncs.OPEN_SETTINGS: ImageViewer.OpenConfig(); return;
                case MenuFuncs.RELOAD_SETTINGS: ImageViewer.ReloadConfig(); return;

                case MenuFuncs.VERSION_NAME: Process.Start("http://torrunt.net/vimage"); return;
            }

            for (int i = 0; i < ImageViewer.Config.CustomActions.Count; i++)
            {
                if ((ImageViewer.Config.CustomActions[i] as dynamic).name == func)
                    ImageViewer.DoCustomAction((ImageViewer.Config.CustomActions[i] as dynamic).func);
            }
        }

        /// <summary>returns the ToolStripMenuItem based on the name of the function.</summary>
        public ToolStripMenuItem GetItemByFunc(string func)
        {
            ToolStripMenuItem item = null;
            item = GetItemByFuncFrom(func, ImageViewer.Config.ContextMenu, Items);
            if (item == null)
                item = GetItemByFuncFrom(func, ImageViewer.Config.ContextMenu_Animation, Items);

            return item;
        }
        private ToolStripMenuItem GetItemByFuncFrom(string func, List<object> list, ToolStripItemCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
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
                    else if ((list[c] as dynamic).func == func)
                        return collection[(list[c] as dynamic).name] as ToolStripMenuItem;
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
