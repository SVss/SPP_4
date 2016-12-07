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

        public ObservableCollection<NewsViewModel> NewsList =
            new ObservableCollection<NewsViewModel>();

        public object SelectedFeed { get; set; }
        public object SelectedNews { get; set; }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }

        // Commands

        public RelayCommand UpdateNewsCommand { get; }
        public RelayCommand StopUpdateCommand { get; }

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
            UpdateNewsCommand = new RelayCommand(UpdateNews);
            StopUpdateCommand = new RelayCommand(StopUpdate);

            UnselectAllFeedsCommand = new RelayCommand(UnselectAllFeeds);
            SelectAllFeedsCommand = new RelayCommand(SelectAllFeeds);
            SwitchSelectedFeedCommand = new RelayCommand(SwitchSelectedFeed);
        }

        // Internals

        private void StopUpdate(object obj)
        {
            _model.StopUpdate();
        }

        private void UpdateNews(object obj)
        {
            _model.UpdateNews();
        }

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

        public void EndUpdating()
        {
            _model.EndUpdating();
        }
    }
}
