using System.Windows;
using RssReader.ViewModel;

namespace RssReader.View.Dialogs
{
    /// <summary>
    /// Interaction logic for FiltersDialog.xaml
    /// </summary>
    public partial class FiltersDialog : Window
    {
        public FiltersDialog(UserViewModel dataContext)
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
