using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SFML.Window;
using SFML.Graphics;
using Tao.OpenGl;
using Tao.DevIl;
using System.Diagnostics;

namespace vimage
{
    class ImageViewer
    {
        public readonly float ZOOM_SPEED = 0.02f;
        public readonly float ZOOM_SPEED_FAST = 0.1f;
        public readonly float ZOOM_MIN = 0.05f;

        public RenderWindow Window;
        public dynamic Image;
        public string File;
        public string[] FolderContents;
        public int FolderPosition = 0;

        private Config Config;

        private bool Updated = false;
        private bool CloseNextTick = false;

        private bool Dragging = false;
        private Vector2i DragPos = new Vector2i();
        private Vector2i MousePos = new Vector2i();
        private bool ZoomInOnCenter = false;
        private bool ZoomFaster = false;
        private float CurrentZoom = 1;
        private bool FlippedX = false;
        private bool FitToMonitorHeight = false;
        private bool FitToMonitorHeightForced = false;
        /// <summary>If true will resize to bounds height instead of working area height</summary>
        private bool FitToMonitorHeightAlternative = false;
        private bool BackgroundsForImagesWithTransparency = false;
        private bool AlwaysOnTop = false;
        private bool AlwaysOnTopForced = false;
        /// <summary>
        /// If true will turn AlwaysOnTop mode on next update if the window height >= monitor height and window width < monitor width.
        /// If the window is wider and taller than the monitor it will automatically be above the task bar anyway.
        /// </summary>
        private bool ForceAlwaysOnTopNextTick = false;

        public ImageViewer(string file)
        {
            Il.ilInit();

            // Save Mouse Position -> will open image at this position
            Vector2i mousePos = Mouse.GetPosition();

            // Get Image
            File = file;

            if (Graphics.NumberOfFramesInImage(File) > 1)
            {
                // Animated Image
                Image = Graphics.GetAnimatedImage(File);
                if (Image.Texture == null)
                    return;
            }
            else
            {
                // Image
                Texture texture = Graphics.GetTexture(File);
                if (texture == null)
                    return;

                texture.Smooth = true;
                Image = new Sprite(texture);
            }
            Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            
            // Load Config File
            Config = new Config();
            Config.Load(AppDomain.CurrentDomain.BaseDirectory + "config.txt");

            // Create Window
            Window = new RenderWindow(new VideoMode(Image.Texture.Size.X, Image.Texture.Size.Y), "vimage", Styles.None);
            Window.SetActive();

            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND(false);
            bb.dwFlags = DWM_BB.Enable;
            bb.fEnable = true;
            bb.hRgnBlur = new IntPtr();
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

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
                    bounds = GetCurrentBounds(mousePos);
                else
                    bounds = GetCurrentBounds(Window.Position);

                // Force Fit To Monitor Height?
                if (Config.Setting_LimitImagesToMonitorHeight && Image.Texture.Size.Y > bounds.Height)
                {
                    // Fit to monitor height if it's higher than monitor height.
                    //Window.Position = new Vector2i(Window.Position.X, 0);
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
                    FitToMonitorHeightForced = true;
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
                    ForceAlwaysOnTop();
            }

            // Defaults
                // Smoothing
            if (Image is AnimatedImage)
                Image.Data.Smooth = Config.Setting_SmoothingDefault;
            else
                Image.Texture.Smooth = Config.Setting_SmoothingDefault;
                // Backgrounds For Images With Transparency
            BackgroundsForImagesWithTransparency = Config.Setting_BackgroundForImagesWithTransparencyDefault;

            Redraw();
            
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
                if (CloseNextTick)
                    break;

                // Process events
                Window.DispatchEvents();
                
                // Animated Image?
                if (Image is AnimatedImage)
                {
                    bool imageUpdated = Image.Update((float)clock.Elapsed.TotalMilliseconds);
                    if (!Updated && imageUpdated)
                        Updated = true;
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
                }

                if (ForceAlwaysOnTopNextTick)
                {
                    IntRect bounds = GetCurrentBounds(Window.Position);
                    if (Window.Size.Y >= bounds.Height && Window.Size.X < bounds.Width)
                        ForceAlwaysOnTop();
                    else
                        ForceAlwaysOnTopNextTick = false;
                }
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

        ////////////////////////
        //      Controls     //
        ///////////////////////
        
        private void OnMouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (Config.IsControl(e.Button, Config.Control_Drag))
                Dragging = true;

            if (Dragging)
                DragPos = new Vector2i(e.X, e.Y);
        }
        private void OnMouseUp(Object sender, MouseButtonEventArgs e)
        {
            // Dragging
            if (Config.IsControl(e.Button, Config.Control_Drag))
                Dragging = false;

            // Close
            if (Config.IsControl(e.Button, Config.Control_Close))
                CloseNextTick = true;

            // Fit To Monitor Height
            if (Config.IsControl(e.Button, Config.Control_FitToMonitorHeight))
                ToggleFitToMonitorHeight();
        }
        private void OnMouseMoved(Object sender, MouseMoveEventArgs e)
        {
            MousePos = new Vector2i(e.X, e.Y);

            if (Dragging)
                UnforceAlwaysOnTop();
        }
        private void OnMouseWheelMoved(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Zoom(CurrentZoom + (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED));
            else if (e.Delta < 0)
                Zoom(Math.Max(CurrentZoom - (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED), ZOOM_MIN));

            FitToMonitorHeightForced = false;
            FitToMonitorHeight = false;
        }

