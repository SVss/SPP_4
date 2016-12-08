using System;
using RssReader.Model;
using RssReader.Utils;

namespace RssReader.ViewModel
{
    public class NewsViewModel: BaseViewModel
    {
        private const int ShortCaptionWidth = 53;

        private readonly NewsModel _model;

        public string Caption => _model.Caption;
        public string Description => _model.Description;    // TODO: get rid of image / show it X)
        public string Link => _model.Link.ToString();

        public string FullText => Caption + Environment.NewLine + Environment.NewLine + Description;

        public object ShortCaption
        {
            get
            {
                if (Caption.Length > ShortCaptionWidth)
                    return Caption.Substring(0, ShortCaptionWidth-3) + "...";
                else
                    return Caption;
            }
        }

        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands

        public RelayCommand OpenInBrowserCommand { get; }

        // Public

        public NewsViewModel(NewsModel model)
        {
            this._model = model;
            OpenInBrowserCommand = new RelayCommand(OpenInBrowser);
        }

        // Internals

        private void OpenInBrowser(object args)
        {
            System.Diagnostics.Process.Start(Link.ToString());
        }
    }
}
