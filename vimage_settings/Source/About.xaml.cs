using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _ = Process.Start(
                new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true }
            );
            e.Handled = true;
        }
    }
}
