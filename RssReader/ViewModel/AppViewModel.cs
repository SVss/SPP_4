using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml;
using RssReader.Model;
using RssReader.Utils;
using RssReader.View.Dialogs;

namespace RssReader.ViewModel
{
    public class AppViewModel: BaseViewModel
    {
        private readonly AppModel _model = new AppModel();
        private object _selectedUserMain;
        private object _selectedOpenUser;
        private object _selectedUserConfig;

        public object SelectedUserMain
        {
            get { return _selectedUserMain; }
            set
            {
                if (_selectedUserMain != value)
                {
                    _selectedUserMain = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<UserViewModel> OpenedUsersList { get; } =
            new ObservableCollection<UserViewModel>();

        public object SelectedOpenUserDialog
        {
            get { return _selectedOpenUser; }
            set
            {
                if (_selectedOpenUser != value)
                {
                    _selectedOpenUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public object SelectedUsersConfigDialog
        {
            get { return _selectedUserConfig; }
            set
            {
                if (_selectedUserConfig != value)
                {
                    _selectedUserConfig = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<UserViewModel> UsersList { get; } =
            new ObservableCollection<UserViewModel>();
        
        // Commands

            // Main window
        public RelayCommand ExitCommand { get; }

        public RelayCommand ShowOpenUserDialogCommand { get; }
        public RelayCommand CloseUserCommand { get; }
        public RelayCommand ShowUsersConfigDialogCommand { get; }

        public RelayCommand OpenUserCommand { get; }

            // Users Config Dialog
        public RelayCommand AddUserCommand { get; }
        public RelayCommand RemoveUserCommand { get; }
        public RelayCommand EditUserCommand { get; }


        // Public

        public AppViewModel()
        {
            EventsManager.SetMainWindowCancelEventHandler(MainWindowOnClosing);

            // commands

            ExitCommand = new RelayCommand(CloseApplication);

            ShowOpenUserDialogCommand = new RelayCommand(ShowOpenUserDialog);
            CloseUserCommand = new RelayCommand(CloseCurrentUser, CanCloseCurrentUser);
            ShowUsersConfigDialogCommand = new RelayCommand(ShowUsersConfigDialog, CanOpenUsersConfig);

            OpenUserCommand = new RelayCommand(OpenUser, o => SelectedOpenUserDialog != null);

            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser, o => SelectedUsersConfigDialog != null);
            EditUserCommand = new RelayCommand(ShowEditUserDialog, o => SelectedUsersConfigDialog != null);

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
                UsersList.Add(new UserViewModel(user));
            }
        }
        

        // Internals

        private void MainWindowOnClosing(object sender, CancelEventArgs e)
        {
            foreach (var user in OpenedUsersList)
            {
                user.Close();
            }

            OpenedUsersList.Clear();

            XmlDocument doc = _model.SaveUsersToXml();
            doc.Save(ConfigConsts.ConfigPath);
        }

        private void CloseApplication(object args)
        {
            Application.Current.Shutdown();
        }

        private void CloseCurrentUser(object obj)
        {
            CloseUser(SelectedUserMain);
        }

        private void CloseUser(object user)
        {
            var u = (user as UserViewModel);
            if (u != null)
            {
                u.Close();
                OpenedUsersList.Remove(u);
                if (u == SelectedUserMain)
                {
                    if (OpenedUsersList.Count > 0)
                    {
                        SelectedUserMain = OpenedUsersList.First();
                    }
                    else
                    {
                        SelectedUserMain = null;
                    }
                }
            }
        }

        private void ShowOpenUserDialog(object obj)
        {
            var dialog = new OpenUserDialog(this);
            dialog.ShowDialog();
        }


        private void OpenUser(object o)
        {
            var u = SelectedOpenUserDialog as UserViewModel;
            if (u != null && !OpenedUsersList.Contains(u))
            {
                OpenedUsersList.Add(u);
                u.Open();
            }
            SelectedUserMain = SelectedOpenUserDialog;
        }


        private void ShowUsersConfigDialog(object obj)
        {
            var dialog = new UsersConfigDialog(this);
            dialog.ShowDialog();
        }

        private bool CanOpenUsersConfig(object o)
        {
            bool result = true;
            foreach (UserViewModel user in OpenedUsersList)
            {
                result &= user.IsReady;
            }
            return result;
        }

        private bool CanCloseCurrentUser(object o)
        {
            return (SelectedUserMain != null) &&
                (SelectedUserMain as UserViewModel).IsReady;
        }

        // Users Config Dialog

        private void AddUser(object args)
        {
            // TODO
            MessageBox.Show("Add user dialog.");
        }

        private void RemoveUser(object args)
        {
            var answ = MessageBox.Show("Do you really want to remove selected user?", "Remove user", MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (answ == MessageBoxResult.Yes)
            {
                CloseUser(SelectedUsersConfigDialog);
                var userToRemove = SelectedUsersConfigDialog as UserViewModel;
                UsersList.Remove(userToRemove);
                _model.RemoveUser(userToRemove.GetModel());
                SelectedUsersConfigDialog = null;
            }
        }

        private void ShowEditUserDialog(object obj)
        {
            var dialog = new EditUserDialog(SelectedUsersConfigDialog as UserViewModel);
            dialog.ShowDialog();
        }
    }
}
