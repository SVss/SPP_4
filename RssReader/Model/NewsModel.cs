using System;

namespace RssReader.Model
{
    public class NewsModel
    {
        public string Caption { get; private set; }
        public string Description { get; private set; }
        public Uri Link { get; private set; }

        public string FullText => Caption + Environment.NewLine + Description;

        public NewsModel(string caption, string description, string linkPath)
        {
            Uri result;
            if (!Uri.TryCreate(linkPath, UriKind.Absolute, out result))
            {
                result = new Uri("about:blank");
            }
            this.Link = result;

            this.Caption = caption;
            this.Description = description;
        }
    }
}
