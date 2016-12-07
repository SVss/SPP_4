using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Xml;
using RssReader.Utils;
using RssReader.ViewModel;
using ThreadPool;

namespace RssReader.Model
{
    public class UserModel
    {
        private ExtThreadPool _userThreadPool;
        private readonly object _sync = new object();
        private readonly SynchronizationContext _context = SynchronizationContext.Current;

        public string Name { get; set; }
        public int ThreadsCount { get; set; }

        public List<FeedModel> FeedsList { get; } = new List<FeedModel>();
        public List<FilterModel> FiltersList { get; private set; } = new List<FilterModel>();

        //public ObservableCollection<NewsModel> NewsList { get; private set; } =
        //    new ObservableCollection<NewsModel>();

        // Public

        public UserModel(string name, int threadsCount)
        {
            this.Name = name;
            this.ThreadsCount = threadsCount;
        }

        public void UpdateNews(ObservableCollection<NewsViewModel> newsList)
        {
            newsList.Clear();
            _userThreadPool.ClearQueue();

            foreach (var feed in FeedsList)
            {
                if (!feed.IsShown)
                    continue;
                
                _userThreadPool.EnqueueTask(() =>
                {
                    IList<NewsModel> result = RssFetcher.FetchNews(feed.Link);

                    // to add items in observable-collection's dispatcher thread:
                    _context.Send((x) =>
                    {
                        foreach (var news in result)
                        {
                            var res = new NewsViewModel(news);

                            bool isShown = true;
                            foreach (FilterModel filter in FiltersList)
                            {
                                isShown &= filter.Check(res.FullText);
                            }
                            if (isShown)
                                newsList.Add(res);
                        }
                    }, null);
                });
            }
        }

        public void EndUpdating()
        {
            _userThreadPool.Dispose();
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

            result._userThreadPool = new ExtThreadPool(threadsCount);

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
