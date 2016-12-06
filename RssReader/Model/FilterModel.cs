using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;

namespace RssReader.Model
{
    public abstract class FilterModel
    {
        public ObservableCollection<string> AndList { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> OrList { get; } = new ObservableCollection<string>();

        public FilterType Type { get; protected set; }

        // Public

        public virtual bool Check(string s)
        {
            return false;
        }

        public static FilterModel FromXmlElement(XmlElement xe)
        {
            throw new NotImplementedException();
        }

        public XmlElement ToXmlElement()
        {
            throw new NotImplementedException();
        }

        // Protected

        protected FilterModel()
        {

        }

        protected bool CheckAndList(string s)
        {
            bool result = true;

            int i = 0;
            while (result && (i < AndList.Count))
            {
                result = s.Contains(AndList[i]);
                ++i;
            }

            return result;
        }

        protected bool CheckOrList(string s)
        {
            bool result = false;

            int i = 0;
            while (!result && (i < OrList.Count))
            {
                result = s.Contains(OrList[i]);
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

        private IncludeFilterModel()
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

        private ExcludeFilterModel()
        {
            this.Type = FilterType.Exclude;
        }

    }

    public enum FilterType
    {
        Include = 0,
        Exclude
    }
}
