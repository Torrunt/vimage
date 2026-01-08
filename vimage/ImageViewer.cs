using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using vimage.Common;
using vimage.Display;
using vimage.Utils;
using Action = vimage.Common.Action;

namespace vimage
{
    internal class ImageViewer
    {
        public static string VERSION_NO =
            Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion?.Split('+')[0] ?? "#";

        public readonly float ZOOM_MIN = 0.05f;
        public readonly float ZOOM_MAX = 75f;

        public RenderWindow Window;
        public SFML.ObjectBase? Image;
        public string File = "";
        public List<string> FolderContents = [];
        public int FolderPosition = 0;
        private readonly ContextMenu? ContextMenu;
        public Color ImageColor = Color.White;
        public Vector2u Size = new();
        public int Rotation = 0;
        public List<ViewState> ViewStateHistory = [];
        private Action LastAction = Action.None;

        public Config Config;
        public Controls Controls;
        private readonly FileSystemWatcher? ConfigFileWatcher;
        private bool ReloadConfigNextTick = false;

        private bool Updated = false;
        public bool CloseNextTick = false;

        /// <summary>
        /// Instead of setting the Window Position directly when the image is going to be Updated, this is set.
        /// This prevents the old image being shown at the new image location for a split-second before the new image is loaded.
        /// </summary>
        private Vector2i NextWindowPos = new();
        private Vector2u NextWindowSize = new();
        private bool Dragging = false;
        private Vector2i DragPos = new();
        private Vector2i MousePos = new();
        private bool DragLimitToBoundsMod = false;
        private bool ZoomAlt = false;
        private bool ZoomFaster = false;
        private float CurrentZoom = 1;
        private bool AutomaticallyZoomed = false;
        private int DefaultRotation = 0;
        public bool FlippedX = false;
        public bool FitToMonitorHeight = false;
        public bool FitToMonitorWidth = false;
        private bool FitToMonitorHeightForced = false;

        /// <summary>If true will resize based on working area instead of bounds (ie: screen area minus task bar).</summary>
        private bool FitToMonitorAlt = false;
        public bool BackgroundsForImagesWithTransparency = false;
        public Color BackgroundColour = new(230, 230, 230);
        private bool Cropping = false;
        private RectangleShape? CropRect;
        private Vector2i CropStartPos = new();
        public bool Locked = false;
        public bool AlwaysOnTop = false;
        private bool AlwaysOnTopForced = false;

        /// <summary>
        /// If true will turn AlwaysOnTop mode on next update if the window height >= monitor height and window width is less than monitor width.
        /// If the window is wider and taller than the monitor it will automatically be above the task bar anyway.
        /// </summary>
        private bool ForceAlwaysOnTopNextTick = false;
        private bool ClickThroughAble = false;
        private bool ShowTitleBar = false;

        /// <summary>0=false, 1=next, -1=prev.</summary>
        private int PreloadingNextImage = 0;
        private bool PreloadNextImageStart = false;
        private bool PreloadingImage = false;
        public SortBy SortImagesBy = SortBy.Name;
        public SortDirection SortImagesByDir = SortDirection.Ascending;

        /// <summary>Bitmap of image loaded in via Clipboard (used to copy it back into clipboard).</summary>
        private System.Drawing.Bitmap? ClipboardBitmap;

        private static readonly Random rnd = new();

        public ImageViewer(string file, string[] args)
        {
            // Save Mouse Position -> will open image at this position
            var mousePos = Mouse.GetPosition();

            // Create Window
            Window = new RenderWindow(new VideoMode(0, 0), File + " - vimage", Styles.None)
            {
                Position = mousePos,
            };

            // Make Window Transparent (can only tell if image being viewed has transparency)
            var bb = new DWM_BLURBEHIND
            {
                dwFlags = DWM_BB.Enable | DWM_BB.BlurRegion,
                fEnable = true,
                hRgnBlur = DWM.CreateRectRgn(0, 0, -1, -1),
            };
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

            // Load Config File
            try
            {
                Config = Config.Load(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")
                );
            }
            catch (UnauthorizedAccessException)
            {
                System.Windows.Forms.MessageBox.Show(
                    "vimage does not have write permissions for the folder it's located in.\nPlease place it somewhere else (or set it to run as admin).",
                    "vimage - Error"
                );
                Config = new Config();
            }
            Controls = new Controls(Config);

            if (Config.ListenForConfigChanges)
            {
                ConfigFileWatcher = new FileSystemWatcher(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "config.json"
                )
                {
                    NotifyFilter = NotifyFilters.LastWrite,
                };
                ConfigFileWatcher.Changed += new FileSystemEventHandler(OnConfigChanged);
                ConfigFileWatcher.EnableRaisingEvents = true;
            }
            BackgroundsForImagesWithTransparency =
                Config.BackgroundForImagesWithTransparencyDefault;
            var backColour = System.Drawing.ColorTranslator.FromHtml(Config.BackgroundColour);
            BackgroundColour = new Color(backColour.R, backColour.G, backColour.B, backColour.A);
            Graphics.MaxTextures = (uint)Config.MaxTextures;
            Graphics.MaxAnimations = (uint)Config.MaxAnimations;
            Graphics.TextureMaxSize = Math.Min(Graphics.TextureMaxSize, 8192);
            ShowTitleBar = Config.ShowTitleBar;
            if (ShowTitleBar)
                DWM.TitleBarSetVisible(Window, true);

            // Get Image
            _ = ChangeImage(file);

            if (Image == null)
            {
                Window.Close();
                return;
            }

            // Position window at mouse position?
            var winPos = mousePos;
            var bounds = ImageViewerUtils.GetCurrentBounds(winPos);
            if (
                Config.PositionLargeWideImagesInCorner
                && CurrentImageSize().X > CurrentImageSize().Y
                && CurrentImageSize().X * CurrentZoom >= bounds.Width
            )
            {
                winPos = new Vector2i(bounds.Left, bounds.Top);
            }
            else if (Config.OpenAtMousePosition)
            {
                // At Mouse Position
                winPos = new Vector2i(
                    mousePos.X - (int)(NextWindowSize.X / 2),
                    mousePos.Y - (int)(NextWindowSize.Y / 2)
                );
                if (!FitToMonitorHeightForced)
                {
                    if (winPos.Y < bounds.Top)
                        winPos.Y = 0;
                    else if (winPos.Y + NextWindowSize.Y > bounds.Height)
                        winPos.Y = bounds.Height - (int)NextWindowSize.Y;
                }
                else
                    winPos.Y = bounds.Top;

                if (winPos.X < bounds.Left)
                    winPos.X = bounds.Left;
                else if (winPos.X + NextWindowSize.X > bounds.Left + bounds.Width)
                    winPos.X = bounds.Left + bounds.Width - (int)NextWindowSize.X;
            }
            else
            {
                winPos = new Vector2i(
                    bounds.Left + (int)((bounds.Width - (Size.X * CurrentZoom)) / 2),
                    bounds.Top + (int)((bounds.Height - (Size.Y * CurrentZoom)) / 2)
                );
            }
            NextWindowPos = winPos;

            // Arguments?
            if (args.Length > 1)
                ApplyArguments(args, true);

            // Display Window
            Window.Size = NextWindowSize;
            Window.Position = NextWindowPos;
            Redraw();
            Updated = false;
            _ = Window.SetActive();
            ViewStateHistory.Clear();

            // Get/Set Folder Sorting
            // (threaded to avoid potential hang when having to check Windows for current folder sorting)
            Task.Run(() =>
            {
                var (sortBy, sortDir) = WindowsFileSorting.GetSorting(
                    Config.DefaultSortBy,
                    Config.DefaultSortDir,
                    file
                );
                SortImagesBy = sortBy;
                SortImagesByDir = sortDir;
            });

            // Create Context Menu
            ContextMenu = new ContextMenu(this);
            ContextMenu.LoadItems(
                Config.ContextMenu,
                Config.ContextMenu_Animation,
                Config.ContextMenu_Animation_InsertAtIndex
            );
            ContextMenu.Setup(true);

            // Interaction
            Window.Closed += OnWindowClosed;
            Window.MouseButtonPressed += OnMouseDown;
            Window.MouseButtonReleased += OnMouseUp;
            Window.MouseWheelScrolled += OnMouseWheelScrolled;
            Window.MouseMoved += OnMouseMoved;
            Window.KeyReleased += OnKeyUp;
            Window.KeyPressed += OnKeyDown;

            // Loop
            var clock = new Stopwatch();
            clock.Start();

            bool doRedraw;

            while (Window.IsOpen)
            {
                // Add in some idle time to not thrash the CPU
                Thread.Sleep(1);

                if (CloseNextTick)
                {
                    Window.Close();
                    break;
                }

                // Process events
                Window.DispatchEvents();

                if (ReloadConfigNextTick)
                {
                    ReloadConfig();
                    ReloadConfigNextTick = false;
                }

                doRedraw = false;

                // Animated Image?
                if (Image is AnimatedImage animatedImage)
                {
                    var imageUpdated = animatedImage.Update((float)clock.Elapsed.TotalMilliseconds);
                    if (!Updated && imageUpdated)
                        doRedraw = true;
                }
                clock.Restart();

                // Drag Window
                if (Dragging)
                {
                    NextWindowPos = new Vector2i(
                        Mouse.GetPosition().X - DragPos.X,
                        Mouse.GetPosition().Y - DragPos.Y
                    );
                    if (DragLimitToBoundsMod)
                    {
                        // limit to monitor bounds
                        var currentBounds = ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());
                        var left = currentBounds.Left;
                        var top = currentBounds.Top;
                        var right = left + currentBounds.Width - (int)Window.Size.X;
                        var bottom = currentBounds.Top + currentBounds.Height - (int)Window.Size.Y;

                        if (Window.Size.X > currentBounds.Width)
                        {
                            if (NextWindowPos.X > left)
                                NextWindowPos.X = left;
                            else if (NextWindowPos.X < right)
                                NextWindowPos.X = right;
                        }
                        else if (NextWindowPos.X < left)
                            NextWindowPos.X = left;
                        else if (NextWindowPos.X > right)
                            NextWindowPos.X = right;

                        if (Window.Size.Y > currentBounds.Height)
                        {
                            if (NextWindowPos.Y > top)
                                NextWindowPos.Y = top;
                            else if (NextWindowPos.Y < bottom)
                                NextWindowPos.Y = bottom;
                        }
                        else if (NextWindowPos.Y < top)
                            NextWindowPos.Y = top;
                        else if (NextWindowPos.Y > bottom)
                            NextWindowPos.Y = bottom;
                    }
                    Window.Position = ShowTitleBar
                        ? NextWindowPos - DWM.GetTitleBarDifference(Window.SystemHandle)
                        : NextWindowPos;

                    doRedraw = true;
                }
                else if (Cropping)
                {
                    MousePos = Mouse.GetPosition(Window);
                    if (MousePos.X < 0)
                        MousePos.X = 0;
                    else if (MousePos.X > Window.Size.X)
                        MousePos.X = (int)Window.Size.X;
                    if (MousePos.Y < 0)
                        MousePos.Y = 0;
                    else if (MousePos.Y > Window.Size.Y)
                        MousePos.Y = (int)Window.Size.Y;
                    var m = Window.MapPixelToCoords(MousePos);
                    CropRect?.Size = new Vector2f(
                        m.X - CropRect.Position.X,
                        m.Y - CropRect.Position.Y
                    );

                    doRedraw = true;
                }

                // Update
                if (Updated)
                {
                    Updated = false;
                    Window.Size = NextWindowSize;
                    Window.Position = NextWindowPos;
                    doRedraw = true;
                }

                // Redraw
                if (doRedraw)
                    Redraw();

                if (ForceAlwaysOnTopNextTick)
                    ForceAlwaysOnTop();

                if (PreloadNextImageStart)
                    PreloadNextImage();
            }
        }

