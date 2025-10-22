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
            if (file == "")
                return;

            // Extension supported?
            var imageInfo = new ImageMagick.MagickImageInfo(file);
            if (!ImageViewerUtils.IsSupportedFileType(imageInfo.Format))
            {
                System.Windows.Forms.MessageBox.Show(
                    "vimage does not support this file format.",
                    "vimage - Unknown File Format"
                );
                return;
            }

            Sentry.SentrySdk.Init(options =>
            {
                options.Dsn = SENTRY_DSN;
                options.IsGlobalModeEnabled = true;
                options.AutoSessionTracking = true;
            });
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

            _ = new ImageViewer(file, args);
        }
    }
}
