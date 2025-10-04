using System.Collections;
using SFML.Window;

namespace vimage.Common
{
    public enum SortBy
    {
        FolderDefault,
        Name,
        Date,
        DateModified,
        DateCreated,
        Size,
    }

    public enum SortDirection
    {
        FolderDefault,
        Ascending,
        Descending,
    }

    public enum SizingOption
    {
        FitWidth,
        FitHeight,
        FitAuto,
        KeepZoom,
        FullSize,
    }

    public struct ContextMenuItem
    {
        public string name;
        public object func;
    }

    public struct CustomAction
    {
        public string name;
        public string func;
    }

    public struct CustomActionBinding
    {
        public string name;
        public List<int> bindings;
    }

    public class Config
    {
        public List<int> Control_Drag = [];
        public List<int> Control_Close = [];
        public List<int> Control_OpenContextMenu = [];
        public List<int> Control_PrevImage = [];
        public List<int> Control_NextImage = [];
        public List<int> Control_RotateClockwise = [];
        public List<int> Control_RotateAntiClockwise = [];
        public List<int> Control_Flip = [];
        public List<int> Control_FitToMonitorHeight = [];
        public List<int> Control_FitToMonitorWidth = [];
        public List<int> Control_FitToMonitorAuto = [];
        public List<int> Control_FitToMonitorAlt = [];
        public List<int> Control_ZoomIn = [];
        public List<int> Control_ZoomOut = [];
        public List<int> Control_ZoomFaster = [];
        public List<int> Control_ZoomAlt = [];
        public List<int> Control_DragLimitToMonitorBounds = [];
        public List<int> Control_ToggleSmoothing = [];
        public List<int> Control_ToggleBackground = [];
        public List<int> Control_ToggleLock = [];
        public List<int> Control_ToggleAlwaysOnTop = [];
        public List<int> Control_ToggleTitleBar = [];
        public List<int> Control_PauseAnimation = [];
        public List<int> Control_PrevFrame = [];
        public List<int> Control_NextFrame = [];
        public List<int> Control_OpenSettings = [];
        public List<int> Control_ResetImage = [];
        public List<int> Control_OpenAtLocation = [];
        public List<int> Control_Delete = [];
        public List<int> Control_Copy = [];
        public List<int> Control_CopyAsImage = [];
        public List<int> Control_OpenDuplicateImage = [];
        public List<int> Control_OpenFullDuplicateImage = [];
        public List<int> Control_RandomImage = [];
        public List<int> Control_MoveLeft = [];
        public List<int> Control_MoveRight = [];
        public List<int> Control_MoveUp = [];
        public List<int> Control_MoveDown = [];
        public List<int> Control_TransparencyToggle = [];
        public List<int> Control_TransparencyInc = [];
        public List<int> Control_TransparencyDec = [];
        public List<int> Control_Crop = [];
        public List<int> Control_UndoCrop = [];
        public List<int> Control_ExitAll = [];
        public List<int> Control_RerenderSVG = [];

        public List<List<int>> Controls;
        public List<string> ControlNames =
        [
            "Drag",
            "Close",
            "Open Context Menu",
            "Prev Image",
            "Next Image",
            "Rotate Clockwise",
            "Rotate Anti-Clockwise",
            "Flip",
            "Fit To Monitor Auto",
            "Fit To Monitor Width",
            "Fit To Monitor Height",
            "Fit To Monitor Alt",
            "Zoom In",
            "Zoom Out",
            "Zoom Faster",
            "Zoom Alt",
            "Drag Limit to Monitor Bounds",
            "Toggle Smoothing",
            "Toggle Background For Transparency",
            "Toggle Lock",
            "Toggle Always On Top",
            "Toggle Title Bar",
            "Pause Animation",
            "Prev Frame",
            "Next Frame",
            "Open Settings",
            "Reset Image",
            "Open At Location",
            "Delete",
            "Copy",
            "Copy as Image",
            "Open Duplicate Image",
            "Open Full Duplicate Image",
            "Random Image",
            "Move Left",
            "Move Right",
            "Move Up",
            "Move Down",
            "Transparency Toggle",
            "Transparency Increase",
            "Transparency Decrease",
            "Crop",
            "Undo Crop",
            "Exit All Instances",
            "Re-render SVG",
        ];

