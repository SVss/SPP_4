using System.Collections.ObjectModel;
using System.Xml;
using RssReader.Utils;

namespace RssReader.Model
{
    public abstract class FilterModel
    {
        public ObservableCollection<string> AndList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> OrList { get; } = new ObservableCollection<string>();

        public FilterType Type { get; protected set; }

        // Public

        public abstract bool Check(string s);

        public static FilterModel FromXmlElement(XmlElement xe)
        {
            if (xe == null)
            {
                throw new BadXmlException();
            }

            FilterModel result = null;

            if (xe.Name == ConfigConsts.IncludeFilterTag)
            {
                result = new IncludeFilterModel();
            }
            else if (xe.Name == ConfigConsts.ExcludeFilterTag)
            {
                result = new ExcludeFilterModel();
            }
            else
            {
                throw new BadXmlException();
            }

            foreach (XmlElement method in xe.ChildNodes)
            {
                if (method.Name == ConfigConsts.AndFilterMethodTag)
                {
                    foreach (XmlElement itm in method.ChildNodes)
                    {
                        result.AndList.Add(itm.InnerText);
                    }
                }
                else if (method.Name == ConfigConsts.OrFilterMethodTag)
                {
                    foreach (XmlElement itm in method.ChildNodes)
                    {
                        result.OrList.Add(itm.InnerText);
                    }
                }
                else
                {
                    throw new BadXmlException();
                }
            }

            return result;
        }

        public XmlElement ToXmlElement(XmlDocument document)
        {
            string name = string.Empty;
            if (this.Type == FilterType.Include)
            {
                name = ConfigConsts.IncludeFilterTag;
            }
            else if (this.Type == FilterType.Exclude)
            {
                name = ConfigConsts.ExcludeFilterTag;
            }

            XmlElement result = document.CreateElement(name);

            if (AndList.Count > 0)
            {
                var list = document.CreateElement(ConfigConsts.AndFilterMethodTag);
                foreach (string s in AndList)
                {
                    var itm = document.CreateElement(ConfigConsts.FilterItemTag);
                    itm.InnerText = s;

                    list.AppendChild(itm);
                }
                result.AppendChild(list);
            }

            if (OrList.Count > 0)
            {
                var list = document.CreateElement(ConfigConsts.OrFilterMethodTag);
                foreach (string s in OrList)
                {
                    var itm = document.CreateElement(ConfigConsts.FilterItemTag);
                    itm.InnerText = s;

                    list.AppendChild(itm);
                }
                result.AppendChild(list);
            }

            return result;
        }

        // Protected

        protected FilterModel()
        {

        }

        protected bool CheckAndList(string s)
        {
            if (AndList.Count == 0)
            {
                return Type == FilterType.Include;
            }

            bool result = true;

            int i = 0;
            while (result && (i < AndList.Count))
            {
                result = s.ToUpper().Contains(AndList[i].ToUpper());
                ++i;
            }

            return result;
        }

        protected bool CheckOrList(string s)
        {
            if (OrList.Count == 0)
            {
                return Type == FilterType.Include;
            }

            bool result = false;

            int i = 0;
            while (!result && (i < OrList.Count))
            {
                result = s.ToUpper().Contains(OrList[i].ToUpper());
                ++i;
            }

            return result;
        }
    }

    public class IncludeFilterModel : FilterModel
    {
        // Public

        public override bool Check(string s)
        {
            return CheckAndList(s) && CheckOrList(s);
        }

        // Internals

        protected internal IncludeFilterModel()
        {
            this.Type = FilterType.Include;
        }

    }

    public class ExcludeFilterModel : FilterModel
    {
        // Public

        public override bool Check(string s)
        {
            return !CheckOrList(s) && !CheckAndList(s);
        }

        // Internals

        protected internal ExcludeFilterModel()
        {
            this.Type = FilterType.Exclude;
        }

    }
}
