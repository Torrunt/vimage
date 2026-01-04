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
    public struct CustomAction
    {
        public string name;
        public string func;
    }

    [Serializable]
    public struct CustomActionBinding
    {
        public string name;
        public List<string> bindings;
    }

    [Serializable]
    public class Controls
    {
        public List<string> Drag = ["MOUSELEFT"];
        public List<string> Close = ["ESC", "BACKSPACE"];
        public List<string> OpenContextMenu = ["MOUSERIGHT"];
        public List<string> PrevImage = ["LEFT", "PAGE UP", "MOUSE4"];
        public List<string> NextImage = ["RIGHT", "PAGE DOWN", "MOUSE5"];
        public List<string> RotateClockwise = ["UP"];
        public List<string> RotateAntiClockwise = ["DOWN"];
        public List<string> Flip = ["F"];
        public List<string> FitToMonitorHeight = ["CTRL+MOUSEMIDDLE"];
        public List<string> FitToMonitorWidth = ["SHIFT+MOUSEMIDDLE"];
        public List<string> FitToMonitorAuto = ["MOUSEMIDDLE"];
        public List<string> FitToMonitorAlt = ["RSHIFT", "LSHIFT"];
        public List<string> ZoomIn = ["SCROLLUP"];
        public List<string> ZoomOut = ["SCROLLDOWN"];
        public List<string> ZoomFaster = ["RSHIFT", "LSHIFT"];
        public List<string> ZoomAlt = ["RCTRL", "CTRL"];
        public List<string> DragLimitToMonitorBounds = ["ALT"];
        public List<string> ToggleSmoothing = ["S"];
        public List<string> ToggleBackground = ["B"];
        public List<string> ToggleLock = [""];
        public List<string> ToggleAlwaysOnTop = ["L"];
        public List<string> ToggleTitleBar = [""];
        public List<string> PauseAnimation = ["SPACE"];
        public List<string> PrevFrame = ["<"];
        public List<string> NextFrame = [">"];
        public List<string> PlaybackSpeedIncrease = ["PLUS"];
        public List<string> PlaybackSpeedDecrease = ["MINUS"];
        public List<string> PlaybackSpeedReset = ["0"];
        public List<string> OpenSettings = [""];
        public List<string> ResetImage = ["R"];
        public List<string> OpenAtLocation = ["O"];
        public List<string> Delete = ["DELETE"];
        public List<string> Copy = ["CTRL+C"];
        public List<string> CopyAsImage = ["ALT+C"];
        public List<string> OpenDuplicateImage = ["C"];
        public List<string> OpenFullDuplicateImage = ["SHIFT+C"];
        public List<string> RandomImage = ["M"];
        public List<string> MoveLeft = ["CTRL+LEFT", "RCTRL+LEFT"];
        public List<string> MoveRight = ["CTRL+RIGHT", "RCTRL+RIGHT"];
        public List<string> MoveUp = ["CTRL+UP", "RCTRL+UP"];
        public List<string> MoveDown = ["CTRL+DOWN", "RCTRL+DOWN"];
        public List<string> TransparencyToggle = ["T"];
        public List<string> TransparencyInc = ["T+SCROLLDOWN"];
        public List<string> TransparencyDec = ["T+SCROLLUP"];
        public List<string> Crop = ["X"];
        public List<string> UndoCrop = ["CTRL+Z"];
        public List<string> ExitAll = ["SHIFT+ESC"];
        public List<string> RerenderSVG = ["SHIFT+R"];
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

        public Controls Controls = new();

        public List<ContextMenuItem> ContextMenu =
        [
            new ContextMenuItem { name = "Close", func = new FuncAction(Action.Close) },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
            new ContextMenuItem { name = "Next", func = new FuncAction(Action.NextImage) },
            new ContextMenuItem { name = "Previous", func = new FuncAction(Action.PrevImage) },
            new ContextMenuItem
            {
                name = "Sort by",
                children =
                [
                    new ContextMenuItem { name = "Name", func = new FuncAction(Action.SortName) },
                    new ContextMenuItem { name = "Date", func = new FuncAction(Action.SortDate) },
                    new ContextMenuItem
                    {
                        name = "Date modified",
                        func = new FuncAction(Action.SortDateModified),
                    },
                    new ContextMenuItem
                    {
                        name = "Date created",
                        func = new FuncAction(Action.SortDateCreated),
                    },
                    new ContextMenuItem { name = "Size", func = new FuncAction(Action.SortSize) },
                    new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
                    new ContextMenuItem
                    {
                        name = "Ascending",
                        func = new FuncAction(Action.SortAscending),
                    },
                    new ContextMenuItem
                    {
                        name = "Descending",
                        func = new FuncAction(Action.SortDescending),
                    },
                ],
            },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
            new ContextMenuItem
            {
                name = "Rotate right",
                func = new FuncAction(Action.RotateClockwise),
            },
            new ContextMenuItem
            {
                name = "Rotate left",
                func = new FuncAction(Action.RotateAntiClockwise),
            },
            new ContextMenuItem { name = "Flip", func = new FuncAction(Action.Flip) },
            new ContextMenuItem
            {
                name = "Fit to height",
                func = new FuncAction(Action.FitToMonitorHeight),
            },
            new ContextMenuItem
            {
                name = "Fit to width",
                func = new FuncAction(Action.FitToMonitorWidth),
            },
            new ContextMenuItem
            {
                name = "Smoothing",
                func = new FuncAction(Action.ToggleSmoothing),
            },
            new ContextMenuItem
            {
                name = "Always on top",
                func = new FuncAction(Action.ToggleAlwaysOnTop),
            },
            new ContextMenuItem { name = "Reset", func = new FuncAction(Action.ResetImage) },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
            new ContextMenuItem { name = "Edit", func = new FuncString("EDIT PAINT") },
            new ContextMenuItem { name = "Copy", func = new FuncAction(Action.Copy) },
            new ContextMenuItem { name = "Delete", func = new FuncAction(Action.Delete) },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
            new ContextMenuItem
            {
                name = "[filename.14]",
                func = new FuncAction(Action.OpenAtLocation),
            },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
            new ContextMenuItem { name = "Settings", func = new FuncAction(Action.OpenSettings) },
            new ContextMenuItem
            {
                name = "vimage [version]",
                func = new FuncAction(Action.VisitWebsite),
            },
        ];
        public List<ContextMenuItem> ContextMenu_Animation =
        [
            new ContextMenuItem
            {
                name = "Pause/Play",
                func = new FuncAction(Action.PauseAnimation),
            },
            new ContextMenuItem { name = "-", func = new FuncAction(Action.None) },
        ];

        public int ContextMenu_Animation_InsertAtIndex = 2;
        public bool ContextMenuShowMargin = false;
        public bool ContextMenuShowMarginSub = true;

        public List<CustomAction> CustomActions =
        [
            new CustomAction
            {
                name = "TOGGLE OVERLAY MODE",
                func = "-toggleSync -clickThrough -alwaysOnTop -defaultTransparency",
            },
            new CustomAction { name = "EDIT PAINT", func = @"mspaint.exe %f" },
            new CustomAction
            {
                name = "EDIT PAINTDOTNET",
                func = "\"C:\\Program Files\\Paint.NET\\PaintDotNet.exe\" %f",
            },
            new CustomAction { name = "TOGGLE TASKBAR", func = "-taskbarIcon" },
        ];

        public List<CustomActionBinding> CustomActionBindings =
        [
            new CustomActionBinding { name = "TOGGLE OVERLAY MODE", bindings = ["SHIFT+L"] },
            new CustomActionBinding { name = "EDIT PAINT", bindings = [] },
            new CustomActionBinding { name = "EDIT PAINTDOTNET", bindings = [] },
            new CustomActionBinding { name = "TOGGLE TASKBAR", bindings = [] },
        ];

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
