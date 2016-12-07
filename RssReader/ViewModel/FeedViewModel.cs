using System;
using System.ComponentModel;
using RssReader.Model;

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

        public bool IsReady => _model.IsReady;

        // Public

        public FeedViewModel(FeedModel model)
        {
            this._model = model;
            model.PropertyChanged += ModelOnPropertyChanged;
        }

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged("IsReady");
        }
    }
}
