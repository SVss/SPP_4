using System;

namespace RssReader.Model
{
    class NewsModel
    {
        public string Caption { get; private set; }
        public string Description { get; private set; }
        public string Link { get; private set; }

        public string FullText => Caption + Environment.NewLine + Description;

        public NewsModel(string caption, string description, string link)
        {
            this.Caption = caption;
            this.Description = description;
            this.Link = link;
        }
    }
}
