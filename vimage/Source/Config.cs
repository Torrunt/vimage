using System;
using System.IO;
using System.Collections.Generic;
using SFML.Window;

namespace vimage
{
    public enum SortBy { FolderDefault, Name, Date, DateModified, DateCreated, Size }
    public enum SortDirection { FolderDefault, Ascending, Descending }

    public class Config
    {
        public List<int> Control_Drag = new List<int>();
        public List<int> Control_Close = new List<int>();
        public List<int> Control_OpenContextMenu = new List<int>();
        public List<int> Control_PrevImage = new List<int>();
        public List<int> Control_NextImage = new List<int>();
        public List<int> Control_RotateClockwise = new List<int>();
        public List<int> Control_RotateAntiClockwise = new List<int>();
        public List<int> Control_Flip = new List<int>();
        public List<int> Control_FitToMonitorHeight = new List<int>();
        public List<int> Control_FitToMonitorWidth = new List<int>();
        public List<int> Control_FitToMonitorAuto = new List<int>();
        public List<int> Control_FitToMonitorAlt = new List<int>();
        public List<int> Control_ZoomIn = new List<int>();
        public List<int> Control_ZoomOut = new List<int>();
        public List<int> Control_ZoomFaster = new List<int>();
        public List<int> Control_ZoomAlt = new List<int>();
        public List<int> Control_DragLimitToMonitorBounds = new List<int>();
        public List<int> Control_ToggleSmoothing = new List<int>();
        public List<int> Control_ToggleMipmapping = new List<int>();
        public List<int> Control_ToggleBackground = new List<int>();
        public List<int> Control_ToggleLock = new List<int>();
        public List<int> Control_ToggleAlwaysOnTop = new List<int>();
        public List<int> Control_ToggleTitleBar = new List<int>();
        public List<int> Control_PauseAnimation = new List<int>();
        public List<int> Control_PrevFrame = new List<int>();
        public List<int> Control_NextFrame = new List<int>();
        public List<int> Control_OpenSettings = new List<int>();
        public List<int> Control_ResetImage = new List<int>();
        public List<int> Control_OpenAtLocation = new List<int>();
        public List<int> Control_Delete = new List<int>();
        public List<int> Control_Copy = new List<int>();
        public List<int> Control_CopyAsImage = new List<int>();
        public List<int> Control_OpenDuplicateImage = new List<int>();
        public List<int> Control_OpenFullDuplicateImage = new List<int>();
        public List<int> Control_RandomImage = new List<int>();
        public List<int> Control_MoveLeft = new List<int>();
        public List<int> Control_MoveRight = new List<int>();
        public List<int> Control_MoveUp = new List<int>();
        public List<int> Control_MoveDown = new List<int>();
        public List<int> Control_TransparencyToggle = new List<int>();
        public List<int> Control_TransparencyInc = new List<int>();
        public List<int> Control_TransparencyDec = new List<int>();
        public List<int> Control_Crop = new List<int>();
        public List<int> Control_UndoCrop = new List<int>();
        public List<int> Control_ExitAll = new List<int>();

        public List<List<int>> Controls;
        public List<string> ControlNames = new List<string>()
        {
            "Drag", "Close", "Open Context Menu", "Prev Image", "Next Image", "Rotate Clockwise", "Rotate Anti-Clockwise", "Flip",
            "Fit To Monitor Auto", "Fit To Monitor Width", "Fit To Monitor Height", "Fit To Monitor Alt", "Zoom In", "Zoom Out", "Zoom Faster", "Zoom Alt", "Drag Limit to Monitor Bounds",
            "Toggle Smoothing", "Toggle Mipmapping", "Toggle Background For Transparency", "Toggle Lock", "Toggle Always On Top", "Toggle Title Bar", "Pause Animation", "Prev Frame", "Next Frame",
            "Open Settings", "Reset Image", "Open At Location", "Delete", "Copy", "Copy as Image", "Open Duplicate Image", "Open Full Duplicate Image",
            "Random Image", "Move Left", "Move Right", "Move Up", "Move Down", "Transparency Toggle", "Transparency Increase", "Transparency Decrease", "Crop", "Undo Crop", "Exit All Instances"
        };

