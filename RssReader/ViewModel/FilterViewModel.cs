using System;
using System.Collections.ObjectModel;
using System.Windows;
using RssReader.Model;
using RssReader.Utils;
using RssReader.View.Dialogs;

namespace RssReader.ViewModel
{
    public class FilterViewModel: BaseViewModel
    {
        private FilterModel _model;

        public FilterType Type => _model.Type;
        public ObservableCollection<string> AndList { get; }
        public ObservableCollection<string> OrList { get; }

        public object SelectedAndFilter { get; set; }
        public object SelectedOrFilter { get; set; }

        // Commands
        public RelayCommand AddAndFilterCommand { get; }
        public RelayCommand RemoveAndFilterCommand { get; }

        public RelayCommand AddOrFilterCommand { get; }
        public RelayCommand RemoveOrFilterCommand { get; }

        // Public

        public FilterViewModel(FilterModel filter)
        {
            _model = filter;

            AndList = _model.AndList;
            OrList = _model.OrList;

            AddAndFilterCommand = new RelayCommand(AddFilter);
            RemoveAndFilterCommand = new RelayCommand(RemoveFilter, o => SelectedAndFilter != null);

            AddOrFilterCommand = new RelayCommand(AddFilter);
            RemoveOrFilterCommand = new RelayCommand(RemoveFilter, o => SelectedOrFilter != null);
        }

        // Internals

        private void AddFilter(object o)
        {
            var l = o as ObservableCollection<string>;
            var s = new StringContainer();

            var dialog = new AddFilterDialog(s);
            bool? res = dialog.ShowDialog();

            if (!(res == null || !res.Value))
            {
                if (l != null && !(l.Contains(s.Value)))
                {
                    l.Add(s.Value);
                }
            }
        }

        private void RemoveFilter(object obj)
        {
            var l = obj  as ObservableCollection<string>;
            string filterToRemove = string.Empty;
            if (l == AndList)
            {
                filterToRemove = SelectedAndFilter as string;
            }
            else if (l == OrList)
            {
                filterToRemove = SelectedOrFilter as string;
            }

            if (!string.IsNullOrEmpty(filterToRemove))
            {
                l?.Remove(filterToRemove);
            }
        }
    }
}
