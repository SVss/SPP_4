﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public object SelectedUserMain { get; set; }
        public ObservableCollection<UserViewModel> OpenedUsersList { get; } =
            new ObservableCollection<UserViewModel>();

        public object SelectedOpenUserDialog { get; set; }
        public object SelectedUsersConfigDialog { get; set; }
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
            CloseUserCommand = new RelayCommand(CloseCurrentUser, o => SelectedUserMain != null);
            ShowUsersConfigDialogCommand = new RelayCommand(ShowUsersConfigDialog, CanOpenUsersConfig);

            OpenUserCommand = new RelayCommand(OpenUser, o => SelectedOpenUserDialog != null);

            AddUserCommand = new RelayCommand(AddUser);
            RemoveUserCommand = new RelayCommand(RemoveUser);
            EditUserCommand = new RelayCommand(ShowEditUserDialog);

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
                user.EndUpdating();
            }

            XmlDocument doc = _model.SaveUsersToXml();
            doc.Save(ConfigConsts.ConfigPath);
        }

        private void CloseApplication(object args)
        {
            Application.Current.Shutdown();
        }

        private void CloseCurrentUser(object obj)
        {
            MessageBox.Show("Close current user");
        }

        private void ShowOpenUserDialog(object obj)
        {
            var dialog = new OpenUserDialog(this);
            dialog.ShowDialog();
        }


        private void OpenUser(object o)
        {
            OpenedUsersList.Add(SelectedOpenUserDialog as UserViewModel);
            SelectedUserMain = SelectedOpenUserDialog;
            OnPropertyChanged("SelectedUserMain");
        }


        private void ShowUsersConfigDialog(object obj)
        {
            MessageBox.Show("Users config dialog.");
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


        // Users Config Dialog

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

        private void ShowEditUserDialog(object obj)
        {
            // TODO
            MessageBox.Show("Edit user.");
        }
    }
}
