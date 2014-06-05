using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SFML.Window;
using SFML.Graphics;
using Tao.OpenGl;
using DevIL.Unmanaged;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace vimage
{
    class ImageViewer
    {
        public const string VERSION_NAME = "vimage version 6";

        public readonly string[] EXTENSIONS =
        {
            "bmp", "cut", "dds", "doom", "exr", "hdr", "gif", "ico", "jp2", "jpg", "jpeg", "lbm", "mdl", "mng",
            "pal", "pbm", "pcd", "pcx", "pgm", "pic", "png", "ppm", "psd", "psp", "raw", "sgi", "tga", "tif"
        };

        public readonly float ZOOM_SPEED = 0.02f;
        public readonly float ZOOM_SPEED_FAST = 0.1f;
        public readonly float ZOOM_MIN = 0.05f;
        public readonly float ZOOM_MAX = 75f;
        public uint ZOOM_MAX_WIDTH;

        public RenderWindow Window;
        public dynamic Image;
        public string File;
        public List<string> FolderContents = new List<string>();
        public int FolderPosition = 0;
        private ContextMenu ContextMenu;

        public Config Config;

        private bool Updated = false;
        public bool CloseNextTick = false;

        /// <summary>
        /// Instead of setting the Window Position directly when the image is going to be Updated, this is set.
        /// This prevents the old image being shown at the new image location for a split-second before the new image is loaded.
        /// </summary>
        private Vector2i NextWindowPos = new Vector2i();
        private bool Dragging = false;
        private Vector2i DragPos = new Vector2i();
        private Vector2i MousePos = new Vector2i();
        private bool ZoomAlt = false;
        private bool ZoomFaster = false;
        private float CurrentZoom = 1;
        private bool AutomaticallyZoomed = false;
        private int DefaultRotation = 0;
        public bool FlippedX = false;
        public bool FitToMonitorHeight = false;
        private bool FitToMonitorHeightForced = false;
        /// <summary>If true will resize to bounds height instead of working area height</summary>
        private bool FitToMonitorHeightAlternative = false;
        public bool BackgroundsForImagesWithTransparency = false;
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

        public ImageViewer(string file)
        {
            IL.Initialize();

            // Extension supported?
            if (!ImageViewerUtils.IsValidExtension(file, EXTENSIONS))
                return;

            // Save Mouse Position -> will open image at this position
            Vector2i mousePos = Mouse.GetPosition();

            // Get Image
            LoadImage(file);
            
            // Load Config File
            Config = new Config();
            Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
            
            // Create Context Menu
            ContextMenu = new ContextMenu(this);
            ContextMenu.LoadItems(Config.ContextMenu, Config.ContextMenu_Animation, Config.ContextMenu_Animation_InsertAtIndex);
            ContextMenu.Setup(false);

            // Create Window
            Window = new RenderWindow(new VideoMode(Image.Texture.Size.X, Image.Texture.Size.Y), File + " - vimage", Styles.None);
            Window.SetActive();

            ZOOM_MAX_WIDTH = (uint)Math.Ceiling(VideoMode.DesktopMode.Width * 2.5);

            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND(false);
            bb.dwFlags = DWM_BB.Enable;
            bb.fEnable = true;
            bb.hRgnBlur = new IntPtr();
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

            bool _forceAlwaysOnTop = false;

            // Resize Window
            if (Config.Setting_PositionLargeWideImagesInCorner && Image.Texture.Size.X > Image.Texture.Size.Y && Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
            {
                // Position Window at 0,0 if the image is wide (ie: a Desktop Wallpaper / Screenshot)
                Window.Position = new Vector2i(0, 0);
            }
            else
            {
                // Get Bounds
                IntRect bounds;
                if (Config.Setting_OpenAtMousePosition)
                    bounds = ImageViewerUtils.GetCurrentBounds(mousePos);
                else
                    bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);

                // Force Fit To Monitor Height?
                if (Config.Setting_LimitImagesToMonitorHeight && Image.Texture.Size.Y > bounds.Height)
                {
                    // Fit to monitor height if it's higher than monitor height.
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                    FitToMonitorHeightForced = true;
                }
                else if (Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_MinImageSize)
                {
                    // Reisze images smaller than min size to min size
                    AutomaticallyZoomed = true;
                    Zoom(Config.Setting_MinImageSize / Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y), true);
                }


                // Open At Mouse Position?
                if (Config.Setting_OpenAtMousePosition)
                {
                    Vector2i winPos = new Vector2i(mousePos.X - (int)(Window.Size.X / 2), mousePos.Y - (int)(Window.Size.Y / 2));
                    if (!FitToMonitorHeightForced)
                    {
                        if (winPos.Y < bounds.Top)
                            winPos.Y = 0;
                        else if (winPos.Y + Window.Size.Y > bounds.Height)
                            winPos.Y = bounds.Height - (int)Window.Size.Y;
                    }
                    else
                        winPos.Y = bounds.Top;

                    if (winPos.X < bounds.Left)
                        winPos.X = bounds.Left;
                    else if (winPos.X + Window.Size.X > bounds.Left + bounds.Width)
                        winPos.X = bounds.Left + bounds.Width - (int)Window.Size.X;

                    Window.Position = winPos;
                }

                // Force Always On Top Mode (so it's above the task bar)
                if (FitToMonitorHeightForced || (Image.Texture.Size.Y >= bounds.Height && Image.Texture.Size.X < bounds.Width))
                    _forceAlwaysOnTop = true;
            }

            // Defaults
                // Rotation (some images have a rotation set in their exif data)
            RotateImage(DefaultRotation, false);
                // Smoothing
            if (Image is AnimatedImage)
                Image.Data.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;
            else
                Image.Texture.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;
                // Backgrounds For Images With Transparency
            BackgroundsForImagesWithTransparency = Config.Setting_BackgroundForImagesWithTransparencyDefault;

            ForceAlwaysOnTopNextTick = _forceAlwaysOnTop;

            Redraw();
            NextWindowPos = Window.Position;
            
            // Interaction
            Window.Closed += OnWindowClosed;
            Window.MouseButtonPressed += OnMouseDown;
            Window.MouseButtonReleased += OnMouseUp;
            Window.MouseWheelMoved += OnMouseWheelMoved;
            Window.MouseMoved += OnMouseMoved;
            Window.KeyReleased += OnKeyUp;
            Window.KeyPressed += OnKeyDown;

            // Loop
            Stopwatch clock = new Stopwatch();
            clock.Start();
            
            while (Window.IsOpen())
            {
                // Add in some idle time to not thrash the CPU
                Thread.Sleep(1);

                if (CloseNextTick)
                    break;

                // Process events
                Window.DispatchEvents();
                
                // Animated Image?
                if (Image is AnimatedImage)
                {
                    bool imageUpdated = Image.Update((float)clock.Elapsed.TotalMilliseconds);
                    if (!Updated && imageUpdated)
                        Update();
                }
                clock.Restart();
                
                // Drag Window
                if (Dragging)
                    Window.Position = new Vector2i(Mouse.GetPosition().X - DragPos.X, Mouse.GetPosition().Y - DragPos.Y);

                // Update
                if (Updated)
                {
                    Updated = false;
                    Redraw();
                    Window.Position = NextWindowPos;
                }

                if (ForceAlwaysOnTopNextTick)
                {
                    IntRect bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);
                    if (Window.Size.Y >= bounds.Height && Window.Size.X < bounds.Width)
                        ForceAlwaysOnTop();
                    else
                        ForceAlwaysOnTopNextTick = false;
                }

                if (Updated && PreloadNextImageStart)
                    PreloadNextImage();
            }
        }

        private void Redraw()
        {
            // Clear screen
            if (!BackgroundsForImagesWithTransparency)
            {
                Window.Clear(new Color(0, 0, 0, 0));
                Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            }
            else
                Window.Clear(new Color(230, 230, 230));
            // Display Image
            Window.Draw(Image);
            // Update the window
            Window.Display();
        }
        private void OnWindowClosed(Object sender, EventArgs e)
        {
            Window.Close();
        }
        /// <summary>Sets Updated status and refreshes NextWindowPos.</summary>
        private void Update()
        {
            Updated = true;
            NextWindowPos = Window.Position; // Refresh the NextWindowPos var just in case the thing that induced the update didn't change the window position
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
        private void OnMouseWheelMoved(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Zoom(Math.Min(CurrentZoom + (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED), ZOOM_MAX), !ZoomAlt);
            else if (e.Delta < 0)
                Zoom(Math.Max(CurrentZoom - (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED), ZOOM_MIN), !ZoomAlt);

            AutomaticallyZoomed = false;
            FitToMonitorHeightForced = false;
            FitToMonitorHeight = false;
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

            // Fit To Monitor Height
            if (Config.IsControl(code, Config.Control_FitToMonitorHeight))
                ToggleFitToMonitorHeight();

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
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "config.txt");
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

            if (Config.IsControl(code, Config.Control_OpenDuplicateImage))
            {
                Process p = new Process();
                p.StartInfo.FileName = Application.ExecutablePath;
                p.StartInfo.Arguments = "\"" + File + "\"";
                p.Start();
            }

            ZoomFaster = false;
            ZoomAlt = false;
            FitToMonitorHeightAlternative = false;
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

            // Fit To Monitor Height Alternative
            if (Config.IsControl(code, Config.Control_FitToMonitorHeightAlternative))
                FitToMonitorHeightAlternative = true;
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
            // Limit Zooming at 2.5x the screen width (ZOOM_MAX_WIDTH) if it hasn't already reached 75x (ZOOM_MAX)
            if (value > CurrentZoom && (uint)Math.Ceiling(Image.Texture.Size.X * value) >= ZOOM_MAX_WIDTH)
                value = CurrentZoom;

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
                Window.Size = newSize;

                Vector2i difference = new Vector2i((int)newSize.X, (int)newSize.Y) - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                NextWindowPos = new Vector2i(Window.Position.X - (difference.X / 2), Window.Position.Y - (difference.Y / 2));
            }
            else
            {
                if (Image.Rotation == 0 || Image.Rotation == 180)
                    Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
                else
                    Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));
                NextWindowPos = Window.Position;
            }

            Updated = true;
        }

        public void RotateImage(int Rotation, bool aroundCenter = true)
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
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2) + 1, (Image.Texture.Size.Y / 2));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                case 270:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                default:
                    Image.Scale = new Vector2f(1f, 1f);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2) + (Rotation == 180 ? 1 : 0));
                    WindowSize = new Vector2u((uint)(Image.Texture.Size.X * CurrentZoom), (uint)(Image.Texture.Size.Y * CurrentZoom));
                    break;
            }
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y));
            Image.Rotation = Rotation;

            Window.Size = WindowSize;
            if (aroundCenter)
                NextWindowPos = new Vector2i((int)center.X - (int)(WindowSize.X / 2), (int)center.Y - (int)(WindowSize.Y / 2));
            else
                NextWindowPos = Window.Position;

            Updated = true;
        }

        public void FlipImage()
        {
            FlippedX = !FlippedX;
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y));
            Redraw();
        }

        public void ToggleFitToMonitorHeight()
        {
            UnforceAlwaysOnTop();

            IntRect bounds;
            if (FitToMonitorHeightAlternative)
                bounds = ImageViewerUtils.GetCurrentWorkingArea(Window.Position);
            else
                bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);

            if (CurrentZoom == 1)
            {
                // Fit to Monitor Height
                FitToMonitorHeight = true;
                if (Image.Rotation == 90 || Image.Rotation == 270)
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.X) / Image.Texture.Size.X), true);
                else
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                NextWindowPos = new Vector2i(NextWindowPos.X, 0);
            }
            else
            {
                // Full Size
                FitToMonitorHeight = false;
                Zoom(1, true);
                NextWindowPos = new Vector2i(NextWindowPos.X < 0 ? 0 : NextWindowPos.X, NextWindowPos.Y < 0 ? 0 : NextWindowPos.Y);
            }


            if (Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                NextWindowPos = new Vector2i(0, 0); // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            else if (!FitToMonitorHeightAlternative)
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
            if (Config.Setting_LimitImagesToMonitorHeight && Image.Texture.Size.Y > Image.Texture.Size.X && Image.Texture.Size.Y > currentBounds.Height)
            {
                // Fit to monitor height if it's higher than monitor height.
                Zoom(1 + (((float)currentBounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                FitToMonitorHeightForced = true;
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
            bool isGif = extension.Equals("gif");
            bool success = false;

            // Image
            if (!isGif)
            {
                Texture texture = Graphics.GetTexture(fileName);
                if (texture != null)
                {
                    success = true;
                    texture.Smooth = true;
                    Image = new Sprite(texture);
                }
                else if (!extension.Equals("ico"))
                    return false;
            }
            // Animated GIF or image that failed to load normally (ie some .icos)
            if (isGif || !success)
            {
                Image = Graphics.GetAnimatedImage(fileName);
                if (Image.Texture == null)
                    return false;
            }
            Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            DefaultRotation = ImageViewerUtils.GetDefaultRotationFromEXIF(fileName);

            return true;
        }
        private bool ChangeImage(string fileName)
        {
            Image.Dispose();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            Dragging = false;
            float prevRotation = Image.Rotation;
            int prevDefaultRotation = DefaultRotation;

            if (AutomaticallyZoomed)
            {
                // don't keep current zoom value if it wasn't set by user
                AutomaticallyZoomed = false;
                CurrentZoom = 1;
            }

            if (!LoadImage(fileName))
                return false;
            
            SFML.Graphics.View view = new SFML.Graphics.View(Window.DefaultView);
            view.Center = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            view.Size = new Vector2f(Image.Texture.Size.X, Image.Texture.Size.Y);
            Window.SetView(view);

            // Rotation
            RotateImage(prevRotation == prevDefaultRotation ? DefaultRotation : (int)prevRotation, false);
            // Smoothing
            if (Image is AnimatedImage)
                Image.Data.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;
            else
                Image.Texture.Smooth = Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) < Config.Setting_SmoothingMinImageSize ? false : Config.Setting_SmoothingDefault;

            IntRect bounds = ImageViewerUtils.GetCurrentBounds(Window.Position);
            if (Config.Setting_LimitImagesToMonitorHeight && (FitToMonitorHeight || (Image.Texture.Size.Y * CurrentZoom >= bounds.Height || (FitToMonitorHeightForced && Image.Texture.Size.Y >= bounds.Height))))
            {
                // Fit to monitor height if it's higher than monitor height (or FitToMonitorHeight is true).
                Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y), true);
                NextWindowPos = new Vector2i(NextWindowPos.X, bounds.Top);
                if (!FitToMonitorHeight)
                    FitToMonitorHeightForced = true;
            }
            else if (FitToMonitorHeightForced)
            {
                Zoom(1, true);
                FitToMonitorHeightForced = false;
            }
            else if (Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y) * CurrentZoom < Config.Setting_MinImageSize)
            {
                // Reisze images smaller than min size to min size
                AutomaticallyZoomed = true;
                Zoom(Config.Setting_MinImageSize / Math.Min(Image.Texture.Size.X, Image.Texture.Size.Y), true);
            }
            else
                Zoom(CurrentZoom, true);

            // Position Window at 0,0 if the image is wide (ie: a Desktop Wallpaper / Screenshot)
            if (Config.Setting_PositionLargeWideImagesInCorner && Image.Texture.Size.X > Image.Texture.Size.Y && Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                NextWindowPos = new Vector2i(0, 0);

            // Force Always On Top Mode (so it's above the task bar) - will only happen if height >= window height
            ForceAlwaysOnTopNextTick = true;

            Window.SetTitle(fileName + " - vimage");
            ContextMenu.Setup(false);

            return true;
        }

        /// <summary>Loads an image into memory but doesn't set it as the displayed image.</summary>
        private bool PreloadImage(string fileName)
        {
            if (ImageViewerUtils.GetExtension(fileName).Equals("gif"))
            {
                // Animated Image
                AnimatedImageData image = Graphics.GetAnimatedImageData(fileName);
            }
            else
            {
                // Image
                Texture texture = Graphics.GetTexture(fileName);
                if (texture == null)
                    return false;
            }

            return true;
        }
        private void PreloadNextImage()
        {
            if (PreloadingNextImage == 0)
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

                Gl.glActiveTexture(Gl.GL_TEXTURE1);
                success = PreloadImage(FolderContents[pos]);
                Gl.glActiveTexture(Gl.GL_TEXTURE0);
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

            // Preload next image?
            if (Config.Setting_PreloadNextImage)
            {
                PreloadingNextImage = -1;
                PreloadNextImageStart = true;
            }
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

            string[] contents = Directory.GetFiles(File.Substring(0, File.LastIndexOf("\\")));
            contents = Array.FindAll(contents, delegate(string s) { return ImageViewerUtils.IsValidExtension(s, EXTENSIONS); });

            switch (SortImagesBy)
            {
                case SortBy.Name:
                {
                    // Natural Sorting
                    Func<string, object> convert = str =>
                    {
                        ulong number;
                        bool success = ulong.TryParse(str.Substring(0, Math.Min(str.Length, 19)), out number);
                        // max ulong is 18446744073709551615 (20 chars)
                        if (success)
                            return number;
                        else
                            return str;
                    };
                    if (SortImagesByDir == SortDirection.Ascending)
                    {
                        FolderContents.AddRange(contents.OrderBy(
                            str => Regex.Split(str.Replace(" ", ""), "([0-9]+)").Select(convert),
                            new EnumerableComparer<object>()));
                    }
                    else
                    {
                        FolderContents.AddRange(contents.OrderByDescending(
                            str => Regex.Split(str.Replace(" ", ""), "([0-9]+)").Select(convert),
                            new EnumerableComparer<object>()));
                    }

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
        public void OpenFileAtLocation()
        {
            Process.Start("explorer.exe", "/select, " + File);
        }

        public void ReloadConfig()
        {
            Config.Init();
            Config.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
            ContextMenu.LoadItems(Config.ContextMenu, Config.ContextMenu_Animation, Config.ContextMenu_Animation_InsertAtIndex);
            ContextMenu.Setup(true);
        }

    }

    enum SortBy { Name, DateModified, DateCreated, Size }
    enum SortDirection { Ascending, Descending }
}
