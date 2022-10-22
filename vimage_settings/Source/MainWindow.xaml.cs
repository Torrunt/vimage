using System;
using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.vimageConfig;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ContextMenuEditor.Save();

            App.vimageConfig.Save(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.txt"));
        }
    }
}
