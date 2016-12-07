using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using RssReader.Model;
using RssReader.Utils;

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

        public string NewsCount => NewsList.Count.ToString();

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
        public RelayCommand UpdateNewsCommand { get; }

        public RelayCommand UnselectAllFeedsCommand { get; }
        public RelayCommand SelectAllFeedsCommand { get; }
        public RelayCommand SwitchSelectedFeedCommand { get; }

            // EditUser Dialog
        public RelayCommand OpenFiltersDialogCommand { get; }


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

            OpenFiltersDialogCommand = new RelayCommand(OpenFiltersDialog);
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


        private void OpenFiltersDialog(object obj)
        {
            MessageBox.Show("Filters Dialog here");
        }
    }
}
