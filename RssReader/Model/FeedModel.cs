﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using RssReader.Utils;

namespace RssReader.Model
{
    public class FeedModel: INotifyPropertyChanged
    {
        private bool _isReady = true;

        public Uri Link { get; set; }
        public bool IsShown { get; set; } = true;

        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            set
            {
                if (_isReady != value)
                {
                    _isReady = value;
                    OnPropertyChanged();
                }
            }
        }

        // Public

        public static FeedModel FromXmlElement(XmlElement xe)
        {
            if (xe.Name != ConfigConsts.ChannelTag)
            {
                throw new BadXmlException();
            }

            FeedModel result = null;

            string linkPath = xe.InnerText;
            try
            {
                result = new FeedModel(linkPath);
            }
            catch (ArgumentException)
            {
                throw new BadXmlException();
            }

            return result;
        }

        public XmlElement ToXmlElement(XmlDocument document)
        {
            XmlElement result = document.CreateElement(ConfigConsts.ChannelTag);
            result.InnerText = Link.ToString();
            return result;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        // Internals

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private FeedModel(string linkPath)
        {
            Uri result;
            if (!Uri.TryCreate(linkPath, UriKind.Absolute, out result))
            {
                throw new ArgumentException();
            }

            Link = result;
        }
    }
}
