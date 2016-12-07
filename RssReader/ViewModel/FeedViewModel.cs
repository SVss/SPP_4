using System;
using System.ComponentModel;
using RssReader.Model;
using RssReader.Utils;

namespace RssReader.ViewModel
{
    public class FeedViewModel: BaseViewModel
    {
        private readonly FeedModel _model;

        public bool IsShown
        {
            get
            {
                return _model.IsShown;
            }
            set
            {
                _model.IsShown = value;
                OnPropertyChanged();
            }
        }

        public string Link
        {
            get { return _model.Link.ToString(); }
            set
            {
                Uri result;
                bool res = Uri.TryCreate(value, UriKind.Absolute, out result);
                if (res)
                {
                    _model.Link = result;
                    OnPropertyChanged();
                }
            }
        }

        public FeedStatus Status => _model.Status;

        // Commands
        public RelayCommand AddFeedCommand { get; }

        // Public

        public FeedViewModel(FeedModel model)
        {
            this._model = model;
            model.PropertyChanged += ModelOnPropertyChanged;

            AddFeedCommand = new RelayCommand(AddFeed, CanAddFeed);
        }

        public FeedModel GetModel()
        {
            return _model;
        }

        // Internals

        private void AddFeed(object obj)
        {
            // nop
        }

        private bool CanAddFeed(object arg)
        {
            return !string.IsNullOrEmpty(Link);
        }


        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged("Status");
        }
    }
}
