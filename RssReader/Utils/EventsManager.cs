using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RssReader.Utils
{
    public static class EventsManager
    {
        // Window Events

        private static Window _mainWindow = null;
        public static void SetMainWindow(Window mainWindow)
        {
            if (Equals(_mainWindow, mainWindow))
                return;

            _mainWindow = mainWindow;

            if (_windowCancelEventHandler != null)
            {
                _mainWindow.Closing += _windowCancelEventHandler;
                _windowCancelEventHandler = null;
            }
        }

        private static CancelEventHandler _windowCancelEventHandler = null;
        public static void SetMainWindowCancelEventHandler(CancelEventHandler windowCancelEventHandler)
        {
            if (_mainWindow == null)
            {
                _windowCancelEventHandler = windowCancelEventHandler;
            }
            else
            {
                _mainWindow.Closing += windowCancelEventHandler;
                _windowCancelEventHandler = null;
            }
        }
        
        // Dialogs

        public static void ShowAddDialog()
        {
            throw new NotImplementedException();
        }
        
    }
}
