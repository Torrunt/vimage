// vimage - http://torrunt.net/vimage
// Corey Zeke Womack (Torrunt) - me@torrunt.net

using System;

namespace vimage
{
    internal class Program
    {
        public const string SENTRY_DSN = "";

        private static void Main(string[] args)
        {
            string file = "";
            if (args.Length > 0)
            {
                file = args[0];
                if (!System.IO.File.Exists(file))
                    return;
            }

            // Extension supported?
            ImageMagick.MagickImageInfo? imageInfo = null;
            if (file != "")
            {
                try
                {
                    imageInfo = new ImageMagick.MagickImageInfo(
                        file,
                        Utils.ImageViewerUtils.GetDefaultMagickReadSettings()
                    );
                }
                catch (ImageMagick.MagickMissingDelegateErrorException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        "vimage does not support this file format.",
                        "vimage - Unknown File Format"
                    );
                    return;
                }
                catch (ImageMagick.MagickCorruptImageErrorException)
                {
                    System.Windows.Forms.MessageBox.Show(
                        "The file appears to be corrupted and cannot be opened.",
                        "vimage - Corrupted File"
                    );
                    return;
                }
                if (!Utils.ImageViewerUtils.IsSupportedFileType(imageInfo.Format))
                {
                    System.Windows.Forms.MessageBox.Show(
                        "vimage does not support this file format.",
                        "vimage - Unknown File Format"
                    );
                    return;
                }
            }

            // Setup Sentry
            Sentry.SentrySdk.Init(options =>
            {
                options.Dsn = SENTRY_DSN;
                options.IsGlobalModeEnabled = true;
                options.AutoSessionTracking = true;
            });
            if (imageInfo != null)
            {
                Sentry.SentrySdk.ConfigureScope(scope =>
                {
                    scope.Contexts["File"] = new
                    {
                        Path = file,
                        Format = Enum.GetName(imageInfo.Format),
                        imageInfo.Width,
                        imageInfo.Height,
                        imageInfo.Orientation,
                    };
                });
            }

            _ = new ImageViewer(file, args);
        }
    }
}
