using System;
using System.Collections.Generic;
using System.Xml;
using RssReader.Model;

namespace RssReader.Utils
{
    public static class RssFetcher
    {
        public static IList<NewsModel> FetchNews(Uri linkPath)
        {
            XmlTextReader reader = new XmlTextReader(linkPath.ToString());
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            var result = new List<NewsModel>();

            var xe = doc.FirstChild;
            if (xe is XmlDeclaration)
                xe = xe.NextSibling;

            if ((xe == null) || (xe.Name != RssConstants.RssTag))
            {
                return result;
            }

            xe = xe.FirstChild;
            if ((xe == null) || (xe.Name != RssConstants.ChannelTag))
            {
                return result;
            }

            foreach (XmlElement itm in xe.ChildNodes)
            {
                string caption = string.Empty,
                        descrition = string.Empty,
                        link = string.Empty;

                if (itm.Name == RssConstants.ItemTag)
                {
                    foreach (XmlElement entry in itm.ChildNodes)
                    {
                        if (entry.Name == RssConstants.TitleTag)
                        {
                            caption = entry.InnerText;
                        }
                        else if (entry.Name == RssConstants.DescriptionTag)
                        {
                            foreach (XmlNode child in entry.ChildNodes)
                            {
                                if (child.NodeType == XmlNodeType.Text ||
                                    child.NodeType == XmlNodeType.CDATA)
                                {
                                    descrition += child.Value;
                                }
                            }
                            descrition = entry.InnerText;
                        }
                        else if (entry.Name == RssConstants.LinkTag)
                        {
                            link = entry.InnerText;
                        }
                    }
                    result.Add(new NewsModel(caption, descrition, link));
                }
            }
            return result;
        }
    }

    internal static class RssConstants
    {
        public static string RssTag => "rss";
        public static string ChannelTag => "channel";
        public static string ItemTag => "item";
        public static string TitleTag => "title";
        public static string LinkTag => "link";
        public static string DescriptionTag => "description";
    }
}
