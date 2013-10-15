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
        public readonly float ZOOM_SPEED = 0.025f;
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
        private float CurrentZoom = 1;
        private bool FlippedX = false;
        private bool FlippedY = false;

        private bool MoveWindowOnRotate = false;

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

                Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
                Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            }
            else
            {
                // Image
                Texture texture = Graphics.GetTexture(File);

                if (texture == null)
                    return;

                texture.Smooth = true;
                Image = new Sprite(texture);
                Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
                Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            }

            //MoveWindowOnRotate = Image.Texture.Size.X != Image.Texture.Size.Y;
            
            // Settings
            //ContextSettings Settings = new ContextSettings();
            //Settings.AntialiasingLevel = 6;

            // Create Window
            Window = new RenderWindow(new VideoMode(Image.Texture.Size.X, Image.Texture.Size.Y), "vimage", Styles.None);
            Window.SetActive();

            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND(false);
            bb.dwFlags = DWM_BB.Enable;
            bb.fEnable = true;
            bb.hRgnBlur = new IntPtr();
            DWM.DwmEnableBlurBehindWindow(Window.SystemHandle, ref bb);

            // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            if (Image.Texture.Size.X >= VideoMode.DesktopMode.Width)
                Window.Position = new Vector2i(0, 0);

            Redraw();
            
            // Interaction
            Window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseDown);
            Window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseUp);
            Window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(OnMouseWheelMoved);
            Window.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyDown);

            // Loop
            Stopwatch clock = new Stopwatch();
            clock.Start();

            while (Window.IsOpen())
            {
                if (CloseNextTick)
                    break;

                // Process events
                Window.DispatchEvents();

                if (Image is AnimatedImage)
                {
                    bool imageUpdated = Image.Update((float)clock.Elapsed.TotalMilliseconds);
                    if (!Updated && imageUpdated)
                        Updated = true;
                }
                clock.Restart();
                
                if (LeftMouseDown)
                    Window.Position = new Vector2i(Mouse.GetPosition().X - DragPos.X, Mouse.GetPosition().Y - DragPos.Y);

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

        private void OnMouseWheelMoved(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Zoom(CurrentZoom + ZOOM_SPEED);
            else if (e.Delta < 0 && CurrentZoom > ZOOM_MIN)
                Zoom(CurrentZoom - ZOOM_SPEED);
        }

        private void Zoom(float value)
        {
            CurrentZoom = value;
            //Console.Write((uint)(Image.Texture.Size.X * value) + "\n");

            if (Image.Rotation == 0 || Image.Rotation == 180)
                Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
            else
                Window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));

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
                    if (MoveWindowOnRotate)
                        Window.Position = new Vector2i(Window.Position.X + ((int)Image.Texture.Size.Y / 2), Window.Position.Y - ((int)Image.Texture.Size.Y / 2));
                    break;
                case 270:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    Window.Size = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    if (MoveWindowOnRotate)
                        Window.Position = new Vector2i(Window.Position.X + ((int)Image.Texture.Size.Y / 2), Window.Position.Y - ((int)Image.Texture.Size.Y / 2));
                    break;
                default:
                    Image.Scale = new Vector2f(1f, 1f);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2) + (Rotation == 180 ? 1 : 0));
                    Window.Size = new Vector2u((uint)(Image.Texture.Size.X * CurrentZoom), (uint)(Image.Texture.Size.Y * CurrentZoom));
                    if (MoveWindowOnRotate && Image.Rotation != 180 && Image.Rotation != 0)
                        Window.Position = new Vector2i(Window.Position.X - ((int)Image.Texture.Size.Y / 2), Window.Position.Y + ((int)Image.Texture.Size.Y / 2));
                    break;
            }
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
            Image.Rotation = Rotation;

            //Console.Write(Image.Rotation + " " + Image.Scale + "\n");
            Updated = true;
        }

        private void OnKeyDown(Object sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                // Close
                case Keyboard.Key.Escape:
                    CloseNextTick = true;
                    break;
                // Rotate Image
                case Keyboard.Key.Up:
                    RotateImage((int)Image.Rotation + 90);
                    break;
                case Keyboard.Key.Down:
                    RotateImage((int)Image.Rotation - 90);
                    break;

                // Flip Image
                case Keyboard.Key.F:
                    FlippedX = !FlippedX;
                    Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
                    Redraw();
                    break;
                case Keyboard.Key.G:
                    FlippedY = !FlippedY;
                    Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (FlippedX ? -1 : 1), Math.Abs(Image.Scale.Y) * (FlippedY ? -1 : 1));
                    Redraw();
                    break;

                // Next/Prev Image in Folder
                case Keyboard.Key.Left:
                case Keyboard.Key.PageUp:
                    GetFolderContents();
                    do
                    {
                        try
                        {
                            FolderPosition = FolderPosition == 0 ? FolderContents.Count() - 1 : FolderPosition - 1;
                            ChangeImage(FolderContents[FolderPosition]);
                        }
                        catch { continue; }
                        break;
                    }
                    while (true);
                    break;
                case Keyboard.Key.Right:
                case Keyboard.Key.PageDown:
                    GetFolderContents();
                    do
                    {
                        try
                        {
                            FolderPosition = FolderPosition == FolderContents.Count() - 1 ? 0 : FolderPosition + 1;
                            ChangeImage(FolderContents[FolderPosition]);
                        }
                        catch { continue; }
                        break;
                    }
                    while (true);
                    break;

                // Toggle Settings
                case Keyboard.Key.S:
                    if (Image is AnimatedImage)
                        Image.Data.Smooth = !Image.Data.Smooth;
                    else
                        Image.Texture.Smooth = !Image.Texture.Smooth;
                    Updated = true;
                    break;
            }
        }

        private void GetFolderContents()
        {
            if (FolderContents != null && FolderContents.Count() > 0)
                return;

            FolderContents = Directory.GetFiles( File.Substring(0, File.LastIndexOf("\\")) );
            FolderPosition = Array.IndexOf(FolderContents, File);
        }

        private void ChangeImage(string fileName)
        {
            float prevRotation = Image.Rotation;

            Image.Dispose();

            if (Graphics.NumberOfFramesInImage(fileName) > 1)
            {
                // Animated Image
                Image = Graphics.GetAnimatedImage(fileName);

                if (Image.Texture == null)
                    return;

                Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
                Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            }
            else
            {
                // Image
                Texture texture = Graphics.GetTexture(fileName);

                if (texture == null)
                    return;

                texture.Smooth = true;
                Image = new Sprite(texture);
                Image.Origin = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
                Image.Position = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            }

            View view = new View(Window.DefaultView);
            view.Center = new Vector2f(Image.Texture.Size.X / 2, Image.Texture.Size.Y / 2);
            view.Size = new Vector2f(Image.Texture.Size.X, Image.Texture.Size.Y);
            Window.SetView(view);

            RotateImage((int)prevRotation);
            Zoom(CurrentZoom);

            // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            if (Image.Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                Window.Position = new Vector2i(0, 0);
        }

    }
}
