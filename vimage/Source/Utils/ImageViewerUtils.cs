using System;
using System.Linq;
using SFML.Graphics;
using SFML.System;

namespace vimage
{
    internal class ImageViewerUtils
    {
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
            using var image = new ImageMagick.MagickImage();
            image.Ping(fileName);

            var exif = image.GetExifProfile();
            if (exif is null)
                return 0;

            var orientation = exif.GetValue(ImageMagick.ExifTag.Orientation)?.Value ?? 1;
            return orientation switch
            {
                3 or 4 => 180,
                5 or 6 => 90,
                7 or 8 => 270,
                _ => 0,
            };
        }

        /// <summary>Returns DateTime from EXIF data or the FileInfo is there isn't one</summary>
        public static DateTime GetDateValueFromEXIF(string fileName)
        {
            using var image = new ImageMagick.MagickImage();
            image.Ping(fileName);

            var exif = image.GetExifProfile();
            if (exif is null)
                return new System.IO.FileInfo(fileName).LastWriteTime;

            var dateTime = exif.GetValue(ImageMagick.ExifTag.DateTime);
            if (dateTime == null || string.IsNullOrWhiteSpace(dateTime.Value))
                return new System.IO.FileInfo(fileName).LastWriteTime;

            if (
                DateTime.TryParseExact(
                    dateTime.Value,
                    "yyyy:MM:dd HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var parsed
                )
            )
            {
                return parsed;
            }

            return new System.IO.FileInfo(fileName).LastWriteTime;
        }

        public static bool IsSupportedFileType(string fileName)
        {
            var info = ImageMagick.MagickFormatInfo.Create(fileName);
            if (info is null || !info.SupportsReading)
                return false;
            return info.Format switch
            {
                ImageMagick.MagickFormat.Avi
                or ImageMagick.MagickFormat.Flv
                or ImageMagick.MagickFormat.M2v
                or ImageMagick.MagickFormat.M4v
                or ImageMagick.MagickFormat.Mkv
                or ImageMagick.MagickFormat.Mov
                or ImageMagick.MagickFormat.Mp4
                or ImageMagick.MagickFormat.Mpeg
                or ImageMagick.MagickFormat.Mpg
                or ImageMagick.MagickFormat.Pdf
                or ImageMagick.MagickFormat.Wmv => false,
                _ => true,
            };
        }

        public static bool IsAnimatedImage(string fileName)
        {
            var info = ImageMagick.MagickFormatInfo.Create(fileName);
            if (info is null || !info.SupportsReading)
                return false;
            if (!info.SupportsMultipleFrames)
            {
                if (info.Format == ImageMagick.MagickFormat.Png)
                {
                    // SupportsMultipleFrames is always false for png so check if it's an apng
                    using var collection = new ImageMagick.MagickImageCollection();
                    collection.Ping(
                        fileName,
                        new ImageMagick.MagickReadSettings
                        {
                            Format = ImageMagick.MagickFormat.APng,
                        }
                    );
                    if (collection.Count > 1)
                        return true;
                }
                return false;
            }

            return info.Format switch
            {
                ImageMagick.MagickFormat.Gif
                or ImageMagick.MagickFormat.Gif87
                or ImageMagick.MagickFormat.WebP
                or ImageMagick.MagickFormat.APng
                or ImageMagick.MagickFormat.Mng => true,
                _ => false,
            };
        }
    }
}
