using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using RssReader.Model;
using RssReader.Utils;

namespace RssReader.ViewModel
{
    public class UserViewModel: BaseViewModel
    {
        private readonly UserModel _model;

        public ObservableCollection<FeedViewModel> FeedsList { get; } =
            new ObservableCollection<FeedViewModel>();

        public ObservableCollection<NewsViewModel> NewsList { get; } =
            new ObservableCollection<NewsViewModel>();

        public string NewsCount => NewsList.Count.ToString();

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

        public bool IsReady => _model.IsReady;

        // Commands

        public RelayCommand UpdateNewsCommand { get; }

        public RelayCommand UnselectAllFeedsCommand { get; }
        public RelayCommand SelectAllFeedsCommand { get; }
        public RelayCommand SwitchSelectedFeedCommand { get; }

        // Public

        public UserViewModel(UserModel model)
        {
            this._model = model;
            model.PropertyChanged += ModelOnPropertyChanged;

            foreach (FeedModel feed in _model.FeedsList)
            {
                this.FeedsList.Add(new FeedViewModel(feed));
            }

            NewsList.CollectionChanged += (sender, args) => { OnPropertyChanged("NewsCount"); };

            // commands
            UpdateNewsCommand = new RelayCommand(UpdateNews, CanUpdateNews);

            UnselectAllFeedsCommand = new RelayCommand(UnselectAllFeeds);
            SelectAllFeedsCommand = new RelayCommand(SelectAllFeeds);
            SwitchSelectedFeedCommand = new RelayCommand(SwitchSelectedFeed);
        }

        public void EndUpdating()
        {
            _model.EndUpdating();
        }

        // Internals

        private void UpdateNews(object obj)
        {
            _model.UpdateNews(NewsList);
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

        private bool CanUpdateNews(object o)
        {
            return IsReady;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            CommandManager.InvalidateRequerySuggested();
        }

    }
}
