namespace RssReader.ViewModel
{
    public class StringContainer: BaseViewModel
    {
        private string _value;

        public string Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
