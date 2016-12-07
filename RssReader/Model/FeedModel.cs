using System;
using System.Collections.Generic;
using System.Xml;
using RssReader.Utils;

namespace RssReader.Model
{
    public class FeedModel
    {
        public Uri Link { get; set; }
        public bool IsShown { get; set; } = true;

        // Public

        public FeedModel(string linkPath)
        {
            Uri result;
            if (!Uri.TryCreate(linkPath, UriKind.Absolute, out result))
            {
                throw new ArgumentException();
            }

            Link = result;
        }

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
    }
}
