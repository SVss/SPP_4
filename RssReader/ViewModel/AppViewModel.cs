using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        // Commands

        public RelayCommand ExitCommand { get; }

        public RelayCommand AddUserCommand { get; }
        public RelayCommand RemoveUserCommand { get; }
        public RelayCommand EditUserFiltersCommand { get; }

        // Public

        public AppViewModel()
        {
            EventsManager.SetMainWindowCancelEventHandler(MainWindowOnClosing);

            // commands

            ExitCommand = new RelayCommand(CloseApplication);
            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser);
            EditUserFiltersCommand = new RelayCommand(EditUserFilters);

            // configuration

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
                    MessageBox.Show("Error loading configuration.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
        
        // Internals

        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            foreach (var user in UsersList)
            {
                user.EndUpdating();
            }

            XmlDocument doc = _model.SaveUsersToXml();
            doc.Save(ConfigConsts.ConfigPath);
        }

        private void CloseApplication(object args)
        {
            Application.Current.Shutdown();
        }

        private void AddUser(object args)
        {
            // TODO
            MessageBox.Show("Add user dialog.");
        }

        private void RemoveUser(object args)
        {
            // TODO
            MessageBox.Show("Remove user dialog.");
        }


        private void EditUserFilters(object obj)
        {
            // TODO
            MessageBox.Show("Edit user filters.");
        }
    }
}