        private void OnKeyUp(Object sender, KeyEventArgs e)
        {
            // Close
            if (Config.IsControl(e.Code, Config.Control_Close))
                CloseNextTick = true;

            // Dragging
            if (Config.IsControl(e.Code, Config.Control_Drag))
                Dragging = false;

            // Rotate Image
            if (Config.IsControl(e.Code, Config.Control_RotateClockwise))
                RotateImage((int)Image.Rotation + 90);
            if (Config.IsControl(e.Code, Config.Control_RotateAntiClockwise))
                RotateImage((int)Image.Rotation - 90);

            // Flip Image
            if (Config.IsControl(e.Code, Config.Control_Flip))
            {
                FlippedX = !FlippedX;
                Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y));
                Redraw();
            }

			// Reset Image
			if(Config.IsControl(e.Code, Config.Control_ResetImage))
			{
				Zoom(1f);
				Image.Scale = new Vector2f(Math.Abs(Image.Scale.X), Math.Abs(Image.Scale.Y));
				RotateImage(0);
			}

            // Fit To Monitor Height
            if (Config.IsControl(e.Code, Config.Control_FitToMonitorHeight))
                ToggleFitToMonitorHeight();

            // Animated Image Controls
            if (Config.IsControl(e.Code, Config.Control_PauseAnimation))
            {
                // Pause/Play
                if (Image is AnimatedImage)
                {
                    if (Image.Playing)
                        Image.Stop();
                    else
                        Image.Play();
                }
            }

            // Next/Prev Image in Folder
            if (Config.IsControl(e.Code, Config.Control_PrevImage))
            {
                GetFolderContents();
                bool success = false;
                do
                {
                    FolderPosition = FolderPosition == 0 ? FolderContents.Count() - 1 : FolderPosition - 1;
                    success = ChangeImage(FolderContents[FolderPosition]);
                }
                while (!success);
            }
            if (Config.IsControl(e.Code, Config.Control_NextImage))
            {
                GetFolderContents();
                bool success = false;
                do
                {
                    FolderPosition = FolderPosition == FolderContents.Count() - 1 ? 0 : FolderPosition + 1;
                    success = ChangeImage(FolderContents[FolderPosition]);
                }
                while (!success);
            }
            