        public bool Setting_OpenAtMousePosition
        {
            get { return (Boolean)Settings["OPENATMOUSEPOSITION"]; }
            set { Settings["OPENATMOUSEPOSITION"] = value; }
        }
        public bool Setting_SmoothingDefault
        {
            get { return (Boolean)Settings["SMOOTHINGDEFAULT"]; }
            set { Settings["SMOOTHINGDEFAULT"] = value; }
        }
        public bool Setting_Mipmapping
        {
            get { return (Boolean)Settings["MIPMAPPING"]; }
            set { Settings["MIPMAPPING"] = value; }
        }
        public bool Setting_BackgroundForImagesWithTransparencyDefault
        {
            get { return (Boolean)Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"]; }
            set { Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"] = value; }
        }
        public string Setting_BackgroundColour
        {
            get { return (string)Settings["BACKGROUNDCOLOUR"]; }
            set { Settings["BACKGROUNDCOLOUR"] = value; }
        }
        public string Setting_TransparencyToggleValue
        {
            get { return (string)Settings["TRANSPARENCYTOGGLEVALUE"]; }
            set { Settings["TRANSPARENCYTOGGLEVALUE"] = value; }
        }
        public int Setting_LimitImagesToMonitor
        {
            get { return (int)Settings["LIMITIMAGESTOMONITOR"]; }
            set { Settings["LIMITIMAGESTOMONITOR"] = value; }
        }
        public const int NONE   = 0;
        public const int HEIGHT = 1;
        public const int WIDTH  = 2;
        public const int AUTO   = 3;

        public bool Setting_PositionLargeWideImagesInCorner
        {
            get { return (Boolean)Settings["POSITIONLARGEWIDEIMAGESINCORNER"]; }
            set { Settings["POSITIONLARGEWIDEIMAGESINCORNER"] = value; }
        }
        public bool Setting_LoopImageNavigation
        {
            get { return (Boolean)Settings["LOOPIMAGENAVIGATION"]; }
            set { Settings["LOOPIMAGENAVIGATION"] = value; }
        }
        public bool Setting_PreloadNextImage
        {
            get { return (Boolean)Settings["PRELOADNEXTIMAGE"]; }
            set { Settings["PRELOADNEXTIMAGE"] = value; }
        }
        public bool Setting_ShowTitleBar
        {
            get { return (Boolean)Settings["SHOWTITLEBAR"]; }
            set { Settings["SHOWTITLEBAR"] = value; }
        }
        public bool Setting_OpenSettingsEXE
        {
            get { return (Boolean)Settings["OPENSETTINGSEXE"]; }
            set { Settings["OPENSETTINGSEXE"] = value; }
        }
        public bool Setting_ListenForConfigChanges
        {
            get { return (Boolean)Settings["LISTENFORCONFIGCHANGES"]; }
            set { Settings["LISTENFORCONFIGCHANGES"] = value; }
        }
        public int Setting_MinImageSize
        {
            get { return (int)Settings["MINIMAGESIZE"]; }
            set { Settings["MINIMAGESIZE"] = value; }
        }
        public int Setting_SmoothingMinImageSize
        {
            get { return (int)Settings["SMOOTHINGMINIMAGESIZE"]; }
            set { Settings["SMOOTHINGMINIMAGESIZE"] = value; }
        }
        public int Setting_ZoomSpeed
        {
            get { return (int)Settings["ZOOMSPEED"]; }
            set { Settings["ZOOMSPEED"] = value; }
        }
        public int Setting_ZoomSpeedFast
        {
            get { return (int)Settings["ZOOMSPEEDFAST"]; }
            set { Settings["ZOOMSPEEDFAST"] = value; }
        }
        public int Setting_MoveSpeed
        {
            get { return (int)Settings["MOVESPEED"]; }
            set { Settings["MOVESPEED"] = value; }
        }
        public int Setting_MoveSpeedFast
        {
            get { return (int)Settings["MOVESPEEDFAST"]; }
            set { Settings["MOVESPEEDFAST"] = value; }
        }
        public int Setting_MaxTextures
        {
            get { return (int)Settings["MAXTEXTURES"]; }
            set { Settings["MAXTEXTURES"] = value; }
        }
        public int Setting_MaxAnimations
        {
            get { return (int)Settings["MAXANIMATIONS"]; }
            set { Settings["MAXANIMATIONS"] = value; }
        }

        public int Setting_SettingsAppWidth
        {
            get { return (int)Settings["SETTINGSAPPWIDTH"]; }
            set { Settings["SETTINGSAPPWIDTH"] = value; }
        }
        public int Setting_SettingsAppHeight
        {
            get { return (int)Settings["SETTINGSAPPHEIGHT"]; }
            set { Settings["SETTINGSAPPHEIGHT"] = value; }
        }

        public SortBy Setting_DefaultSortBy
        {
            get { return (SortBy)Settings["DEFAULTSORTBY"]; }
            set { Settings["DEFAULTSORTBY"] = value; }
        }
        public SortDirection Setting_DefaultSortDir
        {
            get { return (SortDirection)Settings["DEFAULTSORTDIR"]; }
            set { Settings["DEFAULTSORTDIR"] = value; }
        }

        public string Setting_CropToolFillColour
        {
            get { return (string)Settings["CROPTOOLFILLCOLOUR"]; }
            set { Settings["CROPTOOLFILLCOLOUR"] = value; }
        }
        public string Setting_CropToolOutlineColour
        {
            get { return (string)Settings["CROPTOOLOUTLINECOLOUR"]; }
            set { Settings["CROPTOOLOUTLINECOLOUR"] = value; }
        }
        public int Setting_CropToolOutlineThickness
        {
            get { return (int)Settings["CROPTOOLOUTLINETHICKNESS"]; }
            set { Settings["CROPTOOLOUTLINETHICKNESS"] = value; }
        }

        public List<object> ContextMenu = new List<object>();
        public List<object> ContextMenu_Animation = new List<object>();
        public int ContextMenu_Animation_InsertAtIndex
        {
            get { return (int)Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"]; }
            set { Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"] = value; }
        }
        public bool ContextMenuShowMargin
        {
            get { return (Boolean)Settings["CONTEXTMENU_SHOWMARGIN"]; }
            set { Settings["CONTEXTMENU_SHOWMARGIN"] = value; }
        }
        public bool ContextMenuShowMarginSub
        {
            get { return (Boolean)Settings["CONTEXTMENU_SHOWMARGINSUB"]; }
            set { Settings["CONTEXTMENU_SHOWMARGINSUB"] = value; }
        }

        public List<object> CustomActions = new List<object>();
        public List<object> CustomActionBindings = new List<object>();

        private Dictionary<string, object> Settings;

        public const int MouseCodeOffset = 150;
        public const int MOUSE_SCROLL_UP = 155;
        public const int MOUSE_SCROLL_DOWN = 156;

        public Config()
        {
            Controls = new List<List<int>>()
            {
                Control_Drag, Control_Close, Control_OpenContextMenu, Control_PrevImage, Control_NextImage, Control_RotateClockwise,
                Control_RotateAntiClockwise, Control_Flip, Control_FitToMonitorHeight, Control_FitToMonitorWidth, Control_FitToMonitorAuto,
                Control_FitToMonitorAlt, Control_ZoomIn, Control_ZoomOut, Control_ZoomFaster, Control_ZoomAlt, Control_DragLimitToMonitorBounds, Control_ToggleSmoothing, Control_ToggleMipmapping,
                Control_ToggleBackground, Control_ToggleLock, Control_ToggleAlwaysOnTop, Control_ToggleTitleBar, Control_PauseAnimation, Control_PrevFrame, Control_NextFrame, Control_OpenSettings,
                Control_ResetImage, Control_OpenAtLocation, Control_Delete, Control_Copy, Control_CopyAsImage, Control_OpenDuplicateImage, Control_OpenFullDuplicateImage,
                Control_RandomImage, Control_MoveLeft, Control_MoveRight, Control_MoveUp, Control_MoveDown,
                Control_TransparencyToggle, Control_TransparencyInc, Control_TransparencyDec, Control_Crop, Control_UndoCrop, Control_ExitAll
            };

            Init();
        }
        public void Init()
        {
            SetDefaultControls();

            SetDefaultContextMenu();
            SetDefaultCustomActions();

            Settings = new Dictionary<string, object>()
            {
                { "OPENATMOUSEPOSITION", true },
                { "SMOOTHINGDEFAULT", true },
                { "MIPMAPPING", true },
                { "BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT", false },
                { "BACKGROUNDCOLOUR", "#FFE6E6E6" },
                { "TRANSPARENCYTOGGLEVALUE", "#64FFFFFF" },
                { "LIMITIMAGESTOMONITOR", AUTO },
                { "POSITIONLARGEWIDEIMAGESINCORNER", true },
                { "LOOPIMAGENAVIGATION", true },
                { "PRELOADNEXTIMAGE", true },
                { "SHOWTITLEBAR", false },
                { "OPENSETTINGSEXE", true },
                { "LISTENFORCONFIGCHANGES", true },
                { "MINIMAGESIZE", 64 },
                { "SMOOTHINGMINIMAGESIZE", 65 },
                { "ZOOMSPEED", 2 },
                { "ZOOMSPEEDFAST", 10 },
                { "MOVESPEED", 2 },
                { "MOVESPEEDFAST", 10 },
                { "MAXTEXTURES", 80 },
                { "MAXANIMATIONS", 8 },
                { "SETTINGSAPPWIDTH", 600 },
                { "SETTINGSAPPHEIGHT", 550 },
                { "DEFAULTSORTBY", SortBy.FolderDefault },
                { "DEFAULTSORTDIR", SortDirection.FolderDefault },
                
                { "CROPTOOLFILLCOLOUR", "#78FFFFFF" },
                { "CROPTOOLOUTLINECOLOUR", "#FF000000" },
                { "CROPTOOLOUTLINETHICKNESS", 2 },

                { "DRAG", Control_Drag },
                { "CLOSE", Control_Close },
                { "OPENCONTEXTMENU", Control_OpenContextMenu },
                { "PREVIMAGE", Control_PrevImage },
                { "NEXTIMAGE", Control_NextImage },
                { "ROTATECLOCKWISE", Control_RotateClockwise },
                { "ROTATEANTICLOCKWISE", Control_RotateAntiClockwise },
                { "FLIP", Control_Flip },
                { "FITTOMONITORHEIGHT", Control_FitToMonitorHeight },
                { "FITTOMONITORWIDTH", Control_FitToMonitorWidth },
                { "FITTOMONITORAUTO", Control_FitToMonitorAuto },
                { "FITTOMONITORALT", Control_FitToMonitorAlt },
                { "ZOOMIN", Control_ZoomIn },
                { "ZOOMOUT", Control_ZoomOut },
                { "ZOOMFASTER", Control_ZoomFaster },
                { "ZOOMALT", Control_ZoomAlt },
                { "DRAGLIMITTOMONITORBOUNDS", Control_DragLimitToMonitorBounds },
                { "TOGGLESMOOTHING", Control_ToggleSmoothing },
                { "TOGGLEMIPMAPPING", Control_ToggleMipmapping },
                { "TOGGLEBACKGROUNDFORTRANSPARENCY", Control_ToggleBackground },
                { "TOGGLELOCK", Control_ToggleLock },
                { "TOGGLEALWAYSONTOP", Control_ToggleAlwaysOnTop },
                { "TOGGLETITLEBAR", Control_ToggleTitleBar },
                { "PAUSEANIMATION", Control_PauseAnimation },
                { "PREVFRAME", Control_PrevFrame },
                { "NEXTFRAME", Control_NextFrame },
                { "OPENSETTINGS", Control_OpenSettings },
				{ "RESETIMAGE", Control_ResetImage },
                { "OPENATLOCATION", Control_OpenAtLocation },
                { "DELETE", Control_Delete },
                { "COPY", Control_Copy },
                { "COPYASIMAGE", Control_CopyAsImage },
                { "OPENDUPLICATEIMAGE", Control_OpenDuplicateImage },
                { "OPENFULLDUPLICATEIMAGE", Control_OpenFullDuplicateImage },
                { "RANDOMIMAGE", Control_RandomImage },
                { "MOVELEFT", Control_MoveLeft },
                { "MOVERIGHT", Control_MoveRight },
                { "MOVEUP", Control_MoveUp },
                { "MOVEDOWN", Control_MoveDown },
                { "TRANSPARENCYTOGGLE", Control_TransparencyToggle },
                { "TRANSPARENCYINC", Control_TransparencyInc },
                { "TRANSPARENCYDEC", Control_TransparencyDec },
                { "CROP", Control_Crop },
                { "UNDOCROP", Control_UndoCrop },
                { "EXITALL", Control_ExitAll },

                { "CONTEXTMENU", ContextMenu },
                { "CONTEXTMENU_ANIMATION", ContextMenu_Animation },
                { "CONTEXTMENU_ANIMATION_INSERTATINDEX", 2 },
                { "CONTEXTMENU_SHOWMARGIN", false },
                { "CONTEXTMENU_SHOWMARGINSUB", true },

                { "CUSTOMACTIONS", CustomActions },
                { "CUSTOMACTIONBINDINGS", CustomActionBindings }
            };
        }

        public void SetDefaultControls()
        {
            Control_Drag.Clear();
            Control_Close.Clear();
            Control_OpenContextMenu.Clear();
            Control_PrevImage.Clear();
            Control_NextImage.Clear();
            Control_RotateClockwise.Clear();
            Control_RotateAntiClockwise.Clear();
            Control_Flip.Clear();
            Control_FitToMonitorHeight.Clear();
            Control_FitToMonitorWidth.Clear();
            Control_FitToMonitorAuto.Clear();
            Control_FitToMonitorAlt.Clear();
            Control_ZoomIn.Clear();
            Control_ZoomOut.Clear();
            Control_ZoomFaster.Clear();
            Control_ZoomAlt.Clear();
            Control_DragLimitToMonitorBounds.Clear();
            Control_ToggleSmoothing.Clear();
            Control_ToggleMipmapping.Clear();
            Control_ToggleBackground.Clear();
            Control_ToggleLock.Clear();
            Control_ToggleAlwaysOnTop.Clear();
            Control_ToggleTitleBar.Clear();
            Control_PauseAnimation.Clear();
            Control_PrevFrame.Clear();
            Control_NextFrame.Clear();
            Control_OpenSettings.Clear();
            Control_ResetImage.Clear();
            Control_OpenAtLocation.Clear();
            Control_Delete.Clear();
            Control_Copy.Clear();
            Control_CopyAsImage.Clear();
            Control_OpenDuplicateImage.Clear();
            Control_OpenFullDuplicateImage.Clear();
            Control_RandomImage.Clear();
            Control_MoveLeft.Clear();
            Control_MoveRight.Clear();
            Control_MoveUp.Clear();
            Control_MoveDown.Clear();
            Control_TransparencyToggle.Clear();
            Control_TransparencyInc.Clear();
            Control_TransparencyDec.Clear();
            Control_Crop.Clear();
            Control_UndoCrop.Clear();
            Control_ExitAll.Clear();

            SetControls(Control_Drag, "MOUSELEFT");
            SetControls(Control_Close, "ESC", "BACKSPACE");
            SetControls(Control_OpenContextMenu, "MOUSERIGHT");
            SetControls(Control_PrevImage, "LEFT", "PAGE UP", "MOUSE4");
            SetControls(Control_NextImage, "RIGHT", "PAGE DOWN", "MOUSE5");
            SetControls(Control_RotateClockwise, "UP");
            SetControls(Control_RotateAntiClockwise, "DOWN");
            SetControls(Control_Flip, "F");
            SetControls(Control_FitToMonitorHeight, "CTRL+MOUSEMIDDLE");
            SetControls(Control_FitToMonitorWidth, "SHIFT+MOUSEMIDDLE");
            SetControls(Control_FitToMonitorAuto, "MOUSEMIDDLE");
            SetControls(Control_FitToMonitorAlt, "RSHIFT", "LSHIFT");
            SetControls(Control_ZoomIn, "SCROLLUP");
            SetControls(Control_ZoomOut, "SCROLLDOWN");
            SetControls(Control_ZoomFaster, "RSHIFT", "LSHIFT");
            SetControls(Control_ZoomAlt, "RCTRL", "CTRL");
            SetControls(Control_DragLimitToMonitorBounds, "ALT");
            SetControls(Control_ToggleSmoothing, "S");
            SetControls(Control_ToggleMipmapping, "SHIFT+S");
            SetControls(Control_ToggleBackground, "B");
            SetControls(Control_ToggleLock, "");
            SetControls(Control_ToggleAlwaysOnTop, "L");
            SetControls(Control_ToggleTitleBar, "");
            SetControls(Control_PauseAnimation, "SPACE");
            SetControls(Control_PrevFrame, "<");
            SetControls(Control_NextFrame, ">");
            SetControls(Control_OpenSettings, "");
            SetControls(Control_ResetImage, "R");
            SetControls(Control_OpenAtLocation, "O");
            SetControls(Control_Delete, "DELETE");
            SetControls(Control_Copy, "CTRL+C");
            SetControls(Control_CopyAsImage, "ALT+C");
            SetControls(Control_OpenDuplicateImage, "C");
            SetControls(Control_OpenFullDuplicateImage, "SHIFT+C");
            SetControls(Control_RandomImage, "M");
            SetControls(Control_MoveLeft, "CTRL+LEFT", "RCTRL+LEFT");
            SetControls(Control_MoveRight, "CTRL+RIGHT", "RCTRL+RIGHT");
            SetControls(Control_MoveUp, "CTRL+UP", "RCTRL+UP");
            SetControls(Control_MoveDown, "CTRL+DOWN", "RCTRL+DOWN");
            SetControls(Control_TransparencyToggle, "T");
            SetControls(Control_TransparencyInc, "T+SCROLLDOWN");
            SetControls(Control_TransparencyDec, "T+SCROLLUP");
            SetControls(Control_Crop, "X");
            SetControls(Control_UndoCrop, "CTRL+Z");
            SetControls(Control_ExitAll, "SHIFT+ESC");
        }
        public void SetDefaultContextMenu()
        {
            ContextMenu.Clear();
            ContextMenu_Animation.Clear();

            ContextMenu.Add(new { name = "Close", func = Action.Close });
            ContextMenu.Add(new { name = "-", func = Action.None });
            ContextMenu.Add(new { name = "Next", func = Action.NextImage });
            ContextMenu.Add(new { name = "Previous", func = Action.PrevImage });
            ContextMenu.Add("Sort by");
            List<object> SubMenu_SortBy = new List<object>();
            SubMenu_SortBy.Add(new { name = "Name", func = Action.SortName });
            SubMenu_SortBy.Add(new { name = "Date", func = Action.SortDate });
            SubMenu_SortBy.Add(new { name = "Date modified", func = Action.SortDateModified });
            SubMenu_SortBy.Add(new { name = "Date created", func = Action.SortDateCreated });
            SubMenu_SortBy.Add(new { name = "Size", func = Action.SortSize });
            SubMenu_SortBy.Add(new { name = "-", func = Action.None });
            SubMenu_SortBy.Add(new { name = "Ascending", func = Action.SortAscending });
            SubMenu_SortBy.Add(new { name = "Descending", func = Action.SortDescending });
            ContextMenu.Add(SubMenu_SortBy);
            ContextMenu.Add(new { name = "-", func = Action.None });
            ContextMenu.Add(new { name = "Rotate right", func = Action.RotateClockwise });
            ContextMenu.Add(new { name = "Rotate left", func = Action.RotateAntiClockwise });
            ContextMenu.Add(new { name = "Flip", func = Action.Flip });
            ContextMenu.Add(new { name = "Fit to height", func = Action.FitToMonitorHeight });
            ContextMenu.Add(new { name = "Fit to width", func = Action.FitToMonitorWidth });
            ContextMenu.Add(new { name = "Smoothing", func = Action.ToggleSmoothing });
            ContextMenu.Add(new { name = "Mipmapping", func = Action.ToggleMipmapping });
            ContextMenu.Add(new { name = "Background", func = Action.ToggleBackground });
            ContextMenu.Add(new { name = "Always on top", func = Action.ToggleAlwaysOnTop });
            ContextMenu.Add(new { name = "Reset image", func = Action.ResetImage });
            ContextMenu.Add(new { name = "-", func = Action.None });
            ContextMenu.Add(new { name = "Open file location", func = Action.OpenAtLocation });
            ContextMenu.Add(new { name = "Copy", func = Action.Copy });
            ContextMenu.Add(new { name = "Delete", func = Action.Delete });
            ContextMenu.Add(new { name = "-", func = Action.None });
            ContextMenu.Add(new { name = "Settings", func = Action.OpenSettings });
            ContextMenu.Add(new { name = "vimage [version]", func = Action.VisitWebsite });

            ContextMenu_Animation.Add(new { name = "Pause/Play", func = Action.TransparencyToggle });
            ContextMenu_Animation.Add(new { name = "-", func = Action.None });
        }
        public void SetDefaultCustomActions()
        {
            CustomActions.Clear();
            CustomActionBindings.Clear();

            CustomActions.Add(new { name = "EDIT PAINT", func = @"%windir%\system32\mspaint.exe %f" });
            CustomActionBindings.Add(new { name = "EDIT PAINT", bindings = new List<int>() });
            CustomActions.Add(new { name = "EDIT PAINTDOTNET", func = "\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\" %f" });
            CustomActionBindings.Add(new { name = "EDIT PAINTDOTNET", bindings = new List<int>() });
            CustomActions.Add(new { name = "TOGGLE TASKBAR", func = "-taskbarToggle" });
            CustomActionBindings.Add(new { name = "TOGGLE TASKBAR", bindings = new List<int>() });
        }

        /// <summary> Loads and parses a config txt file. If it doesn't exist, a default one will be made. </summary>
        public void Load(string configFile)
        {
            // If config file doesn't exist, make one
            if (!File.Exists(configFile))
            {
                Save(configFile);
                return;
            }
            // Clear default controls and context menu before they are loaded back in
            foreach (var list in Settings)
            {
                if (list.Value is List<int>)
                    ((List<int>)list.Value).Clear();
            }
            ContextMenu.Clear();
            ContextMenu_Animation.Clear();
            CustomActions.Clear();
            CustomActionBindings.Clear();


            StreamReader reader = File.OpenText(configFile);
            string line = reader.ReadLine();

            while (line != null)
            {
                // if line is empty of has no '=' symbol, go to next
                if (line.Equals("") || line.IndexOf('=') == -1)
                {
                    line = reader.ReadLine();
                    continue;
                }

                // trim
                line = RemoveSpaces(line);

                // ignore comments
                if (line.IndexOf("//") != -1)
                    line = line.Substring(0, line.IndexOf("//"));

                // split variable name and values
                string[] nameValue = line.Split('=');

                // invalid name?
                string name = nameValue[0].ToUpper();
                if (!Settings.ContainsKey(name))
                {
                    line = reader.ReadLine();
                    continue;
                }

                // nothing after '='?, check next line
                if (nameValue[1].Equals(""))
                {
                    line = RemoveSpaces(reader.ReadLine());

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.IndexOf('=') != -1)
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    // read section
                    line = ReadSection(reader, Settings[name] as List<object>, name);
                    continue;
                }


                // split values by commas
                string[] values = nameValue[1].Split(',');

                // Assign Values
                if (Settings[name] is List<int>)
                {
                    // Control
                    StringToControls(values, (List<int>)Settings[name]);
                }
                else if (Settings[name] is int || Settings[name] is Enum)
                {
                    // Integer
                    int i;
                    if (int.TryParse(values[0], out i))
                        Settings[name] = i;
                }
                else if (Settings[name] is Boolean)
                {
                    // Boolean
                    if (values[0].Equals("1") || values[0].ToUpper().Equals("T") || values[0].ToUpper().Equals("TRUE"))
                        Settings[name] = true;
                    else
                        Settings[name] = false;
                }
                else if (Settings[name] is String)
                {
                    Settings[name] = values[0];
                }

                // next line
                line = reader.ReadLine();
            }

            reader.Close();
        }
        private string ReadSection(StreamReader reader, List<object> setting, string sectionName = "")
        {
            string line = reader.ReadLine();
            string trimedLine = RemoveSpaces(line);
            string[] splitValues;

            if (trimedLine.Equals("}"))
                return line;

            while (line != null)
            {
                if (!trimedLine.Equals("-") && trimedLine.IndexOf(':') == -1)
                {
                    // Subsection
                    string subSectionName = line.Replace("\t", "");

                    line = RemoveSpaces(reader.ReadLine());

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.IndexOf('=') != -1)
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    setting.Add(subSectionName);
                    List<object> subSetting = new List<object>();
                    setting.Add(subSetting);

                    line = ReadSection(reader, subSetting, sectionName);
                    trimedLine = line;
                }
                else
                {
                    // Item
                    if (trimedLine.Equals("-"))
                        splitValues = new[] { "-", "-" }; // line break
                    else
                        splitValues = line.Split(new char[] { ':' }, 2);

                    // trim tabs from name, spaces from map name
                    splitValues[0] = splitValues[0].Replace("\t", "").Trim();
                    splitValues[1] = splitValues[1].Trim();

                    // assign Values
                    if (sectionName == "CUSTOMACTIONBINDINGS")
                        setting.Add(new { name = splitValues[0], bindings = StringToControls(splitValues[1].Split(',')) });
                    else if (sectionName.IndexOf("CONTEXTMENU") != -1)
                    {
                        Action action = Actions.StringToAction(splitValues[1]);
                        setting.Add(new { name = splitValues[0], func = action <= 0 ? (object)splitValues[1] : action });
                    }
                    else
                        setting.Add(new { name = splitValues[0], func = splitValues[1] });

                    // next line
                    line = reader.ReadLine();
                    trimedLine = RemoveSpaces(line);
                }

                // break if end brace
                if (trimedLine.Equals("}"))
                {
                    if (reader.Peek() == -1)
                        return line;
                    line = RemoveSpaces(reader.ReadLine());
                    break;
                }
            }

            return line;
        }


        /// <summary> Saves settings to config txt file. </summary>
        public void Save(string configFile)
        {
            // Open
            try
            {
                FileStream fileStream;
                if (File.Exists(configFile))
                {
                    // Clear if file already exists
                    File.WriteAllText(configFile, String.Empty);
                    fileStream = File.Open(configFile, FileMode.Open, FileAccess.Write);
                }
                else
                    fileStream = File.Create(configFile);
                StreamWriter writer = new StreamWriter(fileStream);

                // Write
                writer.Write("// General Settings" + Environment.NewLine);

                WriteSetting(writer, "OpenAtMousePosition", Setting_OpenAtMousePosition);
                WriteSetting(writer, "SmoothingDefault", Setting_SmoothingDefault);
                WriteSetting(writer, "Mipmapping", Setting_Mipmapping);
                WriteSetting(writer, "BackgroundForImagesWithTransparencyDefault", Setting_BackgroundForImagesWithTransparencyDefault);
                WriteSetting(writer, "BackgroundColour", Setting_BackgroundColour);
                WriteSetting(writer, "TransparencyToggleValue", Setting_TransparencyToggleValue);
                WriteSetting(writer, "LimitImagesToMonitor", Setting_LimitImagesToMonitor, "0=NONE, 1=HEIGHT, 2=WIDTH, 3=AUTO");
                WriteSetting(writer, "PositionLargeWideImagesInCorner", Setting_PositionLargeWideImagesInCorner,
                    "ie: Desktop Wallpapers and Screenshots");
                WriteSetting(writer, "LoopImageNavigation", Setting_LoopImageNavigation);
                WriteSetting(writer, "PreloadNextImage", Setting_PreloadNextImage,
                    "when using the next/prev image buttons, the image after the one just loaded will be loaded as well");
                WriteSetting(writer, "ShowTitleBar", Setting_ShowTitleBar);
                WriteSetting(writer, "OpenSettingsEXE", Setting_OpenSettingsEXE, "if false, will open config.txt instead");
                WriteSetting(writer, "ListenForConfigChanges", Setting_ListenForConfigChanges,
                    "vimage will reload settings automatically when they are changed.");
                WriteSetting(writer, "MinImageSize", Setting_MinImageSize,
                    "if an image is smaller than this (in width or height) it will scaled up to it automatically");
                WriteSetting(writer, "SmoothingMinImageSize", Setting_SmoothingMinImageSize,
                    "images smaller than this will not have smoothing turned on (if 0, all images with use smoothing)");
                WriteSetting(writer, "ZoomSpeed", Setting_ZoomSpeed);
                WriteSetting(writer, "ZoomSpeedFast", Setting_ZoomSpeedFast);
                WriteSetting(writer, "MoveSpeed", Setting_MoveSpeed);
                WriteSetting(writer, "MoveSpeedFast", Setting_MoveSpeedFast);
                WriteSetting(writer, "MaxTextures", Setting_MaxTextures);
                WriteSetting(writer, "MaxAnimations", Setting_MaxAnimations);
                WriteSetting(writer, "SettingsAppWidth", Setting_SettingsAppWidth);
                WriteSetting(writer, "SettingsAppHeight", Setting_SettingsAppHeight);
                WriteSetting(writer, "DefaultSortBy", (int)Setting_DefaultSortBy);
                WriteSetting(writer, "DefaultSortDir", (int)Setting_DefaultSortDir);

                writer.Write(Environment.NewLine);

                WriteSetting(writer, "CropToolFillColour", Setting_CropToolFillColour);
                WriteSetting(writer, "CropToolOutlineColour", Setting_CropToolOutlineColour);
                WriteSetting(writer, "CropToolOutlineThickness", Setting_CropToolOutlineThickness);

                writer.Write(Environment.NewLine);
                writer.Write("// Bindings" + Environment.NewLine);

                WriteControl(writer, "Drag", Control_Drag);
                WriteControl(writer, "Close", Control_Close);
                WriteControl(writer, "OpenContextMenu", Control_OpenContextMenu);
                WriteControl(writer, "PrevImage", Control_PrevImage);
                WriteControl(writer, "NextImage", Control_NextImage);
                WriteControl(writer, "RotateClockwise", Control_RotateClockwise);
                WriteControl(writer, "RotateAntiClockwise", Control_RotateAntiClockwise);
                WriteControl(writer, "Flip", Control_Flip);
                WriteControl(writer, "FitToMonitorHeight", Control_FitToMonitorHeight);
                WriteControl(writer, "FitToMonitorWidth", Control_FitToMonitorWidth);
                WriteControl(writer, "FitToMonitorAuto", Control_FitToMonitorAuto);
                WriteControl(writer, "FitToMonitorAlt", Control_FitToMonitorAlt);
                WriteControl(writer, "ZoomIn", Control_ZoomIn);
                WriteControl(writer, "ZoomOut", Control_ZoomOut);
                WriteControl(writer, "ZoomFaster", Control_ZoomFaster);
                WriteControl(writer, "ZoomAlt", Control_ZoomAlt);
                WriteControl(writer, "DragLimitToMonitorBounds", Control_DragLimitToMonitorBounds);
                WriteControl(writer, "ToggleSmoothing", Control_ToggleSmoothing);
                WriteControl(writer, "ToggleMipmapping", Control_ToggleMipmapping);
                WriteControl(writer, "ToggleBackgroundForTransparency", Control_ToggleBackground);
                WriteControl(writer, "ToggleLock", Control_ToggleLock);
                WriteControl(writer, "ToggleAlwaysOnTop", Control_ToggleAlwaysOnTop);
                WriteControl(writer, "ToggleTitleBar", Control_ToggleTitleBar);
                WriteControl(writer, "PauseAnimation", Control_PauseAnimation);
                WriteControl(writer, "PrevFrame", Control_PrevFrame);
                WriteControl(writer, "NextFrame", Control_NextFrame);
                WriteControl(writer, "OpenSettings", Control_OpenSettings);
                WriteControl(writer, "ResetImage", Control_ResetImage);
                WriteControl(writer, "OpenAtLocation", Control_OpenAtLocation);
                WriteControl(writer, "Delete", Control_Delete);
                WriteControl(writer, "Copy", Control_Copy);
                WriteControl(writer, "CopyAsImage", Control_CopyAsImage);
                WriteControl(writer, "OpenDuplicateImage", Control_OpenDuplicateImage);
                WriteControl(writer, "OpenFullDuplicateImage", Control_OpenFullDuplicateImage);
                WriteControl(writer, "RandomImage", Control_RandomImage);
                WriteControl(writer, "MoveLeft", Control_MoveLeft);
                WriteControl(writer, "MoveRight", Control_MoveRight);
                WriteControl(writer, "MoveUp", Control_MoveUp);
                WriteControl(writer, "MoveDown", Control_MoveDown);
                WriteControl(writer, "TransparencyToggle", Control_TransparencyToggle);
                WriteControl(writer, "TransparencyInc", Control_TransparencyInc);
                WriteControl(writer, "TransparencyDec", Control_TransparencyDec);
                WriteControl(writer, "Crop", Control_Crop);
                WriteControl(writer, "UndoCrop", Control_UndoCrop);
                WriteControl(writer, "ExitAll", Control_ExitAll);

                writer.Write(Environment.NewLine);
                writer.Write("// Context Menu" + Environment.NewLine);

                WriteContextMenuSetup(writer, "ContextMenu", ContextMenu);
                WriteContextMenuSetup(writer, "ContextMenu_Animation", ContextMenu_Animation);

                WriteSetting(writer, "ContextMenu_Animation_InsertAtIndex", ContextMenu_Animation_InsertAtIndex);
                WriteSetting(writer, "ContextMenu_ShowMargin", ContextMenuShowMargin);
                WriteSetting(writer, "ContextMenu_ShowMarginSub", ContextMenuShowMarginSub);

                writer.Write(Environment.NewLine);
                WriteCustomActions(writer, "CustomActions", CustomActions);
                WriteCustomActionBindings(writer, "CustomActionBindings", CustomActionBindings);

                // Close
                writer.Close();
            }
            catch (UnauthorizedAccessException)
            {
#if SETTINGSAPP
                System.Windows.MessageBox.Show("vimage does not have write permissions for the folder it's located in.\nPlease place it somewhere else (or set it to run as admin).", "vimage - Error");
#else
                System.Windows.Forms.MessageBox.Show("vimage does not have write permissions for the folder it's located in.\nPlease place it somewhere else (or set it to run as admin).", "vimage - Error");
#endif
            }
        }
        private void WriteSetting(StreamWriter writer, string name, bool value, string comment = "")
        {
            writer.Write(name + " = " + (value ? 1 : 0));
            WriteComment(writer, comment);
        }
        private void WriteSetting(StreamWriter writer, string name, int value, string comment = "")
        {
            writer.Write(name + " = " + value.ToString());
            WriteComment(writer, comment);
        }
        private void WriteSetting(StreamWriter writer, string name, string value, string comment = "")
        {
            writer.Write(name + " = " + value);
            WriteComment(writer, comment);
        }
        private void WriteComment(StreamWriter writer, string comment = "")
        {
            if (!comment.Equals(""))
                writer.Write(" // " + comment);
            writer.Write(Environment.NewLine);
        }
        private void WriteControl(StreamWriter writer, string name, List<int> controls)
        {
            writer.Write(name + " = " + ControlsToString(controls) + Environment.NewLine);
        }
        private void WriteContextMenuSetup(StreamWriter writer, string name, List<object> contextMenu)
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            WriteContextMenuItems(writer, contextMenu, 1);
            writer.Write("}" + Environment.NewLine);
        }
        private void WriteContextMenuItems(StreamWriter writer, List<object> items, int depth = 1)
        {
            if (items == null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(VariableAmountOfStrings(depth, "\t"));

                if (items[i] is string)
                {
                    // Submenu
                    writer.Write((items[i] as string) + Environment.NewLine + VariableAmountOfStrings(depth, "\t") + "{" + Environment.NewLine);
                    i++;
                    WriteContextMenuItems(writer, items[i] as List<object>, depth + 1);
                    writer.Write(VariableAmountOfStrings(depth, "\t") + "}" + Environment.NewLine);
                }
                else
                {
                    // Item
                    string itemName = (items[i] as dynamic).name as string;
                    string itemFunc = (string)((items[i] as dynamic).func is Action ? ((Action)(items[i] as dynamic).func).ToNameString() : (items[i] as dynamic).func);
                    if (itemName.Equals("-"))
                        writer.Write("-" + Environment.NewLine);
                    else if (itemName.Equals(""))
                        writer.Write(": " + itemFunc + Environment.NewLine);
                    else
                        writer.Write(itemName + " : " + itemFunc + Environment.NewLine);
                }
            }
        }
        private void WriteCustomActions(StreamWriter writer, string name, List<object> customActions)
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            for (int i = 0; i < CustomActions.Count; i++)
                writer.Write("\t" + (customActions[i] as dynamic).name + " : " + (customActions[i] as dynamic).func + Environment.NewLine);
            writer.Write("}" + Environment.NewLine);
        }
        private void WriteCustomActionBindings(StreamWriter writer, string name, List<object> customActionBindings)
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            for (int i = 0; i < customActionBindings.Count; i++)
                writer.Write("\t" + (customActionBindings[i] as dynamic).name + " : " + ControlsToString((customActionBindings[i] as dynamic).bindings) + Environment.NewLine);
            writer.Write("}" + Environment.NewLine);
        }


