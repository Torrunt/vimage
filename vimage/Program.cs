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

namespace vimage
{
    class Program
    {
        public static readonly float ZOOM_SPEED = 0.025f;
        public static readonly float ZOOM_MIN = 0.05f;
        

        public static RenderWindow window;
        public static Texture Texture;
        public static Sprite Image;
        public static string File;
        public static string[] FolderContents;
        public static int FolderPosition = 0;

        private static bool Updated = false;
        private static bool CloseNextTick = false;

        private static Vector2i DragPos = new Vector2i();
        private static float CurrentZoom = 1;
        private static bool Flipped = false;

        private static bool MoveWindowOnRotate = false;

        

        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                //File = "G:\\Misc\\Desktop Backgrounds\\0diHF.jpg"; // For quick debugging
                return;
            }

            Il.ilInit();

            // Get Image
            if (args.Count() != 0)
                File = args[0];
            
            Texture = Graphics.GetTexture(File);
            Texture.Smooth = true;
            Image = new Sprite(Texture);
            Image.Origin = new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2);
            Image.Position = new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2);

            //MoveWindowOnRotate = Image.Texture.Size.X != Image.Texture.Size.Y;
            
            // Settings
            //ContextSettings Settings = new ContextSettings();
            //Settings.AntialiasingLevel = 6;

            // Create Window
            window = new RenderWindow(new VideoMode(Texture.Size.X, Texture.Size.Y), "vimage", Styles.None);
            window.SetActive();

            // Make Window Transparent (can only tell if image being viewed has transparency)
            DWM_BLURBEHIND bb = new DWM_BLURBEHIND(false);
            bb.dwFlags = DWM_BB.Enable;
            bb.fEnable = true;
            bb.hRgnBlur = new IntPtr();
            DWM.DwmEnableBlurBehindWindow(window.SystemHandle, ref bb);

            // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            if (Texture.Size.X >= VideoMode.DesktopMode.Width)
                window.Position = new Vector2i(0, 0);

            Redraw();
            
            // Interaction
            window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseDown);
            window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseUp);
            window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
            window.MouseWheelMoved += new EventHandler<MouseWheelEventArgs>(OnMouseWheelMoved);
            window.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyDown);

            while (window.IsOpen())
            {
                if (CloseNextTick)
                    break;

                // Process events
                window.DispatchEvents();

                if (Updated)
                {
                    Updated = false;
                    Redraw();
                }
            }
        }
        static void Redraw()
        {
            // Clear screen
            Gl.glClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            // Display Image
            window.Draw(Image);
            // Update the window
            window.Display();
        }

        static void OnMouseDown(Object sender, MouseButtonEventArgs e)
        {
            window.SetVisible(true);
            if (e.Button.Equals(Mouse.Button.Left))
                DragPos = new Vector2i(e.X, e.Y);
        }
        static void OnMouseUp(Object sender, MouseButtonEventArgs e)
        {
            if (e.Button.Equals(Mouse.Button.Right))
                CloseNextTick = true;
            else if (e.Button.Equals(Mouse.Button.Middle))
            {
                if (CurrentZoom == 1)
                {
                    // Fit to Monitor Height
                    window.Position = new Vector2i(window.Position.X, 0);
                    Zoom(1 + (((float)VideoMode.DesktopMode.Height - Texture.Size.Y) / Texture.Size.Y));
                }
                else
                {
                    // Full Size
                    Zoom(1);
                }
            }
        }
        static void OnMouseMoved(Object sender, MouseMoveEventArgs e)
        {
            if (Mouse.IsButtonPressed(Mouse.Button.Left))
                window.Position = new Vector2i(Mouse.GetPosition().X - DragPos.X, Mouse.GetPosition().Y - DragPos.Y);
        }

        static void OnMouseWheelMoved(Object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                Zoom(CurrentZoom + ZOOM_SPEED);
            else if (e.Delta < 0 && CurrentZoom > ZOOM_MIN)
                Zoom(CurrentZoom - ZOOM_SPEED);
        }

        static void Zoom(float value)
        {
            CurrentZoom = value;
            //Console.Write((uint)(Image.Texture.Size.X * value) + "\n");

            if (Image.Rotation == 0 || Image.Rotation == 180)
                window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom));
            else
                window.Size = new Vector2u((uint)Math.Ceiling(Image.Texture.Size.Y * CurrentZoom), (uint)Math.Ceiling(Image.Texture.Size.X * CurrentZoom));

            Updated = true;
        }

        static void RotateImage(int Rotation)
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
                    window.Size = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    if (MoveWindowOnRotate)
                        window.Position = new Vector2i(window.Position.X + ((int)Image.Texture.Size.Y / 2), window.Position.Y - ((int)Image.Texture.Size.Y / 2));
                    break;
                case 270:
                    Image.Scale = new Vector2f((float)Image.Texture.Size.Y / (float)Image.Texture.Size.X, (float)Image.Texture.Size.X / (float)Image.Texture.Size.Y);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2));
                    window.Size = new Vector2u((uint)(Image.Texture.Size.Y * CurrentZoom), (uint)(Image.Texture.Size.X * CurrentZoom));
                    if (MoveWindowOnRotate)
                        window.Position = new Vector2i(window.Position.X + ((int)Image.Texture.Size.Y / 2), window.Position.Y - ((int)Image.Texture.Size.Y / 2));
                    break;
                default:
                    Image.Scale = new Vector2f(1f, 1f);
                    Image.Position = new Vector2f((Image.Texture.Size.X / 2), (Image.Texture.Size.Y / 2) + (Rotation == 180 ? 1 : 0));
                    window.Size = new Vector2u((uint)(Image.Texture.Size.X * CurrentZoom), (uint)(Image.Texture.Size.Y * CurrentZoom));
                    if (MoveWindowOnRotate && Image.Rotation != 180 && Image.Rotation != 0)
                        window.Position = new Vector2i(window.Position.X - ((int)Image.Texture.Size.Y / 2), window.Position.Y + ((int)Image.Texture.Size.Y / 2));
                    break;
            }
            Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (Flipped ? -1 : 1), Image.Scale.Y);
            Image.Rotation = Rotation;

            //Console.Write(Image.Rotation + " " + Image.Scale + "\n");
            Updated = true;
        }

        static void OnKeyDown(Object sender, KeyEventArgs e)
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
                case Keyboard.Key.Slash:
                    Flipped = !Flipped;
                    Image.Scale = new Vector2f(Math.Abs(Image.Scale.X) * (Flipped ? -1 : 1), Image.Scale.Y);
                    Redraw();
                    break;

                // Next/Prev Image in Folder
                case Keyboard.Key.Left:
                    getFolderContents();
                    do
                    {
                        try
                        {
                            FolderPosition = FolderPosition == 0 ? FolderContents.Count() - 1 : FolderPosition - 1;
                            changeImage(FolderContents[FolderPosition]);
                        }
                        catch { continue; }
                        break;
                    }
                    while (true);
                    break;
                case Keyboard.Key.Right:
                    getFolderContents();
                    do
                    {
                        try
                        {
                            FolderPosition = FolderPosition == FolderContents.Count() - 1 ? 0 : FolderPosition + 1;
                            changeImage(FolderContents[FolderPosition]);
                        }
                        catch { continue; }
                        break;
                    }
                    while (true);
                    break;

                // Toggle Settings
                case Keyboard.Key.S:
                    Texture.Smooth = !Texture.Smooth;
                    Updated = true;
                    break;
            }
        }

        static void getFolderContents()
        {
            if (FolderContents != null && FolderContents.Count() > 0)
                return;

            FolderContents = Directory.GetFiles( File.Substring(0, File.LastIndexOf("\\")) );
            FolderPosition = Array.IndexOf(FolderContents, File);
        }

        static void changeImage(string fileName)
        {
            float prevRotation = Image.Rotation;

            Image.Dispose();
            Texture = Graphics.GetTexture(fileName);
            Texture.Smooth = true;

            Image = new Sprite(Texture);
            Image.Origin = new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2);
            Image.Position = new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2);

            View view = new View(window.DefaultView);
            view.Center = new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2);
            view.Size = new Vector2f(Texture.Size.X, Texture.Size.Y);
            window.SetView(view);

            RotateImage((int)prevRotation);
            Zoom(CurrentZoom);

            // Position Window at 0,0 if the image is large (ie: a Desktop wallpaper)
            if (Texture.Size.X * CurrentZoom >= VideoMode.DesktopMode.Width)
                window.Position = new Vector2i(0, 0);
        }
    }

}
