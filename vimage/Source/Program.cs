// vimage - http://torrunt.net/vimage
// Corey Zeke Womack (Torrunt) - me@torrunt.net

using System;
using System.Threading;
using CrashReporterDotNET;

namespace vimage
{
    class Program
    {
        static void Main(string[] args)
        {
            string file;
            if (args.Length == 0)
                return;
            else
                file = args[0];
            //file = @"G:\Google Drive\Pictures\Misc\vimage demostration images\Beaver_Transparent.png";

            if (System.IO.File.Exists(file))
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    System.Windows.Forms.Application.ThreadException += ApplicationThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                }

                ImageViewer imageViewer = new ImageViewer(file);
            }
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
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
            var reportCrash = new ReportCrash { ToEmail = "torruntalt@gmail.com" };
            reportCrash.Send(exception);
        }

    }
}
