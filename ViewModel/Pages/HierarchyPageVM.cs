using Ascon.Pilot.SDK;
using Ascon.Pilot.Theme.Controls;
using MyIceLibrary.Command;
using MyIceLibrary.Helper;
using MyIceLibrary.Model;
using MyIceLibrary.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MyIceLibrary.ViewModel.Pages
{
    public class HierarchyPageVM : INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region View Model Properties

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

        private ObservableCollection<CurrentObjectInfo> _selectedObjectInfo;
        public ObservableCollection<CurrentObjectInfo> SelectedObjectInfo
        {
            get => _selectedObjectInfo;
            set
            {
                _selectedObjectInfo = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public ICommand SelectedElementCommand => new RelayCommand<TreeItem>(OnTabSelected);
        public ICommand LoadHierarchyCommand => new RelayCommand<object>(_ => LoadAccessTree());
        public ICommand FilteredBoxExecuteEnterCommand => new RelayCommand<string>(FilteredBoxExecuteEnter);

        private readonly IObjectsRepository _objectsRepository;
        private readonly ObjectsTreeBuilder _treeBuilder;

        private ObservableCollection<TreeItem> _originalTree;

        public HierarchyPageVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _treeBuilder = new ObjectsTreeBuilder(objectsRepository);
        }

        private void LoadAccessTree()
        {
            _originalTree = _treeBuilder.CreateOrganisationUnitTreeTopToButtomAsync();
            TreeItems = new ObservableCollection<TreeItem>(_originalTree);
        }

        private async void FilteredBoxExecuteEnter(string parameter)
        {
            TreeItems = await _treeBuilder.FilteredTreeItemsAsync(parameter, _originalTree);
        }

        private void OnTabSelected(TreeItem selectedTab)
        {

            IOrganisationUnit unit = selectedTab.DataObject as IOrganisationUnit;
            if (unit.Person() != -1)
            {
                SelectedObjectInfo = GetPersonInfo(unit);
            }
            else
            {
                SelectedObjectInfo = GetUnitInfo(unit);
            }
        }

        private ObservableCollection<CurrentObjectInfo> GetPersonInfo(IOrganisationUnit unit)
        {
            IPerson person = _objectsRepository.GetPerson(unit.Person());

            List<CurrentObjectInfo> info = new List<CurrentObjectInfo>()
            {
                new CurrentObjectInfo() { Name = "Id", Value = person.Id },
                new CurrentObjectInfo() { Name = "Login", Value = person.Login },
                new CurrentObjectInfo() { Name = "DisplayName", Value = person.DisplayName },
                new CurrentObjectInfo() { Name = "Comment", Value = person.Comment },
                new CurrentObjectInfo() { Name = "ServiceInfo", Value = person.ServiceInfo },
                new CurrentObjectInfo() { Name = "Sid", Value = person.Sid },
                new CurrentObjectInfo() { Name = "IsAdmin", Value = person.IsAdmin },
                new CurrentObjectInfo() { Name = "CreatedUtc", Value = person.CreatedUtc },

            };

            return new ObservableCollection<CurrentObjectInfo>(info);
        }

        private ObservableCollection<CurrentObjectInfo> GetUnitInfo(IOrganisationUnit unit)
        {
            List<CurrentObjectInfo> info = new List<CurrentObjectInfo>()
            {
                new CurrentObjectInfo() { Name = "Id", Value = unit.Id },
                new CurrentObjectInfo() { Name = "Title", Value = unit.Title },
                new CurrentObjectInfo() { Name = "Kind", Value = unit.Kind() },
                new CurrentObjectInfo() { Name = "IsDeleted", Value = unit.IsDeleted },
            };

            return new ObservableCollection<CurrentObjectInfo>(info);
        }
    }
}
