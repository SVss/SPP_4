using System.Windows;
using RssReader.ViewModel;

namespace RssReader.View.Dialogs
{
    /// <summary>
    /// Interaction logic for UsersConfigDialog.xaml
    /// </summary>
    public partial class OpenUserDialog : Window
    {
        public OpenUserDialog(AppViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
