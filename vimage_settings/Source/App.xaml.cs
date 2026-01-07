using System;
using System.Windows;
using vimage.Common;

namespace vimage_settings
{
    public partial class App : Application
    {
        private static Config? config;
        public static Config? Config
        {
            get => config;
            set => config = value;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                config = Config.Load(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json")
                );
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "vimage does not have write permissions for the folder it's located in.\nPlease place it somewhere else (or set it to run as admin).",
                    "vimage - Error"
                );
            }
        }
    }
}
