using System.Collections.ObjectModel;
using RssReader.Model;

namespace RssReader.ViewModel
{
    public class UserViewModel: BaseViewModel
    {
        private readonly UserModel _model;

        public ObservableCollection<FeedViewModel> FeedsList { get; private set; } =
            new ObservableCollection<FeedViewModel>();

        public string Name
        {
            get { return _model.Name; }
            set
            {
                _model.Name = value;
                OnPropertyChanged();
            }
        }

        // Public

        public UserViewModel(UserModel model)
        {
            this._model = model;
            foreach (FeedModel feed in _model.FeedsList)
            {
                this.FeedsList.Add(new FeedViewModel(feed));
            }
        }
    }
}
