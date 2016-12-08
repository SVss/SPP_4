using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using RssReader.Model;
using RssReader.Utils;
using RssReader.View.Dialogs;

namespace RssReader.ViewModel
{
    public class UserViewModel: BaseViewModel
    {
        private readonly UserModel _model;

        public UserModel GetModel()
        {
            return _model;
        }

        public int ThreadsCount
        {
            get { return _model.ThreadsCount; }
            set
            {
                if (_model.ThreadsCount != value)
                {
                    if (value > 0)
                    {
                        _model.ThreadsCount = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public ObservableCollection<FeedViewModel> FeedsList { get; } =
            new ObservableCollection<FeedViewModel>();

        public ObservableCollection<NewsViewModel> NewsList { get; } =
            new ObservableCollection<NewsViewModel>();

        public FilterViewModel IncludeFilter { get; }
        public FilterViewModel ExcludeFilter { get; }

        private int _shownNewsCount = 0;
        public string ShownNewsCount => _shownNewsCount.ToString();
        public string TotalNewsCount => NewsList.Count.ToString();

        public object SelectedFeed { get; set; }
        public object SelectedNews { get; set; }

        public string Name
        {
            get { return _model.Name; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReady => _model.IsReady;


        // Commands
            // Main Window
        public RelayCommand LoadNewsCommand { get; }

        public RelayCommand AddFeedCommand { get; }
        public RelayCommand RemoveFeedCommand { get; }

        public RelayCommand UnselectAllFeedsCommand { get; }
        public RelayCommand SelectAllFeedsCommand { get; }
        public RelayCommand SwitchSelectedFeedCommand { get; }

            // EditUser Dialog
        public RelayCommand OpenFiltersDialogCommand { get; }

            // AddUser Dialog
        public RelayCommand AddUserCommand { get; }

        // Public

        public UserViewModel(UserModel model)
        {
            this._model = model;
            model.PropertyChanged += ModelOnPropertyChanged;

            foreach (FeedModel feed in _model.FeedsList)
            {
                this.FeedsList.Add(new FeedViewModel(feed));
            }

            IncludeFilter = new FilterViewModel(_model.IncludeFilter);
            ExcludeFilter = new FilterViewModel(_model.ExcludeFilter);

            // AutoFiltering on filters change
            IncludeFilter.AndList.CollectionChanged += FilterNewsList;
            IncludeFilter.OrList.CollectionChanged += FilterNewsList;
            ExcludeFilter.AndList.CollectionChanged += FilterNewsList;
            ExcludeFilter.OrList.CollectionChanged += FilterNewsList;

            NewsList.CollectionChanged += (sender, args) => { OnPropertyChanged("TotalNewsCount"); };
            NewsList.CollectionChanged += FilterNewsList;

            // commands
            LoadNewsCommand = new RelayCommand(LoadNews, CanLoadNews);

            AddFeedCommand = new RelayCommand(AddFeed);
            RemoveFeedCommand = new RelayCommand(RemoveFeed, o => SelectedFeed != null);

            UnselectAllFeedsCommand = new RelayCommand(UnselectAllFeeds);
            SelectAllFeedsCommand = new RelayCommand(SelectAllFeeds);
            SwitchSelectedFeedCommand = new RelayCommand(SwitchSelectedFeed);

            OpenFiltersDialogCommand = new RelayCommand(OpenFiltersDialog);

            AddUserCommand = new RelayCommand(AddUser, CanAddUser);
        }

        public void Open()
        {
            _model.Open();
        }

        public void Close()
        {
            _model.Close();
        }

        // Internals

        private void LoadNews(object obj)
        {
            _model.LoadNews(NewsList);
        }

        private bool CanLoadNews(object o)
        {
            return IsReady;
        }


        private void FilterNewsList(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            _shownNewsCount = 0;
            foreach (NewsViewModel news in NewsList)
            {
                bool r = IncludeFilter.Check(news.FullText) &&
                    ExcludeFilter.Check(news.FullText);

                news.IsVisible = r;
                if (r)
                {
                    ++_shownNewsCount;
                }
            }
            OnPropertyChanged("ShownNewsCount");
        }


        private void AddFeed(object obj)
        {
            var f = new FeedViewModel(new FeedModel("about:blank"));

            var dialog = new AddFeedDialog(f);
            bool? res = dialog.ShowDialog();

            if (!((res == null) || !res.Value))
            {
                FeedsList.Add(f);
                _model.AddFeed(f.GetModel());
            }
        }

        private void RemoveFeed(object obj)
        {
            var f = SelectedFeed as FeedViewModel;
            FeedsList.Remove(f);
            _model.RemoveFeed(f.GetModel());
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

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private void OpenFiltersDialog(object obj)
        {
            var dialog = new FiltersDialog(this);
            dialog.ShowDialog();
        }

        private void AddUser(object obj)
        {
            // nop
        }

        private bool CanAddUser(object arg)
        {
            return (!string.IsNullOrEmpty(Name) && (ThreadsCount > 0));
        }

    }
}
