using System;
using System.Linq;
using SFML.Graphics;
using SFML.System;

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
        public static IntRect GetCurrentBounds(Vector2i pos)
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

            return new IntRect(backupScreen.Bounds.X, backupScreen.Bounds.Y, backupScreen.Bounds.Width, backupScreen.Bounds.Height);
        }


        public static string GetExtension(string fileName) { return fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower(); }

        /// <summary>Returns Orientation from the EXIF data of a jpg.</summary>
        public static int GetDefaultRotationFromEXIF(string fileName)
        {
            if (!(GetExtension(fileName).Equals("jpg") || GetExtension(fileName).Equals("jpeg")))
                return 0;
            gma.Drawing.ImageInfo.Info info = new gma.Drawing.ImageInfo.Info(fileName);
            try
            {
                switch (info.Orientation.ToString())
                {
                    case "RightTop": return 90;
                    case "BottomLeft": return 180;
                    case "LeftBottom": return 270;
                    default: return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool IsValidExtension(string fileName, string[] extensions)
        {
            string extension = ImageViewerUtils.GetExtension(fileName);
            return Array.Exists(extensions, delegate(string s) { return s == extension; });
        }

    }
}
