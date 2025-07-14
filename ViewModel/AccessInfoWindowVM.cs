using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel
{
    public class AccessInfoWindowVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region View Model Properties

        public bool IsExpanded => true;

        private ObservableCollection<TreeItem> _treeItems;
        public ObservableCollection<TreeItem> TreeItems
        {
            get => _treeItems;
            set
            {
                _treeItems = value;
                OnPropertyChanged();
            }
        }
        #endregion

        private readonly IObjectsRepository _objectsRepository;

        public AccessInfoWindowVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _currentWindow = WindowHelper.CreateWindowWithUserControl<AccessInfoWindow>();

        }

        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);
        public ICommand OpenDialogCommand => new RelayCommand<object>(_ => OpenWindow());

        private void OnTabSelected(TreeItem selectedTab)
        {

        }
        private readonly DialogWindow _currentWindow;

        private void OpenWindow()
        {
            LoadAccessTree();
            _currentWindow.Show();
        }

        private void LoadAccessTree()
        {

            var root = _objectsRepository.GetOrganisationUnits();
            var La = new List<TreeItem>();


        }
    }
}