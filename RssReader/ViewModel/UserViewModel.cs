using System.Collections.ObjectModel;
using RssReader.Model;
using RssReader.Utils;

namespace RssReader.ViewModel
{
    public class UserViewModel: BaseViewModel
    {
        private readonly UserModel _model;

        public ObservableCollection<FeedViewModel> FeedsList { get; private set; } =
            new ObservableCollection<FeedViewModel>();

        public object SelectedFeed { get; set; }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged("Name");
            }
        }

        // Commands

        public RelayCommand UnselectAllFeedsCommand { get; }
        public RelayCommand SelectAllFeedsCommand { get; }
        public RelayCommand SwitchSelectedFeedCommand { get; }

        // Public

        public UserViewModel(UserModel model)
        {
            this._model = model;
            foreach (FeedModel feed in _model.FeedsList)
            {
                this.FeedsList.Add(new FeedViewModel(feed));
            }

            // commands
            UnselectAllFeedsCommand = new RelayCommand(UnselectAllFeeds);
            SelectAllFeedsCommand = new RelayCommand(SelectAllFeeds);
            SwitchSelectedFeedCommand = new RelayCommand(SwitchSelectedFeed);
        }

        // Internals

        // Event handlers
        private void UnselectAllFeeds(object args)
        {
            foreach (FeedViewModel feed in FeedsList)
            {
                feed.IsShown = false;
            }
        }

        private void SelectAllFeeds(object args)
        {
            foreach (FeedViewModel feed in FeedsList)
            {
                feed.IsShown = true;
            }
        }

        private void SwitchSelectedFeed(object args)
        {
            var x = SelectedFeed as FeedViewModel;
            if (x != null)
            {
                x.IsShown = !x.IsShown;
            }
        }

    }
}
