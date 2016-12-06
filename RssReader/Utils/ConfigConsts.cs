using System.Diagnostics;
using System.IO;

namespace RssReader.Utils
{
    internal static class ConfigConsts
    {
        public static string ConfigPath =>
            Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\config.xml";

        public static string RootTag => "root";

        public static string UserTag => "user";
        public static string UserNameAttr => "name";
        public static string ThreadsCountTag => "threads";

        public static string ChannelsListTag => "feeds";
        public static string ChannelTag => "channel";

        public static string FiltersListTag => "filters";
        public static string IncludeFilterTag => "include";
        public static string ExcludeFilterTag => "exclude";
        public static string FilterItemTag => "item";
    }
}
