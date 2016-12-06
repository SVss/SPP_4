using System;
using System.Collections.Generic;
using System.Xml;

namespace RssReader.Model
{
    class UserModel
    {
        public string Name { get; private set; }
        public int ThreadsCount { get; set; }

        public List<FeedModel> FeedsList { get; } = new List<FeedModel>();

        public FilterModel IncludFilter = null;
        public FilterModel ExcledeFilter = null;

        // Public

        public UserModel(string name, int threadsCount)
        {
            this.Name = name;
            this.ThreadsCount = threadsCount;
        }

        public static UserModel FromXmlElement(XmlElement xe)
        {
            throw new NotImplementedException();
        }

        public XmlElement ToXmlElement()
        {
            throw new NotImplementedException();
        }
    }
}
