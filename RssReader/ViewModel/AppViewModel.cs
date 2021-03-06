﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
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

        private static SaveFileDialog _saveDialog = new SaveFileDialog()
        {
            Title = "Export config...",
            DefaultExt = "xml",
            Filter = "XML-file|*.xml"
        };

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
        public RelayCommand SaveConfigCommand { get; }
        public RelayCommand ExportConfigCommand { get; }

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
            SaveConfigCommand = new RelayCommand(o => SaveConfig(ConfigConsts.ConfigPath));
            ExportConfigCommand = new RelayCommand(o => ExportConfig());

            ShowOpenUserDialogCommand = new RelayCommand(ShowOpenUserDialog);
            CloseUserCommand = new RelayCommand(CloseCurrentUser, CanCloseCurrentUser);
            ShowUsersConfigDialogCommand = new RelayCommand(ShowUsersConfigDialog, CanOpenUsersConfig);

            OpenUserCommand = new RelayCommand(OpenUser, o => SelectedOpenUserDialog != null);

            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser, o => SelectedUsersConfigDialog != null);
            EditUserCommand = new RelayCommand(ShowEditUserDialog, o => o != null);

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

            SaveConfig(ConfigConsts.ConfigPath);
        }

        private void ExportConfig()
        {
            bool? openResult = _saveDialog.ShowDialog();
            if (openResult == null || !openResult.Value)
                return;

            string path = _saveDialog.FileNames[0];

            try
            {
                SaveConfig(path);
                MessageBox.Show("Config exported", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    string.Format("Can't save file {0}", path),
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            SaveConfig(path);
        }

        private void SaveConfig(string path)
        {
            XmlDocument doc = _model.SaveUsersToXml();
            doc.Save(path);
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

                SelectedUserMain = SelectedOpenUserDialog;
            }
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
            var u = new UserViewModel(new UserModel("", 0));

            var dialog = new AddUserDialog(u);
            bool? result = dialog.ShowDialog();

            if (!((result == null) || (!result.Value)))
            {
                UsersList.Add(u);
                _model.UsersList.Add(u.GetModel());
            }
        }

        private void RemoveUser(object args)
        {
            if (SelectedUsersConfigDialog != null)
            {
                var answ = MessageBox.Show("Do you really want to remove selected user?", "Remove user",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (answ == MessageBoxResult.Yes)
                {
                    CloseUser(SelectedUsersConfigDialog);
                    var userToRemove = SelectedUsersConfigDialog as UserViewModel;

                    UsersList.Remove(userToRemove);
                    _model.RemoveUser(userToRemove?.GetModel());
                    SelectedUsersConfigDialog = null;
                }
            }
        }

        private void ShowEditUserDialog(object obj)
        {
            if (obj != null)
            {
                var dialog = new EditUserDialog(obj as UserViewModel);
                dialog.ShowDialog();
            }
        }
    }
}
