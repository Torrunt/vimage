using System;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace vimage
{
    internal class ImageViewerUtils
    {
        public static readonly string[] EXTENSIONS =
        [
            ".avif",
            ".bmp",
            ".gif",
            ".hdr",
            ".heic",
            ".ico",
            ".jfi",
            ".jfif",
            ".jif",
            ".jiff",
            ".jpe",
            ".jpeg",
            ".jpg",
            ".pic",
            ".png",
            ".psd",
            ".svg",
            ".tga",
            ".webp",
        ];
        public static readonly string[] EXTENSIONS_DEVIL =
        [
            ".avif",
            ".bmp",
            ".cut",
            ".dds",
            ".doom",
            ".exr",
            ".gif",
            ".hdr",
            ".heic",
            ".ico",
            ".jfi",
            ".jfif",
            ".jif",
            ".jiff",
            ".jp2",
            ".jpe",
            ".jpeg",
            ".jpg",
            ".lbm",
            ".mdl",
            ".mng",
            ".pal",
            ".pbm",
            ".pcd",
            ".pcx",
            ".pgm",
            ".pic",
            ".png",
            ".ppm",
            ".psd",
            ".psp",
            ".raw",
            ".sgi",
            ".svg",
            ".tga",
            ".tif",
            ".tiff",
            ".webp",
        ];

        /// <summary> Returns the working area IntRect of the monitor the position is located on.</summary>
        public static IntRect GetCurrentWorkingArea(Vector2i pos)
        {
            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (
                    pos.X < screen.Bounds.X
                    || pos.Y < screen.Bounds.Y
                    || pos.X > screen.Bounds.X + screen.Bounds.Width
                    || pos.Y > screen.Bounds.Y + screen.Bounds.Height
                )
                    continue;

                return new IntRect(
                    screen.WorkingArea.X,
                    screen.WorkingArea.Y,
                    screen.WorkingArea.Width,
                    screen.WorkingArea.Height
                );
            }
            var firstScreen = System.Windows.Forms.Screen.AllScreens.ElementAt(0);

            return new IntRect(
                firstScreen.WorkingArea.X,
                firstScreen.WorkingArea.Y,
                firstScreen.WorkingArea.Width,
                firstScreen.WorkingArea.Height
            );
        }

        /// <summary> Returns the bounds IntRect of the monitor the position is located on.</summary>
        public static IntRect GetCurrentBounds(Vector2i pos, bool returnBackupScreen = true)
        {
            var backupScreen = System.Windows.Forms.Screen.AllScreens.ElementAt(0);

            foreach (var screen in System.Windows.Forms.Screen.AllScreens)
            {
                if (
                    pos.X < screen.Bounds.X
                    || pos.Y < screen.Bounds.Y
                    || pos.X > screen.Bounds.X + screen.Bounds.Width
                    || pos.Y > screen.Bounds.Y + screen.Bounds.Height
                )
                {
                    if (
                        (pos.X > screen.Bounds.X && screen.Bounds.X > backupScreen.Bounds.X)
                        || (pos.X < screen.Bounds.X && screen.Bounds.X < backupScreen.Bounds.X)
                        || (pos.Y > screen.Bounds.Y && screen.Bounds.Y > backupScreen.Bounds.Y)
                        || (pos.Y < screen.Bounds.Y && screen.Bounds.Y < backupScreen.Bounds.Y)
                    )
                        backupScreen = screen;
                    continue;
                }

                return new IntRect(
                    screen.Bounds.X,
                    screen.Bounds.Y,
                    screen.Bounds.Width,
                    screen.Bounds.Height
                );
            }

            return returnBackupScreen
                ? new IntRect(
                    backupScreen.Bounds.X,
                    backupScreen.Bounds.Y,
                    backupScreen.Bounds.Width,
                    backupScreen.Bounds.Height
                )
                : new IntRect();
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
            string extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            if (!(extension == ".jpg" || extension == ".jpeg" || extension == ".jpe"))
                return 0;
            try
            {
                var file = ExifLibrary.ImageFile.FromFile(fileName);
                var orientation = file
                    .Properties.Get<ExifLibrary.ExifEnumProperty<ExifLibrary.Orientation>>(
                        ExifLibrary.ExifTag.Orientation
                    )
                    .Value;

                return orientation switch
                {
                    ExifLibrary.Orientation.Flipped => 0,
                    ExifLibrary.Orientation.FlippedAndRotated180 => 0,
                    ExifLibrary.Orientation.FlippedAndRotatedLeft => 0,
                    ExifLibrary.Orientation.FlippedAndRotatedRight => 0,
                    ExifLibrary.Orientation.Normal => 0,
                    ExifLibrary.Orientation.Rotated180 => 180,
                    ExifLibrary.Orientation.RotatedLeft => 270,
                    ExifLibrary.Orientation.RotatedRight => 90,
                    _ => 0,
                };
            }
            catch (Exception) { }
            return 0;
        }

        /// <summary>Returns DateTime from EXIF data or the FileInfo is there isn't one</summary>
        public static DateTime GetDateValueFromEXIF(string fileName)
        {
            try
            {
                var file = ExifLibrary.ImageFile.FromFile(fileName);
                var dateTime = file.Properties.Get<ExifLibrary.ExifDateTime>(
                    ExifLibrary.ExifTag.DateTime
                );
                return dateTime.Value;
            }
            catch (Exception) { }

            return new System.IO.FileInfo(fileName).LastWriteTime;
        }

        public static bool IsValidExtension(string fileName, bool useDevil = true)
        {
            return (useDevil ? EXTENSIONS_DEVIL : EXTENSIONS).Contains(
                System.IO.Path.GetExtension(fileName).ToLowerInvariant()
            );
        }
    }
}
