using System.Text.Json.Serialization;

namespace vimage.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortBy
    {
        Folder,
        Name,
        Date,
        DateModified,
        DateCreated,
        Size,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortDirection
    {
        Folder,
        Ascending,
        Descending,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SizingOption
    {
        FitWidth,
        FitHeight,
        FitAuto,
        KeepZoom,
        FullSize,
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LimitImagesToMonitorOption
    {
        None,
        Height,
        Width,
        Auto,
    }

    [Serializable]
    public struct ContextMenuItem
    {
        public string name;
        public ActionFunc? func;
        public List<ContextMenuItem>? children;
    }

    [Serializable]
    public struct CustomActionItem
    {
        public string name;
        public string func;
    }

    [Serializable]
    public class Config
    {
        public bool OpenAtMousePosition = true;
        public bool SmoothingDefault = true;
        public bool Mipmapping = true;
        public bool BackgroundForImagesWithTransparencyDefault = false;
        public string BackgroundColour = "#4D000000";
        public string TransparencyToggleValue = "#BEFFFFFF";
        public SizingOption ImageSizing = SizingOption.FitWidth;
        public LimitImagesToMonitorOption LimitImagesToMonitor = LimitImagesToMonitorOption.Auto;

        public bool PositionLargeWideImagesInCorner = true;
        public bool LoopImageNavigation = true;
        public bool PreloadNextImage = true;
        public bool ClearMemoryOnResetImage = true;
        public bool ShowTitleBar = false;
        public bool OpenSettingsEXE = true;
        public bool ListenForConfigChanges = true;
        public int MinImageSize = 64;
        public int SmoothingMinImageSize = 65;
        public int ZoomSpeed = 2;
        public int ZoomSpeedFast = 10;
        public int MoveSpeed = 2;
        public int MoveSpeedFast = 10;
        public int MaxTextures = 80;
        public int MaxAnimations = 8;
        public int MaxTextureSize = 10000;

        public int SettingsAppWidth = 600;
        public int SettingsAppHeight = 550;

        public SortBy DefaultSortBy = SortBy.Folder;
        public SortDirection DefaultSortDir = SortDirection.Folder;

        public string CropToolFillColour = "#78FFFFFF";
        public string CropToolOutlineColour = "#FF000000";
        public int CropToolOutlineThickness = 2;

        public Dictionary<Action, List<string>> Controls = new()
        {
            [Action.Drag] = ["MOUSELEFT"],
            [Action.Close] = ["ESC", "BACKSPACE"],
            [Action.OpenContextMenu] = ["MOUSERIGHT"],
            [Action.PrevImage] = ["LEFT", "PAGE UP", "MOUSE4"],
            [Action.NextImage] = ["RIGHT", "PAGE DOWN", "MOUSE5"],

            [Action.RotateClockwise] = ["UP"],
            [Action.RotateAntiClockwise] = ["DOWN"],
            [Action.Flip] = ["F"],
            [Action.FitToMonitorHeight] = ["CTRL+MOUSEMIDDLE"],
            [Action.FitToMonitorWidth] = ["SHIFT+MOUSEMIDDLE"],
            [Action.FitToMonitorAuto] = ["MOUSEMIDDLE"],
            [Action.FitToMonitorAlt] = ["RSHIFT", "LSHIFT"],
            [Action.ZoomIn] = ["SCROLLUP"],
            [Action.ZoomOut] = ["SCROLLDOWN"],
            [Action.ZoomFaster] = ["RSHIFT", "LSHIFT"],
            [Action.ZoomAlt] = ["RCTRL", "CTRL"],
            [Action.DragLimitToMonitorBounds] = ["ALT"],

            [Action.ToggleSmoothing] = ["S"],
            [Action.ToggleBackground] = ["B"],
            [Action.ToggleLock] = [],
            [Action.ToggleAlwaysOnTop] = ["L"],
            [Action.ToggleTitleBar] = [],

            [Action.PauseAnimation] = ["SPACE"],
            [Action.PrevFrame] = ["<"],
            [Action.NextFrame] = [">"],
            [Action.PlaybackSpeedIncrease] = ["PLUS"],
            [Action.PlaybackSpeedDecrease] = ["MINUS"],
            [Action.PlaybackSpeedReset] = ["0"],

            [Action.OpenSettings] = [""],
            [Action.ResetImage] = ["R"],
            [Action.OpenAtLocation] = ["O"],
            [Action.Delete] = ["DELETE"],
            [Action.Copy] = ["CTRL+C"],
            [Action.CopyAsImage] = ["ALT+C"],
            [Action.OpenDuplicateImage] = ["C"],
            [Action.OpenFullDuplicateImage] = ["SHIFT+C"],
            [Action.RandomImage] = ["M"],

            [Action.MoveLeft] = ["CTRL+LEFT", "RCTRL+LEFT"],
            [Action.MoveRight] = ["CTRL+RIGHT", "RCTRL+RIGHT"],
            [Action.MoveUp] = ["CTRL+UP", "RCTRL+UP"],
            [Action.MoveDown] = ["CTRL+DOWN", "RCTRL+DOWN"],

            [Action.TransparencyToggle] = ["T"],
            [Action.TransparencyInc] = ["T+SCROLLDOWN"],
            [Action.TransparencyDec] = ["T+SCROLLUP"],
            [Action.Crop] = ["X"],
            [Action.UndoCrop] = ["CTRL+Z"],
            [Action.ExitAll] = ["SHIFT+ESC"],
            [Action.RerenderSVG] = ["SHIFT+R"],

            [Action.VisitWebsite] = [],

            [Action.SortName] = [],
            [Action.SortDate] = [],
            [Action.SortDateModified] = [],
            [Action.SortDateCreated] = [],
            [Action.SortSize] = [],
            [Action.SortAscending] = [],
            [Action.SortDescending] = [],
        };

        public List<ContextMenuItem> ContextMenu =
        [
            new ContextMenuItem { name = "Close", func = new ActionEnum(Action.Close) },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
            new ContextMenuItem { name = "Next", func = new ActionEnum(Action.NextImage) },
            new ContextMenuItem { name = "Previous", func = new ActionEnum(Action.PrevImage) },
            new ContextMenuItem
            {
                name = "Sort by",
                children =
                [
                    new ContextMenuItem { name = "Name", func = new ActionEnum(Action.SortName) },
                    new ContextMenuItem { name = "Date", func = new ActionEnum(Action.SortDate) },
                    new ContextMenuItem
                    {
                        name = "Date modified",
                        func = new ActionEnum(Action.SortDateModified),
                    },
                    new ContextMenuItem
                    {
                        name = "Date created",
                        func = new ActionEnum(Action.SortDateCreated),
                    },
                    new ContextMenuItem { name = "Size", func = new ActionEnum(Action.SortSize) },
                    new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
                    new ContextMenuItem
                    {
                        name = "Ascending",
                        func = new ActionEnum(Action.SortAscending),
                    },
                    new ContextMenuItem
                    {
                        name = "Descending",
                        func = new ActionEnum(Action.SortDescending),
                    },
                ],
            },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
            new ContextMenuItem
            {
                name = "Rotate right",
                func = new ActionEnum(Action.RotateClockwise),
            },
            new ContextMenuItem
            {
                name = "Rotate left",
                func = new ActionEnum(Action.RotateAntiClockwise),
            },
            new ContextMenuItem { name = "Flip", func = new ActionEnum(Action.Flip) },
            new ContextMenuItem
            {
                name = "Fit to height",
                func = new ActionEnum(Action.FitToMonitorHeight),
            },
            new ContextMenuItem
            {
                name = "Fit to width",
                func = new ActionEnum(Action.FitToMonitorWidth),
            },
            new ContextMenuItem
            {
                name = "Smoothing",
                func = new ActionEnum(Action.ToggleSmoothing),
            },
            new ContextMenuItem
            {
                name = "Always on top",
                func = new ActionEnum(Action.ToggleAlwaysOnTop),
            },
            new ContextMenuItem { name = "Reset", func = new ActionEnum(Action.ResetImage) },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
            new ContextMenuItem { name = "Edit", func = new CustomAction("EDIT PAINT") },
            new ContextMenuItem { name = "Copy", func = new ActionEnum(Action.Copy) },
            new ContextMenuItem { name = "Delete", func = new ActionEnum(Action.Delete) },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
            new ContextMenuItem
            {
                name = "[filename.14]",
                func = new ActionEnum(Action.OpenAtLocation),
            },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
            new ContextMenuItem { name = "Settings", func = new ActionEnum(Action.OpenSettings) },
            new ContextMenuItem
            {
                name = "vimage [version]",
                func = new ActionEnum(Action.VisitWebsite),
            },
        ];
        public List<ContextMenuItem> ContextMenu_Animation =
        [
            new ContextMenuItem
            {
                name = "Pause/Play",
                func = new ActionEnum(Action.PauseAnimation),
            },
            new ContextMenuItem { name = "-", func = new ActionEnum(Action.None) },
        ];

        public int ContextMenu_Animation_InsertAtIndex = 2;
        public bool ContextMenuShowMargin = false;
        public bool ContextMenuShowMarginSub = true;

        public List<CustomActionItem> CustomActions =
        [
            new CustomActionItem
            {
                name = "TOGGLE OVERLAY MODE",
                func = "-toggleSync -clickThrough -alwaysOnTop -defaultTransparency",
            },
            new CustomActionItem { name = "EDIT PAINT", func = @"mspaint.exe %f" },
            new CustomActionItem
            {
                name = "EDIT PAINTDOTNET",
                func = "\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\" %f",
            },
            new CustomActionItem { name = "TOGGLE TASKBAR", func = "-taskbarIcon" },
        ];

        public Dictionary<string, List<string>> CustomActionBindings = new()
        {
            ["TOGGLE OVERLAY MODE"] = ["SHIFT+L"],
            ["EDIT PAINT"] = [],
            ["EDIT PAINTDOTNET"] = [],
            ["TOGGLE TASKBAR"] = [],
        };

        private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        /// <summary>
        /// Serialize Config to JSON and save to file
        /// </summary>
        public void Save(string path)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Loads and parses a config JSON file. If it doesn't exist, a default one will be made.
        /// </summary>
        public static Config Load(string path)
        {
            if (!File.Exists(path))
            {
                var cfg = new Config();
                File.WriteAllText(
                    path,
                    System.Text.Json.JsonSerializer.Serialize(cfg, JsonOptions)
                );
                return cfg;
            }

            return System.Text.Json.JsonSerializer.Deserialize<Config>(
                    File.ReadAllText(path),
                    JsonOptions
                ) ?? new Config();
        }
    }
}
