// vimage - http://torrunt.net/vimage
// Corey Zeke Womack (Torrunt) - me@torrunt.net

using System;
using System.Threading;

namespace vimage
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string file = "";
            if (args.Length > 0)
            {
                file = args[0];
                if (!System.IO.File.Exists(file))
                    return;
            }

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                System.Windows.Forms.Application.ThreadException += ApplicationThreadException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            }

            // Extension supported?
            if (file != "" && !ImageViewerUtils.IsValidExtension(file))
            {
                if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
                {
                    System.Windows.Forms.MessageBox.Show(
                        "vimage does not support this file format.",
                        "vimage - Unknown File Format"
                    );
                }
                return;
            }

            var imageViewer = new ImageViewer(file, args);
        }

        private static void CurrentDomainOnUnhandledException(
            object sender,
            UnhandledExceptionEventArgs unhandledExceptionEventArgs
        )
        {
            ReportCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
            Environment.Exit(0);
        }

        private static void ApplicationThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ReportCrash(e.Exception);
        }

        public static void ReportCrash(Exception exception)
        {
            //   var reportCrash = new ReportCrash { ToEmail = "torruntalt@gmail.com" };
            //   reportCrash.Send(exception);
        }
    }
}
