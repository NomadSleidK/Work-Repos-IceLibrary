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

        private ObservableCollection<AccessTreeItem> _treeItems;
        public ObservableCollection<AccessTreeItem> TreeItems
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

        public ICommand SelectedElementCommand => new RelayCommand<AccessTreeItem>(OnTabSelected);
        public ICommand OpenDialogCommand => new RelayCommand<object>(_ => OpenWindow());

        private readonly IObjectsRepository _objectsRepository;
        private readonly DialogWindow _currentWindow;

        public AccessInfoWindowVM(IObjectsRepository objectsRepository)
        {
            _objectsRepository = objectsRepository;
            _currentWindow = WindowHelper.CreateWindowWithUserControl<AccessInfoWindow>();
            _currentWindow.DataContext = this;
        }

        private void OpenWindow()
        {
            LoadAccessTree();
            _currentWindow.Show();
        }

        private void LoadAccessTree()
        {
            var tree = BuildTree();
            TreeItems = new ObservableCollection<AccessTreeItem>(tree);
        }

        public List<AccessTreeItem> BuildTree()
        {
            //var child = new List<TreeItem>() { new TreeItem() { Name = "R1r", Children = null } };
            //var tree = new List<TreeItem>()
            //{
            //    new TreeItem() { Name = "A1", Children = child },
            //    new TreeItem() { Name = "A2", Children = child },
            //    new TreeItem() { Name = "A3", Children = child },           
            //};

            var unitsId = _objectsRepository.GetOrganisationUnit(0).Children;
            var units = new List<IOrganisationUnit>();

            foreach (var id in unitsId)
            {
                units.Add(_objectsRepository.GetOrganisationUnit(id));
            }

            var tree = new List<AccessTreeItem>();

            foreach (var unit in units)
            {
                tree.Add(BuildTreeItem(unit));
            }

            return tree;
        }

        private AccessTreeItem BuildTreeItem(IOrganisationUnit unit)
        {
            var item = new AccessTreeItem
            {
                Name = unit.Title,
                IsExpanded = true,
                Children = new List<AccessTreeItem>(),
                Unit = unit,
            };
          
            if (unit.Kind() == OrganizationUnitKind.Position)
            {
                item.Children.Add(new AccessTreeItem() { Name = _objectsRepository.GetPerson(unit.Person()).DisplayName,
                    IsExpanded = true, Unit = unit,  Children = null });
            }

            foreach (var childId in unit.Children)
            {
                var childUnit = _objectsRepository.GetOrganisationUnit(childId);
                if (childUnit != null)
                {
                    item.Children.Add(BuildTreeItem(childUnit));
                }
            }

            return item;
        } 
        
        private void OnTabSelected(AccessTreeItem selectedTab)
        {
            if (selectedTab.Unit.Person() != -1)
            {
                SelectedObjectInfo = GetPersonInfo(selectedTab.Unit);
            }
            else
            {
                SelectedObjectInfo = GetUnitInfo(selectedTab.Unit);
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