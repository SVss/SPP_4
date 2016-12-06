using System;
using System.Collections.Generic;
using System.Xml;
using RssReader.Utils;

namespace RssReader.Model
{
    public class UserModel
    {
        public string Name { get; private set; }
        public int ThreadsCount { get; set; }

        public List<FeedModel> FeedsList { get; } = new List<FeedModel>();
        public List<FilterModel> FiltersList { get; private set; } = new List<FilterModel>();

        // Public

        public UserModel(string name, int threadsCount)
        {
            this.Name = name;
            this.ThreadsCount = threadsCount;
        }

        public static UserModel FromXmlElement(XmlElement xe)
        {
            if (xe.Name != ConfigConsts.UserTag)
            {
                throw new BadXmlException();
            }

            string name;
            int threadsCount = 0;

            try
            {
                name = xe.Attributes[ConfigConsts.UserNameAttr].Value;
                threadsCount = Convert.ToInt32(xe.Attributes[ConfigConsts.ThreadsCountTag].Value);
            }
            catch (Exception ex)
            {
                if (ex is XmlException || ex is FormatException || ex is OverflowException)
                    throw new BadXmlException();

                throw;
            }
            UserModel result = new UserModel(name, threadsCount);

            // Read feeds
            XmlElement child = xe.FirstChild as XmlElement;
            if ((child == null) || (child.Name != ConfigConsts.ChannelsListTag))
            {
                throw new BadXmlException();
            }

            foreach (XmlElement channel in child.ChildNodes)
            {
                result.FeedsList.Add(FeedModel.FromXmlElement(channel));
            }

            // Read filters
            child = child.NextSibling as XmlElement;
            if ((child == null) || (child.Name != ConfigConsts.FiltersListTag))
            {
                throw new BadXmlException();
            }

            foreach (XmlElement filter in child.ChildNodes)
            {
                result.FiltersList.Add(FilterModel.FromXmlElement(filter));
            }

            return result;
        }

        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(ConfigConsts.UserTag);

            result.SetAttribute(ConfigConsts.UserNameAttr, Name);
            result.SetAttribute(ConfigConsts.ThreadsCountTag, ThreadsCount.ToString());

            // Write feeds
            var child = document.CreateElement(ConfigConsts.ChannelsListTag);
            foreach (FeedModel feed in FeedsList)
            {
                child.AppendChild(feed.ToXmlElement(document));
            }
            result.AppendChild(child);

            // Write filters
            child = document.CreateElement(ConfigConsts.FiltersListTag);
            foreach (FilterModel filter in FiltersList)
            {
                child.AppendChild(filter.ToXmlElement(document));
            }
            result.AppendChild(child);

            return result;
        }
    }
}