            // Open config.txt
            if (Config.IsControl(e.Code, Config.Control_OpenConfig))
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "config.txt");
            // Reload Config
            if (Config.IsControl(e.Code, Config.Control_ReloadConfig))
            {
                Config.Init();
                Config.Load(AppDomain.CurrentDomain.BaseDirectory + "config.txt");
            }

            // Toggle Settings
            if (Config.IsControl(e.Code, Config.Control_ToggleSmoothing))
            {
                if (Image is AnimatedImage)
                    Image.Data.Smooth = !Image.Data.Smooth;
                else
                    Image.Texture.Smooth = !Image.Texture.Smooth;
                Updated = true;
            }

            if (Config.IsControl(e.Code, Config.Control_ToggleBackgroundForTransparency))
            {
                BackgroundsForImagesWithTransparency = !BackgroundsForImagesWithTransparency;
                Updated = true;
            }

            // Toggle Always On Top
            if (Config.IsControl(e.Code, Config.Control_ToggleAlwaysOnTop))
            {
                AlwaysOnTop = !AlwaysOnTop;
                AlwaysOnTopForced = false;
                DWM.SetAlwaysOnTop(Window.SystemHandle, AlwaysOnTop);
            }


            ZoomFaster = false;
            ZoomInOnCenter = false;
            FitToMonitorHeightAlternative = false;
        }
        private void OnKeyDown(Object sender, KeyEventArgs e)
        {
            // Animated Image Controls
            if (Config.IsControl(e.Code, Config.Control_NextFrame))
            {
                // Next Frame
                if (Image is AnimatedImage)
                {
                    if (Image.Playing)
                        Image.Stop();
                    Image.NextFrame();
                    Updated = true;
                }
            }
            if (Config.IsControl(e.Code, Config.Control_PrevFrame))
            {
                // Prev Frame
                if (Image is AnimatedImage)
                {
                    if (Image.Playing)
                        Image.Stop();
                    Image.PrevFrame();
                    Updated = true;
                }
            }

            // Zooming
            if (Config.IsControl(e.Code, Config.Control_ZoomFaster))
                ZoomFaster = true;
            if (Config.IsControl(e.Code, Config.Control_ZoomInOnCenter))
                ZoomInOnCenter = true;

            // Dragging
            if (Config.IsControl(e.Code, Config.Control_Drag))
            {
                if (!Dragging)
                    DragPos = MousePos;
                Dragging = true;
            }

            // Fit To Monitor Height Alternative
            if (Config.IsControl(e.Code, Config.Control_FitToMonitorHeightAlternative))
                FitToMonitorHeightAlternative = true;
        }

        ///////////////////////////
        //      Manipulation     //
        ///////////////////////////

        private void Zoom(float value)
        {
			if(CurrentZoom < 3)
	            CurrentZoom = value;

            UnforceAlwaysOnTop();

            if (ZoomInOnCenter)
            {
                Vector2u newSize;
                if (Image.Rotation == 0 || Image.Rotation == 180)
                    newSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
                else
                    newSize = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));
                Window.Size = newSize;

                Vector2i difference = new Vector2i((int)newSize.X, (int)newSize.Y) - new Vector2i((int)Window.Size.X, (int)Window.Size.Y);
                Window.Position = new Vector2i(Window.Position.X - (difference.X / 2), Window.Position.Y - (difference.Y / 2));
            }
            else
            {
                if (Image.Rotation == 0 || Image.Rotation == 180)
                    Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
                else
                    Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));
            }

            Updated = true;

			System.Diagnostics.Debug.WriteLine(string.Format("Zoomlock setting is {0}", Config.Setting_ZoomLock), "vimage");
        }

        private void RotateImage(int Rotation, bool aroundCenter = true)
        {
            if (Rotation >= 360)
                Rotation = 0;
            else if (Rotation < 0)
                Rotation = 270;

            Vector2f prev_center = new Vector2f(Window.Position.X + (Window.Size.X / 2), Window.Position.Y + (Window.Size.Y / 2));
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
                Window.Position = new Vector2i((int)prev_center.X - (int)(WindowSize.X / 2), (int)prev_center.Y - (int)(WindowSize.Y / 2));

            Updated = true;
        }

        private void ToggleFitToMonitorHeight()
        {
            UnforceAlwaysOnTop();

            IntRect bounds;
            if (FitToMonitorHeightAlternative)
                bounds = GetCurrentWorkingArea(Window.Position);
            else
                bounds = GetCurrentBounds(Window.Position);

            if (CurrentZoom == 1)
            {
                // Fit to Monitor Height
                FitToMonitorHeight = true;
                Window.Position = new Vector2i(Window.Position.X, 0);
                if (Image.Rotation == 90 || Image.Rotation == 270)
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.X) / Image.Texture.Size.X));
                else
                    Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
            }
            else
            {
                // Full Size
                FitToMonitorHeight = false;
                Zoom(1);
                Window.Position = new Vector2i(Window.Position.X < 0 ? 0 : Window.Position.X, Window.Position.Y < 0 ? 0 : Window.Position.Y);
            }


            if (Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                Window.Position = new Vector2i(0, 0); // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            else if (!FitToMonitorHeightAlternative)
                ForceAlwaysOnTopNextTick = true;
        }

        private bool ChangeImage(string fileName)
        {
            float prevRotation = Image.Rotation;

            Image.Dispose();

            if (Graphics.NumberOfFramesInImage(fileName) > 1)
            {
                // Animated Image
                Image = Graphics.GetAnimatedImage(fileName);
                if (Image.Texture == null)
                    return false;
            }
            else
            {
                // Image
                Texture texture = Graphics.GetTexture(fileName);
                if (texture == null)
                    return false;

                texture.Smooth = true;
                Image = new Sprite(texture);
            }
            Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);

            View view = new View(Window.DefaultView);
            view.Center = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            view.Size = new Vector2f(Image.Texture.Size.X, Image.Texture.Size.Y);
            Window.SetView(view);

            RotateImage((int)prevRotation, false);

            UnforceAlwaysOnTop();

            IntRect bounds = GetCurrentBounds(Window.Position);
            if (Config.Setting_LimitImagesToMonitorHeight && (FitToMonitorHeight || Image.Texture.Size.Y >= bounds.Height))
            {
                // Fit to monitor height if it's higher than monitor height (or FitToMonitorHeight is true).
                Window.Position = new Vector2i(Window.Position.X, bounds.Top);
                Zoom(1 + (((float)bounds.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
                if (!FitToMonitorHeight)
                    FitToMonitorHeightForced = true;
            }
            else if (FitToMonitorHeightForced)
            {
                Zoom(1);
                FitToMonitorHeightForced = false;
            }
            else
                Zoom(CurrentZoom);

            // Position Window at 0,0 if the image is wide (ie: a Desktop Wallpaper / Screenshot)
            if (Config.Setting_PositionLargeWideImagesInCorner && Image.Texture.Size.X > Image.Texture.Size.Y && Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                Window.Position = new Vector2i(0, 0);

            // Force Always On Top Mode (so it's above the task bar) - will only happen if height >= window height
            ForceAlwaysOnTopNextTick = true;

            return true;
        }
        private void GetFolderContents()
        {
            if (FolderContents != null && FolderContents.Count() > 0)
                return;

            FolderContents = Directory.GetFiles(File.Substring(0, File.LastIndexOf("\\")));
            FolderPosition = Array.IndexOf(FolderContents, File);
        }

        private void ForceAlwaysOnTop()
        {
            ForceAlwaysOnTopNextTick = false;
            AlwaysOnTop = true;
            AlwaysOnTopForced = true;
            DWM.SetAlwaysOnTop(Window.SystemHandle);
        }
        /// <summary>Turns Always On Top off if it was forced.</summary>
        private void UnforceAlwaysOnTop()
        {
            ForceAlwaysOnTopNextTick = false;

            if (!AlwaysOnTopForced)
                return;

            AlwaysOnTop = false;
            AlwaysOnTopForced = false;
            DWM.SetAlwaysOnTop(Window.SystemHandle, false);
        }

        ////////////////////
        //      Utils     //
        ////////////////////

        /// <summary> Returns the working area IntRect of the monitor the position is located on.</summary>
        private IntRect GetCurrentWorkingArea(Vector2i pos)
        {
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (pos.X < screen.Bounds.X || pos.Y < screen.Bounds.Y || pos.X > screen.Bounds.X + screen.Bounds.Width || pos.Y > screen.Bounds.Y + screen.Bounds.Height)
                    continue;

                return new IntRect(screen.WorkingArea.X, screen.WorkingArea.Y, screen.WorkingArea.Width, screen.WorkingArea.Height);
            }
            System.Windows.Forms.Screen firstScreen = System.Windows.Forms.Screen.AllScreens.ElementAt(0);

            return new IntRect(firstScreen.WorkingArea.X, firstScreen.WorkingArea.Y, firstScreen.WorkingArea.Width, firstScreen.WorkingArea.Height);
        }
        /// <summary> Returns the bounds IntRect of the monitor the position is located on.</summary>
        private IntRect GetCurrentBounds(Vector2i pos)
        {
            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (pos.X < screen.Bounds.X || pos.Y < screen.Bounds.Y || pos.X > screen.Bounds.X + screen.Bounds.Width || pos.Y > screen.Bounds.Y + screen.Bounds.Height)
                    continue;

                return new IntRect(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
            }
            System.Windows.Forms.Screen firstScreen = System.Windows.Forms.Screen.AllScreens.ElementAt(0);

            return new IntRect(firstScreen.Bounds.X, firstScreen.Bounds.Y, firstScreen.Bounds.Width, firstScreen.Bounds.Height);
        }

    }
}
