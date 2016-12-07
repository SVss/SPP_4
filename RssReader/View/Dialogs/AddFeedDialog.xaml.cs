using System.Windows;
using RssReader.ViewModel;

namespace RssReader.View.Dialogs
{
    /// <summary>
    /// Interaction logic for AddFeedDialog.xaml
    /// </summary>
    public partial class AddFeedDialog : Window
    {
        public AddFeedDialog(FeedViewModel dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void AddClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
