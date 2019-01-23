using System;
using System.Linq;
using SFML.Graphics;
using SFML.System;
using ExifLib;

namespace vimage
{
    class ImageViewerUtils
    {
        /// <summary> Returns the working area IntRect of the monitor the position is located on.</summary>
        public static IntRect GetCurrentWorkingArea(Vector2i pos)
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
        public static IntRect GetCurrentBounds(Vector2i pos, bool returnBackupScreen = true)
        {
            System.Windows.Forms.Screen backupScreen = System.Windows.Forms.Screen.AllScreens.ElementAt(0);

            foreach (System.Windows.Forms.Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (pos.X < screen.Bounds.X || pos.Y < screen.Bounds.Y || pos.X > screen.Bounds.X + screen.Bounds.Width || pos.Y > screen.Bounds.Y + screen.Bounds.Height)
                {
                    if ((pos.X > screen.Bounds.X && screen.Bounds.X > backupScreen.Bounds.X) ||
                        (pos.X < screen.Bounds.X && screen.Bounds.X < backupScreen.Bounds.X) ||
                        (pos.Y > screen.Bounds.Y && screen.Bounds.Y > backupScreen.Bounds.Y) ||
                        (pos.Y < screen.Bounds.Y && screen.Bounds.Y < backupScreen.Bounds.Y))
                        backupScreen = screen;
                    continue;
                }

                return new IntRect(screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height);
            }

            return returnBackupScreen ? new IntRect(backupScreen.Bounds.X, backupScreen.Bounds.Y, backupScreen.Bounds.Width, backupScreen.Bounds.Height) : new IntRect();
        }

        public static Vector2i LimitToBounds(Vector2i pos, Vector2u size, IntRect bounds)
        {
            if (pos.X < bounds.Left)
                pos.X = bounds.Left;
            else if (pos.X > bounds.Left + bounds.Width - size.X)
                pos.X = bounds.Left + bounds.Width - (int)size.X;

            if (pos.Y < bounds.Top)
                pos.Y = bounds.Top;
            else if (pos.Y > bounds.Top + bounds.Height - size.Y)
                pos.Y = bounds.Top + bounds.Height - (int)size.Y;
            return pos;
        }

        public static Vector2i LimitToWindow(Vector2i pos, RenderWindow Window)
        {
            if (pos.X < Window.Position.X)
                pos.X = Window.Position.X;
            else if (pos.X > Window.Position.X + Window.Size.X)
                pos.X = (int)(Window.Position.X + Window.Size.X);
            if (pos.Y < Window.Position.Y)
                pos.Y = Window.Position.Y;
            else if (pos.Y > Window.Position.Y + Window.Size.Y)
                pos.Y = (int)(Window.Position.Y + Window.Size.Y);
            return pos;
        }

        /// <summary>Returns Orientation from the EXIF data of a jpg.</summary>
        public static int GetDefaultRotationFromEXIF(string fileName)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            if (!(extension == ".jpg" || extension == ".jpeg" || extension == ".jpe"))
                return 0;
            ushort orientation = 0;
            try
            {
                using (ExifReader reader = new ExifReader(fileName))
                {
                    if (!reader.GetTagValue(ExifTags.Orientation, out orientation))
                        return 0;

                    switch (orientation)
                    {
                        case 6: return 90;
                        case 3: return 180;
                        case 8: return 270;
                        default: return 0;
                    }
                }
            }
            catch (Exception) { }
            return 0;
        }

        /// <summary>Returns DateTime from EXIF data or the FileInfo is there isn't one</summary>
        public static DateTime GetDateValueFromEXIF(string fileName)
        {
            try
            {
                using (ExifReader reader = new ExifReader(fileName))
                {
                    DateTime date;
                    if (reader.GetTagValue(ExifTags.DateTime, out date))
                        return date;
                }
            }
            catch (Exception) { }

            return new System.IO.FileInfo(fileName).LastWriteTime;
        }

        public static bool IsValidExtension(string fileName, string[] extensions)
        {
            string extension = System.IO.Path.GetExtension(fileName);
            return Array.Exists(extensions, delegate(string s) { return s == extension; });
        }

    }
}
