using System;
using System.Windows;
using vimage.Common;

namespace vimage_settings
{
    public partial class App : Application
    {
        public static Config? vimageConfig;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            vimageConfig = new Config();
            vimageConfig.Load(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt")
            );
        }
    }
}
