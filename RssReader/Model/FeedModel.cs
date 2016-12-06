using System;
using System.Collections.Generic;
using System.Xml;

namespace RssReader.Model
{
    class FeedModel
    {
        public string Link { get; private set; }
        public List<NewsModel> NewsList { get; private set; } = new List<NewsModel>();

        public FeedModel(string link)
        {
            this.Link = link;
        }

        public static FeedModel FromXmlElement(XmlElement xe)
        {
            throw new NotImplementedException();
        }

        public XmlElement ToXmlElement()
        {
            throw new NotImplementedException();
        }
    }
}