        public bool Setting_UseDevIL
        {
            get { return (bool)Settings["USEDEVIL"]; }
            set { Settings["USEDEVIL"] = value; }
        }
        public bool Setting_OpenAtMousePosition
        {
            get { return (bool)Settings["OPENATMOUSEPOSITION"]; }
            set { Settings["OPENATMOUSEPOSITION"] = value; }
        }
        public bool Setting_SmoothingDefault
        {
            get { return (bool)Settings["SMOOTHINGDEFAULT"]; }
            set { Settings["SMOOTHINGDEFAULT"] = value; }
        }
        public bool Setting_Mipmapping
        {
            get { return (bool)Settings["MIPMAPPING"]; }
            set { Settings["MIPMAPPING"] = value; }
        }
        public bool Setting_BackgroundForImagesWithTransparencyDefault
        {
            get { return (bool)Settings["BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT"]; }
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
        public SizingOption Setting_ImageSizing
        {
            get { return (SizingOption)Settings["IMAGESIZING"]; }
            set { Settings["IMAGESIZING"] = value; }
        }
        public int Setting_LimitImagesToMonitor
        {
            get { return (int)Settings["LIMITIMAGESTOMONITOR"]; }
            set { Settings["LIMITIMAGESTOMONITOR"] = value; }
        }
        public const int NONE = 0;
        public const int HEIGHT = 1;
        public const int WIDTH = 2;
        public const int AUTO = 3;

        public bool Setting_PositionLargeWideImagesInCorner
        {
            get { return (bool)Settings["POSITIONLARGEWIDEIMAGESINCORNER"]; }
            set { Settings["POSITIONLARGEWIDEIMAGESINCORNER"] = value; }
        }
        public bool Setting_LoopImageNavigation
        {
            get { return (bool)Settings["LOOPIMAGENAVIGATION"]; }
            set { Settings["LOOPIMAGENAVIGATION"] = value; }
        }
        public bool Setting_PreloadNextImage
        {
            get { return (bool)Settings["PRELOADNEXTIMAGE"]; }
            set { Settings["PRELOADNEXTIMAGE"] = value; }
        }
        public bool Setting_ClearMemoryOnResetImage
        {
            get { return (bool)Settings["CLEARMEMORYONRESETIMAGE"]; }
            set { Settings["CLEARMEMORYONRESETIMAGE"] = value; }
        }
        public bool Setting_ShowTitleBar
        {
            get { return (bool)Settings["SHOWTITLEBAR"]; }
            set { Settings["SHOWTITLEBAR"] = value; }
        }
        public bool Setting_OpenSettingsEXE
        {
            get { return (bool)Settings["OPENSETTINGSEXE"]; }
            set { Settings["OPENSETTINGSEXE"] = value; }
        }
        public bool Setting_ListenForConfigChanges
        {
            get { return (bool)Settings["LISTENFORCONFIGCHANGES"]; }
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
        public int Setting_MaxTextureSize
        {
            get { return (int)Settings["MAXTEXTURESIZE"]; }
            set { Settings["MAXTEXTURESIZE"] = value; }
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

        public List<object> ContextMenu = [];
        public List<object> ContextMenu_Animation = [];
        public int ContextMenu_Animation_InsertAtIndex
        {
            get { return (int)Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"]; }
            set { Settings["CONTEXTMENU_ANIMATION_INSERTATINDEX"] = value; }
        }
        public bool ContextMenuShowMargin
        {
            get { return (bool)Settings["CONTEXTMENU_SHOWMARGIN"]; }
            set { Settings["CONTEXTMENU_SHOWMARGIN"] = value; }
        }
        public bool ContextMenuShowMarginSub
        {
            get { return (bool)Settings["CONTEXTMENU_SHOWMARGINSUB"]; }
            set { Settings["CONTEXTMENU_SHOWMARGINSUB"] = value; }
        }

        public List<CustomAction> CustomActions = [];
        public List<CustomActionBinding> CustomActionBindings = [];

        private Dictionary<string, object> Settings = [];

        public const int MouseCodeOffset = 150;
        public const int MOUSE_SCROLL_UP = 155;
        public const int MOUSE_SCROLL_DOWN = 156;

        public Config()
        {
            Controls =
            [
                Control_Drag,
                Control_Close,
                Control_OpenContextMenu,
                Control_PrevImage,
                Control_NextImage,
                Control_RotateClockwise,
                Control_RotateAntiClockwise,
                Control_Flip,
                Control_FitToMonitorHeight,
                Control_FitToMonitorWidth,
                Control_FitToMonitorAuto,
                Control_FitToMonitorAlt,
                Control_ZoomIn,
                Control_ZoomOut,
                Control_ZoomFaster,
                Control_ZoomAlt,
                Control_DragLimitToMonitorBounds,
                Control_ToggleSmoothing,
                Control_ToggleBackground,
                Control_ToggleLock,
                Control_ToggleAlwaysOnTop,
                Control_ToggleTitleBar,
                Control_PauseAnimation,
                Control_PrevFrame,
                Control_NextFrame,
                Control_OpenSettings,
                Control_ResetImage,
                Control_OpenAtLocation,
                Control_Delete,
                Control_Copy,
                Control_CopyAsImage,
                Control_OpenDuplicateImage,
                Control_OpenFullDuplicateImage,
                Control_RandomImage,
                Control_MoveLeft,
                Control_MoveRight,
                Control_MoveUp,
                Control_MoveDown,
                Control_TransparencyToggle,
                Control_TransparencyInc,
                Control_TransparencyDec,
                Control_Crop,
                Control_UndoCrop,
                Control_ExitAll,
                Control_RerenderSVG,
            ];

            Init();
        }

        public void Init()
        {
            SetDefaultControls();

            SetDefaultContextMenu();
            SetDefaultCustomActions();

            Settings = new Dictionary<string, object>()
            {
                { "USEDEVIL", true },
                { "OPENATMOUSEPOSITION", true },
                { "SMOOTHINGDEFAULT", true },
                { "MIPMAPPING", true },
                { "BACKGROUNDFORIMAGESWITHTRANSPARENCYDEFAULT", false },
                { "BACKGROUNDCOLOUR", "#4D000000" },
                { "TRANSPARENCYTOGGLEVALUE", "#BEFFFFFF" },
                { "IMAGESIZING", SizingOption.FitWidth },
                { "LIMITIMAGESTOMONITOR", AUTO },
                { "POSITIONLARGEWIDEIMAGESINCORNER", true },
                { "LOOPIMAGENAVIGATION", true },
                { "PRELOADNEXTIMAGE", true },
                { "CLEARMEMORYONRESETIMAGE", true },
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
                { "MAXTEXTURESIZE", 10000 },
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
                { "RERENDERSVG", Control_RerenderSVG },
                { "CONTEXTMENU", ContextMenu },
                { "CONTEXTMENU_ANIMATION", ContextMenu_Animation },
                { "CONTEXTMENU_ANIMATION_INSERTATINDEX", 2 },
                { "CONTEXTMENU_SHOWMARGIN", false },
                { "CONTEXTMENU_SHOWMARGINSUB", true },
                { "CUSTOMACTIONS", CustomActions },
                { "CUSTOMACTIONBINDINGS", CustomActionBindings },
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
            Control_RerenderSVG.Clear();

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
            SetControls(Control_RerenderSVG, "SHIFT+R");
        }

        public void SetDefaultContextMenu()
        {
            ContextMenu.Clear();
            ContextMenu_Animation.Clear();

            ContextMenu.Add(new ContextMenuItem { name = "Close", func = Action.Close });
            ContextMenu.Add(new ContextMenuItem { name = "-", func = Action.None });
            ContextMenu.Add(new ContextMenuItem { name = "Next", func = Action.NextImage });
            ContextMenu.Add(new ContextMenuItem { name = "Previous", func = Action.PrevImage });
            ContextMenu.Add("Sort by");
            List<object> SubMenu_SortBy =
            [
                new ContextMenuItem { name = "Name", func = Action.SortName },
                new ContextMenuItem { name = "Date", func = Action.SortDate },
                new ContextMenuItem { name = "Date modified", func = Action.SortDateModified },
                new ContextMenuItem { name = "Date created", func = Action.SortDateCreated },
                new ContextMenuItem { name = "Size", func = Action.SortSize },
                new ContextMenuItem { name = "-", func = Action.None },
                new ContextMenuItem { name = "Ascending", func = Action.SortAscending },
                new ContextMenuItem { name = "Descending", func = Action.SortDescending },
            ];
            ContextMenu.Add(SubMenu_SortBy);
            ContextMenu.Add(new ContextMenuItem { name = "-", func = Action.None });
            ContextMenu.Add(
                new ContextMenuItem { name = "Rotate right", func = Action.RotateClockwise }
            );
            ContextMenu.Add(
                new ContextMenuItem { name = "Rotate left", func = Action.RotateAntiClockwise }
            );
            ContextMenu.Add(new ContextMenuItem { name = "Flip", func = Action.Flip });
            ContextMenu.Add(
                new ContextMenuItem { name = "Fit to height", func = Action.FitToMonitorHeight }
            );
            ContextMenu.Add(
                new ContextMenuItem { name = "Fit to width", func = Action.FitToMonitorWidth }
            );
            ContextMenu.Add(
                new ContextMenuItem { name = "Smoothing", func = Action.ToggleSmoothing }
            );
            ContextMenu.Add(
                new ContextMenuItem { name = "Always on top", func = Action.ToggleAlwaysOnTop }
            );
            ContextMenu.Add(new ContextMenuItem { name = "Reset", func = Action.ResetImage });
            ContextMenu.Add(new ContextMenuItem { name = "-", func = Action.None });
            ContextMenu.Add(new ContextMenuItem { name = "Edit", func = "EDIT PAINT" });
            ContextMenu.Add(new ContextMenuItem { name = "Copy", func = Action.Copy });
            ContextMenu.Add(new ContextMenuItem { name = "Delete", func = Action.Delete });
            ContextMenu.Add(new ContextMenuItem { name = "-", func = Action.None });
            ContextMenu.Add(
                new ContextMenuItem { name = "[filename.14]", func = Action.OpenAtLocation }
            );
            ContextMenu.Add(new ContextMenuItem { name = "-", func = Action.None });
            ContextMenu.Add(new ContextMenuItem { name = "Settings", func = Action.OpenSettings });
            ContextMenu.Add(
                new ContextMenuItem { name = "vimage [version]", func = Action.VisitWebsite }
            );

            ContextMenu_Animation.Add(
                new ContextMenuItem { name = "Pause/Play", func = Action.PauseAnimation }
            );
            ContextMenu_Animation.Add(new ContextMenuItem { name = "-", func = Action.None });
        }

        public void SetDefaultCustomActions()
        {
            CustomActions.Clear();
            CustomActionBindings.Clear();

            CustomActions.Add(
                new CustomAction
                {
                    name = "TOGGLE OVERLAY MODE",
                    func = "-toggleSync -clickThrough -alwaysOnTop -defaultTransparency",
                }
            );
            CustomActionBindings.Add(
                new CustomActionBinding { name = "TOGGLE OVERLAY MODE", bindings = [-2, 38, 11] }
            );
            CustomActions.Add(new CustomAction { name = "EDIT PAINT", func = @"mspaint.exe %f" });
            CustomActionBindings.Add(
                new CustomActionBinding { name = "EDIT PAINT", bindings = [] }
            );
            CustomActions.Add(
                new CustomAction
                {
                    name = "EDIT PAINTDOTNET",
                    func = "\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\" %f",
                }
            );
            CustomActionBindings.Add(
                new CustomActionBinding { name = "EDIT PAINTDOTNET", bindings = [] }
            );
            CustomActions.Add(new CustomAction { name = "TOGGLE TASKBAR", func = "-taskbarIcon" });
            CustomActionBindings.Add(
                new CustomActionBinding { name = "TOGGLE TASKBAR", bindings = [] }
            );
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
            foreach (KeyValuePair<string, object> list in Settings)
            {
                if (list.Value is List<int> list1)
                    list1.Clear();
            }
            ContextMenu.Clear();
            ContextMenu_Animation.Clear();
            CustomActions.Clear();
            CustomActionBindings.Clear();

            var reader = File.OpenText(configFile);
            var line = reader.ReadLine();

            while (line != null)
            {
                // if line is empty of has no '=' symbol, go to next
                if (line.Equals("") || !line.Contains('='))
                {
                    line = reader.ReadLine();
                    continue;
                }

                // trim
                line = RemoveSpaces(line);

                // ignore comments
                if (line.IndexOf("//") is int commentIndex and >= 0)
                    line = line[..commentIndex];

                // split variable name and values
                var nameValue = line.Split('=');

                // invalid name?
                var name = nameValue[0].ToUpper();
                if (!Settings.ContainsKey(name))
                {
                    line = reader.ReadLine();
                    continue;
                }

                // nothing after '='?, check next line
                if (nameValue[1].Equals(""))
                {
                    line = reader.ReadLine();
                    if (line == null)
                        continue;
                    line = RemoveSpaces(line);

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.Contains('='))
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    // read section
                    if (Settings[name] is IList settingList)
                        line = ReadSection(reader, settingList, name);
                    continue;
                }

                // split values by commas
                var values = nameValue[1].Split(',');

                // Assign Values
                if (Settings[name] is List<int> list)
                {
                    // Control
                    _ = StringToControls(values, list);
                }
                else if (Settings[name] is int || Settings[name] is Enum)
                {
                    // Integer
                    if (int.TryParse(values[0], out int i))
                        Settings[name] = i;
                }
                else if (Settings[name] is bool)
                {
                    // Boolean
                    Settings[name] =
                        values[0].Equals("1")
                        || values[0].ToUpper().Equals("T")
                        || values[0].ToUpper().Equals("TRUE")
                            ? true
                            : (object)false;
                }
                else if (Settings[name] is string)
                {
                    Settings[name] = values[0];
                }

                // next line
                line = reader.ReadLine();
            }

            reader.Close();
        }

        private static string? ReadSection(
            StreamReader reader,
            IList setting,
            string sectionName = ""
        )
        {
            var line = reader.ReadLine();
            if (line is null)
                return line;
            var trimedLine = RemoveSpaces(line);
            string[] splitValues;

            if (trimedLine.Equals("}"))
                return line;

            while (line is not null && trimedLine is not null)
            {
                if (!trimedLine.Equals("-") && !trimedLine.Contains(':'))
                {
                    // Subsection
                    string subSectionName = line.Replace("\t", "");

                    line = reader.ReadLine();
                    if (line is null)
                        continue;
                    line = RemoveSpaces(line);

                    // line is empty or is part of another setting, skip
                    if (line.Equals("") || line.Contains('='))
                        continue;
                    // line doesn't have open brace, skip
                    if (!line.Equals("{"))
                        continue;

                    setting.Add(subSectionName);
                    List<object> subSetting = [];
                    setting.Add(subSetting);

                    line = ReadSection(reader, subSetting, sectionName);
                    trimedLine = line;
                }
                else
                {
                    // Item
                    if (trimedLine.Equals("-"))
                        splitValues = ["-", "-"]; // line break
                    else
                        splitValues = line.Split([':'], 2);

                    // trim tabs from name, spaces from map name
                    splitValues[0] = splitValues[0].Replace("\t", "").Trim();
                    splitValues[1] = splitValues[1].Trim();

                    // assign Values
                    if (sectionName == "CUSTOMACTIONBINDINGS")
                    {
                        setting.Add(
                            new CustomActionBinding
                            {
                                name = splitValues[0],
                                bindings = StringToControls(splitValues[1].Split(',')),
                            }
                        );
                    }
                    else if (sectionName.Contains("CONTEXTMENU"))
                    {
                        var action = Actions.StringToAction(splitValues[1]);
                        setting.Add(
                            new ContextMenuItem
                            {
                                name = splitValues[0],
                                func = action <= 0 ? splitValues[1] : action,
                            }
                        );
                    }
                    else if (sectionName.Contains("CUSTOMACTIONS"))
                    {
                        setting.Add(
                            new CustomAction { name = splitValues[0], func = splitValues[1] }
                        );
                    }
                    else
                    {
                        setting.Add(
                            new ContextMenuItem { name = splitValues[0], func = splitValues[1] }
                        );
                    }

                    // next line
                    line = reader.ReadLine();
                    if (line == null)
                        continue;
                    trimedLine = RemoveSpaces(line);
                }

                // break if end brace
                if (trimedLine is not null && trimedLine.Equals("}"))
                {
                    if (reader.Peek() == -1)
                        return line;
                    line = reader.ReadLine();
                    if (line is not null)
                        line = RemoveSpaces(line);
                    break;
                }
            }

            return line;
        }

        /// <summary> Saves settings to config txt file. </summary>
        public void Save(string configFile)
        {
            // Open
            FileStream fileStream;
            if (File.Exists(configFile))
            {
                // Clear if file already exists
                File.WriteAllText(configFile, string.Empty);
                fileStream = File.Open(configFile, FileMode.Open, FileAccess.Write);
            }
            else
                fileStream = File.Create(configFile);
            var writer = new StreamWriter(fileStream);

            // Write
            writer.Write("// General Settings" + Environment.NewLine);

            WriteSetting(writer, "UseDevil", Setting_UseDevIL);
            WriteSetting(writer, "OpenAtMousePosition", Setting_OpenAtMousePosition);
            WriteSetting(writer, "SmoothingDefault", Setting_SmoothingDefault);
            WriteSetting(writer, "Mipmapping", Setting_Mipmapping);
            WriteSetting(
                writer,
                "BackgroundForImagesWithTransparencyDefault",
                Setting_BackgroundForImagesWithTransparencyDefault
            );
            WriteSetting(writer, "BackgroundColour", Setting_BackgroundColour);
            WriteSetting(writer, "TransparencyToggleValue", Setting_TransparencyToggleValue);
            WriteSetting(writer, "ImageSizing", (int)Setting_ImageSizing);
            WriteSetting(
                writer,
                "LimitImagesToMonitor",
                Setting_LimitImagesToMonitor,
                "0=NONE, 1=HEIGHT, 2=WIDTH, 3=AUTO"
            );
            WriteSetting(
                writer,
                "PositionLargeWideImagesInCorner",
                Setting_PositionLargeWideImagesInCorner,
                "ie: Desktop Wallpapers and Screenshots"
            );
            WriteSetting(writer, "LoopImageNavigation", Setting_LoopImageNavigation);
            WriteSetting(
                writer,
                "PreloadNextImage",
                Setting_PreloadNextImage,
                "when using the next/prev image buttons, the image after the one just loaded will be loaded as well"
            );
            WriteSetting(
                writer,
                "ClearMemoryOnResetImage",
                Setting_ClearMemoryOnResetImage,
                "when the Reset Image action is used, all textures/animations will be cleared from memory (except ones used for current image)"
            );
            WriteSetting(writer, "ShowTitleBar", Setting_ShowTitleBar);
            WriteSetting(
                writer,
                "OpenSettingsEXE",
                Setting_OpenSettingsEXE,
                "if false, will open config.txt instead"
            );
            WriteSetting(
                writer,
                "ListenForConfigChanges",
                Setting_ListenForConfigChanges,
                "vimage will reload settings automatically when they are changed."
            );
            WriteSetting(
                writer,
                "MinImageSize",
                Setting_MinImageSize,
                "if an image is smaller than this (in width or height) it will scaled up to it automatically"
            );
            WriteSetting(
                writer,
                "SmoothingMinImageSize",
                Setting_SmoothingMinImageSize,
                "images smaller than this will not have smoothing turned on (if 0, all images with use smoothing)"
            );
            WriteSetting(writer, "ZoomSpeed", Setting_ZoomSpeed);
            WriteSetting(writer, "ZoomSpeedFast", Setting_ZoomSpeedFast);
            WriteSetting(writer, "MoveSpeed", Setting_MoveSpeed);
            WriteSetting(writer, "MoveSpeedFast", Setting_MoveSpeedFast);
            WriteSetting(writer, "MaxTextures", Setting_MaxTextures);
            WriteSetting(writer, "MaxAnimations", Setting_MaxAnimations);
            WriteSetting(
                writer,
                "MaxTextureSize",
                Setting_MaxTextureSize,
                "will cut up images into multiple textures if they are larger than this value"
            );
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
            WriteControl(writer, "RerenderSVG", Control_RerenderSVG);

            writer.Write(Environment.NewLine);
            writer.Write("// Context Menu" + Environment.NewLine);

            WriteContextMenuSetup(writer, "ContextMenu", ContextMenu);
            WriteContextMenuSetup(writer, "ContextMenu_Animation", ContextMenu_Animation);

            WriteSetting(
                writer,
                "ContextMenu_Animation_InsertAtIndex",
                ContextMenu_Animation_InsertAtIndex
            );
            WriteSetting(writer, "ContextMenu_ShowMargin", ContextMenuShowMargin);
            WriteSetting(writer, "ContextMenu_ShowMarginSub", ContextMenuShowMarginSub);

            writer.Write(Environment.NewLine);
            WriteCustomActions(writer, "CustomActions", CustomActions);
            WriteCustomActionBindings(writer, "CustomActionBindings", CustomActionBindings);

            // Close
            writer.Close();
        }

        private static void WriteSetting(
            StreamWriter writer,
            string name,
            bool value,
            string comment = ""
        )
        {
            writer.Write(name + " = " + (value ? 1 : 0));
            WriteComment(writer, comment);
        }

        private static void WriteSetting(
            StreamWriter writer,
            string name,
            int value,
            string comment = ""
        )
        {
            writer.Write(name + " = " + value.ToString());
            WriteComment(writer, comment);
        }

        private static void WriteSetting(
            StreamWriter writer,
            string name,
            string value,
            string comment = ""
        )
        {
            writer.Write(name + " = " + value);
            WriteComment(writer, comment);
        }

        private static void WriteComment(StreamWriter writer, string comment = "")
        {
            if (!comment.Equals(""))
                writer.Write(" // " + comment);
            writer.Write(Environment.NewLine);
        }

        private static void WriteControl(StreamWriter writer, string name, List<int> controls)
        {
            writer.Write(name + " = " + ControlsToString(controls) + Environment.NewLine);
        }

        private static void WriteContextMenuSetup(
            StreamWriter writer,
            string name,
            List<object> contextMenu
        )
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            WriteContextMenuItems(writer, contextMenu, 1);
            writer.Write("}" + Environment.NewLine);
        }

        private static void WriteContextMenuItems(StreamWriter writer, IList items, int depth = 1)
        {
            if (items is null)
                return;

            for (int i = 0; i < items.Count; i++)
            {
                writer.Write(VariableAmountOfStrings(depth, "\t"));

                if (items[i] is string str)
                {
                    // Submenu
                    writer.Write(
                        str
                            + Environment.NewLine
                            + VariableAmountOfStrings(depth, "\t")
                            + "{"
                            + Environment.NewLine
                    );
                    i++;
                    if (items[i] is IList itemList)
                        WriteContextMenuItems(writer, itemList, depth + 1);
                    writer.Write(VariableAmountOfStrings(depth, "\t") + "}" + Environment.NewLine);
                }
                else if (items[i] is ContextMenuItem contextMenuItem)
                {
                    // Item
                    var itemName = contextMenuItem.name;
                    if (itemName is not null)
                    {
                        var itemFunc = (string)(
                            contextMenuItem.func is Action action
                                ? action.ToNameString()
                                : contextMenuItem.func
                        );
                        if (itemName.Equals("-"))
                            writer.Write("-" + Environment.NewLine);
                        else if (itemName.Equals(""))
                            writer.Write(": " + itemFunc + Environment.NewLine);
                        else
                            writer.Write(itemName + " : " + itemFunc + Environment.NewLine);
                    }
                }
                else
                {
                    Console.WriteLine("WTF");
                }
            }
        }

        private static void WriteCustomActions(
            StreamWriter writer,
            string name,
            List<CustomAction> customActions
        )
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            for (int i = 0; i < customActions.Count; i++)
            {
                writer.Write(
                    "\t"
                        + customActions[i].name
                        + " : "
                        + customActions[i].func
                        + Environment.NewLine
                );
            }
            writer.Write("}" + Environment.NewLine);
        }

        private static void WriteCustomActionBindings(
            StreamWriter writer,
            string name,
            List<CustomActionBinding> customActionBindings
        )
        {
            writer.Write(name + " =" + Environment.NewLine + "{" + Environment.NewLine);
            for (int i = 0; i < customActionBindings.Count; i++)
            {
                writer.Write(
                    "\t"
                        + customActionBindings[i].name
                        + " : "
                        + ControlsToString(customActionBindings[i].bindings)
                        + Environment.NewLine
                );
            }
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
                    str += ControlToString(controls[i]) + "+" + ControlToString(controls[i + 1]);
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
            return (int)code >= MouseCodeOffset
                ? MouseButtonToString((int)code)
                : KeyToString((Keyboard.Key)code);
        }

        public static List<int> StringToControls(string[] values, List<int>? list = null)
        {
            list ??= [];
            // Control
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i].Contains('+'))
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

            int codeID = code is Mouse.Button ? (int)code + MouseCodeOffset : (int)code;
            int index = Control.IndexOf(codeID);
            if (index == -1)
                return false;
            bool value;
            do
            {
                // key-combo?
                value =
                    (index < 1 || Control[index - 1] != -2)
                    && (
                        index > 1 && Control[index - 2] == -2
                            ? Keyboard.IsKeyPressed((Keyboard.Key)Control[index - 1])
                            : !onlyIfKeyCombo
                    );

                // loop if there might be second binding using the same keyCode (eg: CTRL+UP and RCTRL+UP)
                index = !value ? Control.IndexOf(codeID, index + 1) : -1;
            } while (index != -1);

            return value;
        }