        private void Redraw()
        {
            // Clear screen
            if (!BackgroundsForImagesWithTransparency && !ShowTitleBar)
            {
                Window.Clear(new Color(0, 0, 0, 0));
            }
            else
            {
                Window.Clear(
                    ShowTitleBar
                        ? new Color(BackgroundColour.R, BackgroundColour.G, BackgroundColour.B)
                        : BackgroundColour
                );
            }
            // Draw Image
            if (Image is Drawable drawable)
                Window.Draw(drawable);
            // Draw Other
            if (Cropping && CropRect != null)
                Window.Draw(CropRect);
            // Update the window
            Window.Display();
        }

        private void OnWindowClosed(object? sender, EventArgs e)
        {
            Window.Close();
        }

        private void Update()
        {
            Window.Clear(new Color(0, 0, 0, 0));
            Window.Display();
            Window.Position = NextWindowPos;
            Window.Size = NextWindowSize;
            Redraw();
        }

        ////////////////////////
        //      Actions      //
        ///////////////////////

        public void DoAction(Action action)
        {
            switch (action)
            {
                case Action.Close:
                    CloseNextTick = true;
                    return;
                case Action.OpenContextMenu:
                    OpenContextMenu();
                    return;
                case Action.PrevImage:
                    PrevImage();
                    return;
                case Action.NextImage:
                    NextImage();
                    return;

                case Action.RotateClockwise:
                    RotateImage(Rotation + 90);
                    return;
                case Action.RotateAntiClockwise:
                    RotateImage(Rotation - 90);
                    return;
                case Action.Flip:
                    _ = FlipImage();
                    return;
                case Action.FitToMonitorHeight:
                    ToggleFitToMonitor(LimitImagesToMonitorOption.Height);
                    return;
                case Action.FitToMonitorWidth:
                    ToggleFitToMonitor(LimitImagesToMonitorOption.Width);
                    return;
                case Action.FitToMonitorAuto:
                    ToggleFitToMonitor(LimitImagesToMonitorOption.Auto);
                    return;
                case Action.ZoomIn:
                    Zoom(
                        Math.Min(
                            CurrentZoom
                                + (
                                    ZoomFaster
                                        ? (Config.ZoomSpeedFast / 100f)
                                        : (Config.ZoomSpeed / 100f)
                                ),
                            ZOOM_MAX
                        ),
                        !ZoomAlt,
                        true
                    );
                    return;
                case Action.ZoomOut:
                    Zoom(
                        Math.Max(
                            CurrentZoom
                                - (
                                    ZoomFaster
                                        ? (Config.ZoomSpeedFast / 100f)
                                        : (Config.ZoomSpeed / 100f)
                                ),
                            ZOOM_MIN
                        ),
                        !ZoomAlt,
                        true
                    );
                    return;

                case Action.ToggleSmoothing:
                    _ = ToggleSmoothing();
                    return;
                case Action.ToggleBackground:
                    _ = ToggleBackground();
                    return;
                case Action.TransparencyToggle:
                    _ = ToggleImageTransparency();
                    return;
                case Action.ToggleLock:
                    _ = ToggleLock();
                    return;
                case Action.ToggleAlwaysOnTop:
                    _ = ToggleAlwaysOnTop();
                    return;
                case Action.ToggleTitleBar:
                    _ = ToggleTitleBar();
                    return;

                case Action.NextFrame:
                    NextFrame();
                    return;
                case Action.PrevFrame:
                    PrevFrame();
                    return;
                case Action.PauseAnimation:
                    _ = ToggleAnimation();
                    return;
                case Action.PlaybackSpeedIncrease:
                    AdjustPlaybackSpeed(0.1f);
                    return;
                case Action.PlaybackSpeedDecrease:
                    AdjustPlaybackSpeed(-0.1f);
                    return;
                case Action.PlaybackSpeedReset:
                    ResetPlaybackSpeed();
                    return;

                case Action.OpenSettings:
                    OpenConfig();
                    return;
                case Action.ResetImage:
                    ResetImage();
                    return;
                case Action.OpenAtLocation:
                    OpenFileAtLocation();
                    return;
                case Action.Delete:
                    DeleteFile();
                    return;
                case Action.Copy:
                    CopyFile();
                    return;
                case Action.CopyAsImage:
                    CopyAsImage();
                    return;
                case Action.OpenDuplicateImage:
                    OpenDuplicateWindow();
                    return;
                case Action.OpenFullDuplicateImage:
                    OpenDuplicateWindow(true);
                    return;
                case Action.RandomImage:
                    RandomImage();
                    return;

                case Action.MoveLeft:
                    if (Dragging)
                        return;
                    NextWindowPos.X -= ZoomFaster ? Config.MoveSpeedFast : Config.MoveSpeed;
                    Window.Position = NextWindowPos;
                    return;
                case Action.MoveRight:
                    if (Dragging)
                        return;
                    NextWindowPos.X += ZoomFaster ? Config.MoveSpeedFast : Config.MoveSpeed;
                    Window.Position = NextWindowPos;
                    return;
                case Action.MoveUp:
                    if (Dragging)
                        return;
                    NextWindowPos.Y -= ZoomFaster ? Config.MoveSpeedFast : Config.MoveSpeed;
                    Window.Position = NextWindowPos;
                    return;
                case Action.MoveDown:
                    if (Dragging)
                        return;
                    NextWindowPos.Y += ZoomFaster ? Config.MoveSpeedFast : Config.MoveSpeed;
                    Window.Position = NextWindowPos;
                    return;

                case Action.TransparencyIncrease:
                    AdjustImageTransparency(-1);
                    return;
                case Action.TransparencyDecrease:
                    AdjustImageTransparency(1);
                    return;

                case Action.UndoCrop:
                    UndoCrop();
                    return;
                case Action.ExitAll:
                    ExitAllInstances();
                    return;
                case Action.RerenderSVG:
                    RenderSVGAtCurrentZoom();
                    return;

                case Action.VisitWebsite:
                    _ = Process.Start(
                        new ProcessStartInfo
                        {
                            FileName = "http://torrunt.net/vimage",
                            UseShellExecute = true,
                        }
                    );
                    return;

                case Action.SortName:
                    ChangeSortBy(SortBy.Name);
                    return;
                case Action.SortDate:
                    ChangeSortBy(SortBy.Date);
                    return;
                case Action.SortDateModified:
                    ChangeSortBy(SortBy.DateModified);
                    return;
                case Action.SortDateCreated:
                    ChangeSortBy(SortBy.DateCreated);
                    return;
                case Action.SortSize:
                    ChangeSortBy(SortBy.Size);
                    return;
                case Action.SortAscending:
                    ChangeSortByDirection(SortDirection.Ascending);
                    return;
                case Action.SortDescending:
                    ChangeSortByDirection(SortDirection.Descending);
                    return;
            }
        }

        ////////////////////////
        //      Controls     //
        ///////////////////////

        private void OnMouseMoved(object? sender, MouseMoveEventArgs e)
        {
            MousePos = new Vector2i(e.X, e.Y);

            if (Dragging)
                UnforceAlwaysOnTop();
        }

