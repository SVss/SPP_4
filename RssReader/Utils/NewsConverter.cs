using System;
using System.Globalization;
using System.Windows.Data;
using RssReader.Model;
using RssReader.ViewModel;

namespace RssReader.Utils
{
    class NewsConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var x = value as NewsModel;
            return new NewsViewModel(x);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
