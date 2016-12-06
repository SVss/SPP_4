using System;
using RssReader.Model;

namespace RssReader.ViewModel
{
    public class FeedViewModel: BaseViewModel
    {
        private readonly FeedModel _model;
        private bool _isShown = true;

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

        public bool IsShown
        {
            get { return _isShown; }
            set
            {
                _isShown = value;
                OnPropertyChanged();
            }
        }

        // Public

        public FeedViewModel(FeedModel model)
        {
            this._model = model;
        }
    }
}
