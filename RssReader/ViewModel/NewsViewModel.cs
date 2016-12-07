using System;
using System.Windows;
using RssReader.Model;

namespace RssReader.ViewModel
{
    public class NewsViewModel: BaseViewModel
    {
        private const int ShortCaptionWidth = 103;

        private readonly NewsModel _model;

        public string Caption => _model.Caption;
        public string Description => _model.Description;
        public string Link => _model.Link.ToString();

        public string FullText => Caption + Environment.NewLine + Description;

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

        public bool IsVisible { get; set; } = true; // changed by Filters

        // Public

        public NewsViewModel(NewsModel model)
        {
            this._model = model;
        }

        // Internals

        private void OpenInBrowser(object args)
        {
            MessageBox.Show("Open in browser!");
        }
    }
}
