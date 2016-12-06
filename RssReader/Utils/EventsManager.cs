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

        // TreeView Events

        private static TreeView _fileTreeView = null;
        public static void ProvideFileTreeViewToSubscribeEvents(TreeView fileTreeView)
        {
            if (Equals(_fileTreeView, fileTreeView))
                return;

            _fileTreeView = fileTreeView;

            if (_fileTreeViewSelectedItemChangedHandler != null)
            {
                _fileTreeView.SelectedItemChanged += _fileTreeViewSelectedItemChangedHandler;
                _fileTreeViewSelectedItemChangedHandler = null;
            }

            if (_fileTreeViewMouseDoubleClick != null)
            {
                _fileTreeView.MouseDoubleClick += _fileTreeViewMouseDoubleClick;
                _fileTreeViewMouseDoubleClick = null;
            }
        }
        
        private static RoutedPropertyChangedEventHandler<object> _fileTreeViewSelectedItemChangedHandler = null;
        public static void SubscribeFileTreeViewSelectedItemChangedEvent(RoutedPropertyChangedEventHandler<object> selectedItemChangedEventHandler)
        {
            if (_fileTreeView == null)
            {
                _fileTreeViewSelectedItemChangedHandler = selectedItemChangedEventHandler;
            }
            else
            {
                _fileTreeView.SelectedItemChanged += selectedItemChangedEventHandler;
                _fileTreeViewSelectedItemChangedHandler = null;
            }
        }

        private static MouseButtonEventHandler _fileTreeViewMouseDoubleClick = null;
        public static void SubscribeFileTreeViewMouseDoubleClick(MouseButtonEventHandler mouseButtonEventHandler)
        {
            if (_fileTreeView == null)
            {
                _fileTreeViewMouseDoubleClick = mouseButtonEventHandler;
            }
            else
            {
                _fileTreeView.MouseDoubleClick += mouseButtonEventHandler;
                _fileTreeViewMouseDoubleClick = null;
            }
        }

        // Dialog

        public static void ShowAddDialog()
        {
            throw new NotImplementedException();
        }
        
    }
}
