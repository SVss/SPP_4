using System.Windows;
using RssReader.ViewModel;

namespace RssReader.View.Dialogs
{
    /// <summary>
    /// Interaction logic for UsersConfigDialog.xaml
    /// </summary>
    public partial class UsersConfigDialog : Window
    {
        public UsersConfigDialog(AppViewModel dataContext)
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
