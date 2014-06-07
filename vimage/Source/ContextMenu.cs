using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace vimage
{
    class ContextMenu : ContextMenuStrip
    {
        private ImageViewer ImageViewer;
        public int Setting = -1;
        private List<string> Items_General;
        private List<string> Items_Animation;

        private Dictionary<string, string> FuncByName;

        public const string CLOSE = "CLOSE";
        public const string NEXT_IMAGE = "NEXTIMAGE";
        public const string PREV_IMAGE = "PREVIMAGE";
        public const string ROTATE_CLOCKWISE = "ROTATECLOCKWISE";
        public const string ROTATE_ANTICLOCKWISE = "ROTATEANTICLOCKWISE";
        public const string FLIP = "FLIP";
        public const string FIT_TO_HEIGHT = "FITTOHEIGHT";
        public const string RESET_IMAGE = "RESETIMAGE";
        public const string TOGGLE_SMOOTHING = "TOGGLESMOOTHING";
        public const string TOGGLE_BACKGROUND = "TOGGLEBACKGROUND";
        public const string ALWAYS_ON_TOP = "ALWAYSONTOP";
        public const string OPEN_FILE_LOCATION = "OPENFILELOCATION";
        public const string DELETE = "DELETE";
        public const string OPEN_SETTINGS = "OPENSETTINGS";
        public const string RELOAD_SETTINGS = "RELOADSETTINGS";
        public const string VERSION_NAME = "VERSIONNAME";

        public const string SORT_NAME = "SORTNAME";
        public const string SORT_DATE_MODIFIED = "SORTDATEMODIFIED";
        public const string SORT_DATE_CREATED = "SORTDATECREATED";
        public const string SORT_SIZE = "SORTSIZE";
        public const string SORT_ASCENDING = "SORTASCENDING";
        public const string SORT_DESCENDING = "SORTDESCENDING";

        public const string NEXT_FRAME = "NEXTFRAME";
        public const string PREV_FRAME = "PREVFRAME";
        public const string TOGGLE_ANIMATION = "TOGGLEANIMATION";


        public ContextMenu(ImageViewer ImageViewer)
            : base()
        {
            this.ImageViewer = ImageViewer;
        }

        public void LoadItems(List<object> General, List<object> Animation, int AnimationInsertAtIndex)
        {
            FuncByName = new Dictionary<string, string>();

            Items_General = new List<string>();
            for (int i = 0; i < General.Count; i++)
            {
                if (General[i] is string)
                {
                    // Submenu
                    Items_General.Add(General[i] + ":");
                    FuncByName.Add(General[i] as string, General[i] as string);
                }
                else if (General[i] is List<object>)
                {
                    // Submenu Item
                    for (int s = 0; s < (General[i] as List<object>).Count; s++)
                    {
                        Items_General.Add(":" + ((General[i] as List<object>)[s] as dynamic).name);
                        if (!(((General[i] as List<object>)[s] as dynamic).name as string).Equals("-"))
                            FuncByName.Add(((General[i] as List<object>)[s] as dynamic).name, ((General[i] as List<object>)[s] as dynamic).func);
                    }
                }
                else
                {
                    // Item
                    if (((General[i] as dynamic).func as string).Equals(VERSION_NAME))
                    {
                        Items_General.Add(ImageViewer.VERSION_NAME);
                        FuncByName.Add(ImageViewer.VERSION_NAME, (General[i] as dynamic).func);
                    }
                    else
                    {
                        Items_General.Add((General[i] as dynamic).name);
                        if (!((General[i] as dynamic).name as string).Equals("-"))
                            FuncByName.Add((General[i] as dynamic).name, (General[i] as dynamic).func);
                    }
                }
            }

            Items_Animation = new List<string>(Items_General);
            List<string> items = new List<string>();
            for (int i = 0; i < Animation.Count; i++)
            {
                if (Animation[i] is string)
                {
                    // Submenu
                    items.Add(Animation[i] + ":");
                    FuncByName.Add(Animation[i] as string, Animation[i] as string);
                }
                else if (Animation[i] is List<object>)
                {
                    // Submenu Item
                    for (int s = 0; s < (Animation[i] as List<object>).Count; s++)
                    {
                        items.Add(":" + ((Animation[i] as List<object>)[s] as dynamic).name);
                        if (!(((Animation[i] as List<object>)[s] as dynamic).name as string).Equals("-"))
                            FuncByName.Add(((Animation[i] as List<object>)[s] as dynamic).name, ((Animation[i] as List<object>)[s] as dynamic).func);
                    }
                }
                else
                {
                    // Item
                    items.Add((Animation[i] as dynamic).name);
                    if (!((Animation[i] as dynamic).name as string).Equals("-"))
                        FuncByName.Add((Animation[i] as dynamic).name, (Animation[i] as dynamic).func);
                }
            }
            Items_Animation.InsertRange(AnimationInsertAtIndex, items);
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

                if (name.IndexOf(":") == name.Length - 1)
                {
                    // non-clickable item?
                    name = name.Substring(0, name.Length - 1);
                    itemClickable = false;
                }

                if (items[i].IndexOf(":") == 0)
                {
                    // sub item
                    ToolStripDropDownItem dropDownItem = Items[Items.Count - 1] as ToolStripDropDownItem;
                    name = items[i].Substring(1);
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

            ((ToolStripMenuItem)Items[ImageViewer.VERSION_NAME]).BackColor = System.Drawing.Color.CornflowerBlue;

            RefreshItems();
        }

        public void RefreshItems()
        {
            ToolStripMenuItem item;

            item = GetItemByFunc(FLIP);
            if (item != null) item.Checked = ImageViewer.FlippedX;

            item = GetItemByFunc(FIT_TO_HEIGHT);
            if (item != null) item.Checked = ImageViewer.FitToMonitorHeight;

            item = GetItemByFunc(TOGGLE_SMOOTHING);
            if (item != null) item.Checked = ImageViewer.Smoothing();

            item = GetItemByFunc(TOGGLE_BACKGROUND);
            if (item != null) item.Checked = ImageViewer.BackgroundsForImagesWithTransparency;

            item = GetItemByFunc(ALWAYS_ON_TOP);
            if (item != null) item.Checked = ImageViewer.AlwaysOnTop;


            item = GetItemByFunc(SORT_NAME);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Name;

            item = GetItemByFunc(SORT_DATE_MODIFIED);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateModified;

            item = GetItemByFunc(SORT_DATE_CREATED);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.DateCreated;

            item = GetItemByFunc(SORT_SIZE);
            if (item != null) item.Checked = ImageViewer.SortImagesBy == SortBy.Size;

            item = GetItemByFunc(SORT_ASCENDING);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Ascending;

            item = GetItemByFunc(SORT_DESCENDING);
            if (item != null) item.Checked = ImageViewer.SortImagesByDir == SortDirection.Descending;
        }

        private void ContexMenuItemClicked(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;

            if (!(item as ToolStripDropDownItem).HasDropDownItems)
                Close();

            switch (FuncByName[item.Name])
            {
                case CLOSE: ImageViewer.CloseNextTick = true; break;

                case NEXT_IMAGE: ImageViewer.NextImage(); break;
                case PREV_IMAGE: ImageViewer.PrevImage(); break;

                case SORT_NAME: ImageViewer.ChangeSortBy(SortBy.Name); break;
                case SORT_DATE_MODIFIED: ImageViewer.ChangeSortBy(SortBy.DateModified); break;
                case SORT_DATE_CREATED: ImageViewer.ChangeSortBy(SortBy.DateCreated); break;
                case SORT_SIZE: ImageViewer.ChangeSortBy(SortBy.Size); break;
                case SORT_ASCENDING: ImageViewer.ChangeSortByDirection(SortDirection.Ascending); break;
                case SORT_DESCENDING: ImageViewer.ChangeSortByDirection(SortDirection.Descending); break;

                case NEXT_FRAME: ImageViewer.NextFrame(); break;
                case PREV_FRAME: ImageViewer.PrevFrame(); break;
                case TOGGLE_ANIMATION: ImageViewer.ToggleAnimation(); break;

                case ROTATE_CLOCKWISE: ImageViewer.RotateImage((int)ImageViewer.Image.Rotation + 90); break;
                case ROTATE_ANTICLOCKWISE: ImageViewer.RotateImage((int)ImageViewer.Image.Rotation - 90); break;
                case FLIP: ImageViewer.FlipImage(); break;
                case FIT_TO_HEIGHT: ImageViewer.ToggleFitToMonitorHeight(); break;
                case RESET_IMAGE: ImageViewer.ResetImage(); break;
                case TOGGLE_SMOOTHING: ImageViewer.ToggleSmoothing(); break;
                case TOGGLE_BACKGROUND: ImageViewer.ToggleBackground(); break;
                case ALWAYS_ON_TOP: ImageViewer.ToggleAlwaysOnTop(); break;

                case OPEN_FILE_LOCATION: ImageViewer.OpenFileAtLocation(); break;
                case DELETE: ImageViewer.DeleteFile(); break;

                case OPEN_SETTINGS: ImageViewer.OpenConfig(); break;
                case RELOAD_SETTINGS: ImageViewer.ReloadConfig(); break;

                case VERSION_NAME: Process.Start("http://torrunt.net/vimage"); break;
            }
        }

        /// <summary>returns the ToolStripMenuItem based on the name of the function.</summary>
        public ToolStripMenuItem GetItemByFunc(string func)
        {
            ToolStripDropDownItem lastSubMenu = null;

            // ContextMenu
            for (int i = 0; i < Items.Count; i++)
            {
                for (int c = 0; c < ImageViewer.Config.ContextMenu.Count; c++)
                {
                    if (ImageViewer.Config.ContextMenu[c] is string)
                    {
                        // Submenu
                        lastSubMenu = Items[(ImageViewer.Config.ContextMenu[c] as string)] as ToolStripDropDownItem;
                    }
                    else if (ImageViewer.Config.ContextMenu[c] is List<object>)
                    {
                        // Submenu items
                        for (int s = 0; s < (ImageViewer.Config.ContextMenu[c] as List<object>).Count; s++)
                        {
                            if (((ImageViewer.Config.ContextMenu[c] as List<object>)[s] as dynamic).func == func)
                                return lastSubMenu.DropDownItems[((ImageViewer.Config.ContextMenu[c] as List<object>)[s] as dynamic).name] as ToolStripMenuItem;
                        }
                    }
                    else if ((ImageViewer.Config.ContextMenu[c] as dynamic).func == func)
                        return Items[(ImageViewer.Config.ContextMenu[c] as dynamic).name] as ToolStripMenuItem;
                }
            }

            // ContextMenu_Animations
            for (int i = 0; i < Items.Count; i++)
            {
                for (int c = 0; c < ImageViewer.Config.ContextMenu_Animation.Count; c++)
                {
                    if (ImageViewer.Config.ContextMenu_Animation[c] is string)
                    {
                        // Submenu
                        lastSubMenu = Items[(ImageViewer.Config.ContextMenu_Animation[c] as dynamic).name] as ToolStripDropDownItem;
                    }
                    else if (ImageViewer.Config.ContextMenu_Animation[c] is List<object>)
                    {
                        // Submenu items
                        for (int s = 0; s < (ImageViewer.Config.ContextMenu_Animation[c] as List<object>).Count; s++)
                        {
                            if (((ImageViewer.Config.ContextMenu_Animation[c] as List<object>)[s] as dynamic).func == func)
                                return lastSubMenu.DropDownItems[((ImageViewer.Config.ContextMenu_Animation[c] as List<object>)[s] as dynamic).name] as ToolStripMenuItem;
                        }
                    }
                    else if ((ImageViewer.Config.ContextMenu_Animation[c] as dynamic).func == func)
                        return Items[(ImageViewer.Config.ContextMenu_Animation[c] as dynamic).name] as ToolStripMenuItem;
                }
            }

            return null;
        }

    }
}