        public static void SetControls(List<int> controls, params string[] bindings)
        {
            foreach (string str in bindings)
            {
                if (str.Equals(""))
                    continue;

                if (str.Contains('+'))
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

        public List<int>? UpdateControl(string name, int bind)
        {
            name = name.ToUpper();
            if (!Settings.TryGetValue(name, out object? value))
                return null;

            var Control = (List<int>)value;

            if (bind == -1)
                Control.Clear();
            else if (Control.IndexOf(bind) == -1)
                Control.Add(bind);

            return Control;
        }

        /// <summary> Converts upper-case string to SFML Mouse.Button (as an int + offset). </summary>
        public static int StringToMouseButton(string str)
        {
            return str switch
            {
                "MOUSELEFT" or "MOUSE1" => (int)Mouse.Button.Left + MouseCodeOffset,
                "MOUSERIGHT" or "MOUSE2" => (int)Mouse.Button.Right + MouseCodeOffset,
                "MOUSEMIDDLE" or "MOUSE3" => (int)Mouse.Button.Middle + MouseCodeOffset,
                "MOUSEX1" or "MOUSEXBUTTON1" or "MOUSE4" => (int)Mouse.Button.XButton1
                    + MouseCodeOffset,
                "MOUSEX2" or "MOUSEXBUTTON2" or "MOUSE5" => (int)Mouse.Button.XButton2
                    + MouseCodeOffset,
                "SCROLLUP" => MOUSE_SCROLL_UP,
                "SCROLLDOWN" => MOUSE_SCROLL_DOWN,
                _ => -1,
            };
        }

        public static string MouseButtonToString(int code)
        {
            return (code - MouseCodeOffset) switch
            {
                0 => "MOUSELEFT",
                1 => "MOUSERIGHT",
                2 => "MOUSEMIDDLE",
                3 => "MOUSE4",
                4 => "MOUSE5",
                5 => "SCROLLUP",
                6 => "SCROLLDOWN",
                _ => "",
            };
        }

        /// <summary> Converts upper-case string to SFML Keyboard.Key. </summary>
        public static Keyboard.Key StringToKey(string str)
        {
            return str switch
            {
                "A" => Keyboard.Key.A,
                "B" => Keyboard.Key.B,
                "C" => Keyboard.Key.C,
                "D" => Keyboard.Key.D,
                "E" => Keyboard.Key.E,
                "F" => Keyboard.Key.F,
                "G" => Keyboard.Key.G,
                "H" => Keyboard.Key.H,
                "I" => Keyboard.Key.I,
                "J" => Keyboard.Key.J,
                "K" => Keyboard.Key.K,
                "L" => Keyboard.Key.L,
                "M" => Keyboard.Key.M,
                "N" => Keyboard.Key.N,
                "O" => Keyboard.Key.O,
                "P" => Keyboard.Key.P,
                "Q" => Keyboard.Key.Q,
                "R" => Keyboard.Key.R,
                "S" => Keyboard.Key.S,
                "T" => Keyboard.Key.T,
                "U" => Keyboard.Key.U,
                "V" => Keyboard.Key.V,
                "W" => Keyboard.Key.W,
                "X" => Keyboard.Key.X,
                "Y" => Keyboard.Key.Y,
                "Z" => Keyboard.Key.Z,
                "0" or "NUM0" => Keyboard.Key.Num0,
                "1" or "NUM1" => Keyboard.Key.Num1,
                "2" or "NUM2" => Keyboard.Key.Num2,
                "3" or "NUM3" => Keyboard.Key.Num3,
                "4" or "NUM4" => Keyboard.Key.Num4,
                "5" or "NUM5" => Keyboard.Key.Num5,
                "6" or "NUM6" => Keyboard.Key.Num6,
                "7" or "NUM7" => Keyboard.Key.Num7,
                "8" or "NUM8" => Keyboard.Key.Num8,
                "9" or "NUM9" => Keyboard.Key.Num9,
                "ESCAPE" or "ESC" => Keyboard.Key.Escape,
                "CTRL" or "CONTROL" or "LCTRL" or "LEFTCTRL" or "LCONTROL" or "LEFTCONTROL" =>
                    Keyboard.Key.LControl,
                "SHIFT" or "LSHIFT" or "LEFTSHIFT" => Keyboard.Key.LShift,
                "ALT" or "LALT" or "LEFTALT" => Keyboard.Key.LAlt,
                "LSYSTEM" or "LEFTSYSTEM" => Keyboard.Key.LSystem,
                "RCTRL" or "RIGHTCTRL" or "RCONTROL" or "RIGHTCONTROL" => Keyboard.Key.RControl,
                "RSHIFT" or "RIGHTSHIFT" => Keyboard.Key.RShift,
                "RALT" or "RIGHTALT" => Keyboard.Key.RAlt,
                "RSYSTEM" or "RIGHTSYSTEM" => Keyboard.Key.RSystem,
                "MENU" => Keyboard.Key.Menu,
                "LBRACKET" or "[" or "{" => Keyboard.Key.LBracket,
                "RBRACKET" or "]" or "}" => Keyboard.Key.RBracket,
                "SEMICOLON" or ";" or ":" => Keyboard.Key.Semicolon,
                "COMMA" or "<" => Keyboard.Key.Comma,
                "PERIOD" or ">" or "." => Keyboard.Key.Period,
                "QUOTE" or "APOSTROPHE" or "\"" or "'" => Keyboard.Key.Apostrophe,
                "SLASH" or "?" or "/" or "QUESTION" => Keyboard.Key.Slash,
                "BACKSLASH" or "|" or "\\" => Keyboard.Key.Backslash,
                "TILDE" or "GRAVE" or "~" or "`" => Keyboard.Key.Grave,
                "EQUAL" or "PLUS" or "=" => Keyboard.Key.Equal,
                "DASH" or "MINUS" or "_" or "-" => Keyboard.Key.Hyphen,
                "SPACE" => Keyboard.Key.Space,
                "RETURN" => Keyboard.Key.Enter,
                "BACK" or "BACKSPACE" => Keyboard.Key.Backspace,
                "TAB" => Keyboard.Key.Tab,
                "PAGEUP" or "PGUP" => Keyboard.Key.PageUp,
                "PAGEDOWN" or "PGDOWN" or "NEXT" => Keyboard.Key.PageDown,
                "END" => Keyboard.Key.End,
                "HOME" => Keyboard.Key.Home,
                "INSERT" or "INS" => Keyboard.Key.Insert,
                "DELETE" or "DEL" or "DECIMAL" => Keyboard.Key.Delete,
                "ADD" => Keyboard.Key.Add,
                "SUBTRACT" => Keyboard.Key.Subtract,
                "MULTIPLY" => Keyboard.Key.Multiply,
                "DIVIDE" => Keyboard.Key.Divide,
                "LEFT" => Keyboard.Key.Left,
                "RIGHT" => Keyboard.Key.Right,
                "UP" => Keyboard.Key.Up,
                "DOWN" => Keyboard.Key.Down,
                "NUMPAD0" => Keyboard.Key.Numpad0,
                "NUMPAD1" => Keyboard.Key.Numpad1,
                "NUMPAD2" => Keyboard.Key.Numpad2,
                "NUMPAD3" => Keyboard.Key.Numpad3,
                "NUMPAD4" => Keyboard.Key.Numpad4,
                "NUMPAD5" => Keyboard.Key.Numpad5,
                "NUMPAD6" => Keyboard.Key.Numpad6,
                "NUMPAD7" => Keyboard.Key.Numpad7,
                "NUMPAD8" => Keyboard.Key.Numpad8,
                "NUMPAD9" => Keyboard.Key.Numpad9,
                "F1" => Keyboard.Key.F1,
                "F2" => Keyboard.Key.F2,
                "F3" => Keyboard.Key.F3,
                "F4" => Keyboard.Key.F4,
                "F5" => Keyboard.Key.F5,
                "F6" => Keyboard.Key.F6,
                "F7" => Keyboard.Key.F7,
                "F8" => Keyboard.Key.F8,
                "F9" => Keyboard.Key.F9,
                "F10" => Keyboard.Key.F10,
                "F11" => Keyboard.Key.F11,
                "F12" => Keyboard.Key.F12,
                "F13" => Keyboard.Key.F13,
                "F14" => Keyboard.Key.F14,
                "F15" => Keyboard.Key.F15,
                "PAUSE" => Keyboard.Key.Pause,
                _ => Keyboard.Key.Unknown,
            };
        }

        /// <summary> Converts SFML Keyboard.Key to upper-case string. </summary>
        public static string KeyToString(Keyboard.Key key)
        {
            return key switch
            {
                Keyboard.Key.A => "A",
                Keyboard.Key.B => "B",
                Keyboard.Key.C => "C",
                Keyboard.Key.D => "D",
                Keyboard.Key.E => "E",
                Keyboard.Key.F => "F",
                Keyboard.Key.G => "G",
                Keyboard.Key.H => "H",
                Keyboard.Key.I => "I",
                Keyboard.Key.J => "J",
                Keyboard.Key.K => "K",
                Keyboard.Key.L => "L",
                Keyboard.Key.M => "M",
                Keyboard.Key.N => "N",
                Keyboard.Key.O => "O",
                Keyboard.Key.P => "P",
                Keyboard.Key.Q => "Q",
                Keyboard.Key.R => "R",
                Keyboard.Key.S => "S",
                Keyboard.Key.T => "T",
                Keyboard.Key.U => "U",
                Keyboard.Key.V => "V",
                Keyboard.Key.W => "W",
                Keyboard.Key.X => "X",
                Keyboard.Key.Y => "Y",
                Keyboard.Key.Z => "Z",
                Keyboard.Key.Num0 => "0",
                Keyboard.Key.Num1 => "1",
                Keyboard.Key.Num2 => "2",
                Keyboard.Key.Num3 => "3",
                Keyboard.Key.Num4 => "4",
                Keyboard.Key.Num5 => "5",
                Keyboard.Key.Num6 => "6",
                Keyboard.Key.Num7 => "7",
                Keyboard.Key.Num8 => "8",
                Keyboard.Key.Num9 => "9",
                Keyboard.Key.Escape => "ESC",
                Keyboard.Key.LControl => "CTRL",
                Keyboard.Key.LShift => "SHIFT",
                Keyboard.Key.LAlt => "ALT",
                Keyboard.Key.LSystem => "SYSTEM",
                Keyboard.Key.RControl => "RCTRL",
                Keyboard.Key.RShift => "RSHIFT",
                Keyboard.Key.RAlt => "RALT",
                Keyboard.Key.RSystem => "RSYSTEM",
                Keyboard.Key.Menu => "MENU",
                Keyboard.Key.LBracket => "[",
                Keyboard.Key.RBracket => "]",
                Keyboard.Key.Semicolon => ";",
                Keyboard.Key.Comma => "<",
                Keyboard.Key.Period => ">",
                Keyboard.Key.Apostrophe => "\"",
                Keyboard.Key.Slash => "/",
                Keyboard.Key.Backslash => "\\",
                Keyboard.Key.Grave => "~",
                Keyboard.Key.Equal => "EQUAL",
                Keyboard.Key.Hyphen => "DASH",
                Keyboard.Key.Space => "SPACE",
                Keyboard.Key.Enter => "RETURN",
                Keyboard.Key.Backspace => "BACK",
                Keyboard.Key.Tab => "TAB",
                Keyboard.Key.PageUp => "PGUP",
                Keyboard.Key.PageDown => "PGDOWN",
                Keyboard.Key.End => "END",
                Keyboard.Key.Home => "HOME",
                Keyboard.Key.Insert => "INSERT",
                Keyboard.Key.Delete => "DELETE",
                Keyboard.Key.Add => "ADD",
                Keyboard.Key.Subtract => "SUBTRACT",
                Keyboard.Key.Multiply => "MULTIPLY",
                Keyboard.Key.Divide => "DIVIDE",
                Keyboard.Key.Left => "LEFT",
                Keyboard.Key.Right => "RIGHT",
                Keyboard.Key.Up => "UP",
                Keyboard.Key.Down => "DOWN",
                Keyboard.Key.Numpad0 => "NUMPAD0",
                Keyboard.Key.Numpad1 => "NUMPAD1",
                Keyboard.Key.Numpad2 => "NUMPAD2",
                Keyboard.Key.Numpad3 => "NUMPAD3",
                Keyboard.Key.Numpad4 => "NUMPAD4",
                Keyboard.Key.Numpad5 => "NUMPAD5",
                Keyboard.Key.Numpad6 => "NUMPAD6",
                Keyboard.Key.Numpad7 => "NUMPAD7",
                Keyboard.Key.Numpad8 => "NUMPAD8",
                Keyboard.Key.Numpad9 => "NUMPAD9",
                Keyboard.Key.F1 => "F1",
                Keyboard.Key.F2 => "F2",
                Keyboard.Key.F3 => "F3",
                Keyboard.Key.F4 => "F4",
                Keyboard.Key.F5 => "F5",
                Keyboard.Key.F6 => "F6",
                Keyboard.Key.F7 => "F7",
                Keyboard.Key.F8 => "F8",
                Keyboard.Key.F9 => "F9",
                Keyboard.Key.F10 => "F10",
                Keyboard.Key.F11 => "F11",
                Keyboard.Key.F12 => "F12",
                Keyboard.Key.F13 => "F13",
                Keyboard.Key.F14 => "F14",
                Keyboard.Key.F15 => "F15",
                Keyboard.Key.Pause => "PAUSE",
                _ => "",
            };
        }

        public static int StringToControl(string str)
        {
            return str.StartsWith("MOUSE") || str == "SCROLLUP" || str == "SCROLLDOWN"
                ? StringToMouseButton(str)
                : (int)StringToKey(str);
        }

        public static bool KeyModifier(Keyboard.Key key)
        {
            return key == Keyboard.Key.LControl
                || key == Keyboard.Key.LShift
                || key == Keyboard.Key.LAlt
                || key == Keyboard.Key.RControl
                || key == Keyboard.Key.RShift
                || key == Keyboard.Key.RAlt;
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

        private static string RemoveSpaces(string str)
        {
            return str.Replace(" ", "").Replace("\t", "");
        }
    }
}
