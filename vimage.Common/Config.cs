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
    public class ContextMenuItem(
        string name,
        ActionFunc? func = null,
        List<ContextMenuItem>? children = null
    )
    {
        public string Name = name;
        public ActionFunc? Func = func;
        public List<ContextMenuItem>? Children = children;
    }

    [Serializable]
    public class CustomActionItem(string name, string func)
    {
        public string Name = name;
        public string Func = func;
    }

    [Serializable]
    public class Config
    {
        public bool OpenAtMousePosition { get; set; } = true;
        public bool SmoothingDefault { get; set; } = true;
        public bool Mipmapping { get; set; } = true;
        public bool BackgroundForImagesWithTransparencyDefault { get; set; } = false;
        public string BackgroundColour { get; set; } = "#4D000000";
        public string TransparencyToggleValue { get; set; } = "#BEFFFFFF";
        public SizingOption ImageSizing { get; set; } = SizingOption.FitWidth;
        public LimitImagesToMonitorOption LimitImagesToMonitor { get; set; } =
            LimitImagesToMonitorOption.Auto;

        public bool PositionLargeWideImagesInCorner { get; set; } = true;
        public bool LoopImageNavigation { get; set; } = true;
        public bool PreloadNextImage { get; set; } = true;
        public bool ClearMemoryOnResetImage { get; set; } = true;
        public bool ShowTitleBar { get; set; } = false;
        public bool OpenSettingsEXE { get; set; } = true;
        public bool ListenForConfigChanges { get; set; } = true;
        public int MinImageSize { get; set; } = 64;
        public int SmoothingMinImageSize { get; set; } = 65;
        public int ZoomSpeed { get; set; } = 2;
        public int ZoomSpeedFast { get; set; } = 10;
        public int MoveSpeed { get; set; } = 2;
        public int MoveSpeedFast { get; set; } = 10;
        public int MaxTextures { get; set; } = 80;
        public int MaxAnimations { get; set; } = 8;
        public int MaxTextureSize { get; set; } = 10000;

        public int SettingsAppWidth { get; set; } = 600;
        public int SettingsAppHeight { get; set; } = 550;

        public SortBy DefaultSortBy { get; set; } = SortBy.Folder;
        public SortDirection DefaultSortDir { get; set; } = SortDirection.Folder;

        public string CropToolFillColour { get; set; } = "#78FFFFFF";
        public string CropToolOutlineColour { get; set; } = "#FF000000";
        public int CropToolOutlineThickness { get; set; } = 2;

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
            new ContextMenuItem("Close", new ActionEnum(Action.Close)),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
            new ContextMenuItem("Next", new ActionEnum(Action.NextImage)),
            new ContextMenuItem("Previous", new ActionEnum(Action.PrevImage)),
            new ContextMenuItem(
                "Sort by",
                children:
                [
                    new ContextMenuItem("Name", new ActionEnum(Action.SortName)),
                    new ContextMenuItem("Date", new ActionEnum(Action.SortDate)),
                    new ContextMenuItem("Date modified", new ActionEnum(Action.SortDateModified)),
                    new ContextMenuItem("Date created", new ActionEnum(Action.SortDateCreated)),
                    new ContextMenuItem("Size", new ActionEnum(Action.SortSize)),
                    new ContextMenuItem("-", new ActionEnum(Action.None)),
                    new ContextMenuItem("Ascending", new ActionEnum(Action.SortAscending)),
                    new ContextMenuItem("Descending", new ActionEnum(Action.SortDescending)),
                ]
            ),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
            new ContextMenuItem("Rotate right", new ActionEnum(Action.RotateClockwise)),
            new ContextMenuItem("Rotate left", new ActionEnum(Action.RotateAntiClockwise)),
            new ContextMenuItem("Flip", new ActionEnum(Action.Flip)),
            new ContextMenuItem("Fit to height", new ActionEnum(Action.FitToMonitorHeight)),
            new ContextMenuItem("Fit to width", new ActionEnum(Action.FitToMonitorWidth)),
            new ContextMenuItem("Smoothing", new ActionEnum(Action.ToggleSmoothing)),
            new ContextMenuItem("Always on top", new ActionEnum(Action.ToggleAlwaysOnTop)),
            new ContextMenuItem("Reset", new ActionEnum(Action.ResetImage)),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
            new ContextMenuItem("Edit", new CustomAction("EDIT PAINT")),
            new ContextMenuItem("Copy", new ActionEnum(Action.Copy)),
            new ContextMenuItem("Delete", new ActionEnum(Action.Delete)),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
            new ContextMenuItem("[filename.14]", new ActionEnum(Action.OpenAtLocation)),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
            new ContextMenuItem("Settings", new ActionEnum(Action.OpenSettings)),
            new ContextMenuItem("vimage [version]", new ActionEnum(Action.VisitWebsite)),
        ];
        public List<ContextMenuItem> ContextMenu_Animation =
        [
            new ContextMenuItem("Pause/Play", new ActionEnum(Action.PauseAnimation)),
            new ContextMenuItem("-", new ActionEnum(Action.None)),
        ];

        public int ContextMenu_Animation_InsertAtIndex { get; set; } = 2;
        public bool ContextMenuShowMargin { get; set; } = false;
        public bool ContextMenuShowMarginSub { get; set; } = true;

        public List<CustomActionItem> CustomActions =
        [
            new CustomActionItem("EDIT PAINT", @"mspaint.exe %f"),
            new CustomActionItem(
                "EDIT PAINTDOTNET",
                "\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\" %f"
            ),
            new CustomActionItem(
                "TOGGLE OVERLAY MODE",
                "-toggleSync -clickThrough -alwaysOnTop -defaultTransparency"
            ),
            new CustomActionItem("TOGGLE TASKBAR", "-taskbarIcon"),
        ];

        public Dictionary<string, List<string>> CustomActionBindings = new()
        {
            ["EDIT PAINT"] = [],
            ["EDIT PAINTDOTNET"] = [],
            ["TOGGLE OVERLAY MODE"] = ["SHIFT+L"],
            ["TOGGLE TASKBAR"] = ["SHIFT+G"],
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