        /// <summary> Converts list of controls (Keyboard.Key and Mouse.Button) to their string names seperated by commas. </summary>
        public static string ControlsToString(List<int> controls)
        {
            string str = "";
            bool nextIsKeyCombo = false;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] == -2 && controls.Count > 2)
                {
                    nextIsKeyCombo = true;
                    continue;
                }
                if (nextIsKeyCombo)
                {
                    str += ControlToString(controls[i]) + "+" + ControlToString(controls[i+1]);
                    nextIsKeyCombo = false;
                    i++;
                }
                else
                    str += ControlToString(controls[i]);

                if (i != controls.Count - 1)
                    str += ", ";
            }

            return str;
        }
        /// <summary> Converts Keyboard.Key or Mouse.Button to their string name. </summary>
        public static string ControlToString(object code)
        {
            if ((int)code >= MouseCodeOffset)
                return MouseButtonToString((int)code);
            else
                return KeyToString((Keyboard.Key)code);
        }

        public static List<int> StringToControls(string[] values, List<int> list = null)
        {
            if (list == null)
                list = new List<int>();
            // Control
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains("+"))
                {
                    // Combo
                    list.Add(-2); // denote that it's a key combo

                    string[] v = values[i].Split('+');

                    Keyboard.Key modifier = StringToKey(v[0].ToUpper());
                    if (modifier != Keyboard.Key.Unknown)
                        list.Add((int)modifier);
                    int control = StringToControl(v[1].ToUpper());
                    if (control != -1)
                        list.Add(control);
                }
                else
                {
                    int control = StringToControl(values[i].ToUpper());
                    if (control != -1)
                        list.Add(control);
                }
            }

            return list;
        }


        /// <summary> Returns true if code is one of Control bindings. </summary>
        public static bool IsControl(object code, List<int> Control, bool onlyIfKeyCombo = false)
        {
            if (Control.Count == 0)
                return false;

            int codeID = -1;
            if (code is Mouse.Button)
                codeID = (int)code + MouseCodeOffset;
            else
                codeID = (int)code;

            int index = Control.IndexOf(codeID);
            if (index == -1)
                return false;
            bool value = false;
            do
            {
                // key-combo?
                if (index >= 1 && Control[index - 1] == -2)
                    value = false;
                else if (index > 1 && Control[index - 2] == -2)
                    value = Keyboard.IsKeyPressed((Keyboard.Key)Control[index - 1]);
                else if (onlyIfKeyCombo)
                    value = false;
                else
                    value = true;

                // loop if there might be second binding using the same keyCode (eg: CTRL+UP and RCTRL+UP)
                if (!value)
                    index = Control.IndexOf(codeID, index + 1);
                else
                    index = -1;
            }
            while (index != -1);

            return value;
        }

        public static void SetControls(List<int> controls, params string[] bindings)
        {
            foreach (string str in bindings)
            {
                if (str.Equals(""))
                    continue;

                if (str.Contains("+"))
                {
                    // Combo
                    controls.Add(-2); // denote that it's a key combo

                    string[] v = str.Split('+');

                    Keyboard.Key modifier = StringToKey(v[0].ToUpper());
                    if (modifier != Keyboard.Key.Unknown)
                        controls.Add((int)modifier);
                    int control = StringToControl(v[1].ToUpper());
                    if (control != -1)
                        controls.Add(control);
                }
                else
                {
                    int control = StringToControl(str.ToUpper());
                    if (control != -1)
                        controls.Add(control);
                }
            }
        }

        public List<int> UpdateControl(string name, int bind)
        {
            name = name.ToUpper();
            if (!Settings.ContainsKey(name))
                return null;

            List<int> Control = (List<int>)Settings[name];

            if (bind == -1)
                Control.Clear();
            else if (Control.IndexOf(bind) == -1)
                Control.Add(bind);

            return Control;
        }

        /// <summary> Converts upper-case string to SFML Mouse.Button (as an int + offset). </summary>
        public static int StringToMouseButton(string str)
        {
            switch (str)
            {
                case "MOUSELEFT":
                case "MOUSE1":
                    return (int)Mouse.Button.Left + MouseCodeOffset;
                case "MOUSERIGHT":
                case "MOUSE2":
                    return (int)Mouse.Button.Right + MouseCodeOffset;
                case "MOUSEMIDDLE":
                case "MOUSE3":
                    return (int)Mouse.Button.Middle + MouseCodeOffset;
                case "MOUSEX1":
                case "MOUSEXBUTTON1":
                case "MOUSE4":
                    return (int)Mouse.Button.XButton1 + MouseCodeOffset;
                case "MOUSEX2":
                case "MOUSEXBUTTON2":
                case "MOUSE5":
                    return (int)Mouse.Button.XButton2 + MouseCodeOffset;
                case "SCROLLUP":
                    return MOUSE_SCROLL_UP;
                case "SCROLLDOWN":
                    return MOUSE_SCROLL_DOWN;
            }

            return -1;
        }
        public static string MouseButtonToString(int code)
        {
            switch (code - MouseCodeOffset)
            {
                case 0: return "MOUSELEFT";
                case 1: return "MOUSERIGHT";
                case 2: return "MOUSEMIDDLE";
                case 3: return "MOUSE4";
                case 4: return "MOUSE5";
                case 5: return "SCROLLUP";
                case 6: return "SCROLLDOWN";
            }

            return "";
        }

        /// <summary> Converts upper-case string to SFML Keyboard.Key. </summary>
        public static Keyboard.Key StringToKey(string str)
        {
            switch (str)
            {
                case "A":
                    return Keyboard.Key.A;
                case "B":
                    return Keyboard.Key.B;
                case "C":
                    return Keyboard.Key.C;
                case "D":
                    return Keyboard.Key.D;
                case "E":
                    return Keyboard.Key.E;
                case "F":
                    return Keyboard.Key.F;
                case "G":
                    return Keyboard.Key.G;
                case "H":
                    return Keyboard.Key.H;
                case "I":
                    return Keyboard.Key.I;
                case "J":
                    return Keyboard.Key.J;
                case "K":
                    return Keyboard.Key.K;
                case "L":
                    return Keyboard.Key.L;
                case "M":
                    return Keyboard.Key.M;
                case "N":
                    return Keyboard.Key.N;
                case "O":
                    return Keyboard.Key.O;
                case "P":
                    return Keyboard.Key.P;
                case "Q":
                    return Keyboard.Key.Q;
                case "R":
                    return Keyboard.Key.R;
                case "S":
                    return Keyboard.Key.S;
                case "T":
                    return Keyboard.Key.T;
                case "U":
                    return Keyboard.Key.U;
                case "V":
                    return Keyboard.Key.V;
                case "W":
                    return Keyboard.Key.W;
                case "X":
                    return Keyboard.Key.X;
                case "Y":
                    return Keyboard.Key.Y;
                case "Z":
                    return Keyboard.Key.Z;
                case "0":
                case "NUM0":
                    return Keyboard.Key.Num0;
                case "1":
                case "NUM1":
                    return Keyboard.Key.Num1;
                case "2":
                case "NUM2":
                    return Keyboard.Key.Num2;
                case "3":
                case "NUM3":
                    return Keyboard.Key.Num3;
                case "4":
                case "NUM4":
                    return Keyboard.Key.Num4;
                case "5":
                case "NUM5":
                    return Keyboard.Key.Num5;
                case "6":
                case "NUM6":
                    return Keyboard.Key.Num6;
                case "7":
                case "NUM7":
                    return Keyboard.Key.Num7;
                case "8":
                case "NUM8":
                    return Keyboard.Key.Num8;
                case "9":
                case "NUM9":
                    return Keyboard.Key.Num9;
                case "ESCAPE":
                case "ESC":
                    return Keyboard.Key.Escape;
                case "CTRL":
                case "CONTROL":
                case "LCTRL":
                case "LEFTCTRL":
                case "LCONTROL":
                case "LEFTCONTROL":
                    return Keyboard.Key.LControl;
                case "SHIFT":
                case "LSHIFT":
                case "LEFTSHIFT":
                    return Keyboard.Key.LShift;
                case "ALT":
                case "LALT":
                case "LEFTALT":
                    return Keyboard.Key.LAlt;
                case "LSYSTEM":
                case "LEFTSYSTEM":
                    return Keyboard.Key.LSystem;
                case "RCTRL":
                case "RIGHTCTRL":
                case "RCONTROL":
                case "RIGHTCONTROL":
                    return Keyboard.Key.RControl;
                case "RSHIFT":
                case "RIGHTSHIFT":
                    return Keyboard.Key.RShift;
                case "RALT":
                case "RIGHTALT":
                    return Keyboard.Key.RAlt;
                case "RSYSTEM":
                case "RIGHTSYSTEM":
                    return Keyboard.Key.RSystem;
                case "MENU":
                    return Keyboard.Key.Menu;
                case "LBRACKET":
                case "[":
                case "{":
                    return Keyboard.Key.LBracket;
                case "RBRACKET":
                case "]":
                case "}":
                    return Keyboard.Key.RBracket;
                case "SEMICOLON":
                case ";":
                case ":":
                    return Keyboard.Key.SemiColon;
                case "COMMA":
                case "<":
                    return Keyboard.Key.Comma;
                case "PERIOD":
                case ">":
                case ".":
                    return Keyboard.Key.Period;
                case "QUOTE":
                case "\"":
                case "'":
                    return Keyboard.Key.Quote;
                case "SLASH":
                case "?":
                case "/":
                case "QUESTION":
                    return Keyboard.Key.Slash;
                case "BACKSLASH":
                case "|":
                case "\\":
                    return Keyboard.Key.BackSlash;
                case "TILDE":
                case "~":
                case "`":
                    return Keyboard.Key.Tilde;
                case "EQUAL":
                case "PLUS":
                case "=":
                    return Keyboard.Key.Equal;
                case "DASH":
                case "MINUS":
                case "_":
                case "-":
                    return Keyboard.Key.Dash;
                case "SPACE":
                    return Keyboard.Key.Space;
                case "RETURN":
                    return Keyboard.Key.Return;
                case "BACK":
                case "BACKSPACE":
                    return Keyboard.Key.BackSpace;
                case "TAB":
                    return Keyboard.Key.Tab;
                case "PAGEUP":
                case "PGUP":
                    return Keyboard.Key.PageUp;
                case "PAGEDOWN":
                case "PGDOWN":
                case "NEXT":
                    return Keyboard.Key.PageDown;
                case "END":
                    return Keyboard.Key.End;
                case "HOME":
                    return Keyboard.Key.Home;
                case "INSERT":
                case "INS":
                    return Keyboard.Key.Insert;
                case "DELETE":
                case "DEL":
                case "DECIMAL":
                    return Keyboard.Key.Delete;
                case "ADD":
                    return Keyboard.Key.Add;
                case "SUBTRACT":
                    return Keyboard.Key.Subtract;
                case "MULTIPLY":
                    return Keyboard.Key.Multiply;
                case "DIVIDE":
                    return Keyboard.Key.Divide;
                case "LEFT":
                    return Keyboard.Key.Left;
                case "RIGHT":
                    return Keyboard.Key.Right;
                case "UP":
                    return Keyboard.Key.Up;
                case "DOWN":
                    return Keyboard.Key.Down;
                case "NUMPAD0":
                    return Keyboard.Key.Numpad0;
                case "NUMPAD1":
                    return Keyboard.Key.Numpad1;
                case "NUMPAD2":
                    return Keyboard.Key.Numpad2;
                case "NUMPAD3":
                    return Keyboard.Key.Numpad3;
                case "NUMPAD4":
                    return Keyboard.Key.Numpad4;
                case "NUMPAD5":
                    return Keyboard.Key.Numpad5;
                case "NUMPAD6":
                    return Keyboard.Key.Numpad6;
                case "NUMPAD7":
                    return Keyboard.Key.Numpad7;
                case "NUMPAD8":
                    return Keyboard.Key.Numpad8;
                case "NUMPAD9":
                    return Keyboard.Key.Numpad9;
                case "F1":
                    return Keyboard.Key.F1;
                case "F2":
                    return Keyboard.Key.F2;
                case "F3":
                    return Keyboard.Key.F3;
                case "F4":
                    return Keyboard.Key.F4;
                case "F5":
                    return Keyboard.Key.F5;
                case "F6":
                    return Keyboard.Key.F6;
                case "F7":
                    return Keyboard.Key.F7;
                case "F8":
                    return Keyboard.Key.F8;
                case "F9":
                    return Keyboard.Key.F9;
                case "F10":
                    return Keyboard.Key.F10;
                case "F11":
                    return Keyboard.Key.F11;
                case "F12":
                    return Keyboard.Key.F12;
                case "F13":
                    return Keyboard.Key.F13;
                case "F14":
                    return Keyboard.Key.F14;
                case "F15":
                    return Keyboard.Key.F15;
                case "PAUSE":
                    return Keyboard.Key.Pause;
            }
            return Keyboard.Key.Unknown;
        }
        /// <summary> Converts SFML Keyboard.Key to upper-case string. </summary>
        public static string KeyToString(Keyboard.Key key)
        {
            switch (key)
            {
                case Keyboard.Key.A:
	                return "A";
                case Keyboard.Key.B:
	                return "B";
                case Keyboard.Key.C:
	                return "C";
                case Keyboard.Key.D:
	                return "D";
                case Keyboard.Key.E:
	                return "E";
                case Keyboard.Key.F:
	                return "F";
                case Keyboard.Key.G:
	                return "G";
                case Keyboard.Key.H:
	                return "H";
                case Keyboard.Key.I:
	                return "I";
                case Keyboard.Key.J:
	                return "J";
                case Keyboard.Key.K:
	                return "K";
                case Keyboard.Key.L:
	                return "L";
                case Keyboard.Key.M:
	                return "M";
                case Keyboard.Key.N:
	                return "N";
                case Keyboard.Key.O:
	                return "O";
                case Keyboard.Key.P:
	                return "P";
                case Keyboard.Key.Q:
	                return "Q";
                case Keyboard.Key.R:
	                return "R";
                case Keyboard.Key.S:
	                return "S";
                case Keyboard.Key.T:
	                return "T";
                case Keyboard.Key.U:
	                return "U";
                case Keyboard.Key.V:
	                return "V";
                case Keyboard.Key.W:
	                return "W";
                case Keyboard.Key.X:
	                return "X";
                case Keyboard.Key.Y:
	                return "Y";
                case Keyboard.Key.Z:
	                return "Z";
                case Keyboard.Key.Num0:
	                return "0";
                case Keyboard.Key.Num1:
	                return "1";
                case Keyboard.Key.Num2:
	                return "2";
                case Keyboard.Key.Num3:
	                return "3";
                case Keyboard.Key.Num4:
	                return "4";
                case Keyboard.Key.Num5:
	                return "5";
                case Keyboard.Key.Num6:
	                return "6";
                case Keyboard.Key.Num7:
	                return "7";
                case Keyboard.Key.Num8:
	                return "8";
                case Keyboard.Key.Num9:
	                return "9";
                case Keyboard.Key.Escape:
	                return "ESC";
                case Keyboard.Key.LControl:
	                return "CTRL";
                case Keyboard.Key.LShift:
	                return "SHIFT";
                case Keyboard.Key.LAlt:
	                return "ALT";
                case Keyboard.Key.LSystem:
	                return "SYSTEM";
                case Keyboard.Key.RControl:
	                return "RCTRL";
                case Keyboard.Key.RShift:
	                return "RSHIFT";
                case Keyboard.Key.RAlt:
	                return "RALT";
                case Keyboard.Key.RSystem:
	                return "RSYSTEM";
                case Keyboard.Key.Menu:
	                return "MENU";
                case Keyboard.Key.LBracket:
	                return "[";
                case Keyboard.Key.RBracket:
	                return "]";
                case Keyboard.Key.SemiColon:
	                return ";";
                case Keyboard.Key.Comma:
	                return "<";
                case Keyboard.Key.Period:
	                return ">";
                case Keyboard.Key.Quote:
	                return "\"";
                case Keyboard.Key.Slash:
	                return "/";
                case Keyboard.Key.BackSlash:
	                return "\\";
                case Keyboard.Key.Tilde:
	                return "~";
                case Keyboard.Key.Equal:
	                return "EQUAL";
                case Keyboard.Key.Dash:
	                return "DASH";
                case Keyboard.Key.Space:
	                return "SPACE";
                case Keyboard.Key.Return:
	                return "RETURN";
                case Keyboard.Key.BackSpace:
	                return "BACK";
                case Keyboard.Key.Tab:
	                return "TAB";
                case Keyboard.Key.PageUp:
	                return "PGUP";
                case Keyboard.Key.PageDown:
	                return "PGDOWN";
                case Keyboard.Key.End:
	                return "END";
                case Keyboard.Key.Home:
	                return "HOME";
                case Keyboard.Key.Insert:
	                return "INSERT";
                case Keyboard.Key.Delete:
	                return "DELETE";
                case Keyboard.Key.Add:
	                return "ADD";
                case Keyboard.Key.Subtract:
	                return "SUBTRACT";
                case Keyboard.Key.Multiply:
	                return "MULTIPLY";
                case Keyboard.Key.Divide:
	                return "DIVIDE";
                case Keyboard.Key.Left:
	                return "LEFT";
                case Keyboard.Key.Right:
	                return "RIGHT";
                case Keyboard.Key.Up:
	                return "UP";
                case Keyboard.Key.Down:
	                return "DOWN";
                case Keyboard.Key.Numpad0:
	                return "NUMPAD0";
                case Keyboard.Key.Numpad1:
	                return "NUMPAD1";
                case Keyboard.Key.Numpad2:
	                return "NUMPAD2";
                case Keyboard.Key.Numpad3:
	                return "NUMPAD3";
                case Keyboard.Key.Numpad4:
	                return "NUMPAD4";
                case Keyboard.Key.Numpad5:
	                return "NUMPAD5";
                case Keyboard.Key.Numpad6:
	                return "NUMPAD6";
                case Keyboard.Key.Numpad7:
	                return "NUMPAD7";
                case Keyboard.Key.Numpad8:
	                return "NUMPAD8";
                case Keyboard.Key.Numpad9:
	                return "NUMPAD9";
                case Keyboard.Key.F1:
	                return "F1";
                case Keyboard.Key.F2:
	                return "F2";
                case Keyboard.Key.F3:
	                return "F3";
                case Keyboard.Key.F4:
	                return "F4";
                case Keyboard.Key.F5:
	                return "F5";
                case Keyboard.Key.F6:
	                return "F6";
                case Keyboard.Key.F7:
	                return "F7";
                case Keyboard.Key.F8:
	                return "F8";
                case Keyboard.Key.F9:
	                return "F9";
                case Keyboard.Key.F10:
	                return "F10";
                case Keyboard.Key.F11:
	                return "F11";
                case Keyboard.Key.F12:
	                return "F12";
                case Keyboard.Key.F13:
	                return "F13";
                case Keyboard.Key.F14:
	                return "F14";
                case Keyboard.Key.F15:
	                return "F15";
                case Keyboard.Key.Pause:
	                return "PAUSE";
            }

            return "";
        }

        public static int StringToControl(string str)
        {
            if (str.StartsWith("MOUSE") || (str == "SCROLLUP" || str == "SCROLLDOWN"))
                return StringToMouseButton(str);
            else
                return (int)StringToKey(str);
        }

        public static bool KeyModifier(Keyboard.Key key)
        {
            return key == Keyboard.Key.LControl || key == Keyboard.Key.LShift || key == Keyboard.Key.LAlt ||
                key == Keyboard.Key.RControl || key == Keyboard.Key.RShift || key == Keyboard.Key.RAlt;
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

        private static string RemoveSpaces(string str) { return str.Replace(" ", "").Replace("\t", ""); }
    }
}