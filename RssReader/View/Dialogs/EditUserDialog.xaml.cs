using System.Windows;
using RssReader.ViewModel;

namespace RssReader.View.Dialogs
{
    /// <summary>
    /// Interaction logic for EditUserDialog.xaml
    /// </summary>
    public partial class EditUserDialog : Window
    {
        public EditUserDialog(UserViewModel dataContext)
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