        private void OnMouseWheelScrolled(object? sender, MouseWheelScrollEventArgs e)
        {
            if (Locked)
                return;

            var dir = e.Delta > 0 ? MouseWheel.ScrollUp : MouseWheel.ScrollDown;

            var (a, isKeyCombo) = Controls.GetActionFromInput(new MouseWheelInput(dir));
            if (a == null)
                return;

            if (a is CustomAction customAction)
            {
                DoCustomAction(customAction.Value);
                return;
            }
            if (a is not ActionEnum action)
                return;

            DoAction(action.Value);
            LastAction = isKeyCombo ? action.Value : Action.None;
        }

        private void OnMouseDown(object? sender, MouseButtonEventArgs e)
        {
            ControlDown(new MouseInput(e.Button));
        }

        private void OnMouseUp(object? sender, MouseButtonEventArgs e)
        {
            ControlUp(new MouseInput(e.Button));
            LastAction = Action.None;
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            ControlDown(new KeyInput(e.Code));
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            ControlUp(new KeyInput(e.Code));
            LastAction = Action.None;
        }

        private void ControlUp(ControlInput value)
        {
            var modifiers = Controls.GetModifierActionsFromInput(value);
            if (modifiers.Count > 0)
            {
                foreach (var m in modifiers)
                {
                    switch (m)
                    {
                        case Action.Drag:
                            Dragging = false;
                            break;
                        case Action.ZoomFaster:
                            ZoomFaster = false;
                            break;
                        case Action.ZoomAlt:
                            ZoomAlt = false;
                            break;
                        case Action.DragLimitToMonitorBounds:
                            DragLimitToBoundsMod = false;
                            break;
                        case Action.FitToMonitorAlt:
                            FitToMonitorAlt = false;
                            break;
                        case Action.Crop:
                            if (Cropping)
                                CropEnd();
                            break;
                    }
                }
            }

            var (a, _) = Controls.GetActionFromInput(value);
            if (a == null)
                return;

            // Prevent action from triggering on control up if an action just happened on control down
            if (LastAction != Action.None)
                return;

            if (a is CustomAction customAction)
            {
                DoCustomAction(customAction.Value);
                return;
            }
            if (a is not ActionEnum action)
                return;

            // Prevent action if locked (unless unlocking or opening context menu)
            if (
                Locked
                && !(action.Value == Action.ToggleLock || action.Value == Action.OpenContextMenu)
            )
                return;

            DoAction(action.Value);
        }

        private void ControlDown(ControlInput value)
        {
            if (Locked)
                return;

            var modifiers = Controls.GetModifierActionsFromInput(value);
            if (modifiers.Count > 0)
            {
                foreach (var m in modifiers)
                {
                    switch (m)
                    {
                        case Action.Drag:
                            if (!Dragging)
                                DragPos = MousePos;
                            Dragging = true;
                            break;
                        case Action.ZoomFaster:
                            ZoomFaster = true;
                            break;
                        case Action.ZoomAlt:
                            ZoomAlt = true;
                            break;
                        case Action.DragLimitToMonitorBounds:
                            DragLimitToBoundsMod = true;
                            break;
                        case Action.FitToMonitorAlt:
                            FitToMonitorAlt = true;
                            break;
                        case Action.Crop:
                            if (Cropping)
                                break;
                            CropStart();
                            LastAction = Action.Crop;
                            break;
                    }
                }
                return;
            }

            var (a, _) = Controls.GetActionFromInput(value, true);
            if (a == null)
                return;

            if (a is not ActionEnum action)
                return;

            DoAction(action.Value);
            LastAction = action.Value;
        }

        ///////////////////////////
        //      Manipulation     //
        ///////////////////////////

        public void NextFrame()
        {
            if (Image is not AnimatedImage animatedImage)
                return;

            if (animatedImage.Playing)
                animatedImage.Stop();
            animatedImage.NextFrame();
            Update();
        }

        public void PrevFrame()
        {
            if (Image is not AnimatedImage animatedImage)
                return;

            if (animatedImage.Playing)
                animatedImage.Stop();
            animatedImage.PrevFrame();
            Update();
        }

        public bool ToggleAnimation(int val = -1)
        {
            if (Image is not AnimatedImage animatedImage)
                return false;

            if ((val == -1 && animatedImage.Playing) || val == 0)
                animatedImage.Stop();
            else if (val != 0)
                animatedImage.Play();

            return animatedImage.Playing;
        }

        public void AdjustPlaybackSpeed(float increment = 1)
        {
            if (Image is not AnimatedImage animatedImage)
                return;
            animatedImage.SpeedMultiplier = Math.Max(
                0.1f,
                (float)Math.Round(animatedImage.SpeedMultiplier + increment, 2)
            );
        }

        public void ResetPlaybackSpeed()
        {
            if (Image is not AnimatedImage animatedImage)
                return;
            animatedImage.SpeedMultiplier = 1;
        }

        private void Zoom(float value, bool center = false, bool manualZoom = false)
        {
            // Prevent zooming while cropping
            if (Cropping)
                return;

            // Limit zooming to prevent the going past the GPU's max texture size
            if (value > CurrentZoom && (uint)Math.Ceiling(Size.X * value) >= Texture.MaximumSize)
                value = CurrentZoom;

            var currentBounds = new IntRect();
            if (DragLimitToBoundsMod)
            {
                currentBounds = ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());
                if (
                    value >= CurrentZoom
                    && (
                        Window.Size.X >= currentBounds.Width
                        || Window.Size.Y >= currentBounds.Height
                    )
                )
                    return;
            }

            float originalZoom = CurrentZoom;
            CurrentZoom = value;

            Dragging = false;
            UnforceAlwaysOnTop();

            if (
                ShowTitleBar
                && CurrentZoom <= originalZoom
                && CurrentImageSize().X * CurrentZoom < 130
            )
                CurrentZoom = Math.Max(130f / CurrentImageSize().X, CurrentZoom); // limit zoom if title bar is on

            if (center)
            {
                var newSize =
                    Rotation == 0 || Rotation == 180
                        ? new Vector2u(
                            (uint)Math.Ceiling(Size.X * CurrentZoom),
                            (uint)Math.Ceiling(Size.Y * CurrentZoom)
                        )
                        : new Vector2u(
                            (uint)Math.Ceiling(Size.Y * CurrentZoom),
                            (uint)Math.Ceiling(Size.X * CurrentZoom)
                        );
                var difference =
                    new Vector2i((int)newSize.X, (int)newSize.Y)
                    - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                NextWindowSize = newSize;
                NextWindowPos = new Vector2i(
                    Window.Position.X - (difference.X / 2),
                    Window.Position.Y - (difference.Y / 2)
                );
            }
            else
            {
                NextWindowSize =
                    Rotation == 0 || Rotation == 180
                        ? new Vector2u(
                            (uint)Math.Ceiling(Size.X * CurrentZoom),
                            (uint)Math.Ceiling(Size.Y * CurrentZoom)
                        )
                        : new Vector2u(
                            (uint)Math.Ceiling(Size.Y * CurrentZoom),
                            (uint)Math.Ceiling(Size.X * CurrentZoom)
                        );
                NextWindowPos = Window.Position;
            }

            if (DragLimitToBoundsMod)
            {
                // limit to monitor bounds
                if (
                    NextWindowSize.X > currentBounds.Width
                    || NextWindowSize.Y > currentBounds.Height
                )
                {
                    // recalculate zoom size
                    float monitorRatio = (float)currentBounds.Width / currentBounds.Height;
                    float windowRatio = (float)NextWindowSize.X / NextWindowSize.Y;

                    if (windowRatio <= monitorRatio)
                    {
                        // limit to monitor height
                        float r = (float)currentBounds.Height / NextWindowSize.Y;
                        NextWindowSize = new Vector2u(
                            (uint)(NextWindowSize.X * r),
                            (uint)currentBounds.Height
                        );
                    }
                    else
                    {
                        // limit to monitor width
                        float r = (float)currentBounds.Width / NextWindowSize.X;
                        NextWindowSize = new Vector2u(
                            (uint)currentBounds.Width,
                            (uint)(NextWindowSize.Y * r)
                        );
                    }
                    CurrentZoom =
                        (float)NextWindowSize.X
                        / ((Rotation == 0 || Rotation == 180) ? Size.X : Size.Y);

                    if (center && CurrentZoom != originalZoom)
                    {
                        var difference =
                            new Vector2i((int)NextWindowSize.X, (int)NextWindowSize.Y)
                            - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                        NextWindowPos = new Vector2i(
                            Window.Position.X - (difference.X / 2),
                            Window.Position.Y - (difference.Y / 2)
                        );
                    }
                    else
                        NextWindowPos = Window.Position;
                }

                NextWindowPos = ImageViewerUtils.LimitToBounds(
                    NextWindowPos,
                    NextWindowSize,
                    currentBounds
                );
            }

            if (manualZoom)
            {
                AutomaticallyZoomed = false;
                FitToMonitorHeightForced = false;
                FitToMonitorHeight = false;
                FitToMonitorWidth = false;
            }

