using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using DevIL.Unmanaged;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace vimage
{
    class ImageViewer
    {
        public const string VERSION_NAME = "vimage version #";

        public readonly string[] EXTENSIONS =
        {
            "bmp", "cut", "dds", "doom", "exr", "hdr", "gif", "ico", "jp2", "jpg", "jpeg", "lbm", "mdl", "mng",
            "pal", "pbm", "pcd", "pcx", "pgm", "pic", "png", "ppm", "psd", "psp", "raw", "sgi", "tga", "tif"
        };

        public readonly float ZOOM_MIN = 0.05f;
        public readonly float ZOOM_MAX = 75f;

        public RenderWindow Window;
        public dynamic Image;
        public string File;
        public List<string> FolderContents = new List<string>();
        public int FolderPosition = 0;
        private ContextMenu ContextMenu;
        public Color ImageColor = Color.White;

        public Config Config;
        private FileSystemWatcher ConfigFileWatcher;
        private bool ReloadConfigNextTick = false;

        private bool Updated = false;
        public bool CloseNextTick = false;

        /// <summary>
        /// Instead of setting the Window Position directly when the image is going to be Updated, this is set.
        /// This prevents the old image being shown at the new image location for a split-second before the new image is loaded.
        /// </summary>
        private Vector2i NextWindowPos = new Vector2i();
        private Vector2u NextWindowSize = new Vector2u();
        private bool Dragging = false;
        private Vector2i DragPos = new Vector2i();
        private Vector2i MousePos = new Vector2i();
        private bool TransparencyMod = false;
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
        public Color BackgroundColour = new Color(230, 230, 230);
        public bool AlwaysOnTop = false;
        private bool AlwaysOnTopForced = false;
        /// <summary>
        /// If true will turn AlwaysOnTop mode on next update if the window height >= monitor height and window width < monitor width.
        /// If the window is wider and taller than the monitor it will automatically be above the task bar anyway.
        /// </summary>
        private bool ForceAlwaysOnTopNextTick = false;
        /// <summary>0=false, 1=next, -1=prev.</summary>
        private int PreloadingNextImage = 0;
        private bool PreloadNextImageStart = false;
        public SortBy SortImagesBy = SortBy.Name;
        public SortDirection SortImagesByDir = SortDirection.Ascending;

        private static readonly Random rnd = new Random();

        public ImageViewer(string file)
        {
            IL.Initialize();

            // Extension supported?
            if (!ImageViewerUtils.IsValidExtension(file, EXTENSIONS))
                return;

            // Save Mouse Position -> will open image at this position
            Vector2i mousePos = Mouse.GetPosition();

            // Create Window
            Window = new RenderWindow(new VideoMode(0, 0), File + " - vimage", Styles.None);

            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND();
            bb.dwFlags = DWM_BB.Enable | DWM_BB.BlurRegion;
            bb.fEnable = true;
            bb.hRgnBlur = DWM.CreateRectRgn(0, 0, -1, -1);
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

            // Load Config File
            Config = new Config();
            Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));

            if (Config.Setting_ListenForConfigChanges)
            {
                ConfigFileWatcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "config.txt");
                ConfigFileWatcher.NotifyFilter = NotifyFilters.LastWrite;
                ConfigFileWatcher.Changed += new FileSystemEventHandler(OnConfigChanged);
                ConfigFileWatcher.EnableRaisingEvents = true;
            }
            BackgroundsForImagesWithTransparency = Config.Setting_BackgroundForImagesWithTransparencyDefault;
            System.Drawing.Color backColour = System.Drawing.ColorTranslator.FromHtml(Config.Setting_BackgroundColour);
            BackgroundColour = new Color(backColour.R, backColour.G, backColour.B, backColour.A);

            // Get Image
            ChangeImage(file);

            if (Image == null)
            {
                Window.Close();
                return;
            }

            // Position window at mouse position?
            Vector2i winPos = NextWindowPos;
            IntRect bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);
            if (Config.Setting_OpenAtMousePosition &&
                !(Config.Setting_PositionLargeWideImagesInCorner && CurrentImageSize().X > CurrentImageSize().Y && CurrentImageSize().X * CurrentZoom >= bounds.Width))
            {
                // At Mouse Position
                winPos = new Vector2i(mousePos.X - (int)(NextWindowSize.X / 2), mousePos.Y - (int)(NextWindowSize.Y / 2));

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
            NextWindowPos = winPos;

            // Display Window
            Window.Size = NextWindowSize;
            Window.Position = NextWindowPos;
            Redraw();
            Updated = false;
            Window.SetActive();

            // Get/Set Folder Sorting
            SortImagesBy = Config.Setting_DefaultSortBy;
            SortImagesByDir = Config.Setting_DefaultSortDir;

            if (SortImagesBy == SortBy.FolderDefault || SortImagesByDir == SortDirection.FolderDefault)
            {
                // Get parent folder name
                string parentFolder = file.Substring(0, file.LastIndexOf('\\'));
                parentFolder = parentFolder.Substring(parentFolder.LastIndexOf('\\') + 1, parentFolder.Length - parentFolder.LastIndexOf('\\') - 1);

                // Get sort column info from window with corresponding name
                SHDocVw.ShellWindows shellWindows = new SHDocVw.ShellWindows();
                foreach (SHDocVw.ShellBrowserWindow shellWindow in shellWindows)
                {
                    if (shellWindow.LocationName != parentFolder)
                        continue;

                    Shell32.ShellFolderView view = (Shell32.ShellFolderView)shellWindow.Document;

                    string sort = view.SortColumns;
                    sort = sort.Substring(5, sort.Length - 5);

                    // Direction
                    if (sort[0] == '-')
                    {
                        sort = sort.Substring(1, sort.Length - 1);

                        if (SortImagesByDir == SortDirection.FolderDefault)
                            SortImagesByDir = SortDirection.Descending;
                    }
                    else if (SortImagesByDir == SortDirection.FolderDefault)
                        SortImagesByDir = SortDirection.Ascending;

                    // By
                    if (SortImagesBy == SortBy.FolderDefault)
                    {
                        switch (sort)
                        {
                            case "System.ItemDate;": SortImagesBy = SortBy.Date; break;
                            case "System.DateModified;": SortImagesBy = SortBy.DateModified; break;
                            case "System.DateCreated;": SortImagesBy = SortBy.DateCreated; break;
                            case "System.Size;": SortImagesBy = SortBy.Size; break;
                            default: SortImagesBy = SortBy.Name; break;
                        }
                    }
                }
            }
            // Default sorting if folder was closed
            if (SortImagesBy == SortBy.FolderDefault)
                SortImagesBy = SortBy.Name;
            if (SortImagesByDir == SortDirection.FolderDefault)
                SortImagesByDir = SortDirection.Ascending;

            // Create Context Menu
            ContextMenu = new ContextMenu(this);
            ContextMenu.LoadItems(Config.ContextMenu, Config.ContextMenu_Animation, Config.ContextMenu_Animation_InsertAtIndex);
            ContextMenu.Setup(false);

            // Interaction
            Window.Closed += OnWindowClosed;
            Window.MouseButtonPressed += OnMouseDown;
            Window.MouseButtonReleased += OnMouseUp;
            Window.MouseWheelScrolled += OnMouseWheelScrolled;
            Window.MouseMoved += OnMouseMoved;
            Window.KeyReleased += OnKeyUp;
            Window.KeyPressed += OnKeyDown;

            // Loop
            Stopwatch clock = new Stopwatch();
            clock.Start();
            
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
                
                // Animated Image?
                if (Image is AnimatedImage)
                {
                    bool imageUpdated = Image.Update((float)clock.Elapsed.TotalMilliseconds);
                    if (!Updated && imageUpdated)
                        Redraw();
                }
                clock.Restart();

                // Drag Window
                if (Dragging)
                {
                    NextWindowPos = new Vector2i(Mouse.GetPosition().X - DragPos.X, Mouse.GetPosition().Y - DragPos.Y);
                    if (DragLimitToBoundsMod)
                    {
                        // limit to monitor bounds
                        IntRect currentBounds = ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());

                        if (Window.Size.X > currentBounds.Width)
                        {
                            if (NextWindowPos.X > currentBounds.Left)
                                NextWindowPos.X = currentBounds.Left;
                            else if (NextWindowPos.X < currentBounds.Left + currentBounds.Width - Window.Size.X)
                                NextWindowPos.X = currentBounds.Left + currentBounds.Width - (int)Window.Size.X;
                        }
                        else if (NextWindowPos.X < currentBounds.Left)
                            NextWindowPos.X = currentBounds.Left;
                        else if (NextWindowPos.X > currentBounds.Left + currentBounds.Width - Window.Size.X)
                            NextWindowPos.X = currentBounds.Left + currentBounds.Width - (int)Window.Size.X;

                        if (Window.Size.Y > currentBounds.Height)
                        {
                            if (NextWindowPos.Y > currentBounds.Top)
                                NextWindowPos.Y = currentBounds.Top;
                            else if (NextWindowPos.Y < currentBounds.Top + currentBounds.Height - Window.Size.Y)
                                NextWindowPos.Y = currentBounds.Top + currentBounds.Height - (int)Window.Size.Y;
                        }
                        else if (NextWindowPos.Y < currentBounds.Top)
                            NextWindowPos.Y = currentBounds.Top;
                        else if (NextWindowPos.Y > currentBounds.Top + currentBounds.Height - Window.Size.Y)
                            NextWindowPos.Y = currentBounds.Top + currentBounds.Height - (int)Window.Size.Y;
                    }
                    Window.Position = NextWindowPos;
                }

                // Update
                if (Updated)
                {
                    Updated = false;
                    Window.Size = NextWindowSize;
                    Redraw();
                    Window.Position = NextWindowPos;
                }

                if (ForceAlwaysOnTopNextTick)
                {
                    bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);
                    if (Window.Size.Y >= bounds.Height && Window.Size.X < bounds.Width)
                        ForceAlwaysOnTop();
                    else
                        ForceAlwaysOnTopNextTick = false;
                }

                if (PreloadNextImageStart)
                    PreloadNextImage();
            }
        }

        private void Redraw()
        {
            // Clear screen
            if (!BackgroundsForImagesWithTransparency)
                Window.Clear(new Color(0, 0, 0, 0));
            else
                Window.Clear(BackgroundColour);
            // Display Image
            Window.Draw(Image);
            // Update the window
            Window.Display();
        }
        private void OnWindowClosed(Object sender, EventArgs e)
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
        //      Controls     //
        ///////////////////////

        private void OnMouseMoved(Object sender, MouseMoveEventArgs e)
        {
            MousePos = new Vector2i(e.X, e.Y);

            if (Dragging)
                UnforceAlwaysOnTop();
        }
        private void OnMouseWheelScrolled(Object sender, MouseWheelScrollEventArgs e)
        {
            if (TransparencyMod)
            {
                // Change Image Transparency
                ImageColor = new Color(ImageColor.R, ImageColor.G, ImageColor.B,
                    (byte)Math.Min(Math.Max(ImageColor.A + ((e.Delta > 0 ? 1 : -1) * (255 * (ZoomFaster ? (Config.Setting_ZoomSpeedFast / 100f) : (Config.Setting_ZoomSpeed / 100f)))), 2), 255));
                Image.Color = ImageColor;
                Updated = true;
            }
            else
            {
                // Zooming
                if (e.Delta > 0)
                    Zoom(Math.Min(CurrentZoom + (ZoomFaster ? (Config.Setting_ZoomSpeedFast / 100f) : (Config.Setting_ZoomSpeed / 100f)), ZOOM_MAX), !ZoomAlt);
                else if (e.Delta < 0)
                    Zoom(Math.Max(CurrentZoom - (ZoomFaster ? (Config.Setting_ZoomSpeedFast / 100f) : (Config.Setting_ZoomSpeed / 100f)), ZOOM_MIN), !ZoomAlt);
            }

            AutomaticallyZoomed = false;
            FitToMonitorHeightForced = false;
            FitToMonitorHeight = false;
            FitToMonitorWidth = false;
        }

        private void OnMouseDown(Object sender, MouseButtonEventArgs e) { ControlDown(e.Button); }
        private void OnMouseUp(Object sender, MouseButtonEventArgs e) { ControlUp(e.Button); }
        private void OnKeyDown(Object sender, SFML.Window.KeyEventArgs e) { ControlDown(e.Code); }
        private void OnKeyUp(Object sender, SFML.Window.KeyEventArgs e) { ControlUp(e.Code); }

        private void ControlUp(object code)
        {
            // Close
            if (Config.IsControl(code, Config.Control_Close))
                CloseNextTick = true;

            // Dragging
            if (Config.IsControl(code, Config.Control_Drag))
                Dragging = false;

            // Open Context Menu
            if (Config.IsControl(code, Config.Control_OpenContextMenu))
            {
                ContextMenu.RefreshItems();
                ContextMenu.Show(Window.Position.X + MousePos.X - 1, Window.Position.Y + MousePos.Y - 1);
                ContextMenu.Capture = true;
            }

            // Rotate Image
            if (Config.IsControl(code, Config.Control_RotateClockwise))
                RotateImage((int)Image.Rotation + 90);
            if (Config.IsControl(code, Config.Control_RotateAntiClockwise))
                RotateImage((int)Image.Rotation - 90);

            // Flip Image
            if (Config.IsControl(code, Config.Control_Flip))
                FlipImage();

            // Reset Image
            if (Config.IsControl(code, Config.Control_ResetImage))
                ResetImage();

            // Fit To Monitor Height/Width
            if (Config.IsControl(code, Config.Control_FitToMonitorHeight))
                ToggleFitToMonitor(Config.HEIGHT);
            if (Config.IsControl(code, Config.Control_FitToMonitorWidth))
                ToggleFitToMonitor(Config.WIDTH);
            if (Config.IsControl(code, Config.Control_FitToMonitorAuto))
                ToggleFitToMonitor(Config.AUTO);

            // Animated Image - Pause/Play
            if (Config.IsControl(code, Config.Control_PauseAnimation))
                ToggleAnimation();

            // Next/Prev Image in Folder
            if (!Updated && Config.IsControl(code, Config.Control_PrevImage))
                PrevImage();
            if (!Updated && Config.IsControl(code, Config.Control_NextImage))
                NextImage();

            // Open config.txt
            if (Config.IsControl(code, Config.Control_OpenConfig))
                OpenConfig();
            // Reload Config
            if (Config.IsControl(code, Config.Control_ReloadConfig))
                ReloadConfig();

            // Toggle Settings
            if (Config.IsControl(code, Config.Control_ToggleSmoothing))
                ToggleSmoothing();

            if (Config.IsControl(code, Config.Control_ToggleBackgroundForTransparency))
                ToggleBackground();

            // Toggle Always On Top
            if (Config.IsControl(code, Config.Control_ToggleAlwaysOnTop))
                ToggleAlwaysOnTop();

            // Open File At Location
            if (Config.IsControl(code, Config.Control_OpenAtLocation))
                OpenFileAtLocation();

            // Delete File
            if (Config.IsControl(code, Config.Control_Delete))
                DeleteFile();

            // Copy File
            if (Config.IsControl(code, Config.Control_Copy))
                CopyFile();
            if (Config.IsControl(code, Config.Control_CopyAsImage))
                CopyAsImage();

            if (Config.IsControl(code, Config.Control_OpenDuplicateImage))
                OpenDuplicateWindow();

            if (Config.IsControl(code, Config.Control_RandomImage))
                RandomImage();

            // Toggle Image Transparency
            if (Config.IsControl(code, Config.Control_TransparencyToggle))
            {
                if (ImageColor == Color.White)
                {
                    System.Drawing.Color colour = System.Drawing.ColorTranslator.FromHtml(Config.Setting_TransparencyToggleValue);
                    ImageColor = new Color(colour.R, colour.G, colour.B, colour.A);
                }
                else
                    ImageColor = Color.White;
                Image.Color = ImageColor;
                Updated = true;
            }

            // Custom Actions
            for (int i = 0; i < Config.CustomActionBindings.Count; i++)
            {
                if (Config.IsControl(code, ((Config.CustomActionBindings[i] as dynamic).bindings as List<int>)))
                    DoCustomAction((Config.CustomActions.Where(a => (a as dynamic).name == (Config.CustomActionBindings[i] as dynamic).name).First() as dynamic).func);
            }

            // Zooming - release
            if (Config.IsControl(code, Config.Control_ZoomFaster))
                ZoomFaster = false;
            if (Config.IsControl(code, Config.Control_ZoomAlt))
                ZoomAlt = false;
            if (Config.IsControl(code, Config.Control_DragLimitToMonitorBounds))
                DragLimitToBoundsMod = false;
            if (Config.IsControl(code, Config.Control_FitToMonitorAlt))
                FitToMonitorAlt = false;
            if (Config.IsControl(code, Config.Control_TransparencyHold))
                TransparencyMod = false;

            if ((Keyboard.Key)code == Keyboard.Key.LControl)
                Config.CtrlDown = false;
            if ((Keyboard.Key)code == Keyboard.Key.LShift)
                Config.ShiftDown = false;
            if ((Keyboard.Key)code == Keyboard.Key.LAlt)
                Config.AltDown = false;
            if ((Keyboard.Key)code == Keyboard.Key.RControl)
                Config.RCtrlDown = false;
            if ((Keyboard.Key)code == Keyboard.Key.RShift)
                Config.RShiftDown = false;
            if ((Keyboard.Key)code == Keyboard.Key.RAlt)
                Config.RAltDown = false;
        }
        private void ControlDown(object code)
        {
            // Dragging
            if (Config.IsControl(code, Config.Control_Drag))
            {
                if (!Dragging)
                    DragPos = MousePos;
                Dragging = true;
            }

            // Animated Image Controls
            if (Config.IsControl(code, Config.Control_NextFrame))
                NextFrame();
            if (Config.IsControl(code, Config.Control_PrevFrame))
                PrevFrame();

            // Zooming
            if (Config.IsControl(code, Config.Control_ZoomFaster))
                ZoomFaster = true;
            if (Config.IsControl(code, Config.Control_ZoomAlt))
                ZoomAlt = true;
            if (Config.IsControl(code, Config.Control_DragLimitToMonitorBounds))
                DragLimitToBoundsMod = true;
            if (Config.IsControl(code, Config.Control_FitToMonitorAlt))
                FitToMonitorAlt = true;
            if (Config.IsControl(code, Config.Control_TransparencyHold))
                TransparencyMod = true;

            // Moving
            if (!Dragging)
            {
                if (Config.IsControl(code, Config.Control_MoveLeft))
                {
                    NextWindowPos.X -= ZoomFaster ? Config.Setting_MoveSpeedFast : Config.Setting_MoveSpeed;
                    Window.Position = NextWindowPos;
                }
                else if (Config.IsControl(code, Config.Control_MoveRight))
                {
                    NextWindowPos.X += ZoomFaster ? Config.Setting_MoveSpeedFast : Config.Setting_MoveSpeed;
                    Window.Position = NextWindowPos;
                }
                if (Config.IsControl(code, Config.Control_MoveUp))
                {
                    NextWindowPos.Y -= ZoomFaster ? Config.Setting_MoveSpeedFast : Config.Setting_MoveSpeed;
                    Window.Position = NextWindowPos;
                }
                if (Config.IsControl(code, Config.Control_MoveDown))
                {
                    NextWindowPos.Y += ZoomFaster ? Config.Setting_MoveSpeedFast : Config.Setting_MoveSpeed;
                    Window.Position = NextWindowPos;
                }
            }

            if ((Keyboard.Key)code == Keyboard.Key.LControl)
                Config.CtrlDown = true;
            if ((Keyboard.Key)code == Keyboard.Key.LShift)
                Config.ShiftDown = true;
            if ((Keyboard.Key)code == Keyboard.Key.LAlt)
                Config.AltDown = true;
            if ((Keyboard.Key)code == Keyboard.Key.RControl)
                Config.RCtrlDown = true;
            if ((Keyboard.Key)code == Keyboard.Key.RShift)
                Config.RShiftDown = true;
            if ((Keyboard.Key)code == Keyboard.Key.RAlt)
                Config.RAltDown = true;
        }
     

        ///////////////////////////
        //      Manipulation     //
        ///////////////////////////

        public void NextFrame()
        {
            if (Image is AnimatedImage)
            {
                if (Image.Playing)
                    Image.Stop();
                Image.NextFrame();
                Update();
            }
        }
        public void PrevFrame()
        {
            if (Image is AnimatedImage)
            {
                if (Image.Playing)
                    Image.Stop();
                Image.PrevFrame();
                Update();
            }
        }
        public void ToggleAnimation()
        {
            if (Image is AnimatedImage)
            {
                if (Image.Playing)
                    Image.Stop();
                else
                    Image.Play();
            }
        }

        private void Zoom(float value, bool center = false)
        {
            // Limit zooming to prevent the going past the GPU's max texture size
            if (value > CurrentZoom && (uint)Math.Ceiling(Image.Texture.Size.X * value) >= Texture.MaximumSize)
                value = CurrentZoom;

            IntRect currentBounds = new IntRect();
            if (DragLimitToBoundsMod)
            {
                currentBounds = ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());
                if (value >= CurrentZoom && (Window.Size.X >= currentBounds.Width || Window.Size.Y >= currentBounds.Height))
                    return;
            }

            float originalZoom = CurrentZoom;
            CurrentZoom = value;
            
            Dragging = false;
            UnforceAlwaysOnTop();

            if (center)
            {
                Vector2u newSize;
                if (Image.Rotation == 0 || Image.Rotation == 180)
                    newSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
                else
                    newSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));
                Vector2i difference = new Vector2i((int)newSize.X, (int)newSize.Y) - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                NextWindowSize = newSize;
                NextWindowPos = new Vector2i(Window.Position.X - (difference.X / 2), Window.Position.Y - (difference.Y / 2));
            }
            else
            {
                if (Image.Rotation == 0 || Image.Rotation == 180)
                    NextWindowSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
                else
                    NextWindowSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));
                NextWindowPos = Window.Position;
            }

            if (DragLimitToBoundsMod)
            {
                // limit to monitor bounds
                if (NextWindowSize.X > currentBounds.Width || NextWindowSize.Y > currentBounds.Height)
                {
                    // recalculate zoom size
                    float monitorRatio = (float)currentBounds.Width / currentBounds.Height;
                    float windowRatio = (float)NextWindowSize.X / NextWindowSize.Y;

                    if (windowRatio <= monitorRatio)
                    {
                        // limit to monitor height
                        float r = (float)currentBounds.Height / NextWindowSize.Y;
                        NextWindowSize = new Vector2u((uint)(NextWindowSize.X * r), (uint)currentBounds.Height);
                    }
                    else
                    {
                        // limit to monitor width
                        float r = (float)currentBounds.Width / NextWindowSize.X;
                        NextWindowSize = new Vector2u((uint)currentBounds.Width, (uint)(NextWindowSize.Y * r));
                    }
                    CurrentZoom = (float)NextWindowSize.X / ((Image.Rotation == 0 || Image.Rotation == 180) ? Image.Texture.Size.X : Image.Texture.Size.Y);

                    if (center && CurrentZoom != originalZoom)
                    {
                        Vector2i difference = new Vector2i((int)NextWindowSize.X, (int)NextWindowSize.Y) - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                        NextWindowPos = new Vector2i(Window.Position.X - (difference.X / 2), Window.Position.Y - (difference.Y / 2));
                    }
                    else
                        NextWindowPos = Window.Position;
                }

                if (NextWindowPos.X < currentBounds.Left)
                    NextWindowPos.X = currentBounds.Left;
                else if (NextWindowPos.X > currentBounds.Left + currentBounds.Width - NextWindowSize.X)
                    NextWindowPos.X = currentBounds.Left + currentBounds.Width - (int)NextWindowSize.X;

                if (NextWindowPos.Y < currentBounds.Top)
                    NextWindowPos.Y = currentBounds.Top;
                else if (NextWindowPos.Y > currentBounds.Top + currentBounds.Height - NextWindowSize.Y)
                    NextWindowPos.Y = currentBounds.Top + currentBounds.Height - (int)NextWindowSize.Y;
            }

            Updated = true;
        }

        public void RotateImage(int Rotation, bool aroundCenter = true, bool updateWindowSize = true)
        {
            if (Rotation >= 360)
                Rotation = 0;
            else if (Rotation < 0)
                Rotation = 270;

            Vector2f center = new Vector2f(Window.Position.X + (Window.Size.X / 2), Window.Position.Y + (Window.Size.Y / 2));
            Vector2u WindowSize;

            UnforceAlwaysOnTop();

            switch (Rotation)
            {
                case 90:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                case 270:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                default:
                    Image.Scale = new Vector2f(1f, 1f);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.X * CurrentZoom), (uint)(Image.Texture.Size.Y * CurrentZoom));
                    break;
            }
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y));
            Image.Rotation = Rotation;

            if (updateWindowSize)
                NextWindowSize = WindowSize;
            if (aroundCenter)
                NextWindowPos = new Vector2i((int)center.X - (int)(WindowSize.X / 2), (int)center.Y - (int)(WindowSize.Y / 2));
            else
                NextWindowPos = Window.Position;

            Updated = true;
        }

        public Vector2u CurrentImageSize() { return (Image.Rotation == 0 || Image.Rotation == 180) ? Image.Texture.Size : new Vector2u(Image.Texture.Size.Y, Image.Texture.Size.X); }

        public void FlipImage()
        {
            FlippedX = !FlippedX;
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y));
            Redraw();
        }

        public void ToggleFitToMonitor(int dimension)
        {
            UnforceAlwaysOnTop();

            IntRect bounds;
            if (FitToMonitorAlt)
                bounds = ImageViewerUtils.GetCurrentWorkingArea(Mouse.GetPosition());
            else
                bounds = ImageViewerUtils.GetCurrentBounds(Mouse.GetPosition());

            if (CurrentZoom == 1)
            {
                // Fit to Monitor Height
                if (dimension == Config.AUTO)
                {
                    if (bounds.Height < bounds.Width)
                        dimension = Config.HEIGHT;
                    else
                        dimension = Config.WIDTH;
                }

                if (dimension == Config.HEIGHT)
                {
                    FitToMonitorHeight = true;
                    if (Image.Rotation == 90 || Image.Rotation == 270)
                        Zoom(1 + (((float)bounds.Height - Image.Texture.Size.X) / Image.Texture.Size.X), true);
                    else
                        Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                    NextWindowPos = new Vector2i(NextWindowPos.X, bounds.Top);
                }
                else if (dimension == Config.WIDTH)
                {
                    FitToMonitorWidth = true;
                    if (Image.Rotation == 90 || Image.Rotation == 270)
                        Zoom(1 + (((float)bounds.Width - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                    else
                        Zoom(1 + (((float)bounds.Width - Image.Texture.Size.X) / Image.Texture.Size.X), true);
                    NextWindowPos = new Vector2i(bounds.Left, NextWindowPos.Y);
                }
            }
            else
            {
                // Full Size
                FitToMonitorHeight = false;
                FitToMonitorWidth = false;
                Zoom(1, true);
                NextWindowPos = new Vector2i(NextWindowPos.X < 0 ? 0 : NextWindowPos.X, NextWindowPos.Y < 0 ? 0 : NextWindowPos.Y);
            }


            if (CurrentImageSize().X * CurrentZoom >= bounds.Width)
                NextWindowPos = new Vector2i(bounds.Left, bounds.Top); // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            else if (!FitToMonitorAlt)
                ForceAlwaysOnTopNextTick = true;

            AutomaticallyZoomed = false;
        }

        public void ResetImage()
        {
            Zoom(1f);
            AutomaticallyZoomed = false;
            FlippedX = false;
            RotateImage(DefaultRotation);

            // Force Fit To Monitor Height?
            Vector2i imagePos = new Vector2i((int)NextWindowPos.X + ((int)Image.Texture.Size.X / 2), (int)NextWindowPos.Y + ((int)Image.Texture.Size.Y / 2));
            IntRect currentBounds = ImageViewerUtils.GetCurrentBounds(imagePos);
            if (Config.Setting_LimitImagesToMonitor != Config.NONE)
            {
                // Fit to monitor height/width
                int limit = Config.Setting_LimitImagesToMonitor;

                if (limit == Config.AUTO)
                {
                    if (currentBounds.Height < currentBounds.Width)
                        limit = Config.HEIGHT;
                    else
                        limit = Config.WIDTH;
                }

                if (limit == Config.HEIGHT && Image.Texture.Size.Y > currentBounds.Height)
                {
                    Zoom(1 + (((float)currentBounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                    FitToMonitorHeightForced = true;
                }
                else if (limit == Config.WIDTH && Image.Texture.Size.X > currentBounds.Width)
                {
                    Zoom(1 + (((float)currentBounds.Width - Image.Texture.Size.X) / Image.Texture.Size.X), true);
                    AutomaticallyZoomed = true;
                }
            }
            if (Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_MinImageSize)
            {
                // Reisze images smaller than min size to min size
                AutomaticallyZoomed = true;
                Zoom(Config.Setting_MinImageSize / Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y), true);
            }

            // Center image or place in top-left corner if it's a large/wide image.
            IntRect currentWorkingArea;
            if (!FitToMonitorHeightForced)
                currentWorkingArea = ImageViewerUtils.GetCurrentWorkingArea(imagePos);
            else
                currentWorkingArea = currentBounds;

            if (Config.Setting_PositionLargeWideImagesInCorner && Image.Texture.Size.X * CurrentZoom > Image.Texture.Size.Y * CurrentZoom && Image.Texture.Size.X * CurrentZoom >= currentWorkingArea.Width)
                NextWindowPos = new Vector2i(currentWorkingArea.Left, currentWorkingArea.Top);
            else
                NextWindowPos = new Vector2i(currentWorkingArea.Left + (currentWorkingArea.Width / 2) - ((int)(Image.Texture.Size.X * CurrentZoom) / 2), currentWorkingArea.Top + (currentWorkingArea.Height / 2) - ((int)(Image.Texture.Size.Y * CurrentZoom) / 2));

            // Force Always on Top?
            if (FitToMonitorHeightForced || (Image.Texture.Size.Y >= currentBounds.Height && Image.Texture.Size.X < currentBounds.Width))
                ForceAlwaysOnTopNextTick = true;
        }

        public void ToggleSmoothing()
        {
            if (Image is AnimatedImage)
                Image.Data.Smooth = !Image.Data.Smooth;
            else
                Image.Texture.Smooth = !Image.Texture.Smooth;
            Update();
        }
        public bool Smoothing()
        {
            if (Image is AnimatedImage)
                return Image.Data.Smooth;
            else
                return Image.Texture.Smooth;
        }

        public void ToggleBackground()
        {
            BackgroundsForImagesWithTransparency = !BackgroundsForImagesWithTransparency;
            Update();
        }

        public void ToggleAlwaysOnTop()
        {
            AlwaysOnTop = !AlwaysOnTop;
            AlwaysOnTopForced = false;
            DWM.SetAlwaysOnTop(Window.SystemHandle, AlwaysOnTop);
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

        ///////////////////////////
        //     Image Loading     //
        ///////////////////////////

        private bool LoadImage(string fileName)
        {
            File = fileName;

            string extension = ImageViewerUtils.GetExtension(fileName);

            // Image
            if (extension.Equals("gif"))
            {
                // Animated GIF
                Image = Graphics.GetAnimatedImage(fileName);
            }
            else if (extension.Equals("ico"))
            {
                // Icon
                Image = Graphics.GetSpriteFromIcon(fileName);
            }
            else
            {
                // Other
                dynamic texture = Graphics.GetTexture(fileName);
                if (texture is Texture)
                {
                    texture.Smooth = true;
                    Image = new Sprite(texture);
                }
                else if (texture is DisplayObject)
                    Image = (DisplayObject)texture;
            }

            if (Image?.Texture == null)
                return false;

            Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            DefaultRotation = ImageViewerUtils.GetDefaultRotationFromEXIF(fileName);

            return true;
        }
        private bool ChangeImage(string fileName)
        {
            Dragging = false;
            Vector2u prevSize = Image == null ? new Vector2u() : new Vector2u(Image.Texture.Size.X, Image.Texture.Size.Y);
            float prevRotation = Image == null ? 0 : Image.Rotation;
            int prevDefaultRotation = DefaultRotation;

            IntRect bounds = ImageViewerUtils.GetCurrentBounds(Window.Position +
                (Image == null ? new Vector2i() : new Vector2i((int)(Image.Texture.Size.X * CurrentZoom) / 2, (int)(Image.Texture.Size.Y * CurrentZoom) / 2)));

            // Dispose of previous image
            if (Image != null)
            {
                Image.Dispose();
                Image = null;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }

            // Load new image
            if (!LoadImage(fileName))
                return false;

            SFML.Graphics.View view = new SFML.Graphics.View(Window.DefaultView);
            view.Center = new Vector2f(Image.Texture.Size.X / 2f, Image.Texture.Size.Y / 2f);
            view.Size = new Vector2f(Image.Texture.Size.X, Image.Texture.Size.Y);
            Window.SetView(view);

            // Rotation
            RotateImage(prevRotation == prevDefaultRotation ? DefaultRotation : (int)prevRotation, false, false);
            // Smoothing
            if (Image is AnimatedImage)
                Image.Data.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;
            else
                Image.Texture.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;

            // Color
            if (ImageColor != Color.White)
                Image.Color = ImageColor;

            // Don't keep current zoom value if it wasn't set by user
            if (AutomaticallyZoomed || FitToMonitorHeightForced)
            {
                AutomaticallyZoomed = false;
                FitToMonitorHeightForced = false;
                CurrentZoom = 1;
            }

            bool wasFitToMonitorDimension = FitToMonitorHeightForced;
            if (Config.Setting_LimitImagesToMonitor != Config.NONE)
            {
                // Fit to monitor height/width
                int limit = Config.Setting_LimitImagesToMonitor;

                if (limit == Config.AUTO)
                {
                    if (bounds.Height < bounds.Width)
                        limit = Config.HEIGHT;
                    else
                        limit = Config.WIDTH;
                }

                if (limit == Config.HEIGHT && (FitToMonitorHeight || CurrentImageSize().Y * CurrentZoom > bounds.Height))
                {
                    Zoom(1 + (((float)bounds.Height - CurrentImageSize().Y) / CurrentImageSize().Y), true);

                    bounds = ImageViewerUtils.GetCurrentBounds(NextWindowPos +
                        new Vector2i((int)(CurrentImageSize().X * CurrentZoom) / 2, (int)(CurrentImageSize().Y * CurrentZoom) / 2));
                    NextWindowPos = new Vector2i(NextWindowPos.X, bounds.Top);

                    if (!FitToMonitorHeight)
                        FitToMonitorHeightForced = true;

                    wasFitToMonitorDimension = true;
                }
                else if (limit == Config.WIDTH && CurrentImageSize().X * CurrentZoom > bounds.Width)
                {
                    Zoom(1 + (((float)bounds.Width - CurrentImageSize().X) / CurrentImageSize().X), true);

                    bounds = ImageViewerUtils.GetCurrentBounds(NextWindowPos +
                        new Vector2i((int)(CurrentImageSize().X * CurrentZoom) / 2, (int)(CurrentImageSize().Y * CurrentZoom) / 2));
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
                else if (Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) * CurrentZoom < Config.Setting_MinImageSize)
                {
                    // Reisze images smaller than min size to min size
                    if (Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_MinImageSize)
                    {
                        AutomaticallyZoomed = true;
                        Zoom(Config.Setting_MinImageSize / Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y), true);
                    }
                    else
                        Zoom(1, true);
                }
                else
                    Zoom(CurrentZoom, true);
            }

            bounds = ImageViewerUtils.GetCurrentBounds(NextWindowPos +
                new Vector2i((int)(Image.Texture.Size.X * CurrentZoom) / 2, (int)(Image.Texture.Size.Y * CurrentZoom) / 2));

            // Position Window at top-left if the image is wide (ie: a Desktop Wallpaper / Screenshot)
            // Otherwise, if image is hanging off monitor just center it.
            if (Config.Setting_PositionLargeWideImagesInCorner && CurrentImageSize().X > CurrentImageSize().Y && CurrentImageSize().X * CurrentZoom >= bounds.Width)
                NextWindowPos = new Vector2i(bounds.Left, bounds.Top);
            else if (!prevSize.Equals(Image.Texture.Size) && (NextWindowPos.Y <= bounds.Top ||
                NextWindowPos.X + (Image.Texture.Size.X * CurrentZoom) >= bounds.Left + bounds.Width ||
                NextWindowPos.Y + (Image.Texture.Size.Y * CurrentZoom) >= bounds.Top + bounds.Height))
                NextWindowPos = new Vector2i(bounds.Left + (int)((bounds.Width - (Image.Texture.Size.X * CurrentZoom)) / 2), bounds.Top + (int)((bounds.Height - (Image.Texture.Size.Y * CurrentZoom)) / 2));

            // Force Always On Top Mode (so it's above the task bar) - will only happen if height >= window height
            ForceAlwaysOnTopNextTick = true;

            Window.SetTitle(fileName + " - vimage");
            ContextMenu?.Setup(false);

            return true;
        }

        /// <summary>Loads an image into memory but doesn't set it as the displayed image.</summary>
        private bool PreloadImage(string fileName)
        {
            if (ImageViewerUtils.GetExtension(fileName).Equals("gif"))
            {
                // Animated Image
                Graphics.GetAnimatedImageData(fileName);
            }
            else if (ImageViewerUtils.GetExtension(fileName).Equals("ico"))
            {
                // Icon
                if (Graphics.GetSpriteFromIcon(fileName) == null)
                    return false;
            }
            else 
            {
                // Image
                if (Graphics.GetTexture(fileName) == null)
                    return false;
            }

            return true;
        }
        private void PreloadNextImage()
        {
            if (PreloadingNextImage == 0 || FolderContents.Count == 0)
                return;

            PreloadNextImageStart = false;
            
            bool success = false;
            int pos = FolderPosition;
            do
            {
                if (PreloadingNextImage == 1)
                    pos = pos == FolderContents.Count() - 1 ? 0 : pos + 1;
                else if (PreloadingNextImage == -1)
                    pos = pos == 0 ? FolderContents.Count() - 1 : pos - 1;
                else
                    return;

                success = PreloadImage(FolderContents[pos]);
            }
            while (!success);
            
            PreloadingNextImage = 0;
        }

        public void NextImage()
        {
            GetFolderContents();
            if (FolderContents.Count() <= 1)
                return;

            bool success = false;
            do
            {
                FolderPosition = FolderPosition == FolderContents.Count() - 1 ? 0 : FolderPosition + 1;
                success = ChangeImage(FolderContents[FolderPosition]);
            }
            while (!success);

            Update();

            // Preload next image?
            if (Config.Setting_PreloadNextImage)
            {
                PreloadingNextImage = 1;
                PreloadNextImageStart = true;
            }
        }
        public void PrevImage()
        {
            GetFolderContents();
            if (FolderContents.Count() <= 1)
                return;

            bool success = false;
            do
            {
                FolderPosition = FolderPosition == 0 ? FolderContents.Count() - 1 : FolderPosition - 1;
                success = ChangeImage(FolderContents[FolderPosition]);
            }
            while (!success);

            Update();

            // Preload next image?
            if (Config.Setting_PreloadNextImage)
            {
                PreloadingNextImage = -1;
                PreloadNextImageStart = true;
            }
        }
        public void RandomImage()
        {
            GetFolderContents();
            if (FolderContents.Count() <= 1)
                return;

            bool success = false;
            do { success = ChangeImage(FolderContents[rnd.Next(0, FolderContents.Count)]); }
            while (!success);
        }

        public void ChangeSortBy(SortBy by)
        {
            if (by == SortImagesBy)
                return;
            SortImagesBy = by;

            if (SortImagesBy == SortBy.Name)
                SortImagesByDir = SortDirection.Ascending;
            else
                SortImagesByDir = SortDirection.Descending;

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
            if (FolderContents != null && FolderContents.Count() > 0)
                return;
            string directory = File.Substring(0, File.LastIndexOf("\\"));
            if (!Directory.Exists(directory))
                return;

            string[] contents = Directory.GetFiles(directory);
            contents = Array.FindAll(contents, delegate(string s) { return ImageViewerUtils.IsValidExtension(s, EXTENSIONS); });

            switch (SortImagesBy)
            {
                case SortBy.Name:
                {
                    FolderContents = contents.ToList();
                    FolderContents.Sort(new WindowsFileSorting.NaturalStringComparer());
                    if (SortImagesByDir == SortDirection.Descending)
                        FolderContents.Reverse();
                    break;
                }
                case SortBy.Date:
                {
                    if (SortImagesByDir == SortDirection.Ascending)
                        FolderContents.AddRange(contents.OrderBy(d => ImageViewerUtils.GetDateValueFromEXIF(d)));
                    else
                        FolderContents.AddRange(contents.OrderByDescending(d => ImageViewerUtils.GetDateValueFromEXIF(d)));
                    break;
                }
                case SortBy.DateModified:
                {
                    if (SortImagesByDir == SortDirection.Ascending)
                        FolderContents.AddRange(contents.OrderBy(d => new FileInfo(d).LastWriteTime));
                    else
                        FolderContents.AddRange(contents.OrderByDescending(d => new FileInfo(d).LastWriteTime));
                    break;
                }
                case SortBy.DateCreated:
                {
                    if (SortImagesByDir == SortDirection.Ascending)
                        FolderContents.AddRange(contents.OrderBy(d => new FileInfo(d).CreationTime));
                    else
                        FolderContents.AddRange(contents.OrderByDescending(d => new FileInfo(d).CreationTime));
                    break;
                }
                case SortBy.Size:
                {
                    if (SortImagesByDir == SortDirection.Ascending)
                        FolderContents.AddRange(contents.OrderBy(d => new FileInfo(d).Length));
                    else
                        FolderContents.AddRange(contents.OrderByDescending(d => new FileInfo(d).Length));
                    break;
                }
            }

            FolderPosition = FolderContents.IndexOf(File);
        }

        public void DeleteFile()
        {
            string fileName = File;

            GetFolderContents();
            if (FolderContents.Count == 1)
            {
                Image.Dispose();
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                Window.Close();
            }
            else
            {
                NextImage();
                FolderContents.Clear();
            }

            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(fileName, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
        }
        public void CopyFile()
        {
            Thread thread = new Thread(() =>
            {
                System.Collections.Specialized.StringCollection files = new System.Collections.Specialized.StringCollection();
                files.Add(File);
                Clipboard.SetFileDropList(files);
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        public void CopyAsImage()
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    System.Drawing.Bitmap bitmap;
                    if (File.IndexOf(".ico") == File.Length - 4)
                    {
                        // If .ico - copy largest version
                        System.Drawing.Icon icon = new System.Drawing.Icon(File, 256, 256);
                        bitmap = Graphics.ExtractVistaIcon(icon);
                        if (bitmap == null)
                            bitmap = icon.ToBitmap();
                    }
                    else
                        bitmap = new System.Drawing.Bitmap(File);
                    Clipboard.SetImage(bitmap);
                }
                catch (Exception) { }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
        public void OpenFileAtLocation()
        {
            Process.Start("explorer.exe", "/select, " + File);
        }

        public void OpenDuplicateWindow()
        {
            Process p = new Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.Arguments = "\"" + File + "\"";
            p.Start();
        }

        public void OpenConfig()
        {
            if (Config.Setting_OpenSettingsEXE)
            {
                string vimage_settings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vimage_settings.exe");
                if (System.IO.File.Exists(vimage_settings))
                {
                    Process.Start(vimage_settings);
                    return;
                }
            }

            Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
        }
        public void ReloadConfig()
        {
            Config.Init();
            Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));

            // Update ContextMenu
            ContextMenu.LoadItems(Config.ContextMenu, Config.ContextMenu_Animation, Config.ContextMenu_Animation_InsertAtIndex);
            ContextMenu.Setup(true);

            // Update Background Colour?
            System.Drawing.Color backColour = System.Drawing.ColorTranslator.FromHtml(Config.Setting_BackgroundColour);
            Color newBackColour = new Color(backColour.R, backColour.G, backColour.B, backColour.A);
            if (BackgroundColour != newBackColour)
            {
                BackgroundColour = newBackColour;
                if (BackgroundsForImagesWithTransparency)
                    Update();
            }
        }
        private void OnConfigChanged(object source, FileSystemEventArgs e)
        {
            // Wait a bit for the config file to be unlocked
            Thread.Sleep(500);
            ReloadConfigNextTick = true;
        }

        public void DoCustomAction(string action)
        {
            action = action.Replace("%f", "\"" + File + "\"");
            action = action.Replace("%d", File.Substring(0, File.LastIndexOf('\\') + 1));

            // Split exe and arguments by the first space (regex to exclude the spaces within the quotes of the exe's path)
            Regex rgx = new Regex("(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            string[] s = rgx.Split(action, 2);

            if (s[0].Contains("%"))
                s[0] = Environment.ExpandEnvironmentVariables(s[0]);

            Process.Start(s[0], s[1]);
        }

    }
}
