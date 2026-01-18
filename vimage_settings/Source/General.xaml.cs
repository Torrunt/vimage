using System.Windows.Controls;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : UserControl
    {
        public General()
        {
            InitializeComponent();
            DataContext = App.Config;
        }
    }
}
