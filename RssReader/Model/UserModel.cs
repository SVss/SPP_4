﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using RssReader.Utils;
using RssReader.ViewModel;
using ThreadPool;

namespace RssReader.Model
{
    public class UserModel: INotifyPropertyChanged
    {
        private ExtThreadPool _userThreadPool = null;
        private readonly object _sync = new object();
        private readonly SynchronizationContext _context = SynchronizationContext.Current;

        private string _name;
        private int _threadsCount;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value != _name)
                    {
                        _name = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public int ThreadsCount
        {
            get { return _threadsCount; }
            set
            {
                if (value != _threadsCount)
                {
                    if (value > 0)
                    {
                        _threadsCount = value;
                        _userThreadPool?.Reinit(_threadsCount);
                        OnPropertyChanged();
                    }
                }
            }
        }

        public List<FeedModel> FeedsList { get; } = new List<FeedModel>();
        public FilterModel IncludeFilter { get; set; }
        public FilterModel ExcludeFilter { get; set; }

        public bool IsReady
        {
            get
            {
                bool result = true;
                for (int i = 0; result && (i < FeedsList.Count); ++i)
                {
                    result &= ((FeedsList[i].Status == FeedStatus.Error) || 
                        (FeedsList[i].Status == FeedStatus.Ready));
                }
                return result;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Public

        public UserModel(string name, int threadsCount)
        {
            this.Name = name;
            this._threadsCount = threadsCount;
            IncludeFilter = new IncludeFilterModel();
            ExcludeFilter = new ExcludeFilterModel();
        }

        public void AddFeed(FeedModel feed)
        {
            FeedsList.Add(feed);
        }

        public void RemoveFeed(FeedModel feed)
        {
            FeedsList.Remove(feed);
        }

        public void LoadNews(ObservableCollection<NewsViewModel> newsList)
        {
            newsList.Clear();
            _userThreadPool.ClearQueue();
            
            foreach (FeedModel feed in FeedsList)
            {
                feed.Status = FeedStatus.Ready;

                if (!feed.IsShown)
                    continue;

                feed.Status = FeedStatus.Waiting;

                _userThreadPool.EnqueueTask(() =>
                {
                    feed.Status = FeedStatus.Loading;
                    IList<NewsModel> result = RssFetcher.FetchNews(feed.Link);

                    if (result == null)
                    {
                        feed.Status = FeedStatus.Error;
                    }
                    else
                    {
                        // to add items in observable-collection's dispatcher thread
                        _context.Send((x) =>
                        {
                            foreach (var news in result)
                            {
                                var res = new NewsViewModel(news);
                                newsList.Add(res);
                            }
                            feed.Status = FeedStatus.Ready;

                        }, null);
                    }
                });
            }

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
                var feedModel = FeedModel.FromXmlElement(channel);
                feedModel.PropertyChanged += result.FeedModelOnPropertyChanged;
                result.FeedsList.Add(feedModel);
            }

            // Read filters
            child = child.NextSibling as XmlElement;
            if ((child == null) || (child.Name != ConfigConsts.FiltersListTag))
            {
                throw new BadXmlException();
            }

            if (child.ChildNodes.Count == 2)
            {
                foreach (XmlElement filter in child.ChildNodes)
                {
                    var nextFilter = FilterModel.FromXmlElement(filter);
                    if (nextFilter is IncludeFilterModel)
                    {
                        result.IncludeFilter = nextFilter;
                    }
                    else if (nextFilter is ExcludeFilterModel)
                    {
                        result.ExcludeFilter = nextFilter;
                    }
                }
            }
            else
            {
                result.IncludeFilter = new IncludeFilterModel();
                result.ExcludeFilter = new ExcludeFilterModel();
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

            child.AppendChild(IncludeFilter.ToXmlElement(document));
            child.AppendChild(ExcludeFilter.ToXmlElement(document));
            
            result.AppendChild(child);

            return result;
        }

        // Internals

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FeedModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnPropertyChanged("IsReady");
        }

        public void Open()
        {
            _userThreadPool = new ExtThreadPool(ThreadsCount);
        }

        public void Close()
        {
            _userThreadPool.Dispose();
            _userThreadPool = null;
        }

    }
}
