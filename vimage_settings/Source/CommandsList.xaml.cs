using System;
using System.Windows;
using System.Windows.Navigation;
using System.Diagnostics;

namespace vimage_settings
{
    /// <summary>
    /// Interaction logic for CommandsList.xaml
    /// </summary>
    public partial class CommandsList : Window
    {
        public CommandsList()
        {
            InitializeComponent();
            SourceInitialized += (s, e) => { MaxHeight = ActualHeight; };
        }
    }
}