            Updated = true;
        }

        public void RotateImage(
            int rotation,
            bool aroundCenter = true,
            bool updateWindowSize = true
        )
        {
            if (rotation >= 360)
                rotation = 0;
            else if (rotation < 0)
                rotation = 270;
            Rotation = rotation;

            var center = new Vector2f(
                Window.Position.X + (Window.Size.X / 2),
                Window.Position.Y + (Window.Size.Y / 2)
            );
            Vector2u WindowSize;

            UnforceAlwaysOnTop();

            var view = Window.GetView();
            view.Rotation = -Rotation;
            if (Rotation == 90 || Rotation == 270)
            {
                WindowSize = new Vector2u(
                    (uint)(Size.Y * CurrentZoom),
                    (uint)(Size.X * CurrentZoom)
                );
                view.Size = new Vector2f(Size.Y, Size.X * (FlippedX ? -1 : 1));
            }
            else
            {
                WindowSize = new Vector2u(
                    (uint)(Size.X * CurrentZoom),
                    (uint)(Size.Y * CurrentZoom)
                );
                view.Size = new Vector2f(Size.X * (FlippedX ? -1 : 1), Size.Y);
            }
            Window.SetView(view);

            if (updateWindowSize)
                NextWindowSize = WindowSize;
            NextWindowPos = aroundCenter
                ? new Vector2i(
                    (int)center.X - (int)(WindowSize.X / 2),
                    (int)center.Y - (int)(WindowSize.Y / 2)
                )
                : Window.Position;

            if (ShowTitleBar && updateWindowSize)
                Zoom(CurrentZoom, true); // make sure image is not too thin if title bar is on

            Updated = true;
        }

        public Vector2u CurrentImageSize()
        {
            return (Rotation == 0 || Rotation == 180) ? Size : new Vector2u(Size.Y, Size.X);
        }

        public bool FlipImage(int val = -1)
        {
            FlippedX = val == -1 ? !FlippedX : val == 1;
            var view = Window.GetView();
            view.Size = new Vector2f(
                Rotation == 90 || Rotation == 270
                    ? view.Size.X
                    : Math.Abs(view.Size.X) * (FlippedX ? -1 : 1),
                Rotation == 90 || Rotation == 270
                    ? Math.Abs(view.Size.Y) * (FlippedX ? -1 : 1)
                    : view.Size.Y
            );
            Window.SetView(view);
            Redraw();

            return FlippedX;
        }

        public void ToggleFitToMonitor(LimitImagesToMonitorOption dimension)
        {
            if (Cropping)
                return;

            UnforceAlwaysOnTop();

            IntRect bounds;
            var workingArea = ImageViewerUtils.GetCurrentWorkingArea(Mouse.GetPosition());
            bounds = FitToMonitorAlt
                ? workingArea
                : ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());

            if (dimension == LimitImagesToMonitorOption.Auto)
            {
                dimension =
                    bounds.Height < bounds.Width
                        ? LimitImagesToMonitorOption.Height
                        : LimitImagesToMonitorOption.Width;
            }

            bool center = false;
            if (
                CurrentZoom == 1
                || (FitToMonitorHeight && dimension != LimitImagesToMonitorOption.Height)
                || (FitToMonitorWidth && dimension != LimitImagesToMonitorOption.Width)
            )
            {
                // Fit to Monitor Height/Width
                if (dimension == LimitImagesToMonitorOption.Height)
                {
                    FitToMonitorWidth = false;
                    FitToMonitorHeight = true;
                    if (Rotation == 90 || Rotation == 270)
                        Zoom((float)bounds.Height / Size.X, Size.Y < bounds.Width);
                    else
                        Zoom((float)bounds.Height / Size.Y, Size.X < bounds.Width);
                    NextWindowPos =
                        NextWindowSize.X >= NextWindowSize.Y && bounds.Width > bounds.Height
                            ? ImageViewerUtils.LimitToBounds(NextWindowPos, NextWindowSize, bounds)
                            : new Vector2i(NextWindowPos.X, bounds.Top);
                }
                else if (dimension == LimitImagesToMonitorOption.Width)
                {
                    FitToMonitorWidth = true;
                    FitToMonitorHeight = false;
                    if (Rotation == 90 || Rotation == 270)
                        Zoom((float)bounds.Width / Size.Y, true);
                    else
                        Zoom((float)bounds.Width / Size.X, true);
                    NextWindowPos =
                        NextWindowSize.Y >= NextWindowSize.X && bounds.Height > bounds.Width
                            ? ImageViewerUtils.LimitToBounds(NextWindowPos, NextWindowSize, bounds)
                            : new Vector2i(bounds.Left, NextWindowPos.Y);
                }
            }
            else
            {
                // Full Size
                Zoom(1, true);
                if (FitToMonitorWidth && bounds.Width > bounds.Height)
                    center = true; // center image if returning to normal size after FitToMonitorWidth (landscape monitor only)
                else
                    NextWindowPos = ImageViewerUtils.LimitToBounds(
                        NextWindowPos,
                        NextWindowSize,
                        bounds
                    );

                FitToMonitorHeight = false;
                FitToMonitorWidth = false;
            }

            // Position window
            if (
                Config.PositionLargeWideImagesInCorner
                && NextWindowSize.X >= bounds.Width
                && bounds.Width > bounds.Height
            )
                NextWindowPos = new Vector2i(bounds.Left, bounds.Top); // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            else if (center || CurrentImageSize().X >= bounds.Width) // Position Window at center if originally large
                NextWindowPos = new Vector2i(
                    NextWindowSize.X >= bounds.Width - 2
                        ? bounds.Left
                        : bounds.Left
                            + (bounds.Width / 2)
                            - ((int)(CurrentImageSize().X * CurrentZoom) / 2),
                    NextWindowSize.Y >= bounds.Height - 2
                        ? bounds.Top
                        : bounds.Top
                            + (bounds.Height / 2)
                            - ((int)(CurrentImageSize().Y * CurrentZoom) / 2)
                );

            if (ShowTitleBar)
                NextWindowPos -= DWM.GetTitleBarDifference(Window.SystemHandle);

            // Temporarily set always on top to bring it infront of the taskbar?
            if (!FitToMonitorAlt)
                ForceAlwaysOnTopCheck(bounds, workingArea);

            AutomaticallyZoomed = false;
        }

        public void ResetImage()
        {
            // Reset size / crops
            ResetSize();

            Window.SetView(
                new View(Window.DefaultView)
                {
                    Center = new Vector2f(Size.X / 2f, Size.Y / 2f),
                    Size = new Vector2f(Size.X, Size.Y),
                }
            );

            // Zoom, Flip and Rotate
            Zoom(1f);
            AutomaticallyZoomed = false;
            FlippedX = false;
            RotateImage(DefaultRotation);

            // Color
            if (ImageColor != Color.White)
                SetImageColor(Color.White);

            // Click-Through-Able?
            if (ClickThroughAble)
                _ = ToggleClickThroughAble();

            // Forcre Fit To Monitor Height?
            var mousePos = Mouse.GetPosition();
            var bounds = ImageViewerUtils.GetCurrentBounds(mousePos);
            if (Config.LimitImagesToMonitor != LimitImagesToMonitorOption.None)
            {
                // Fit to monitor height/width
                var limit = Config.LimitImagesToMonitor;

                if (limit == LimitImagesToMonitorOption.Auto)
                {
                    limit =
                        bounds.Height < bounds.Width
                            ? LimitImagesToMonitorOption.Height
                            : LimitImagesToMonitorOption.Width;
                }

                if (limit == LimitImagesToMonitorOption.Height && Size.Y > bounds.Height)
                {
                    Zoom((float)bounds.Height / Size.Y, true);
                    FitToMonitorHeightForced = true;
                }
                else if (limit == LimitImagesToMonitorOption.Width && Size.X > bounds.Width)
                {
                    Zoom((float)bounds.Width / Size.X, true);
                    AutomaticallyZoomed = true;
                }
            }
            if (Math.Min(Size.X, Size.Y) < Config.MinImageSize)
            {
                // Reisze images smaller than min size to min size
                AutomaticallyZoomed = true;
                Zoom(Config.MinImageSize / Math.Min(Size.X, Size.Y), true);
            }

            // Center image or place in top-left corner if it's a large/wide image.
            IntRect currentWorkingArea;
            var workingArea = ImageViewerUtils.GetCurrentWorkingArea(mousePos);
            currentWorkingArea = !FitToMonitorHeightForced ? workingArea : bounds;

            if (
                Config.PositionLargeWideImagesInCorner
                && Size.X > Size.Y
                && Size.X * CurrentZoom >= bounds.Width
            )
            {
                // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
                NextWindowPos = new Vector2i(bounds.Left, bounds.Top);
            }
            else
            {
                // Center
                NextWindowPos = new Vector2i(
                    NextWindowSize.X >= currentWorkingArea.Width - 2
                        ? currentWorkingArea.Left
                        : currentWorkingArea.Left
                            + (currentWorkingArea.Width / 2)
                            - ((int)(Size.X * CurrentZoom) / 2),
                    NextWindowSize.Y >= currentWorkingArea.Height - 2
                        ? currentWorkingArea.Top
                        : currentWorkingArea.Top
                            + (currentWorkingArea.Height / 2)
                            - ((int)(Size.Y * CurrentZoom) / 2)
                );
            }

            // Temporarily set always on top to bring it infront of the taskbar?
            ForceAlwaysOnTopCheck(bounds, workingArea);

            ViewStateHistory.Clear();

            if (Config.ClearMemoryOnResetImage)
                Graphics.ClearMemory(Image, File);
        }

        public bool ToggleSmoothing(int val = -1)
        {
            if (Image is AnimatedImage animatedImage)
                animatedImage.Data.Smooth = val == -1 ? !animatedImage.Data.Smooth : (val == 1);
            else if (Image is Sprite sprite)
                sprite.Texture.Smooth = val == -1 ? !sprite.Texture.Smooth : (val == 1);
            else if (Image is DisplayObject displayObject && displayObject.NumChildren > 0)
            {
                for (int i = 0; i < displayObject.NumChildren; i++)
                {
                    var child = displayObject.GetChildAt(i);
                    if (child is not Sprite childSprite)
                        continue;
                    childSprite.Texture.Smooth =
                        val == -1 ? !childSprite.Texture.Smooth : (val == 1);
                }
            }
            Updated = true;

            return Smoothing();
        }

        public bool Smoothing()
        {
            if (Image is AnimatedImage animatedImage)
                return animatedImage.Data.Smooth;
            else if (Image is Sprite sprite)
                return sprite.Texture.Smooth;
            else if (Image is DisplayObject displayObject && displayObject.NumChildren > 0)
            {
                var child = displayObject.GetChildAt(0);
                if (child is Sprite childSprite)
                    return childSprite.Texture.Smooth;
            }
            return false;
        }

        public bool ToggleBackground(int val = -1)
        {
            BackgroundsForImagesWithTransparency =
                val == -1 ? !BackgroundsForImagesWithTransparency : val == 1;
            Updated = true;

            return BackgroundsForImagesWithTransparency;
        }

        public bool ToggleImageTransparency(int val = -1)
        {
            if ((val == -1 && ImageColor == Color.White) || val == 1)
            {
                var colour = System.Drawing.ColorTranslator.FromHtml(
                    Config.TransparencyToggleValue
                );
                ImageColor = new Color(colour.R, colour.G, colour.B, colour.A);
            }
            else
                ImageColor = Color.White;
            SetImageColor(ImageColor);
            Updated = true;

            return true;
        }

        public void AdjustImageTransparency(int amount = 1)
        {
            var adjustment =
                amount
                * (255 * (ZoomFaster ? (Config.ZoomSpeedFast / 100f) : (Config.ZoomSpeed / 100f)));
            SetImageColor(
                new Color(
                    ImageColor.R,
                    ImageColor.G,
                    ImageColor.B,
                    (byte)Math.Clamp(ImageColor.A + adjustment, 2, 255)
                )
            );
            Updated = true;
        }

        public void SetImageTransparency(byte alpha = 255)
        {
            SetImageColor(new Color(ImageColor.R, ImageColor.G, ImageColor.B, alpha));
            Updated = true;
        }

        public void SetImageColor(Color color)
        {
            ImageColor = color;
            if (Image is DisplayObject obj)
                obj.Color = color;
            else if (Image is Sprite sprite)
                sprite.Color = color;
        }

        public void ResetSize()
        {
            if (Image is AnimatedImage animatedImage)
                Size = animatedImage.Texture.Size;
            else if (Image is DisplayObject displayObject)
                Size = displayObject.Texture.Size;
            else if (Image is Sprite sprite)
                Size = sprite.Texture.Size;
        }

        public bool ToggleLock(int val = -1)
        {
            Locked = val == -1 ? !Locked : val == 1;
            Dragging = false;

            return Locked;
        }

        public bool ToggleAlwaysOnTop(int val = -1)
        {
            if (val != -1 && AlwaysOnTop == (val == 1))
                return AlwaysOnTop;

            AlwaysOnTop = val == -1 ? !AlwaysOnTop : val == 1;
            AlwaysOnTopForced = false;
            DWM.SetAlwaysOnTop(Window.SystemHandle, AlwaysOnTop);

            return AlwaysOnTop;
        }

        public void ForceAlwaysOnTop()
        {
            ForceAlwaysOnTopNextTick = false;
            AlwaysOnTop = true;
            AlwaysOnTopForced = true;
            DWM.SetAlwaysOnTop(Window.SystemHandle);
        }

        /// <summary>Turns Always On Top off if it was forced.</summary>
        public void UnforceAlwaysOnTop()
        {
            ForceAlwaysOnTopNextTick = false;

            if (!AlwaysOnTopForced)
                return;

            AlwaysOnTop = false;
            AlwaysOnTopForced = false;
            DWM.SetAlwaysOnTop(Window.SystemHandle, false);
        }

        public void ForceAlwaysOnTopCheck(IntRect bounds, IntRect workingArea)
        {
            if (
                NextWindowSize.Y >= bounds.Height
                && NextWindowSize.X < bounds.Width
                && (
                    (
                        bounds.Height != workingArea.Height
                        && (
                            NextWindowPos.Y + NextWindowSize.Y
                                >= workingArea.Top + workingArea.Height
                            || NextWindowPos.Y <= workingArea.Top
                        )
                    )
                    || (
                        bounds.Width != workingArea.Width
                        && (
                            NextWindowPos.X <= workingArea.Left
                            || NextWindowPos.X + NextWindowSize.X
                                >= workingArea.Left + workingArea.Width
                        )
                    )
                )
            )
                ForceAlwaysOnTopNextTick = true;
        }

        public bool ToggleClickThroughAble(int val = -1)
        {
            if (val != -1 && ClickThroughAble == (val == 1))
                return ClickThroughAble;

            ClickThroughAble = val == -1 ? !ClickThroughAble : val == 1;
            DWM.SetClickThroughAble(Window.SystemHandle, ClickThroughAble);

            return ClickThroughAble;
        }

        public bool ToggleTitleBar(int val = -1)
        {
            if (val != -1 && ShowTitleBar == (val == 1))
                return ShowTitleBar;

            var diff = ShowTitleBar
                ? DWM.GetTitleBarDifference(Window.SystemHandle)
                : new Vector2i();

            ShowTitleBar = val == -1 ? !ShowTitleBar : val == -1;

            DWM.TitleBarSetVisible(Window, ShowTitleBar);
            if (ShowTitleBar)
            {
                Window.Position -= DWM.GetTitleBarDifference(Window.SystemHandle);
                Zoom(CurrentZoom, true);
            }
            else
            {
                NextWindowPos = Window.Position + diff;
                Updated = true;
            }

            return ShowTitleBar;
        }

        public bool ToggleTaskBarIconVisible(int val = -1)
        {
            bool visible = val == -1 ? !DWM.TaskbarIconVisible : (val == 1);
            if (val != -1 && DWM.TaskbarIconVisible == visible)
                return visible;
            DWM.TaskBarIconSetVisible(Window.SystemHandle, visible);
            return visible;
        }

        public void CropStart()
        {
            if (Cropping)
                return;
            Cropping = true;

            if (CropRect == null)
            {
                CropRect = new RectangleShape();

                var colour = System.Drawing.ColorTranslator.FromHtml(Config.CropToolFillColour);
                CropRect.FillColor = new Color(colour.R, colour.G, colour.B, colour.A);

                if (Config.CropToolOutlineThickness > 0)
                {
                    colour = System.Drawing.ColorTranslator.FromHtml(Config.CropToolOutlineColour);
                    CropRect.OutlineColor = new Color(colour.R, colour.G, colour.B, colour.A);
                }
            }
            CropRect.OutlineThickness = Config.CropToolOutlineThickness * (1 / CurrentZoom);

            CropStartPos = ImageViewerUtils.LimitToWindow(Mouse.GetPosition(), Window);
            CropRect.Position = Window.MapPixelToCoords(
                CropStartPos
                    - (ShowTitleBar ? DWM.GetWindowClientPos(Window.SystemHandle) : Window.Position)
            );
        }

        public void CropEnd()
        {
            if (!Cropping || CropRect == null)
                return;
            Cropping = false;

            // Too small - cancel
            if (Math.Abs(CropRect.Size.X) < 10 || Math.Abs(CropRect.Size.Y) < 10)
            {
                Redraw();
                return;
            }

            // Record view state
            ViewStateHistory.Add(GetCurrentViewState());

            // Get position
            var pos = ImageViewerUtils.LimitToWindow(Mouse.GetPosition(), Window);

            // Apply crop
            var view = Window.GetView();
            view.Center = new Vector2f(
                CropRect.Position.X + (CropRect.Size.X / 2f),
                CropRect.Position.Y + (CropRect.Size.Y / 2f)
            );
            Size = new Vector2u((uint)Math.Abs(CropRect.Size.X), (uint)Math.Abs(CropRect.Size.Y));
            if (Rotation == 90 || Rotation == 270)
            {
                NextWindowSize = new Vector2u(
                    (uint)(Size.Y * CurrentZoom),
                    (uint)(Size.X * CurrentZoom)
                );
                view.Size = new Vector2f(Size.Y, Size.X * (FlippedX ? -1 : 1));
            }
            else
            {
                NextWindowSize = new Vector2u(
                    (uint)(Size.X * CurrentZoom),
                    (uint)(Size.Y * CurrentZoom)
                );
                view.Size = new Vector2f(Size.X * (FlippedX ? -1 : 1), Size.Y);
            }
            Window.SetView(view);
            Window.Size = NextWindowSize;

            // Re-apply current zoom
            if (CurrentZoom != 1 || ShowTitleBar)
                Zoom(CurrentZoom);

            // Position
            NextWindowPos = new Vector2i(
                pos.X < CropStartPos.X ? pos.X : CropStartPos.X,
                pos.Y < CropStartPos.Y ? pos.Y : CropStartPos.Y
            );
            if (ShowTitleBar)
                NextWindowPos -= DWM.GetTitleBarDifference(Window.SystemHandle);

            CropRect.Size = new Vector2f();

            Updated = true;
        }

        public void UndoCrop()
        {
            if (ViewStateHistory.Count == 0)
                return;

            var state = ViewStateHistory[^1];

            Size = state.Size;
            Rotation = state.Rotation;
            CurrentZoom = state.Zoom;
            FlippedX = state.FlippedX;
            NextWindowSize =
                Rotation == 90 || Rotation == 270
                    ? new Vector2u((uint)(Size.Y * CurrentZoom), (uint)(Size.X * CurrentZoom))
                    : new Vector2u((uint)(Size.X * CurrentZoom), (uint)(Size.Y * CurrentZoom));
            NextWindowPos = state.Position;
            Window.SetView(state.View);

            ViewStateHistory.RemoveAt(ViewStateHistory.Count - 1);

            Updated = true;
        }

        public ViewState GetCurrentViewState()
        {
            return new ViewState(
                Window.GetView(),
                Size,
                NextWindowPos,
                CurrentZoom,
                Rotation,
                FlippedX
            );
        }

        public void RenderSVGAtCurrentZoom()
        {
            if (
                CurrentZoom == 1
                || Image is null
                || !File.EndsWith(".svg", StringComparison.OrdinalIgnoreCase)
            )
                return;

            var targetWidth = (uint)(Size.X * CurrentZoom);
            var targetHeight = (uint)(Size.Y * CurrentZoom);

            var densityX = targetWidth / (Size.X / 72.0);
            var densityY = targetHeight / (Size.Y / 72.0);

            var texture = Graphics.GetTexture(
                File,
                new ImageMagick.MagickReadSettings
                {
                    Density = new ImageMagick.Density(densityX, densityY),
                    Width = targetWidth,
                    Height = targetHeight,
                    BackgroundColor = ImageMagick.MagickColors.None,
                },
                false
            );
            if (texture == null || texture is not SingleTexture tex)
                return;

            Image = new Sprite(tex.Texture);
            ResetSize();

            Window.SetView(
                new View(Window.DefaultView)
                {
                    Center = new Vector2f(Size.X / 2f, Size.Y / 2f),
                    Size = new Vector2f(Size.X, Size.Y),
                }
            );
            // Zoom, Flip and Rotate
            Zoom(1f);
            AutomaticallyZoomed = false;
            FlippedX = false;
            RotateImage(DefaultRotation);
        }

        ///////////////////////////
        //     Image Loading     //
        ///////////////////////////

        private bool LoadImage(string fileName)
        {
            File = fileName;

            if (ImageViewerUtils.IsAnimatedImage(fileName))
                Image = Graphics.GetAnimatedImage(fileName);
            else
                Image = Graphics.GetImage(fileName);

            if (Image is null)
                return false;

            ResetSize();
            DefaultRotation = ImageViewerUtils.GetDefaultRotationFromEXIF(fileName);

            return true;
        }

        private bool LoadedClipboardImage = false;

        private bool LoadImageFromClipboard()
        {
            File = "";
            LoadedClipboardImage = false;

            var thread = new Thread(() =>
            {
                if (!System.Windows.Forms.Clipboard.ContainsImage())
                {
                    LoadedClipboardImage = false;
                    return;
                }

                var image = System.Windows.Forms.Clipboard.GetImage();
                if (image == null)
                {
                    LoadedClipboardImage = false;
                    return;
                }

                var stream = new MemoryStream();
                try
                {
                    image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                catch
                {
                    LoadedClipboardImage = false;
                    return;
                }

                ClipboardBitmap = new System.Drawing.Bitmap(image);
                Image = new Sprite(new Texture(stream));

                if (Image == null)
                {
                    LoadedClipboardImage = false;
                    return;
                }

                ResetSize();
                DefaultRotation = 0;

                LoadedClipboardImage = true;
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join(); // wait for thread to finish

            return LoadedClipboardImage;
        }

        private bool ChangeImage(string fileName)
        {
            Dragging = false;
            var prevSize = Image == null ? new Vector2u() : Size;
            float prevRotation = Image == null ? 0 : Rotation;
            int prevDefaultRotation = DefaultRotation;

            var bounds = ImageViewerUtils.GetCurrentBounds(
                Window.Position
                    + (
                        Image == null
                            ? new Vector2i()
                            : new Vector2i(
                                (int)(Size.X * CurrentZoom) / 2,
                                (int)(Size.Y * CurrentZoom) / 2
                            )
                    )
            );

            // Dispose of previous image
            Image?.Dispose();
            Image = null;

            // Load new image
            if (fileName != "" && !LoadImage(fileName))
                return false;
            else if (fileName == "" && !LoadImageFromClipboard())
                return false;

            var view = Window.DefaultView;
            view.Center = new Vector2f(Size.X / 2f, Size.Y / 2f);
            view.Size = new Vector2f(Size.X, Size.Y);
            Window.SetView(view);

            // Rotation
            RotateImage(
                prevRotation == prevDefaultRotation ? DefaultRotation : (int)prevRotation,
                false,
                false
            );
            // Smoothing
            if (Image is AnimatedImage animatedImage)
            {
                animatedImage.Data.Smooth =
                    Math.Min(Size.X, Size.Y) >= Config.SmoothingMinImageSize
                    && Config.SmoothingDefault;
                animatedImage.Data.Mipmap = Config.Mipmapping;
            }
            else if (Image is Sprite sprite)
            {
                sprite.Texture.Smooth =
                    Math.Min(Size.X, Size.Y) >= Config.SmoothingMinImageSize
                    && Config.SmoothingDefault;
                if (Config.Mipmapping)
                    sprite.Texture.GenerateMipmap();
            }

            // Color
            if (Image is not null && ImageColor != Color.White)
                SetImageColor(ImageColor);

            // Don't keep current zoom value if it wasn't set by user
            if (
                AutomaticallyZoomed
                || FitToMonitorHeightForced
                || Config.ImageSizing == SizingOption.FullSize
            )
            {
                AutomaticallyZoomed = false;
                FitToMonitorHeightForced = false;
                CurrentZoom = 1;
            }
            else if (CurrentZoom != 1 && Config.ImageSizing != SizingOption.KeepZoom)
            {
                // Resize Image to be similar size to previous image
                var actualPrevSize =
                    (Rotation == 0 || Rotation == 180)
                        ? prevSize
                        : new Vector2u(prevSize.Y, prevSize.X);
                CurrentZoom =
                    Config.ImageSizing == SizingOption.FitHeight
                    || (
                        Config.ImageSizing == SizingOption.FitAuto
                        && actualPrevSize.Y < actualPrevSize.X
                    )
                        ? actualPrevSize.Y * CurrentZoom / CurrentImageSize().Y
                        : actualPrevSize.X * CurrentZoom / CurrentImageSize().X;
            }

            bool wasFitToMonitorDimension = FitToMonitorHeightForced;
            if (Config.LimitImagesToMonitor != LimitImagesToMonitorOption.None)
            {
                // Fit to monitor height/width
                var limit = Config.LimitImagesToMonitor;

                if (limit == LimitImagesToMonitorOption.Auto)
                {
                    limit =
                        bounds.Height < bounds.Width
                            ? LimitImagesToMonitorOption.Height
                            : LimitImagesToMonitorOption.Width;
                }

                if (
                    limit == LimitImagesToMonitorOption.Height
                    && (FitToMonitorHeight || CurrentImageSize().Y * CurrentZoom > bounds.Height)
                )
                {
                    Zoom((float)bounds.Height / CurrentImageSize().Y, true);

                    bounds = ImageViewerUtils.GetCurrentBounds(
                        NextWindowPos
                            + new Vector2i(
                                (int)(CurrentImageSize().X * CurrentZoom) / 2,
                                (int)(CurrentImageSize().Y * CurrentZoom) / 2
                            )
                    );
                    NextWindowPos = new Vector2i(NextWindowPos.X, bounds.Top);

                    if (!FitToMonitorHeight)
                        FitToMonitorHeightForced = true;

                    wasFitToMonitorDimension = true;
                }
                else if (
                    limit == LimitImagesToMonitorOption.Width
                    && CurrentImageSize().X * CurrentZoom > bounds.Width
                )
                {
                    Zoom((float)bounds.Width / CurrentImageSize().X, true);

                    bounds = ImageViewerUtils.GetCurrentBounds(
                        NextWindowPos
                            + new Vector2i(
                                (int)(CurrentImageSize().X * CurrentZoom) / 2,
                                (int)(CurrentImageSize().Y * CurrentZoom) / 2
                            )
                    );
                    NextWindowPos = new Vector2i(bounds.Left, NextWindowPos.Y);

                    AutomaticallyZoomed = true;
                    wasFitToMonitorDimension = true;
                }
            }
            if (!wasFitToMonitorDimension)
            {
                if (FitToMonitorHeightForced)
                {
                    Zoom(1, true);
                    FitToMonitorHeightForced = false;
                }
                else if (Math.Min(Size.X, Size.Y) * CurrentZoom < Config.MinImageSize)
                {
                    // Reisze images smaller than min size to min size
                    if (Math.Min(Size.X, Size.Y) < Config.MinImageSize)
                    {
                        AutomaticallyZoomed = true;
                        Zoom(Config.MinImageSize / Math.Min(Size.X, Size.Y), true);
                    }
                    else
                        Zoom(1, true);
                }
                else
                    Zoom(CurrentZoom, true);
            }

            var boundsPos =
                NextWindowPos
                + new Vector2i(
                    (int)(CurrentImageSize().X * CurrentZoom) / 2,
                    (int)(CurrentImageSize().Y * CurrentZoom) / 2
                );
            bounds = ImageViewerUtils.GetCurrentBounds(boundsPos, false);
            if (bounds == default)
            {
                boundsPos = Mouse.GetPosition();
                bounds = ImageViewerUtils.GetCurrentBounds(boundsPos);
            }

            // Position Window at top-left if the image is wide (ie: a Desktop Wallpaper / Screenshot)
            // Otherwise, if image is hanging off monitor just center it.
            if (
                Config.PositionLargeWideImagesInCorner
                && CurrentImageSize().X > CurrentImageSize().Y
                && CurrentImageSize().X * CurrentZoom >= bounds.Width
            )
            {
                NextWindowPos = new Vector2i(bounds.Left, bounds.Top);
            }
            else if (
                !prevSize.Equals(Size)
                && (
                    NextWindowPos.Y <= bounds.Top
                    || NextWindowPos.X + (CurrentImageSize().X * CurrentZoom)
                        >= bounds.Left + bounds.Width
                    || NextWindowPos.Y + (CurrentImageSize().Y * CurrentZoom)
                        >= bounds.Top + bounds.Height
                )
            )
            {
                NextWindowPos = new Vector2i(
                    bounds.Left + (int)((bounds.Width - (CurrentImageSize().X * CurrentZoom)) / 2),
                    bounds.Top + (int)((bounds.Height - (CurrentImageSize().Y * CurrentZoom)) / 2)
                );
            }

            // Temporarily set always on top to bring it infront of the taskbar?
            ForceAlwaysOnTopCheck(bounds, ImageViewerUtils.GetCurrentWorkingArea(boundsPos));

            Window.SetTitle(fileName == "" ? "vimage" : Path.GetFileName(fileName) + " - vimage");
            ContextMenu?.Setup();

            if (NextWindowSize.X == bounds.Width && NextWindowSize.Y == bounds.Height)
                DWM.PreventExlusiveFullscreen(Window); // prevent exlusive fullscreen when image is the same size as current screen

            ViewStateHistory.Clear();

            return true;
        }

        private void PreloadNextImage()
        {
            if (PreloadingImage || PreloadingNextImage == 0 || FolderContents.Count == 0)
                return;

            PreloadingImage = true;
            PreloadNextImageStart = false;
            int pos = FolderPosition;

            bool success;
            do
            {
                if (PreloadingNextImage == 1)
                    pos = pos == FolderContents.Count - 1 ? 0 : pos + 1;
                else if (PreloadingNextImage == -1)
                    pos = pos == 0 ? FolderContents.Count - 1 : pos - 1;
                else
                    return;

                success = Graphics.PreloadImage(FolderContents[pos]);
            } while (!success);

            PreloadingNextImage = 0;
            PreloadingImage = false;
        }

        public void NextImage()
        {
            GetFolderContents();
            if (FolderContents.Count <= 1)
                return;

            if (!Config.LoopImageNavigation && FolderPosition == FolderContents.Count - 1)
                return;

            bool success;
            do
            {
                FolderPosition =
                    FolderPosition >= FolderContents.Count - 1 ? 0 : FolderPosition + 1;
                success = ChangeImage(FolderContents[FolderPosition]);
            } while (!success);

            Update();

            // Preload next image?
            if (Config.PreloadNextImage)
            {
                PreloadingNextImage = 1;
                PreloadNextImageStart = true;
            }
        }

        public void PrevImage()
        {
            GetFolderContents();
            if (FolderContents.Count <= 1)
                return;

            if (!Config.LoopImageNavigation && FolderPosition == 0)
                return;

            bool success;
            do
            {
                FolderPosition =
                    FolderPosition <= 0 ? FolderContents.Count - 1 : FolderPosition - 1;
                success = ChangeImage(FolderContents[FolderPosition]);
            } while (!success);

            Update();

            // Preload next image?
            if (Config.PreloadNextImage)
            {
                PreloadingNextImage = -1;
                PreloadNextImageStart = true;
            }
        }

        public void RandomImage()
        {
            GetFolderContents();
            if (FolderContents.Count <= 1)
                return;

            bool success;
            do
            {
                FolderPosition = rnd.Next(0, FolderContents.Count);
                success = ChangeImage(FolderContents[FolderPosition]);
            } while (!success);
        }

        public void ChangeSortBy(SortBy by)
        {
            if (by == SortImagesBy)
                return;
            SortImagesBy = by;

            SortImagesByDir =
                SortImagesBy == SortBy.Name ? SortDirection.Ascending : SortDirection.Descending;

            FolderContents.Clear();
            GetFolderContents();
        }

        public void ChangeSortByDirection(SortDirection dir)
        {
            if (dir == SortImagesByDir)
                return;
            SortImagesByDir = dir;

            FolderContents.Clear();
            GetFolderContents();
        }

        ///////////////////////////
        //         Other         //
        ///////////////////////////

        private void GetFolderContents()
        {
            if (FolderContents.Count > 0)
                return;

            if (File == "")
            {
                FolderContents = [];
                return;
            }

            var directory = Path.GetDirectoryName(File);
            if (directory is null || !Directory.Exists(directory))
                return;

            var contents = Directory
                .GetFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(ImageViewerUtils.IsSupportedFileType);

            switch (SortImagesBy)
            {
                case SortBy.Name:
                {
                    FolderContents = [.. contents];
                    FolderContents.Sort(new WindowsFileSorting.NaturalStringComparer());
                    if (SortImagesByDir == SortDirection.Descending)
                        FolderContents.Reverse();
                    break;
                }
                case SortBy.Date:
                {
                    FolderContents.AddRange(
                        SortImagesByDir == SortDirection.Ascending
                            ? contents.OrderBy(ImageViewerUtils.GetDateValueFromEXIF)
                            : contents.OrderByDescending(ImageViewerUtils.GetDateValueFromEXIF)
                    );
                    break;
                }
                case SortBy.DateModified:
                {
                    FolderContents.AddRange(
                        SortImagesByDir == SortDirection.Ascending
                            ? contents.OrderBy(d => new FileInfo(d).LastWriteTime)
                            : contents.OrderByDescending(d => new FileInfo(d).LastWriteTime)
                    );
                    break;
                }
                case SortBy.DateCreated:
                {
                    FolderContents.AddRange(
                        SortImagesByDir == SortDirection.Ascending
                            ? contents.OrderBy(d => new FileInfo(d).CreationTime)
                            : contents.OrderByDescending(d => new FileInfo(d).CreationTime)
                    );
                    break;
                }
                case SortBy.Size:
                {
                    FolderContents.AddRange(
                        SortImagesByDir == SortDirection.Ascending
                            ? contents.OrderBy(d => new FileInfo(d).Length)
                            : contents.OrderByDescending(d => new FileInfo(d).Length)
                    );
                    break;
                }
            }

            FolderPosition = FolderContents.IndexOf(File);
        }

        public void DeleteFile()
        {
            if (File == "")
                return;

            var fileName = File;

            GetFolderContents();
            if (FolderContents.Count == 1)
            {
                Image?.Dispose();
                Image = null;
                Window.Close();
            }
            else
            {
                NextImage();
                FolderContents.Clear();
            }

            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(
                fileName,
                Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin
            );
        }

        public void CopyFile()
        {
            if (File == "")
            {
                // No file (viewing clipboard image?) - copy as image instead
                CopyAsImage();
                return;
            }

            var thread = new Thread(() =>
            {
                System.Windows.Forms.Clipboard.SetFileDropList([File]);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void CopyAsImage()
        {
            var thread = new Thread(() =>
            {
                if (File == "")
                {
                    // No File (viewing clipboard image?)
                    if (ClipboardBitmap == null)
                        return;
                    System.Windows.Forms.Clipboard.SetImage(ClipboardBitmap);
                }
                else
                {
                    using var image = Graphics.GetMagickImage(File);
                    if (image == null)
                        return;
                    using var ms = new MemoryStream();
                    image.Write(ms, ImageMagick.MagickFormat.Png32);
                    ms.Position = 0;
                    var data = new System.Windows.Forms.DataObject();
                    data.SetData("PNG", false, ms);
                    System.Windows.Forms.Clipboard.SetDataObject(data, true);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void OpenFileAtLocation()
        {
            if (File == "")
                return;
            _ = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{File}\"",
                    UseShellExecute = true,
                }
            );
        }

        public void OpenDuplicateWindow(bool full = false)
        {
            var view = Window.GetView();
            var startInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath,
                Arguments = $"\"{File}\"",
            };
            if (full)
            {
                startInfo.Arguments += $" -sizeX {Size.X}";
                startInfo.Arguments += $" -sizeY {Size.Y}";
                startInfo.Arguments += $" -centerX {view.Center.X}";
                startInfo.Arguments += $" -centerY {view.Center.Y}";
                if (CurrentZoom != 1)
                    startInfo.Arguments += $" -zoom {CurrentZoom}";
                if (FlippedX)
                    startInfo.Arguments += " -flip";
                if (Rotation != 0)
                    startInfo.Arguments += $" -rotation {Rotation}";
                if (AlwaysOnTop)
                    startInfo.Arguments += " -alwaysOnTop";
                if (ImageColor != Color.White)
                {
                    string colour =
                        "#"
                        + ImageColor.A.ToString("X2", null)
                        + ImageColor.R.ToString("X2", null)
                        + ImageColor.G.ToString("X2", null)
                        + ImageColor.B.ToString("X2", null);
                    startInfo.Arguments += $" -colour {colour}";
                }
                startInfo.Arguments += $" -x {Window.Position.X}";
                startInfo.Arguments += $" -y {Window.Position.Y}";
            }
            Console.WriteLine(startInfo.Arguments);
            _ = Process.Start(startInfo);
        }

        public static void ExitAllInstances()
        {
            var current = Process.GetCurrentProcess();
            Process
                .GetProcessesByName(current.ProcessName)
                .Where(t => t.Id != current.Id)
                .ToList()
                .ForEach(t => t.Kill());

            current.Kill();
        }

        public void OpenConfig()
        {
            if (Config.OpenSettingsEXE)
            {
                string vimage_settings = Path.Combine(
                    AppContext.BaseDirectory,
                    "vimage_settings.exe"
                );
                if (System.IO.File.Exists(vimage_settings))
                {
                    _ = Process.Start(vimage_settings);
                    return;
                }
            }

            _ = Process.Start(
                new ProcessStartInfo
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "config.json"),
                    UseShellExecute = true,
                }
            );
        }

        public void ReloadConfig()
        {
            Config = Config.Load(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")
            );
            Controls.Update(Config);

            // Update ContextMenu
            if (ContextMenu is not null)
            {
                ContextMenu.FileNameCurrent = "";
                ContextMenu.LoadItems(
                    Config.ContextMenu,
                    Config.ContextMenu_Animation,
                    Config.ContextMenu_Animation_InsertAtIndex
                );
                ContextMenu.Setup(true);
            }

            // Update Background Colour?
            var backColour = System.Drawing.ColorTranslator.FromHtml(Config.BackgroundColour);
            var newBackColour = new Color(backColour.R, backColour.G, backColour.B, backColour.A);
            if (BackgroundColour != newBackColour)
            {
                BackgroundColour = newBackColour;
                if (BackgroundsForImagesWithTransparency)
                    Update();
            }

            if (ShowTitleBar != Config.ShowTitleBar)
                _ = ToggleTitleBar();
        }

        private void OnConfigChanged(object source, FileSystemEventArgs e)
        {
            // Wait a bit for the config file to be unlocked
            Thread.Sleep(500);
            ReloadConfigNextTick = true;
        }

        public void OpenContextMenu()
        {
            if (ContextMenu is null)
                return;
            ContextMenu.RefreshItems();
            ContextMenu.Show(Mouse.GetPosition().X, Mouse.GetPosition().Y);
            ContextMenu.Capture = true;
        }

        public void DoCustomAction(string actionKey)
        {
            var a = Config.CustomActions.FirstOrDefault((a) => a.Name == actionKey);
            var action = a?.Func;
            if (action == null || action == "")
                return;

            if (action.StartsWith('-'))
            {
                // Apply arguments to current instance of vimage
                ApplyArguments(action.Split(' '));
                return;
            }

            // Open new process with arguments
            if (File == "" && (action.Contains("%f") || action.Contains("%d")))
                return; // don't do the action if it requires the Filename but there isn't one

            action = action.Replace("%f", "\"" + File + "\"");
            action = action.Replace("%d", Path.GetDirectoryName(File) + "\\");

            // Split exe and arguments by the first space (regex to exclude the spaces within the quotes of the exe's path)
            var s = Actions.CustomActionSplitRegex().Split(action, 2);

            if (s[0].Contains('%'))
                s[0] = Environment.ExpandEnvironmentVariables(s[0]);

            try
            {
                _ = Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = s[0],
                        Arguments = s[1],
                        UseShellExecute = true,
                    }
                );
            }
            catch (Exception) { }
        }

        public void ApplyArguments(string[] args, bool ignoreFirst = false)
        {
            var viewCenter = new Vector2f(Size.X / 2f, Size.Y / 2f);

            int toggleSync = -1;
            int toggleSyncVal = -1;
            bool t = false;

            for (int i = ignoreFirst ? 1 : 0; i < args.Length; i++)
            {
                int val = -1;
                if (i < args.Length - 1 && !int.TryParse(args[i + 1], out val))
                    val = -1;

                float valf;
                switch (args[i])
                {
                    case "-x":
                        if (val != -1)
                        {
                            NextWindowPos.X = val;
                            i++;
                        }
                        break;
                    case "-y":
                        if (val != -1)
                        {
                            NextWindowPos.Y = val;
                            i++;
                        }
                        break;
                    case "-sizeX":
                        if (val != -1)
                        {
                            Size.X = (uint)val;
                            i++;
                        }
                        break;
                    case "-sizeY":
                        if (val != -1)
                        {
                            Size.Y = (uint)val;
                            i++;
                        }
                        break;
                    case "-centerX":
                        if (!float.TryParse(args[i + 1], out valf))
                            valf = -1;
                        if (valf != -1)
                        {
                            viewCenter.X = valf;
                            Updated = true;
                            i++;
                        }
                        break;
                    case "-centerY":
                        if (!float.TryParse(args[i + 1], out valf))
                            valf = -1;
                        if (valf != -1)
                        {
                            viewCenter.Y = valf;
                            Updated = true;
                            i++;
                        }
                        break;
                    case "-zoom":
                        if (!float.TryParse(args[i + 1], out valf))
                            valf = 0;
                        if (valf != 0)
                            Zoom(valf, true);
                        i++;
                        break;
                    case "-rotation":
                        if (val != -1)
                        {
                            if (Rotation != val)
                                RotateImage(val);
                            i++;
                        }
                        break;
                    case "-color":
                    case "-colour":
                        var colour = System.Drawing.ColorTranslator.FromHtml(args[i + 1]);
                        SetImageColor(new Color(colour.R, colour.G, colour.B, colour.A));
                        Updated = true;
                        i++;
                        break;
                    case "-alpha":
                        if (val != -1)
                        {
                            if (val >= 0 && val <= 255)
                                SetImageTransparency((byte)val);
                            i++;
                        }
                        break;

                    case "-frame":
                        if (val != -1)
                        {
                            if (Image is AnimatedImage animatedImage)
                            {
                                _ = animatedImage.SetFrame(val);
                                Updated = true;
                            }

                            i++;
                        }
                        break;
                    case "-next":
                        NextImage();
                        break;
                    case "-prev":
                        PrevImage();
                        break;
                    case "-random":
                        RandomImage();
                        break;
                    case "-reset":
                        ResetImage();
                        break;
                    case "-clearMemory":
                        Graphics.ClearMemory(Image, File);
                        break;
                    case "-rerenderSVG":
                        RenderSVGAtCurrentZoom();
                        break;

                    case "-fitToMonitorHeight":
                        ToggleFitToMonitor(LimitImagesToMonitorOption.Height);
                        break;
                    case "-fitToMonitorWidth":
                        ToggleFitToMonitor(LimitImagesToMonitorOption.Width);
                        break;
                    case "-fitToMonitorAuto":
                        ToggleFitToMonitor(LimitImagesToMonitorOption.Auto);
                        break;

                    case "-toggleSync":
                        toggleSync = 0;
                        break;

                    case "-flip":
                        if (val == 0 || val == 1)
                        {
                            _ = FlipImage(val);
                            i++;
                        }
                        else
                            t = FlipImage(toggleSyncVal);
                        break;
                    case "-smoothing":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleSmoothing(val);
                            i++;
                        }
                        else
                            t = ToggleSmoothing(toggleSyncVal);
                        break;
                    case "-background":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleBackground(val);
                            i++;
                        }
                        else
                            t = ToggleBackground(toggleSyncVal);
                        break;
                    case "-lock":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleLock(val);
                            i++;
                        }
                        else
                            t = ToggleLock(toggleSyncVal);
                        break;
                    case "-alwaysOnTop":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleAlwaysOnTop(val);
                            i++;
                        }
                        else
                            t = ToggleAlwaysOnTop(toggleSyncVal);
                        break;
                    case "-clickThrough":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleClickThroughAble(val);
                            i++;
                        }
                        else
                            t = ToggleClickThroughAble(toggleSyncVal);
                        break;
                    case "-titleBar":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleTitleBar(val);
                            i++;
                        }
                        else
                            t = ToggleTitleBar(toggleSyncVal);
                        break;
                    case "-taskbarIcon":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleTaskBarIconVisible(val);
                            i++;
                        }
                        else
                            t = ToggleTaskBarIconVisible(toggleSyncVal);
                        break;
                    case "-animation":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleAnimation(val);
                            i++;
                        }
                        else
                            t = ToggleAnimation(toggleSyncVal);
                        break;
                    case "-defaultTransparency":
                        if (val == 0 || val == 1)
                        {
                            _ = ToggleImageTransparency(val);
                            i++;
                        }
                        else
                            t = ToggleImageTransparency(toggleSyncVal);
                        break;
                }

                if (toggleSyncVal == -1)
                {
                    if (toggleSync == 0)
                        toggleSync = 1;
                    else if (toggleSync == 1)
                        toggleSyncVal = t ? 1 : 0;
                }
            }

            // Update view
            var view = Window.GetView();
            view.Center = viewCenter;
            if (Rotation == 90 || Rotation == 270)
            {
                NextWindowSize = new Vector2u(
                    (uint)(Size.Y * CurrentZoom),
                    (uint)(Size.X * CurrentZoom)
                );
                view.Size = new Vector2f(Size.Y, Size.X * (FlippedX ? -1 : 1));
            }
            else
            {
                NextWindowSize = new Vector2u(
                    (uint)(Size.X * CurrentZoom),
                    (uint)(Size.Y * CurrentZoom)
                );
                view.Size = new Vector2f(Size.X * (FlippedX ? -1 : 1), Size.Y);
            }
            Window.SetView(view);
            Window.Size = NextWindowSize;
        }
    }

    internal class ViewState(
        View view,
        Vector2u size,
        Vector2i position,
        float zoom,
        int rotation,
        bool flippedX
    )
    {
        public View View = new(view);
        public Vector2u Size = size;
        public Vector2i Position = position;
        public float Zoom = zoom;
        public int Rotation = rotation;
        public bool FlippedX = flippedX;
    }
}
