using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using RssReader.Model;
using RssReader.Utils;

namespace RssReader.ViewModel
{
    public class AppViewModel: BaseViewModel
    {
        private readonly AppModel _model = new AppModel();

        public object SelectedUser { get; set; }
        public ObservableCollection<UserViewModel> UsersList { get; } =
            new ObservableCollection<UserViewModel>();

        public AppViewModel()
        {
            var config = new XmlDocument();
            try
            {
                config.Load(ConfigConsts.ConfigPath);

                _model.LoadUsersFromXml(config);
            }
            catch (Exception ex)
            {
                if (ex is BadXmlException || ex is XmlException ||
                    ex is ArgumentNullException || ex is System.IO.IOException)
                {
                    MessageBox.Show("Error loading configuration.");
                }
                else
                {
                    throw;
                }
            }

            foreach (UserModel user in _model.UsersList)
            {
                this.UsersList.Add(new UserViewModel(user));
            }
        }

    }
}
