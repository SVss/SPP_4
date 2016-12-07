using System.Windows;
using RssReader.Utils;

namespace RssReader.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            EventsManager.SetMainWindow(this);
        }
    }
}
