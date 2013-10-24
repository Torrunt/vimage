using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private bool Updated = false;
        private bool CloseNextTick = false;

        private bool LeftMouseDown = false;
        private Vector2i DragPos = new Vector2i();
        private Vector2i MousePos = new Vector2i();
        private bool ZoomInOnCenter = false;
        private bool ZoomFaster = false;
        private float CurrentZoom = 1;
        private bool FlippedX = false;
        private bool FlippedY = false;

        public ImageViewer(string file)
        {
            Il.ilInit();

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
            
            // Create Window
            Window = new RenderWindow(new VideoMode(Image.Texture.Size.X, Image.Texture.Size.Y), "vimage", Styles.None);
            Window.SetActive();
            
            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND(false);
            bb.dwFlags = DWM_BB.Enable;
            bb.fEnable = true;
            bb.hRgnBlur = new IntPtr();
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

            if (Image.Texture.Size.X >= VideoMode.DesktopMode.Width)
            {
                // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
                Window.Position = new Vector2i(0, 0);
            }
            else if (Image.Texture.Size.Y > VideoMode.DesktopMode.Height)
            {
                // Fit to monitor height if it's higher than monitor height.
                Window.Position = new Vector2i(Window.Position.X, 0);
                Zoom(1 + (((float)VideoMode.DesktopMode.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
            }

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
                
                // Drag on LeftMouseDown
                if (LeftMouseDown)
                    Window.Position = new Vector2i(Mouse.GetPosition().X - DragPos.X, Mouse.GetPosition().Y - DragPos.Y);

                // Update
                if (Updated)
                {
                    Updated = false;
                    Redraw();
                }
            }
        }
        private void Redraw()
        {
            // Clear screen
            Window.Clear(new Color(0, 0, 0, 0));
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            // Display Image
            Window.Draw(Image);
            // Update the window
            Window.Display();
        }
        private void OnWindowClosed(Object sender, EventArgs e)
        {
            Window.Close();
        }

        private void OnMouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.Button.Equals(Mouse.Button.Left))
                LeftMouseDown = true;

            if (LeftMouseDown)
                DragPos = new Vector2i(e.X, e.Y);
        }
        private void OnMouseUp(Object sender, MouseButtonEventArgs e)
        {
            if (e.Button.Equals(Mouse.Button.Left))
                LeftMouseDown = false;

            if (e.Button.Equals(Mouse.Button.Right))
                CloseNextTick = true;
            else if (e.Button.Equals(Mouse.Button.Middle))
            {
                if (CurrentZoom == 1)
                {
                    // Fit to Monitor Height
                    Window.Position = new Vector2i(Window.Position.X, 0);
                    Zoom(1 + (((float)VideoMode.DesktopMode.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
                }
                else
                {
                    // Full Size
                    Zoom(1);
                    Window.Position = new Vector2i(Window.Position.X < 0 ? 0 : Window.Position.X, Window.Position.Y < 0 ? 0 : Window.Position.Y);
                }
            }
        }
        private void OnMouseMoved(Object sender, MouseMoveEventArgs e)
        {
            MousePos = new Vector2i(e.X, e.Y);
        }

        private void OnMouseWheelMoved(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Zoom(CurrentZoom + (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED));
            else if (e.Delta < 0)
                Zoom(Math.Max(CurrentZoom - (ZoomFaster ? ZOOM_SPEED_FAST : ZOOM_SPEED), ZOOM_MIN));
        }

        private void Zoom(float value)
        {
            CurrentZoom = value;

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
        }

        private void RotateImage(int Rotation)
        {
            if (Rotation >= 360)
                Rotation = 0;
            else if (Rotation < 0)
                Rotation = 270;

            switch (Rotation)
            {
                case 90:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2) + 1, (Image.Texture.Size.Y / 2));
                    Window.Size = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                case 270:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    Window.Size = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    break;
                default:
                    Image.Scale = new Vector2f(1f, 1f);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2) + (Rotation == 180 ? 1 : 0));
                    Window.Size = new Vector2u((uint)(Image.Texture.Size.X * CurrentZoom), (uint)(Image.Texture.Size.Y * CurrentZoom));
                    break;
            }
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
            Image.Rotation = Rotation;

            Updated = true;
        }

        private void OnKeyUp(Object sender, KeyEventArgs e)
        {
            ZoomFaster = false;
            ZoomInOnCenter = false;

            switch (e.Code)
            {
                // Close
                case Keyboard.Key.Escape:
                case Keyboard.Key.Back:
                    CloseNextTick = true;
                    break;

                // Rotate Image
                case Keyboard.Key.Up: RotateImage((int)Image.Rotation + 90); break;
                case Keyboard.Key.Down: RotateImage((int)Image.Rotation - 90); break;

                // Flip Image
                case Keyboard.Key.F:
                {
                    FlippedX = !FlippedX;
                    Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
                    Redraw();
                    break;
                }
                case Keyboard.Key.G:
                {
                    FlippedY = !FlippedY;
                    Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
                    Redraw();
                    break;
                }

                // Animated Image Controls
                case Keyboard.Key.Space:
                {
                    // Pause/Play
                    if (Image is AnimatedImage)
                    {
                        if (Image.Playing)
                            Image.Stop();
                        else
                            Image.Play();
                    }
                    break;
                }

                // Next/Prev Image in Folder
                case Keyboard.Key.Left:
                case Keyboard.Key.PageUp:
                {
                    GetFolderContents();
                    bool success = false;
                    do
                    {
                        FolderPosition = FolderPosition == 0 ? FolderContents.Count() - 1 : FolderPosition - 1;
                        success = ChangeImage(FolderContents[FolderPosition]);
                    }
                    while (!success);
                    break;
                }
                case Keyboard.Key.Right:
                case Keyboard.Key.PageDown:
                {
                    GetFolderContents();
                    bool success = false;
                    do
                    {
                        FolderPosition = FolderPosition == FolderContents.Count() - 1 ? 0 : FolderPosition + 1;
                        success = ChangeImage(FolderContents[FolderPosition]);
                    }
                    while (!success);
                    break;
                }

                // Toggle Settings
                case Keyboard.Key.S:
                {
                    if (Image is AnimatedImage)
                        Image.Data.Smooth = !Image.Data.Smooth;
                    else
                        Image.Texture.Smooth = !Image.Texture.Smooth;
                    Updated = true;
                    break;
                }
            }
        }
        private void OnKeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                // Animated Image Controls
                case Keyboard.Key.Period:
                {
                    // Next Frame
                    if (Image is AnimatedImage)
                    {
                        if (Image.Playing)
                            Image.Stop();
                        Image.NextFrame();
                        Updated = true;
                    }
                    break;
                }
                case Keyboard.Key.Comma:
                {
                    // Prev Frame
                    if (Image is AnimatedImage)
                    {
                        if (Image.Playing)
                            Image.Stop();
                        Image.PrevFrame();
                        Updated = true;
                    }
                    break;
                }

                // Zooming
                case Keyboard.Key.RShift:
                case Keyboard.Key.LShift:
                {
                    ZoomFaster = true;
                    break;
                }
                case Keyboard.Key.RControl:
                case Keyboard.Key.LControl:
                {
                    ZoomInOnCenter = true;
                    break;
                }
            }
        }

        private void GetFolderContents()
        {
            if (FolderContents != null && FolderContents.Count() > 0)
                return;

            FolderContents = Directory.GetFiles(File.Substring(0, File.LastIndexOf("\\")));
            FolderPosition = Array.IndexOf(FolderContents, File);
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

            RotateImage((int)prevRotation);
            Zoom(CurrentZoom);

            if (Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
            {
                // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
                Window.Position = new Vector2i(0, 0);
            }
            else if (Image.Texture.Size.Y * CurrentZoom > VideoMode.DesktopMode.Height)
            {
                // Fit to monitor height if it's higher than monitor height.
                Window.Position = new Vector2i(Window.Position.X, 0);
                Zoom(1 + (((float)VideoMode.DesktopMode.Height - Image.Texture.Size.Y) / Image.Texture.Size.Y));
            }

            return true;
        }

    }
}
